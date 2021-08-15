using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
    [SerializeField]
    public Database db;

    public Database LoadDB() 
    {        
        return db = new Database();
    }
}
