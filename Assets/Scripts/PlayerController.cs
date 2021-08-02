using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameController GC;
    private Rigidbody RB;

    public float Speed = 1f;  
    
    private GameObject Camera;    
    private Vector3 CameraRelPosition;

    private List<GameObject> Colliders;

    /// <summary>
    /// Player setup
    /// </summary>
    void Start()
    {
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        RB = gameObject.GetComponent<Rigidbody>();

        Camera = GameObject.FindGameObjectWithTag("MainCamera");

        Colliders = new List<GameObject>();
        CameraRelPosition = transform.position - Camera.transform.position;
    }

    /// <summary>
    /// Player updates
    /// </summary>
    void FixedUpdate()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 TargetDir = new Vector3(xInput, 0, zInput).normalized;

        if (Mathf.Abs(xInput + zInput) > 0.1f)
        {
            RB.rotation = Quaternion.LookRotation(TargetDir);
        }

        RB.velocity = TargetDir * Speed * Time.fixedDeltaTime;

        Camera.transform.position = RB.position - CameraRelPosition;       
    }

    /// <summary>
    /// Called when player ENTERS the trigger zone of map objs
    /// </summary>
    /// <param name="other"> Collider trigger we've left</param>
    private void OnTriggerEnter(Collider other)
    {     
        // add new collider
        Colliders.Add(other.gameObject);

        ResetAllOutlines();

        // display prompt
        GC.CollisionListChangeListener(Colliders);
    }

    /// <summary>
    /// Called when player EXITS the trigger zone of map objs
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        var outline = other.gameObject.GetComponent<Outline>();
        if ( outline != null) 
        {
            Destroy(outline);
        }

        Colliders.Remove(other.gameObject); 
        GC.CollisionListChangeListener(Colliders);

        ResetAllOutlines();
    }

    /// <summary>
    /// Resets the outline shown around front obj in List<Colliders>
    /// </summary>
    private void ResetAllOutlines()     
    {
        // remove all outlines
        foreach (var obj in Colliders)
        {
            var outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                Destroy(outline);
            }
        }

        // add outline to first collider in list
        if (Colliders.Count != 0 && Colliders[0].gameObject.GetComponent<Outline>() == null)
        {
            var outline = Colliders[0].gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.cyan;
            outline.OutlineWidth = 10f;
        }

    }
}
