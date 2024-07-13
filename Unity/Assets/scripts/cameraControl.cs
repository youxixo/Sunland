using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{

    public Transform player;

    // Update is called once per frame
    void Update()
    {
        if(player.position.x > -39.08861 && player.position.x < 168.963)
        {
            transform.position = new Vector3(player.position.x, player.position.y, -10f);
        }
    }
}
