using UnityEngine;

/// <summary>
/// Derived class for all furniture-type items
/// </summary>
public class Furniture : Interactable
{

    private readonly float PLACE_DISTANCE = 0.5f;
    private readonly float PLACE_ANGLE = 180;
    private readonly float ROTATE_ANGLE = 15f;
    //public int Value = 0;

    public void Start()
    {
        canPickup = true;
        canPlace = true;
        canRotate = true;
        canInteract = false;
    }

    /// <summary>
    /// Can't be interacted with
    /// </summary>
    public override void onPlayerInteract()
    {        
        //print($"Furniture of name {Name}...");
    }

    /// <summary>
    /// Remove visible stuff from map and adds itself to inventory of player
    /// </summary>
    public override void onPlayerPickup()
    {
        var col = gameObject.GetComponents<Collider>();
        foreach (var c in col) { c.enabled = false; }

        var mr = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var m in mr) { m.enabled = false; }

        GameObject.Find("Player").GetComponent<PlayerController>().AddToInventory(this);
    }


    /// <summary>
    /// Re enable mesh and colliders, while rotating towards the player
    /// </summary>
    public override void onPlayerPlace()
    {
        var plr = GameObject.Find("Player").GetComponent<PlayerController>();

        var fwrd = plr.transform.forward;
        var fwrd_normalized = fwrd.normalized;

        var dest = plr.transform.position + (fwrd_normalized * PLACE_DISTANCE);
        dest = new Vector3(dest.x, 0, dest.z);      
        
        transform.position = dest;
        transform.rotation = Quaternion.Euler(0, plr.transform.rotation.eulerAngles.y - PLACE_ANGLE, 0);
        
        var col = gameObject.GetComponents<Collider>();
        foreach (var c in col) { c.enabled = true; }               

        var mr = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var m in mr) { m.enabled = true; }

        //print($"Placing {Name}...");

        plr.RemoveFromInventory(this);
    }

    /// <summary>
    /// Rotate by ROTATE_ANGLE degs Left
    /// </summary>
    public override void onPlayerRotateLeft()
    {
        var rot = transform.rotation.eulerAngles.y - ROTATE_ANGLE;
        transform.rotation = Quaternion.Euler(0,rot, 0);
    }

    /// <summary>
    /// Rotates right
    /// </summary>
    public override void onPlayerRotateRight()
    {
        var rot = transform.rotation.eulerAngles.y + ROTATE_ANGLE;
        transform.rotation = Quaternion.Euler(0, rot, 0);
    }
}

