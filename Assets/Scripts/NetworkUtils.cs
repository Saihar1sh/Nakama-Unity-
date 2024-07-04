using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shGames.Network
{
    public static class NetworkUtils
    {
        private const string deviceIDStatKey = "deviceID";
        private const string sessionNameKey = "session";

        public static string GetDeviceID()
        {
            string deviceID = string.Empty;
            if (PlayerPrefs.HasKey(deviceIDStatKey))
            {
                deviceID = PlayerPrefs.GetString(deviceIDStatKey, SystemInfo.deviceUniqueIdentifier);
            }
            else
            {
                deviceID = SystemInfo.deviceUniqueIdentifier;
                if (deviceID == SystemInfo.unsupportedIdentifier)
                {
                    deviceID = System.Guid.NewGuid().ToString();
                }
                SetDeviceID(deviceID);
            }
            return deviceID;
        }
        public static string GetSessionAuthToken()
        {
            return PlayerPrefs.GetString(sessionNameKey);
        }

        private static void SetDeviceID(string deviceId)
        {
            PlayerPrefs.SetString(deviceIDStatKey, deviceId);
        }
        public static void SetSessionAuthToken(string authToken)
        {
            PlayerPrefs.SetString(sessionNameKey, authToken);
        }

    }
}
