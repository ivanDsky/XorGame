using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class MapClass
{
    public Dictionary<Vector3Int,FieldCell> cellMap;
    public Vector2Int size;

    public MapClass(Vector2Int size)
    {
        
        this.size = size;
        cellMap = new Dictionary<Vector3Int, FieldCell>();
    }

    public bool IsAvaiable(Vector3Int pos,Tilemap tilemap)
    {
        List<FieldCell> cnt = new List<FieldCell>();
        for (int addX = -1; addX <= 1; ++addX)
        {
            for (int addY = -1; addY <= 1; ++addY)
            {
                if (Math.Abs(addX + addY) != 1) continue;
                Vector3Int nPos = new Vector3Int(pos.x + addX,pos.y + addY,0);
                if (!cellMap.ContainsKey(nPos)) continue;
                FieldCell cell = cellMap[nPos];
                Player neighbour = cell.tilePlayer;
                if (neighbour == null) return true;
                cnt.Add(cell);
            }
        }
        for (int i = 0; i < cnt.Count - 1; ++i)
        {
            if (cnt[i].tilePlayer != cnt[i + 1].tilePlayer) return true;
        }
        PlayerUse(cellMap[pos],cnt[0].tilePlayer,tilemap);
        return false;
    }

    public void PlayerUse(FieldCell cell,Player player,Tilemap tilemap)
    {
        cellMap[cell.pos].tile = player.playerTile;
        cellMap[cell.pos].tilePlayer = player;
        player.cellPositions.Add(cell.pos);
        tilemap.SetTile(cell.pos,player.playerTile);
    }
    
    public void PlayerUnuse(FieldCell cell,Player player,Tilemap tilemap,TileBase defaulTile)
    {
        cellMap[cell.pos].tile = defaulTile;
        cellMap[cell.pos].tilePlayer = null;
        player.cellPositions.Remove(cell.pos);
        tilemap.SetTile(cell.pos,defaulTile);
        tilemap.SetTileFlags(cell.pos,TileFlags.None);
    }
}
