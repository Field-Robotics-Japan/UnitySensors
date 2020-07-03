using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lidar : MonoBehaviour {

    public float maxAngle = 10;
    public float minAngle = -10;
    public int numberOfLayers = 16;
    public int numberOfIncrements = 360;
    public float maxRange = 100f;
    public Color GizmoPointColor = Color.green;
    public float GizmoPointSize = 0.01f;

    float vertIncrement;
    float azimutIncrAngle;

    [HideInInspector]
    public float[] distances;
    [HideInInspector]
    public float[] azimuts;
    private bool isInitialized = false;
    private Vector3[] hit_points;


    // Use this for initialization
    void Start () {
        distances = new float[numberOfLayers* numberOfIncrements];
        azimuts = new float[numberOfIncrements];
        vertIncrement = (float)(maxAngle - minAngle) / (float)(numberOfLayers - 1);
        azimutIncrAngle = (float)(360.0f / numberOfIncrements);
        hit_points = new Vector3[numberOfLayers*numberOfIncrements];
        isInitialized = true;
    }

// Update is called once per frame
    public void Scan () {
        Vector3 fwd = new Vector3(0, 0, 1);
        Vector3 dir;
        RaycastHit hit;
        int indx = 0;
        float angle;
        float distance = maxRange;

        //azimut angles
        for (int incr = 0; incr < numberOfIncrements; incr++)
        {
            for (int layer = 0; layer < numberOfLayers; layer++)
            {
                indx = layer + incr * numberOfLayers;
                angle = minAngle + (float)layer * vertIncrement;
                azimuts[incr] = incr * azimutIncrAngle;
                dir = transform.rotation * Quaternion.Euler(-angle, azimuts[incr], 0)*fwd;
                if (Physics.Raycast(transform.position, dir, out hit, maxRange))
                {
                    distance = hit.distance;
                    
                }
                distances[indx] = (float)hit.distance;
                hit_points[indx] = transform.position + dir*hit.distance;
            }
        }

    }

    public bool IsInitialized () {
        return isInitialized;
    }

    void OnDrawGizmos() 
	{
        if(hit_points != null && hit_points.Length != 0) {
            Gizmos.color = GizmoPointColor;
            foreach(Vector3 p in hit_points)
            {
                Gizmos.DrawSphere(p, GizmoPointSize);
            }
        }
	}
}
