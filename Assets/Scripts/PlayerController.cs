using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float _Speed = 1f;    
    private GameObject _CAM;
    private Vector3 _CameraRelativePosition;
    

    // Start is called before the first frame update
    void Start()
    {      
        _CAM = GameObject.FindGameObjectWithTag("MainCamera");
        _CameraRelativePosition = transform.position - _CAM.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(xInput, 0, zInput);
        movement *= _Speed * Time.deltaTime;

        if (movement.magnitude > 0)
            transform.rotation = Quaternion.LookRotation(movement);

        transform.Translate(movement, Space.World);

        _CAM.transform.position = transform.position - _CameraRelativePosition;

       
    }    
}
