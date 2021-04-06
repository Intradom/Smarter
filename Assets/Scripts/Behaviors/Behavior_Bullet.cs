using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D ref_self_rbody = null;

    [SerializeField] private string[] tags_destroy = null;

    public void Set_Vel(Vector2 vel)
    {
        ref_self_rbody.velocity = vel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in tags_destroy)
        {
            if (collision.tag == tag)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
