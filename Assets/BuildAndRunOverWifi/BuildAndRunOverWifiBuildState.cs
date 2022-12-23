using UnityEngine;

namespace BuildAndRunOverWifi
{
    [CreateAssetMenu(fileName = "", menuName = "Build And Run Over Wifi/Create BuildAndRunOverWifiBuildState")]
    public class BuildAndRunOverWifiBuildState : ScriptableObject
    {
        public string deviceWifiIPAddress;
    }
}
    