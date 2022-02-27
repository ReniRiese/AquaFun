using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Image gameResultIcon;
    
    [SerializeField] Sprite successSprite;
    [SerializeField] Sprite failureSprite;

    public void SetResultIcon(bool success)
    {
        if (success)
        {
            gameResultIcon.sprite = successSprite;
        }
        else
        {
            gameResultIcon.sprite = failureSprite;
        }
    }
}
