using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerAction
{
    public List<Vector3Int> availableTranslations = new List<Vector3Int>();

    public PlayerAction(List<Vector3Int> translations)
    {
        availableTranslations = translations;
    }
    
    public PlayerAction(PlayerActionScriptableObject template)
    {
        availableTranslations = template.availableTranslations;
    }
}

[Serializable]
public class PlayerMove : PlayerAction
{
    public bool copyCell = false;
    public PlayerMove(PlayerActionScriptableObject translations,bool copyCell) : base(translations)
    {
        this.copyCell = copyCell;
    }
}

[Serializable]
public class PlayerAttack : PlayerAction
{
    public bool copyCell = false;
    public PlayerAttack(PlayerActionScriptableObject translations,bool copyCell) : base(translations)
    {
        this.copyCell = copyCell;
    }
}