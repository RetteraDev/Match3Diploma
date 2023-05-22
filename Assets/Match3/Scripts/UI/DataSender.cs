using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Match3
{
    public class DataSender : MonoBehaviour
    {
        string URL = "https://docs.google.com/forms/d/e/1FAIpQLSdQoYNus_vKEGR5j-m43SRDjdUb_p_O_11E0raWlXEutR2YoQ/formResponse";
        string model = "";

        private void Start() => model = SystemInfo.deviceUniqueIdentifier;

        public void Send(string levelName, string levelStatus, string movesUsed, string movesLeft, 
            string timeUsed, string timeLeft, string obstaclesDestroyed, string obstaclesLeft,
            string horisontalBonus, string verticalBonus, string rainbowBonus, string score, string points, string stars)
        {
            string[] parameters =  new string[] {levelName, levelStatus, movesUsed, movesLeft, timeUsed, timeLeft,
                obstaclesDestroyed, obstaclesLeft, horisontalBonus, verticalBonus, rainbowBonus, score, points, stars};
            StartCoroutine(Post(model, parameters));
        }

        IEnumerator Post(string model, string[] statistics)
        {
            WWWForm form = new WWWForm();
            form.AddField("entry.1961932892", model);
            form.AddField("entry.643027393", statistics[0]);
            form.AddField("entry.1143542895", statistics[1]);
            form.AddField("entry.2014368924", statistics[2]);
            form.AddField("entry.1676969702", statistics[3]);
            form.AddField("entry.1944122071", statistics[4]);
            form.AddField("entry.1070954891", statistics[5]);
            form.AddField("entry.2142674457", statistics[6]);
            form.AddField("entry.182534212", statistics[7]);
            form.AddField("entry.1573609905", statistics[8]);
            form.AddField("entry.1656864923", statistics[9]);
            form.AddField("entry.1031326177", statistics[10]);
            form.AddField("entry.1696604288", statistics[11]);
            form.AddField("entry.1520047453", statistics[12]);
            form.AddField("entry.2108335462", statistics[13]);

            UnityWebRequest www = UnityWebRequest.Post(URL, form);
            
            yield return www.SendWebRequest();

        }
    }
}