using UnityEngine;

public class PID : MonoBehaviour {

    public float kp, ki, kd = 0;
    public float P, I, D, prevError = 0;

    public void Init(float kp, float ki, float kd) {
        this.kp = kp;
        this.ki = ki;
        this.kd = kd;
    }

    public float GetOutput(float currentError, float deltaTime)
    {
        P = currentError;
        I += P * deltaTime;
        D = (P - prevError) / deltaTime;
        prevError = currentError;
        
        return P*kp + I*ki + D*kd;
    }
}
