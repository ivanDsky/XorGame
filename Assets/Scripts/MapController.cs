using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    public PlayerActionScriptableObject _4sides;
    public PlayerActionScriptableObject _8sides;
    public TileBase defaulTile;
    public MapClass map;
    public Vector2Int mapSize;
    public Player[] players;
    public int currentPlayerIndex;
    public Color turnColor = new Color(0.47f,1,1);
    public Color turnColorNoCopy = new Color(0.19f,0.5f,1);
    public Color attackColorNoCopy = new Color(1f, 0.15f, 0.09f);
    private Camera _mainCamera;
    private Tilemap _tilemap;

    private void Awake()
    {
        foreach (var player in players)
        {
            player.playerMoveCopy = new PlayerMove(_4sides,true);
            player.playerMoveNoCopy = new PlayerMove(_8sides,false);
            player.PlayerAttackNoCopy = new PlayerAttack(_4sides,false);
        }
        _mainCamera = Camera.main;
        _tilemap = GetComponent<Tilemap>();
        map = new MapClass(mapSize);
        foreach (var pos in _tilemap.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            if (_tilemap.HasTile(localPlace))
            {
                map.cellMap[localPlace] = new FieldCell(_tilemap.GetTile(localPlace),localPlace);
                foreach (var player in players)
                {
                    if (player.playerTile == _tilemap.GetTile(localPlace))
                    {
                        player.cellPositions.Add(localPlace);
                        map.cellMap[localPlace].tilePlayer = player;
                    }
                }
                _tilemap.SetTileFlags(localPlace,TileFlags.None);
            }
        }
    }

    private void Update()
    {
        Player currentPlayer = players[currentPlayerIndex];
        DrawMove(currentPlayer, turnColorNoCopy, currentPlayer.playerMoveNoCopy);
        DrawMove(currentPlayer, turnColor, currentPlayer.playerMoveCopy);
        DrawMove(currentPlayer, attackColorNoCopy, currentPlayer.PlayerAttackNoCopy);
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 currentPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = _tilemap.WorldToCell(currentPos);
            if (map.cellMap.ContainsKey(cellPos) && _tilemap.GetColor(cellPos) == turnColor)
            {
                ClearCells();
                MakeCopyMove(cellPos, currentPlayer);
            }
            if (map.cellMap.ContainsKey(cellPos) && _tilemap.GetColor(cellPos) == turnColorNoCopy)
            {
                ClearCells();
                List<Vector3Int> availableCells = GetPos(currentPlayer, cellPos, currentPlayer.playerMoveNoCopy);
                MakeNoCopyMove(availableCells[0],cellPos, currentPlayer);
            }
            if (map.cellMap.ContainsKey(cellPos) && _tilemap.GetColor(cellPos) == attackColorNoCopy)
            {
                ClearCells();
                List<Vector3Int> availableCells = GetPos(currentPlayer, cellPos, currentPlayer.PlayerAttackNoCopy);
                MakeNoCopyAttack(availableCells[0],cellPos, currentPlayer);
            }
        }
    }

    public void ClearCells()
    {
        foreach (var cell in map.cellMap)
        {
            _tilemap.SetColor(cell.Key,Color.white);
        }
    }

    public bool DrawMove(Player player,Color color,PlayerMove action)
    {
        bool setTile = false;
        foreach (var cell in player.cellPositions)
        {
            foreach (var add in action.availableTranslations)
            {
                Vector3Int checkPos = cell + add;
                if (map.cellMap.ContainsKey(checkPos) && map.cellMap[checkPos].tilePlayer == null)
                {
                    _tilemap.SetColor(checkPos,color);
                    setTile = true;
                }
            }
        }
        return setTile;
    }
    
    public bool DrawMove(Player player,Color color,PlayerAttack action)
    {
        bool setTile = false;
        foreach (var cell in player.cellPositions)
        {
            foreach (var add in action.availableTranslations)
            {
                Vector3Int checkPos = cell + add;
                if (map.cellMap.ContainsKey(checkPos) &&
                    map.cellMap[checkPos].tilePlayer != null &&
                    map.cellMap[checkPos].tilePlayer != player)
                {
                    setTile = true;
                    _tilemap.SetTileFlags(checkPos,TileFlags.None);
                    _tilemap.SetColor(checkPos,color);
                }
            }
        }
        return setTile;
    }
    
    public List<Vector3Int> GetPos(Player player,Vector3Int posTo,PlayerAttack action)
    {
        List<Vector3Int> playerCells = new List<Vector3Int>();
        foreach (var add in action.availableTranslations)
        {
            Vector3Int checkPos = posTo + add;
            if (map.cellMap.ContainsKey(checkPos) &&
                map.cellMap[checkPos].tilePlayer == player)
            {
                playerCells.Add(checkPos);
            }
        }

        return playerCells;
    }
    
    public List<Vector3Int> GetPos(Player player,Vector3Int posTo,PlayerMove action)
    {
        List<Vector3Int> playerCells = new List<Vector3Int>();
        foreach (var add in action.availableTranslations)
        {
            Vector3Int checkPos = posTo + add;
            if (map.cellMap.ContainsKey(checkPos) &&
                map.cellMap[checkPos].tilePlayer == player)
            {
                playerCells.Add(checkPos);
            }
        }

        return playerCells;
    }

    public void MakeCopyMove(Vector3Int pos, Player player)
    {
        map.PlayerUse(map.cellMap[pos],player,_tilemap);
        currentPlayerIndex++;
        currentPlayerIndex %= players.Length;
    }
    
    public void MakeNoCopyMove(Vector3Int posFrom,Vector3Int posTo, Player player)
    {
        map.PlayerUse(map.cellMap[posTo],player,_tilemap);
        map.PlayerUnuse(map.cellMap[posFrom],map.cellMap[posFrom].tilePlayer,_tilemap,defaulTile);
        currentPlayerIndex++;
        currentPlayerIndex %= players.Length;
    }
    
    public void MakeNoCopyAttack(Vector3Int posFrom,Vector3Int posTo, Player player)
    {
        map.PlayerUnuse(map.cellMap[posTo],map.cellMap[posTo].tilePlayer,_tilemap,defaulTile);
        map.PlayerUse(map.cellMap[posTo],player,_tilemap);
        map.PlayerUnuse(map.cellMap[posFrom],player,_tilemap,defaulTile);
        currentPlayerIndex++;
        currentPlayerIndex %= players.Length;
    }
    
}
