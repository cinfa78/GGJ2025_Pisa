using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings", order = 1)]
public class GameSettings : ScriptableObject{
    [Header("Pope")] public Vector2 minMaxSpeed;
    public Vector2 minMaxChargeSpeed;
    public Vector2 minMaxBubbleGrowth;
    public Vector2 minMaxLowerAngle;
    public Vector2 minMaxUpperAngle;
}

[Serializable]
public class PopeStatistics{
    public float movementSpeed = 5;
    public float incrementPerShot = 0.1f;
    [Header("Shot")]
    public float angleIncrement = 60;
    public Vector2 minMaxAngle = new Vector2(-45, 90);
}