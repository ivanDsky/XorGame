using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    public static MapClass Map;
    public Vector2Int mapSize;
    public Player[] players;
    public int currentPlayerIndex;
    public Color turnColor = new Color(0.47f,1,1);
    private Camera _mainCamera;
    private Tilemap _tilemap;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _tilemap = GetComponent<Tilemap>();
        Map = new MapClass(mapSize);
        foreach (var pos in _tilemap.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            if (_tilemap.HasTile(localPlace))
            {
                _tilemap.SetTileFlags(localPlace,TileFlags.None);
                Map.cellMap[(Vector2Int)localPlace] = new FieldCell(_tilemap.GetTile(localPlace),(Vector2Int)localPlace);
            }
        }

        foreach (var player in players)
        {
            foreach (var cellPosition in player.cellPositions)
            {
                Map.PlayerUse(Map.cellMap[cellPosition],player,_tilemap);
            }
        }
    }

    private void Update()
    {
        DrawMove(players[currentPlayerIndex]);
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 currentPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int cellPos = (Vector2Int)_tilemap.WorldToCell(currentPos);
            if (Map.cellMap.ContainsKey(cellPos) && _tilemap.GetColor((Vector3Int) cellPos) == turnColor)
                MakeMove(cellPos,players[currentPlayerIndex]);
        }
    }

    public void DrawMove(Player player)
    {
        foreach (var pair in Map.cellMap)
        {
            Vector2Int pos = pair.Key;
            _tilemap.SetColor((Vector3Int)pos,Color.white);
            if (!pair.Value.isAvaiable) continue;
            for (int addX = -1; addX <= 1; ++addX)
            {
                for (int addY = -1; addY <= 1; ++addY)
                {
                    if (addX == 0 && addY == 0) continue;
                    Vector2Int neighbourPos = new Vector2Int(pos.x + addX,pos.y + addY);
                    if (!Map.cellMap.ContainsKey(neighbourPos)) continue;
                    FieldCell neighbour = Map.cellMap[neighbourPos];
                    if (neighbour.tilePlayer == player)
                    {
                        _tilemap.SetColor((Vector3Int)pos,turnColor);
                        addX = 2;
                        break;
                    }
                }
            }
        }
    }

    public void MakeMove(Vector2Int pos, Player player)
    {
        Map.PlayerUse(Map.cellMap[pos],player,_tilemap);
        for (int addX = -1; addX <= 1; ++addX)
        {
            for (int addY = -1; addY <= 1; ++addY)
            {
                Vector2Int neighbourPos = new Vector2Int(pos.x + addX,pos.y + addY);
                if (!Map.cellMap.ContainsKey(neighbourPos)) continue;
                FieldCell neighbour = Map.cellMap[neighbourPos];
                neighbour.isAvaiable = Map.IsAvaiable(neighbourPos,_tilemap);
            }
        }
        currentPlayerIndex++;
        currentPlayerIndex %= players.Length;
    }
    
    
}
