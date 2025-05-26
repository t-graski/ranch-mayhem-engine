using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI;

public class NumberAnimator(float startNumber, float endNumber, float animationSpeed = 0.1f)
{
    private readonly float _animationSpeed = animationSpeed;
    private readonly float _startNumber = startNumber;
    private float _currentNumber = startNumber;
    private readonly float _endNumber = endNumber;

    private bool _isAnimationDone = true;
    private bool _isAnimating = false;

    public void StartAnimation()
    {
        _isAnimationDone = false;
        _isAnimating = true;
    }

    public void FinishAnimation()
    {
        _isAnimating = false;
        _isAnimationDone = true;
        _currentNumber = _endNumber;
    }

    public void Update()
    {
        _currentNumber = MathHelper.Lerp(_currentNumber, _endNumber, _animationSpeed);

        if (Math.Abs(_currentNumber - _endNumber) < 1)
        {
            FinishAnimation();
        }
    }

    public float GetCurrent() => _currentNumber;
}
