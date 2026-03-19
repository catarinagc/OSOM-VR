using UnityEngine;

public class ScrollViewResize : MonoBehaviour
{
    [SerializeField] RectTransform imageContent;
    private RectTransform rectTransform;
    private float initialWidth;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialWidth = rectTransform.sizeDelta.x;
    }

    void Update()
    {
        float childWidth = imageContent.sizeDelta.x;
        Debug.Log(childWidth);
        Debug.Log(rectTransform.sizeDelta.x);
        if (childWidth + 20.0f < rectTransform.sizeDelta.x)
        {
            rectTransform.sizeDelta = new Vector2(childWidth, rectTransform.sizeDelta.y + 20.0f);
        }
        else if (childWidth > rectTransform.sizeDelta.x)
        {
            rectTransform.sizeDelta = new Vector2(initialWidth, rectTransform.sizeDelta.y);
        }
    }
}