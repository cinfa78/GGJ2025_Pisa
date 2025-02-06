using System;
using System.IO;
using UnityEngine;


public class SaveManager{
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
}