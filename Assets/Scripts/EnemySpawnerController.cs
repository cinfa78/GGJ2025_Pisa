using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyType{
    Side = 0,
    Bottom = 1
}

public class EnemySpawnerController : MonoBehaviour{
    [FormerlySerializedAs("devils")] public GameObject[] sideDevils;
    public Transform[] sideSpawnPoints;
    
    public GameObject[] bottomDevils;
    public Transform[] bottomSpawnPoints;
    
    public Boss boss;
    public Transform bossSpawnPoint;

    public void SpawnEnemy(int enemyType = -1){
        if (enemyType == -1)
            enemyType = Random.Range(0, System.Enum.GetValues(typeof(EnemyType)).Length);

        switch (enemyType){
            case 0: Instantiate(sideDevils[Random.Range(0, sideDevils.Length)], sideSpawnPoints[Random.Range(0, sideSpawnPoints.Length)].position, Quaternion.identity); break;
            case 2: Instantiate(bottomDevils[Random.Range(0, bottomDevils.Length)], bottomSpawnPoints[Random.Range(0, bottomSpawnPoints.Length)].position, Quaternion.identity); break;
        }
    }

    public void SpawnBoss(){
        var newBoss = Instantiate(boss, bossSpawnPoint.position, Quaternion.identity);
    }
}