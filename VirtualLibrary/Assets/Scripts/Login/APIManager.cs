using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ApiManager : MonoBehaviour
{
    // Backend Server URL
    private const string BaseUrl = "http://localhost:8080/api";


    // Register a new user  
    public async Task<string> RegisterUser(string username, string password)
    {
        var url = $"{BaseUrl}/register";

        var user = new { username = username, password = password };
        var jsonData = JsonConvert.SerializeObject(user);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone) await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }
        }
    }

    // Login a user
    public async Task<string> LoginUser(string username, string password)
    {
        var url = $"{BaseUrl}/login";

        var user = new { username = username, password = password };
        var jsonData = JsonConvert.SerializeObject(user);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone) await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }
        }
    }
}
