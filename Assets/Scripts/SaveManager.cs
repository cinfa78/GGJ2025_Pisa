using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SaveData{
    public string[] KilledDemonsData;
    public int PopeNumber;
    public string[] PopeNames;

    public SaveData(string[] killedDemons, int popeNumber){
        KilledDemonsData = killedDemons;
        PopeNumber = popeNumber;
    }

    public bool Contains(string searchedDemon){
        for (int i = 0; i < KilledDemonsData.Length; i++){
            if (KilledDemonsData[i] == searchedDemon) return true;
        }

        return false;
    }
}

public class SaveManager : MonoBehaviour{
    public static SaveManager Instance;
    public DemonListSo demonListSo;
    public static int PopeNumber => PlayerPrefs.GetInt("popes", 1);
    public static event Action<int> OnNewPope;
    private SaveData _saveData;
    public SaveData GetSavedData => _saveData;
    private bool _initialized = false;
    public static List<string> freshlyUnlockedDemons = new List<string>();

    private void Awake(){
        if (Instance != null){
            Destroy(gameObject);
        }
        else{
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadData();
    }

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

    public void SaveData(){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/saveData.dat";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveData data = new SaveData(_saveData.KilledDemonsData, _saveData.PopeNumber);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public void LoadData(){
        Debug.Log($"Loading data");
        string path = Application.persistentDataPath + "/saveData.dat";

        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            _saveData = (SaveData)formatter.Deserialize(stream);
            stream.Close();
            Debug.Log($"Data Loaded {_saveData}");
        }
        else{
            ResetSaveData();
        }

        _initialized = true;
    }

    [Button("Reset SaveData")]
    private void ResetSaveData(){
        Debug.Log($"Created new data");
        _saveData = new SaveData(new string[0], 1);
        SaveData();
    }

    [Button("Print Save Data")]
    public void PrintSaveData(){
        Debug.Log($"Demons:");
        foreach (var dn in _saveData.KilledDemonsData){
            Debug.Log(dn);
        }

        Debug.Log($"Total Demons: {_saveData.KilledDemonsData.Length}");
        Debug.Log($"Popes: {_saveData.PopeNumber}");
    }

    public int GetPopeNumber(){
        if (_saveData == null){
            LoadData();
        }

        return _saveData.PopeNumber;
    }

    public void AddKilledDemons(List<string> killedDemons){
        var newKilledDemonsList = new List<string>(_saveData.KilledDemonsData);
        newKilledDemonsList.AddRange(killedDemons);
        _saveData.KilledDemonsData = newKilledDemonsList.ToArray();
        SaveData();
        freshlyUnlockedDemons = new List<string>(killedDemons);
    }

    public void AddDeadPope(){
        _saveData.PopeNumber++;
        SaveData();
    }

    public void JustUnlockedDemon(string demonName){
        freshlyUnlockedDemons.Remove(demonName);
    }

    public void AddPopeName(string popeName){
        var popeNames = new List<string>(_saveData.PopeNames);
        popeNames.Add(popeName);
        _saveData.PopeNames = popeNames.ToArray();
        SaveData();
    }

    public static List<string> GetRandomDemonNames(int amount = 3){
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

        var result = new List<string>();
        for (int i = 0; i < Mathf.Min(amount, uniqueDemonNames.Count); i++){
            int dn = Random.Range(0, uniqueDemonNames.Count);
            //Debug.Log($"Adding {uniqueDemonNames[dn]}");
            result.Add(uniqueDemonNames[dn]);
            uniqueDemonNames.RemoveAt(dn);
        }

        return result;
    }

    static string IntToRoman(int number){
        string[] thousands ={ "", "M", "MM", "MMM" };
        string[] hundreds ={ "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
        string[] tens ={ "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
        string[] ones ={ "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

        return thousands[number / 1000] +
               hundreds[(number % 1000) / 100] +
               tens[(number % 100) / 10] +
               ones[number % 10];
    }
}