
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionPrompt = "Press E to interact with the chest";
    public string InteractionPrompt => _interactionPrompt;

    // The ID of the book this chest represents (can be set in the Inspector)
    [SerializeField] private long bookId;

    // URL for the API to fetch book details (Adjust the URL if needed)
    private string apiUrl = "http://localhost:8080/api/books/";

    public bool Interact(Interactor interactor)
    {
        Debug.Log($"Interacting with chest: {gameObject.name}");
        Debug.Log("Chest Opened");
        StartCoroutine(FetchBookDetails(bookId));
        return true;
    }

    private IEnumerator FetchBookDetails(long id)
    {
        string url = apiUrl + id;
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Wait for the request to complete
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the JSON response
            string jsonResponse = request.downloadHandler.text;
            Book fetchedBook = JsonUtility.FromJson<Book>(jsonResponse);

            // Display both the book details and the chest object name
            Debug.Log($"Chest Name: {gameObject.name}");
            Debug.Log("Book Response: " + jsonResponse);  // Log the raw response
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
}