using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D ref_self_rbody = null;
    [SerializeField] private CircleCollider2D ref_bullet_spawn = null;

    [SerializeField] private GameObject prefab_bullet = null;

    private float move_x = 0f;
    private float move_y = 0f;
    private float last_shot_time = 0f;

    private void Update()
    {
        move_x = Input.GetAxis("Horizontal");
        move_y = Input.GetAxis("Vertical");

        // Orient self towards mouse
        Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(this.transform.position);
        float dir_angle_deg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(dir_angle_deg, Vector3.forward);

        // Shooting
        float e_time = Time.time - last_shot_time;
        if (Input.GetButton("Fire1") && e_time > Manager_Main.Instance.PLAYER_BULLET_CD)
        {
            float bullet_dev = ref_bullet_spawn.radius;
            Vector2 bullet_offset = new Vector2(Random.Range(-1f, 1f) * bullet_dev, Random.Range(-1f, 1f) * bullet_dev);
            Behavior_Bullet script_bullet = Instantiate(prefab_bullet, (Vector2)ref_bullet_spawn.transform.position + bullet_offset, this.transform.rotation).GetComponent<Behavior_Bullet>();
            script_bullet.Set_Vel(script_bullet.transform.right * Manager_Main.Instance.PLAYER_BULLET_SPEED);
            last_shot_time = Time.time;
        }
    }

    private void FixedUpdate()
    {
        ref_self_rbody.AddForce(new Vector2(move_x, move_y) * Manager_Main.Instance.PLAYER_MOVE_SPEED * Time.fixedDeltaTime);
    }
}
