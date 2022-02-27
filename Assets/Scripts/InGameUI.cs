using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image progressionBar;
    [SerializeField] private TextMeshProUGUI positionText;
    
    [SerializeField] private GameObject menuItems;
    [SerializeField] private GameObject gameFinished;
    
    [SerializeField] private TextMeshProUGUI gameResultText;
    [SerializeField] private CollectibleUI collectibleUI;
    
    [SerializeField] private QuestUI finishRaceQuestUI;
    [SerializeField] private QuestUI swimRingQuestUI;
    
    private bool gameEnded;
    Movements playerMovements;
    Movements[] allMovements;
    private List<float> leaderboard;
    
    private void Start()
    {
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;

        startButton.onClick.AddListener(StartButton);
        restartButton.onClick.AddListener(RestartButton);
        
        Initialize();
    }

    public void Initialize()
    {
        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
        gameFinished.SetActive(false);
        menuItems.SetActive(true);

        playerMovements = FindObjectOfType<Player>().GetComponent<Movements>();
        allMovements = FindObjectsOfType<Movements>();
    }

    private void Update()
    {
        if (gameEnded) return;

        progressionBar.fillAmount = (float)playerMovements.actualWaypoint / SlideGenerator.TotalPath.Count;
        leaderboard = allMovements.Select(m => m.posOnPath.y).ToList();
        leaderboard.Sort();

        int position = leaderboard.IndexOf(playerMovements.posOnPath.y) + 1;

        string suffix;
        
        switch(position % 10)
        {
            case 1:
                suffix = "st";
                break;
            case 2:
                suffix = "nd";
                break;
            case 3:
                suffix = "rd";
                break;
            default:
                suffix = "th";
                break;
        }
        
        positionText.text = position + suffix;
    }
    
    public void End(bool reachedPool)
    {
        gameEnded = true;
        restartButton.gameObject.SetActive(true);
        
        gameFinished.SetActive(true);
        
        if (reachedPool)
        {
            gameResultText.text = "You finished the race!";
            finishRaceQuestUI.SetResultIcon(true);
        }
        else
        {
            // TODO: Todesanimation?
            gameResultText.text = "Try again...";
            finishRaceQuestUI.SetResultIcon(false);
        }
        
        swimRingQuestUI.SetResultIcon(PlayerCollectedAllItems());
    }
    
    public bool PlayerCollectedAllItems()
    {
        return collectibleUI.IsCollected();
    }
    
    private void StartButton()
    {
        Movements.Moving = true;
        startButton.gameObject.SetActive(false);
        menuItems.SetActive(false);
        FindObjectOfType<Player>().InitializeCamera();
    }
    
    private void RestartButton()
    {
        SceneManager.LoadScene(0);
    }
    
    
}
