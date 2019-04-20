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
    private bool isFLying;
    private char planeNumber;

    // cache
    private Rigidbody rigid;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion initCameraRot;
    private bool triggerPressed = false;
    private bool triggerReleased = false;
    private float timeDelay;

    private int starCaught;
    
    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        bTargetReady = true;
        isFLying = false;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        Debug.Log("detected");
    }

    // launches the object towards the TargetObject with a given LaunchAngle
    void Launch()
    {
        if (TargetObject != null && !isFLying)
        {
            isFLying = true;
            //Debug.Log("is flying");
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
        this.transform.SetPositionAndRotation(initialPosition, initialRotation);
        bTargetReady = false;
        isFLying = false;
        CurrentStar = null;
        //Debug.Log("reset");
    }

    // Update is called once per frame
    void Update()
    {
        if ((OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0))
        {
            triggerPressed = true;
            //Debug.Log("trigger pressed: " + triggerPressed);
        } else if (triggerPressed)
        {
            triggerReleased = true;
            //Debug.Log("tirgger released: " + triggerReleased);
        }


        if (triggerPressed)
        {
            IncrementAngle();
            if (triggerReleased)
            {
                //if (bTargetReady)
                //{
                    if (CurrentStar != null)
                    {
                        CurrentStar.SetActive(false);
                    }
                    //Debug.Log("about to launch");
                    //Debug.Log(isFLying);
                    //Debug.Log(LaunchAngle);
                    Launch();
                //}
                //else
                //{
                //    bTargetReady = true;
                //}
                triggerReleased = false;
                triggerPressed = false;
            }
        }

        if (CurrentStar != null && !isFLying)
        {
            CurrentStar.SetActive(true);
            string starName = CurrentStar.gameObject.name;
            int starNumber = starName.Length - 1;
            if (starName[starNumber] == planeNumber)
            {
                CurrentStar.SetActive(false);
                //CurrentStar = null;
            }
        } else if (CurrentStar != null)
        {
            CurrentStar.SetActive(false);
        }

        //if ((OVRInput.Get(OVRInput.RawButton.B)) || (Input.GetKeyDown(KeyCode.Space)))
        //{
        //    if (bTargetReady)
        //    {
        //        //Debug.Log("before: " + transform.rotation);
        //        //transform.rotation = localCam.transform.rotation;
        //        //Debug.Log("after: " + transform.rotation);
        //        if (CurrentStar != null)
        //        {
        //            CurrentStar.SetActive(false);
        //        }
        //        //Debug.Log("about to launch");
        //        Launch();
        //    }
        //    else
        //    {
        //        //ResetToInitialState();
        //        bTargetReady = true;
        //    }
        //}

        if ((OVRInput.Get(OVRInput.RawButton.X)))
        {
            ResetToInitialState();
            Debug.Log("stars: " + starCaught);
        }

        Quaternion rot = Quaternion.Euler(0, transform.rotation.y, 0);
        transform.rotation = rot;
    }

    private void OnCollisionEnter(Collision other)
    {
       // Debug.Log("landed on " + other.gameObject.name);
        if (other.gameObject.name.Contains("plane"))
        {
            isFLying = false;
            //Debug.Log("not flying");

            planeNumber = other.gameObject.name[other.gameObject.name.Length - 1];
            if (CurrentStar != null)
            {
                string planeName = other.gameObject.name;
                string starName = CurrentStar.gameObject.name;
                int planeNumber = planeName.Length - 1;
                int starNumber = starName.Length - 1;
                if (starName[starNumber] == planeName[planeNumber])
                {
                    //Debug.Log("deactivate star collision");
                    CurrentStar.SetActive(false);
                }
            }
        } else
        {
            planeNumber = 'n';
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("star"))
        {
            Destroy(other.gameObject);
            Debug.Log("caught a star");
            starCaught++;
        }
    }
    private void IncrementAngle()
    {
        if (Time.time > timeDelay)
        {
            float inc;
            timeDelay = Time.time + 0.75f;
            LaunchAngle = LaunchAngle + 5f;
            inc = LaunchAngle % 75;
            if (inc < 35f)
            {
                LaunchAngle = 35;
            }
        }
    }
}