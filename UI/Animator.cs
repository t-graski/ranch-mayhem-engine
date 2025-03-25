using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI;

// TODO: Find a way to animate all children of UIComponent, but not parents. 

public class Animator
{
    private readonly UIComponent _component;
    private readonly Dictionary<UIComponent, (float startX, float currentX, float endX)> _children = [];
    private readonly AnimationDirection _direction;
    private readonly float _animationSpeed;

    private float _startY;
    private float _currentY;
    private float _endY;

    private float _startX;
    private float _currentX;
    private float _endX;

    private bool _isAnimationDone = true;
    private bool _isAnimating = false;

    public Animator(UIComponent component, AnimationDirection direction, float animationSpeed = 0.1f)
    {
        _component = component;
        _animationSpeed = animationSpeed;
        _direction = direction;

        switch (_direction)
        {
            case AnimationDirection.Top:
                _startY = -component.Options.Size.Y;
                _currentY = _startY;
                _endY = component.GlobalPosition.Y;
                component.GlobalPosition = new Vector2(component.GlobalPosition.X, _startY);
                break;
            case AnimationDirection.Right:
                // _startX = component.GlobalPosition.X + 2000;
                // _currentX = _startX;
                // _endX = component.GlobalPosition.X;
                // component.GlobalPosition = new Vector2(_startX, component.GlobalPosition.Y);
                //
                // if (_component is Container container)
                // {
                //     var children = container.GetChildren();
                //
                //     foreach (var child in children)
                //     {
                //         var newPos = _startX + child.LocalPosition.X;
                //
                //         _children.Add(child, (newPos, newPos,
                //             _endX + child.LocalPosition.X));
                //
                //         child.GlobalPosition = new Vector2(newPos, child.GlobalPosition.Y);
                //     }
                // }

                break;
            default:
                Logger.Log($"{GetType().FullName}::ctor direction:{direction} is not valid.", LogLevel.Error);
                break;
        }
    }

    public void StartAnimation()
    {
        _isAnimationDone = false;
        _isAnimating = true;

        _component.IsAnimating = true;
        foreach (var (key, value) in _children)
        {
            key.IsAnimating = true;
        }
    }

    public void Reset()
    {
        if (!_isAnimationDone) return;

        switch (_direction)
        {
            case AnimationDirection.Top:
                _startY = -_component.Options.Size.Y;
                _currentY = _startY;
                _endY = _component.GlobalPosition.Y;
                _component.GlobalPosition = new Vector2(_component.GlobalPosition.X, _startY);
                break;
            case AnimationDirection.Right:
                _startX = _component.GlobalPosition.X + 2000;
                _currentX = _startX;
                _endX = _component.GlobalPosition.X;
                _component.GlobalPosition = new Vector2(_startX, _component.GlobalPosition.Y);

                if (_component is Container container)
                {
                    var children = container.GetChildren();
                    _children.Clear();

                    foreach (var child in children)
                    {
                        var newPos = _startX + child.LocalPosition.X;

                        _children.Add(child, (newPos, newPos,
                            _endX + child.LocalPosition.X));

                        child.GlobalPosition = new Vector2(newPos, child.GlobalPosition.Y);
                    }
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Update()
    {
        if (_isAnimating)
        {
            switch (_direction)
            {
                case AnimationDirection.Top:
                    _currentY = MathHelper.Lerp(_currentY, _endY, _animationSpeed);

                    if (Math.Abs(_currentY - _endY) < 1)
                    {
                        _currentY = _endY;
                        _isAnimating = false;
                        _isAnimationDone = true;
                    }

                    _component.GlobalPosition = new Vector2(_component.GlobalPosition.X, _currentY);
                    break;
                case AnimationDirection.Right:
                    _currentX = MathHelper.Lerp(_currentX, _endX, _animationSpeed);

                    if (Math.Abs(_currentX - _endX) < 1)
                    {
                        _currentX = _endX;
                        _isAnimating = false;
                        _isAnimationDone = true;

                        foreach (var (key, (startX, currentX, endX)) in _children)
                        {
                            _children[key] = (startX, endX, endX);
                            key.IsAnimating = false;
                        }
                    }

                    _component.GlobalPosition = new Vector2(_currentX, _component.GlobalPosition.Y);

                    foreach (var (key, (startX, currentX, endX)) in _children)
                    {
                        _children[key] = (startX, MathHelper.Lerp(currentX, endX, _animationSpeed), endX);
                        key.GlobalPosition = new Vector2(_children[key].currentX, key.GlobalPosition.Y);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // Logger.Log($"{GetType().FullName}::Update startY= {_startY}, currentY={_currentY}, endY={_endY}");
        }
    }
}