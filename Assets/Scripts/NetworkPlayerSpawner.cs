using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;
    public GameObject ballPrefab1;
    public GameObject ballPrefab2;
    public GameObject ballPrefab3;

    private Vector3 ballSpawnPosition1;
    private Vector3 ballSpawnPosition2;
    private Vector3 ballSpawnPosition3;

    private void Start()
    {
        ballSpawnPosition1 = ballPrefab1.transform.position;
        ballSpawnPosition2 = ballPrefab2.transform.position;
        ballSpawnPosition3 = ballPrefab3.transform.position;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // NetworkPlayer_stick
        // NetworkPlayer_female
        // NetworkPlayer_male
        // NetworkPlayer_gray

        spawnedPlayerPrefab = PhotonNetwork.Instantiate("root_corrected_1", transform.position, transform.rotation);

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnBalls();
        }
    }

    private void SpawnBalls()
    {
        PhotonNetwork.Instantiate("ball1", ballSpawnPosition1, Quaternion.identity);
        PhotonNetwork.Instantiate("ball2", ballSpawnPosition2, Quaternion.identity);
        PhotonNetwork.Instantiate("ball3", ballSpawnPosition3, Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (spawnedPlayerPrefab != null)
        {
            PhotonNetwork.Destroy(spawnedPlayerPrefab);
        }
    }
}
