using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class DoorMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("‰ñ“]Ž²")]
    private Transform rotatePos;

    private float turnEnd;

    private Ray doorRay;

    private RaycastHit2D doorHit;

    [SerializeField]
    private LayerMask mask;

    Vector2 Rotation;

    bool open;
    // Start is called before the first frame update
    void Start()
    {
        open = false;
    }

    // Update is called once per frame
    void Update()
    {
        Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y * 0.5f),
                                             -transform.up * transform.localScale.y,
                                             transform.localScale.y);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + transform.localScale.y * 0.5f),
                                             -transform.up * transform.localScale.y,
                                             Color.red);

        if (open)
        {
            DoorShock();
        }
    }

    public void DoorRotation(int hoge,bool fuga)
    {
        Debug.Log(hoge);

        Debug.Log("ŠJ‚­");

        open = true;

        if (hoge < 3)
        {
            return;
        }

        turnEnd = hoge * 30;

        
        if (fuga)
        {
            rotatePos.transform.DOLocalRotate(new Vector3(0, 0, turnEnd), 3.0f).SetEase(Ease.OutBounce);

        }
        else
        {
            rotatePos.transform.DOLocalRotate(new Vector3(0, 0, 360 - turnEnd), 3.0f).SetEase(Ease.OutBounce);

        }
    }

    private void DoorShock()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y * 0.5f),
                                             -rotatePos.up * transform.localScale.y,
                                             transform.localScale.y);

        if (hit)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.gameObject.GetComponent<EnemyCoreC>().GetSonic();
            }
        }
    }
}
