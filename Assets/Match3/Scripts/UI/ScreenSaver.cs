using UnityEngine;

public class ScreenSaver : MonoBehaviour
{
    public GameObject[] screen;
    void Awake()
    {
        if (screen[0] != null) BodySafeArea();
        if (screen[1] != null) TopSafeArea();
        if (screen[2] != null) BottomSafeAre();
    }

    void BodySafeArea()
    {
        var safeArea = Screen.safeArea;
        var myRectTransform = screen[0].GetComponent<RectTransform>();
        
        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        myRectTransform.anchorMin = anchorMin;
        myRectTransform.anchorMax = anchorMax;
    }
    void TopSafeArea()
    {
        var safeArea = Screen.safeArea;
        var myRectTransform = screen[1].GetComponent<RectTransform>();

        var anchorMin = safeArea.position + safeArea.size;
        var anchorMax = new Vector2 (1, 1);

        anchorMin.x = 0;
        anchorMin.y /= Screen.height;

        myRectTransform.anchorMin = anchorMin;
        myRectTransform.anchorMax = anchorMax;
    }
    void BottomSafeAre()
    {
        var safeArea = Screen.safeArea;
        var myRectTransform = screen[2].GetComponent<RectTransform>();

        var anchorMin = new Vector2 (0, 0);
        var anchorMax = safeArea.position;

        anchorMax.x = 1;
        anchorMax.y /= Screen.height;
        myRectTransform.anchorMin = anchorMin;
        myRectTransform.anchorMax = anchorMax;
    }
}
