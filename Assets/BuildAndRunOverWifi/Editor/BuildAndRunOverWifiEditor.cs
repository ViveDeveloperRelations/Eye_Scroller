using BuildAndRunOverWifi;
using System;
using System.IO;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildAndRunOverWifiEditor : Editor
{
    private static BuildPlayerOptions buildPlayerOptions;

    [MenuItem("File/Build And Run Over Wifi/Show Panel")]
    public static void Init()
    {
        Debug.Log("Build And Run Over Wifi\n------------------\n------------------");

        BuildAndRunOverWifiEditorWindow window = (BuildAndRunOverWifiEditorWindow)EditorWindow.
            GetWindow<BuildAndRunOverWifiEditorWindow>("Build And Run Over Wifi");
        window.Show();
    }

    public static void Build()
    {
        Debug.Log("Being build process");

        // Create build player options
        buildPlayerOptions = new BuildPlayerOptions();

        // Configure build player options
        var productName = Application.identifier.Substring(Application.identifier.LastIndexOf(".") + 1);
        buildPlayerOptions.locationPathName = $"Builds/{productName}.apk";
        buildPlayerOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        if (buildPlayerOptions.scenes == null ||
            buildPlayerOptions.scenes.Length == 0 ||
            buildPlayerOptions.locationPathName == null)
        {
            Debug.LogError("Build failed, check build player options");
            return;
        }

        Debug.Log("Starting android build!");
        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if (buildReport.summary.result.Equals(BuildResult.Succeeded))
            Debug.Log("Build was completed successfully");
        else
            Debug.LogError("Build failed");
    }

    public static void Deploy()
    {
        Debug.Log($"Starting deployment of {Application.identifier}");
        var absolutePath = Path.GetFullPath(buildPlayerOptions.locationPathName);

        if (!File.Exists(absolutePath))
            throw new Exception("Apk not found");

        AdbReflectionSetup adbReflection = new AdbReflectionSetup();
        var adbFacade = adbReflection.AdbFacade;

        var commands = new[]
        {
            $"connect {BuildAndRunOverWifiEditorWindow.buildState.deviceWifiIPAddress}:5555",
            $"install -r -g -d \"{absolutePath}\"",
            $"shell monkey -p {Application.identifier} -c android.intent.category.LAUNCHER 1"
        };

        foreach (var startupCommand in commands)
        {
            AdbRunCommand(adbFacade, startupCommand, "Failed to run command: " + startupCommand, true);
        }

        Debug.Log("Deployment complete");
        Debug.Log("Starting up apk");
    }

    public static string GetDeviceIPAddress()
    {
        AdbReflectionSetup adbReflection = new AdbReflectionSetup();
        var adbFacade = adbReflection.AdbFacade;
        var shellCommand = @"""ip addr show wlan0  | grep 'inet ' | cut -d ' ' -f 6 | cut -d / -f 1""";
        var deviceIP = adbFacade.Run(new[] { "shell", shellCommand }, "error running get ip, try connecting usb");
        return deviceIP;
    }

    public static bool PingHost(string nameOrAddress)
    {
        bool pingable = false;
        try
        {
            System.Net.NetworkInformation.Ping pinger = null;

            try
            {
                pinger = new System.Net.NetworkInformation.Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

        }
        catch
        {
        } //usually an invalid address will cause an exception in dispose

        return pingable;
    }

    private static string AdbRunCommand(AdbFacade adbFacade, string command, string errorMessage, bool throwOnFail)
    {
        try
        {
            return adbFacade.Run(new[] { command }, errorMessage);
        }
        catch
        {
            Debug.Log($"{command} failed");
            if (throwOnFail) throw;
            return string.Empty;
        }
    }

    public static bool IsHeadsetReachable(string wifiAddress)
    {
        bool canReach = false;
        try
        {
            canReach = BuildAndRunOverWifiEditor.PingHost(wifiAddress);
        }
        catch { Debug.Log("PingHost exception"); }
        return canReach;
    }
}

