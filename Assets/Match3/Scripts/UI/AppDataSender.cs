using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Match3
{
    public class AppDataSender : MonoBehaviour
    {
        string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfE4RU6VB4VGDl-amS7mkyhWyVijh1hUsS5WVAkaVrSYnjjxA/formResponse";
        string model = "";

        private void Start()
        {
            model = SystemInfo.deviceUniqueIdentifier;
            if(!PlayerPrefs.HasKey("ApplicationQuit")) PlayerPrefs.SetString("ApplicationQuit", "true");
            if(PlayerPrefs.GetString("ApplicationQuit") == "true")
            {
                Send("Open Application");
                PlayerPrefs.SetString("ApplicationQuit", "false");
            }
        }

        public void Send(string message)
        {
            StartCoroutine(PostAppStatus(model, message));
        }

        IEnumerator PostAppStatus(string model, string message)
        {
            WWWForm form = new WWWForm();
            form.AddField("entry.1961167345", model);
            form.AddField("entry.1698418858", message);

            UnityWebRequest www = UnityWebRequest.Post(URL, form);
            
            yield return www.SendWebRequest();
        }
        
    }
}