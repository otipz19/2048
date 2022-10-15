using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector2 mapCoords;
    [SerializeField]
    private TextMeshPro ValueUI;
    private bool isAddingUp;

    public int Value
    {
        get { return int.Parse(ValueUI.text); }
        set { ValueUI.text = value.ToString(); }
    }

    public void Place(Vector2 mapCoords)
    {
        this.mapCoords = mapCoords;
        Vector2 sceneCoords = Game.S.Map.MapToSceneCoords(mapCoords);
        transform.position = sceneCoords;
    }

    public void Move(MoveDirection moveDirection)
    {
        Vector2 offsetCoords = Vector2.zero;
        switch (moveDirection)
        {
            case MoveDirection.Up:
                offsetCoords = new Vector2(0, -1);
                break;
            case MoveDirection.Down:
                offsetCoords = new Vector2(0, 1);
                break;
            case MoveDirection.Left:
                offsetCoords = new Vector2(-1, 0);
                break;
            case MoveDirection.Right:
                offsetCoords = new Vector2(1, 0);
                break;
        }
        Vector2 tracedMapCoords = TraceDirection(offsetCoords);
        if (!isAddingUp)
        {
            Game.S.Map.Slots[(int)mapCoords.x, (int)mapCoords.y].Tile = null;
            mapCoords = tracedMapCoords;
            Game.S.Map.Slots[(int)mapCoords.x, (int)mapCoords.y].Tile = this;
        }
        Place(mapCoords = tracedMapCoords);
    }
    
    private Vector2 TraceDirection(Vector2 offset)
    {
        Vector2 tracedCoords = mapCoords;
        while(true)
        {
            Vector2 newCoords = tracedCoords + offset;
            if (newCoords.x < Game.S.Map.MapSettings.Width && newCoords.x >= 0 &&
                newCoords.y < Game.S.Map.MapSettings.Height && newCoords.y >= 0)
            {
                if(Game.S.Map.Slots[(int)newCoords.x, (int)newCoords.y].Tile == null)
                {
                    tracedCoords = newCoords;
                }
                else if (Game.S.Map.Slots[(int)newCoords.x, (int)newCoords.y].Tile.Value == this.Value &&
                         !Game.S.Map.Slots[(int)newCoords.x, (int)newCoords.y].IsAddingUp)
                {
                    tracedCoords = newCoords;
                    Game.S.Map.Slots[(int)newCoords.x, (int)newCoords.y].StartAddUp(this);
                    isAddingUp = true;
                    return tracedCoords;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            } 
        }
        return tracedCoords;
    }
}
