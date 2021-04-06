using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Main : MonoBehaviour
{
    public static Manager_Main Instance = null;

    /* Parameters */

    // Player
    public float PLAYER_MOVE_SPEED = 0f;

    // Dot

    // Misc

    private void Awake()
    {
        Instance = this;
    }
}
