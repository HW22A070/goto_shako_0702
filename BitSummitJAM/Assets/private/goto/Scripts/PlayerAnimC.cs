using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimC : MonoBehaviour
{
    private bool _jPLock;

    private GraphicC _gurC;
    private Mover _moverC;
    private PlayerPunch _playerPunchC;
    private Jumper _jumperC;
    private PlayerManager _playermanagerC;
    private ShrimpDush _shrimpDushC;

    // Start is called before the first frame update
    void Start()
    {
        _gurC = GetComponent<GraphicC>();
        _moverC = GetComponent<Mover>();
        _playerPunchC= GetComponent<PlayerPunch>();
        _jumperC = GetComponent<Jumper>();
        _playermanagerC = GetComponent<PlayerManager>();
        _shrimpDushC = GetComponent<ShrimpDush>();
    }

    // Update is called once per frame
    void Update()
    {
        //�_���[�W�Ђ��
        if (_playermanagerC.GraphicSender())
        {
            if (_gurC._animationMode != 3) _gurC.ResetAnimation(3);
        }
        //�ʏ�p���`
        else if (_playerPunchC.GraphicSender())
        {
            if (_jumperC.Jumppunch || _jPLock)
            {
                if (_gurC._animationMode != 7) _gurC.ResetAnimation(7);
                _jPLock = true;
            }

            else if (_playerPunchC.SonicPunch)
            {
                if (_gurC._animationMode != 8) _gurC.ResetAnimation(8);

            }
            else
            {
                if (_gurC._animationMode != 2) _gurC.ResetAnimation(2);
            }
        }
        //�Ռ��g�`���[�W
        else if (_playerPunchC.Charge)
        {
            if (_playerPunchC.GraphicSender2() >= 0f && _playerPunchC.GraphicSender2() <= 180f)
            {
                if (_gurC._animationMode != 6) _gurC.ResetAnimation(6);
            }
            else
            {
                if (_gurC._animationMode != 9) _gurC.ResetAnimation(9);
            }
        }
        //�ʏ�`���[�W
        else if (_playerPunchC.punch > 0)
        {
            if (_moverC.GraphicSender() >= 0.5f || _moverC.GraphicSender() <= -0.5f)
            {
                if (_gurC._animationMode != 13) _gurC.ResetAnimation(13);
            }
            else if (_gurC._animationMode != 5) _gurC.ResetAnimation(5);
        }
        //�G�r�_�b�V��
        else if (_shrimpDushC.GraphicSender())
        {
            if (_gurC._animationMode != 12) _gurC.ResetAnimation(12);
        }
        //�ړ�
        else if (_moverC.GraphicSender() >= 0.5f || _moverC.GraphicSender() <= -0.5f)
        {
            if (_gurC._animationMode != 4) _gurC.ResetAnimation(4);
        }
        //�ҋ@
        else
        {
            _gurC.ResetAnimation(0);
        }

        if(_gurC._animationMode != 7)
        {
            _jPLock = false;
        }
    }
}
