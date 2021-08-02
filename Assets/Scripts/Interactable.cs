using UnityEngine;

public abstract class Interactable
{
    public readonly bool canCollide;
    public readonly bool canPickup;
    public readonly bool canPlace;
    public readonly GameObject Model;
    public readonly Sprite Icon;
    public readonly string Name;

    public abstract void onAction(PlayerController plr);
    public abstract void onPlace(PlayerController plr);
    public abstract void onPickup(PlayerController plr);
    public abstract void onHold(PlayerController plr);

    public abstract void LoadResources(string name);
    // load resource with resources.load<T>(str path);
   
}
