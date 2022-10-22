using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SerializableMap
{
    public int[,] tilesValues;
    public int playerScore;
    public SerializableMap() { }
    public SerializableMap(SerializableMap copyFrom)
    {
        tilesValues = new int[copyFrom.tilesValues.GetLength(0), copyFrom.tilesValues.GetLength(1)];
        for (int i = 0; i < copyFrom.tilesValues.GetLength(0); i++)
            for (int j = 0; j < copyFrom.tilesValues.GetLength(1); j++)
                this.tilesValues[i, j] = copyFrom.tilesValues[i, j];
    }
}

public class Map
{
    private Transform tilesParent;
    [SerializeField]
    private Slot[,] slots;
    public MapSettings MapSettings { get; private set; }

    [SerializeField]
    private int tilesMovingCount;
    private bool tilesStartedMoving;

    private readonly string savePath;
    private bool gameHasEnded;
    private SerializableMap curTurnSave;
    private SerializableMap prevTurnSave;

    public bool SaveLoadedSuccesfully { get; private set; }

    public float TilesYOffset { get; set; }

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
        savePath = Application.persistentDataPath + "/MapData/" + MapSettings.ToString() + ".dat";
    }

    public void Start()
    {
        tilesParent = new GameObject("TilesParent").transform;
        Transform emptySlotsParent = new GameObject("EmptySlotsParent").transform;
        //Rotated intentionally
        slots = new Slot[MapSettings.Width, MapSettings.Height];
        for (int i = 0; i < MapSettings.Width; i++)
            for (int j = 0; j < MapSettings.Height; j++)
            {
                slots[i, j] = new Slot();

                GameObject emptySlot = GameObject.Instantiate(Game.S.EmptySlotPrefab);
                emptySlot.transform.localScale = Vector3.one * MapSettings.TilesScale;
                emptySlot.transform.position = MapToScenePos(new Vector2(i, j));
                emptySlot.transform.parent = emptySlotsParent;
            }

        LoadSaveFile();
        if (!SaveLoadedSuccesfully)
            PlayerScore.S.Score = 0;
    }

    public void SerializeMapData()
    {
        if (!gameHasEnded && curTurnSave != null)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/MapData"))
                Directory.CreateDirectory(Application.persistentDataPath + "/MapData");
            FileStream file = File.Create(savePath);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(file, curTurnSave);
            file.Close();
        }
    }

    public void SaveMapData()
    {
        if(curTurnSave != null)
            prevTurnSave = new SerializableMap(curTurnSave);
        curTurnSave = new SerializableMap();

        curTurnSave.playerScore = PlayerScore.S.Score;

        curTurnSave.tilesValues = new int[MapSettings.Width, MapSettings.Height];
        for (int i = 0; i < MapSettings.Width; i++)
            for (int j = 0; j < MapSettings.Height; j++)
            {
                if (slots[i, j].Tile != null)
                    curTurnSave.tilesValues[i, j] = slots[i, j].Tile.Value;
                else
                    curTurnSave.tilesValues[i, j] = -1;
            }
    }

    private void LoadSaveFile()
    {
        if (File.Exists(savePath))
        {
            FileStream file = File.Open(savePath, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            SerializableMap serializableMap = (SerializableMap)binaryFormatter.Deserialize(file);
            file.Close();
            LoadMapData(serializableMap);
            SaveLoadedSuccesfully = true;
        }
        else
        {
            SaveLoadedSuccesfully = false;
        }
    }

    private void LoadMapData(SerializableMap serializableMap)
    {
        PlayerScore.S.Score = serializableMap.playerScore;

        for (int i = 0; i < MapSettings.Width; i++)
            for (int j = 0; j < MapSettings.Height; j++)
            {
                //slots[i, j] = new Slot();
                slots[i, j].Tile = null;
                if (serializableMap.tilesValues[i, j] != -1)
                {
                    AddTile(i, j, serializableMap.tilesValues[i, j]);
                }
            }
    }

    public void LoadPreviousTurn()
    {
        if (prevTurnSave != null)
        {
            foreach (Slot slot in slots)
                if (slot.Tile != null)
                    GameObject.Destroy(slot.Tile.gameObject);
            LoadMapData(prevTurnSave);
            SaveMapData();
            prevTurnSave = null;
        }
    }

    /// <summary>
    /// Deletes previous save file and prevents creating of a new one
    /// </summary>
    public void GameHasEnded()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        gameHasEnded = true;
    }

    public Vector2 MapToScenePos(Vector2 mapPos)
    {
        float x = MapSettings.StartCoordinates.x + mapPos.x * MapSettings.Margin;
        float y = MapSettings.StartCoordinates.y + TilesYOffset - mapPos.y * MapSettings.Margin;
        return new Vector2(x, y);
    }

    public void AddTiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            //Getting free slot for new tile
            int x = 0, y = 0;
            while (true)
            {
                x = (int)Random.Range(0, MapSettings.Width);
                y = (int)Random.Range(0, MapSettings.Height);
                if (slots[x, y].Tile == null)
                    break;
            }
            AddTile(x, y, 2);
        }
    }

    private void AddTile(int x, int y, int value)
    {
        GameObject tileGO = GameObject.Instantiate(Game.S.TilePrefab);
        Tile tile = tileGO.GetComponent<Tile>();
        tile.transform.parent = tilesParent;
        tile.MaxScale = Vector3.one * MapSettings.TilesScale;
        tile.Value = value;
        slots[x, y].Tile = tile;
        tile.Place(MapToScenePos(new Vector2(x, y)));
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
            SaveMapData();
            tilesStartedMoving = false;
            return true;
        }
        return false;
    }
}