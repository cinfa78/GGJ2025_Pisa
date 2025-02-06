using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Demon Names", fileName = "DemonNames")]
public class DemonListSo : ScriptableObject{
    [Serializable]
    public struct DemonData{
        public string name;
        [ShowAssetPreview]public Sprite sprite;
        public Color wingsColor;
    }
    public List<DemonData> data;
    public List<string> names;
    private List<string> tempNames;
    
    public List<string> GetNames(int amount){
        tempNames = names;
        var result = new List<string>();
        for (int i = 0; i < Mathf.Min(amount, tempNames.Count); i++){
            int randPosition = Random.Range(0, tempNames.Count);
            result.Add(tempNames[randPosition]);
            tempNames.RemoveAt(randPosition);
        }

        return result;
    }
}