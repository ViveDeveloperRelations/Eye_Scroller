using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private ControlCanvas controlCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Movie Browser"))
            controlCanvas.GetMovieBrowserButton().onClick.Invoke();

        if (GUI.Button(new Rect(10, 70, 100, 30), "Teleprompter"))
            controlCanvas.GetTeleprompterButton().onClick.Invoke();

        if (GUI.Button(new Rect(10, 130, 100, 30), "Quit"))
            controlCanvas.GetQuitButton().onClick.Invoke();
    }
}
