using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings", order = 1)]
public class GameSettings : ScriptableObject{
    [Header("Pope")] public Vector2 minMaxSpeed;
    public Vector2 minMaxAngleIncrement;
    public Vector2 minMaxBubbleGrowth;
    public Vector2 minMaxLowerAngle;
    public Vector2 minMaxUpperAngle;
}