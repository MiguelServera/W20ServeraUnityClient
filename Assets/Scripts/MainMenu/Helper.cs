using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Helper : MonoBehaviour
{

    internal static IEnumerator InitializeToken(string email, string password)
    {
        Player player = FindObjectOfType<Player>();
        if (string.IsNullOrEmpty(player.Token))
        {
            UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/Token", "POST");

            // application/x-www-form-urlencoded
            WWWForm dataToSend = new WWWForm();
            dataToSend.AddField("grant_type", "password");
            dataToSend.AddField("username", email);
            dataToSend.AddField("password", password);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend.data);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            httpClient.SetRequestHeader("Accept", "application/json");
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("Helper > InitToken: " + httpClient.error);
            }
            else
            {
                string jsonResponse = httpClient.downloadHandler.text;
                AuthorizationToken authToken = JsonUtility.FromJson<AuthorizationToken>(jsonResponse);
                player.Token = authToken.access_token;
            }
            httpClient.Dispose();
        }
    }

    internal static IEnumerator GetPlayerId()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Account/UserId", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerId: " + httpClient.error);
        }
        else
        {
            //It puts a backlash so we can't insert the ID right. We must replace it with nothing so it stays like a normal string.
            player.Id = httpClient.downloadHandler.text.Replace("\"", "");
        }

        httpClient.Dispose();
    }

    internal static IEnumerator GetPlayerInfo()
    {
        Player player = FindObjectOfType<Player>();
        //Again, is /api, not api/...
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/Info", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerInfo: " + httpClient.error);
        }
        else
        {
            string stringToRecieve = httpClient.downloadHandler.text;
            PlayerSerializable playerSerializable = JsonUtility.FromJson<PlayerSerializable>(stringToRecieve);
            player.Id = playerSerializable.Id;
            player.Name = playerSerializable.Name;
            player.Email = playerSerializable.Email;
            player.BirthDay = DateTime.Parse(playerSerializable.BirthDay);
        }

        httpClient.Dispose();
    }

}
