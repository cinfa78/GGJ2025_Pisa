using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
	Shot = 0,
	Spike = 1,
	Bottom = 2
}

public class EnemySpawnerController : MonoBehaviour {
	public GameObject[] devils;
	public Transform[] sideSpawnPoints;
	public Transform[] bottomSpawnPoints;

	public void SpawnEnemy(int enemyType = -1) {
		if (enemyType == -1)
			enemyType = Random.Range(0, devils.Length);


		switch (enemyType) {
			case 0: Instantiate(devils[0], sideSpawnPoints[Random.Range(0, sideSpawnPoints.Length)].position, Quaternion.identity); break;
			case 1: Instantiate(devils[1], sideSpawnPoints[Random.Range(0, sideSpawnPoints.Length)].position, Quaternion.identity); break;
			case 2: Instantiate(devils[2], bottomSpawnPoints[Random.Range(0, bottomSpawnPoints.Length)].position, Quaternion.identity); break;
		}
	}
}