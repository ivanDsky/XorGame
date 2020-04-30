using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Translations",order = 80)]
public class PlayerActionScriptableObject : ScriptableObject
{
    public List<Vector3Int> availableTranslations = new List<Vector3Int>();
}
