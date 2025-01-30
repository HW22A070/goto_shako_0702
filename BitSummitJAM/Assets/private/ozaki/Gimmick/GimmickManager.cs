using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gimmick
{
    BIGSTONE,
    IVYSTONE,
    IVY,
    BRAKSTONE,
    DOOR,
    HEAL,
}

public class GimmickManager : MonoBehaviour
{
    [SerializeField]
    private Gimmick gimmick;

    private StoneMove stoneMove;

    private StoneBreak stoneBreak;

    private IvyFall ivyFall;

    private Ivy ivy;

    private DoorMove doorMove;

    private Heal heal;

    // Start is called before the first frame update
    void Start()
    {
        switch (gimmick)
        {
            case Gimmick.BIGSTONE:
                stoneMove = this.GetComponent<StoneMove>();
                break;
            case Gimmick.IVYSTONE:
                ivyFall = this.GetComponent<IvyFall>();
                break;
            case Gimmick.IVY:
                ivy = this.GetComponent<Ivy>();
                break;
            case Gimmick.BRAKSTONE:
                stoneBreak = this.GetComponent<StoneBreak>();
                break;
            case Gimmick.DOOR:
                doorMove = this.GetComponent<DoorMove>();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GimmickHit(int hoge = 0, bool fuga = true , int piyo = 0)
    {
        switch (gimmick)
        {
            case Gimmick.BIGSTONE:
                stoneMove.Mover(hoge,fuga);
                break;
            case Gimmick.IVYSTONE:
                ivyFall.Fall();
                break;
            case Gimmick.IVY:
                ivy.IvyCut();
                break;
            case Gimmick.BRAKSTONE:
                stoneBreak.Break(hoge);
                break;
            case Gimmick.DOOR:
                doorMove.DoorRotation(piyo,fuga);
                break;
        }
    }
    public void ShockGimmickHit()
    {
        switch (gimmick)
        {
            case Gimmick.IVYSTONE:
                ivyFall.Fall();
                break;
            case Gimmick.IVY:
                ivy.IvyCut();
                break;
        }
    }
}
