using System.Collections.Generic;
using UnityEngine;

public class ShopBehaviour : Interactable
{
    private readonly int MAX_ITEMS = 5;
    public List<ITEMS_MAP> Stock = new List<ITEMS_MAP>();   

    /// <summary>
    /// Calls OpenShopMenu() in GC class
    /// </summary>
    public override void onPlayerInteract()
    {
        ShuffleStock();
        GameObject.Find("GameController").GetComponent<GameController>()
            .OpenShopMenu(Stock);
    }

    public override void onPlayerPickup()
    {
        return;
    }

    public override void onPlayerPlace()
    {
        Destroy(this);
    }

    /// <summary>
    /// generate a new selection of stock
    /// </summary>
    public void ShuffleStock() 
    {        
        Stock.Clear();

        Stock = GameObject.Find("GameController").GetComponent<DatabaseController>()
            .db.GetRandomShopItems(MAX_ITEMS);  
    }
}
