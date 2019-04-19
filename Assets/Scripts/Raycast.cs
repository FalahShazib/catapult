using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour {

    public Camera playerCamera;
    public ApplyForce manager;
    public GameObject[] targets;
    public GameObject[] islandStars;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        int layer = 9;
        int layerMask = 1 << layer;

        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 20;
        Debug.DrawRay(transform.position, forward, Color.green, 1, true);
        for (int i = 0; i < 6; i++)
        {
            islandStars[i].SetActive(false);
        }

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 10000000, layerMask))
        {
            if (hit.collider.gameObject.name == "island1")
            {
                manager.TargetObject = targets[0].transform;
                islandStars[0].SetActive(true);
                manager.CurrentStar = islandStars[0];
                Debug.Log("island1");
            }
            else if (hit.collider.gameObject.name == "island2")
            {
                manager.TargetObject = targets[1].transform;
                islandStars[1].SetActive(true);
                manager.CurrentStar = islandStars[1];
                Debug.Log("island2");
            }
            else if (hit.collider.gameObject.name == "island3")
            {
                manager.TargetObject = targets[2].transform;
                islandStars[2].SetActive(true);
                manager.CurrentStar = islandStars[2];
                Debug.Log("island3");
            }
            else if (hit.collider.gameObject.name == "island4")
            {
                manager.TargetObject = targets[3].transform;
                islandStars[3].SetActive(true);
                manager.CurrentStar = islandStars[3];
                Debug.Log("island4");
            }
            else if (hit.collider.gameObject.name == "island5")
            {
                manager.TargetObject = targets[4].transform;
                islandStars[4].SetActive(true);
                manager.CurrentStar = islandStars[4];
                Debug.Log("island5");
            }
            else if (hit.collider.gameObject.name == "island6")
            {
                manager.TargetObject = targets[5].transform;
                islandStars[5].SetActive(true);
                manager.CurrentStar = islandStars[5];
                Debug.Log("island6");
            }
            else
            {
                
                manager.TargetObject = null;
                manager.CurrentStar = null;
            }
        }
    }
}
