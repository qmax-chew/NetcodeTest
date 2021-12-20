using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Steamworks;


public class LobbyUI : MonoBehaviour
{
    public VerticalLayoutGroup lobbyName;
    public VerticalLayoutGroup lobbyPlayerCount;
    LobbyManager lobbyManager;
    public GameObject lobbyListPrefab;
    public int currentPage = 1;

    [SerializeField] private Transform chatBoxContent;
    [SerializeField] private GameObject chatLine;

    // Start is called before the first frame update
    void Start()
    {
        lobbyManager = LobbyManager.Instance();
        for (int index = 0; index < lobbyName.transform.childCount; index++)
        {
            Transform g = lobbyName.transform.GetChild(index);
            g.GetComponent<Button>().onClick.AddListener(() => lobbyManager.SelectLobby(g.GetSiblingIndex() * currentPage));
    
        }
        SteamMatchmaking.OnChatMessage += (lobby, member, message) =>
        {
            var newLine = Instantiate(chatLine);
            newLine.GetComponent<TextMeshProUGUI>().text = member + ":" + message;
            newLine.transform.parent = chatBoxContent;
            Debug.Log($"{member}: {message}");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async public void RefreshLobbyList()
    {
        var handle = lobbyManager.RefreshLobbyList();
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        button.interactable = false;
        while (handle.IsCompleted != true)
        {
            await Task.Yield();
        }
        button.interactable = true;

        RectTransform parent = lobbyName.GetComponent<RectTransform>();
        for (int index = 0; index < lobbyName.transform.childCount; index++)
        {
            Transform g = lobbyName.transform.GetChild(index);
            Transform t = lobbyPlayerCount.transform.GetChild(index);
            t.gameObject.SetActive(false);
            g.gameObject.SetActive(false);
            if (index < lobbyManager.lobbyList.Count)
            {
                g.gameObject.SetActive(true);
                t.gameObject.SetActive(true);
                foreach (KeyValuePair<string, string> kvp in lobbyManager.lobbyList[index].Data)
                {
                    g.GetComponentInChildren<Text>().text = kvp.Key;
                }
                   
                t.GetComponentInChildren<TextMeshProUGUI>().text = lobbyManager.lobbyList[index].MemberCount + "/"+ lobbyManager.lobbyList[index].MaxMembers;
            }
        }
    }


    async public void CreateLobbyUI()
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        button.interactable = false;
        var createLobby = await lobbyManager.CreateLobby();
        while (createLobby != true)
        {
            await Task.Yield();
        }
        button.interactable = true;

    }

    async public void JoinLobbyUI()
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        button.interactable = false;
        var joinLobby = await lobbyManager.JoinLobby();
        while (joinLobby != true)
        {
            await Task.Yield();
        }
        button.interactable = true;
    }

    async public void SendMessageUI()
    {
        var text = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
        var send = await lobbyManager.LobbySendMessage(text.text);
        while (send != true)
        {
            await Task.Yield();
        }
    }
}
