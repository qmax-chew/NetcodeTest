using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Steamworks;
using Steamworks.Data;


public enum LobbyPlayerLimit
{
    TwoPlayer = 2,
    FourPlayer = 4,
    EightPlayer = 8
}

public class LobbyManager : MonoBehaviour
{
    private bool lobbyDisconneted;
    public ulong lobbyId;
    public Lobby currentLobby;
    private Lobby selectedLobbyData;
    public LobbyPlayerLimit limit = LobbyPlayerLimit.FourPlayer;
    public SteamId OpponentSteamId { get; set; }
    public List<Lobby> lobbyList;

    // Start is called before the first frame update
    void Start()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallback;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task<bool> CreateLobby()
    {
        try
        {
            var createLobby =await SteamMatchmaking.CreateLobbyAsync((int)limit);
            if (!createLobby.HasValue)
            {
                Debug.Log("Lobby created but not correctly instantiated");
                throw new System.Exception();
            }
            selectedLobbyData = createLobby.Value;
            selectedLobbyData.SetPublic();
            selectedLobbyData.SetJoinable(true);
            string lobbyName = SteamClient.Name + "'s lobby";
            selectedLobbyData.SetData("name", lobbyName);
            currentLobby = selectedLobbyData;
        }
        catch (System.Exception exception)
        {
            Debug.Log("Failed to create multiplayer lobby");
            Debug.Log(exception.ToString());
            return false;
        }
        return true;
    }

    //public async Task<bool> JoinLobby()
    //{
    //    //try
    //    //{
    //    //    var joinLobby = await SteamMatchmaking.JoinLobbyAsync(selectedLobbyData.Id)
    //    //}
    //    //return true;
    //}

        void OnLobbyCreatedCallback(Result result, Lobby lobby)
    {
        // Lobby was created
        lobbyDisconneted = false;
        if (result != Result.OK)
        {
            Debug.Log("lobby creation result not ok");
            Debug.Log(result.ToString());
        }
    }

    public async Task<bool> RefreshLobbyList()
    {
        try
        {
            lobbyList.Clear();
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithMaxResults(20).FilterDistanceClose().RequestAsync();
            if (lobbies != null)
            {
                foreach (Lobby lobby in lobbies)
                {
                    lobbyList.Add(lobby);
                }
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log("Error fetching multiplayer lobbies");
            return true;
        }
    }

    void SelectLobby(int no)
    {
        selectedLobbyData = lobbyList[no];
    }
}
