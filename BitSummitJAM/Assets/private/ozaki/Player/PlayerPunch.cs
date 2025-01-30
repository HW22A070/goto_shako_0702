using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
//using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerPunch : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�f�o�b�N�p�v���C���[�̐F�擾�p")]
    private GameObject playerSkin;

    [SerializeField]
    [Tooltip("�p���`�|�W�V����")]
    private GameObject punchPos;

    [SerializeField]
    [Tooltip("�p���`�擾")]
    private GameObject punchHit;

    /// <summary>
    /// �p���`�����Ă��邩
    /// </summary>
    private bool ispunch = false;

    [SerializeField]
    private float punchTime;

    private float punchnowTime = 0;

    [SerializeField]
    [Tooltip("�p���`�̕���")]
    private GameObject punchVectol;

    [SerializeField]
    [Tooltip("�Ռ��g")]
    private GameObject Shock;

    /// <summary>
    /// �p���`�p�x
    /// </summary>
    public float punchDegree;

    /// <summary>
    /// �p���`�`���[�W����
    /// </summary>
    private float chargeTime;

    public bool SonicPunch;

    [SerializeField]
    [Tooltip("�p���`�`���[�W��i�K�ڂ̎���")]
    private float punch1;

    [SerializeField]
    [Tooltip("�p���`��1")]
    private int punchPower1;

    [SerializeField]
    [Tooltip("�p���`�`���[�W��i�K�ڂ̎���")]
    private float punch2;

    [SerializeField]
    [Tooltip("�p���`��2")]
    private int punchPower2;

    [SerializeField]
    [Tooltip("�p���`�`���[�W�O�i�K�ڂ̎���")]
    private float punch3;

    [SerializeField]
    [Tooltip("�p���`��3")]
    private int punchPower3;

    /// <summary>
    /// �p���`��ł������̃p���`��
    /// </summary>
    public int NowPunchPower;

    /// <summary>
    /// �p���`�`���[�W
    /// </summary>
    public int punch;

    /// <summary>
    /// �`���[�W�����Ă��邩
    /// </summary>
    public bool Charge;

    [Tooltip("�p���`�p�x")]
    private Vector3 punchRotate;

    [Tooltip("�W�����v�p�x")]
    private Vector3 jumpRotate;

    /// <summary>
    /// �p���`�W�����v�擾
    /// </summary>
    private Jumper punchjump;

    /// <summary>
    /// �p���`�pRay
    /// </summary>
    private RaycastHit2D punchRayHit;

    private PlayerManager playerManager;

    /// <summary>
    /// PlayerSound�擾
    /// </summary>
    private PlayerSoundC _cPlayerSound;

    [SerializeField, Tooltip("�p���`�G�t�F�N�g")]
    private PunchEffectC _prhbPunchEffect;

    [SerializeField, Tooltip("�_���[�W")]
    private GameObject _damageEffect;

    [SerializeField]
    LayerMask mask;

    float range;

    [SerializeField]
    bool punchcool;

    [SerializeField]
    float cooltime;

    float nowcooltime = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.Instance;

        Rigidbody2D rb = punchHit.GetComponent<Rigidbody2D>();

        //�`���[�W���Ԃ�������
        chargeTime = 0;

        //�W�����v�擾
        punchjump = GetComponent<Jumper>();

        //����p�x������
        punchRotate = new Vector3(0f, 0f, 0f);

        //�W�����v�p�x������
        jumpRotate = new Vector3(0f, 0f, 0f);

        punchHit.SetActive(false);

        punchVectol.SetActive(false);

        //PlayerSoundC�擾
        _cPlayerSound = GetComponent<PlayerSoundC>();

        punchcool = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("joystick button 0") && !PlayerManager.IsPlayerMoveRock && !punchcool ||
            Input.GetKey("joystick button 1") && !PlayerManager.IsPlayerMoveRock && !punchcool)
        {
            PunchCharge();

            PunchRight();
        }

        if (1 <= punch && Input.GetKey("joystick button 4") && !PlayerManager.IsPlayerMoveRock)
        {
            Charge = true;
            //�p���`�̕��������߂�
            PunchRotate();
        }

        if ((1 <= punch && Input.GetKeyUp("joystick button 0") ||
            1 <= punch && Input.GetKeyUp("joystick button 4") || 
            1 <= punch && Input.GetKeyUp("joystick button 1") && !PlayerManager.IsPlayerMoveRock))
        {

            Debug.Log("�p���`");

            PunchPowerScale();

            ispunch = true;

            punchHit.SetActive(true);

            PunchCheck();

            if ((Input.GetKeyUp("joystick button 4") && punchDegree >= 0f && punchDegree <= 200f ||
                Input.GetKey("joystick button 4") && punchDegree >= 0f && punchDegree <= 200f) && !PlayerManager.IsPlayerMoveRock)
            {
                ShockPunch();
            }

            chargeTime = 0;

            Charge = false;

            punch = 0;

            ChaegeReset();

            punchcool = true;
        }

        if (ispunch)
        {
            NowPunch();
        }

        if (PlayerManager.IsPlayerMoveRock) punch = 0;

        if(punchcool)
        {
            PunchCoolTime();
        }
    }

    private void PunchCharge()
    {
        chargeTime += Time.deltaTime;

        if (punch1 <= chargeTime && chargeTime < punch2)
        {
            if (punch != 1)
            {
                Instantiate(_prhbPunchEffect, punchPos.transform.position, transform.rotation).transform.parent = punchPos.transform;
                _cPlayerSound.SoundStart(0);
            }
            punch = 1;
        }

        else if (punch2 <= chargeTime && chargeTime < punch3)
        {
            if (punch != 2)
            {
                _cPlayerSound.SoundStart(1);
            }
            punch = 2;
        }

        else if (punch3 <= chargeTime)
        {
            if (punch != 3)
            {
                _cPlayerSound.SoundStart(2);
            }
            punch = 3;
        }

    }

    private void ChaegeReset()
    {
        chargeTime = 0;

        punch = 0;

        punchVectol.SetActive(false);

        if (GetComponent<PlayerManager>().IsRight)
        {
            punchPos.transform.eulerAngles = new Vector3(0f, 0f, 0f);

            punchRotate = transform.right;
        }
        else
        {
            punchPos.transform.eulerAngles = new Vector3(0f, 180f, 0f);

            punchRotate = -transform.right;
        }

    }

    private void PunchCheck()
    {
        Debug.Log("�p���`����");

        _cPlayerSound.SoundStart(7);

        PunchRange();

        if (punchRayHit = Physics2D.Raycast(punchHit.transform.position,
                                            punchRotate,
                                            range))
        {
            Debug.Log("�p���`");
            Debug.Log(punchRayHit.collider.gameObject);
            if (punchRayHit.collider)
            {
                GameObject damageEffect = Instantiate(_damageEffect, punchHit.transform.position + new Vector3(-1, 1, 0), transform.localRotation);
                Destroy(damageEffect, 0.2f);

                if (punchRayHit.collider.gameObject.tag == "Ground" &&
                    Physics2D.Raycast(transform.position,
                                      -transform.up,
                                      transform.localScale.y * 0.5f, mask))
                {

                    Debug.Log("��������");
                    if (punch != 1)
                    {
                        _cPlayerSound.SoundStart(8);
                    }

                    punchjump.Jumppunch = true;
                }
                else if ((punchRayHit.collider.gameObject.tag == "Enemy"))
                {
                    Debug.Log("�G�ɓ�������" + NowPunchPower);

                    punchRayHit.collider.gameObject.GetComponent<EnemyCoreC>().GetBlow(NowPunchPower);
                    _cPlayerSound.SoundStart(5);

                }
                else if ((punchRayHit.collider.gameObject.tag == "Gimmick"))
                {
                    Debug.Log("�M�~�b�N��������");

                    var right = playerManager.Right();

                    punchRayHit.collider.gameObject.GetComponent<GimmickManager>().GimmickHit(NowPunchPower, right, punch);
                    _cPlayerSound.SoundStart(5);
                }


            }

        }
        Debug.DrawRay(punchHit.transform.position,
                punchRotate * range,
                Color.red,
                5);
    }

    /// <summary>
    /// �p���`��]
    /// </summary>
    private void PunchRotate()
    {
        punchVectol.SetActive(true);

        // �E�X�e�B�b�N������
        var horizontal = Input.GetAxis("Horizontal");

        // �E�X�e�B�b�N�c����
        var vertical = Input.GetAxis("Vertical");

        // �E�X�e�B�b�N���͊p�x���Z�o
        punchDegree = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;

        // �p�x���v���X�̐�����
        if (punchDegree < 0f)
        {
            punchDegree += 360f;
        }

        if (horizontal == 0 && vertical == 0)
        {
            punchPos.SetActive(false);
        }
        else
        {
            punchPos.SetActive(true);
        }

        // �E�X�e�B�b�N�̓��͂�����Ƃ�
        if (horizontal != 0 || vertical != 0)
        {
            // �㔼�~�͈̔͂œ����O����ύX
            // ������p�x����
            punchRotate =
            new Vector3(Mathf.Cos((punchDegree) * Mathf.Deg2Rad), Mathf.Sin((punchDegree) * Mathf.Deg2Rad), 0f);
            // �����O����ύX
            punchPos.transform.eulerAngles = new Vector3(0f, 0f, punchDegree);
        }

    }

    /// <summary>
    /// �Ռ��g�����
    /// </summary>
    private void ShockPunch()
    {
        SonicPunch = true;
        // �����Ɠ����ɃI�u�W�F�N�g�����擾
        var shockObj = Instantiate(Shock, punchHit.transform.position, punchPos.transform.rotation);
        // �I�u�W�F�N�g����X�N���v�g�����擾
        shockObj.GetComponent<ShockMove>().MoveDirection = punchPos.transform.right;

        _cPlayerSound.SoundStart(9);
    }

    /// <summary>
    /// �p���`�͂��擾����
    /// </summary>
    private void NowPunch()
    {
        punchnowTime += Time.deltaTime;

        if (punchTime <= punchnowTime)
        {
            ispunch = false;

            punchHit.SetActive(false);

            punchnowTime = 0;

            punchDegree = 0;

            if (GetComponent<PlayerManager>().IsRight)
            {
                punchPos.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            else
            {
                punchPos.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }

            SonicPunch = false;


        }
    }

    public void PunchReset()
    {
        punch = 0;

        chargeTime = 0;

        punchVectol.SetActive(false);

        Charge = false;

        if (GetComponent<PlayerManager>().IsRight)
        {
            punchPos.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else
        {
            punchPos.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void PunchPowerScale()
    {
        switch (punch)
        {
            case 1:
                NowPunchPower = punchPower1;
                Debug.Log(NowPunchPower);
                break;
            case 2:
                NowPunchPower = punchPower2;
                Debug.Log(NowPunchPower);
                break;
            case 3:
                NowPunchPower = punchPower3;
                Debug.Log(NowPunchPower);
                break;
        }
    }


    public bool GraphicSender()
    {
        return ispunch;
    }

    public float GraphicSender2()
    {
        return punchDegree;
    }

    public int PunchLevelSender()
    {
        return punch;
    }

    private void PunchRight()
    {
        var hoge = playerManager.Right();

        if (hoge)
        {
            punchDegree = 0;
        }
        else
        {
            punchDegree = 180;
        }
    }

    public void PlayerOut()
    {
        Charge = false;

        punchVectol.SetActive(false);
    }

    void PunchRange()
    {
        switch (punch)
        {
            case 1:
                range = punchHit.transform.localScale.x * 0.7f;
                break;
            case 2:
                range = punchHit.transform.localScale.x * 2.0f;
                break;
            case 3:
                range = punchHit.transform.localScale.x * 3.5f;
                break;
        }
    }

    void PunchCoolTime()
    {
        nowcooltime += Time.deltaTime;

        if(cooltime <= nowcooltime)
        {
            punchcool = false;

            nowcooltime = 0;
        }
    }
}
