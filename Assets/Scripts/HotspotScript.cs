using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotspotScript : MonoBehaviour
{
    [SerializeField] public Vector2 realWorldPosition;
    [SerializeField] public GameObject hotspotImageObj;
    [SerializeField] public int hotspotID;

    [SerializeField] private GameObject textID;

    [SerializeField] private GameObject textID2;

    private bool isActive = false;
    private bool isHovering = false;

    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textID.GetComponent<TextMeshProUGUI>().text = hotspotID.ToString();
        textID2.GetComponent<TextMeshProUGUI>().text = hotspotID.ToString();
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHovering)
        {
            // Rotate
            transform.Rotate(Vector3.up * 100f * Time.deltaTime);

            // Float up/down
            float floatAmount = Mathf.Sin(Time.time * 5f) * 0.1f;
            transform.localPosition = startPosition + new Vector3(0f, floatAmount, 0f);
        }
    }

    public void StartHover()
    {
        isHovering = true;
    }

    public void StopHover()
    {
        isHovering = false;
        transform.localPosition = startPosition;
        transform.rotation = Quaternion.identity;
    }

    public void OnInteract()
    {
        hotspotImageObj.GetComponent<Image_UI_Manager>().PrepareOpen();
        hotspotImageObj.SetActive(true);
    }
}
