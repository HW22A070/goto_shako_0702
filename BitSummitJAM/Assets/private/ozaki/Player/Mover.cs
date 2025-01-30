using DG.Tweening;
using UnityEngine;
public class Mover : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�n�ʂ̃��C���[�ݒ�")]
    private LayerMask groundMaskNumber;

    [SerializeField]
    [Tooltip("��������")]
    private float speed;

    [SerializeField]
    [Tooltip("�󒆋@����")]
    private float skymove;

    [SerializeField]
    [Tooltip("�����x")]
    private float acceleration;

    [SerializeField]
    [Tooltip("�ō����x")]
    private float maxSpeed;

    [SerializeField]
    [Tooltip("�󒆍ō����x")]
    private float skyMaxSpeed;

    [SerializeField]
    [Tooltip("�n�ʂɂ��Ă��邩")]
    private bool Ground;

    /// <summary>
    /// ���b�n�ʂɂ��邩����p
    /// </summary>
    private float GroundTime;

    [SerializeField]
    private float punch;

    private float horizontal;
    [SerializeField]
    [Tooltip("�p���`�W�����v1")]
    private float punchone;
    [SerializeField]
    [Tooltip("�p���`�W�����v2")]
    private float punchtwo;
    [SerializeField]
    [Tooltip("�p���`�W�����v3")]
    private float punchthree;

    private Vector3 playerPos;
    // Start is called before the first frame update
    void Start()
    {
        //�|�W�V�����X�V
        playerPos = transform.position;

        //�O���E���h�ɂ����Ԃŏ�����
        Ground = true;

        //�n�ʂ�Raycast�p�̃}�X�N���擾
        LayerMask mask = groundMaskNumber;
    }

    private void Update()
    {
        //�ǂɐG��Ă�����
        if (CheckWall_Right() && speed > 0)
        {
            speed = 0;

            punch = 0;
        }
        if (CheckWall_Left() && speed < 0)
        {
            speed = 0;

            punch = 0;
        }

        //LB�{�^���������ꂽ��
        if (Input.GetKeyUp("joystick button 4") && !PlayerManager.IsPlayerMoveRock)
        {
            //�p���`���擾
            int hoge = GetComponent<PlayerPunch>().punch;

            //L�X�e�B�b�N���͂��擾
            horizontal = -Input.GetAxis("Horizontal");

            if (hoge == 1)
            {
                punch = punchone * horizontal;
            }
            else if (hoge == 2)
            {
                punch = punchtwo * horizontal;
            }
            else if (hoge == 3)
            {
                punch = punchthree * horizontal;
            }

        }

        //�n�ʂɋ߂����̊m�F
        if (Physics2D.Raycast(playerPos, -transform.up, transform.localScale.y,groundMaskNumber))
        {
            //�߂����
            Ground = true;            

            GroundTime += Time.deltaTime;

            if (GroundTime > 0.1f)
            {
                punch = 0;

                GroundTime = 0;
            }
        }
        else
        {
            //����Ă�����
            Ground = false;
        }

        //���W���X�V
        playerPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //���������߂�Ƃ�
        if (GetComponent<PlayerPunch>().Charge)
        {
            //����������0��
            speed = 0;
        }

        //�n�ʂ��牓����
        if (Ground == false)
        {
            if (Mathf.Abs(punch) >= skyMaxSpeed - 5)
            {
                if (punch >= 0)
                {
                    punch = skyMaxSpeed - 5;
                }
                if (punch <= 0)
                {
                    punch = -skyMaxSpeed + 5;
                }
            }

            //�������� * �p���`�ړ��X�s�[�h * ����
            transform.position += transform.right * (speed + punch) * Time.fixedDeltaTime;

            if (!PlayerManager.IsPlayerMoveRock) speed += Input.GetAxisRaw("Horizontal") * skymove;
            else  speed = 0;

        }

        //�n�ʂɋ߂���
        else
        {
            if (Input.GetAxis("Horizontal") == 0)
            {
                if (speed == 0)
                {
                    return;
                }
                if (speed > 0)
                {
                    speed -= 0.5f;
                }
                if (speed < 0)
                {
                    speed += 0.5f;
                }
                if(Mathf.Abs(speed) <= 0.5)
                {
                    speed = 0;
                }
            }

            //�������� * ����
            transform.position += transform.right * speed * Time.fixedDeltaTime;
        }

        //�X�s�[�h�ɉ����x��ǉ�
        if(!PlayerManager.IsPlayerMoveRock)speed += acceleration * Input.GetAxisRaw("Horizontal");

        //�n�ʂɂ���Ƃ��̍ō����x���w��
        if (Mathf.Abs(speed) >= maxSpeed && Ground)
        {
            if (speed > 0)
            {
                speed = maxSpeed;
            }
            else
            {
                speed = -maxSpeed;
            }
        }


        //�n�ʂ��痣��Ă��鎞�̍ō����x���w��
        else if (Mathf.Abs(speed) >= skyMaxSpeed && Ground == false)
        {
            if (speed > 0)
            {
                speed = skyMaxSpeed;
            }
            else
            {
                speed = -skyMaxSpeed;
            }
        }
    }

    private bool CheckWall_Right()
    {
        var rayTuple = CreateRay_Right();

        return
                Physics2D.Raycast(
                    rayTuple.Item1.origin,
                    rayTuple.Item1.direction,
                    this.transform.localScale.x * 0.6f,
                    groundMaskNumber) ||
                Physics2D.Raycast(
                    rayTuple.Item2.origin,
                    rayTuple.Item2.direction,
                    this.transform.localScale.x * 0.6f,
                    groundMaskNumber) ||
                Physics2D.Raycast(
                    rayTuple.Item3.origin,
                    rayTuple.Item3.direction,
                    this.transform.localScale.x * 0.6f,
                    groundMaskNumber);
    }

    private bool CheckWall_Left()
    {
        var rayTuple = CreateRay_Left();

        return
            Physics2D.Raycast(
                rayTuple.Item1.origin,
                rayTuple.Item1.direction,
                this.transform.localScale.x * 0.6f,
                groundMaskNumber) ||
            Physics2D.Raycast(
                rayTuple.Item2.origin,
                rayTuple.Item2.direction,
                this.transform.localScale.x * 0.6f,
                groundMaskNumber) ||
            Physics2D.Raycast(
                rayTuple.Item3.origin,
                rayTuple.Item3.direction,
                this.transform.localScale.x * 0.6f,
                groundMaskNumber);
    }


    /// <summary>
    /// �E������Ray�𐶐�����֐�
    /// </summary>
    /// <returns>
    /// Item1 = �������ď�[�����_��Ray
    /// Item2 = �������ĉ��[�����_��Ray
    /// Item3 = ���S�����_��Ray
    /// </returns>
    private (Ray, Ray, Ray) CreateRay_Right()
    {
        //�E����Ray�̌��_���擾
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.y += this.transform.localScale.y * 0.4f;

        //������Ray�̌��_���擾
        Vector3 leftOrigin = this.transform.position;
        leftOrigin.y -= this.transform.localScale.y * 0.4f;

        //�쐬����Ray2��Ԃ�
        return (new Ray(rightOrigin, this.transform.right),
                new Ray(leftOrigin, this.transform.right),
                new Ray(this.transform.position, this.transform.right));
    }

    /// <summary>
    /// ��������Ray�𐶐�����֐�
    /// </summary>
    /// <returns>
    /// Item1 = �������ď�[�����_��Ray
    /// Item2 = �������ĉ��[�����_��Ray
    /// Item3 = ���S�����_��Ray
    /// </returns>
    private (Ray, Ray, Ray) CreateRay_Left()
    {
        //�E����Ray�̌��_���擾
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.y += this.transform.localScale.y * 0.4f;

        //������Ray�̌��_���擾
        Vector3 leftOrigin = this.transform.position;
        leftOrigin.y -= this.transform.localScale.y * 0.4f;

        //�쐬����Ray2��Ԃ�
        return (new Ray(rightOrigin, -this.transform.right),
                new Ray(leftOrigin, -this.transform.right),
                new Ray(this.transform.position, -this.transform.right));
    }

    public float GraphicSender()
    {
        return speed;
    }


    public void Goal(Vector3 hoge)
    {
        this.transform.DOMove(new Vector3(hoge.x, hoge.y, hoge.z), 3.0f).SetEase(Ease.Linear);
    }
}
