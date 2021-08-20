using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ClockController : MonoBehaviour
{
    private GameObject ClockHand;
    private GameObject Light;

    private const float TIME_PER_DAY = 150f;

    private const float ROTATE_OFFSET_CLOCK = -90f;
    private const float ROTATE_OFFSET_LIGHT = 35f; 



    [SerializeField]
    private float TimeLeftToday = TIME_PER_DAY;



    // Start is called before the first frame update
    void Start()
    {        
        ClockHand = GameObject.Find("Hand");
        Light = GameObject.Find("Directional Light");

        TimeLeftToday = TIME_PER_DAY;        
    }

    // Update is called once per frame
    void Update()
    {
        TimeLeftToday -= Time.deltaTime;
        if (TimeLeftToday <= 0f)
        {
            TimeLeftToday = TIME_PER_DAY;
            GameObject.Find("GameController").GetComponent<GameController>().RefreshStock();
        }

        float rot = GetPercentOfDay() * 360f;
        Light.transform.rotation = Quaternion.Euler(rot-ROTATE_OFFSET_CLOCK, 0, 0);
        ClockHand.transform.rotation = Quaternion.Euler(0, 0, rot-ROTATE_OFFSET_LIGHT);
    }

    /// <summary>
    /// The percent of the day that has elapsed
    /// </summary>
    /// <returns>float betwen 0 and 1 representing decimal percentage of day elapsed</returns>
    public float GetPercentOfDay()
    {
        return TimeLeftToday / TIME_PER_DAY;
    }

    /// <summary>
    /// the time of day represented as a string
    /// </summary>
    /// <returns>NIGHT, MORNING, or AFTERNOON as a string</returns>
    public string GetTimeOfDay()
    {
        var p = GetPercentOfDay();

        if (p < 0.3f)
        {
            return "MORNING";
        }
        if (p < 0.75f)
        {
            return "NIGHT";
        }
        else if (p < 1f)
        {
            return "AFTERNOON";
        }
        else 
        {
            return "MORNING";
        }
    }

}
