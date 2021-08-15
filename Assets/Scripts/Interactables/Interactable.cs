using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// base class for items in LifeSim that can be interacted with
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    //public Sprite Icon;
    public string Name;
    

    /// <summary>
    /// Queries the database with the name of the interactable (set in inspector)
    /// </summary>
    /// <param name="db">database object to query</param>
    /// <returns></returns>
    public string GetTranslation() 
    {
        if (Name != null && Name != "") 
        {
            // far too verbose but we'll live for now
            return GameObject.Find("GameController").GetComponent<DatabaseController>().db.TranslationLookup(Name, "CY");
        }

        return "N/A";
    }

    /// <summary>
    /// Called when player interacts with an object
    /// </summary>
    public virtual void onPlayerInteract() { throw new System.NotImplementedException(); }
    protected bool canInteract = false;
    
    /// <summary>
    /// Called when player picks up an object
    /// </summary>
    public virtual void onPlayerPickup() { throw new System.NotImplementedException(); }
    protected bool canPickup = false;

    /// <summary>
    /// Called when player presses place button
    /// </summary>
    public virtual void onPlayerPlace() { throw new System.NotImplementedException(); }
    protected bool canPlace = false;

    /// <summary>
    /// Called when player presses rotate left button
    /// </summary>
    public virtual void onPlayerRotateLeft() { throw new System.NotImplementedException(); }
    protected bool canRotate = false;

    /// <summary>
    /// Called when player presses rotate left button
    /// </summary>
    public virtual void onPlayerRotateRight() { throw new System.NotImplementedException(); }


}

