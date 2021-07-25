using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogButtonBounce : MonoBehaviour
{
    [SerializeField]
    private Vector2 StartPos;
    [SerializeField]
    private Vector2 Velocity;
    public float Upforce;
    public float Gravity;

    public RectTransform _RectTransform;

    // Start is called before the first frame update
    void Start()
    {
        StartPos = _RectTransform.anchoredPosition;
        Velocity = new Vector2(0, Upforce);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_RectTransform.anchoredPosition.y < StartPos.y)
        {
            Velocity.y = Upforce;
        }

        Velocity.y -= Gravity;

        _RectTransform.anchoredPosition += Velocity * Time.deltaTime;
    }
}
