using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    private Vector3 power = 10000 * Vector3.up;
    [SerializeField]
    private float desiredSpeed = 1;
    private WheelCollider leftWheel;
    private WheelCollider rightWheel;
    private new Rigidbody rigidbody;

    void Start()
    {
        var left = this.transform.Find("Wheels/Left Wheel") ?? throw new NotFoundException("left wheel not found");
        var right = this.transform.Find("Wheels/Right Wheel") ?? throw new NotFoundException("right wheel not found");
        this.leftWheel = left.GetComponent<WheelCollider>() ?? throw new NotFoundException("left wheel wheelcollider not found");
        this.rightWheel = right.GetComponent<WheelCollider>() ?? throw new NotFoundException("right wheel wheelcollider not found");
        this.rigidbody = GetComponent<Rigidbody>();
        leftWheel.motorTorque = 0;
        rightWheel.motorTorque = 0;
    }

    void FixedUpdate()
    {
        var newTorque = 10 * (rigidbody.velocity.magnitude - desiredSpeed);
        leftWheel.motorTorque = newTorque;
        rightWheel.motorTorque = newTorque;
    }
}
