using UnityEngine;

public class CollapsableUI : MonoBehaviour
{
    [SerializeField] GameObject collapse_UI;
    [SerializeField] GameObject sprite;

    public void OnClick()
    {
        sprite.transform.Rotate(0, 0, 180);
        if (collapse_UI.active)
            collapse_UI.SetActive(false);
        else
            collapse_UI.SetActive(true);
    }
}
