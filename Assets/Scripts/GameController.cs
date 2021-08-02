using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{  
    GameObject NearInteractablePrompt;

    // Start is called before the first frame update
    void Start()
    {
        Database DB = new Database();
        NearInteractablePrompt = GameObject.Find("Canvas/PROMPT");
        NearInteractablePrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollisionListChangeListener(List<GameObject> colliders) 
    {
        //foreach (var c in activeCols) { print(c.ToString()); }        
        NearInteractablePrompt.SetActive(colliders.Count > 0);
    }

    /// <summary>
    /// update friendship values depending on response chosen
    /// </summary>
    /// <param name="index"></param>
    public void ResponseClickedListener(int index) 
    {
        // update db
    }
}
