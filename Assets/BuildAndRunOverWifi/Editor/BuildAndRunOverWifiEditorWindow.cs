using BuildAndRunOverWifi;
using System;
using UnityEditor;
using UnityEngine;

public class BuildAndRunOverWifiEditorWindow : EditorWindow
{
    public static BuildAndRunOverWifiBuildState buildState;
    private Editor buildStateEditor;

    private void OnEnable()
    {
        buildState = Resources.Load<BuildAndRunOverWifiBuildState>("BuildAndRunOverWifiBuildState");
        buildStateEditor = Editor.CreateEditor(buildState);
    }

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Application is Playing\n" + "Before any build operation, please stop playing.", MessageType.None);
            return;
        }

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorGUILayout.HelpBox("Please switch to Android build target", MessageType.None);

            if (GUILayout.Button("Switch to Android build target"))
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            return;
        }

        ShowPanel();
        this.Repaint();
    }

    private void ShowPanel()
    {
        GUI.enabled = false;
        buildStateEditor.OnInspectorGUI();
        GUI.enabled = true;

        if (GUILayout.Button("Get IP from headset (if connected through unity)"))
        {
            try
            {
                buildState.deviceWifiIPAddress = BuildAndRunOverWifiEditor.GetDeviceIPAddress().Trim();
                EditorUtility.SetDirty(buildState);
                AssetDatabase.SaveAssets();

                if (!string.IsNullOrEmpty(buildState.deviceWifiIPAddress))
                    Debug.Log($"Device IP Address Found: {buildState.deviceWifiIPAddress}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowNotification(new GUIContent("Failed to get IP from headset"));
            }
        }

        if (GUILayout.Button("Test reachability of headset"))
        {
            bool canReach = BuildAndRunOverWifiEditor.IsHeadsetReachable(buildState.deviceWifiIPAddress);
            ShowNotification(new GUIContent(canReach ? "Reachable" : "Not reachable"));
        }

        if (GUILayout.Button("Build And Run Over Wifi"))
        {
            BuildAndRunOverWifiEditor.Build();
            BuildAndRunOverWifiEditor.Deploy();
        }
    }
}

