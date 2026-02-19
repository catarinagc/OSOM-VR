using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] GameObject breakwaterOrigin;
    [SerializeField] float modelScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //procurar todos os GameObjects com Tag "Hotspot"
        Vector3 realOriginPos = breakwaterOrigin.GetComponent<originPointScript>().realWorldPosition;
        GameObject[] hotspots = GameObject.FindGameObjectsWithTag("Hotspot");
        for (int i = 0; i < hotspots.Length; i++)
        {
            Vector3 realHotspotPos = hotspots[i].GetComponent<HotspotScript>().realWorldPosition;
            Vector3 ofsset = (realHotspotPos - realOriginPos) * modelScale;
            hotspots[i].transform.position = breakwaterOrigin.transform.position + ofsset;
        }

    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
