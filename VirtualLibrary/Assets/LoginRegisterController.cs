using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginRegisterController : MonoBehaviour
{
    [SerializeField]
    private Button loginButton;

    [SerializeField]
    private Button registerButton;
    [SerializeField]
    private Button loadGameButton;
    [SerializeField]
    public TMP_InputField usernameInput;
    [SerializeField]
    public TMP_InputField passwordInput;
    [SerializeField]
    public TextMeshProUGUI feedbackText;

    [SerializeField]
    private ApiManager apiManager;


    // general setup with the inspector
    void Start()
    {
        apiManager = Object.FindFirstObjectByType<ApiManager>();
        if (apiManager == null)
        {
            Debug.LogError("ApiManager not found. Please ensure it is added to the scene.");
        }

        if (usernameInput == null)
        {
            Debug.LogError("usernameInput is not assigned in the Inspector.");
        }

        if (passwordInput == null)
        {
            Debug.LogError("passwordInput is not assigned in the Inspector.");
        }

        if (feedbackText == null)
        {
            Debug.LogError("feedbackText is not assigned in the Inspector.");
        }
    }


    // Login button handler
    public async void OnLoginButtonClicked()
    {
        if (apiManager == null || usernameInput == null || passwordInput == null)
        {
            Debug.LogError("UIController references are not set up correctly.");
            return;
        }

        string username = usernameInput.text;
        string password = passwordInput.text;

        string response = await apiManager.LoginUser(username, password);

        if (!string.IsNullOrEmpty(response))
        {

            feedbackText.text = "Login Successful!";
            // set the load game active
            loadGameButton.gameObject.SetActive(true);

        }
        else
        {
            feedbackText.text = "Login Failed!";
        }
    }


    public void OnLoadGameButtonClicked(string username)
    {

        // Store the username in the Singleton
        UserSessionManager.Instance.LoggedInUsername = username;
        SceneManager.LoadScene("Library"); // Replace "Scene2" with the actual target scene name

    }
    public async void OnRegisterButtonClicked()
    {
        if (apiManager == null || usernameInput == null || passwordInput == null)
        {
            Debug.LogError("UIController references are not set up correctly.");
            return;
        }

        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            feedbackText.text = "Please enter a valid username and password.";
            return;
        }

        string response = await apiManager.RegisterUser(username, password);

        if (!string.IsNullOrEmpty(response))
        {
            feedbackText.text = "Registration Successful!";
            // set the load game active
            loadGameButton.gameObject.SetActive(true);

        }
        else
        {
            feedbackText.text = "Registration Failed!";
        }
    }
}
