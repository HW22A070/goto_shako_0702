using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// �W�����v���Ǘ�����N���X
/// </summary>
public class Jumper : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�W�����v�̏���")]
    private float _v0;

    [SerializeField, Min(0.001f)]
    [Tooltip("�W�����v��")]
    private float _jumpPow;

    [SerializeField]
    [Tooltip("�d��")]
    private float _gravity;

    [SerializeField, Min(1)]
    [Tooltip("��R��")]
    private float _resistance;

    [SerializeField]
    [Tooltip("����")]
    private float _mass;

    [SerializeField, Min(1)]
    [Tooltip("�W�����v�\�ȉ�")]
    private int _defaultJumpCount;

    /// <summary>
    /// L�X�e�B�b�N�̓��͎擾�p
    /// </summary>
    private float vertical;

    /// <summary>
    /// �c�����̑��x
    /// </summary>
    private float _verticalVelocity = 0;

    /// <summary>
    /// �I�[���x
    /// </summary>
    private float _finalVelocity;

    /// <summary>
    /// �O��̐ڒn����̌���
    /// </summary>
    private bool _isLastGround = true;
    /// <summary>
    /// �W�����v���i��ׂ̃^�C�}�[
    /// </summary>
    private float _jumpTimer = 0;

    /// <summary>
    /// �c��W�����v�\��
    /// </summary>
    private int _jumpCount;

    /// <summary>
    /// "Ground"���C���[�̔ԍ�
    /// </summary>
    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private bool jumppunch = false;
    public bool Jumppunch
    {
        get => jumppunch; set => jumppunch = value;
    }

    [SerializeField]
    private float punch;

    [SerializeField]
    [Tooltip("�p���`�W�����v1")]
    private float punchone;
    [SerializeField]
    [Tooltip("�p���`�W�����v2")]
    private float punchtwo;
    [SerializeField]
    [Tooltip("�p���`�W�����v3")]
    private float punchthree;
    // Start is called before the first frame update
    private void Start()
    {
        //�I�[���x���擾
        UpdateFinalVelocity();

        //�W�����v�񐔂�������
        _jumpCount = _defaultJumpCount;

    }

    //GetKeyUp�ȂǁAFixedUpdate�œs�������Ȃ����̂͂������ɏ���
    private void Update()
    {
#if UNITY_EDITOR

        //�f�o�b�O�p�Ɉ��̍�������������烍�O���o���ăv���C���[�h�I��
        if (this.transform.position.y < -20)
        {
            UnityEditor.EditorApplication.isPlaying = false;

            Debug.Log("�v���C���[�̍�����������������ׁAplay���[�h���I�����܂���");
        }

#endif

        //�W�����v�L�[�������ꂽ��
        if (Input.GetKeyUp("joystick button 2"))
        {
            _jumpTimer = 0;

            //�����󒆂ŗ����ꂽ��J�E���g�����炷
            if (!CheckGround())
                --_jumpCount;
        }

        if (Input.GetKeyUp("joystick button 4"))
        {
            int hoge = GetComponent<PlayerPunch>().punch;

            if (hoge == 1)
            {
                punch = punchone;
            }
            else if (hoge == 2)
            {
                punch = punchtwo;
            }
            else if (hoge == 3)
            {
                punch = punchthree;
            }

            vertical = -Input.GetAxis("Vertical");

            _jumpTimer = 0;

            //Jumppunch = true;

            //�����󒆂ŗ����ꂽ��J�E���g�����炷
            if (!CheckGround())
                --_jumpCount;

        }

        //�ڒn�󋵂��ω��������H(�������A�W�����v�ɂ���ċ󒆂ɏo���Ƃ��͌��؂��Ȃ�)
        if (_isLastGround != CheckGround())
        {
            //true����false�ɐ؂�ւ�����Ȃ�W�����v�񐔂�1���炷
            //false����true�ɐ؂�ւ�����Ȃ�f�t�H���g�ɖ߂�
            if (_isLastGround)
            {
                //�����W�����v�ŋ󒆂ɏo�ĂȂ��Ȃ�΃J�E���g�𑦍��ɂP���炷
                if (_jumpTimer < Mathf.Epsilon)
                    --_jumpCount;
            }
            else
            {
                _jumpCount = _defaultJumpCount;
            }

            jumppunch = false;
        }

        //�ڒn�󋵂��X�V
        _isLastGround = CheckGround();

        //�n�ʂɖ��܂��Ă����������ɕ␳��������
        if (CheckBury())
            Correction_Up();

        //�V��ɖ��܂��Ă����牺�����ɕ␳��������
        if (OnCeiling())
            Correction_Down();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //�ڒn���Ă��邩�ŕ���
        if (CheckGround())
        {
            //�ڒn���Ă�����c�����̑��x��0�ɂ���
            _verticalVelocity = 0;
        }
        else
        {
            //�d�͂̓K�p
            ApplyingGravity();
        }

        //�W�����v�\�Ȏ��ɁA�W�����v�L�[���������ꂽ��
        if (Input.GetKey("joystick button 2") && JumpAble())
        {
            Jump();
        }

        //�����V��ɂԂ������Ƃ�������Ɉړ����悤�Ƃ��Ă�����
        if (OnCeiling() && _verticalVelocity > 0)
        {
            //���x��0��
            _verticalVelocity = 0;

            //�W�����v�p�̃^�C�}�[�ɏ��������ė]�v�ɃW�����v���Ȃ��悤��
            _jumpTimer = _v0;

            //�W�����v�񐔂����炷
            --_jumpCount;
        }

        if (Jumppunch == true)
        {
            JumpPunch();
        }

        //����
        Move();
    }

    /// <summary>
    /// �I�[���x���X�V����֐�
    /// </summary>
    private void UpdateFinalVelocity()
    {
        //�I�[���x���`
        _finalVelocity = _mass * _gravity / _resistance;
    }

    /// <summary>
    /// �W�����v������֐�
    /// </summary>
    private void Jump()
    {
        GetComponent<PlayerPunch>().PunchReset();

        _jumpTimer += Time.deltaTime / _jumpPow;

        //�����A�^�C�}�[�������𒴂��Ă��瑬�x�ɐG��Ȃ�
        //���������x���オ�邩��
        if (_v0 < _jumpTimer || _verticalVelocity > _finalVelocity) return;

        //�ǉ��̑��x��^����
        _verticalVelocity = _v0 - _jumpTimer;
    }
    /// <summary>
    /// �p���`�`���[�W
    /// </summary>
    public void JumpPunch()
    {
        //�p���`�̏����ɂk�X�e�B�b�N�̓��͂����Z
        float _v1 = _v0 + Mathf.Abs(vertical) * punch;

        _jumpTimer += Time.deltaTime / _jumpPow;

        //�����A�^�C�}�[�������𒴂��Ă��瑬�x�ɐG��Ȃ�
        //���������x���オ�邩��
        if (_v1 < _jumpTimer || _verticalVelocity > _finalVelocity) return;

        //�ǉ��̑��x��^����
        _verticalVelocity = _v1 - _jumpTimer;

    }

    /// <summary>
    /// ���ۂɓ������֐�
    /// </summary>
    private void Move()
    {
        this.transform.position += this.transform.up * _verticalVelocity;
    }

    /// <summary>
    ///�������ɕ␳��������֐�
    /// </summary>
    private void Correction_Down()
    {
        //�������Ray2���擾
        var rayTupleData = CreateRay_Up();

        //�E������m�F����
        RaycastHit2D hit = Physics2D.Raycast(
                                rayTupleData.Item1.origin,
                                rayTupleData.Item1.direction,
                                this.transform.localScale.y * 0.5f,
                                groundMask);

        //�����Փ˕������m�o������
        if (hit.collider != null)
        {
            //���������ʒu�ƌ��݈ʒu�̒����������擾
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            //�ړ��ʂ��v�Z
            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            //���݂̍��W���擾
            var currentPosition = this.transform.position;

            //�ړ�������̍��W���v�Z
            currentPosition.y -= moveVal;

            //���W���X�V
            this.transform.position = currentPosition;
        }
        else
        {
            hit = Physics2D.Raycast(
                       rayTupleData.Item2.origin,
                       rayTupleData.Item2.direction,
                       this.transform.localScale.y * 0.6f,
                       groundMask);

            //���������ł��I�u�W�F�N�g���擾�o���Ȃ���΂����ŏI��
            if (hit.collider == null) return;

            //���������ʒu�ƌ��݈ʒu�̒����������擾
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            //�ړ��ʂ��v�Z
            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            //���݂̍��W���擾
            var currentPosition = this.transform.position;

            //�ړ�������̍��W���v�Z
            currentPosition.y -= moveVal;

            //���W���X�V
            this.transform.position = currentPosition;
        }
    }

    /// <summary>
    /// ������ɕ␳��������֐�
    /// </summary>
    private void Correction_Up()
    {
        //��������Ray2���擾
        var rayTupleData = CreateRay_Down();

        //�E������m�F����
        RaycastHit2D hit = Physics2D.Raycast(
                                rayTupleData.Item1.origin,
                                rayTupleData.Item1.direction,
                                this.transform.localScale.y * 0.5f,
                                groundMask);

        //�����Փ˕������m�o������
        if (hit.collider != null)
        {
            //���������ʒu�ƌ��݈ʒu�̒����������擾
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            //�ړ��ʂ��v�Z
            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            //���݂̍��W���擾
            var currentPosition = this.transform.position;

            //�ړ�������̍��W���v�Z
            currentPosition.y += moveVal;

            //���W���X�V
            this.transform.position = currentPosition;
        }
        else
        {
            hit = Physics2D.Raycast(
                       rayTupleData.Item2.origin,
                       rayTupleData.Item2.direction,
                       this.transform.localScale.y * 0.5f,
                       groundMask);

            //���������ł��I�u�W�F�N�g���擾�o���Ȃ���΂����ŏI��
            if (hit.collider == null) return;

            //���������ʒu�ƌ��݈ʒu�̒����������擾
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            //�ړ��ʂ��v�Z
            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            //���݂̍��W���擾
            var currentPosition = this.transform.position;

            //�ړ�������̍��W���v�Z
            currentPosition.y += moveVal;

            //���W���X�V
            this.transform.position = currentPosition;
        }
    }

    /// <summary>
    /// �d�͂�K�p����֐�
    /// </summary>
    private void ApplyingGravity()
    {
        //�I�[���x�ɒB���ĂȂ���Ώd�͂�������
        if (_verticalVelocity < _finalVelocity * -1) return;

        _verticalVelocity -= _gravity;

    }

    /// <summary>
    /// �ڒn���Ă��邩��Ԃ�
    /// </summary>
    /// <returns>�n�ʂɐڒn���Ă���</returns>
    private bool CheckGround()
    {
        //��������Ray2���擾
        var rayTupleData = CreateRay_Down();

        //�E�����ǂ��炩�ŃR���C�_�[�ɂԂ�������true����
        return
                Physics2D.Raycast(
                rayTupleData.Item1.origin,
                rayTupleData.Item1.direction,
                this.transform.localScale.y * 0.5f,
                groundMask
                ) ||
                Physics2D.Raycast(
                rayTupleData.Item2.origin,
                rayTupleData.Item2.direction,
                this.transform.localScale.y * 0.5f,
                groundMask
                );
    }

    /// <summary>
    /// �n�ʂɖ��܂��Ă��邩��Ԃ�
    /// </summary>
    /// <returns>�n�ʂɐڒn���Ă���</returns>
    private bool CheckBury()
    {
        //��������Ray2���擾
        var rayTupleData = CreateRay_Down();

        //CheckGround���Z��Ray���΂��Ĕ��f����
        return
                Physics2D.Raycast(
                rayTupleData.Item1.origin,
                rayTupleData.Item1.direction,
                this.transform.localScale.y * 0.5f - 0.01f,
                groundMask
                ) ||
                Physics2D.Raycast(
                rayTupleData.Item2.origin,
                rayTupleData.Item2.direction,
                this.transform.localScale.y * 0.5f - 0.01f,
                groundMask
                );
    }

    /// <summary>
    /// ��������Ray�𐶐�����֐�
    /// </summary>
    /// <returns>
    /// Item1 = �������ĉE�[�����_��Ray
    /// Item2 = �������č��[�����_��Ray
    /// </returns>
    private (Ray, Ray) CreateRay_Down()
    {
        //�E����Ray�̌��_���擾
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.x += this.transform.localScale.x * 0.4f;

        //������Ray�̌��_���擾
        Vector3 leftOrigin = this.transform.position;
        leftOrigin.x -= this.transform.localScale.x * 0.4f;

        //�쐬����Ray2��Ԃ�
        return (new Ray(rightOrigin, -this.transform.up),
                new Ray(leftOrigin, -this.transform.up));
    }

    /// <summary>
    /// Ray�𐶐�����֐�
    /// </summary>
    /// <returns>
    /// Item1 = �������ĉE�[�����_��Ray
    /// Item2 = �������č��[�����_��Ray
    /// </returns>
    private (Ray, Ray) CreateRay_Up()
    {
        //�E����Ray�̌��_���擾
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.x += this.transform.localScale.x * 0.4f;

        //������Ray�̌��_���擾
        Vector3 leftOrigin = this.transform.position;
        leftOrigin.x -= this.transform.localScale.x * 0.4f;

        //�쐬����Ray2��Ԃ�
        return (new Ray(rightOrigin, this.transform.up),
                new Ray(leftOrigin, this.transform.up));
    }

    /// <summary>
    /// �V��ɒB��������Ԃ��֐�
    /// </summary>
    private bool OnCeiling()
    {
        //�������Ray���쐬
        var rayTupleData = CreateRay_Up();

        //�V��ɍ��E�����ꂩ��Ray���Ԃ���������Ԃ�
        return
             Physics2D.Raycast(
                    rayTupleData.Item1.origin,
                    rayTupleData.Item1.direction,
                    this.transform.localScale.y * 0.5f,
                    groundMask) ||
           Physics2D.Raycast(
                    rayTupleData.Item2.origin,
                    rayTupleData.Item2.direction,
                    this.transform.localScale.y * 0.5f,
                    groundMask);


    }

    /// <summary>
    /// �W�����v�\����Ԃ��֐�
    /// </summary>
    private bool JumpAble()
    {
        return _jumpCount > 0;
    }
}