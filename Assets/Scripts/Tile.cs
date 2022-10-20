using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro ValueUI;

    public bool IsMoving { get; private set; }
    private Vector2 startPos;
    private Vector2 targetPos;
    private float moveDuration;
    private float moveStartTime;

    public Slot SlotToResponse { get; set; }

    public bool IsAppearing { get; private set; }
    private Vector3 startScale;
    private Vector3 targetScale;
    private float appearDuration;
    private float appearStartTime;

    public int Value
    {
        get { return int.Parse(ValueUI.text); }
        set { ValueUI.text = value.ToString(); }
    }

    private void Awake()
    {
        StartAppear(Vector3.zero, Vector3.one, 0.2f);
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
            Destroy(this.gameObject);
        }
    }

    private void StartAppear(Vector3 startScale, Vector3 targetScale, float duration)
    {
        IsAppearing = true;
        this.startScale = startScale;
        this.targetScale = targetScale;
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
        transform.localScale = Vector3.Lerp(startScale, targetScale, u);
    }

    private float EaseInterpolation(float u)
    {
        return 1 - Mathf.Pow(1 - u, 3);
    }
}