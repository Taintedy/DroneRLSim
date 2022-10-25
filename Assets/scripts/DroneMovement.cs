using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Nav;
using RosMessageTypes.Std;
using UnityEngine.SceneManagement;

public class DroneMovement : MonoBehaviour
{
    public Vector3 velocity;
    public GameObject lowLim;
    public GameObject higLim;
    private Rigidbody _rb;
    ROSConnection ros;
    public float publishMessageFrequency = 100f;
    private float timeElapsed = 0f;
    private Vector3 prevPose;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        transform.position = new Vector3(Random.Range(lowLim.transform.position.x, higLim.transform.position.x), Random.Range(lowLim.transform.position.y, higLim.transform.position.y), Random.Range(lowLim.transform.position.z, higLim.transform.position.z));
        prevPose = transform.position;
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<OdometryMsg>("/drone_state");
        ros.RegisterPublisher<BoolMsg>("/collision_detection");
        ROSConnection.GetOrCreateInstance().Subscribe<TwistMsg>("/cmd_vel", UpdateVelocity);
        ROSConnection.GetOrCreateInstance().Subscribe<BoolMsg>("/restart_request", Restart);
    }

    void FixedUpdate()
    {
        _rb.velocity = velocity;
        timeElapsed += Time.fixedDeltaTime;
        if (timeElapsed > 1/publishMessageFrequency)
        {
            OdometryMsg droneState = new OdometryMsg();

            Vector3 currentPose = transform.position;
            Vector3 currentVelocity = (currentPose - prevPose) / Time.fixedDeltaTime;

            droneState.pose.pose.position.x = currentPose.x;
            droneState.pose.pose.position.z = currentPose.y;
            droneState.pose.pose.position.y = currentPose.z;

            droneState.twist.twist.linear.x = currentVelocity.x;
            droneState.twist.twist.linear.z = currentVelocity.y;
            droneState.twist.twist.linear.y = currentVelocity.z;

            // Finally send the message to server_endpoint.py running in ROS
            ros.Publish("/drone_state", droneState);
            prevPose = currentPose;
            timeElapsed = 0;
        }
    }

    void UpdateVelocity(TwistMsg vel)
    {
        velocity.x = (float) vel.linear.x;
        velocity.y = (float) vel.linear.z;
        velocity.z = (float) vel.linear.y;
    }

    void Restart(BoolMsg isRestart)
    {
        transform.position = new Vector3(Random.Range(lowLim.transform.position.x, higLim.transform.position.x), Random.Range(lowLim.transform.position.y, higLim.transform.position.y), Random.Range(lowLim.transform.position.z, higLim.transform.position.z));
        prevPose = transform.position;
    }

    void OnCollisionStay(Collision collision)
    {
        ros.Publish("/collision_detection", new BoolMsg(true));
    }
}
