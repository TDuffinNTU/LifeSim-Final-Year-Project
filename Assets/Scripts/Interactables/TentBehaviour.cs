public class TentBehaviour : Interactable
{
    public override void onPlayerInteract()
    {
        print("russle russle russle");
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

