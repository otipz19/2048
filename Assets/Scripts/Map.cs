using UnityEngine;

public class Map
{
    private Transform tilesParent;

    public Slot[,] Slots { get; private set; }
    public MapSettings MapSettings { get; private set; }

    public Map(MapSettings mapSettings)
    {
        MapSettings = mapSettings;
        //Rotated intentionally
        Slots = new Slot[MapSettings.Width, MapSettings.Height];
        for (int i = 0; i < MapSettings.Width; i++)
            for (int j = 0; j < MapSettings.Height; j++)
            {
                Slots[i, j] = new Slot();
            }
        tilesParent = new GameObject("TilesParent").transform;
    }

    public void InitializeTiles()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject tileGO = GameObject.Instantiate(Game.S.TilePrefab);
            Tile tile = tileGO.GetComponent<Tile>();
            tile.transform.parent = tilesParent;
            tile.Value = 2;

            //Getting free slot for new tile
            int x, y;
            while (true)
            {
                x = (int)Random.Range(0, MapSettings.Width);
                y = (int)Random.Range(0, MapSettings.Height);
                if(Slots[x, y].Tile == null)
                    break;
            }

            Slots[x, y].Tile = tile;
            Vector2 mapCoords = new Vector2(x, y);
            tile.Place(mapCoords);
        }
    }

    public Vector2 MapToSceneCoords(Vector2 mapCoords)
    {
        float x = MapSettings.StartCoordinates.x + mapCoords.x * MapSettings.Margin;
        float y = MapSettings.StartCoordinates.y - mapCoords.y * MapSettings.Margin;
        return new Vector2(x, y);
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
                            if (Slots[x, y].Tile != null)
                                Slots[x, y].Tile.Move(moveDirection);
                        }

                    break;
                }

            case MoveDirection.Right:
                {
                    for (int x = MapSettings.Width - 1; x >= 0; x--)
                        for (int y = 0; y < MapSettings.Height; y++)
                        {
                            if (Slots[x, y].Tile != null)
                                Slots[x, y].Tile.Move(moveDirection);
                        }

                    break;
                }

            case MoveDirection.Up:
                {
                    for (int y = 0; y < MapSettings.Height; y++)
                        for (int x = 0; x < MapSettings.Width; x++)
                        {
                            if (Slots[x, y].Tile != null)
                                Slots[x, y].Tile.Move(moveDirection);
                        }

                    break;
                }

            case MoveDirection.Down:
                {
                    for (int y = MapSettings.Height - 1; y >= 0; y--)
                        for (int x = 0; x < MapSettings.Width; x++)
                        {
                            if (Slots[x, y].Tile != null)
                                Slots[x, y].Tile.Move(moveDirection);
                        }

                    break;
                }
        }
    }

    public void SlotsCheckAddUp()
    {
        for (int i = 0; i < MapSettings.Width; i++)
            for (int j = 0; j < MapSettings.Height; j++)
                Slots[i, j].CheckAddUp();
    }
}
