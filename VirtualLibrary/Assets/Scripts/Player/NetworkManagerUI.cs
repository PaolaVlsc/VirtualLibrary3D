using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button hostButton;

    private void Awake()
    {
        serverButton.onClick.AddListener(OnServerButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
        hostButton.onClick.AddListener(OnHostButtonClicked);
    }

    private void OnServerButtonClicked()
    {
        NetworkManager.Singleton.StartServer();
    }

    private void OnClientButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void OnHostButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
    }

}
