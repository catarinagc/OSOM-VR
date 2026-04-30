using UnityEngine;
using TMPro;
public class HeighScript_RemoveLater : MonoBehaviour
{
    [SerializeField] TMP_Text value_text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHeightChangeText(float value)
    {
        value_text.text = value.ToString();
    }
}
