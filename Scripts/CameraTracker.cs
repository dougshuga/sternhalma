using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraTracker : MonoBehaviour
{

    [SerializeField] float cameraMoveSpeed = 12;
    [SerializeField] float cameraZoomSpeed = 2;
    [SerializeField] float cameraRotationSpeed = 10;
    CinemachineVirtualCamera virtualCamera;
    private float minCameraSize = 12;
    private float maxCameraSize = 18;
    private float minXPosition = -8;
    private float maxXPosition = 8;
    private float minYPosition = -6.5f;
    private float maxYPosition = 6.5f;
    [SerializeField] InputField chatInput;

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.m_Lens.OrthographicSize = maxCameraSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (chatInput && chatInput.gameObject.activeSelf || chatInput.isFocused)
        {
            return;
        }

        // move camera in different directions
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                Mathf.Clamp(transform.localPosition.y + Time.deltaTime * cameraMoveSpeed, minYPosition, maxYPosition),
                transform.localPosition.z
            );
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                Mathf.Clamp(transform.localPosition.y - Time.deltaTime * cameraMoveSpeed, minYPosition, maxYPosition),
                transform.localPosition.z
            );
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.localPosition = new Vector3(
                Mathf.Clamp(transform.localPosition.x + Time.deltaTime * cameraMoveSpeed, minXPosition, maxXPosition),
                transform.localPosition.y,
                transform.localPosition.z
            );
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.localPosition = new Vector3(
                Mathf.Clamp(transform.localPosition.x - Time.deltaTime * cameraMoveSpeed, minXPosition, maxXPosition),
                transform.localPosition.y,
                transform.localPosition.z
            );
        }

        // zoom camera in, out
        if (Input.GetKey(KeyCode.S))
        {
            if (virtualCamera.m_Lens.OrthographicSize < maxCameraSize)
            {
                virtualCamera.m_Lens.OrthographicSize += Time.deltaTime * cameraZoomSpeed;
            }
        }
        if (Input.mouseScrollDelta.y < 0)  // mousewheel should only scroll chat viewport when that's open.
        {
            if (chatInput && chatInput.gameObject.activeSelf)
            {
                return;
            }
            else if (virtualCamera.m_Lens.OrthographicSize < maxCameraSize)
            {
                virtualCamera.m_Lens.OrthographicSize += Time.deltaTime * cameraZoomSpeed * 30;
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (virtualCamera.m_Lens.OrthographicSize > minCameraSize)
            {
                virtualCamera.m_Lens.OrthographicSize -= Time.deltaTime * cameraZoomSpeed;
            }
        }
        if (Input.mouseScrollDelta.y > 0)  // mousewheel should only scroll chat viewport when that's open.
        {
            if (chatInput && chatInput.gameObject.activeSelf)
            {
                return;
            }
            else if (virtualCamera.m_Lens.OrthographicSize > minCameraSize)
            {
                virtualCamera.m_Lens.OrthographicSize -= Time.deltaTime * cameraZoomSpeed * 30;
            }
        }

        // rotate camera
        if (Input.GetKey(KeyCode.R))
        {
            virtualCamera.transform.parent.Rotate(
                Vector3.forward,
                cameraRotationSpeed * Time.deltaTime
            );
        }
        if (Input.GetKey(KeyCode.E))
        {
            virtualCamera.transform.parent.Rotate(
                Vector3.forward,
                -cameraRotationSpeed * Time.deltaTime
            );
        }
    }
}
