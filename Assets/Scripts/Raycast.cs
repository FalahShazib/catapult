using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour {

    public Camera playerCamera;
    public ApplyForce manager;
    public GameObject[] targets;
    public GameObject[] islandStars;

    private int currStar;

	
	// Update is called once per frame
	void Update () {

        // only hit items on targets (9th) layer
        int layer = 9;
        int layerMask = 1 << layer;

        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 20000;
        Debug.DrawRay(transform.position, forward, Color.green, 1, true);
        

        // For each island we set its target and star in the manager
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 10000000, layerMask))
        {
            if (hit.collider.gameObject.name == "island1")
            {
                manager.TargetObject = targets[0].transform;
                manager.CurrentStar = islandStars[0];
                manager.TargetName = "island1";
                currStar = 0;
            }
            else if (hit.collider.gameObject.name == "island2")
            {
                manager.TargetObject = targets[1].transform;
                manager.CurrentStar = islandStars[1];
                manager.TargetName = "island2";
                currStar = 1;
            }
            else if (hit.collider.gameObject.name == "island3")
            {
                manager.TargetObject = targets[2].transform;
                manager.CurrentStar = islandStars[2];
                manager.TargetName = "island3";
                currStar = 2;
            }
            else if (hit.collider.gameObject.name == "island4")
            {
                manager.TargetObject = targets[3].transform;
                manager.CurrentStar = islandStars[3];
                manager.TargetName = "island4";
                currStar = 3;
            }
            else if (hit.collider.gameObject.name == "island5")
            {
                manager.TargetObject = targets[4].transform;
                manager.CurrentStar = islandStars[4];
                manager.TargetName = "island5";
                currStar = 4;
            }
            else if (hit.collider.gameObject.name == "island6")
            {
                manager.TargetObject = targets[5].transform;
                manager.CurrentStar = islandStars[5];
                manager.TargetName = "island6";
                currStar = 5;
            }
            else if (hit.collider.gameObject.name == "balloon")
            {
                manager.TargetObject = targets[6].transform;
                manager.CurrentStar = islandStars[6];
                manager.TargetName = "balloon";
                currStar = 6;
            }
            else
            {
                manager.TargetObject = null;
                manager.CurrentStar = null;
                manager.TargetName = null;
            }

            // deactivating all inactive stars
            for (int i = 0; i < 7; i++)
            {
                if (i != currStar)
                {
                    islandStars[i].SetActive(false);
                }
            }
        }
    }
}
