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
            if (hit.collider.gameObject.name.Contains("island"))
            {
                string islandName = hit.collider.gameObject.name;
                int islandNumber = int.Parse(islandName[islandName.Length - 1].ToString());
                manager.TargetObject = targets[islandNumber - 1].transform;
                manager.CurrentStar = islandStars[islandNumber - 1];
                manager.TargetName = "island" + islandNumber.ToString();
                currStar = islandNumber - 1;
            }
            else if (hit.collider.gameObject.name == "balloon")
            {
                manager.TargetObject = null;
                manager.CurrentStar = null;
                manager.TargetName = "balloon";
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
