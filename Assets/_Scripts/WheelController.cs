using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    private Vector2 desiredVelocity = Vector2.left;
    private WheelCollider leftWheel;
    private WheelCollider rightWheel;
    private new Rigidbody rigidbody;
    private PID anglePid;
    private PID speedPid;

    private float startTime;

    void Start()
    {
        var left = this.transform.Find("Wheels/Left Wheel") ?? throw new NotFoundException("left wheel not found");
        var right = this.transform.Find("Wheels/Right Wheel") ?? throw new NotFoundException("right wheel not found");
        this.leftWheel = left.GetComponent<WheelCollider>() ?? throw new NotFoundException("left wheel wheelcollider not found");
        this.rightWheel = right.GetComponent<WheelCollider>() ?? throw new NotFoundException("right wheel wheelcollider not found");
        this.rigidbody = GetComponent<Rigidbody>();
        leftWheel.motorTorque = 0;
        rightWheel.motorTorque = 0;
        anglePid = new GameObject("anglePid " + Guid.NewGuid().ToString(), new Type[] {typeof(PID)}).GetComponent<PID>();
        speedPid = new GameObject("speedPid " + Guid.NewGuid().ToString(), new Type[] {typeof(PID)}).GetComponent<PID>();
        speedPid.Init(1f, 0.0f, 0.1f);
        anglePid.Init(0.01f, 0f, 0f);
        startTime = Time.unscaledTime;
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
        // var angleError = Mathf.InverseLerp(0, 180, Vector2.SignedAngle(currentVelocity, desiredVelocity));
        var angleError = Vector2.SignedAngle(currentVelocity, desiredVelocity);
        var angleCorrection = anglePid.GetOutput(angleError, Time.fixedDeltaTime);
        var speedError = desiredVelocity.magnitude - currentVelocity.magnitude;
        var speedCorrection = speedPid.GetOutput(speedError, Time.fixedDeltaTime);

        leftWheel.motorTorque = Mathf.Clamp(leftWheel.motorTorque + speedCorrection + angleCorrection, -20, 20);
        rightWheel.motorTorque = Mathf.Clamp(rightWheel.motorTorque + speedCorrection - angleCorrection, -20, 20);

        print($"speed {speedCorrection} = pid({speedError}, dt) {Time.unscaledTime - startTime}");
        print($"angle {angleCorrection} = pid({angleError}, dt) {Time.unscaledTime - startTime}");

        ApplyLocalPositionToVisuals(leftWheel);
        ApplyLocalPositionToVisuals(rightWheel);
    }

    public void ApplyLocalPositionToVisuals(WheelCollider wheel)
    {
        if (wheel.transform.childCount == 0) {
            return;
        }
     
        Transform visualWheel = wheel.transform.GetChild(0);
     
        Vector3 position;
        Quaternion rotation;
        wheel.GetWorldPose(out position, out rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
}
