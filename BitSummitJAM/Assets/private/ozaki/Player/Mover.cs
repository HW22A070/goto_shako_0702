using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Mover : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundMaskNumber;


    [SerializeField]
    [Tooltip("��������")]
    private float _speed;

    [SerializeField]
    [Tooltip("�󒆋@����")]
    private float skymove;

    [SerializeField]
    [Tooltip("�����x")]
    private float acceleration;

    [SerializeField]
    [Tooltip("�ō����x")]
    private float _maxSpeed;

    [SerializeField]
    [Tooltip("�󒆍ō����x")]
    private float _skyMaxSpeed;

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
        LayerMask mask = 1 << groundMaskNumber;
    }

    private void Update()
    {
        //�ǂɐG��Ă�����
        if (CheckWall_Right() && _speed > 0)
        {
            _speed = 0;

            punch = 0;
        }
        if (CheckWall_Left() && _speed < 0)
        {
            _speed = 0;

            punch = 0;
        }

        //LB�{�^���������ꂽ��
        if (Input.GetKeyUp("joystick button 4"))
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

        //if (Ground == false)
        //{
        //    if (Physics2D.Raycast(playerPos, -transform.up, transform.localScale.y))
        //    {
        //        punch = 0;
        //    }
        //}

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
            _speed = 0;
        }

        //�n�ʂ��牓����
        if (Ground == false)
        {
            if (Mathf.Abs(punch) >= _skyMaxSpeed - 5)
            {
                if (punch >= 0)
                {
                    punch = _skyMaxSpeed - 5;
                }
                if (punch <= 0)
                {
                    punch = -_skyMaxSpeed + 5;
                }
            }

            //�������� * �p���`�ړ��X�s�[�h * ����
            transform.position += transform.right * (_speed + punch) * Time.fixedDeltaTime;

            _speed += Input.GetAxisRaw("Horizontal") * skymove;

        }

        //�n�ʂɋ߂���
        else
        {
            if (Input.GetAxis("Horizontal") == 0)
            {
                if (_speed == 0)
                {
                    return;
                }
                if (_speed > 0)
                {
                    _speed -= 0.5f;
                }
                if (_speed < 0)
                {
                    _speed += 0.5f;
                }
                if(Mathf.Abs(_speed) <= 0.5)
                {
                    _speed = 0;
                }
            }

            //�������� * ����
            transform.position += transform.right * _speed * Time.fixedDeltaTime;
        }

        //�X�s�[�h�ɉ����x��ǉ�
        _speed += acceleration * Input.GetAxisRaw("Horizontal");

        //�n�ʂɂ���Ƃ��̍ō����x���w��
        if (Mathf.Abs(_speed) >= _maxSpeed && Ground)
        {
            if (_speed > 0)
            {
                _speed = _maxSpeed;
            }
            else
            {
                _speed = -_maxSpeed;
            }
        }


        //�n�ʂ��痣��Ă��鎞�̍ō����x���w��
        else if (Mathf.Abs(_speed) >= _skyMaxSpeed && Ground == false)
        {
            if (_speed > 0)
            {
                _speed = _skyMaxSpeed;
            }
            else
            {
                _speed = -_skyMaxSpeed;
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

    public void KnockBack(bool hoge)
    {
        if (hoge)
        {
            this.transform.DOMoveX(2f, 0.5f);
        }
        else
        {
            this.transform.DOMoveX(-2f, 0.5f);
        }
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
        return _speed;
    }
}
