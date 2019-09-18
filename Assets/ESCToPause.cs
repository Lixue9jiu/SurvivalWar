using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESCToPause : MonoBehaviour
{
    public PlayerController player;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            player.enabled = !player.enabled;
        }
    }
}
