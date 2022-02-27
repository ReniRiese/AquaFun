using System;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleUI : MonoBehaviour
{ 
    [SerializeField] Sprite collectedSprite;

    [SerializeField] private bool isCollected;

    public void Start()
    {
        isCollected = false;
    }

    public void CollectItem()
    {
        isCollected = true;
        GetComponent<Image>().sprite = collectedSprite;
    }

    public bool IsCollected()
    {
        return isCollected;
    }
}
