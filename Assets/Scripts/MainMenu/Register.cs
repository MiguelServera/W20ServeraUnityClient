using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Globalization;
public class Register : MonoBehaviour
{
    // Cached references
    public InputField nameInputField;
    public InputField birthdateInputField;
    public InputField emailInputField;
    public InputField passwordInputField;
    public InputField confirmPasswordInputField;
    public Button registerButton;
    public Text messageBoardText;
    public Player player;

    //Search for the gameObject "Player"
    void Start()
    {
        player = FindObjectOfType<Player>();
    }
    public void OnRegisterButtonClick()
    {
        StartCoroutine(RegisterNewUser());
    }

    private IEnumerator RegisterNewUser()
    {
        yield return RegisterUser();
        yield return Helper.InitializeToken(emailInputField.text, passwordInputField.text);  //Sets player.Token
        messageBoardText.text += "\nToken: " + player.Token.Substring(0,7) + "...";
        yield return Helper.GetPlayerId();  //Sets player.Id
        messageBoardText.text += "\nId: " + player.Id;
        player.Email = emailInputField.text;
        player.Name = nameInputField.text;
        player.BirthDay = DateTime.Parse(birthdateInputField.text);
        yield return InsertPlayer();
        messageBoardText.text += $"\nPlayer \"{player.Name}\" registered.";
        player.Id = string.Empty;
        player.Token = string.Empty;
        player.Name = string.Empty;
        player.Email = string.Empty;
        player.BirthDay = DateTime.MinValue;
    }

    private IEnumerator RegisterUser()
    {
        //Error, is /api, not api/...
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Account/Register", "POST");

        AspNetUserRegister newUser = new AspNetUserRegister();
        newUser.Email = emailInputField.text;
        newUser.Password = passwordInputField.text;
        newUser.ConfirmPassword = confirmPasswordInputField.text;

        string jsonData = JsonUtility.ToJson(newUser);
        byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);
        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);
        //Error, it's Content-type, not Content-Type
        httpClient.SetRequestHeader("Content-type", "application/json");
        //We need the certificate to return true, so we create a class that will always return true.
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("OnRegisterButtonClick: Error > " + httpClient.error);
        }

        messageBoardText.text += "\nOnRegisterButtonClick: " + httpClient.responseCode;

        httpClient.Dispose();
    }

    private IEnumerator InsertPlayer()
    {
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;
        playerSerializable.Name = player.Name;
        playerSerializable.Email = player.Email;
        playerSerializable.BirthDay = player.BirthDay.ToString();

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/RegisterPlayer", "POST"))
        {
            string playerData = JsonUtility.ToJson(playerSerializable);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(playerData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("RegisterNewPlayer > InsertPlayer: " + httpClient.error);
            }
			
			messageBoardText.text += "\nRegisterNewPlayer > InsertPlayer: " + httpClient.responseCode;
        }

    }
}
