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
    public Dictionary<Vector2Int,FieldCell> cellMap;
    public Vector2Int size;

    public MapClass(Vector2Int size)
    {
        
        this.size = size;
        cellMap = new Dictionary<Vector2Int, FieldCell>();
    }

    public bool IsAvaiable(Vector2Int pos,Tilemap tilemap)
    {
        List<FieldCell> cnt = new List<FieldCell>();
        for (int addX = -1; addX <= 1; ++addX)
        {
            for (int addY = -1; addY <= 1; ++addY)
            {
                if (Math.Abs(addX + addY) != 1) continue;
                Vector2Int nPos = new Vector2Int(pos.x + addX,pos.y + addY);
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
        tilemap.SetTile((Vector3Int)cell.pos,player.playerTile);
    }
}
