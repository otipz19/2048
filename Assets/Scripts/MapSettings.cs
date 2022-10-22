using UnityEngine;

public struct MapSettings
{
    public int Height { get; private set; }
    public int Width { get; private set; }
    public float Margin { get; private set; }
    public Vector2 StartCoordinates { get; private set; }
    public float TilesScale { get; private set; }

    public MapSettings(int width, int height, float margin, float tilesScale)
    {
        Height = height;
        Width = width;
        Margin = margin + tilesScale;
        TilesScale = tilesScale;
        StartCoordinates = new Vector2(-2.66f + (Margin - TilesScale) + TilesScale / 2, 2.66f - ((Margin - TilesScale) + TilesScale / 2));
    }

    public override string ToString()
    {
        return $"Map{Width}x{Height}";
    }
}