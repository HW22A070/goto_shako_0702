using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VideoMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("追いかけるオブジェクト")]
    private GameObject target;

    private Vector2 targetPos;

    [SerializeField]
    [Tooltip("カメラ位置最大")]
    private Vector2 MaxPos;

    [SerializeField]
    [Tooltip("カメラ位置最小")]
    private Vector2 MinPos;

    [SerializeField]
    [Tooltip("縦軸上限下限")]
    private float height;

    [SerializeField]
    [Tooltip("横軸上限下限")]
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
