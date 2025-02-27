using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField] GameObject collectiblePrefab;
    [SerializeField] int collectibleCount = 25;
    private Room[,] rooms;

    public void SpawnCollectibles(Room[,] generatedRooms)
    {
        rooms = generatedRooms;
        List<Room> avaibleRooms = new List<Room>();

        //Get valid rooms to spawn collectibles
        foreach (Room room in rooms)
        {
            if (room != null)
            {
                avaibleRooms.Add(room);
            }
        }

        //Shuffle rooms to randomize selection
        for (int i = 0; i < avaibleRooms.Count; i++)
        {
            int randIndex = Random.Range(i, avaibleRooms.Count);
            Room temp = avaibleRooms[i];
            avaibleRooms[i] = avaibleRooms[randIndex];
            avaibleRooms[randIndex] = temp;
        }

        //Spawn collectibles in the first few rooms from the shuffled list
        for (int i = 0; i < Mathf.Min(collectibleCount, avaibleRooms.Count); i++)
        {
            Vector3 spawnPos = avaibleRooms[i].transform.position;
            Instantiate(collectiblePrefab, spawnPos, Quaternion.identity);
        }
    }
}
