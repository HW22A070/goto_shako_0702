using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundC : MonoBehaviour
{
    [SerializeField,Tooltip("場所リセットさせる到達座標")]
    private float _resetTrigerPositionX;

    [SerializeField, Tooltip("場所リセットさせる先の座標")]
    private float _resetTargetPositionX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition -= transform.right;
        if (transform.localPosition.x < _resetTrigerPositionX)
        {
            transform.localPosition = new Vector3(_resetTargetPositionX,0,0);
        }
    }
}
