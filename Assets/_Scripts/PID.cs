public class PID {

    private float kp, ki, kd;
    private float P, I, D, prevError = 0;

    public PID(float kp, float ki, float kd) {
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
