using UnityEngine;

public class Slot 
{
    public Tile Tile { get; set; }
    private Tile TileToAddUp { get; set; }
    public bool IsAddingUp { get; set; }

    public void StartAddUp(Tile tileToAddUp)
    {
        IsAddingUp = true;
        TileToAddUp = tileToAddUp;
    }

    public void CheckAddUp()
    {
        if (!IsAddingUp)
            return;

        if (Tile.transform.position == TileToAddUp.transform.position)
        {
            Tile.Value += TileToAddUp.Value;
            GameObject.Destroy(TileToAddUp.gameObject);
            IsAddingUp = false;
        }
    }
}
