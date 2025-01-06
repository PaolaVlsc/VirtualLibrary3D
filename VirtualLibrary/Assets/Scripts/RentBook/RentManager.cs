// using UnityEngine;
// using UnityEngine.Networking;
// using UnityEngine.UI;
// using Newtonsoft.Json;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;

// public class RentManager : MonoBehaviour
// {
//     private string userRentListApiUrl = "http://localhost:8080/api/rentals/user/";
//     [SerializeField] private TextMeshProUGUI rentListText; // Assign the Text UI element in the Inspector

//     public void GetUserRentList()
//     {
//         // long userId = UserSessionManager.Instance.UserId;
//         long userId = 1; // Hardcoded user ID for testing
//         StartCoroutine(FetchUserRentList(userId));
//     }

//     private IEnumerator FetchUserRentList(long userId)
//     {
//         string url = userRentListApiUrl + userId;
//         Debug.Log("Fetching rent list for user ID: " + userId);
//         UnityWebRequest request = UnityWebRequest.Get(url);

//         yield return request.SendWebRequest();

//         if (request.result == UnityWebRequest.Result.Success)
//         {
//             string jsonResponse = request.downloadHandler.text;

//             // Deserialize the JSON response into a list of Rental objects
//             List<Rental> rentalList = JsonConvert.DeserializeObject<List<Rental>>(jsonResponse);

//             // Build the display string
//             string rentListDisplay = $"Rent List for User {userId}:\n";
//             foreach (var rental in rentalList)
//             {
//                 rentListDisplay += $"- Rental ID: {rental.id}, Book ID: {rental.book}, Status: {rental.status}\n" +
//                                    $"  Rent Date: {rental.rentDate}, Due Date: {rental.dueDate}, Return Date: {(rental.returnDate ?? "Not Returned")}\n\n";
//             }

//             // Update the UI Text
//             rentListText.text = rentListDisplay;
//         }
//         else
//         {
//             Debug.LogError("Failed to fetch user rent list: " + request.error);
//             rentListText.text = "Error fetching rent list.";
//         }
//     }
// }
