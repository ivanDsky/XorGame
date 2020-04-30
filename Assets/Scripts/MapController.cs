using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    public PlayerActionScriptableObject _4sides;
    public PlayerActionScriptableObject _8sides;
    public Tilemap chooseTilemap;
    public TileBase defaultTile;
    public TileBase chooseTile;
    public TileBase bonusTile;
    public MapClass map;
    public Vector2Int mapSize;
    public Player[] players;
    public int currentPlayerIndex;
    public Color turnColor = new Color(0.47f,1,1);
    public Color turnColorNoCopy = new Color(0.19f,0.5f,1);
    public Color attackColorNoCopy = new Color(1f, 0.15f, 0.09f);
    public Color chooseColor = Color.black;
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

    public ActionStage actionStage = ActionStage.ChoosePosTo;
    public GameMode gameMode = GameMode.MoveCopy;
    public int gameModeID = 0;
    public Vector3Int posFrom, posTo;
    public IMakeActionInstruction instruction;
    private void Update()
    {
        Player currentPlayer = players[currentPlayerIndex];
        if (actionStage == ActionStage.ChoosePosTo)
        {
            if(gameMode == GameMode.MoveCopy)DrawMove(currentPlayer, turnColor, currentPlayer.playerMoveCopy, currentPlayer.playerMoveCopy);
            if(gameMode == GameMode.MoveNoCopy)DrawMove(currentPlayer, turnColorNoCopy, currentPlayer.playerMoveNoCopy, currentPlayer.playerMoveNoCopy);
            if(gameMode == GameMode.AttackNoCopy)DrawMove(currentPlayer, attackColorNoCopy, currentPlayer.PlayerAttackNoCopy, currentPlayer.PlayerAttackNoCopy);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 currentPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = _tilemap.WorldToCell(currentPos);
            if (map.cellMap.ContainsKey(cellPos))
            {
                if (actionStage == ActionStage.ChoosePosTo && _tilemap.GetColor(cellPos) != Color.white)
                {
                    List<Vector3Int> availableCells = new List<Vector3Int>();
                    posTo = cellPos;
                    if (_tilemap.GetColor(cellPos) == turnColor)
                    {
                        availableCells = GetPos(currentPlayer, cellPos, currentPlayer.playerMoveCopy);
                        instruction = currentPlayer.playerMoveCopy;
                    }

                    if (_tilemap.GetColor(cellPos) == turnColorNoCopy)
                    {
                        availableCells = GetPos(currentPlayer, cellPos, currentPlayer.playerMoveNoCopy);
                        instruction = currentPlayer.playerMoveNoCopy;
                    }

                    if (_tilemap.GetColor(cellPos) == attackColorNoCopy)
                    {
                        availableCells = GetPos(currentPlayer, cellPos, currentPlayer.PlayerAttackNoCopy);
                        instruction = currentPlayer.PlayerAttackNoCopy;
                    }

                    ClearCells();
                    if (availableCells.Count == 1){
                        posFrom = availableCells[0];
                        actionStage = ActionStage.MakeAction;
                    }
                    else
                    {
                        foreach (var cell in availableCells)
                        {
                            chooseTilemap.SetTile(cell,chooseTile);
                            chooseTilemap.SetTileFlags(cell,TileFlags.None);
                            chooseTilemap.SetColor(cell,chooseColor);
                        }
                        actionStage = ActionStage.ChoosePosFrom;
                    }
                }else
                if (actionStage == ActionStage.ChoosePosFrom)
                {
                    if (chooseTilemap.GetColor(cellPos) == chooseColor)
                    {
                        posFrom = cellPos;
                    }
                    actionStage = ActionStage.MakeAction;
                }
                if (actionStage == ActionStage.MakeAction)
                {
                    bool isBonus = false;
                    if (map.cellMap[posTo].tile == bonusTile)
                    {
                        isBonus = true;
                        instruction = currentPlayer.playerMoveCopy;
                    }
                    MakeInstruction(posFrom,posTo,currentPlayer,instruction);
                    if(isBonus)SpawnBonusTile();
                    ClearCells();
                    actionStage = ActionStage.ChoosePosTo;
                    gameModeID = 0;
                    gameMode = GameMode.MoveCopy;
                }
            }

           
        }
    }


    public void SpawnBonusTile()
    {
        int cellCount = map.cellMap.Count - 1;
        foreach (var player in players)
        {
            cellCount -= player.cellPositions.Count;
        }

        int cellID = Random.Range(0, cellCount);
        foreach (var cellPair in map.cellMap)
        {
            if (cellPair.Value.tilePlayer != null) continue;
            if (cellID == 0)
            {
                _tilemap.SetTile(cellPair.Key,bonusTile);
                cellPair.Value.tile = bonusTile;
                Debug.Log(cellPair.Value.pos);
                break;
            }
            --cellID;
        }

    }
    
    public void ChangeGameMode()
    {
        ++gameModeID;
        gameModeID %= 3;
        switch (gameModeID)
        {
            case(0) : 
                gameMode = GameMode.MoveCopy;
                break; 
            case(1) : 
                gameMode = GameMode.MoveNoCopy; 
                break; 
            case(2) : 
                gameMode = GameMode.AttackNoCopy; 
                break; 
        }
        ClearCells();
    }
    
    public void ClearCells()
    {
        foreach (var cell in map.cellMap)
        {
            _tilemap.SetColor(cell.Key,Color.white);
        }
        chooseTilemap.ClearAllTiles();
    }
    
    public bool DrawMove(Player player,Color color,IPlayerAction action,IDrawActionInstruction instruction)
    {
        bool setTile = false;
        foreach (var cell in player.cellPositions)
        {
            foreach (var add in action.availableTranslations)
            {
                Vector3Int checkPos = cell + add;
                if (instruction.Instruction(map,checkPos,player))
                {
                    setTile = true;
                    _tilemap.SetTileFlags(checkPos,TileFlags.None);
                    _tilemap.SetColor(checkPos,color);
                }
            }
        }
        return setTile;
    }

    public List<Vector3Int> GetPos(Player player,Vector3Int posTo,IPlayerAction action)
    {
        List<Vector3Int> playerCells = new List<Vector3Int>();
        foreach (var add in action.availableTranslations)
        {
            Vector3Int checkPos = posTo + add * -1;
            if (map.cellMap.ContainsKey(checkPos) &&
                map.cellMap[checkPos].tilePlayer == player)
            {
                playerCells.Add(checkPos);
            }
        }

        return playerCells;
    }
    
    public void MakeInstruction(Vector3Int posFrom, Vector3Int posTo, Player player, IMakeActionInstruction instruction)
    {
        instruction.Instruction(_tilemap,defaultTile,map,posFrom,posTo,player);
        currentPlayerIndex++;
        currentPlayerIndex %= players.Length;
    }
    
}

public enum ActionStage
{
    ChoosePosTo,
    ChoosePosFrom,
    MakeAction,
}

public enum GameMode
{
    MoveCopy,
    MoveNoCopy,
    AttackNoCopy,
}
