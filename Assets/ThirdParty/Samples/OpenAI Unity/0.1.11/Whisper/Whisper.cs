using System;
using UnityEngine;
using UnityEngine.UI;
using tBot;

namespace OpenAI
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;
        [SerializeField] private Dropdown dropdown;
        
        private readonly string fileName = "output.wav";
        private readonly int duration = 5;
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));
        private OpenAIController openAIController;
        private void Start()
        {
            GameObject go = GameObject.Find("OpenAIController");
            openAIController = go.GetComponent<OpenAIController>();

            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        private void StartRecording()
        {
            isRecording = true;
            recordButton.enabled = false;

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
        }

        private async void EndRecording()
        {
            message.text = "Transcripting...";
            
            Microphone.End(null);
            byte[] data = SaveWav.Save(fileName, clip);

            //var req = new CreateAudioTranscriptionsRequest
            //{
            //    FileData = new FileData() { Data = data, Name = "audio.wav" },
            //    // File = Application.persistentDataPath + "/" + fileName,
            //    Model = "whisper-1",
            //    Language = "en"
            //};
            //var res = await openai.CreateAudioTranscription(req);
            
            var req = new CreateAudioTranslationRequest
            {
                FileData = new FileData() {Data = data, Name = "audio.wav"},
                // File = Application.persistentDataPath + "/" + fileName,
                Model = "whisper-1",
            };
            var res = await openai.CreateAudioTranslation(req);

            progressBar.fillAmount = 0;
            message.text = res.Text;
            recordButton.enabled = true;
            openAIController.inputField.text = res.Text;
            openAIController.GetResponse();
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;
                
                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }
    }
}
