using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundC : MonoBehaviour
{
    [SerializeField, Tooltip("����")]
    private AudioClip _acCharge1, _acCharge2, _acCharge3, _acGetDamage, _acDash, _acHit, _acJump, _acPunch, _acPunchJump, _acSonicFire;

    /// <summary>
    /// PlayerManager��AudioSource�擾
    /// </summary>
    private AudioSource _pmAsource;

    // Start is called before the first frame update
    void Start()
    {
        //AC�擾
        _pmAsource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 0=�`���[�W�P
    /// 1=�`���[�W�Q
    /// 2=�`���[�W�R
    /// 3=�_���[�W
    /// 4=�_�b�V��
    /// 5=����
    /// 6=�W�����v
    /// 7=�p���`
    /// 8=�p���`�W�����v
    /// 9=�Ռ��g����
    /// </summary>
    /// <param name="soundnumber"></param>
    public void SoundStart(int soundnumber)
    {
        AudioClip[] _audios = { _acCharge1, _acCharge2, _acCharge3, _acGetDamage, _acDash, _acHit, _acJump, _acPunch, _acPunchJump, _acSonicFire };
        _pmAsource.PlayOneShot(_audios[soundnumber]);
    }
}
