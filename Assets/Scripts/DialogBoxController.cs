using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBoxController : MonoBehaviour
{
    private Database db;
    public TextMeshProUGUI Name;    
    public TextMeshProUGUI Dialog;
    public Button BtnNice, BtnNeutral, BtnMean;
    private string TextLeft;

    private List<string> Responses;
    private List<string> Phrases;
    private string Emoji;

    //Timing
    public float TimeBetweenLetters;
    private float UntilNextLetter;


    void Start()
    {
        db = new Database();

        var dialog = db.DialogLookup("GREETING_NICE_1_MORNING");

        // this is how we'll init it later!
        var p = new List<string> { db.TranslationLookup(dialog.Name, "CY") };

        var r = new List<string> { 
            db.TranslationLookup(dialog.ResponseMean, "CY") , 
            db.TranslationLookup(dialog.ResponseNeutral, "CY"), 
            db.TranslationLookup(dialog.ResponseNice, "CY") 
        };

        var n = "Jeremy"; // This will be supplied by NPC
        var e = dialog.Emoji;
        Init(p, r, n, e);
    }

    /// <summary>
    /// Pass parameters into dialogbox.
    /// </summary>
    /// <param name="_Phrases">List of phrases for NPC to display</param>
    /// <param name="_Responses">List of 3 player responses to NPC</param>
    /// <param name="_Name">Name of NPC</param>
    /// <param name="_Emoji">Emoji symbols providing context</param>
    public void Init(List<string> _Phrases, List<string> _Responses, string _Name, string _Emoji) 
    {
        // info from our CSVs
        Name.text = _Name;
        Phrases = _Phrases;
        Responses = _Responses;
        Emoji = _Emoji;

        // Text
        NextPhrase();        
    }

    // Update is called once per frame
    void Update()
    {
        if (!BtnNice.IsActive())
        {
            DisplayNextLetter();
        }
        else 
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {                
                // Destroy self when phrases are used up
                if (Phrases.Count > 0)
                {
                    NextPhrase();
                }
                else 
                {
                    Destroy(gameObject);
                }
            }
        }        
    }

    /// <summary>
    /// Attempts to load next char from the array until its empty
    /// </summary>
    private void DisplayNextLetter() 
    {
        UntilNextLetter -= Time.deltaTime;

        if (UntilNextLetter <= 0f && TextLeft.Length >= 0)
        {
            try
            {
                // too lazy to sort out the index error here
                UntilNextLetter = TimeBetweenLetters;
                Dialog.text += TextLeft[0];
                TextLeft = TextLeft.Substring(1);
            }
            catch 
            {
                if (TextLeft.Length <= 0)
                {
                   
                    BtnNice.gameObject.SetActive(true);
                    BtnNeutral.gameObject.SetActive(true);
                    BtnMean.gameObject.SetActive(true);
                }
            }            
        }
    }

    /// <summary>
    /// Prepares next phrase of dialog
    /// </summary>
    private void NextPhrase() 
    {
        // Pop off front
        TextLeft = Phrases[0];
        Phrases.RemoveAt(0);

        // reset comps
        Dialog.text = "";
       
        BtnNice.gameObject.SetActive(false);
        BtnNeutral.gameObject.SetActive(false);
        BtnMean.gameObject.SetActive(false);

        // Timing
        UntilNextLetter = TimeBetweenLetters;
    }

    /// <summary>
    /// alert game controller of change
    /// </summary>
    /// <param name="response">index of button clicked (1 mean - 3 nice)</param>
    public void OnResponseClicked(int response) 
    {
        var GC = GameObject.Find("GameController").GetComponent<GameController>();
        GC.ResponseClickedListener(response);
    }



}
