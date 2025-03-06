using System;
using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI;

// TODO: Find a way to animate all children of UIComponent, but not parents. 

public class Animator
{
    private readonly UIComponent _component;
    private readonly float _animationSpeed;

    private float _startY;
    private float _currentY;
    private float _endY;

    private bool _isAnimationDone = false;
    private bool _isAnimating = false;

    public Animator(UIComponent component, float animationSpeed = 0.1f)
    {
        _component = component;
        _animationSpeed = animationSpeed;

        _startY = -component.Options.Size.Y;
        _currentY = _startY;
        _endY = component.GlobalPosition.Y;

        component.GlobalPosition = new Vector2(component.GlobalPosition.X, _startY);
    }

    public void StartAnimation()
    {
        _isAnimationDone = false;
        _isAnimating = true;
    }

    public void Reset()
    {
        if (!_isAnimationDone) return;

        _startY = -_component.Options.Size.Y;
        _currentY = _startY;
        _endY = _component.GlobalPosition.Y;


        _component.GlobalPosition = new Vector2(_component.GlobalPosition.X, _startY);
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

            _component.GlobalPosition = new Vector2(_component.GlobalPosition.X, _currentY);
        }
    }
}