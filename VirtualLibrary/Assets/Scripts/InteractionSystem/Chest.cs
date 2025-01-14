using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionPrompt = "Press E to interact with the chest or F to rent the book";
    public string InteractionPrompt => _interactionPrompt;

    // The ID of the book this chest represents (can be set in the Inspector)
    [SerializeField] private long bookId;

    // URLs for API requests
    private string apiUrl = "http://localhost:8080/api/books/";
    private string rentApiUrl = "http://localhost:8080/api/rentals/rent"; // Rent API endpoint
    private string addFavoriteApiUrl = "http://localhost:8080/api/v1/favorites/add"; // API for adding to favorites

    public bool Interact(Interactor interactor)
    {
        Debug.Log($"Interacting with chest: {gameObject.name}");
        Debug.Log("Fetching Book Details...");
        StartCoroutine(FetchBookDetails(bookId));
        return true;
    }

    // Method to handle renting the book
    public void RentBook(long userId)
    {
        Debug.Log($"Attempting to rent book ID {bookId} for user ID {userId}");
        StartCoroutine(SendRentRequest(userId, bookId));
    }

    // Fetch details of the book
    private IEnumerator FetchBookDetails(long id)
    {
        string url = apiUrl + id;
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Wait for the request to complete
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Book fetchedBook = JsonUtility.FromJson<Book>(jsonResponse);

            Debug.Log($"Chest Name: {gameObject.name}");
            Debug.Log($"Book Title: {fetchedBook.title}");
            Debug.Log($"Author: {fetchedBook.author}");
            Debug.Log($"Published Year: {fetchedBook.publishedYear}");
            Debug.Log($"Genre: {fetchedBook.genre}");
        }
        else
        {
            Debug.LogError("Failed to fetch book details: " + request.error);
        }
    }

    private IEnumerator SendRentRequest(long userId, long bookId)
    {
        userId = UserSessionManager.Instance.UserId;
        // Use the RentalRequest class
        RentalRequest rentalRequest = new RentalRequest(userId, bookId);
        string jsonData = JsonUtility.ToJson(rentalRequest);

        // Log the JSON body for debugging
        Debug.Log("Sending Rent Request with Body: " + jsonData);

        UnityWebRequest request = new UnityWebRequest(rentApiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Rent successful! Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Failed to rent book: {request.error} | Response: {request.downloadHandler.text}");
        }
    }

    // Method to handle adding the book to favorites
    public void AddToFavorites(long userId)
    {
        Debug.Log($"Attempting to add book ID {bookId} to favorites for user ID {userId}");

        StartCoroutine(SendAddFavoriteRequest(userId, bookId));
    }

    private IEnumerator SendAddFavoriteRequest(long userId, long bookId)
    {
        // Make sure you're using the correct field names and serializing them properly
        FavoriteBookRequest bookRequestFavorite = new FavoriteBookRequest(userId, bookId);
        string jsonData = JsonUtility.ToJson(bookRequestFavorite);

        // Log the JSON body for debugging
        Debug.Log("Sending Add Favorite Request with Body: " + jsonData);

        UnityWebRequest request = new UnityWebRequest(addFavoriteApiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Book successfully added to favorites!");
        }
        else
        {
            Debug.LogError($"Failed to add book to favorites: {request.error} | Response: {request.downloadHandler.text}");
        }
    }

}
