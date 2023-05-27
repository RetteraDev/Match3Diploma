using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;

namespace Match3
{
    public class AnalyticsCore : MonoBehaviour
    {
        async void Awake()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (ConsentCheckException e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
}