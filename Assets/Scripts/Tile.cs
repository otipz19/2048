using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro valueUI;

    [SerializeField]
    private SpriteRenderer backgroundRenderer;

    public bool IsMoving { get; private set; }
    private Vector2 startPos;
    private Vector2 targetPos;
    private float moveDuration;
    private float moveStartTime;

    public Slot SlotToResponse { get; set; }

    public bool IsAppearing { get; private set; }
    private Vector3 startScale;
    public Vector3 MaxScale { get; set; }
    private float appearDuration;
    private float appearStartTime;

    public int Value
    {
        get { return int.Parse(valueUI.text); }
        set 
        { 
            valueUI.text = value.ToString();
            SetColor(value);
            if (value == 2048)
                Game.S.GameWon();
        }
    }

    private void Awake()
    {
        StartAppear(Vector3.zero, 0.15f);
    }

    private void Update()
    {
        if (IsMoving)
        {
            Move();
        }
        if (IsAppearing)
        {
            Appear();
        }
    }

    public void Place(Vector2 sceneCoords)
    {
        transform.position = sceneCoords;
    }

    public void StartMove(Vector2 targetPos, float speed)
    {
        if (Mathf.Approximately(0, Vector3.Distance(targetPos, transform.position)))
            return;

        Game.S.Map.TileStartedMoving();
        IsMoving = true;
        this.targetPos = targetPos;
        startPos = transform.position;
        moveDuration = Vector3.Distance(targetPos, transform.position) / speed;
        moveStartTime = Time.time;
    }

    private void Move()
    {
        float u = EaseInterpolation((Time.time - moveStartTime) / moveDuration);
        if (u >= 1)
        {
            u = 1;
            IsMoving = false;
            Game.S.Map.TileStoppedMoving();
        }
        transform.position = Vector2.Lerp(startPos, targetPos, u);
        if (u == 1 && SlotToResponse != null)
        {
            SlotToResponse.CheckAddUp();
            PlayerScore.S.Score += Value * 2;
            Destroy(this.gameObject);
        }
    }

    private void StartAppear(Vector3 startScale, float duration)
    {
        IsAppearing = true;
        this.startScale = startScale;
        appearStartTime = Time.time;
        appearDuration = duration;
    }

    private void Appear()
    {
        float u = EaseInterpolation((Time.time - appearStartTime) / appearDuration);
        if(u >= 1)
        {
            u = 1;
            IsAppearing = false;
        }
        transform.localScale = Vector3.Lerp(startScale, MaxScale, u);
    }

    private float EaseInterpolation(float u)
    {
        return 1 - Mathf.Pow(1 - u, 3);
    }

    private void SetColor(int value)
    {
        int index = (int)Mathf.Log(value, 2) - 1;
        backgroundRenderer.color = Game.S.TileColors[index % Game.S.TileColors.Length];
        valueUI.color = index < 2 ? new Color(115f / 255, 106f / 255, 100f / 255, 1f) : Color.white;
    }
}