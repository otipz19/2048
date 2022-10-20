using UnityEngine;

public class Map
{
    private Transform tilesParent;
    [SerializeField]
    private Slot[,] slots;
    public MapSettings MapSettings { get; private set; }

    [SerializeField]
    private int tilesMovingCount;
    private bool tilesStartedMoving;

    public int Count
    {
        get
        {
            int count = 0;
            foreach (Slot slot in slots)
                if (slot.Tile != null)
                    count++;
            return count;
        }
    }

    public Map(MapSettings mapSettings)
    {
        MapSettings = mapSettings;
        //Rotated intentionally
        slots = new Slot[MapSettings.Width, MapSettings.Height];
        for (int i = 0; i < MapSettings.Width; i++)
            for (int j = 0; j < MapSettings.Height; j++)
            {
                slots[i, j] = new Slot();
            }
        tilesParent = new GameObject("TilesParent").transform;
    }

    public Vector2 MapToScenePos(Vector2 mapPos)
    {
        float x = MapSettings.StartCoordinates.x + mapPos.x * MapSettings.Margin;
        float y = MapSettings.StartCoordinates.y - mapPos.y * MapSettings.Margin;
        return new Vector2(x, y);
    }

    public void AddTiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject tileGO = GameObject.Instantiate(Game.S.TilePrefab);
            Tile tile = tileGO.GetComponent<Tile>();
            tile.transform.parent = tilesParent;
            tile.Value = 2;

            //Getting free slot for new tile
            int x = 0, y = 0;
            while (true)
            {
                x = (int)Random.Range(0, MapSettings.Width);
                y = (int)Random.Range(0, MapSettings.Height);
                if (slots[x, y].Tile == null)
                    break;
            }

            slots[x, y].Tile = tile;
            tile.Place(MapToScenePos(new Vector2(x, y)));
        }
    }

    public void MoveTiles(MoveDirection moveDirection)
    {
        switch (moveDirection)
        {
            case MoveDirection.Left:
                {
                    for (int x = 0; x < MapSettings.Width; x++)
                        for (int y = 0; y < MapSettings.Height; y++)
                        {
                            if (slots[x, y].Tile != null)
                                MoveTile(slots[x, y].Tile, new Vector2(x, y), new Vector2(-1, 0));
                        }
                    break;
                }

            case MoveDirection.Right:
                {
                    for (int x = MapSettings.Width - 1; x >= 0; x--)
                        for (int y = 0; y < MapSettings.Height; y++)
                        {
                            if (slots[x, y].Tile != null)
                                MoveTile(slots[x, y].Tile, new Vector2(x, y), new Vector2(1, 0));
                        }

                    break;
                }

            case MoveDirection.Up:
                {
                    for (int y = 0; y < MapSettings.Height; y++)
                        for (int x = 0; x < MapSettings.Width; x++)
                        {
                            if (slots[x, y].Tile != null)
                                MoveTile(slots[x, y].Tile, new Vector2(x, y), new Vector2(0, -1));
                        }

                    break;
                }

            case MoveDirection.Down:
                {
                    for (int y = MapSettings.Height - 1; y >= 0; y--)
                        for (int x = 0; x < MapSettings.Width; x++)
                        {
                            if (slots[x, y].Tile != null)
                                MoveTile(slots[x, y].Tile, new Vector2(x, y), new Vector2(0, 1));
                        }

                    break;
                }
        }
        tilesStartedMoving = tilesMovingCount > 0;
    }

    private void MoveTile(Tile tile, Vector2 startPos, Vector2 coordsOffset)
    {
        Vector2 tracedPos = startPos;
        while (true)
        {
            Vector2 nextPos = tracedPos + coordsOffset;
            if (nextPos.x >= MapSettings.Width || nextPos.x < 0 || nextPos.y >= MapSettings.Height || nextPos.y < 0)
            {
                Move(tile, startPos, tracedPos);
                return;
            }
            else if(slots[(int)nextPos.x, (int)nextPos.y].Tile == null)
            {
                tracedPos = nextPos;
            } 
            else
            {
                if (slots[(int)nextPos.x, (int)nextPos.y].Tile.Value == tile.Value && !slots[(int)nextPos.x, (int)nextPos.y].IsAddingUp)
                {
                    tracedPos = nextPos;
                    slots[(int)startPos.x, (int)startPos.y].Tile = null;
                    slots[(int)tracedPos.x, (int)tracedPos.y].StartAddUp(tile);
                    tile.StartMove(MapToScenePos(tracedPos), Game.TileSpeed);
                    return;
                }
                else
                {
                    Move(tile, startPos, tracedPos);
                    return;
                }
                
            }
        }

        void Move(Tile tile, Vector2 startPos, Vector2 tracedPos)
        {
            slots[(int)startPos.x, (int)startPos.y].Tile = null;
            slots[(int)tracedPos.x, (int)tracedPos.y].Tile = tile;
            tile.StartMove(MapToScenePos(tracedPos), Game.TileSpeed);
        }
    }

    public void TileStartedMoving()
    {
        tilesMovingCount++;
    }

    public void TileStoppedMoving()
    {
        tilesMovingCount--;
    }

    public bool CheckAllTilesStoppedMoving()
    {
        if (!tilesStartedMoving)
            return true;
        if(tilesMovingCount == 0)
        {
            AddTiles(1);
            tilesStartedMoving = false;
            return true;
        }
        return false;
    }
}