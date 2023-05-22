using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class AdsController : MonoBehaviour
    {
        public GameObject adsParent;
        public GameObject[] ads;
        private void Start() => HideAds();
        public void ShowAds()
        {
            adsParent.SetActive(true);
            ads[UnityEngine.Random.Range(0, ads.Length)].SetActive(true);
        }
        public void HideAds()
        {
            adsParent.SetActive(false);
            for(int i = 0; i < ads.Length; i++) ads[i].SetActive(false);
        }
    }
}
