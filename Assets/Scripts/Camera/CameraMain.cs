using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraMain : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float shift = 1.0f;

    [SerializeField] Vector3 plusLimit = new Vector3(100.0f, 100.0f, 100.0f);
    [SerializeField] Vector3 minusLimit = new Vector3(100.0f, 100.0f, 100.0f);

    // [SerializeField] Contoller controller;
    [SerializeField] Camera cam;

    [SerializeField] Transform pivot;
    [SerializeField] float distanceToTarget = 10;
    Vector3 previousPosition;

    float defaultFov = 70.0f;

    float fov;
    [SerializeField] float fovSpeed = 500.0f;

    private void Start()
    {
        fov = cam.fieldOfView;
        defaultFov = fov;
    }

    void Update()
    {
        // if(controller.isPlayersNamed)
        // {
            // WASD - camera moovement
            WASDCameraMoovement();

            // FOV (zoom)
            FOVCamera();

            // Camera rotation
            RotateCamera();

            //Default fov
            if (Input.GetKeyDown(KeyCode.R))
            {
                cam.fieldOfView = fov;
                fov = defaultFov;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += Vector3.up * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.position += Vector3.down * speed * Time.deltaTime;
            }

        // }
       
    }


    Vector3 GetInput(Vector3 vec)
    {
        if (Input.GetKey(KeyCode.W))
        {
            vec.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vec.z += -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vec.x += -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            vec.x += 1;
        }

        return vec;
    }

    // Checking for across bounding box border
    Vector3 CheckLimits(Transform pos, Vector3 moveDir)
    {
        Vector3 newPos = pos.position + moveDir;

        if (newPos.x > plusLimit.x)
        {
            newPos.x = plusLimit.x;
        }
        if (newPos.x < -minusLimit.x)
        {
            newPos.x = -minusLimit.x;
        }

        if (newPos.y > plusLimit.y)
        {
            newPos.y = plusLimit.y;
        }
        if (newPos.y < -minusLimit.y)
        {
            newPos.y = -minusLimit.y;
        }

        if (newPos.z > plusLimit.z)
        {
            newPos.z = plusLimit.z;
        }
        if (newPos.z < -minusLimit.z)
        {
            newPos.z = -minusLimit.z;
        }

        return newPos;
    }

    void RotateCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(1))
        {
            distanceToTarget = Vector3.Distance(pivot.position, cam.transform.position);

            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically

            cam.transform.position = pivot.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World);

            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;
        }
    }

    void FOVCamera()
    {
        float wheelMove = Input.GetAxis("Mouse ScrollWheel");
        fov -= wheelMove * fovSpeed * Time.deltaTime;

        if (fov > 90)
            fov = 90;
        if (fov < 30)
            fov = 30;

        cam.fieldOfView = fov;
    }

    void WASDCameraMoovement()
    {
        Vector3 inputDir = new Vector3(0.0f, 0.0f, 0.0f);
        inputDir = GetInput(inputDir);

        Transform pos = transform;

        // disable y-axis
        Vector3 forward = transform.forward;
        forward.y = 0;
        Vector3 right = transform.right;
        right.y = 0;
        Vector3 moveDir = (forward * inputDir.z) + (right * inputDir.x);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDir *= shift * speed * Time.deltaTime;
        }
        else
        {
            moveDir *= speed * Time.deltaTime;
        }

        transform.position = CheckLimits(pos, moveDir);
    }

}
