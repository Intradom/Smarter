using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D ref_self_rbody = null;

    private float move_x = 0f;
    private float move_y = 0f;

    private void Update()
    {
        move_x = Input.GetAxis("Horizontal");
        move_y = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        ref_self_rbody.AddForce(new Vector2(move_x, move_y) * Manager_Main.Instance.PLAYER_MOVE_SPEED * Time.fixedDeltaTime);
    }
}
