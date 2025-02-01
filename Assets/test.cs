using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    void Start()
    {
        ResizeLog();
    }

    void ResizeLog()
    {
        RectTransform logTransform = GetComponent<RectTransform>();
        RectTransform[] componentTransform = GetComponentsInChildren<RectTransform>();

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (RectTransform child in componentTransform)
        {
            if (child == logTransform) continue;

            Vector3[] corners = new Vector3[4];
            child.GetWorldCorners(corners);

            foreach (Vector3 corner in corners)
            {
                minX = Mathf.Min(minX, corner.x);
                minY = Mathf.Min(minY, corner.y);
                maxX = Mathf.Max(maxX, corner.x);
                maxY = Mathf.Max(maxY, corner.y);
            }
        }

        Vector2 newSize = new Vector2(maxX - minX, maxY - minY);
        logTransform.sizeDelta = newSize;
    }
}