using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadCube : MonoBehaviour {

    

    float targetCubeRespawnTime = 10.0f;
    private int lastLocation;

    


    void Update()
    {
        if (targetCubeRespawnTime >= 0.0f)
        {
            
            targetCubeRespawnTime = targetCubeRespawnTime - Time.deltaTime;
            //Debug.Log("targetTime = " + targetTime);
        }

        if (targetCubeRespawnTime <= 0.0f)
        {
            moveUploadCube();
            
        }
    }

    void moveUploadCube()
    {
        int SpawnNumber = RandomLocationNumber();
        GameObject go = GameObject.Find("UploadCubeSpawn" + SpawnNumber);
        lastLocation = SpawnNumber;
        this.transform.position = go.transform.position;

        
        Debug.Log("UploadeCubeMoving");
        targetCubeRespawnTime = 10.0f;

    }

    public int RandomLocationNumber()
    {
        int randomLoc = (Random.Range(1, 4));
        if (randomLoc == lastLocation)
            RandomLocationNumber();

        return randomLoc;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "Player")
        {
            Debug.Log("Entered");
        }
        
    }

}
