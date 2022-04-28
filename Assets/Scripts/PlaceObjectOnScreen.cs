using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class PlaceObjectOnScreen : MonoBehaviour
{
    public GameObject objectToPlace;

    public GameObject placementIndicator;
    private Pose placementPose;
    private Transform placementTransform;

    private bool placementPositionIsValid = false;
    private TrackableId placedPlaneId = TrackableId.invalidId;

    ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private bool isObjectPlaced = false;

    public static event Action onPlacedObject;

    private void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }



    // Update is called once per frame
    void Update()
    {
        if (!isObjectPlaced)
        {
            UpdatePlacementPosition();
            UpdatePlacementIndicator();

            if (placementPositionIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
            }
        }           
        
        
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementTransform.rotation);
        onPlacedObject?.Invoke();
        isObjectPlaced = false;
        placementIndicator.SetActive(false);
    }

    private void UpdatePlacementPosition()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        if (m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            placementPose = s_Hits[0].pose;
            placedPlaneId = s_Hits[0].trackableId;

            var planeManager = GetComponent<ARPlaneManager>();
            ARPlane arPlane = planeManager.GetPlane(placedPlaneId);
            placementTransform = arPlane.transform;
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPositionIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementTransform.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
}
