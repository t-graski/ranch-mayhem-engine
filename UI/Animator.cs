using System;
using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI;

public class Animator
{
    public UIComponent Component;
    private readonly float _animationSpeed;

    private float _startY;
    private float _currentY;
    private float _endY;

    private bool _isAnimationDone = false;
    private bool _isAnimating = false;

    public Animator(UIComponent component, float animationSpeed = 0.1f)
    {
        Component = component;
        _animationSpeed = animationSpeed;

        _startY = -component.Options.Size.Y;
        _currentY = _startY;
        _endY = component.GlobalPosition.Y;

        component.GlobalPosition = new Vector2(component.GlobalPosition.X, _startY);
    }

    public void StartAnimation()
    {
        _isAnimating = true;
    }

    public void Update()
    {
        if (_isAnimating)
        {
            // Logger.Log($"{GetType().FullName}::Update startY= {_startY}, currentY={_currentY}, endY={_endY}");
            _currentY = MathHelper.Lerp(_currentY, _endY, _animationSpeed);

            if (Math.Abs(_currentY - _endY) < 1)
            {
                _currentY = _endY;
                _isAnimating = false;
                _isAnimationDone = true;
            }

            Component.GlobalPosition = new Vector2(Component.GlobalPosition.X, _currentY);
        }
    }
}