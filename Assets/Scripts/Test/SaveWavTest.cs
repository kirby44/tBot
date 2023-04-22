using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace tBot
{
    public class SaveWavTest : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;

        private AudioClip clip;
        private bool isRecording;
        private float time;

        private void Start ()
        {
            Debug.Log("Start method called");
            recordButton.onClick.AddListener(StartRecording);
            print(Application.persistentDataPath);
        }

        public void StartRecording()
        {
            Debug.Log("StartRecording method called");
            isRecording = true;
            recordButton.enabled = false;

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            clip = Microphone.Start(Microphone.devices[0], false, duration, 44100);
            print(Microphone.devices[0]);
        }

        private void EndRecording()
        {
            message.text = "Transcripting...";

            Microphone.End(null);
            byte[] data = SaveWav.Save(fileName, clip);
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, data);
            print("Audio saved to: " + filePath);

            progressBar.fillAmount = 0;
            recordButton.enabled = true;
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
