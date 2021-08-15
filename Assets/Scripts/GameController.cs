using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private GameObject NearInteractablePrompt;
    private GameObject Player;
    private GameObject DialogBox;
    private GameObject CanvasObj;
    private GameObject Shop;
    private GameObject ShopMenu;
    private GameObject Clock;
    private GameObject HelpMenu;
    private GameObject PauseMenu;
    private Database DB;
    private GroundGenerator GG;

    // Start is called before the first frame update
    void Start()
    {
        //return;
        DB = gameObject.GetComponent<DatabaseController>().LoadDB();
        CanvasObj = GameObject.Find("Canvas");

        NearInteractablePrompt = GameObject.Find("Canvas/PROMPT");      
        
        ShopMenu = GameObject.Find("Canvas/SHOPMENU");
        ShopMenu.SetActive(false);

        GG = GameObject.Find("GroundGenerator").GetComponent<GroundGenerator>();
        GG.NewMap();

        Player = GameObject.Find("Player");
        Shop = GameObject.FindObjectOfType<ShopBehaviour>().gameObject;
        Shop.GetComponent<ShopBehaviour>().ShuffleStock();

        Clock = GameObject.Find("Canvas/CLOCK");
        
        HelpMenu= GameObject.Find("Canvas/HELPMENU");
        HelpMenu.SetActive(false);

        PauseMenu = GameObject.Find("Canvas/PAUSEMENU");
        PauseMenu.SetActive(false);

    }

    /// <summary>
    /// Non-player related inputs (ie. ui input)
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) 
        {
            ToggleHelp();
        }

        if (Input.GetKeyDown(KeyCode.Escape))         
        {                        
            TogglePause();
        }
    }

    /// <summary>
    /// Kill the app
    /// </summary>
    public void QuitGame() 
    {
        Application.Quit();
    }

    /// <summary>
    /// Returns to main menu
    /// </summary>
    public void MainMenu() 
    {
        // todo -- load menu menu
    }

    /// <summary>
    /// toggles the pause menu for the player
    /// </summary>
    public void TogglePause() 
    {
        Time.timeScale = Time.timeScale == 1f ? 0f : 1f;
        PauseMenu.SetActive(!PauseMenu.activeInHierarchy);
    }

    /// <summary>
    /// Toggles the help menu for the player
    /// </summary>
    private void ToggleHelp() 
    {
        HelpMenu.SetActive(!HelpMenu.activeInHierarchy);
    }

    /// <summary>
    /// update friendship values depending on response chosen
    /// </summary>
    /// <param name="index"></param>
    public void ResponseClickedListener(int index)
    {
        // update db
    }

    /// <summary>
    /// Triggered when player interacts with an NPC
    /// </summary>
    /// <param name="name"></param>
    public void DialogStartListener(string name, int attitude)
    {
        //print($"starting dialog with {name}...");
        DialogBox = Instantiate(Resources.Load("Prefabs/UI/NPCDialog", typeof(GameObject)) as GameObject);
        DialogBox.transform.SetParent(CanvasObj.transform);
        DialogBox.GetComponent<RectTransform>().localPosition = new Vector3(0, -98, 0);

        string[] attitudemap = { "NICE", "NEUTRAL", "MEAN" };
        //string[] timemap = { "MORNING", "AFTERNOON", "NIGHT" };

        int variant = Random.Range(1, 4);
        string time = Clock.GetComponent<ClockController>().GetTimeOfDay();

        var phrase = DB.TranslationLookup($"GREETING_{attitudemap[attitude - 1]}_{variant}_{time}", "CY");
        var phraselist = new List<String>() { phrase };
        var resplist = new List<String>();

        foreach (var a in attitudemap)
        {
            resplist.Add(DB.TranslationLookup($"RESPONSE_{a}_1_{time}", "CY"));
        }

        DialogBox.GetComponent<DialogBoxController>().Init(phraselist, resplist, name);
        DialogBox.GetComponent<DialogBoxController>().enabled = true;
        SetMoveablesActive(false);
    }

    /// <summary>
    /// close the dialog and get mobiles moving again!
    /// </summary>
    private void ShopClosedDialog() 
    {
        DialogBox = Instantiate(Resources.Load("Prefabs/UI/NPCDialog", typeof(GameObject)) as GameObject);
        DialogBox.transform.SetParent(CanvasObj.transform);
        DialogBox.GetComponent<RectTransform>().localPosition = new Vector3(0, -98, 0);

        DialogBox.GetComponent<DialogBoxController>().Init(new List<string>() 
        { DB.TranslationLookup("SHOP_CLOSED", "CY") }, new List<string>(), name);

        DialogBox.GetComponent<DialogBoxController>().enabled = true;
        SetMoveablesActive(false);
    }

    /// <summary>
    /// Sets the selected inventory slot darker to represent "selected"
    /// </summary>
    /// <param name="inv"></param>
    public void UpdateInventoryIcons(List<Interactable> inv)
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag("InventorySlot");

        //foreach (var x in slots) { print(slots.ToString()); }
        int count = 0;
        foreach (var i in inv)
        {
            slots[count].GetComponentInChildren<TextMeshProUGUI>().text = i.GetTranslation();
            count++;
        }

        for (int c = count; c < slots.Length; c++)
        {
            slots[c].GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    /// <summary>
    /// set the text for the nearby prompt
    /// </summary>
    /// <param name="t">text representing the nearby object</param>
    public void SetPromptText(Interactable i) 
    {
        var t = i != null ? i.GetTranslation() : "Nothing...";        
        NearInteractablePrompt.GetComponent<TextMeshProUGUI>().text = $"<b>Nearby:</b>\n{t}";
    }

    /// <summary>
    /// called by Player when an inv button is selected
    /// </summary>
    /// <param name="index"></param>
    public void HighlightInventorySlot(int index)
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag("InventorySlot");
        int count = 0;
        foreach (var s in slots)
        {
            var b = s.GetComponent<Button>();
            ColorBlock cb = b.colors;
            cb.normalColor = count == index ? Color.grey : Color.white;
            b.colors = cb;
            count++;
        }
    }

    /// <summary>
    /// Called when the dialog with an NPC etc. is complete.
    /// </summary>
    public void DialogFinishListener()
    {
        SetMoveablesActive(true);
    }

    /// <summary>
    /// Set active/inactive state of mobiles
    /// </summary>
    /// <param name="active"></param>
    public void SetMoveablesActive(bool active) 
    {
        // set active state of mobiles
        var lst = GameObject.FindObjectsOfType<NonPlayerController>();
        foreach (var npc in lst)
        {
            npc.enabled = active;
        }

        Player.GetComponent<PlayerController>().enabled = active;
    }

    public void RefreshStock() 
    {
        Shop.GetComponent<ShopBehaviour>().ShuffleStock();
    }

    public void OpenShopMenu(List<ITEMS_MAP> stock) 
    {
        if (Clock.GetComponent<ClockController>().GetTimeOfDay() == "NIGHT") 
        {
            ShopClosedDialog();
            return;
        }

        ShopMenu.SetActive(true);
        SetMoveablesActive(false);

        GameObject[] slots = GameObject.FindGameObjectsWithTag("ShopInventorySlot");
        int count = 0;
        foreach (var i in stock)
        {
            var translation = DB.TranslationLookup(i.Name, "CY");
            slots[count].GetComponentInChildren<TextMeshProUGUI>().text = $"{translation} ({i.Cost})";
            count++;
        }

        for (int c = count; c < slots.Length; c++)
        {
            slots[c].GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        GameObject.FindGameObjectWithTag("Wallet").GetComponent<TextMeshProUGUI>().text = "$" + Player.GetComponent<PlayerController>().Cash.ToString();
    }
    
    
    public void CloseShopMenu() 
    {
        ShopMenu.SetActive(false);
        SetMoveablesActive(true);
    }

    public void BuyItem(int index) 
    {
        var item = Shop.GetComponent<ShopBehaviour>().Stock[index];
        Player.GetComponent<PlayerController>().BuyItem(item);
        GameObject.FindGameObjectWithTag("Wallet").GetComponent<TextMeshProUGUI>().text = "$" + Player.GetComponent<PlayerController>().Cash.ToString();

    }

    public void SellItem(int index) 
    {
        try
        {
            Player.GetComponent<PlayerController>().SellItem(index);
            GameObject.FindGameObjectWithTag("Wallet").GetComponent<TextMeshProUGUI>().text = "$" + Player.GetComponent<PlayerController>().Cash.ToString();
        }
        catch (Exception)
        {
            // do nothing
        }
    }
    

}
