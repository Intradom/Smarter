using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Main : MonoBehaviour
{
    public static Manager_Main Instance = null;

    /* Parameters */

    // Player
    public float PLAYER_MOVE_SPEED = 0f;
    public float PLAYER_BULLET_SPEED = 0f;
    public float PLAYER_BULLET_CD_SEC = 0.1f;

    // Dot

    // Misc

    private void Awake()
    {
        Instance = this;
    }
}
