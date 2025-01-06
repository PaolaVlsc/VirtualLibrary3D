using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class FavoritesManagerUI : MonoBehaviour
{
    public string baseApiUrl = "http://localhost:8080/api/v1/favorites/view/"; // Base URL for favorite books
    public string removeApiUrl = "http://localhost:8080/api/v1/favorites/remove/"; // API URL to remove a favorite
    public GameObject favoriteRowPrefab;
    public Transform favoriteListContainer;
    public GameObject favoriteScrollView; // Scroll view for favorite book list
    public Button viewFavoritesButton;

    void Start()
    {
        favoriteScrollView.SetActive(false); // Initially hide the scroll view
        viewFavoritesButton.onClick.AddListener(OnViewFavoritesClicked);
    }

    private void OnViewFavoritesClicked()
    {
        long loggedInUserId = UserSessionManager.Instance.UserId; // Fetch the user ID from the session manager
        Debug.Log($"Logged-in User ID: {loggedInUserId}");

        favoriteScrollView.SetActive(true); // Show the scroll view when the button is clicked
        StartCoroutine(FetchFavoritesFromAPI(loggedInUserId));
    }

    IEnumerator FetchFavoritesFromAPI(long userId)
    {
        string apiUrl = $"{baseApiUrl}{userId}";
        Debug.Log($"Fetching favorite books for user {userId}...");

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching data: {request.error}");
            }
            else
            {
                Debug.Log("Data fetched successfully");
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"Response: {jsonResponse}");

                // Deserialize JSON into a List<Favorite> using Newtonsoft.Json
                List<Favorite> favorites = JsonConvert.DeserializeObject<List<Favorite>>(jsonResponse);
                Debug.Log($"Parsed {favorites.Count} favorite books");

                // Display favorite books in the UI
                DisplayFavorites(favorites);
            }
        }
    }

    public void DisplayFavorites(List<Favorite> favorites)
    {
        // Clear existing UI elements
        foreach (Transform child in favoriteListContainer)
        {
            Destroy(child.gameObject);
        }

        // Create a row for each favorite book
        foreach (var favorite in favorites)
        {
            GameObject row = Instantiate(favoriteRowPrefab, favoriteListContainer);
            row.transform.Find("BookTitle").GetComponent<TextMeshProUGUI>().text = favorite.bookTitle;
            row.transform.Find("Author").GetComponent<TextMeshProUGUI>().text = favorite.bookAuthor;

            // Handle the "Remove" button
            Button removeButton = row.transform.Find("RemoveButton").GetComponent<Button>();
            removeButton.gameObject.SetActive(true);
            removeButton.onClick.AddListener(() => StartCoroutine(RemoveFavoriteBook(favorite.id)));
        }
    }

    IEnumerator RemoveFavoriteBook(int favoriteId)
    {
        string url = $"{removeApiUrl}{favoriteId}";

        Debug.Log($"Attempting to remove book with ID: {favoriteId}");

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error removing book: {request.error}");
            }
            else
            {
                Debug.Log($"Book with ID {favoriteId} removed from favorites successfully");

                // After removing the book, refresh the favorite list
                long loggedInUserId = UserSessionManager.Instance.UserId;
                StartCoroutine(FetchFavoritesFromAPI(loggedInUserId));
            }
        }
    }

    // Class to hold the favorite book data (matches the structure of the JSON)
    [System.Serializable]
    public class Favorite
    {
        public int id;
        public int userId;
        public int bookId;
        public string bookTitle;
        public string bookAuthor;
    }
}
