using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float enemyDesity = 0.02f;

    private List<Vector3> enemySpawns = new List<Vector3>();

    public void SpawnEnemies(Room[,] rooms, Vector3 playerStartPos)
    {
        if (enemyPrefab == null || rooms == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        List<Room> availableRooms = new List<Room>();
        int mazeWidth = rooms.GetLength(0);
        int mazeHeight = rooms.GetLength(1);
        int totalRooms = mazeWidth * mazeHeight;

        foreach (Room room in rooms)
        {
            if (room != null && Vector3.Distance(room.transform.position, playerStartPos) > 1f)
            {
                availableRooms.Add(room);
            }
        }

        int numOfEnemies = Mathf.Clamp(Mathf.RoundToInt(totalRooms * enemyDesity), 1, 20);

        for (int i = 0; i < numOfEnemies && availableRooms.Count > 0; i++)
        {
            int index = Random.Range(0, availableRooms.Count);
            Room selectedRoom = availableRooms[index];
            Vector3 spawnPos = selectedRoom.transform.position;
            enemySpawns.Add(spawnPos);

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            EnemyAI enemyAi = enemy.GetComponent<EnemyAI>();
            if (enemyAi != null )
            {
                enemyAi.SetTarget(player.transform);
            }

            availableRooms.RemoveAt(index);
        }
    }
}
