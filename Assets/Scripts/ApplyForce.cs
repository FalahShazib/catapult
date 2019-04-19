using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForce : MonoBehaviour
{
    public Transform TargetObject;
    public GameObject CurrentStar;
    public GameObject localCam;
    [Range(20.0f, 75.0f)] public float LaunchAngle;

    // state
    private bool bTargetReady;

    // cache
    private Rigidbody rigid;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion initCameraRot;

    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        bTargetReady = true;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        Debug.Log("detected");
    }

    // launches the object towards the TargetObject with a given LaunchAngle
    void Launch()
    {
        if (TargetObject != null)
        {
            //Debug.Log("stat1: " + transform.rotation);
            // think of it as top-down view of vectors: 
            //   we don't care about the y-component(height) of the initial and target position.
            Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
            Vector3 targetXZPos = new Vector3(TargetObject.position.x, 0.0f, TargetObject.position.z);

            // rotate the object to face the target
            initCameraRot = transform.rotation;
            transform.LookAt(targetXZPos);
            //LaunchAngle = transform.eulerAngles.x;
            //Debug.Log("stat2: " + transform.rotation);
            //Debug.Log(targetXZPos);

            // shorthands for the formula
            float R = Vector3.Distance(projectileXZPos, targetXZPos);
            float G = Physics.gravity.y;
            float tanAlpha = Mathf.Tan(LaunchAngle * Mathf.Deg2Rad);
            float H = TargetObject.position.y - transform.position.y;

            // calculate the local space components of the velocity 
            // required to land the projectile on the target object 
            float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
            float Vy = tanAlpha * Vz;

            //Debug.Log("stat3: " + transform.rotation);
            // create the velocity vector in local space and get it in global space
            Vector3 localVelocity = new Vector3(0f, Vy, Vz);
            Vector3 globalVelocity = transform.TransformDirection(localVelocity);
            //Debug.Log("stat4: " + transform.rotation);

            // launch the object by setting its initial velocity and flipping its state
            transform.rotation = initCameraRot;
            rigid.velocity = globalVelocity;
            bTargetReady = false;
        }
    }


    // resets the projectile to its initial position
    void ResetToInitialState()
    {
        rigid.velocity = Vector3.zero;
        //this.transform.SetPositionAndRotation(TargetObject.position, initialRotation);
        bTargetReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((OVRInput.Get(OVRInput.RawButton.B)) || (Input.GetKeyDown(KeyCode.Space)))
        {
            if (bTargetReady)
            {
                //Debug.Log("before: " + transform.rotation);
                //transform.rotation = localCam.transform.rotation;
                //Debug.Log("after: " + transform.rotation);
                if (CurrentStar != null)
                {
                    CurrentStar.SetActive(false);
                }
                Debug.Log("about to launch");
                Launch();
            }
            else
            {
                ResetToInitialState();
                bTargetReady = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToInitialState();
        }

        Quaternion rot = Quaternion.Euler(0, transform.rotation.y, 0);
        transform.rotation = rot;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "target")
        {
            Debug.Log(other.transform.position);
            this.transform.position = other.transform.position;
        }
    }
}