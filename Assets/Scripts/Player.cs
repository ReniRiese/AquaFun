using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float releaseHardness = 1;
    [SerializeField] private float dragControl = 1;
    [SerializeField] private Transform cameraPlaceHolder;
    [SerializeField] private float flyingRotationControl;
    
    private Movements _movements;
    private GameHandler _gameHandler;

    private void Start()
    {
        _gameHandler = FindObjectOfType<GameHandler>();
        _movements = GetComponent<Movements>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Terrain")) 
        {
            _movements.enabled = false;
            GameHandler.CurrentPlayerStatus = GameHandler.PlayerStatus.Crashed;
            _gameHandler.EndGame();
        }
        
        if (other.transform.CompareTag("Pool"))
        {
            _movements.enabled = false;
            GameHandler.CurrentPlayerStatus = GameHandler.PlayerStatus.ReachedPool;
            _gameHandler.EndGame();
        }
    }
    
    public void InitializeCamera()
    {
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        cameraFollow.target = transform;
        cameraFollow.placeHolder = cameraPlaceHolder;
    }
    
    void Update()
    {
        
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            float mouseX = InputManager.MouseX;
            if (_movements.touchesSlide && _movements.deviationModifAuthorization)
            {
                _movements.crossSectionPosOnSlide = Mathf.Clamp(_movements.crossSectionPosOnSlide + mouseX * dragControl, -1f, 1f);
            }
            else if(!_movements.touchesSlide)
            {
                transform.RotateAround(transform.position, transform.up, flyingRotationControl * mouseX * Time.deltaTime);
            }        
        }
        else if (_movements.deviationModifAuthorization)
        {
            // no input => return to middle of slide
            _movements.crossSectionPosOnSlide = Mathf.Lerp(_movements.crossSectionPosOnSlide, 0, Time.deltaTime * releaseHardness);
        }
    }
}