using UnityEngine;

static public class Settings
{
    public static MapSettings Map4x4
    {
        get { return new MapSettings(4, 4, 0.118f, 1.182f); }
    }
    public static MapSettings Map5x5
    {
        get { return new MapSettings(5, 5, 0.095f, 0.95f); }
    }
    public static MapSettings Map6x6
    {
        get { return new MapSettings(6, 6, 0.0794f, 0.794f); }
    }
    public static MapSettings Map8x8
    {
        get { return new MapSettings(8, 8, 0.0598f, 0.598f); }
    }

    public static MapSettings Map3x5
    {
        get { return new MapSettings(3, 5, 0.15647f, 1.5647f); }
    }

    public static MapSettings Map4x6
    {
        get { return new MapSettings(4, 6, 0.118f, 1.182f); }
    }

    public static MapSettings Map5x8
    {
        get { return new MapSettings(5, 8, 0.095f, 0.95f); }
    }

    public static MapSettings Map6x9
    {
        get { return new MapSettings(6, 9, 0.0794f, 0.794f); }
    }

    public static MapSettings SelectedMapSettings { get; set; }
}
