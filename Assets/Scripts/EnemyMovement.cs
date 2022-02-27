using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO Enemies should always be close to the player ... 

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Vector2 deviationChangeTimeRange;
    [SerializeField] private float dragControl = 1;

    private Movements _movements;
    private float _timer;
    private float _currentDeviationDuration;
    private float _targetDeviation;
    [SerializeField] private float marginFromEjection;
    private float _maxDeviation;

    void Start()
    {
        _movements = GetComponent<Movements>();
        _maxDeviation = _movements.ejectionThreshold - marginFromEjection;
    }
    
    void Update()
    {
        if (!Movements.Moving)
        {
            // dont waste resources
            return;
        }
        
        if (_timer > _currentDeviationDuration)
        {
            _currentDeviationDuration = Random.Range(deviationChangeTimeRange.x, deviationChangeTimeRange.y);

            _targetDeviation = Random.Range(-_maxDeviation, _maxDeviation);
            _timer = 0;
        }

        if (_movements.deviationModifAuthorization)
        {
            _movements.crossSectionPosOnSlide = Mathf.Lerp(_movements.crossSectionPosOnSlide, _targetDeviation, dragControl * Time.deltaTime);   
        }

        _timer += Time.deltaTime;
    }
}
