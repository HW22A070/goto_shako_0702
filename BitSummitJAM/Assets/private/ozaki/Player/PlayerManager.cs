using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build.Player;
using UnityEngine;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    [SerializeField]
    [Tooltip("�v���C���[�ő�Hp")]
    private int maxHp;
    /// <summary>
    /// �v���C���[�p���`�擾
    /// </summary>
    private PlayerPunch playerPunch;
    /// <summary>
    /// �v���C���[�ő�Hp
    /// </summary>
    public int MaxHp
    {
        get => maxHp;
    }
    [SerializeField]
    [Tooltip("�v���C���[Hp")]
    private int hp;
    /// <summary>
    /// �v���C���[�ő�Hp
    /// </summary>
    public int Hp
    {
        get => hp;
        set => hp = value;
    }

    [SerializeField]
    [Tooltip("�v���C���[���E�������Ă��邩")]
    private bool isRight;

    /// <summary>
    /// ���ǂ����������Ă��邩
    /// </summary>
    private bool isRightNow;

    /// <summary>
    /// �ǂ����������Ă��邩
    /// </summary>
    public bool IsRight
    {
        get => isRight;
        set => isRight = value;
    }

    [SerializeField]
    [Tooltip("���G����")]
    private float _Time = 0;

    private float now = 0;

    private bool Hit = false;

    [SerializeField]
    private GameObject playerBody;

    [SerializeField]
    private GameObject punchPos;

    [SerializeField]
    LayerMask mask;

    [SerializeField]
    private PlayerCamera playerCamera;

    /// <summary>
    /// PlayerSound�擾
    /// </summary>
    private PlayerSoundC _cPlayerSound;

    // Start is called before the first frame update
    void Start()
    {
        PlayerManager.IsPlayerMoveRock = false;

        //プレイヤーが死んでいたらその場所に置く
        if (SceneManagementC.PlayerDaed)
        {
            transform.position = SceneManagementC.PositionPlayerDead;
        }

        isRight = true;

        hp = maxHp;

        playerPunch = GetComponent<PlayerPunch>();

        playerCamera = playerCamera.GetComponent<PlayerCamera>();

        //PlayerSoundC�擾
        _cPlayerSound = GetComponent<PlayerSoundC>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            PlayerDamage(0);
        }

        if (PlayerManager.IsPlayerMoveRock)
        {
            isRight = true;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            isRight = true;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            isRight = false;
        }

        if (isRight != isRightNow)
        {
            IsRightChange(isRight);

            isRightNow = isRight;
        }

        if (Hit)
        {
            now += Time.deltaTime;

            if (_Time <= now)
            {
                Hit = false;

                now = 0;


            }
        }

    }

    private void FixedUpdate()
    {
        //if (Physics2D.Raycast(transform.position,
        //                     -transform.up,
        //                     transform.localScale.y * 3.5f
        //                     , mask))
        //{
        //    playerCamera.isGround();

        //}
        //else
        //{
        //    playerCamera.isJump();
        //}
        //Debug.DrawRay(transform.position, -transform.up * 3.5f, Color.red);

        var jump = playerCamera.IsJump();

        if (jump)
        {

            if (Physics2D.Raycast(transform.position,
                                 -transform.up,
                                 transform.localScale.y * 1.5f
                                 , mask))
            {
                playerCamera.isGround();
            }
            Debug.DrawRay(transform.position, -transform.up * 1.5f, Color.red);
        }
        else
        {
            if (!Physics2D.Raycast(transform.position,
                -transform.up,
                transform.localScale.y * 3.5f
                , mask))
            {
                playerCamera.isJump();
            }
            Debug.DrawRay(transform.position, -transform.up * 3.5f, Color.red);
        }
    }

    public void PlayerDamage(int hoge)
    {
        if (Hit == false)
        {
            _cPlayerSound.SoundStart(3);
            Hit = true;

            hp -= hoge;
        }

        GetComponent<PlayerPunch>().PunchReset();

        GetComponent<PlayerPunch>().PlayerOut();

        if (hp <= 0)
        {
            SceneManagementC.PositionPlayerDead = transform.position;
            SceneManagementC.LoadScene("GameOverScene");
        }

        if (Hit)
        {

        }
    }

    public void IsRightChange(bool hoge)
    {
        if (hoge)
        {
            playerBody.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

            punchPos.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
        else
        {
            playerBody.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

            punchPos.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    public bool GraphicSender()
    {
        return Hit;
    }

    public int HPSender()
    {
        return Hp;
    }

    public void HpHeal()
    {
        if (MaxHp > hp)
        {
            hp = hp + 3;

            if(MaxHp < hp)
            {
                hp = MaxHp;
            }
        }
    }

    public bool Right()
    {
        return isRight;
    }

    public static bool IsPlayerMoveRock;
}
