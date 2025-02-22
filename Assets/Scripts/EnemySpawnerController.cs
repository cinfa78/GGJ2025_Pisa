using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyType{
    Side = 0,
    Bottom = 1
}

public class EnemySpawnerController : MonoBehaviour{
    public DevilController genericDevil;
    public EnemyController[] sideDevils;
    public Transform[] sideSpawnPoints;

    public GameObject[] bottomDevils;
    public Transform[] bottomSpawnPoints;

    public Boss boss;
    public Transform bossSpawnPoint;
    private List<EnemyController> _availableSideDevils = new List<EnemyController>();
    private List<string> _spawnedDevils = new List<string>();

    private void OnDrawGizmos(){
        foreach (var ssp in sideSpawnPoints){
            Gizmos.DrawWireSphere(ssp.position, .5f);
        }
    }

    private void Start(){
        Debug.Log(SaveManager.Instance);
        foreach (var sideDevil in sideDevils){
            if (!SaveManager.Instance.GetSavedData.ContainsDemon(sideDevil.gameObject.name)){
                _availableSideDevils.Add(sideDevil);
            }
        }
    }

    public void SpawnEnemy(int enemyType = -1){
        if (enemyType == -1)
            enemyType = Random.Range(0, System.Enum.GetValues(typeof(EnemyType)).Length);

        switch (enemyType){
            case 0:{
                if (_availableSideDevils.Count > 0){
                    int chosenDevilIndex = Random.Range(0, _availableSideDevils.Count);
                    var newDevil = Instantiate(_availableSideDevils[chosenDevilIndex], sideSpawnPoints[Random.Range(0, sideSpawnPoints.Length)].position, Quaternion.identity);
                    newDevil.name = _availableSideDevils[chosenDevilIndex].gameObject.name;
                    _spawnedDevils.Add(_availableSideDevils[chosenDevilIndex].gameObject.name);
                    _availableSideDevils.RemoveAt(chosenDevilIndex);
                }
                else{
                    InstantiateGenericDevil();
                }

                break;
            }
            case 2: Instantiate(bottomDevils[Random.Range(0, bottomDevils.Length)], bottomSpawnPoints[Random.Range(0, bottomSpawnPoints.Length)].position, Quaternion.identity); break;
        }
    }

    private void InstantiateGenericDevil(){
        Instantiate(genericDevil, sideSpawnPoints[Random.Range(0, sideSpawnPoints.Length)].position, Quaternion.identity);
    }

    public void SpawnBoss(){
        var newBoss = Instantiate(boss, bossSpawnPoint.position, Quaternion.identity);
        Boss.OnBossDeath += OnBossDeath;
    }

    private void OnBossDeath(){
        Boss.OnBossDeath -= OnBossDeath;
        //salvo i demoni uccisi
        SaveManager.Instance.AddKilledDemons(_spawnedDevils);
    }

    private void OnDestroy(){
        Boss.OnBossDeath -= OnBossDeath;
    }
}