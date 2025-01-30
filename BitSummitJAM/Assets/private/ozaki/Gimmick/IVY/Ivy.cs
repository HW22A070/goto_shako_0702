using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ivy : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ƒcƒ^‚ÌŠâ")]
    private GameObject IvyStone;

    private IvyFall ivyFall;

    // Start is called before the first frame update
    void Start()
    {
        ivyFall = IvyStone.GetComponent<IvyFall>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IvyCut()
    {
        ivyFall.Fall();

        Destroy(gameObject);
    }
}
