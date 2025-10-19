namespace ranch_mayhem_engine.UI;

public static class EasingFunctions
{
    public static float Linear(float t) => t;

    public static float EaseOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1f / d1)
        {
            return n1 * t * t;
        }

        if (t < 2f / d1)
        {
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        }

        if (t < 2.5f / d1)
        {
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        }

        return n1 * (t -= 2.625f / d1) * t + 0.984375f;
    }

    public static float EaseInBounce(float t)
    {
        return 1f - EaseOutBounce(1f - t);
    }

    public static float EaseInOutBounce(float t)
    {
        return t < 0.5f
            ? (1f - EaseOutBounce(1f - 2f * t)) / 2f
            : (1f + EaseOutBounce(2f * t - 1f)) / 2f;
    }

    public static float EaseInQuad(float t)
    {
        return t * t;
    }

    public static float ApplyEasing(float start, float end, float progress, Func<float, float> easingFunction)
    {
        var easedProgress = easingFunction(progress);
        return start + (end - start) * easedProgress;
    }
}