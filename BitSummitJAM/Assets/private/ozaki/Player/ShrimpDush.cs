using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShrimpDush : MonoBehaviour
{
    /// <summary>
    /// プレイヤーの色取得用
    /// </summary>
    private GameObject Playercolor;

    [SerializeField]
    [Tooltip("エビダッシュ速度")]
    private float dushSpeed;

    [SerializeField]
    [Tooltip("エビダッシュ時間")]
    private float dushTime;

    /// <summary>
    /// エビダッシュ時間計測用
    /// </summary>
    private float nowdushTime;

    [SerializeField]
    [Tooltip("エビダッシュ中")]
    private bool isDush;

    [SerializeField]
    [Tooltip("最大エビダッシュ回数")]
    private int maxDush;

    /// <summary>
    /// エビダッシュ残数
    /// </summary>
    private int nowDush;

    [SerializeField]
    [Tooltip("エビダッシュ回復時間")]
    private float time;

    private float nowtime;

    private Renderer playerColor;


    // Start is called before the first frame update
    void Start()
    {
        isDush = false;
    }

    // Update is called once per frame
    void Update()
    {
        //RBキーが押されたらエビダッシュ
        if (Input.GetKeyDown("joystick button 5") && nowDush > 0)
        {
            --nowDush;

            isDush = true;
        }

        if (maxDush > nowDush)
        {
            nowtime+= Time.deltaTime;

            if (nowtime >= time)
            {
                nowtime = 0;

                nowDush++;
            }
        }
    }

    private void FixedUpdate()
    {

        if (isDush)
        {
            GetComponent<PlayerPunch>().PunchReset();

            if (GetComponent<PlayerManager>().IsRight)
            {
                nowdushTime += Time.deltaTime;

                this.transform.position -= transform.right * dushSpeed * nowdushTime;

                if (nowdushTime >= dushTime)
                {
                    isDush = false;
                }
            }
            else
            {
                nowdushTime += Time.deltaTime;

                this.transform.position += transform.right * dushSpeed * nowdushTime;

                if (nowdushTime >= dushTime)
                {
                    isDush = false;
                }
            }
        }
        else
        {
            nowdushTime = 0;
        }
    }


    public bool GraphicSender()
    {
        return isDush;
    }
}
