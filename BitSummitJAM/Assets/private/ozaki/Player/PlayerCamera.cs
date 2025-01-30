using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float pulusPos;

    private Vector3 CameraPos;

    private float movetime;

    private PlayerManager playerManager;

    bool isRight;

    [SerializeField]
    public bool Jump;

    private Mover move;
    // Start is called before the first frame update
    void Start()
    {
        CameraPos = transform.position;

        playerManager = PlayerManager.Instance;

        move = player.GetComponent<Mover>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            var hoge = Input.GetAxisRaw("Horizontal");
            //ポジション+5
            if (hoge > 0)
            {
                var pos = player.transform.position.x + pulusPos;

                this.transform.DOMoveX(pos, 1f).SetEase(Ease.Linear);

                CameraPos = this.transform.position;
            }
            else if (hoge < 0)
            {
                var pos = player.transform.position.x - pulusPos;

                this.transform.DOMoveX(pos, 1f).SetEase(Ease.Linear);

                CameraPos = this.transform.position;
            }

            else if (hoge == 0)
            {
                var pos = player.transform.position.x;

                this.transform.DOMoveX(pos, 1f).SetEase(Ease.Linear);

                CameraPos = this.transform.position;

            }
            //ポジション-4
            if (Jump)
            {
                var pos = player.transform.position.y - pulusPos + 3.0f;


                //this.transform.DOMoveY(pos, 0.2f).SetEase(Ease.OutQuad);

                CameraPos.y = pos;


                //CameraPos = this.transform.position;
            }
            //ポジション+2
            else
            {
                var pos = player.transform.position.y + 2.0f;

                this.transform.DOMoveY(pos, 0.5f).SetEase(Ease.Linear);

                CameraPos = this.transform.position;
            }

            transform.position = CameraPos;
        }
    }

    public void isJump()
    {
        Jump = true;
    }

    public void isGround()
    {
        Jump = false;
    }

    public void Stop()
    {
        this.gameObject.transform.parent = null;

        player = null;
    }

    public bool IsJump()
    {
        return Jump;
    }
}
