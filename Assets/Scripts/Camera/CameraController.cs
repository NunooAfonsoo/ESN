using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;


public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;


    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;
    [SerializeField] private float rotationAmount;

    private Vector3 newPosition;
    private Quaternion newRotation;
    [SerializeField] private Vector3 zoomAmount;
    [SerializeField] private Vector3 newZoom;


    private bool inRotationLimitTop = false;
    private bool inRotationLimitBot = false;

    [SerializeField] private float maxWorldSizeX;
    [SerializeField] private float maxWorldSizeZ;
    [SerializeField] private float minWorldSizeX;
    [SerializeField] private float minWorldSizeZ;
    [SerializeField] private float maxZoomIn;
    [SerializeField] private float maxZoomOut;


    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
    }


    void LateUpdate()
    {
        if(UIManager.Instance.ActiveMenu == null && !GameManager.Instance.PauseMenuActive)
        {
            HandleMovementInput();
            HandleMouseInput();
            HandleBoundaries();
        }
    }

    private void HandleMovementInput()
    {
        float height = transform.position.y;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += transform.forward * movementSpeed * Time.unscaledDeltaTime * newZoom.y / 50f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition -= transform.forward * movementSpeed * Time.unscaledDeltaTime * newZoom.y / 50f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition -= transform.right * movementSpeed * Time.unscaledDeltaTime * newZoom.y / 50f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += transform.right * movementSpeed * Time.unscaledDeltaTime * newZoom.y / 50f;
        }

        newPosition.y = height;

        transform.position = Vector3.Lerp(transform.position, newPosition, movementTime);
        transform.rotation = Quaternion.Lerp(this.transform.rotation, newRotation, movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, movementTime);

    }


    private Vector3 localRotation;


    private void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y != 0f)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }



        float yValueBefore = localRotation.y;

        if (Input.GetMouseButton(2))
        {

            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                localRotation.x += Input.GetAxis("Mouse X") * rotationAmount;
                localRotation.y -= Input.GetAxis("Mouse Y") * rotationAmount;

                if (inRotationLimitTop)
                {
                    if (yValueBefore - localRotation.y < 0)
                    {
                        localRotation.y = yValueBefore;
                    }
                }

                else if (inRotationLimitBot)
                {
                    if (yValueBefore - localRotation.y > 0)
                    {
                        localRotation.y = yValueBefore;
                    }
                }
            }


            Quaternion possibleNewRotation = Quaternion.Euler(localRotation.y, localRotation.x, 0); //used when not in limits and to get out of limit

            if (!inRotationLimitTop && !inRotationLimitBot)
                newRotation = possibleNewRotation;

            if (inRotationLimitTop)
            {
                if (yValueBefore - localRotation.y > 0)
                {
                    newRotation = possibleNewRotation;
                    inRotationLimitTop = false;
                }
                else
                {
                    newRotation = Quaternion.Euler(0, localRotation.x, 0);
                    newRotation.x = possibleNewRotation.x;
                    newRotation.z = possibleNewRotation.z;
                    newRotation.w = possibleNewRotation.w;
                }
            }
            else if (inRotationLimitBot)
            {
                if (yValueBefore - localRotation.y < 0)
                {
                    newRotation = possibleNewRotation;
                    inRotationLimitBot = false;
                }
                else
                {
                    newRotation = Quaternion.Euler(0, localRotation.x, 0);
                    newRotation.x = possibleNewRotation.x;
                    newRotation.z = possibleNewRotation.z;
                    newRotation.w = possibleNewRotation.w;
                }
            }
        }
    }


    private void HandleBoundaries()
    {
        //Movement Limits
        if (newPosition.x > maxWorldSizeX) { newPosition.x = maxWorldSizeX; }
        if (newPosition.z > maxWorldSizeZ) { newPosition.z = maxWorldSizeZ; }
        if (newPosition.x < minWorldSizeX) { newPosition.x = minWorldSizeX; }
        if (newPosition.z < minWorldSizeZ) { newPosition.z = minWorldSizeZ; }



        //Zoom Limits
        if (newZoom.y < maxZoomIn)
        {
            newZoom.y = maxZoomIn;
            newZoom.z = -maxZoomIn;
        }

        if (newZoom.y > maxZoomOut)
        {
            newZoom.y = maxZoomOut;
            newZoom.z = -maxZoomOut;
        }



        //Rotation Limits

        if (transform.eulerAngles.x < 317f && transform.eulerAngles.x > 80)
        {
            transform.eulerAngles = new Vector3(317f, transform.eulerAngles.y, 0);
            inRotationLimitBot = true;
        }
        else if (transform.eulerAngles.x > 45f && transform.eulerAngles.x < 60f)
        {
            transform.eulerAngles = new Vector3(45f, transform.eulerAngles.y, 0);
            inRotationLimitTop = true;
        }
    }
}
