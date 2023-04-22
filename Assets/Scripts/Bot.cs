using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using tBot.OpenAI;
using System;

namespace tBot
{
    public class Bot : MonoBehaviour
    {
        #region fields

        [SerializeField] private Button recordButton;
        [SerializeField] public Image progressBar;
        [SerializeField] public Text textField;
        /// <summary>
        /// text that appears when game starts
        /// </summary>
        [SerializeField] private string startText;
        /// <summary>
        /// content that decides ChatGPT's behavior
        /// i.e. "You are an honorable knight"
        /// </summary>
        [SerializeField] private string content;

        public AudioRecorder audioRecorder;
        public Whisper whisper;
        public Chat chat;

        public readonly string bearerToken = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User);

        #endregion

        #region property
        public string audioFilePath { get; private set; }

        #endregion
        #region methods
        private void Awake()
        {
            audioFilePath = Path.Combine(Application.persistentDataPath, "output.wav");
            audioRecorder = GetComponent<AudioRecorder>();
            audioRecorder.bot = this;
            whisper = new Whisper(this);
            chat = new Chat(this);
        }

        private void Start()
        {
            recordButton.onClick.AddListener(async () => await OnRecordButtonClick());
            InitializeConversation();
        }

        private async Task OnRecordButtonClick()
        {
            await ConversationAsync();
        }

        private async Task ConversationAsync()
        {
            recordButton.enabled = false;
            AudioClip clip = await audioRecorder.RecordAsync();
            Debug.Log("Recorded");
            byte[] audioData = SaveWav.Save(audioFilePath, clip);
            Debug.Log("Saved wav.file");
            string speechTlanslation = await whisper.SpeechTlanslationAsync(audioData);
            Debug.Log(string.Format("You said:{0}",speechTlanslation));
            await chat.ChatAsync(new Chat.Message("user", speechTlanslation));
            Debug.Log("conversation finished!");
            recordButton.enabled = true;
        }
        private void InitializeConversation()
        {
            textField.text = startText;
            Debug.Log(startText);
            chat.messages.Add(new Chat.Message("system", content));
        }
        #endregion
    }
}