using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAction
{
    public List<Vector2Int> availableTranslations = new List<Vector2Int>();

    public PlayerAction(List<Vector2Int> translations)
    {
        availableTranslations = translations;
    }
}

[Serializable]
public class PlayerMove : PlayerAction
{
    public bool copyCell = false;
    public PlayerMove(List<Vector2Int> translations,bool copyCell) : base(translations)
    {
        this.copyCell = copyCell;
    }
}

[Serializable]
public class PlayerAttack : PlayerAction
{
    public bool copyCell = false;
    public PlayerAttack(List<Vector2Int> translations,bool copyCell) : base(translations)
    {
        this.copyCell = copyCell;
    }
}