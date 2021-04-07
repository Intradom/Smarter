using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Dot : MonoBehaviour
{
    public struct NN_Node
    {
        public List<float> weights;
        public float bias;
    }

    public struct Dot_Data
    {
        public List<List<NN_Node>> NN_weights;
        public Color rend_color;
        public float move_speed;
        public float shoot_cd_secs;
        public float dup_cd_secs;
        public bool can_shoot;
    }

    private enum NN_Action
    {
        Move_Speed, // Percentage of "move_speed"
        Move_Direction,
        Shoot,
        Shoot_Direction,
        size
    }

    [SerializeField] private Rigidbody2D ref_self_rbody = null;
    [SerializeField] private SpriteRenderer ref_self_sprite_rend = null;

    [SerializeField] private GameObject prefab_dot = null;
    [SerializeField] private GameObject prefab_bullet = null;

    [SerializeField] private string layer_mask_interactable = "";
    [SerializeField] private float offset = 0f;

    // NN Params
    private int NN_input_length = 0;
    private int NN_hidden_layers = 0;
    private int NN_hidden_layer_length = 0;

    // Misc
    private Dot_Data data;
    private float last_shot_time = 0f;
    private float last_dup_time = 0f;

    // Init randomly
    public void Init()
    {
        // Weight data
        // Input layer to hidden layer
        data.NN_weights = new List<List<NN_Node>>();
        List<NN_Node> i2h_layer_list = new List<NN_Node>();
        for (int i = 0; i < NN_hidden_layer_length; ++i)
        {
            i2h_layer_list.Add(RandomNode(NN_input_length));
        }
        data.NN_weights.Add(i2h_layer_list);

        // Hidden layer to hidden layer
        for (int i = 0; i < NN_hidden_layers - 1; ++i) // Only necessary if h_layers > 1
        {
            List<NN_Node> h2h_layer_list = new List<NN_Node>();
            for (int j = 0; j < NN_hidden_layer_length; ++j)
            {
                h2h_layer_list.Add(RandomNode(NN_hidden_layer_length));
            }
            data.NN_weights.Add(h2h_layer_list);
        }

        // Hidden layer to output layer
        List<NN_Node> h2o_layer_list = new List<NN_Node>();
        for (int i = 0; i < (int)NN_Action.size; ++i)
        {
            h2o_layer_list.Add(RandomNode(NN_hidden_layer_length));
        }
        data.NN_weights.Add(h2o_layer_list);

        // Misc data
        data.move_speed = Random.Range(Manager_Main.Instance.DOT_MOVE_SPEED_MIN, Manager_Main.Instance.DOT_MOVE_SPEED_MAX);
        data.shoot_cd_secs = Random.Range(Manager_Main.Instance.DOT_BULLET_CD_MIN, Manager_Main.Instance.DOT_BULLET_CD_MAX);
        data.dup_cd_secs = Random.Range(Manager_Main.Instance.DOT_DUP_CD_MIN, Manager_Main.Instance.DOT_DUP_CD_MAX);
        data.can_shoot = (Random.Range(0, 2) == 0) ? false : true;

        // Sprite color
        Color new_c = new Color(Random.value, Random.value, Random.value, 1f);
        ref_self_sprite_rend.color = new_c;
        data.rend_color = new_c;
    }

    // Init with given parameters
    public void Init(Dot_Data dd)
    {
        data.NN_weights = new List<List<NN_Node>>(dd.NN_weights);
        data.move_speed = dd.move_speed;
        data.shoot_cd_secs = dd.shoot_cd_secs;
        data.dup_cd_secs = dd.dup_cd_secs;
        data.can_shoot = dd.can_shoot;
        data.rend_color = dd.rend_color;
        ref_self_sprite_rend.color = data.rend_color;
    }

    private float OutToAngle(float out_value)
    {
        return out_value * Mathf.PI * 2f;
    }

    private float ActivationFunc(float input)
    {
        return (float)System.Math.Tanh(input);
    }

    private float RandomWeight()
    {
        return Random.value * 2f - 1f;
    }

    private NN_Node RandomNode(int size)
    {
        NN_Node node = new NN_Node();

        List<float> rlist = new List<float>();
        for (int i = 0; i < size; ++i)
        {
            rlist.Add(RandomWeight());
        }
        node.weights = rlist;
        node.bias = RandomWeight();

        return node;
    }

    private List<float> GetNNInput()
    {
        List<float> ray_hit_dists = new List<float>(NN_input_length);
        for (int i = 0; i<NN_input_length; ++i)
        {
            float ray_dir = ((Mathf.PI * 2) / NN_input_length) * i;
            Vector2 dir_vec = new Vector2(Mathf.Cos(ray_dir), Mathf.Sin(ray_dir));

            RaycastHit2D hit = Physics2D.Raycast((Vector2)this.transform.position + dir_vec * offset, dir_vec, Mathf.Infinity, LayerMask.GetMask(layer_mask_interactable));
            ray_hit_dists.Add(hit.distance);

            if (hit.collider)
            {
                // Rays only show up in Scene view
                Debug.DrawRay((Vector2)this.transform.position + dir_vec* offset, dir_vec* hit.distance, Color.yellow);
            }
        }
        return ray_hit_dists;
    }

    private List<float> FF_Pass(List<float> input)
    {
        //Debug.Log("In: " + input.Count);
        //Debug.Log("Out: " + output.Count);
        // Make sure the input/output matches the NN dimensions
        if (input.Count != NN_input_length)
        {
            Debug.Log("FF_Pass input/output size mismatch");
            return null;
        }

        List<float> pass_container = new List<float>(new float[Mathf.Max(NN_hidden_layer_length, Mathf.Max(NN_input_length, (int)NN_Action.size))]);

        for (int i = 0; i < NN_input_length; ++i)
        {
            pass_container[i] = input[i];
        }

        // Network
        for (int i = 0; i < data.NN_weights.Count; ++i)
        {
            List<float> old_pass_container = new List<float>(pass_container); // Clones

            // Layer
            for (int j = 0; j < data.NN_weights[i].Count; ++j)
            {
                pass_container[j] = 0;
                // Node
                for (int k = 0; k < data.NN_weights[i][j].weights.Count; ++k)
                {
                    pass_container[j] += data.NN_weights[i][j].weights[k] * old_pass_container[k];
                }
                pass_container[j] += data.NN_weights[i][j].bias;
                pass_container[j] = ActivationFunc(pass_container[j]);
            }
        }

        return pass_container;
    }

    private void Start()
    {
        NN_input_length = Manager_Main.Instance.DOT_NUM_RAYS;
        NN_hidden_layers = Manager_Main.Instance.DOT_HIDDEN_LAYERS;
        NN_hidden_layer_length = Manager_Main.Instance.DOT_HIDDEN_LAYER_LENGTH;

        last_dup_time = Time.time;
    }

    private void Update()
    {
        /* Get input rays */
        List<float> ray_hit_dists = GetNNInput();
        
        /* FF NN Pass */
        List<float> NN_out = FF_Pass(ray_hit_dists);
        if (NN_out == null)
        {
            // Error
            return;
        }

        /* Actions */

        // Movement
        float move_angle = OutToAngle(NN_out[(int)NN_Action.Move_Direction]);
        float move_x = NN_out[(int)NN_Action.Move_Speed] * data.move_speed * Mathf.Cos(move_angle);
        float move_y = NN_out[(int)NN_Action.Move_Speed] * data.move_speed * Mathf.Sin(move_angle);
        ref_self_rbody.AddForce(new Vector2(move_x, move_y) * Time.deltaTime);

        // Shooting
        if (data.can_shoot && NN_out[(int)NN_Action.Move_Speed] > 0f) // Shoot desired
        {
            float e_time = Time.time - last_shot_time;
            if (e_time > data.shoot_cd_secs)
            {
                float shoot_angle = OutToAngle(NN_out[(int)NN_Action.Shoot_Direction]);
                Vector2 bullet_offset = offset * new Vector2(Mathf.Cos(shoot_angle), Mathf.Sin(shoot_angle));
                Behavior_Bullet script_bullet = Instantiate(prefab_bullet, (Vector2)this.transform.position + bullet_offset, Quaternion.AngleAxis(shoot_angle * Mathf.Rad2Deg, Vector3.forward)).GetComponent<Behavior_Bullet>();
                script_bullet.Set_Vel(script_bullet.transform.right * Manager_Main.Instance.DOT_BULLET_SPEED);

                last_shot_time = Time.time;
            }
        }

        /* Duplication */
        float e_time_dup = Time.time - last_dup_time;
        if (e_time_dup > data.dup_cd_secs && GameObject.FindGameObjectsWithTag(prefab_dot.tag).Length < Manager_Main.Instance.MISC_MAX_DOTS)
        {
            Behavior_Dot script_dot = Instantiate(prefab_dot, this.transform.position, Quaternion.identity).GetComponent<Behavior_Dot>();
            if (script_dot)
            {
                script_dot.Init(data);
            }

            last_dup_time = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Lava")
        {
            Destroy(this.gameObject);
        }
        else if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
