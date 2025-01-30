using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrimpDush : MonoBehaviour
{
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

    [SerializeField]
    LayerMask mask;

    private PlayerManager _cPlayerManager;

    [SerializeField,Tooltip("�y��")]
    private PlayerDustC _prfbPlayerDust;

    // Start is called before the first frame update
    void Start()
    {
        isDush = false;
        _cPlayerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //RB�L�[�������ꂽ��G�r�_�b�V��
        if (Input.GetKeyDown("joystick button 5") && nowDush > 0&&!PlayerManager.IsPlayerMoveRock)
        {
            Instantiate(_prfbPlayerDust, transform.position+(transform.up*1.0f), transform.rotation).Hanten(_cPlayerManager.Right());

            Debug.Log("�G�r�_�b�V��");

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

                if(Physics2D.Raycast(transform.position,
                                  - transform.right,
                                  transform.localScale.x * 0.5f + 0.1f,
                                  mask))
                {
                    if (Physics2D.Raycast(transform.position,
                                  transform.right,
                                  transform.localScale.x * 0.5f))
                    {
                        transform.position += transform.right * 0.1f;
                    }
                    Debug.Log("�I��");
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
                if (Physics2D.Raycast(transform.position,
                                  transform.right,
                                  transform.localScale.x * 0.5f + 0.1f,
                                  mask))
                {
                    if(Physics2D.Raycast(transform.position,
                                  transform.right,
                                  transform.localScale.x * 0.5f))
                    {
                        transform.position -= transform.right * 0.1f;
                    }
                    Debug.Log("�I��");

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
