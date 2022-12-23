using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.Essence.Eye;

public class RaycastController : MonoBehaviour
{
    public BoxCollider scrollBackwardsCollider;
    public BoxCollider scrollForwardsCollider;
    public ControlCanvas controlCanvas;

    private void Awake()
    {
        // Enable Eye Tracking:
        if (EyeManager.Instance != null) { EyeManager.Instance.EnableEyeTracking = true; }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction;
        RaycastHit hit;
        Ray ray;

        if (Application.isEditor)
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        else
        {
            //EyeManager.Instance.GetCombindedEyeDirectionNormalized(out direction);
            //ray = new Ray(Camera.main.transform.position, Camera.main.transform.position + direction);
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        }

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.transform.name);
            Debug.Log("hit");

            if (hit.collider.Equals(scrollBackwardsCollider))
            {
                controlCanvas.GetActiveSuperScrollRect().
                    GetComponent<ScrollRectController>().ScrollBackwards();
            }   
            else if (hit.collider.Equals(scrollForwardsCollider))
            {
                controlCanvas.GetActiveSuperScrollRect().
                    GetComponent<ScrollRectController>().ScrollForwards();
            }
        }
    }
}
