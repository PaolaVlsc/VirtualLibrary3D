using UnityEngine;

public class UserSessionManager : MonoBehaviour
{
    // The static instance of this class (the Singleton)
    private static UserSessionManager _instance;

    // Property to access the instance
    public static UserSessionManager Instance
    {
        get
        {
            // If the instance doesn't exist, create one
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<UserSessionManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("UserSessionManager");
                    _instance = singletonObject.AddComponent<UserSessionManager>();
                }
            }
            return _instance;
        }
    }

    // The logged-in username (can be expanded with more user session data)
    private string _loggedInUsername;

    // Property to get and set the logged-in username
    public string LoggedInUsername
    {
        get => _loggedInUsername;
        set => _loggedInUsername = value;
    }

    // Ensure that this object persists across scene loads
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Make sure it persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy if an instance already exists
        }
    }
}
