using UnityEngine;
using UnityEngine.SceneManagement;

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

    public const float TileSpeed = 10f;

    //This should be put into player's settings then
    public readonly MapSettings Map4X4 = new MapSettings(4, 4, new Vector2(-2, 2), 1.33f);

    [SerializeField]
    private bool isInputBlocked;

    private Vector2 touchStartPos;

    private void Awake()
    {
        S = this;
        Map = new Map(Map4X4);
    }

    private void Start()
    {
        Map.AddTiles(2);
    }

    private void Update()
    {
        if (Map.CheckAllTilesStoppedMoving())
            isInputBlocked = false;

        if (isInputBlocked)
            return;

        MoveDirection moveDirection = GetMoveDirection();
        if (moveDirection == MoveDirection.Idle)
            return;

        Map.MoveTiles(moveDirection);
        isInputBlocked = true;
       
        if (Map.Count == Map.MapSettings.Height * Map.MapSettings.Width)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //For desktop
    //private MoveDirection GetMoveDirection()
    //{
    //    if (Input.GetKeyDown(KeyCode.UpArrow))
    //        return MoveDirection.Up;
    //    else if (Input.GetKeyDown(KeyCode.DownArrow))
    //        return MoveDirection.Down;
    //    else if (Input.GetKeyDown(KeyCode.RightArrow))
    //        return MoveDirection.Right;
    //    else if (Input.GetKeyDown(KeyCode.LeftArrow))
    //        return MoveDirection.Left;

    //    return MoveDirection.Idle;
    //}

    //For mobile
    private MoveDirection GetMoveDirection()
    {
        MoveDirection moveDirection = MoveDirection.Idle;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchStartPos = Input.GetTouch(0).position;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector2 swapDirection = Input.GetTouch(0).position - touchStartPos;
            float x = swapDirection.x;
            float y = swapDirection.y;
            switch (x)
            {
                case >= 0 when y >= 0:
                    moveDirection = x >= y ? MoveDirection.Right : MoveDirection.Up;
                    break;
                case < 0 when y >= 0:
                    moveDirection = Mathf.Abs(x) >= y ? MoveDirection.Left : MoveDirection.Up;
                    break;
                case < 0 when y < 0:
                    moveDirection = Mathf.Abs(x) >= Mathf.Abs(y) ? MoveDirection.Left : MoveDirection.Down;
                    break;
                case >= 0 when y < 0:
                    moveDirection = x >= Mathf.Abs(y) ? MoveDirection.Right : MoveDirection.Down;
                    break;
            }
        }

        return moveDirection;
    }
}