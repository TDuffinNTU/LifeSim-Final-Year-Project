using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CsvHelper;
using System.IO;

public class DialogBoxController : MonoBehaviour
{

    public TextMeshProUGUI Name;
    public Image Prompt;
    public TextMeshProUGUI Dialog;
    private string TextLeft;

    private List<string> Responses;
    private List<string> Phrases;
    private string Emoji;

    //Timing
    public float TimeBetweenLetters;
    private float UntilNextLetter;


    // Start is called before the first frame update
    void Start()
    {
        // this is how we'll init it later!
        var p = new List<string> { "hello there this is phrase 1", "this is phrase 2, now close me!"};
        var r = new List<string> { "blah blah" };
        var n = "Jeremy";
        var e = "e";
        Init(p, r, n, e);
    }

    // set out initial data initial data
    public void Init(List<string> _Phrases, List<string> _Responses, string _Name, string _Emoji) 
    {
        // info from our CSVs
        Name.text = _Name;
        Phrases = _Phrases;
        Responses = _Responses;
        Emoji = _Emoji;

        // Text
        NextPhrase();

        // Timing
        UntilNextLetter = TimeBetweenLetters;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Prompt.enabled)
        {
            DisplayNextLetter();
        }
        else 
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {                
                // Kill self when phrases are used up
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

    // Attempts to load next char from the array until its empty
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
                    Prompt.enabled = true;
                }
            }            
        }
    }

    // Set the next load of text to be spoken by the char
    private void NextPhrase() 
    {
        // Pop off front
        TextLeft = Phrases[0];
        Phrases.RemoveAt(0);

        // reset comps
        Dialog.text = "";
        Prompt.enabled = false;
    }
    



}
