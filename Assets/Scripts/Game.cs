using UnityEngine;

public enum MoveDirection
{
    Up,
    Down,
    Right,
    Left,
    Idle,
}

public class Game : MonoBehaviour
{
    public static Game S { get; private set; }

    public Map Map { get; private set; }

    [SerializeField]
    private GameObject tilePrefab;

    public GameObject TilePrefab => tilePrefab;

    //This should be put into player's settings then
    public readonly MapSettings Map4X4 = new MapSettings(4, 4, new Vector2(-2, 2), 1.33f);
    private void Awake()
    {
        S = this;
        Map = new Map(Map4X4);
    }

    private void Start()
    {
        Map.InitializeTiles();
    }

    private void Update()
    {
        MoveDirection moveDirection = GetMoveDirection();
        if (moveDirection == MoveDirection.Idle)
            return;
        Map.MoveTiles(moveDirection);
        
    }

    private MoveDirection GetMoveDirection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            return MoveDirection.Up;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            return MoveDirection.Down;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            return MoveDirection.Right;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            return MoveDirection.Left;

        return MoveDirection.Idle;
    }
}
