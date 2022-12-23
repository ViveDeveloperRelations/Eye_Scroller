﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Debug = UnityEngine.Debug;

namespace BuildAndRunOverWifi
{
    public class AdbFacade
    {
        private Type m_ADBType;
        private ReflectionInstanceHelper m_AdbInstance;
        private Type m_AdbStatusType;
        private Type m_AdbProcType;

        public AdbFacade(Type adbType)
        {
            m_ADBType = adbType ?? throw new ArgumentNullException(nameof(adbType));
            Object newInstanceObject = ReflectionHelpers.InvokePublicStaticMethod(m_ADBType, "GetInstance", null) ??
                                        throw new Exception("Failed to get ADB instance");
            m_AdbInstance = new ReflectionInstanceHelper(m_ADBType, newInstanceObject) ??
                            throw new Exception("Failed to get ADB instance");
            m_AdbStatusType = m_ADBType.Assembly.GetType("UnityEditor.Android.ADB+ADBStatus") ??
                                throw new Exception("Failed to get ADBStatus type");
            m_AdbProcType = m_ADBType.Assembly.GetType("UnityEditor.Android.ADB+ADBProc") ?? //does this need an inner class lookup instead?
                            throw new Exception("Failed to get ADBProcess type");
        }

        public bool IsAdbAvailable()
        {
            return Convert.ToBoolean(m_AdbInstance.InvokePrivateMethod("IsADBAvailable"));
        }

        public string GetAdbPath()
        {
            return m_AdbInstance.InvokePublicMethod("GetADBPath") as string;
        }

        public string Run(string[] command, string errorMessage)
        {
            return m_AdbInstance.InvokePublicMethod("Run", command, errorMessage) as string;
        }

        public AdbStatusFacade GetADBProcessStatus()
        {
            return new AdbStatusFacade(m_AdbStatusType, m_AdbInstance.InvokePrivateMethod("GetADBProcessStatus"));
        }
        static Regex DeviceInfoRegex = new Regex(@"(?<id>^\S+)\s+(?<state>\S+$)");

        public enum DeviceState
        {
            UNKNOWN,
            UNAUTHORIZED,
            DISCONNECTED,
            CONNECTED,
        }
        public struct DeviceInfo
        {
            public string id;
            public DeviceState state;
        }
        private (bool, DeviceInfo) ParseDeviceInfoLine(string input)
        {
            var result = DeviceInfoRegex.Match(input);
            if (result.Success)
            {
                var id = result.Groups["id"].Value;
                var stateValue = result.Groups["state"].Value.ToLowerInvariant();
                var state = DeviceState.UNKNOWN;
                if (stateValue.Equals("device"))
                    state = DeviceState.CONNECTED;
                else if (stateValue.Equals("offline"))
                    state = DeviceState.DISCONNECTED;
                else if (stateValue.Equals("unauthorized"))
                    state = DeviceState.UNAUTHORIZED;

                return (true, new DeviceInfo() { id = id, state = state });
            }
            else
            {
                return (false, default);
            }
        }
        public DeviceInfo[] GetDevices()
        {
            var result = new List<DeviceInfo>();
            try
            {
                var output = Run(new[] { "devices" }, "Failed to get devices");
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var lineRaw in lines)
                {
                    var line = lineRaw.Trim();
                    var (success, deviceInfo) = ParseDeviceInfoLine(line);
                    if (success)
                        result.Add(deviceInfo);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error while parsing device strings");
                UnityEngine.Debug.LogException(e);
            }

            return result.ToArray();
        }

        public bool OneDeviceConnected()
        {
            var devices = GetDevices();
            int connectedDevices = 0;
            foreach (var device in devices)
            {
                if (device.state == DeviceState.CONNECTED)
                    connectedDevices++;
            }
            return connectedDevices == 1;
        }
    }
    public class ADBProcFacade
    {
        private readonly ReflectionInstanceHelper m_AdbProcFacade;

        public ADBProcFacade(Type adbProcType, Object adbProcObject)
        {
            if (adbProcType == null) throw new ArgumentNullException(nameof(adbProcType));
            if (adbProcObject == null) throw new ArgumentNullException(nameof(adbProcObject));
            m_AdbProcFacade = new ReflectionInstanceHelper(adbProcType, adbProcObject);
        }
        public Process Process()
        {
            return m_AdbProcFacade.GetPublicField("procHandle") as Process;
        }
        public string FullPath()
        {
            return m_AdbProcFacade.GetPublicField("fullPath") as string;
        }
        public bool External()
        {
            return Convert.ToBoolean(m_AdbProcFacade.GetPublicField("external"));
        }
    }

    public enum ADBProcessStatus
    {
        Offline,
        External,
        MultiInstance,
        Online,
    }

    public class AdbStatusFacade //adb status is a struct have to handle this differently
    {
        private readonly Type m_AdbStatusType;
        private readonly ReflectionInstanceHelper m_AdbStatusInstance;
        private readonly Type m_AdbProcType;
        private readonly Type m_AdbProcessStatusType;

        public AdbStatusFacade(Type adbStatusType, Object adbStatusObject)
        {
            m_AdbStatusType = adbStatusType ?? throw new ArgumentNullException(nameof(adbStatusType));
            m_AdbStatusInstance = new ReflectionInstanceHelper(adbStatusType, adbStatusObject) ??
                                    throw new Exception("Failed to get ADBStatus instance");
            //FIXME: pass this in 
            m_AdbProcType = m_AdbStatusType.Assembly.GetType("UnityEditor.Android.ADB+ADBProc") ?? //does this need an inner class lookup instead?
                            throw new Exception("Failed to get ADBProcess type");
        }

        public ADBProcFacade[] Processes()
        {
            object[] processes = m_AdbStatusInstance.GetPublicField("processes") as object[];
            if (processes == null)
            {
                //check that this happens when it should
                return null;
            }
            var facades = new ADBProcFacade[processes.Length];
            for (int i = 0; i < processes.Length; i++)
            {
                facades[i] = new ADBProcFacade(m_AdbProcType, processes[i]);
            }
            return facades;
        }

        public ADBProcessStatus Status()
        {
            var statusRawReturn = m_AdbStatusInstance.GetPublicPropertyObject("status");
            Enum.TryParse(statusRawReturn.ToString(), out ADBProcessStatus status);
            return status;
        }
    }
}