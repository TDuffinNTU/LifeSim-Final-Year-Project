using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerController : MonoBehaviour
{
    public float Speed = 1f;

    [SerializeField]
    private float xMove;
    [SerializeField]
    private float zMove;

    private const float CHANGEDIRTIME = 5f;
    private float ChangeDirectionTimer = CHANGEDIRTIME;

    // Start is called before the first frame update
    void Start()
    {
        ChangeDirection();
        zMove = Random.Range(0f, 1f);
        xMove = Random.Range(0f, 1f);
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

        Vector3 movement = new Vector3(xMove, 0, zMove);
        movement *= Speed * Time.deltaTime;

        if (movement.magnitude > 0)
            transform.rotation = Quaternion.LookRotation(movement);

        transform.Translate(movement, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ChangeDirection();
    }

    /// <summary>
    /// Randomly choose new direction to walk
    /// </summary>
    void ChangeDirection() 
    {
        ChangeDirectionTimer = CHANGEDIRTIME;
        float xInput = Random.Range(0f, 1f);
        float zInput = Random.Range(0f, 1f);
    }
}
