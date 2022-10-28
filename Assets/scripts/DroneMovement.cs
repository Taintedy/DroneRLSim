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
    public GameObject platform;
    public Vector3 velocity;
    public GameObject lowLim;
    public GameObject higLim;
    private Rigidbody _rb;
    ROSConnection ros;
    public float publishMessageFrequency = 200f;
    private float timeElapsed = 0f;
    private Vector3 prevPose;
    private Vector3 start_pose;
    bool landing = false;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        // transform.position = new Vector3(Random.Range(lowLim.transform.position.x, higLim.transform.position.x), Random.Range(lowLim.transform.position.y, higLim.transform.position.y), Random.Range(lowLim.transform.position.z, higLim.transform.position.z));
        start_pose = transform.position;
        prevPose = transform.position;
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<OdometryMsg>("/drone_state");
        ros.RegisterPublisher<BoolMsg>("/collision_detection");
        ROSConnection.GetOrCreateInstance().Subscribe<TwistMsg>("/cmd_vel", UpdateVelocity);
        ROSConnection.GetOrCreateInstance().Subscribe<BoolMsg>("/restart_request", Restart);
    }

    void FixedUpdate()
    {
        Debug.Log(platform.transform.position.x - transform.position.x);
        Debug.Log(platform.transform.position.y - transform.position.y);

        if (Mathf.Abs(platform.transform.position.x - transform.position.x) <= 0.3 && Mathf.Abs(platform.transform.position.z - transform.position.z) <= 0.3)
        {
            Debug.Log("landing");
            landing = true;
            velocity.x = 0;
            velocity.z = 0;
            velocity.y = -0.5f;
        }
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
        if (!landing){
        velocity.x = (float) vel.linear.x;
        velocity.y = (float) vel.linear.z;
        velocity.z = (float) vel.linear.y;}
    }

    void Restart(BoolMsg isRestart)
    {
        // transform.position = new Vector3(Random.Range(lowLim.transform.position.x, higLim.transform.position.x), Random.Range(lowLim.transform.position.y, higLim.transform.position.y), Random.Range(lowLim.transform.position.z, higLim.transform.position.z));
        transform.position = start_pose;
        prevPose = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        ros.Publish("/collision_detection", new BoolMsg(true));
    }
}
