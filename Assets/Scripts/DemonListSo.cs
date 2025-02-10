using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Demon Names", fileName = "DemonNames")]
public class DemonListSo : ScriptableObject{
    [Serializable]
    public struct DemonData{
        public string name;
        [ShowAssetPreview] public Sprite sprite;
        public Color wingsColor;
    }

    public List<DemonData> data;
    public List<string> names;
    private List<string> _tempNames;

    [Button("Sort")]
    public void SortNamesList(){
        names.Sort();
        data.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
    }
    [Button("Copy Data")]
    public void CopyNamesToData(){
        SortNamesList();
        foreach (var name in names){
            int i = 0;
            for (i = 0; i < data.Count; i++){
                if (data[i].name == name){
                    break;
                }
            }

            if (i == data.Count){
                
                var newDemonData = new DemonData();
                newDemonData.name = name;
                newDemonData.wingsColor = Color.green;

                data.Add(newDemonData);
            }
        }
    }

    public List<string> GetNames(int amount){
        _tempNames = new List<string>(names);
        var result = new List<string>();
        for (int i = 0; i < Mathf.Min(amount, _tempNames.Count); i++){
            int randPosition = Random.Range(0, _tempNames.Count);
            result.Add(_tempNames[randPosition]);
            _tempNames.RemoveAt(randPosition);
        }

        return result;
    }
}