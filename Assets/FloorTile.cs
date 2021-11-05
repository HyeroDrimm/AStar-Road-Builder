using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    public Transform spawnPoint;
    public int x;
    public int z;

    public void SpawnRoadTile(GameObject roadPrefab, int rotation)
    {
        if (spawnPoint.childCount != 0)
        {
            Destroy(spawnPoint.GetChild(0).gameObject);
        }
        Instantiate(roadPrefab, spawnPoint.position, Quaternion.Euler(0, rotation * 90, 0), spawnPoint);
    }
}
