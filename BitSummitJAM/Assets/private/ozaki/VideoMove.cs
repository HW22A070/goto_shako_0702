using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VideoMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�ǂ�������I�u�W�F�N�g")]
    private GameObject target;

    private Vector2 targetPos;

    [SerializeField]
    [Tooltip("�J�����ʒu�ő�")]
    private Vector2 MaxPos;

    [SerializeField]
    [Tooltip("�J�����ʒu�ŏ�")]
    private Vector2 MinPos;

    [SerializeField]
    [Tooltip("�c���������")]
    private float height;

    [SerializeField]
    [Tooltip("�����������")]
    private float width;

    private Vector3 cameraPos;


    // Start is called before the first frame update
    void Start()
    {
        targetPos = target.transform.position;

        MaxPos.Set(targetPos.x + width, targetPos.y + height);
        MinPos.Set(targetPos.x - width, targetPos.y - height);

        cameraPos = this.transform.position;

    }

    void FixedUpdate()
    {
    //    Vector3 cameraPos = transform.position;

    //    targetPos = target.transform.position;

    //    if (targetPos.x > MaxPos.x)
    //    {
    //        cameraPos.x += targetPos.x - MaxPos.x;
    //    }
    //    else if (targetPos.x < MinPos.x)
    //    {
    //        cameraPos.x += targetPos.x - MinPos.x;
    //    }

    //    if (targetPos.y > MaxPos.y)
    //    {
    //        cameraPos.y += targetPos.y - MaxPos.y;
    //    }
    //    else if (cameraPos.y < MinPos.y)
    //    {
    //        cameraPos.y += targetPos.y - MinPos.y;
    //    }

    //    transform.position = cameraPos;

    //    MaxPos.Set(targetPos.x + width, targetPos.y + height);
    //    MinPos.Set(targetPos.x - width, targetPos.y - height);
    }

    void Update()
    {
        targetPos = target.transform.position;

        if (targetPos.x > MaxPos.x)
        {
            cameraPos.x = targetPos.x + MaxPos.x;

            MaxPos.Set(targetPos.x + width, targetPos.y + height);
            MinPos.Set(targetPos.x - width, targetPos.y - height);
        }
        else if (targetPos.x < MinPos.x)
        {
            cameraPos.x = targetPos.x - MinPos.x;

            MaxPos.Set(targetPos.x + width, targetPos.y + height);
            MinPos.Set(targetPos.x - width, targetPos.y - height);
        }

        if (targetPos.y > MaxPos.y)
        {
            cameraPos.y = targetPos.y + MaxPos.y;

            MaxPos.Set(targetPos.x + width, targetPos.y + height);
            MinPos.Set(targetPos.x - width, targetPos.y - height);
        }
        else if (cameraPos.y < MinPos.y)
        {
            cameraPos.y = targetPos.y - MinPos.y;

            MaxPos.Set(targetPos.x + width, targetPos.y + height);
            MinPos.Set(targetPos.x - width, targetPos.y - height);
        }

        transform.position = cameraPos;
    }
}
