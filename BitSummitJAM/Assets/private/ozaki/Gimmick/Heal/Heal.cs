using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    private PlayerManager playerManager;

    [SerializeField]
    private GameObject player;

    private bool isHealable;

    [SerializeField]
    [Tooltip("ホタテ")]
    private GameObject scallops;

    private Ray ray;
    
    private LayerMask playerMask;


    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.Find("PlayerManager");

        playerManager = PlayerManager.Instance;

        isHealable = true;

        playerMask = 1 << 7;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Hitjudge(player.transform,this.gameObject) && isHealable)
        {
            HealPlayer();

            Debug.Log("プレイヤー回復");
        }
    }

    public void HealPlayer()
    {
        playerManager.HpHeal();

        isHealable = false;

        Destroy(scallops);
        
    }

    /// <summary>
    /// プレイヤーと当たっているか判定
    /// </summary>
    /// <param name="target">プレイヤー</param>
    /// <param name="other">自分</param>
    /// <returns></returns>
    private bool Hitjudge(Transform target, GameObject other)
    {
        //距離を求める
        Vector2 distance = new(Mathf.Abs(target.position.x - other.transform.position.x),
                               Mathf.Abs(target.position.y - other.transform.position.y));

        //サイズの和を求める
        Vector2 size_sum = new((target.localScale.x + other.transform.localScale.x) / 2,
                               (target.localScale.y + other.transform.localScale.y) / 2);

        //当たっているかどうかを返す
        return (distance.x < size_sum.x && distance.y < size_sum.y) ? true : false;
    }
}
