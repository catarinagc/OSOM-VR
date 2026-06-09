using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class HotspotScript : MonoBehaviour
{
    [SerializeField] public Vector2 realWorldPosition;
    [SerializeField] public UI_Manager UI_Manager;
    [SerializeField] public int hotspotID;
    [SerializeField] public char troco_ID;
    private List<InspectionImage> images = new List<InspectionImage>();

    [SerializeField] private GameObject textID;

    [SerializeField] private GameObject textID2;
    [SerializeField] private GameObject mesh;
    [SerializeField] Material selectedMaterial;
    [SerializeField] Material unselectedMaterial;
    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;

    private bool isActive = false;
    private bool isHovering = false;

    public Vector3 startPosition;
    public Vector3 currentGlobalPosition;
    void Awake()
    {
        _renderer = mesh.GetComponent<Renderer>();
    }

    public void setTransparency(bool isSelected)
    {
        if (isSelected)
            _renderer.material = selectedMaterial;
        else
            _renderer.material = unselectedMaterial;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textID.GetComponent<TextMeshProUGUI>().text = hotspotID.ToString();
        textID2.GetComponent<TextMeshProUGUI>().text = hotspotID.ToString();
        startPosition = transform.localPosition;
        currentGlobalPosition = transform.position;
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
        UI_Manager.openHotspotImageUI(hotspotID, troco_ID, images);
        //Debug.Log("hello " + hotspotID + troco_ID);
    }

    public void AddImage(InspectionImage image)
    {
        //Debug.Log("Imagem");
        images.Add(image);
    }
}
