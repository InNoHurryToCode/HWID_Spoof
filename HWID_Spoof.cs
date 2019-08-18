using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using VRCModLoader;
using VRCTools;
using Harmony;

namespace HWID_Spoof
{
    public static class ModInfo
    {
        public const string Name = "HWID_Spoof";
        public const string Author = "Herp Derpinstine, Slaynash";
        public const string Company = "NanoNuke @ nanonuke.net";
        public const string Version = "1.0.0";
    }
    [VRCModInfo(ModInfo.Name, ModInfo.Version, ModInfo.Author)]

    public class HWID_Spoof : VRCMod
    {
        private static string HardwareID;

        void OnApplicationStart()
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("hwidspoof");
            harmonyInstance.Patch(typeof(VRC.Core.API).GetProperty("DeviceID").GetGetMethod(), new HarmonyMethod(typeof(HWID_Spoof).GetMethod("API_DeviceID", BindingFlags.Static | BindingFlags.NonPublic)));
            harmonyInstance.Patch(typeof(Transmtn.Api).GetConstructors().First(x => (x.GetParameters().Length == 10)), new HarmonyMethod(typeof(HWID_Spoof).GetMethod("Transmtn_Api", BindingFlags.Static | BindingFlags.NonPublic)));
            harmonyInstance.Patch(typeof(Transmtn.Api).Assembly.GetTypes().First(x => (x.Name == "WebsocketPipeline")).GetConstructors().First(x => (x.GetParameters().Length == 6)), new HarmonyMethod(typeof(HWID_Spoof).GetMethod("Transmtn_WebsocketPipeline1", BindingFlags.Static | BindingFlags.NonPublic)));
            harmonyInstance.Patch(typeof(Transmtn.Api).Assembly.GetTypes().First(x => (x.Name == "WebsocketPipeline")).GetConstructors().First(x => (x.GetParameters().Length == 7)), new HarmonyMethod(typeof(HWID_Spoof).GetMethod("Transmtn_WebsocketPipeline2", BindingFlags.Static | BindingFlags.NonPublic)));
            harmonyInstance.Patch(typeof(Transmtn.Api).Assembly.GetTypes().First(x => (x.Name == "WebsocketPipeline")).GetConstructors().First(x => (x.GetParameters().Length == 8)), new HarmonyMethod(typeof(HWID_Spoof).GetMethod("Transmtn_WebsocketPipeline3", BindingFlags.Static | BindingFlags.NonPublic)));
        }

        private static string GetHardwareID()
        {
            if (HardwareID == null)
            {
                Guid guid = Guid.NewGuid();
                HardwareID = CalculateHash<SHA1>(guid.ToString());
            }
            return HardwareID;
        }

        private static bool API_DeviceID(ref string __result)
        {
            __result = GetHardwareID();
            return false;
        }

        private static bool Transmtn_Api(Uri httpEndpoint, Uri websocketEndpoint, Transmtn.ApiAuth auth, ref string macAddress, string clientVersion, string platform, Transmtn.Api.ErrorResponse defaultErrorResponse, Transmtn.Api.LogResponse defaultLogResponse, Transmtn.Api.OnReady onReadyResponse, Transmtn.Api.OnConnectionLost onLostConnectionResponse)
        {
            macAddress = GetHardwareID();
            return true;
        }

        private static bool Transmtn_WebsocketPipeline1(Guid connectionId, Uri endpoint, string authToken, ref string macAddress, string clientVersion, string platform)
        {
            macAddress = GetHardwareID();
            return true;
        }
        private static bool Transmtn_WebsocketPipeline2(Guid connectionId, Uri endpoint, string authToken, ref string macAddress, string clientVersion, string platform, Transmtn.IStream<string> inputStream)
        {
            macAddress = GetHardwareID();
            return true;
        }

        private static bool Transmtn_WebsocketPipeline3(Guid connectionId, Uri endpoint, string authToken, ref string macAddress, string clientVersion, string platform, Transmtn.IStream<string> inputStream, Transmtn.IStream<string> outputStream)
        {
            macAddress = GetHardwareID();
            return true;
        }

        private static string CalculateHash<T>(string input)
        where T : HashAlgorithm
        {
            byte[] numArray = HWID_Spoof.CalculateHash<T>(Encoding.UTF8.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < (int)numArray.Length; i++)
                stringBuilder.Append(numArray[i].ToString("x2"));
            return stringBuilder.ToString();
        }

        private static byte[] CalculateHash<T>(byte[] buffer)
        where T : HashAlgorithm
        {
            byte[] numArray;
            T t = (T)(typeof(T).GetMethod("Create", new Type[0]).Invoke(null, null) as T);
            try
            {
                numArray = t.ComputeHash(buffer);
            }
            finally
            {
                if (t != null)
                    ((IDisposable)(object)t).Dispose();
            }
            return numArray;
        }
    }
}
