using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameController _GameController;
    public float _Speed = 1f;  
    
    private GameObject _Camera;    
    private Vector3 _CameraRelativePosition;

    private int _ActiveCollisions = 0;

    // Start is called before the first frame update
    void Start()
    {
        _ActiveCollisions = 0;
        _Camera = GameObject.FindGameObjectWithTag("MainCamera");
        _CameraRelativePosition = transform.position - _Camera.transform.position;
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

        _Camera.transform.position = transform.position - _CameraRelativePosition;       
    }

    private void OnCollisionEnter(Collision collision)
    {
        _ActiveCollisions++;
        _GameController.PromptUpdate(_ActiveCollisions);
        Debug.Log(_ActiveCollisions);
    }

    private void OnCollisionExit(Collision collision)
    {
        _ActiveCollisions--; 
        _GameController.PromptUpdate(_ActiveCollisions);
        Debug.Log(_ActiveCollisions);
    }
}
