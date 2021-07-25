using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ClockController : MonoBehaviour
{
    public Text DayText;
    public RawImage ClockBottom;
    public RawImage ClockTop;
    public Light Light;

    private const int TIME_PER_TICK = 1;
    private const int TICKS_PER_DAY = 360;
    
    private const float ROTATE_OFFSET_CLOCK = -90f;
    private const float ROTATE_OFFSET_LIGHT = 35f;
    private const float LERP_CONSTANT_CLOCK = .01f;
    private const float LERP_CONSTANT_LIGHT = .01f;

    [SerializeField]
    private Quaternion LightRotationTarget;
    
    [SerializeField]
    private Quaternion ClockRotationTarget;

    private float TimeTilTick;

    [SerializeField]
    private int CurrentTick;

    [SerializeField]
    private int CurrentDay;


    // Start is called before the first frame update
    void Start()
    {
        TimeTilTick = TIME_PER_TICK;
        CurrentTick = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TimeTilTick -= Time.fixedDeltaTime;
        if (TimeTilTick <= 0) 
        {
            Tick();
        }

        Light.transform.rotation = Quaternion.Lerp(Light.transform.rotation, LightRotationTarget,  LERP_CONSTANT_LIGHT);
        ClockBottom.transform.rotation = Quaternion.Lerp(ClockBottom.transform.rotation, ClockRotationTarget,  LERP_CONSTANT_CLOCK);
    }

    void SetTick(int tick) 
    {
        if (tick > 0 && tick < TICKS_PER_DAY) 
        {
            CurrentTick = tick;
        }
    }

    int GetTick() { return CurrentTick; }

    int GetDay() { return CurrentDay; }

    void Tick() 
    {
        //Debug.Log("Tick");
        if (CurrentTick >= TICKS_PER_DAY)
        {
            CurrentTick = 0;
            NextDay();
        }
        else 
        {
            CurrentTick++;
        }
        
        TimeTilTick = TIME_PER_TICK;
        UpdateRotations();
    }

    void NextDay() 
    {
        if (CurrentDay == 6)
        {
            CurrentDay = 0;
        }
        else 
        {
            CurrentDay++;
        }
    }

    void UpdateRotations() 
    {
        float percent = (float)CurrentTick / (float)TICKS_PER_DAY;
        float rotation = 360f * percent;
        //Debug.Log(percent + "" + rotation);
        ClockRotationTarget = Quaternion.Euler(0, 0, ClockBottom.transform.rotation.z + rotation - ROTATE_OFFSET_CLOCK);
        LightRotationTarget = Quaternion.Euler(Light.transform.rotation.x + rotation - ROTATE_OFFSET_LIGHT, 0, 0);

    }
}
