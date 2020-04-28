using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Player
{
    public string name;
    public Sprite avatar;
    public TileBase playerTile;
    public int id;
    public List<Vector2Int> cellPositions = new List<Vector2Int>();
}
