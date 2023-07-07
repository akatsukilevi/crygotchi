using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crygotchi;

public static class MathUtils
{
    public static float Rad2Deg = 360.0f / (Mathf.Pi * 2.0f);
    public static float Deg2Rad = (Mathf.Pi * 2.0f) / 360.0f;

    public static float DeltaAngle(float current, float target)
    {
        float delta = Mathf.PosMod((target - current), 360.0F);
        if (delta > 180.0F)
            delta -= 360.0F;
        return delta;
    }

    public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, double deltaTime)
    {
        target = current + DeltaAngle(current, target);
        return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, (float)deltaTime);
    }
    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
    {
        //Make sure we have at least some positive smoothing time, otherwise we will get weird results or DivideByZero exception.
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float num = 2f / smoothTime;
        float num2 = num * deltaTime;
        float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
        float num4 = current - target;
        float num5 = target;
        float num6 = maxSpeed * smoothTime;
        num4 = Mathf.Clamp(num4, -num6, num6);
        target = current - num4;
        float num7 = (currentVelocity + num * num4) * deltaTime;
        currentVelocity = (currentVelocity - num * num7) * num3;
        float num8 = target + (num4 + num7) * num3;
        if (num5 - current > 0f == num8 > num5)
        {
            num8 = num5;
            currentVelocity = (num8 - num5) / deltaTime;
        }
        return num8;
    }
}

