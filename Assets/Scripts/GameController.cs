using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{  
    GameObject _NearInteractablePrompt;

    // Start is called before the first frame update
    void Start()
    {        
        _NearInteractablePrompt = GameObject.Find("Canvas/PROMPT");
        _NearInteractablePrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PromptUpdate(int activeCols) 
    {
        _NearInteractablePrompt.SetActive(activeCols > 1);
    }
}
