using UnityEngine;

[CreateAssetMenu(fileName = "Random_", menuName = "Audio Clip Random")]
public class AudioClipRandom : ScriptableObject{
    public AudioClip[] clips;

    public AudioClip GetRandomClip(){
        return clips[Random.Range(0, clips.Length)];
    }
}