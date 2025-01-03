using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RelayManager : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button leaveButton; // Button to leave the session
    [SerializeField] private TMP_InputField joinInput;
    [SerializeField] private TextMeshProUGUI codeText;

    private async void Start()
    {
        try
        {
            // Initialize Unity services
            await UnityServices.InitializeAsync();

            // Check if the player is already signed in
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // Assign button listeners
            hostButton.onClick.AddListener(CreateRelay);
            joinButton.onClick.AddListener(() => JoinRelay(joinInput.text));
            // Remove the leave button listener
            // leaveButton.onClick.AddListener(LeaveSession);

            // Initially hide the leave button
            leaveButton.gameObject.SetActive(false);

            // Subscribe to client connected callback
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to sign in or initialffize services: {ex.Message}");
        }
    }

    // private void Update()
    // {
    //     // Check if the "K" key is pressed
    //     if (Input.GetKeyDown(KeyCode.K))
    //     {
    //         LeaveSession();
    //     }
    // }

    private async void CreateRelay()
    {
        try
        {
            // Create a relay allocation and get the join code
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(10);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            codeText.text = "Code: " + joinCode;

            // Set up the relay server
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to create relay: {ex.Message}");
        }
    }

    private async void JoinRelay(string joinCode)
    {
        try
        {
            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("Join code is empty.");
                return;
            }

            // Join the relay session
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to join relay: {ex.Message}");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected.");

        // Hide host and join UI for the local player
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            hostButton.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            joinInput.gameObject.SetActive(false);

            // Show the leave button
            leaveButton.gameObject.SetActive(true);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected.");

        // If the host disconnects, all clients should return to the library scene
        if (NetworkManager.Singleton.IsHost && clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Host has disconnected. Shutting down the session for all clients.");

            // Notify all clients and shut down
            NetworkManager.Singleton.Shutdown();

            // Load the library scene for the host
            SceneManager.LoadScene("Playground");
        }
        else if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Local cliesnt disconnected. Returning to the library.");
            SceneManager.LoadScene("Playground");
        }
        else
        {
            Debug.Log($"Client {clientId} disconnected from the session.");
        }
    }

    public void LeaveSession()
    {
        Debug.Log("Leaving session...");

        // Notify clients and shut down
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host is shutting down the session.");
            NetworkManager.Singleton.Shutdown();
            // Load the library scene
            SceneManager.LoadScene("Playground");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Client is disconnecting from the session.");
            NetworkManager.Singleton.Shutdown();
            // Load the library scene
            SceneManager.LoadScene("Playgrounda");
        }

    }

    public override void OnDestroy()
    {
        // Unsubscribe from callbacks to avoid memory leaks
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}
