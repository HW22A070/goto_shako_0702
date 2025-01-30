using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchEffectC : MonoBehaviour
{
    /// <summary>
    /// アニメーション
    /// </summary>
    private Animator _anmtrEffect;

    private PlayerPunch _cPlayerPunch;
    
    void Start()
    {
        _anmtrEffect = gameObject.GetComponent<Animator>();
        _cPlayerPunch = GameObject.Find("PlayerManager").GetComponent<PlayerPunch>();
    }
    
    void Update()
    {
        _anmtrEffect.SetInteger("ChargeMode",_cPlayerPunch.PunchLevelSender());
        if (_cPlayerPunch.PunchLevelSender() <= 0)
        {
            Destroy(gameObject);
        }
    }
}
