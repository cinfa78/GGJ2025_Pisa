using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class PopeStatistics{
    public float MovementSpeed = 5;
    public float IncrementPerShot = 0.1f;
    public float AngleIncrement = 60;
    public float MinAngle = -45;
    public float MaxAngle = 90;

    public PopeStatistics(float movementSpeed, float incrementPerShot, float angleIncrement, Vector2 minMaxAngle){
        MovementSpeed = movementSpeed;
        IncrementPerShot = incrementPerShot;
        AngleIncrement = angleIncrement;
        MinAngle = minMaxAngle.x;
        MaxAngle = minMaxAngle.y;
    }

    public PopeStatistics(){
        MovementSpeed = 5;
        IncrementPerShot = 0.1f;
        AngleIncrement = 60;
        MinAngle = -45;
        MaxAngle = 90;
    }

    public override string ToString(){
        string output = "<color=#FF7700>Pope Statistics:</color>\n";
        output += "\tMovement Speed: " + MovementSpeed + "\n";
        output += "\tIncrement Per Shot: " + IncrementPerShot + "\n";
        output += "\tAngle Increment: " + AngleIncrement + "\n";
        output += "\tMinMax Angle: " + MinAngle + "-" + MaxAngle + "\n";
        return output;
    }
}

[Serializable]
public class SaveData{
    public string[] KilledDemonsData;
    public int PopeNumber;
    public PopeStatistics PopeStatistics;
    public string[] PopeNames;

    public SaveData(string[] killedDemons, string[] popeNames, int popeNumber){
        KilledDemonsData = killedDemons;
        PopeNumber = popeNumber;
        PopeNames = popeNames;
        PopeStatistics = new PopeStatistics();
    }

    public bool ContainsDemon(string searchedDemon){
        for (int i = 0; i < KilledDemonsData.Length; i++){
            if (KilledDemonsData[i] == searchedDemon) return true;
        }

        return false;
    }

    public override string ToString(){
        string result = "";

        result += $"<color=#ff0000>Demons Killed:</color> {KilledDemonsData.Length}\n";
        if (KilledDemonsData != null && KilledDemonsData.Length > 0){
            result += "Demons:\n";
            for (var i = 0; i < KilledDemonsData.Length; i++){
                result += $"\t{KilledDemonsData[i]}\n";
            }
        }

        result += $"<color=#00FFFF>Pope Number:</color> {PopeNumber}\n";
        result += PopeStatistics + "\n";
        if (PopeNames != null && PopeNames.Length > 0){
            result += "Popes History:\n";
            for (var i = 0; i < PopeNames.Length; i++){
                result += $"\t{PopeNames[i]}\n";
            }
        }

        return result;
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

    public int GetPopeNumber() => _saveData.PopeNumber;

    private void Awake(){
        if (Instance != null){
            Destroy(gameObject);
        }
        else{
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Load();
    }

    public void Save(){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/saveData.dat";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveData data = new SaveData(_saveData.KilledDemonsData, _saveData.PopeNames, _saveData.PopeNumber);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public void Load(){
        Debug.Log($"Loading data");
        string path = Application.persistentDataPath + "/saveData.dat";

        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            _saveData = (SaveData)formatter.Deserialize(stream);
            stream.Close();
            Debug.Log($"Data Loaded\n{_saveData}");
        }
        else{
            ResetSaveData();
        }

        _initialized = true;
    }

    [Button("Reset SaveData")]
    private void ResetSaveData(){
        Debug.Log($"Created new data");
        _saveData = new SaveData(Array.Empty<string>(), Array.Empty<string>(), 0);
        GetLastPopeName();
        Save();
    }

    [Button("Print Save Data")]
    public void PrintSaveData(){
        Debug.Log(_saveData);
    }


    public void AddKilledDemons(List<string> killedDemons){
        var newKilledDemonsList = new List<string>(_saveData.KilledDemonsData);
        newKilledDemonsList.AddRange(killedDemons);
        _saveData.KilledDemonsData = newKilledDemonsList.ToArray();
        Save();
        freshlyUnlockedDemons = new List<string>(killedDemons);
    }

    public void AddDeadPope(){
        _saveData.PopeNumber++;
        Save();
    }

    public void JustUnlockedDemon(string demonName){
        freshlyUnlockedDemons.Remove(demonName);
    }

    public void AddPopeName(string popeName){
        if (_saveData.PopeNames == null) _saveData.PopeNames = Array.Empty<string>();
        var popeNames = new List<string>(_saveData.PopeNames);
        popeNames.Add(popeName);
        _saveData.PopeNames = popeNames.ToArray();
        Save();
    }

    public string GetPopeNumber(string newPopeName, bool getCurrent = false){
        int i = 0;
        if (_saveData.PopeNames != null){
            foreach (var name in _saveData.PopeNames){
                if (name == newPopeName){
                    i++;
                }
            }
        }

        return IntToRoman(getCurrent ? i : i + 1);
    }

    public string GetLastPopeName(){
        if (_saveData.PopeNames.Length == 0){
            AddPopeName("Petrus");
            _saveData.PopeNumber++;
            Save();
            return "Petrus" + " " + GetPopeNumber("Petrus", true);
        }

        string popeName = _saveData.PopeNames[_saveData.PopeNumber - 1];
        return popeName + " " + GetPopeNumber(popeName, true);
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