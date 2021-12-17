using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Steamworks;

public class NetworkUI : MonoBehaviour
{
    public TMP_InputField hostSteamID;
    public TextMeshProUGUI ownerSteamID;
    bool showID = false;
    public void StartHost()
    {
        Unity.Netcode.NetworkManager.Singleton.StartHost();

    }

    public void StartServer()
    {
        Unity.Netcode.NetworkManager.Singleton.StartServer();
    }

    public void StartClient()
    {
        Unity.Netcode.NetworkManager.Singleton.StartClient();
    }

    public void ShowSteamID()
    {
        ownerSteamID.text = "Your Steam ID:" + SteamClient.SteamId;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (SteamClient.SteamId != 0 && showID == false)
        {
            ShowSteamID();
            showID = true;
        }
    }
    //public void SetSteamId(string targetID)
    //{

    //    var stringBuilder = new System.Text.StringBuilder(256);
    //    stringBuilder.Append(targetID);
    //    hostSteamID.text = stringBuilder.ToString();
    //}
}
