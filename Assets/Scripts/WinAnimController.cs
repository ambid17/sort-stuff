using System.Collections;
using UnityEngine;

public class WinAnimController : MonoBehaviour
{
    public GameObject boxPrefab;
    public GameObject truckPrefab;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    IEnumerator PlayWinAnimation()
    {
        // Instantiate the truck
        GameObject truck = Instantiate(truckPrefab, new Vector3(-10, 0, 0), Quaternion.identity);
        // Move the truck across the screen
        float truckSpeed = 5f;
        while (truck.transform.position.x < 10)
        {
            truck.transform.Translate(Vector3.right * truckSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(truck);
        // Instantiate boxes and make them bounce
        for (int i = 0; i < 5; i++)
        {
            GameObject box = Instantiate(boxPrefab, new Vector3(-5 + i * 2, 0, 0), Quaternion.identity);
            StartCoroutine(BounceBox(box));
            yield return new WaitForSeconds(0.5f);
        }
    }
}
