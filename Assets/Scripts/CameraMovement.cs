using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera gameCamera;
    public float cameraMovementSpeed = 7;
    public float cameraVerticalSpeedScale = 16f/9f;

    private void Start()
    {
        gameCamera = GetComponent<Camera>();
    }

    public void MoveCamera(Vector3 inputVector)
    {
        var movementVector = Quaternion.Euler(0,30,0) * inputVector;
        gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
    }

    public void DragCamera(Vector2 mouseDelta)
    {
        mouseDelta.y *= cameraVerticalSpeedScale;
        Vector3 dragDirection = (gameCamera.transform.right * -mouseDelta.x) + (gameCamera.transform.forward * -mouseDelta.y);
        dragDirection.y = 0;
        gameCamera.transform.position += dragDirection * Time.deltaTime * cameraMovementSpeed;

    }
}
