using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PopeStatistics{
    public string Name = "Petrus";
    public float MovementSpeed = 5;
    public float IncrementPerShot = 0.1f;
    public float AngleIncrement = 60;
    public float MinAngle = -45;
    public float MaxAngle = 90;

    public PopeStatistics(string name, float movementSpeed, float incrementPerShot, float angleIncrement, Vector2 minMaxAngle){
        Name = name;
        MovementSpeed = movementSpeed;
        IncrementPerShot = incrementPerShot;
        AngleIncrement = angleIncrement;
        MinAngle = minMaxAngle.x;
        MaxAngle = minMaxAngle.y;
    }

    public PopeStatistics(){
        Name = "Petrus";
        MovementSpeed = 5;
        IncrementPerShot = 0.1f;
        AngleIncrement = 60;
        MinAngle = -45;
        MaxAngle = 90;
    }

    public PopeStatistics(PopeStatistics popeStatistics){
        Name = popeStatistics.Name;
        MovementSpeed = popeStatistics.MovementSpeed;
        IncrementPerShot = popeStatistics.IncrementPerShot;
        AngleIncrement = popeStatistics.AngleIncrement;
        MinAngle = popeStatistics.MinAngle;
        MaxAngle = popeStatistics.MaxAngle;
    }

    public override string ToString(){
        string output = $"<color=#FF7700>Pope {Name} Statistics:</color>\n";
        output += "\tMovement Speed: " + MovementSpeed + "\n";
        output += "\tIncrement Per Shot: " + IncrementPerShot + "\n";
        output += "\tAngle Increment: " + AngleIncrement + "\n";
        output += "\tMinMax Angle: " + MinAngle + "-" + MaxAngle + "\n";
        return output;
    }
}

[Serializable]
public class SaveData{
    public int KilledBosses;
    public string[] KilledDemonsData;
    public string[] ToShowKilledDemonsData;
    public int PopeNumber;
    public PopeStatistics PopeStatistics;
    public string[] PopeNames;

    public SaveData(int killedBosses, string[] killedDemons, string[] toShowKilledDemonsData, string[] popeNames, int popeNumber, PopeStatistics popeStatistics){
        KilledBosses = killedBosses;
        KilledDemonsData = killedDemons;
        ToShowKilledDemonsData = toShowKilledDemonsData;
        PopeNumber = popeNumber;
        PopeNames = popeNames;
        PopeStatistics = new PopeStatistics(popeStatistics);
    }

    public bool ContainsDemon(string searchedDemon){
        for (int i = 0; i < KilledDemonsData.Length; i++){
            if (KilledDemonsData[i] == searchedDemon) return true;
        }

        return false;
    }

    public override string ToString(){
        string result = "Current Save Data\n";

        result += $"<color=#00FFFF>Pope Number:</color> {PopeNumber}\n";
        result += PopeStatistics + "\n";
        if (PopeNames != null && PopeNames.Length > 0){
            result += "Popes History:\n";
            for (var i = 0; i < PopeNames.Length; i++){
                result += $"\t{PopeNames[i]}\n";
            }
        }

        result += $"<color=#FF0000>Bosses Killed:</color> {KilledBosses}\n";
        result += $"<color=#FF3333>Demons Killed:</color> {KilledDemonsData.Length}\n";
        if (KilledDemonsData != null && KilledDemonsData.Length > 0){
            result += "Demons:\n";
            for (var i = 0; i < KilledDemonsData.Length; i++){
                result += $"\t{KilledDemonsData[i]}\n";
            }
        }


        return result;
    }
}

public class SaveManager : MonoBehaviour{
    private const string FILENAME = "SalutisData.amen";
    public static SaveManager Instance;
    public GameSettings GameSettings;
    public DemonListSo demonListSo;
    public static event Action<PopeStatistics> OnNewPope;
    private SaveData _saveData;

    public SaveData GetSavedData => _saveData;
    public static List<string> freshlyUnlockedDemons = new List<string>();

    private static string[] _popeNames = new string[]{
        "Alexander",
        "Bonifacius",
        "Celestinus",
        "Clementius",
        "Eugenius",
        "Gregorius",
        "Hadrianus",
        "Innocentius",
        "Laurentius",
        "Leo",
        "Leoninus",
        "Marcus",
        "Maximilianus",
        "Nicolaus",
        "Paulus",
        "Petrus",
        "Pius",
        "Raphael",
        "Silvester",
        "Sextus",
        "Stefanus",
        "Theodorus",
        "Urbanus"
    };

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

    private void OnDestroy(){
        Save();
    }

