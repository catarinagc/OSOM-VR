using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnClick()
    {
        Destroy(gameObject);
    }
}
