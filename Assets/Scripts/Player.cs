using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Player
{
    public int id;
    public string name;
    public Sprite avatar;
    public TileBase playerTile;
    public PlayerMove playerMoveCopy;
    public PlayerMove playerMoveNoCopy;
    public PlayerAttack PlayerAttackNoCopy;
    public List<Vector3Int> cellPositions = new List<Vector3Int>();
}
