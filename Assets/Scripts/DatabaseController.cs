using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
    [SerializeField]
    public Database db;

    /// <summary>
    /// load the db to memory
    /// </summary>
    /// <returns></returns>
    public Database LoadDB() 
    {        
        return db = new Database();
    }
}
