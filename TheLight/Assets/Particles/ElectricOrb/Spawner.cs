using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Transform sphereClone;
    public Transform sphere;

    private int i = 0;
    private int iterate = 1;
    private float displace = 0.5f;

    // Update is called once per frame
    void Update() {


        if (i < 1)
        {
            SpawnObject();
            i++;
        } else
        {
            return;
        }



    }

    private void SpawnObject()
    {
        if (iterate == 1)
        {
            sphere = (Transform)Instantiate(sphereClone, new Vector3(transform.position.x + displace, transform.position.y, transform.position.z), Quaternion.identity);
            iterate++;
        } else

            if (iterate == 2)
            {
                sphere = (Transform)Instantiate(sphereClone, new Vector3(transform.position.x, transform.position.y, transform.position.z + displace), Quaternion.identity);
                iterate++;
            } else

                if (iterate == 3)
                {
                    sphere = (Transform)Instantiate(sphereClone, new Vector3(transform.position.x - displace, transform.position.y, transform.position.z), Quaternion.identity);
                    iterate++;
                } else 

                    if (iterate == 4)
                    {
                        sphere = (Transform)Instantiate(sphereClone, new Vector3(transform.position.x, transform.position.y, transform.position.z - displace), Quaternion.identity);
                        iterate = 1;
                    }
    }
}
