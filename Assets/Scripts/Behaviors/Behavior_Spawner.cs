using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab_dot = null;

    private float last_spawn_time = 0f;

    private void Update()
    {
        float e_time = Time.time - last_spawn_time;
        if (e_time > Manager_Main.Instance.SPAWNER_SPAWN_CD && GameObject.FindGameObjectsWithTag(prefab_dot.tag).Length < Manager_Main.Instance.MISC_MAX_DOTS)
        {
            Behavior_Dot script_dot = Instantiate(prefab_dot, this.transform.position, Quaternion.identity).GetComponent<Behavior_Dot>();
            if (script_dot)
            {
                script_dot.Init();
            }

            last_spawn_time = Time.time;
        }
    }
}
