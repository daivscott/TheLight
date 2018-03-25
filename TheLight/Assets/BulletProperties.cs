using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProperties : MonoBehaviour {


    void OnCollisionEnter()
    {
        Destroy(gameObject);
    }


}
