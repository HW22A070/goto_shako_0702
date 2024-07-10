using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private float punchDegree;

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

    private Renderer playerColor;

    /// <summary>
    /// �p���`�W�����v�擾
    /// </summary>
    private Jumper punchjump;

    /// <summary>
    /// �p���`�pRay
    /// </summary>
    private RaycastHit2D punchRayHit;

    // Start is called before the first frame update
    void Start()
    {
        //�`���[�W���Ԃ�������
        chargeTime = 0;

        //�F���擾
        playerColor = playerSkin.GetComponent<Renderer>();

        //�W�����v�擾
        punchjump = GetComponent<Jumper>();

        //����p�x������
        punchRotate = new Vector3(0f, 0f, 0f);

        //�W�����v�p�x������
        jumpRotate = new Vector3(0f, 0f, 0f);

        punchHit.SetActive(false);

        punchVectol.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("joystick button 0"))
        {
            PunchCharge();
        }

        if (1 <= punch && Input.GetKey("joystick button 4"))
        {
            Charge = true;
            //�p���`�̕��������߂�
            PunchRotate();
        }

        if (1 <= punch && Input.GetKeyUp("joystick button 0") || 1 <= punch && Input.GetKeyUp("joystick button 4"))
        {

            Debug.Log("�p���`");

            PunchPowerScale();

            ispunch = true;

            punchHit.SetActive(true);

            PunchCheck();

            if (Input.GetKeyUp("joystick button 4") && punchDegree >= 0f && punchDegree <= 200f ||
                Input.GetKey("joystick button 4") && punchDegree >= 0f && punchDegree <= 200f)
            {
                ShockPunch();
            }

            chargeTime = 0;

            Charge = false;

            punch = 0;

            playerColor.material.color = Color.white;

            ChaegeReset();
        }

        if(ispunch)
        {
            NowPunch();
        }
    }

    private void PunchCharge()
    {
        chargeTime += Time.deltaTime;

        if (punch1 <= chargeTime && chargeTime < punch2)
        {
            punch = 1;
        }

        else if (punch2 <= chargeTime && chargeTime < punch3)
        {
            punch = 2;
        }

        else if (punch3 <= chargeTime)
        {
            punch = 3;
        }

        switch (punch)
        {

            case 0:
                playerColor.material.color = Color.white;
                break;

            case 1:
                playerColor.material.color = Color.red;
                break;

            case 2:
                playerColor.material.color = Color.green;
                break;
            case 3:
                playerColor.material.color = Color.blue;
                break;

        }
    }

    private void ChaegeReset()
    {
        chargeTime = 0;

        punch = 0;

        playerColor.material.color = Color.white;

        punchVectol.SetActive(false);
    }

    private void PunchCheck()
    {
        Debug.Log("�p���`����");

        if (punchRayHit = Physics2D.Raycast(punchHit.transform.position,
                                            punchRotate,
                                            punchHit.transform.localScale.x * 0.5f,511))
        {
            Debug.Log("�p���`");
            Debug.Log(punchRayHit.collider.gameObject);
            if (punchRayHit.collider)
            {
                if (punchRayHit.collider.gameObject.tag == "Ground")
                {
                    Debug.Log("��������");
                    punchjump.Jumppunch = true;
                }

                else if ((punchRayHit.collider.gameObject.tag == "Enemy"))
                {
                    Debug.Log("�G�ɓ�������"+ NowPunchPower);

                    punchRayHit.collider.gameObject.GetComponent<EnemyCoreC>().GetBlow(NowPunchPower);

                }

                else if ((punchRayHit.collider.gameObject.tag == "Gimmick"))
                {
                    Debug.Log("�M�~�b�N��������");

                    punchRayHit.collider.gameObject.GetComponent<GimmickManager>().GimmickHit(NowPunchPower,true,punch);
                }
            }

        }
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
    }

    /// <summary>
    /// �p���`�͂��擾����
    /// </summary>
    private void NowPunch()
    {
        punchnowTime += Time.deltaTime;

        if(punchTime <= punchnowTime)
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

}
