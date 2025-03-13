using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private float collectibleDensity = 0.5f;


    public void SpawnCollectibles(Room[,] rooms)
    {
        if (collectiblePrefab == null || rooms == null) return;

        List<Room> avaibleRooms = new List<Room>();
        int mazeWidth = rooms.GetLength(0);
        int mazeHeight = rooms.GetLength(1);
        int totalRooms = mazeWidth * mazeHeight;

        //Get valid rooms to spawn collectibles
        foreach (Room room in rooms)
        {
            if (room != null)
            {
                avaibleRooms.Add(room);
            }
        }

        int numOfCollectibles = Mathf.Clamp(Mathf.RoundToInt(totalRooms * collectibleDensity), 1, 250);

        for (int i = 0; i < numOfCollectibles && avaibleRooms.Count > 0; i++)
        {
            int index = Random.Range(0, avaibleRooms.Count);
            Room selectedRoom = avaibleRooms[index];
            Vector3 spawnPos = selectedRoom.transform.position;

            Instantiate(collectiblePrefab, spawnPos, Quaternion.identity);
            GameManager.instance.RegisteredCollectible();
            avaibleRooms.RemoveAt(index);
        }
    }
}
