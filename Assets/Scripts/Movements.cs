using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Movements : MonoBehaviour
{
    public enum MovementCharacter {Player, Enemy};

    public MovementCharacter movementCharacter;
    
    //Path follow vars
    private List<Vector3> totalPath;
    [NonSerialized] public int actualWaypoint = 0;
    private int nextWaypoint => actualWaypoint + 1;
    [HideInInspector] public Vector3 posOnPath;
    
    // every character is Moving at the same time (or not -> static)
    public static bool Moving; 

    // as seen in a cross-section: the players position on the slide
    // 
    // (-1)___           ___(1)
    //        \___(0)___/
    
    [NonSerialized] public float crossSectionPosOnSlide;
    
    [NonSerialized] public bool deviationModifAuthorization = true;
    [NonSerialized] public float absDeviationAcceleration;
    [NonSerialized] public float speedMultiplicator = 1;
    [NonSerialized] public bool touchesSlide = true;

    [SerializeField] private float speed = 1;
    [SerializeField] private float waterBoost = 4;
    [SerializeField] private float moveHardness = 1;
    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] private float slideWidth = 2;
    [SerializeField] private float offsetFromGround;
    [SerializeField] private LayerMask slideLayerMask;
    
    private float _lastDeviation;
    
    public float ejectionThreshold;
    [SerializeField] private float ejectionForce;
    [SerializeField] private float flyingSpeed = 1;
    [SerializeField] private float fallSpeed = 1;
    
    [SerializeField] private float obstacleSpeedMultiplicator = 0.5f;
    [SerializeField] private float obstacleReductionTime = 1;
    
    private Animator _animator;
    private Player _player;
    private GameHandler _gameHandler;
    private static readonly int PoolJump = Animator.StringToHash("poolJump");

    void Start()
    {
        _player = GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _gameHandler = FindObjectOfType<GameHandler>();
        totalPath = SlideGenerator.TotalPath;
    }
    
    void Update()
    {
        RaycastHit hit;
        
        // currently on the way, set position on path
        if (touchesSlide && nextWaypoint < totalPath.Count)
        {
            Vector3 betweenTwoWaypoints = totalPath[nextWaypoint] - totalPath[actualWaypoint];
            
            if (Moving)
            {
                //                               ^ posOnPath
                //                            °  | 
                //                         °     |
                //                      °        |
                //                   °           |
                //   betweenTwoWP |------>-------X--------->Projection
                //
                
                // if the players projection on the path is longer, than the segment betweenTwoWaypoints, he already
                // passed the next waypoint
                
                Vector3 playerProjection = Vector3.Project(posOnPath - totalPath[actualWaypoint], betweenTwoWaypoints);
                
                if (playerProjection.magnitude < betweenTwoWaypoints.magnitude)
                {
                    // in the middle of the slide the player is a little faster
                    float waterBoostMultiplier = 1f - Math.Abs(crossSectionPosOnSlide) / waterBoost; 
                    
                    posOnPath += speed * speedMultiplicator * waterBoostMultiplier * Time.deltaTime * betweenTwoWaypoints.normalized;
                }
                else
                {
                    actualWaypoint++;

                    if (actualWaypoint == totalPath.Count - 1)
                    {
                        JumpInPool();
                    }
                }
            }
            
            // always look forward, rotate at given rotationSpeed
            transform.forward = Vector3.Lerp(transform.forward, betweenTwoWaypoints, Time.deltaTime * rotationSpeed);

            if (Physics.Raycast(posOnPath + Vector3.up + transform.right * crossSectionPosOnSlide * slideWidth / 2f, Vector3.down, out hit, 10,
                slideLayerMask ))
            {
                // place character on slide according to raycast
                transform.position = Vector3.Slerp(transform.position, hit.point + hit.normal * offsetFromGround, Time.deltaTime * moveHardness);
            }

            // if position on slide (from -1 to 1) is bigger than threshold -> start flying!
            if (Mathf.Abs(crossSectionPosOnSlide) > ejectionThreshold)
            {
                StartFlying();
            }
            
        }
        else if(!touchesSlide) //The character is flying
        {
            transform.position += Time.deltaTime * flyingSpeed * transform.forward - transform.up * fallSpeed;

            // if raycast downward of player hits slide again, place on slide, turn to look in the direction of the next waypoint
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, slideLayerMask))
            {
               StopFlying();
            }
        }
        
        //The deviation acceleration is computed here, its used for collisions between characters
        absDeviationAcceleration = Mathf.Abs(_lastDeviation - crossSectionPosOnSlide);
        _lastDeviation = crossSectionPosOnSlide;
    }

    void JumpInPool()
    {
        //Final jump animation setup
        Transform characterHolder = new GameObject().transform;
        characterHolder.parent = transform.parent;
        characterHolder.position = transform.position;
        Vector3 scale = characterHolder.localScale;
        scale.z *= Random.Range(1f, 1.2f);
        characterHolder.localScale = scale;
        characterHolder.localEulerAngles = Vector3.up * (transform.eulerAngles.y + Random.Range(-5f,5f));
            
        transform.parent = characterHolder.transform;
        enabled = false;
        
        _animator.SetTrigger(PoolJump);
        
        if (_player)
        {
            GameHandler.CurrentPlayerStatus = GameHandler.PlayerStatus.ReachedPool;
            _gameHandler.EndGame();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            other.gameObject.SetActive(false);
            StartCoroutine(MomentarilyChangeSpeed(obstacleSpeedMultiplicator, obstacleReductionTime));
        }
    }
    
    private void StartFlying()
    {
        touchesSlide = false;
        //X rotation axis reset
        Vector3 newRot = transform.eulerAngles;
        newRot.x = 0;
        transform.eulerAngles = newRot;
        
        //Bumping out
        transform.position += ((Mathf.Sign(crossSectionPosOnSlide) * transform.right + Vector3.up) * ejectionForce);
        
        GameHandler.CurrentPlayerStatus = GameHandler.PlayerStatus.Flying;
    }
    
    private void StopFlying()
    {
        // find waypoint that is nearest to the player
        List<Vector3> nearestWaypoints = totalPath.OrderBy(p => Vector3.Distance(transform.position, p)).ToList();
        if (nearestWaypoints.Count > 0)
        {
            int nearestId = totalPath.IndexOf(nearestWaypoints[0]);
            actualWaypoint = nearestId;
            posOnPath = totalPath[actualWaypoint];
            touchesSlide = true;
            
            GameHandler.CurrentPlayerStatus = GameHandler.PlayerStatus.Sliding;
        }
        
        // TODO: what happens in else? 
    }
    
    public void ApplyTempDeviation(float modification, float duration) { StartCoroutine(TempDeviation(modification, duration)); }
    private IEnumerator TempDeviation(float modification, float duration)
    {
        deviationModifAuthorization = false;

        float timer = 0;

        //Using a while is usually not safe, but the lines here are full safe
        while (timer < duration)
        {
            float delta = Time.fixedDeltaTime * (modification / duration);
            crossSectionPosOnSlide += delta;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        deviationModifAuthorization = true;
    }

    public void ChangeSpeed(float multiplicator, float duration)
    {
        StartCoroutine(MomentarilyChangeSpeed(multiplicator, duration));
    }
    private IEnumerator MomentarilyChangeSpeed(float multiplicator, float duration)
    {
        speedMultiplicator += (multiplicator - 1f);
        
        yield return new WaitForSeconds(duration);

        speedMultiplicator -= (multiplicator -1f);
    }
    private void OnDrawGizmos()
    {
        switch (movementCharacter)
        {
            case MovementCharacter.Player:
                Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                break;
            default:
            case MovementCharacter.Enemy:
                Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                break;
        }

        Gizmos.DrawWireSphere(posOnPath, 0.5f);
    }
    
}