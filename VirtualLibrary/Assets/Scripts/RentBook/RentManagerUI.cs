using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class RentManagerUI : MonoBehaviour
{
    public string baseApiUrl = "http://localhost:8080/api/rentals/user/";
    public string returnApiUrl = "http://localhost:8080/api/rentals/return/";
    public GameObject rentalRowPrefab;
    public Transform rentalListContainer;
    public GameObject rentalScrollView; // Scroll view for rental list
    public Button viewHistoryButton;

    void Start()
    {
        rentalScrollView.SetActive(false); // Initially hide the scroll view
        viewHistoryButton.onClick.AddListener(OnViewHistoryClicked);
    }

    private void OnViewHistoryClicked()
    {
        long loggedInUserId = UserSessionManager.Instance.UserId; // Fetch the user ID from the session manager
        Debug.Log($"Logged-in User ID: {loggedInUserId}");

        rentalScrollView.SetActive(true); // Show the scroll view when the button is clicked
        StartCoroutine(FetchRentalsFromAPI(loggedInUserId));
    }

    IEnumerator FetchRentalsFromAPI(long userId)
    {
        string apiUrl = $"{baseApiUrl}{userId}";
        Debug.Log($"Fetching data from API for user {userId}...");

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

                // Deserialize JSON into a List<Rental> using Newtonsoft.Json
                List<Rental> rentals = JsonConvert.DeserializeObject<List<Rental>>(jsonResponse);
                Debug.Log($"Parsed {rentals.Count} rentals");

                // Display rentals in the UI
                DisplayRentals(rentals);
            }
        }
    }

    public void DisplayRentals(List<Rental> rentals)
    {
        // Clear existing UI elements
        foreach (Transform child in rentalListContainer)
        {
            Destroy(child.gameObject);
        }

        // Create a row for each rental
        foreach (var rental in rentals)
        {
            GameObject row = Instantiate(rentalRowPrefab, rentalListContainer);
            row.transform.Find("RentalID").GetComponent<TextMeshProUGUI>().text = rental.id.ToString();
            row.transform.Find("Book").GetComponent<TextMeshProUGUI>().text = rental.bookTitle;
            row.transform.Find("Author").GetComponent<TextMeshProUGUI>().text = rental.bookAuthor;
            row.transform.Find("RentDate").GetComponent<TextMeshProUGUI>().text = rental.rentDate;
            row.transform.Find("DueDate").GetComponent<TextMeshProUGUI>().text = rental.dueDate;
            row.transform.Find("ReturnDate").GetComponent<TextMeshProUGUI>().text = rental.returnDate ?? "Not Returned";
            row.transform.Find("Status").GetComponent<TextMeshProUGUI>().text = rental.status;

            // Handle the "Return" button
            Button returnButton = row.transform.Find("ReturnButton").GetComponent<Button>();
            returnButton.gameObject.SetActive(rental.status == "Active");
            returnButton.onClick.AddListener(() => StartCoroutine(ReturnBook(rental.id)));
        }
    }

    IEnumerator ReturnBook(int rentalId)
    {
        string url = $"{returnApiUrl}{rentalId}";

        Debug.Log($"Attempting to return book with ID: {rentalId}");

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error returning book: {request.error}");
            }
            else
            {
                Debug.Log($"Book with ID {rentalId} returned successfully");

                // After returning the book, refresh the rental list
                long loggedInUserId = UserSessionManager.Instance.UserId;
                StartCoroutine(FetchRentalsFromAPI(loggedInUserId));
            }
        }
    }
}
