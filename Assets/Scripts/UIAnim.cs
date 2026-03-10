using UnityEngine;

public class UIAnim : MonoBehaviour
{
    public Animator panelAnimator;

    public void Open()
    {
        panelAnimator.SetTrigger("Open");
    }

    public void Close()
    {
        panelAnimator.SetTrigger("Close");
    }

}
