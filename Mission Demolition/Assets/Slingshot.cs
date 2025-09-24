using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private bool button0DownInUpdate = false;

    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }


    private void OnMouseEnter()
    {
        //print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    private void OnMouseExit()
    {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
         // The player has pressed the mouse button while over Slingshot
        aimingMode = true;
         // Instantiate a Projectile
        projectile = Instantiate(projectilePrefab) as GameObject;
         // Start it at the launchPoint
        projectile.transform.position = launchPos;
         // Set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        // If Slingshot is not in aimingMode, don’t run this code
        if (!aimingMode) return;                                                

        // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;                              
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        // Limit mouseDelta to the radius of the Slingshot SphereCollider    
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        // Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if (Input.GetMouseButtonUp(0))
        { // This 0 is a zero, not an o
            // The mouse has been released
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;                                       
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile; // Set the _MainCamera POI
            projectile = null;                                               
        }

        // If the player presses the 0th mouse button down this frame
        if (Input.GetMouseButtonDown(0))
        {
            // …then set buttonDownInUpdate to true
            button0DownInUpdate = true;
            // …it will stay true until FixedUpdate() sets it to false
        }

    }

    private void FixedUpdate()
    {
        // buttonDownInUpdate replaces Input.GetMouseButtonDown( 0 )
        if (button0DownInUpdate)
        { // Input.GetMouseButtonDown( 0 )
            Debug.Log("Mouse button 0 pressed!");
            // Don’t forget to reset buttonDownInUpdate back to false
            button0DownInUpdate = false;
        }
    }

}
