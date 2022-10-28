using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
public class RosPosePublisher : MonoBehaviour
{
    public string topic = "/target_pose";
    ROSConnection ros;
    public float publishMessageFrequency = 100f;
    private float timeElapsed = 0f;

    public GameObject lowLim;
    public GameObject higLim;
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseMsg>(topic);
        // transform.position = new Vector3(Random.Range(lowLim.transform.position.x, higLim.transform.position.x), Random.Range(lowLim.transform.position.y, higLim.transform.position.y), Random.Range(lowLim.transform.position.z, higLim.transform.position.z));
        ROSConnection.GetOrCreateInstance().Subscribe<BoolMsg>("/restart_request", Restart);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.fixedDeltaTime;
        if (timeElapsed > 1/publishMessageFrequency)
        {
            PoseMsg targetPose = new PoseMsg();

            targetPose.position.x = transform.position.x;
            targetPose.position.y = transform.position.z;
            targetPose.position.z = transform.position.y;

            // Finally send the message to server_endpoint.py running in ROS
            ros.Publish(topic, targetPose);
            timeElapsed = 0;
        }
    }

    void Restart(BoolMsg isRestart)
    {
        // transform.position = new Vector3(Random.Range(lowLim.transform.position.x, higLim.transform.position.x), Random.Range(lowLim.transform.position.y, higLim.transform.position.y), Random.Range(lowLim.transform.position.z, higLim.transform.position.z));
    }
}
