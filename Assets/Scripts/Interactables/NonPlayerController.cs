using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerController : Interactable
{
    public float Speed;
    private float SpeedScale = 1f;

    [SerializeField]
    private float xMove;
    [SerializeField]
    private float zMove;

    private const float CHANGEDIRTIME = 2f;

    [SerializeField]
    private float ChangeDirectionTimer = CHANGEDIRTIME;
    private Rigidbody RB;

    public int Attitude;

    // Start is called before the first frame update
    void Start()
    {
        RB = gameObject.GetComponent<Rigidbody>();
        ChangeDirection();        
    }

    // Update is called once per frame
    void Update()
    {
        // nothing much different to player move control
        ChangeDirectionTimer -= Time.deltaTime;
        if (ChangeDirectionTimer <= 0f) 
        {
            ChangeDirection();
        }

        Vector3 TargetDir = new Vector3(xMove, 0, zMove).normalized;

        if (TargetDir != Vector3.zero)
        {
            RB.rotation = Quaternion.LookRotation(TargetDir);
        }


        RB.velocity = (TargetDir * Speed * Time.deltaTime * SpeedScale);
    }

    /// <summary>
    /// Randomly choose new direction to walk
    /// </summary>
    void ChangeDirection() 
    {
        ChangeDirectionTimer = CHANGEDIRTIME;
        xMove = Random.Range(-1f, 1f);
        zMove = Random.Range(-1f, 1f);
        SpeedScale = Random.Range(0f, 1f);
    }    

    /// <summary>
    /// start a conversation with this NPC
    /// </summary>
    public override void onPlayerInteract()
    {
        var GC = GameObject.Find("GameController").GetComponent<GameController>();
        GC.DialogStartListener(Name, Attitude);

    }

    public override void onPlayerPickup()
    {
        return;
    }

    public override void onPlayerPlace()
    {
        Destroy(this);
    }

    
}
