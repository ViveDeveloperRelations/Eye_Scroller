using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectController : MonoBehaviour
{
    public float speed;
    private ScrollRect scrollRect;


    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScrollBackwards()
    {
        scrollRect.verticalNormalizedPosition += speed * Time.deltaTime;
    }

    public void ScrollForwards()
    {
        scrollRect.verticalNormalizedPosition -= speed * Time.deltaTime;
    }
}
