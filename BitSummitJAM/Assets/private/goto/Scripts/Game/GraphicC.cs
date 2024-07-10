using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicC : MonoBehaviour
{
    [Header("0=通常時\n1=死\n2=攻撃\n3=ひるみ\n4=歩き\n5=通常チャージ\n6=衝撃波チャージ\n7=ジャンプパンチ\n8 = 衝撃波パンチ" +
        "\n9 = 地面チャージ\n10=攻撃準備\n11=スタン\n12=エビダッシュ\n13=通常チャージ歩")]
    public int _animationMode = 0;
    
    [SerializeField, Tooltip("画像遷移速度（秒）")]
    public float[] _graphicChangeDeray;

    /// <summary>
    /// 画像遷移変数
    /// </summary>
    private float _graphicChangeTimer=0;

    [SerializeField, Tooltip("アニメーション対象")]
    private SpriteRenderer _spriteRenderer;

    [SerializeField, Tooltip("各モーション画像")]
    private Sprite[] _normalGur, _deathGur, _attackGur, _damageGur
        , _walkGur, _charge1Gur, _chargeSonicGur, _jumpPGur, _sonicPGur, _chargeJumpGur
        , _attackChargeGur, _stunGur, _dashGur
        , _charge1WGur;

    [SerializeField, Tooltip("アニメーションくりかえし")]
    readonly bool[] _isAnimLoop= { true, false, false, true, true, false, false, false, false, false, true, false,false,true};

    /// <summary>
    /// アニメーション画像ズ
    /// </summary>
    private Sprite[] _animationGraS;

    /// <summary>
    /// 実行コルーチン
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
    /// アニメーション変化
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
    /// アニメーションモード変更
    /// 0=通常時
    /// 1=死
    /// 2=攻撃
    /// 3=ひるみ
    /// 4=歩き
    /// 5=通常チャージ
    /// 6=衝撃波チャージ
    /// 7=ジャンプパンチ
    /// 8=衝撃波パンチ
    /// 9=地面チャージ
    /// 10=攻撃準備
    /// 11=スタン
    /// 12=エビダッシュ
    /// 13=通常Wチャージ
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
