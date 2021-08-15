using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the Player's movement and behaviours
/// </summary>
public class PlayerController : MonoBehaviour
{
    private GameController GC;
    private Rigidbody RB;

    private float MAX_SPEED = 3f;  
    private float ACCELERATION = 8f;

    [SerializeField]
    private float CurrentSpeed = 0f;  
    
    private GameObject Camera;    
    private Vector3 CameraRelPosition;

    [SerializeField]
    private GameObject ObjectNearPlayer;

    private int InventorySlot = 0;
    public int Cash;

    // We can shove sub-classes into inv and get their derived out the other end
    [SerializeField]
    private List<Interactable> Inventory;
    private readonly int MAX_INVENTORY_SLOTS = 5;

    //[SerializeField]
    float xInput, zInput = 0f;

    
    /// <summary>
    /// Player setup
    /// </summary>
    void Start()
    {
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        RB = gameObject.GetComponent<Rigidbody>();
        
        
        Inventory = new List<Interactable>(MAX_INVENTORY_SLOTS);       
        

        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        
        CameraRelPosition = transform.position - Camera.transform.position;
    }

    /// <summary>
    /// For debugging
    /// </summary>
    private void PrintInv() 
    {
        string s = "";
        foreach (var item in Inventory) { s += $"{item.Name}, "; }
        print($"Inventory: {s}");
    }

    /// <summary>
    /// Add item to inventory
    /// </summary>
    /// <param name="i"></param>
    public void AddToInventory(Interactable i) 
    {        
        Inventory.Add(i);
        ObjectNearPlayer = null;
        GC.UpdateInventoryIcons(Inventory);
        
    }

    /// <summary>
    /// Remove item from inventory
    /// </summary>
    /// <param name="i"></param>
    public void RemoveFromInventory(Interactable i)
    {
        Inventory.Remove(i);
        GC.UpdateInventoryIcons(Inventory);        
    }

    /// <summary>
    /// Handles user input
    /// </summary>
    void UserInput() 
    {
        //print("gg");
        // Space to interact:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            try
            {
                var interactable = ObjectNearPlayer.GetComponent<Interactable>();
                interactable.onPlayerInteract();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        // F to pickup
        if (Input.GetKeyDown(KeyCode.F))
        {
            try
            {
                if (Inventory.Capacity != Inventory.Count)
                {
                    ObjectNearPlayer.GetComponent<Interactable>().onPlayerPickup();                    
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        // G to place
        if (Input.GetKeyDown(KeyCode.G))
        {
            try
            {
                Inventory[InventorySlot].onPlayerPlace();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        // Q and E to rotate
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            try
            {
                ObjectNearPlayer.GetComponent<Interactable>().onPlayerRotateLeft();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        // Q and E to rotate
        if (Input.GetKeyDown(KeyCode.E))
        {
            try
            {
                ObjectNearPlayer.GetComponent<Interactable>().onPlayerRotateRight();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        // movement inputs
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// Player updates
    /// </summary>
    void Update()
    {
        UserInput();

        Vector3 TargetDir = new Vector3(xInput, 0, zInput).normalized;

        if (TargetDir != Vector3.zero)
        {
            RB.rotation = Quaternion.LookRotation(TargetDir);
            CurrentSpeed += (ACCELERATION * Time.deltaTime);
            CurrentSpeed = Mathf.Min(MAX_SPEED, CurrentSpeed);
            RB.velocity = (TargetDir * CurrentSpeed);
        }
        else 
        {
            CurrentSpeed -= (ACCELERATION * Time.deltaTime);
            CurrentSpeed = Mathf.Max(CurrentSpeed, 0f);
            RB.velocity = (transform.forward * CurrentSpeed);
        }        

        
        RB.angularVelocity = Vector3.zero;

        Camera.transform.position = RB.position - CameraRelPosition;       
    }

    /// <summary>
    /// Called when player ENTERS the trigger zone of map objs
    /// </summary>
    /// <param name="other"> Collider trigger we've left</param>
    private void OnTriggerEnter(Collider other)
    {
        ObjectNearPlayer = (other.gameObject);
        //print($"Nearby: {other.gameObject.GetComponent<Interactable>().Name}");
    }

    /// <summary>
    /// Called when player EXITS the trigger zone of map objs
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        ObjectNearPlayer = (null);
    }      

    /// <summary>
    /// Called when player clicks an inv slot
    /// </summary>
    /// <param name="index"></param>
    public void SetInventorySlot(int index) 
    {
        InventorySlot = index;
        GC.HighlightInventorySlot(index);
    }

    /// <summary>
    /// Remove player's item and reimburse them based on value of item
    /// </summary>
    /// <param name="index">index of item in inventory to remove</param>
    public void SellItem(int index)
    {
        var db = GameObject.Find("GameController").GetComponent<DatabaseController>().db;

        if (Inventory[index] != null) 
        {
            Cash += db.ItemCostLookup(Inventory[index].Name);
            Destroy(Inventory[index]);
            Inventory.RemoveAt(index);
            GC.UpdateInventoryIcons(Inventory);
        }
    }

    /// <summary>
    /// Remove funds from player and add item to inventory (if space allows)
    /// </summary>
    /// <param name="item">data model of the item being purchased</param>
    public void BuyItem(ITEMS_MAP item)
    {
        var db = GameObject.Find("GameController").GetComponent<DatabaseController>().db;
        string PREFAB_PATH = "Prefabs/Furniture/";

        if (Inventory.Count != Inventory.Capacity && item.Cost <= Cash) 
        {
            Cash -= item.Cost;

            var go = Instantiate(Resources.Load(PREFAB_PATH + item.Name, typeof(GameObject)) as GameObject, 
                new Vector3(0, 1, -1000), Quaternion.identity);

            AddToInventory(go.GetComponent<Furniture>());
        }

    }

}
