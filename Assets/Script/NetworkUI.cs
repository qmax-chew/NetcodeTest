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
    public Netcode.Transports.Facepunch.FacepunchTransport facepunchTransport;
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
        facepunchTransport.targetSteamId = ulong.Parse(hostSteamID.text);
        Unity.Netcode.NetworkManager.Singleton.StartClient();
    }

    public void ShowSteamID()
    {
        ownerSteamID.text = "Your Steam ID:" + facepunchTransport.GetUserSteamID();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (facepunchTransport.GetUserSteamID() != 0 && showID == false)
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
