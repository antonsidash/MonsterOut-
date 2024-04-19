using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using MonsterOutLibrary;

public class SpawnPlayers : MonoBehaviour
{
    // Будет убрано:
    public Vector3 SurvivorsSpawnPosition;

    public Transform MonstersSpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        SurvivorsSpawnPosition = transform.position;
        //PhotonNetwork.Instantiate(Player.name, SpawnPosition, Quaternion.identity);

        // Новый код, с Петром:
        //PhotonNetwork.Instantiate(PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString(), SurvivorsSpawnPosition, Quaternion.identity);
        if (PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() == "Survivors")
            PhotonNetwork.Instantiate(PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString(), SurvivorsSpawnPosition, Quaternion.identity);
        else
            PhotonNetwork.Instantiate(PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString(), MonstersSpawnPosition.position, Quaternion.identity);

        Debug.Log("Spawned player with Character: " + PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString());
        Debug.Log("Spawned player with Team: " + PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString());
    }
}
