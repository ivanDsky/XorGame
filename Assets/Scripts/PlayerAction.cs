using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public interface IPlayerAction
{
    List<Vector3Int> availableTranslations { get; set; }
}

public interface IDrawActionInstruction
{
    bool Instruction(MapClass map,Vector3Int pos,Player player);
}

public interface IMakeActionInstruction
{
    void Instruction(Tilemap tilemap, TileBase defaultTile, MapClass map, Vector3Int posFrom, Vector3Int posTo,
        Player player);
}

[Serializable]
public class PlayerAction : IPlayerAction
{
    public List<Vector3Int> availableTranslations { get; set; }

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
public class PlayerMove : PlayerAction,IDrawActionInstruction,IMakeActionInstruction
{
    public bool copyCell = false;
    public PlayerMove(PlayerActionScriptableObject translations,bool copyCell) : base(translations)
    {
        this.copyCell = copyCell;
    }

    bool IDrawActionInstruction.Instruction(MapClass map,Vector3Int checkPos,Player player)
    {
        return map.cellMap.ContainsKey(checkPos) && map.cellMap[checkPos].tilePlayer == null;
    }
    
    void IMakeActionInstruction.Instruction(Tilemap tilemap,TileBase defaultTile,MapClass map,Vector3Int posFrom,Vector3Int posTo, Player player)
    {
        map.PlayerUse(map.cellMap[posTo],player,tilemap);
        if(!copyCell)map.PlayerUnuse(map.cellMap[posFrom],map.cellMap[posFrom].tilePlayer,tilemap,defaultTile);
    }
}

[Serializable]
public class PlayerAttack : PlayerAction,IDrawActionInstruction,IMakeActionInstruction
{
    public bool copyCell = false;
    public PlayerAttack(PlayerActionScriptableObject translations,bool copyCell) : base(translations)
    {
        this.copyCell = copyCell;
    }
    bool IDrawActionInstruction.Instruction(MapClass map,Vector3Int checkPos,Player player)
    {
        return map.cellMap.ContainsKey(checkPos) &&
               map.cellMap[checkPos].tilePlayer != null &&
               map.cellMap[checkPos].tilePlayer != player;
    }
    
    void IMakeActionInstruction.Instruction(Tilemap tilemap,TileBase defaultTile,MapClass map,Vector3Int posFrom,Vector3Int posTo, Player player)
    {
        map.PlayerUnuse(map.cellMap[posTo],map.cellMap[posTo].tilePlayer,tilemap,defaultTile);
        map.PlayerUse(map.cellMap[posTo],player,tilemap);
        map.PlayerUnuse(map.cellMap[posFrom],player,tilemap,defaultTile);
    }
}