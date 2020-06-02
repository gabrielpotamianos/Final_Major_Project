using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    #region CameraCollision

    public float maxDistance = Constants.CAMERA_COLLISION_MAX_DISTANCE;
    public float minDistance = Constants.CAMERA_COLLISION_MIN_DISTANCE;
    public float smooth = Constants.CAMERA_COLLISION_SMOOTH;
    public LayerMask LayersToHit;
    float distance;
    Vector3 direction;

    #endregion


    void Awake()
    {
        direction = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }


    void Update()
    {

        #region Camera Collision

        Vector3 desiredCameraPos = transform.parent.TransformPoint(direction * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit, LayersToHit))
            distance = Mathf.Clamp(hit.distance * Constants.CAMERA_COLLISION_OFFSET, minDistance, maxDistance);
        else distance = maxDistance;

        transform.localPosition = Vector3.Lerp(transform.localPosition, direction * distance, Time.deltaTime * smooth);

        #endregion

    }
}
