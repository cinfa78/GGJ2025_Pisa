using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif


using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Demon Names", fileName = "DemonNames")]
public class DemonListSo : ScriptableObject{
    [Serializable]
    public struct DemonData{
        public string name;
        [ShowAssetPreview] public Sprite sprite;
        public Color wingsColor;
        public Vector3 wingsPosition;
    }

    [Header("Default")] public Sprite defaultSprite;
    public Color defaultWingsColor;
    public Material defaultMaterial;
    public Vector3 defaultWingsPosition = new Vector3(0.166f, 0.2f, 0f);
    public GameObject defaultPrefab;
    [Space] public List<DemonData> data;
    [ReadOnly] public List<string> names;


    private Dictionary<string, Sprite> _spritesDictionary = new Dictionary<string, Sprite>();

    [Button("Sort")]
    public void SortNamesList(){
        names.Sort();
        data.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
    }


    public List<string> GetNames(int amount){
        var tempNames = new List<string>(names);
        var result = new List<string>();
        for (int i = 0; i < Mathf.Min(amount, tempNames.Count); i++){
            int randPosition = Random.Range(0, tempNames.Count);
            result.Add(tempNames[randPosition]);
            tempNames.RemoveAt(randPosition);
        }

        return result;
    }

#if UNITY_EDITOR

    public void CopyNamesToData(){
        //data = new List<DemonData>();
        LoadSpritesFromFolder();
        SortNamesList();
        foreach (var n in names){
            int i = 0;
            for (i = 0; i < data.Count; i++){
                if (data[i].name == n){
                    break;
                }
            }

            if (i == data.Count){
                var newDemonData = new DemonData();
                newDemonData.name = n;
                newDemonData.wingsPosition = defaultWingsPosition;
                if (_spritesDictionary.ContainsKey(n)){
                    newDemonData.sprite = _spritesDictionary[n];
                    newDemonData.wingsColor = GetTopColor(_spritesDictionary[n]);
                }
                else{
                    newDemonData.sprite = defaultSprite;
                    newDemonData.wingsColor = defaultWingsColor;
                }


                data.Add(newDemonData);
            }
        }
    }

    [Button("Update Data")]
    public void UpdateDemonsData(){
        LoadSpritesFromFolder();
        SortNamesList();
        for (int i = 0; i < data.Count; i++){
            var newDemonData = data[i];
            if (_spritesDictionary.ContainsKey(data[i].name)){
                newDemonData.sprite = _spritesDictionary[data[i].name];
                newDemonData.wingsColor = GetTopColor(_spritesDictionary[data[i].name]);
                //CreatePrefabVariant(newDemonData);
            }
            else{
                newDemonData.sprite = defaultSprite;
                newDemonData.wingsColor = defaultWingsColor;
                newDemonData.wingsPosition = defaultWingsPosition;
            }

            data[i] = newDemonData;
        }
    }

    [Button("Prefabs")]
    private void CreatePrefabs(){
        LoadSpritesFromFolder();
        SortNamesList();
        for (int i = 0; i < data.Count; i++){
            var newDemonData = data[i];
            if (_spritesDictionary.ContainsKey(data[i].name)){
                newDemonData.sprite = _spritesDictionary[data[i].name];
                newDemonData.wingsColor = GetTopColor(_spritesDictionary[data[i].name]);
                CreatePrefabVariant(newDemonData);
            }
            else{
                newDemonData.sprite = defaultSprite;
                newDemonData.wingsColor = defaultWingsColor;
                newDemonData.wingsPosition = defaultWingsPosition;
            }

            data[i] = newDemonData;
        }
    }

    private void LoadSpritesFromFolder(string relativePath = "Assets/Graphics/Demons"){
        _spritesDictionary = new Dictionary<string, Sprite>();
        string fullPath = Path.Combine(Application.dataPath, relativePath.Replace("Assets/", ""));

        if (!Directory.Exists(fullPath)){
            Debug.LogError("Directory does not exist: " + fullPath);
            return;
        }

        string[] files = Directory.GetFiles(fullPath, "*.png");


        foreach (string file in files){
            string fileName = Path.GetFileName(file);
            // string fileNameNoExtension = Path.GetDirectoryName(file) +"/"+ fileName;
            // AssetDatabase.Refresh();
            // File.WriteAllText(fileNameNoExtension + ".meta", Regex.Replace(File.ReadAllText(fileNameNoExtension + ".meta"), "isReadable: 0", "isReadable: 1"));
            // AssetDatabase.Refresh();
            string assetPath = relativePath + "/" + fileName;
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null){
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.isReadable = true;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 32;
                importer.spritePixelsPerUnit = 32;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.SaveAndReimport(); // Ensure it's treated as a sprite
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null){
                _spritesDictionary.Add(sprite.name, sprite);
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private Color GetTopColor(Sprite sprite){
        var colorDictionary = new Dictionary<Color32, int>();
        Texture2D texture = sprite.texture;

        Color32[] pixels = texture.GetPixels32();
        Color32 topColor = new Color32(1, 0, 1, 1);
        int topColorNumber = 0;

        for (int i = 0; i < pixels.Length; i++){
            if (pixels[i].a > 0){
                if (colorDictionary.ContainsKey(pixels[i])){
                    colorDictionary[pixels[i]]++;
                    if (colorDictionary[pixels[i]] > topColorNumber){
                        {
                            topColorNumber = colorDictionary[pixels[i]];
                            topColor = pixels[i];
                        }
                    }
                }
                else{
                    colorDictionary.Add(pixels[i], 1);
                    if (topColorNumber == 0){
                        topColorNumber = 1;
                        topColor = pixels[i];
                    }
                }
            }
        }

        return topColor;
    }

    public void CreatePrefabVariant(DemonData demonData){
        Object originalPrefab = defaultPrefab;
        GameObject objSource = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;
        GameObject prefabVariant = PrefabUtility.SaveAsPrefabAsset(objSource, AssetDatabase.GenerateUniqueAssetPath($"Assets/Prefabs/Demons/{demonData.name}.prefab"));
        prefabVariant.transform.Find("SpriteContainer").GetComponent<SpriteRenderer>().sprite = demonData.sprite;
        prefabVariant.transform.Find("SpriteContainer/WingsContainer/WingBack").GetComponent<SpriteRenderer>().color = demonData.wingsColor;
        prefabVariant.transform.Find("SpriteContainer/WingsContainer/WingFront").GetComponent<SpriteRenderer>().color = demonData.wingsColor;
        PrefabUtility.SavePrefabAsset(prefabVariant);
        DestroyImmediate(objSource);
    }
#endif
}