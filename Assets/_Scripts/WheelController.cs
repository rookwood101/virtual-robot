using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    private Vector2 desiredVelocity = Vector2.left;
    private WheelCollider leftWheel;
    private WheelCollider rightWheel;
    private new Rigidbody rigidbody;
    private PID anglePid = new PID(1, 2, 3);
    private PID speedPid = new PID(1, 2, 3);

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
        // Time.fixedDeltaTime
        // leftWheel.motorTorque
        // rightWheel.motorTorque
        // leftWheel.rpm
        // rightWheel.rpm
        // desiredVelocity = Vector3.forward
        // rigidbody.velocity
        
        // assuming starting at origin, what will be the new position after fixedDeltaTime
        // continuously calculates an error value e(t) as the difference between a 
        // desired setpoint r(t) and a measured process variable y(t)
        // e(t) = r(t) - y(t)
        // and applies a correction based on proportional, integral, and derivative terms
        // The controller attempts to minimize the error over time by adjustment of a control variable u(t)
        //  u(t) = Kp*e(t) + Ki*∫0,t e(τ) dτ + Kd*de(t)/dt

        var currentVelocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
        var angleError = Vector2.SignedAngle(currentVelocity, desiredVelocity);
        var angleCorrection = anglePid.GetOutput(angleError, Time.fixedDeltaTime);
        var speedError = currentVelocity.magnitude - desiredVelocity.magnitude;
        var speedCorrection = speedPid.GetOutput(speedError, Time.fixedDeltaTime);

        leftWheel.motorTorque = leftWheel.motorTorque + speedError + angleError;
        rightWheel.motorTorque = rightWheel.motorTorque + speedError - angleError;

        print($"angle {angleCorrection} = pid({angleError}, dt)");
        print($"speed {speedCorrection} = pid({speedError}, dt)");
    }
}
