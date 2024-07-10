using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicC : MonoBehaviour
{
    [Header("0=�ʏ펞\n1=��\n2=�U��\n3=�Ђ��\n4=����\n5=�ʏ�`���[�W\n6=�Ռ��g�`���[�W\n7=�W�����v�p���`\n8 = �Ռ��g�p���`" +
        "\n9 = �n�ʃ`���[�W\n10=�U������\n11=�X�^��\n12=�G�r�_�b�V��\n13=�ʏ�`���[�W��")]
    public int _animationMode = 0;
    
    [SerializeField, Tooltip("�摜�J�ڑ��x�i�b�j")]
    public float[] _graphicChangeDeray;

    /// <summary>
    /// �摜�J�ڕϐ�
    /// </summary>
    private float _graphicChangeTimer=0;

    [SerializeField, Tooltip("�A�j���[�V�����Ώ�")]
    private SpriteRenderer _spriteRenderer;

    [SerializeField, Tooltip("�e���[�V�����摜")]
    private Sprite[] _normalGur, _deathGur, _attackGur, _damageGur
        , _walkGur, _charge1Gur, _chargeSonicGur, _jumpPGur, _sonicPGur, _chargeJumpGur
        , _attackChargeGur, _stunGur, _dashGur
        , _charge1WGur;

    [SerializeField, Tooltip("�A�j���[�V�������肩����")]
    readonly bool[] _isAnimLoop= { true, false, false, true, true, false, false, false, false, false, true, false,false,true};

    /// <summary>
    /// �A�j���[�V�����摜�Y
    /// </summary>
    private Sprite[] _animationGraS;

    /// <summary>
    /// ���s�R���[�`��
    /// </summary>
    private Coroutine _currentCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        /*
        for(int j = 0;j<=_normalGur.Length;j++)
        {
            _animationGraS[0, j] = _normalGur[j];
        }
        for (int j = 0; j <= _deathGur.Length; j++)
        {
            _animationGraS[0, j] = _deathGur[j];
        }
        for (int j = 0; j <= _normalGur.Length; j++)
        {
            _animationGraS[0, j] = _attackGur[j];
        }
        */
        AnimationChanger();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Animation(float deray, Sprite[] animationGra,bool isLoop)
    {
        for (int anim = 0; anim < animationGra.Length; anim++) {
            _spriteRenderer.sprite = animationGra[anim];
            yield return new WaitForSeconds(deray);
        }
        if(isLoop) AnimationChanger();
    }

    /// <summary>
    /// �A�j���[�V�����ω�
    /// </summary>
    private void AnimationChanger()
    {

        if(_animationMode==0) _animationGraS = _normalGur;
        else if(_animationMode==1)_animationGraS = _deathGur;
        else if(_animationMode==2)_animationGraS = _attackGur;
        else if(_animationMode==3)_animationGraS= _damageGur;
        else if(_animationMode==4)_animationGraS= _walkGur;
        else if(_animationMode==5)_animationGraS= _charge1Gur;
        else if(_animationMode==6)_animationGraS= _chargeSonicGur;
        else if(_animationMode==7)_animationGraS= _jumpPGur;
        else if(_animationMode==8)_animationGraS= _sonicPGur;
        else if(_animationMode==9)_animationGraS= _chargeJumpGur;
        else if (_animationMode == 10) _animationGraS = _attackChargeGur;
        else if (_animationMode == 11) _animationGraS = _stunGur;
        else if (_animationMode == 12) _animationGraS = _dashGur;
        else if (_animationMode == 13) _animationGraS = _charge1WGur;
        _currentCoroutine = StartCoroutine(Animation(_graphicChangeDeray[_animationMode], _animationGraS, _isAnimLoop[_animationMode]));
    }

    /// <summary>
    /// �A�j���[�V�������[�h�ύX
    /// 0=�ʏ펞
    /// 1=��
    /// 2=�U��
    /// 3=�Ђ��
    /// 4=����
    /// 5=�ʏ�`���[�W
    /// 6=�Ռ��g�`���[�W
    /// 7=�W�����v�p���`
    /// 8=�Ռ��g�p���`
    /// 9=�n�ʃ`���[�W
    /// 10=�U������
    /// 11=�X�^��
    /// 12=�G�r�_�b�V��
    /// 13=�ʏ�W�`���[�W
    /// </summary>
    public void ResetAnimation(int resetAnimationMode)
    {
        if (resetAnimationMode != _animationMode)
        {
            Debug.Log(resetAnimationMode);
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;

            _animationMode = resetAnimationMode;
            AnimationChanger();
        }
    }
}
