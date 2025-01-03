using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyEntrance : MonoBehaviour, IInteractable
{
    // Text to display
    [SerializeField] private string _interactionPrompt;
    public string InteractionPrompt => _interactionPrompt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log($"Interacting with door: {gameObject.name}");
        Debug.Log("door Opened");
        LoadMultiplayerLobby();
        return true;
    }

    private void LoadMultiplayerLobby()
    {
        // SceneManager.LoadScene("MultiplayerLobby");
        SceneManager.LoadScene("testlobby");
    }
}
