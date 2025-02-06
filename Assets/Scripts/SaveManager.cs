using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;


public class SaveManager : MonoBehaviour{
    public DemonListSo demonListSo;
    public static int PopeNumber => PlayerPrefs.GetInt("popes", 1);
    public static event Action<int> OnNewPope;

    public static void IncrementPope(){
        int nextPope = PlayerPrefs.GetInt("popes", 1) + 1;
        PlayerPrefs.SetInt("popes", nextPope);
        OnNewPope?.Invoke(nextPope);


        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        Debug.Log("Documents Path: " + documentsPath);

        // Example: Reading files in the Documents folder
        if (Directory.Exists(documentsPath)){
            string[] files = Directory.GetFiles(documentsPath);
            foreach (string file in files){
                Debug.Log("File: " + file);
            }
        }
    }

    [Button("Fill Names")]
    public void FillNames(){
        var uniqueDemonNames = new List<string>();
        TextAsset mytxtData = (TextAsset)Resources.Load("demon_names");
        var text = mytxtData.text;
        string[] lines = text.Split('\n');
        Dictionary<string, string> _demonNames = new Dictionary<string, string>();
        foreach (var line in lines){
            var l = line.Trim().ToLower();

            if (!_demonNames.ContainsKey(l))
                _demonNames.Add(l, l);
        }

        demonListSo.names = new List<string>();
        int i = 0;
        foreach (var _d in _demonNames){
            uniqueDemonNames.Add(_d.Key);
            demonListSo.names.Add(_d.Key);
            i++;
        }
    }

    public static List<string> GetDemonNames(int amount = 3){
        var uniqueDemonNames = new List<string>();
        TextAsset mytxtData = (TextAsset)Resources.Load("demon_names");
        var text = mytxtData.text;
        string[] lines = text.Split('\n');
        Dictionary<string, string> _demonNames = new Dictionary<string, string>();
        foreach (var line in lines){
            var l = line.Trim().ToLower();

            if (!_demonNames.ContainsKey(l))
                _demonNames.Add(l, l);
        }

        foreach (var _d in _demonNames){
            uniqueDemonNames.Add(_d.Key);
        }

        //Debug.Log($"Demons found: {uniqueDemonNames.Count} looking for {amount} demons");

        var result = new List<string>();
        for (int i = 0; i < Mathf.Min(amount, uniqueDemonNames.Count); i++){
            int dn = Random.Range(0, uniqueDemonNames.Count);
            //Debug.Log($"Adding {uniqueDemonNames[dn]}");
            result.Add(uniqueDemonNames[dn]);
            uniqueDemonNames.RemoveAt(dn);
        }

        return result;
    }
}