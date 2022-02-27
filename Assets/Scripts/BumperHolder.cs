using UnityEngine;
using System.Collections;
 
public class BumperHolder : MonoBehaviour {
   
    public enum BumperType
    {
        LeftBumper,
        RightBumper,
        BackBumper
    }
     
    [SerializeField] public BumperType type;
    [SerializeField] public Movements movementsComponent;
    
    [SerializeField] public float backBumpSpeedMultiplicator = 2;
    [SerializeField] public float backBumpDuration  = 0.25f;
    
    [SerializeField] public float sideBumpTime = 0.1f;
    [SerializeField] public float bumpDeviation = 0.7f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Movements>() != null)
        {
            switch (type)
            {
                case BumperType.BackBumper:
                    movementsComponent.ChangeSpeed(backBumpSpeedMultiplicator, backBumpDuration);
                    break;
                
                case BumperType.LeftBumper:
                    SideBumper(other.GetComponent<Movements>());
                    break;
                
                case BumperType.RightBumper:
                    SideBumper(other.GetComponent<Movements>());
                    break;
            }
        }
    }

    // TODO: handle kills?
    void SideBumper(Movements other)
    {
        if (other.absDeviationAcceleration > movementsComponent.absDeviationAcceleration)
        {
            // get killed
            movementsComponent.ApplyTempDeviation((type == BumperType.LeftBumper ? -1f : 1f) * bumpDeviation, sideBumpTime);
        }
        else
        {
            // kill other ...   
        }
    }
}
