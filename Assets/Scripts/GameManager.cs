using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class GameManager : MonoBehaviour
{
    public Transform spawn1, spawn2;
    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player1", spawn1.position, spawn1.rotation);
        }
        else
        {
            PhotonNetwork.Instantiate("Player2", spawn2.position, spawn2.rotation);
        }
    }
}