    public void Save(){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + FILENAME;
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveData data = new SaveData(_saveData.KilledBosses, _saveData.KilledDemonsData, _saveData.ToShowKilledDemonsData, _saveData.PopeNames, _saveData.PopeNumber, _saveData.PopeStatistics);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public void Load(){
        Debug.Log($"<color=#AAAA11>Loading data...</color>");
        string path = Application.persistentDataPath + "/" + FILENAME;

        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            _saveData = (SaveData)formatter.Deserialize(stream);
            freshlyUnlockedDemons = new List<string>(_saveData.ToShowKilledDemonsData).Distinct().ToList();
            stream.Close();
            Debug.Log($"<color=#11FF11>Data Loaded</color>:");
            Debug.Log(_saveData);
        }
        else{
            ResetSaveData();
        }

        //_initialized = true;
    }

    [Button("Reset SaveData")]
    private void ResetSaveData(){
        Debug.Log($"Created new data");
        _saveData = new SaveData(0, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), 0, popeStatistics: new PopeStatistics());
        var popeName = GetLastPopeName();
        _saveData.PopeStatistics.Name = popeName;
        freshlyUnlockedDemons = new List<string>();
        Save();
    }

    [Button("Print Save Data")]
    public void PrintSaveData(){
        Debug.Log(_saveData);
    }


    public void AddKilledDemonsAndBoss(List<string> killedDemons){
        var newKilledDemonsList = new List<string>(_saveData.KilledDemonsData);
        newKilledDemonsList.AddRange(killedDemons);
        newKilledDemonsList = newKilledDemonsList.Distinct().ToList();
        _saveData.KilledDemonsData = newKilledDemonsList.ToArray();

        freshlyUnlockedDemons.AddRange(killedDemons);
        freshlyUnlockedDemons = freshlyUnlockedDemons.Distinct().ToList();
        _saveData.ToShowKilledDemonsData = freshlyUnlockedDemons.ToArray();

        _saveData.KilledBosses++;

        Save();
    }

    public void JustUnlockedDemon(string demonName){
        freshlyUnlockedDemons.Remove(demonName);
        _saveData.ToShowKilledDemonsData = freshlyUnlockedDemons.ToArray();
    }

    public string GenerateNewPope(string forcedName){
        var popeName = forcedName;
        if (forcedName == null){
            popeName = _popeNames[UnityEngine.Random.Range(0, _popeNames.Length)];
        }

        var output = popeName + " " + GetPopeNumber(popeName);
        var popeNames = new List<string>(_saveData.PopeNames);
        popeNames.Add(popeName);
        _saveData.PopeNames = popeNames.ToArray();
        _saveData.PopeNumber++;

        //generare caratteristiche papa
        _saveData.PopeStatistics.Name = popeName;
        _saveData.PopeStatistics.MovementSpeed = Random.Range(GameSettings.minMaxSpeed.x, GameSettings.minMaxSpeed.y);
        _saveData.PopeStatistics.IncrementPerShot = Random.Range(GameSettings.minMaxBubbleGrowth.x, GameSettings.minMaxBubbleGrowth.y);
        _saveData.PopeStatistics.MinAngle = Random.Range(GameSettings.minMaxLowerAngle.x, GameSettings.minMaxLowerAngle.y);
        _saveData.PopeStatistics.MaxAngle = Random.Range(GameSettings.minMaxUpperAngle.x, GameSettings.minMaxUpperAngle.y);
        _saveData.PopeStatistics.AngleIncrement = Random.Range(GameSettings.minMaxAngleIncrement.x, GameSettings.minMaxAngleIncrement.y);
        OnNewPope?.Invoke(_saveData.PopeStatistics);
        Save();
        return output;
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
            Debug.Log($"Creating first pope Petrus I");
            return GenerateNewPope("Petrus");
        }

        string popeName = _saveData.PopeNames[_saveData.PopeNumber - 1];
        return popeName + " " + GetPopeNumber(popeName, true);
    }

    public static string IntToRoman(int number){
        string[] thousands ={ "", "M", "MM", "MMM" };
        string[] hundreds ={ "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
        string[] tens ={ "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
        string[] ones ={ "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

        return thousands[number / 1000] +
               hundreds[(number % 1000) / 100] +
               tens[(number % 100) / 10] +
               ones[number % 10];
    }

    public static int GetDeterministicHashCode(string input){
        //Debug.Log(input);
        using (SHA256 sha256 = SHA256.Create()){
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Math.Abs(BitConverter.ToInt32(hash, 0));
        }
    }
}