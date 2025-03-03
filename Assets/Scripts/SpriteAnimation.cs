using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sprite Animation", fileName = "newAnimation")]
public class SpriteAnimation : ScriptableObject{
    [Serializable]
    public struct Loop{
        public string name;
        public Sprite[] frames;
        public float fps;
        public bool loop;
    }

    public Dictionary<string, Loop> loops = new Dictionary<string, Loop>();
    public Loop[] internalLoops;

    private void OnEnable(){
        if (internalLoops != null && internalLoops.Length > 0)
            foreach (var l in internalLoops){
                loops.Add(l.name, l);
            }
    }

    private void OnValidate(){
        if (internalLoops != null && internalLoops.Length == 0){
            Debug.LogError($"Missing animations in {name}");
        }
    }

    public Loop GetFirstLoop(){
        return internalLoops[0];
    }

    public Loop GetLoop(string name){
        return loops[name];
    }
}