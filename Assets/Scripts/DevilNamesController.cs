using System.Collections.Generic;
using UnityEngine;

public class DevilNamesController : MonoBehaviour{
    private List<string> _uniqueDemonNames = new List<string>();

    private void Awake(){
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
            _uniqueDemonNames.Add(_d.Key);
        }

        _uniqueDemonNames.Sort();
    }

    private void Start(){
        // foreach (var d in _uniqueDemonNames){
        //     Debug.Log(d);
        // }
    }
}