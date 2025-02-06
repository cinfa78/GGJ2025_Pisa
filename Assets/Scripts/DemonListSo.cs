using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Demon Names", fileName = "DemonNames")]
public class DemonListSo : ScriptableObject{
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