using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrubC : MonoBehaviour
{
    /// <summary>
    /// �J�j�ʏ�s�����[�h
    /// -2=��
    /// -1=��e
    /// 0=�ʏ�
    /// 1=�U��
    /// </summary>
    private int _crubMode;

    [SerializeField]
    [Tooltip("���s�X�s�[�h(cm/f)")]
    private float _walkSpeed;

    [SerializeField]
    [Tooltip("U�^�[������ړ�����(0������Ɩ����ɐi��)")]
    private float _turnBackDistance;

    /// <summary>
    /// �����̍��W
    /// </summary>
    private Vector3 _positionOwnNow;

    /// <summary>
    /// �����̏����ʒu
    /// </summary>
    private Vector3 _positionOwnFirst;

    /// <summary>
    /// �J�j���C�L���X�g��`
    /// </summary>
    private Ray _crubRay;

    /// <summary>
    /// �J�jto�v���C���[�Փ˔���
    /// </summary>
    private RaycastHit2D _crubToPlayerRayHit;

    /// <summary>
    /// �J�jto�ǏՓ˔���
    /// </summary>
    private RaycastHit2D _crubToWallRayHitR, _crubToWallRayHitL;

    /// <summary>
    /// �J�jto���Փ˔���
    /// </summary>
    private RaycastHit2D _crubToFloorRayHit;

    [SerializeField]
    [Tooltip("�J�j�X�v���C�g�ύX��")]
    private SpriteRenderer _spriteCrub;

    /// <summary>
    /// GraphicC�o�^
    /// </summary>
    //private GraphicC _graphicScrpt;

    [SerializeField]
    [Tooltip("���ʂƂ��̏����W�����v���x")]
    private float _deathMoveDeltaY = 0.03f;

    [SerializeField]
    [Tooltip("���ʂƂ��̗������x�{��")]
    private float _deathMoveSpeed = 1;

    [SerializeField]
    private AudioClip _deathS;

    /// <summary>
    /// �d��
    /// </summary>
    private float _gravityScale = 0;

    [SerializeField,Tooltip("�d�͍ő�l�A�d�͉����x")]
    private float _gravityMax=0.02f, _gravityDelta=0.01f;


    // Start is called before the first frame update
    void Start()
    {
        //�����ʒu�o�^
        _positionOwnFirst = transform.localPosition;

        //��Ƀ_���[�W����炤
        GetComponent<EnemyCoreC>().IsGetDamage = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!MenuC.Pouse)
        {
            //���݈ʒu�X�V
            _positionOwnNow = transform.localPosition;

            //�ʏ펞
            if (_crubMode == 0)
            {
                //�ړ�
                transform.position += transform.right * _walkSpeed;

                //���WU�^�[��
                if ((_positionOwnNow.x >= _positionOwnFirst.x + _turnBackDistance
                    || _positionOwnNow.x < _positionOwnFirst.x) && _turnBackDistance != 0)
                {
                    TurnBack();
                }

                //���C��`
                _crubRay = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));

                //�ǂ����蔻��i��������������Ԃ��j
                _crubToWallRayHitR = Physics2D.Raycast(_crubRay.origin + transform.right * 1.2f*transform.localScale.x - transform.up, _crubRay.direction, 10, 8);
                _crubToWallRayHitL = Physics2D.Raycast(_crubRay.origin - transform.right * 1.2f * transform.localScale.x - transform.up, _crubRay.direction, 10, 8);
                if (_crubToWallRayHitR || _crubToWallRayHitL)
                {
                    TurnBack();
                }

                //�������蔻��i������Ȃ��Ȃ���������Ԃ��j
                _crubToFloorRayHit = Physics2D.Raycast(_crubRay.origin - transform.up * 1.2f * transform.localScale.x, _crubRay.direction, 10, 8);
                if (_crubToFloorRayHit)
                {
                    _gravityScale = 0;
                }
                else
                {
                    transform.position -= transform.up * _gravityScale;
                    //�d��
                    if (_gravityScale < _gravityMax) _gravityScale += _gravityDelta;
                    else _gravityScale = _gravityMax;
                    Debug.Log("�R");
                    TurnBack();
                }

                //�v���C���[�����蔻��i����������U���j
                _crubToPlayerRayHit = Physics2D.Raycast(_crubRay.origin -= transform.up * 0.5f * transform.localScale.x, _crubRay.direction, 10, 128);
                if (_crubToPlayerRayHit)
                {
                    Debug.Log("�G���H");
                    _crubMode = 1;
                    StartCoroutine(AttackAnimation(_crubToPlayerRayHit.collider.gameObject));
                }
            }

            //�m�b�N�o�b�N
            else if (_crubMode == -1)
            {
                //�������蔻��i������Ȃ��Ȃ����痎����j
                /*
                _crubRay = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));
                _crubToFloorRayHit = Physics2D.Raycast(_crubRay.origin - transform.up * 1.3f, _crubRay.direction, 10, 8);
                if (!_crubToFloorRayHit)
                {
                    transform.position += new Vector3(0, -0.01f, 0);
                }
                */
            }

            //���s��
            else if (_crubMode == -2)
            {
                _spriteCrub.flipY = true;
                _deathMoveSpeed -= _deathMoveDeltaY;
                //�Ђ�����Ԃ��ė����Ă䂭
                transform.position += new Vector3(0, _deathMoveSpeed, 0);
            }
        }

    }


    /// <summary>
    /// U�^�[��
    /// </summary>
    private void TurnBack()
    {
        transform.position += -transform.right * _walkSpeed * 2;
        _walkSpeed *= -1;
        if (_walkSpeed < 0) _spriteCrub.flipX = false;
        else _spriteCrub.flipX = true;
    }

    /// <summary>
    /// �U��
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <returns></returns>
    private IEnumerator AttackAnimation(GameObject targetPlayer)
    {
        //Debug.Log("�J�j���������I");
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().name);
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().Hp);
        targetPlayer.GetComponentInParent<PlayerManager>(). PlayerDamage(1);
        yield return new WaitForSeconds(0.5f);
        _crubMode = 0;
    }

    /// <summary>
    /// ��_���[�W
    /// </summary>
    public void GetDamage()
    {
        if (_crubMode >= 0)
        {
            StartCoroutine(KnockBack());
        }
    }

    /// <summary>
    /// �m�b�N�o�b�N
    /// </summary>
    /// <returns></returns>
    private IEnumerator KnockBack()
    {
        GetComponent<GraphicC>().ResetAnimation(3);
        //transform.position += transform.up * 1.0f;
        _crubMode = -1;
        yield return new WaitForSeconds(1.0f);
        _crubMode = 0;
        GetComponent<GraphicC>().ResetAnimation(0);

    }

    /// <summary>
    /// ���A�N�V�����X�^�[�g
    /// </summary>
    public void OnDeath()
    {
        StopAllCoroutines();
        FindObjectOfType<AudioSource>().PlayOneShot(_deathS);
        _crubMode = -2;
        Destroy(gameObject,1.0f);
    }
}
