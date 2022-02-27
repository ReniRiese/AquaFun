using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlideGenerator : MonoBehaviour
{
    [SerializeField] private Transform finishWayPoint;
    [SerializeField] private List<Transform> slidePrefabs = new List<Transform>();
    [SerializeField] private Transform slideStartPrefab;
    [SerializeField] private int length = 10;

    [SerializeField] private Transform playerPrefab;
    [SerializeField] private string cameraLobbyPlaceHolderGameObjectName = "CameraLobbyPlaceHolder";
    
    [SerializeField] private int numberOfEnemies;
    [SerializeField] private Transform enemyPrefab;
    
    private float spawnPosPercent; //percent of the start toboggan
    
    [NonSerialized] public static List<Vector3> TotalPath = new List<Vector3>();

    public void Start()
    {
        TotalPath.Clear();
        Movements.Moving = false;
        FindObjectOfType<CameraFollow>().target = null;
        GenerateSlide(finishWayPoint);
        
        SpawnCharacters();
    }

    void GenerateSlide(Transform lastWayPoint)
    {
        Transform lastSlideModuleAdded = lastWayPoint;
        for (int i = 0; i < length; i++)
        {
            Transform slidePrefab = slidePrefabs[Random.Range(0, slidePrefabs.Count)];
            
            if (i == length - 1)
            {
                // the last piece is the start prefab
                slidePrefab = slideStartPrefab;
            }
            
            lastSlideModuleAdded = InstantiateSlideModule(lastSlideModuleAdded, slidePrefab);
        }
   
        //Path is cleaned from duplicates
        TotalPath = TotalPath.Distinct().ToList();
    }

    Transform InstantiateSlideModule(Transform lastWayPoint, Transform slidePrefab)
    {
        SlidePart lastSlideModule = lastWayPoint.GetComponent<SlidePart>();
        Transform newSlideModule = Instantiate(slidePrefab, lastSlideModule.GetPathStart().position, Quaternion.identity, transform);
        
        if (lastSlideModule.GetPathStart())
        {
            RotateNewSlideModuleToMatchLastModule(newSlideModule, lastSlideModule.GetPathStart());
        }
        
        //Add lastWayPoint's path to total path
        TotalPath.InsertRange(0, newSlideModule.GetComponent<SlidePart>().GetPathPositions());
        
        return newSlideModule;
    }

    private static void RotateNewSlideModuleToMatchLastModule(Transform newSlideModule, Transform lastSlideModule)
    {
        newSlideModule.localEulerAngles += lastSlideModule.eulerAngles.y * Vector3.up;
    }


    void SpawnCharacters()
    {
        SpawnOneCharacter(playerPrefab);
        for (int i = 0; i < numberOfEnemies; i++)
            SpawnOneCharacter(enemyPrefab);

        Transform camHolder = GameObject.Find(cameraLobbyPlaceHolderGameObjectName).transform;
        FindObjectOfType<Camera>().transform.SetPositionAndRotation(camHolder.position, camHolder.rotation);
    }

    void SpawnOneCharacter(Transform prefab)
    {
        Vector3 spawnPos = TotalPath[0] + (TotalPath[1] - TotalPath[0]) * spawnPosPercent;
        Transform character = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
        spawnPosPercent += 1f / (numberOfEnemies + 1); //+1 is because of the player
        character.GetComponent<Movements>().posOnPath = spawnPos;
    }
}