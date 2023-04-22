using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace tBot
{
    public class WebRequestTest : MonoBehaviour
    {
        #region property

        private string uri = "https://api.openai.com/v1/audio/transcriptions";
        private List<IMultipartFormSection> form;
        private string filePath = "C:/Users/kimur/Unity/tBot/Assets/Audio/JapaneseSample.wav";

        [SerializeField] TextMeshProUGUI textMesh;

        #endregion

        #region constructor

        /*public WebRequest(string uri, WWWForm form)
        {
            this.uri = uri;
            this.form = form;
        }*/

        #endregion

        #region method
        private void Start()
        {
/*            form = new WWWForm();*/
            form = new List<IMultipartFormSection>();
            byte[] data = File.ReadAllBytes(filePath);

            SendRequest();
        }
        private async void SendRequest()
        {
            UnityWebRequest request = UnityWebRequest.Post(uri, form);
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            textMesh.text = request.downloadHandler.text;
        }

        #endregion


    }
}

/*public class WebRequest : MonoBehaviour
{
    //[SerializeField] TextMeshPro textMesh;
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] string url;
    void Start()
    {
        Get(url, (string error) =>
        {
            Debug.Log("Error: " + error);
            textMesh.SetText("Error: " + error);
        }, (string text) =>
        {
            Debug.Log("Received: " + text);
            textMesh.SetText(text);
        });
    }

    private void Get(string url, Action<string> onError, Action<string> onSuccess)
    {
        StartCoroutine(GetCoroutine(url, onError, onSuccess));
    }

    private IEnumerator GetCoroutine(string url, Action<string> onError, Action<string> onSuccess)
    {
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
        {
            yield return unityWebRequest.SendWebRequest();

            //if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError) {
            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                onError(unityWebRequest.error);
            }
            else
            {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }
    }
}*/