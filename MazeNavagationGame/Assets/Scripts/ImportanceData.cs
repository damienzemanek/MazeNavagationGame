using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Importance Data", menuName = "ScriptableObjects/Importance Data")]
public class ImportanceData : ScriptableObject
{
    public int movementImportance; 
}
