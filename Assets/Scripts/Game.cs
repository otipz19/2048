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
    [SerializeField]
    private GameObject emptySlotPrefab;
    public GameObject EmptySlotPrefab => emptySlotPrefab;

    public Color[] TileColors;

    public const float TileSpeed = 15f;

    private bool isInputBlocked;

    private Vector2 touchStartPos;

    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameObject[] buttons;
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject[] scoreLabels;
    [SerializeField]
    private GameObject gameWonBox;

    private void Awake()
    {
        S = this;
        Map = new Map(Settings.SelectedMapSettings);
    }

    private void Start()
    {
        if(Map.MapSettings.Width != Map.MapSettings.Height)
        {
            Vector3 backgroundScale = background.transform.localScale;
            backgroundScale.y = Map.MapSettings.TilesScale * Map.MapSettings.Height + (Map.MapSettings.Margin - Map.MapSettings.TilesScale) * (Map.MapSettings.Height + 1);
            background.transform.localScale = backgroundScale;

            title.SetActive(false);

            float yOffset = (backgroundScale.y - backgroundScale.x) / 2;
            foreach (GameObject button in buttons)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(button.transform.position);
                pos.y += yOffset;
                button.transform.position = Camera.main.WorldToScreenPoint(pos);
            }

            scoreLabels[0].transform.localPosition = new Vector3(-90, -220, 0);
            scoreLabels[1].transform.localPosition = new Vector3(85, -220, 0);

            Map.TilesYOffset = yOffset;
        }

        Map.Start();

        if(!Map.SaveLoadedSuccesfully)
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
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Map.GameHasEnded();
        PlayerScore.S.UpdateRecord();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        Map.SerializeMapData();
    }

    public void OnMenuClicked()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnBackClicked()
    {
        if(!isInputBlocked)
            Map.LoadPreviousTurn();
    }

    public void OnRestartClicked()
    {
        EndGame();
    }

    public void GameWon()
    {
        gameWonBox.SetActive(true);
        gameWonBox.transform.localScale = background.transform.localScale;
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