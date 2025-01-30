using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundC : MonoBehaviour
{
    [SerializeField, Tooltip("音声")]
    private AudioClip _acCharge1, _acCharge2, _acCharge3, _acGetDamage, _acDash, _acHit, _acJump, _acPunch, _acPunchJump, _acSonicFire;

    /// <summary>
    /// PlayerManagerのAudioSource取得
    /// </summary>
    private AudioSource _pmAsource;

    // Start is called before the first frame update
    void Start()
    {
        //AC取得
        _pmAsource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 0=チャージ１
    /// 1=チャージ２
    /// 2=チャージ３
    /// 3=ダメージ
    /// 4=ダッシュ
    /// 5=命中
    /// 6=ジャンプ
    /// 7=パンチ
    /// 8=パンチジャンプ
    /// 9=衝撃波発射
    /// </summary>
    /// <param name="soundnumber"></param>
    public void SoundStart(int soundnumber)
    {
        AudioClip[] _audios = { _acCharge1, _acCharge2, _acCharge3, _acGetDamage, _acDash, _acHit, _acJump, _acPunch, _acPunchJump, _acSonicFire };
        _pmAsource.PlayOneShot(_audios[soundnumber]);
    }
}
