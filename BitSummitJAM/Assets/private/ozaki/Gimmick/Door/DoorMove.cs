using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using DG.Tweening;

public class DoorMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("‰ñ“]Ž²")]
    private Transform rotatePos;

    private float turnEnd;

    private Ray doorRay;

    private RaycastHit2D doorHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoorRotation(int hoge,bool fuga)
    {
        turnEnd = hoge * 30;

        Debug.Log(hoge);

        if(fuga)
        {
            rotatePos.transform.DOLocalRotate(new Vector3(0, 0, turnEnd), 1.0f).SetEase(Ease.OutBounce);
        }
        else
        {
            rotatePos.transform.DOLocalRotate(new Vector3(0, 0, 360 - turnEnd), 1.0f);
        }

        Invoke(nameof(returnRotate), 4.0f);
    }

    private void returnRotate()
    {
        rotatePos.transform.DOLocalRotate(new Vector3(0, 0, 0), 4.0f);
    }
}
