using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayDetector : MonoBehaviour
{
    public static RayDetector Instance;
    
    public Transform planeDetector;
    public Transform gamePlane;
    
    private bool isDetecting = false;
    public Vector3 planePoint;
    
    private Transform cameraTransform;
    private RaycastHit planeHit;
    

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        Instance = this;
        cameraTransform = Camera.main.transform;
    }

    private bool IsPlane(out Vector3 planePoint)
    {
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward, Color.yellow);
        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out planeHit))
        {
            if (planeHit.transform.CompareTag("Plane"))
            {
                planePoint = planeHit.point;
                return true;
            }
        }

        planePoint = Vector3.zero;
        return false;
    }

    private void Update()
    {
        if (isDetecting && IsPlane(out Vector3 hitPoint))
        {
            planeDetector.position = hitPoint;
        }
    }

    public void StartDetect()
    {
        isDetecting = true;
    }
    
    public void StopDetect()
    {
        gamePlane.position = planeHit.point + new Vector3(0, 0.6f, 0);
        gamePlane.GetComponent<Collider>().enabled = true;
        isDetecting = false;
        planeDetector.gameObject.SetActive(false);
    }
}