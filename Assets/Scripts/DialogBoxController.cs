using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogBoxController : MonoBehaviour
{
    //private Database db;
    private TextMeshProUGUI Name;
    private TextMeshProUGUI Dialog;
    private Button BtnNice, BtnNeutral, BtnMean;
    private string TextLeft;

    private List<string> Responses;
    private List<string> Phrases;
    private string Emoji;

    private bool isTyping;

    //Timing
    public float TimeBetweenLetters;
    private float UntilNextLetter;

    /// <summary>
    /// prepare the dialogbox for initialisation
    /// </summary>
    public void PreInit() 
    {
        Name = gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0];
        Dialog = gameObject.GetComponentsInChildren<TextMeshProUGUI>()[1];

        var btns = gameObject.GetComponentsInChildren<Button>();

        BtnMean = btns[0];
        BtnNeutral = btns[1];
        BtnNice = btns[2];
        this.enabled = false;
    }

    /// <summary>
    /// Purely for testing the dialog in places without manually initiating
    /// </summary>
    public void TestInit(string n = "Test") 
    {            
        var p = new List<string>
        {
            $"Hello this is a test string for {n}"
        };

        // uncomment to test responses
        var r = new List<string>
        {
            "Mean >:(",
            "Neutral :|",
            "Nice :)"
        };       
        
        Init(p, r, n);
    }

    /// <summary>
    /// Pass parameters into dialogbox.
    /// </summary>
    /// <param name="_Phrases">List of phrases for NPC to display</param>
    /// <param name="_Responses">List of 3 player responses to NPC, empty list to init flat dialog</param>
    /// <param name="_Name">Name of NPC</param>
    /// <param name="_Emoji">Optional: Emoji symbols providing context</param>
    public void Init(List<string> _Phrases, List<string> _Responses, string _Name, string _Emoji = "") 
    {
        PreInit();
        // info from our CSVs
        Name.text = _Name;
        Phrases = _Phrases;
        Responses = _Responses;
        Emoji = _Emoji;

        if (_Responses.Count == 3) 
        {            
            // Fed up of dragging these into the GO!
            BtnNice.GetComponentInChildren<TextMeshProUGUI>().text = _Responses[2];
            BtnNeutral.GetComponentInChildren<TextMeshProUGUI>().text = _Responses[1];
            BtnMean.GetComponentInChildren<TextMeshProUGUI>().text = _Responses[0];
        }

        // Text
        NextPhrase();        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTyping)
        {
            DisplayNextLetter();
        }
        else if (Responses.Count != 3 && Input.GetKeyDown(KeyCode.Space))
        {
            DialogFinished();           
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
                isTyping = false;
                if (TextLeft.Length <= 0 && Responses.Count == 3)
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
        // Destroy self when phrases are used up
        if (Phrases == null || Phrases.Count <= 0)
        {
            DialogFinished();            
            return;
        }

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
        isTyping = true;
    }

    /// <summary>
    /// alert game controller of change
    /// </summary>
    /// <param name="response">index of button clicked (1 mean - 3 nice)</param>
    public void OnResponseClicked(int response) 
    {
        //print($"response {response} clicked!");
        NextPhrase();

        //return;
        var GC = GameObject.Find("GameController").GetComponent<GameController>();
        GC.ResponseClickedListener(response);
    }


    /// <summary>
    /// Tells gamecontroller that the dialog has completed
    /// </summary> 
    private void DialogFinished()
    {
        var GC = GameObject.Find("GameController").GetComponent<GameController>();
        GC.DialogFinishListener();
        Destroy(gameObject);
    }



}
