using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShrimpDush : MonoBehaviour
{
    /// <summary>
    /// �v���C���[�̐F�擾�p
    /// </summary>
    private GameObject Playercolor;

    [SerializeField]
    [Tooltip("�G�r�_�b�V�����x")]
    private float dushSpeed;

    [SerializeField]
    [Tooltip("�G�r�_�b�V������")]
    private float dushTime;

    /// <summary>
    /// �G�r�_�b�V�����Ԍv���p
    /// </summary>
    private float nowdushTime;

    [SerializeField]
    [Tooltip("�G�r�_�b�V����")]
    private bool isDush;

    [SerializeField]
    [Tooltip("�ő�G�r�_�b�V����")]
    private int maxDush;

    /// <summary>
    /// �G�r�_�b�V���c��
    /// </summary>
    private int nowDush;

    [SerializeField]
    [Tooltip("�G�r�_�b�V���񕜎���")]
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
        //RB�L�[�������ꂽ��G�r�_�b�V��
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
