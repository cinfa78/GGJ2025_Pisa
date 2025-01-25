using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
	Shot = 0,
	Spike = 1
}

public class EnemySpawnerController : MonoBehaviour {
	public float minY;
	public float maxY;
	public GameObject[] devils;

	public void SpawnEnemy(int enemyType = -1) {
		if (enemyType == -1)
			Instantiate(devils[Random.Range(0, devils.Length)]);
		else {
			Instantiate(devils[enemyType]);
		}
	}
}