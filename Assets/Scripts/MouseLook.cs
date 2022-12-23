using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private  GameObject rotatingObject;
    [SerializeField] private int mouseButton;
    [SerializeField] private float mouseSensitivity = 100.0f;
    [SerializeField] private float clampAngle = 80.0f;
    [SerializeField] private bool resetToOrigin;
    private float rotY = 0.0f; // rtation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    void Start()
    {
        if (!Application.isEditor)
            return;

        Cursor.lockState = CursorLockMode.Locked;

        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    void Update()
    {
        if (!Application.isEditor)
            return;

        if (Input.GetMouseButton(mouseButton))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            rotY += mouseX * mouseSensitivity * Time.deltaTime;
            rotX += mouseY * mouseSensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
            rotatingObject.transform.rotation = Quaternion.Euler(rotX, rotY, 0.0f);
        }
        else if (resetToOrigin)
        {
            
            if (!rotatingObject.transform.rotation.Equals(rotatingObject.transform.parent.rotation))
                rotatingObject.transform.rotation = rotatingObject.transform.parent.rotation;
        }
    }

    public Quaternion GetRotation()
    {
        return rotatingObject.transform.rotation;
    }
}
