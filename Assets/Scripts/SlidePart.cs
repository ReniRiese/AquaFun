using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlidePart : MonoBehaviour
{
    [SerializeField] public List<Transform> path;
    
    public Transform GetPathStart()
    {
        return path[0];
    }
    
    public Transform GetPathEnd()
    {
        return path[path.Count-1];
    }

    public List<Vector3> GetPathPositions()
    {
        if (path.Count > 0)
        {
            return path.Select(t => t.position).ToList();
        }
                
        return new List<Vector3>();
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);;
        foreach (Vector3 pathVector in GetPathPositions())
        {
            
            Gizmos.DrawSphere(pathVector, 0.5f);
        }
    }
}