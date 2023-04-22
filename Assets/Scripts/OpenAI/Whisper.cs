using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace tBot.OpenAI

{
    public class Whisper
    {
        #region fields

        private Bot bot;
        private string uri = "https://api.openai.com/v1/audio/translations";

        #endregion

        public Whisper(Bot bot)
        { this.bot = bot; }

        #region methods
        public async Task<string> SpeechTlanslationAsync(byte[] audioData)
        {
            string audioFilePath = bot.audioFilePath;
            string speechTlanslation = await SendRequestAsync(audioData, audioFilePath);
            return speechTlanslation;
        }

        private async Task<string> SendRequestAsync(byte[] audioData, string audioFilePath)
        {
            byte[] boundary = UnityWebRequest.GenerateBoundary();

            List<IMultipartFormSection> form = new List<IMultipartFormSection>();
            form.Add(new MultipartFormFileSection("file", audioData, audioFilePath, "audio/wave"));
            form.Add(new MultipartFormDataSection("model", "whisper-1"));

            using (UnityWebRequest request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST))
            {
                request.SetRequestHeader("Content-Type", $"multipart/form-data; boundary={Encoding.UTF8.GetString(boundary)}");
                request.SetRequestHeader("Authorization", $"Bearer {bot.bearerToken}");

                byte[] formSections = UnityWebRequest.SerializeFormSections(form, boundary);
                string contentType = $"multipart/form-data; boundary={Encoding.UTF8.GetString(boundary)}";
                request.uploadHandler = new UploadHandlerRaw(formSections) { contentType = contentType };
                request.downloadHandler = new DownloadHandlerBuffer();

                AsyncOperation asyncOperation = request.SendWebRequest();
                while (!asyncOperation.isDone) await Task.Yield();
                JObject jObject = JObject.Parse(request.downloadHandler.text);
                string speechTranslation = jObject["text"].ToString();


                if (request.result != UnityWebRequest.Result.Success)
                {
                    return request.error;
                }
                else
                {
                    return speechTranslation;
                }
            }
        }
        #endregion
    }
}