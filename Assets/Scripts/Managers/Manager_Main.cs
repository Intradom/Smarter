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
    public float PLAYER_BULLET_CD = 0f; // in seconds

    // Spawner
    public float SPAWNER_SPAWN_CD = 0f;

    // Dot
    public float DOT_MOVE_SPEED_MIN = 0f;
    public float DOT_MOVE_SPEED_MAX = 0f;
    public float DOT_BULLET_SPEED = 0f;
    public float DOT_BULLET_CD_MIN = 0f; // in seconds
    public float DOT_BULLET_CD_MAX = 0f; // in seconds
    public float DOT_DUP_CD_MIN = 0f;
    public float DOT_DUP_CD_MAX = 0f;
    public int DOT_NUM_RAYS = 0;
    public int DOT_HIDDEN_LAYERS = 0;
    public int DOT_HIDDEN_LAYER_LENGTH = 0;

    // Misc
    public int MISC_MAX_DOTS = 100;

    private void Awake()
    {
        Instance = this;
    }
}
