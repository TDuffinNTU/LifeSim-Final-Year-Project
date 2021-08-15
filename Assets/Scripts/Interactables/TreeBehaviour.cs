public class TreeBehaviour : Interactable
{
    public override void onPlayerInteract()
    {
        print("shake shake shake!");
    }

    public override void onPlayerPickup()
    {
        return;
    }

    public override void onPlayerPlace()
    {
        Destroy(this);
    }
}
