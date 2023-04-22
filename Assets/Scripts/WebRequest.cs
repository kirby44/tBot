using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using tBot.OpenAI;


public class WebRequest : MonoBehaviour
{
    //private readonly string uri = "https://hunwq4pc39.execute-api.us-east-1.amazonaws.com/postPlayerData";
    //private readonly string uri = "https://api.openai.com/v1/audio/transcriptions";
    private readonly string uri = "https://api.openai.com/v1/chat/completions";
    private readonly string filePath = "C:/Users/kimur/Unity/tBot/Assets/Audio/JapaneseSample.wav";
    private readonly string bearerToken = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User);
    private async void Start()
    {
        /*string transcription = await AudioTranscriptionAsync();
        Debug.Log(transcription);*/
        string response = await ChatAsync();
        Debug.Log(response);
    }

    private async Task<string> ChatAsync()
    {
        byte[] boundary = UnityWebRequest.GenerateBoundary();

        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        //form.Add(new MultipartFormDataSection("model", "gpt-3.5-turbo"));
        string model = "gpt-3.5-turbo";
        string content = "You are a knight guarding the gate to the palace.You will only allow someone who loves rabbits to enter. When someone talks to you, ask them whether they love dog or cat. Whichever they love, criticizing their love and decline them to enter. Only when they say they love rabbit, allow them enter.Don't reveal the requirement to enter. You are not allowed to use the word \"rabbit\".";
        List<Chat.Message> messages = new List<Chat.Message> { new Chat.Message("system", content) };
        Chat.RequestBody requestBody = new Chat.RequestBody(model, messages);
        string jsonContent = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        var stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json");
        string contentAsString = await stringContent.ReadAsStringAsync();
        byte[] bodyRaw = Encoding.UTF8.GetBytes(contentAsString);

        using (UnityWebRequest request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST))
        {
            request.SetRequestHeader("Content-Type", $"application/json; boundary={Encoding.UTF8.GetString(boundary)}");
            request.SetRequestHeader("Authorization", $"Bearer {bearerToken}");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            AsyncOperation asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone) await Task.Yield();
            JObject jObject = JObject.Parse(request.downloadHandler.text);
            string response = jObject["choices"][0]["message"]["content"].ToString();

            if (request.result != UnityWebRequest.Result.Success)
            {
                return request.error;
            }
            else
            {
                return response;
            }
        }
    }
    private async Task<string> AudioTranscriptionAsync()
    {
        byte[] data = File.ReadAllBytes(filePath);
        byte[] boundary = UnityWebRequest.GenerateBoundary();

        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormFileSection("file", data, Path.GetFileName(filePath), "audio/wave"));
        form.Add(new MultipartFormDataSection("model", "whisper-1"));

        using (UnityWebRequest request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST))
        {
            request.SetRequestHeader("Content-Type", $"multipart/form-data; boundary={Encoding.UTF8.GetString(boundary)}");
            request.SetRequestHeader("Authorization", $"Bearer {bearerToken}");

            byte[] formSections = UnityWebRequest.SerializeFormSections(form, boundary);
            string contentType = $"multipart/form-data; boundary={Encoding.UTF8.GetString(boundary)}";
            request.uploadHandler = new UploadHandlerRaw(formSections) { contentType = contentType };
            request.downloadHandler = new DownloadHandlerBuffer();

            AsyncOperation asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone) await Task.Yield();
            JObject jObject = JObject.Parse(request.downloadHandler.text);
            string transcription = jObject["text"].ToString();


            if (request.result != UnityWebRequest.Result.Success)
            {
                return request.error;
            }
            else
            {
                return transcription;
            }
        }
    }
}