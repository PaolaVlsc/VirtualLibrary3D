using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ChatSystem : NetworkBehaviour
{
    [SerializeField] private TMP_InputField chatInputField; // Input field for player messages
    [SerializeField] private TMP_Text chatDisplayText;     // Text area to display chat messages

    private bool isChatFocused = false; // Track whether chat is focused
    public static bool IsChatFocused { get; private set; } // Expose the focus state

    // Movement blocker flag
    public static bool BlockMovement { get; private set; } = false;

    private void Start()
    {
        if (chatInputField != null)
        {
            chatInputField.onSubmit.AddListener(OnSubmitMessage);
            chatInputField.gameObject.SetActive(false); // Hide the chat input initially
        }
    }

    private void Update()
    {
        // Handle chat open/close
        if (Input.GetKeyDown(KeyCode.Return)) // Return is the Enter key
        {
            if (!isChatFocused)
            {
                OpenChat();
            }
            else
            {
                SendChatMessage(); // Send message and close chat
            }
        }

        if (isChatFocused && Input.GetMouseButtonDown(0)) // Left mouse button
        {
            CheckClickOutsideInput();
        }
    }

    private void OpenChat()
    {
        isChatFocused = true;
        IsChatFocused = true;
        BlockMovement = true;  // Block movement when chat is open

        // Show the chat input field and enable interaction
        chatInputField.gameObject.SetActive(true);
        chatInputField.ActivateInputField();
    }

    private void CloseChat()
    {
        isChatFocused = false;
        IsChatFocused = false;
        BlockMovement = false;  // Allow movement after chat is closed

        // Hide the chat input field and disable interaction
        chatInputField.DeactivateInputField();
        chatInputField.gameObject.SetActive(false);

        // Clear focus to avoid leftover input handling
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnSubmitMessage(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText)) return; // Ignore empty messages

        // Send message to the server
        SendChatMessageServerRpc(inputText);

        // Clear the input field and close chat
        chatInputField.text = "";
        CloseChat();
    }

    public void SendChatMessage()
    {
        if (!string.IsNullOrWhiteSpace(chatInputField.text))
        {
            OnSubmitMessage(chatInputField.text);
        }
        else
        {
            CloseChat(); // Close chat if no message is written
        }
    }

    private void CheckClickOutsideInput()
    {
        // Check if the clicked UI element is not the chat input field
        if (EventSystem.current.currentSelectedGameObject != chatInputField.gameObject)
        {
            CloseChat();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        string senderName = senderClientId == NetworkManager.Singleton.LocalClientId ? "Host" : $"Player {senderClientId}";

        string formattedMessage = $"{senderName}: {message}";
        BroadcastChatMessageClientRpc(formattedMessage);
    }

    [ClientRpc]
    private void BroadcastChatMessageClientRpc(string message)
    {
        if (chatDisplayText != null)
        {
            chatDisplayText.text += message + "\n";
        }
    }
}
