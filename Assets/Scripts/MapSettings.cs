using UnityEngine;

public struct MapSettings
{
    public int Height { get; private set; }
    public int Width { get; private set; }
    public float Margin { get; private set; }
    public Vector2 StartCoordinates { get; private set; }

    public MapSettings(int height, int width, Vector2 startCoordinates, float margin)
    {
        Height = height;
        Width = width;
        Margin = margin;
        StartCoordinates = startCoordinates;
    }
}

