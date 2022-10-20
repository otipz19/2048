using UnityEngine;

public class Slot 
{
    public Tile Tile { get; set; }
    [SerializeField]
    private Tile tileToAddUp;
    public bool IsAddingUp { get; set; }

    public void StartAddUp(Tile tileToAddUp)
    {
        IsAddingUp = true;
        this.tileToAddUp = tileToAddUp;
        tileToAddUp.SlotToResponse = this;
    }

    public void CheckAddUp()
    {
        Tile.Value += tileToAddUp.Value;
        IsAddingUp = false;
    }
}