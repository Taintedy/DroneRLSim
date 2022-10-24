using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;
public class PlatformMovement : MonoBehaviour
{
    public Vector3[] sinParams;
    public float MoveSpeed;

    public float dispersionX;
    public float dispersionY;
    public float dispersionZ;
    private float Timer;
    private int CurrentNode;
    private List<float[]> _poses;
    private Vector3 startPosition;
    private Vector3 CurrentPositionHolder;
    private Vector3 startRotation;
    private Vector3 CurrentRotationHolder;
    // Start is called before the first frame update
    void Start()
    {
        _poses = createPath(sinParams);
        CurrentNode = 0;
        CheckNode ();

    }

    // Update is called once per frame
    void Update()
    {
        _poses = createPath(sinParams);
        Timer += Time.deltaTime * MoveSpeed;

        float epsPose = (transform.localPosition - CurrentPositionHolder).magnitude;
        float epsRot = (transform.localEulerAngles - CurrentRotationHolder).magnitude;

        if (epsPose >= 0.01) 
        {
            transform.localPosition = Vector3.Lerp(startPosition, CurrentPositionHolder, Timer);
        }
        else
        {
            if(CurrentNode < _poses.Count -1)
            {
            CurrentNode++;
            CheckNode ();
            }
            else
            {
            CurrentNode = 0;
            CheckNode ();
            }
        }
    }

    void CheckNode()
    {
        Timer = 0;
        startPosition = transform.localPosition;
        float randomX = (float)getRandom(0, dispersionX);
        float randomY = (float)getRandom(0, dispersionY);
        float randomZ = (float)getRandom(0, dispersionZ);
        CurrentPositionHolder = new Vector3(_poses[CurrentNode][0] + randomX, _poses[CurrentNode][1] + randomY, _poses[CurrentNode][2] + randomZ);
        startRotation = transform.localEulerAngles;
        CurrentRotationHolder = new Vector3(_poses[CurrentNode][3], _poses[CurrentNode][4], _poses[CurrentNode][5]);
    }

    List<float[]> createPath(Vector3[] sinParams)
    {
        List<float[]> path = new List<float[]>();
        
        for(int i = 0; i <= 360; i++)
        {
            float[] pose = new float[6];
            pose[0] = sinParams[0].x * Mathf.Sin(((sinParams[0].y * 3.14f / 180f) * i + sinParams[0].z));
            pose[1] = Mathf.Abs(sinParams[1].x * Mathf.Sin(((sinParams[1].y * 3.14f / 180f) * i + sinParams[1].z)));
            pose[2] = sinParams[2].x * Mathf.Sin(((sinParams[2].y * 3.14f / 180f) * i + sinParams[2].z));
            pose[3] = sinParams[3].x * Mathf.Sin(((sinParams[3].y * 3.14f / 180f) * i + sinParams[3].z));
            pose[4] = sinParams[4].x * Mathf.Sin(((sinParams[4].y * 3.14f / 180f) * i + sinParams[4].z));
            pose[5] = sinParams[5].x * Mathf.Sin(((sinParams[5].y * 3.14f / 180f) * i + sinParams[5].z));
            path.Add(pose);
        }
        // foreach (var point in path)
        // {
        //     Debug.Log(point[0] + " " +point[1] + " " +point[2] + " " +point[3] + " " +point[4] + " " +point[5] + " " );
        // }
        return path;
    }

    double getRandom(double mean, double stdDev)
    {
        Random rand = new Random(); //reuse this if you are generating many
        double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0-rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                               Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal =
            mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

        return randNormal;
    }

}
