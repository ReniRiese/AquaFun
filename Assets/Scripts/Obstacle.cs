using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float obstacleDensity;

    private void Start()
    {
        transform.GetChild(Random.Range(0, transform.childCount)).gameObject.SetActive(false);
        
        foreach (Transform obstacle in transform)
        {
            if (Random.Range(0f, 1f) < obstacleDensity)
            {
                obstacle.gameObject.SetActive(false);
            }
            
        }
    }
}
