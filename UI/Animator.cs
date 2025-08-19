using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI;

// TODO: Find a way to animate all children of UIComponent, but not parents.

public class Animator
{
    private readonly UiComponent _component;

    // private readonly Dictionary<UiComponent, (float startX, float currentX, float endX)> _children = [];
    private readonly AnimationDirection _direction;
    private readonly float _animationSpeed;
    private readonly AnimationType _animationType;
    private readonly Func<float, float> _easingFunction;

    private float _startY;
    private float _currentY;
    private float _endY;

    private float _startX;
    private float _currentX;
    private float _endX;

    public bool IsAnimationDone { get; private set; } = true;
    private bool _isAnimating = false;
    private float _animationProgress = 0f;


    public event Action? OnAnimationComplete;

    public Animator(
        UiComponent component, AnimationDirection direction, float animationSpeed = 0.1f, AnimationType animationType = AnimationType.Linear
    )
    {
        _component = component;
        _animationSpeed = animationSpeed;
        _direction = direction;
        _animationType = animationType;

        _easingFunction = animationType switch
        {
            AnimationType.EaseOutBounce => EasingFunctions.EaseOutBounce,
            AnimationType.EaseInBounce => EasingFunctions.EaseInBounce,
            AnimationType.EaseInOutBounce => EasingFunctions.EaseInOutBounce,
            _ => EasingFunctions.Linear
        };
    }

    public void StartAnimation()
    {
        IsAnimationDone = false;
        _isAnimating = true;
        _animationProgress = 0f;
        _component.ToggleAnimating();
    }

    public void Reset()
    {
        if (!IsAnimationDone) return;

        _animationProgress = 0f;

        switch (_direction)
        {
            case AnimationDirection.TopIn:
                _startY = -_component.Options.Size.Y;
                _currentY = _startY;
                _endY = _component.GlobalPosition.Y;
                _component.SetGlobalPosition(new Vector2(_component.GlobalPosition.X, _startY));
                break;
            case AnimationDirection.TopOut:
                _startY = _component.GlobalPosition.Y;
                _currentY = _startY;
                _endY = -_component.Options.Size.Y;
                break;
            case AnimationDirection.Right:
                _startX = _component.GlobalPosition.X + 2000;
                _currentX = _startX;
                _endX = _component.GlobalPosition.X;
                _component.SetGlobalPosition(new Vector2(_startX, _component.GlobalPosition.Y));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Update()
    {
        if (_isAnimating)
        {
            // Use progress-based animation for easing functions
            if (_animationType != AnimationType.Linear)
            {
                _animationProgress += _animationSpeed;

                if (_animationProgress >= 1.0f)
                {
                    _animationProgress = 1.0f;
                    AnimationDone();
                    _component.ToggleAnimating();
                }

                switch (_direction)
                {
                    case AnimationDirection.TopIn:
                    case AnimationDirection.TopOut:
                        _currentY = EasingFunctions.ApplyEasing(_startY, _endY, _animationProgress, _easingFunction);
                        _component.SetGlobalPosition(new Vector2(_component.GlobalPosition.X, _currentY));
                        break;
                    case AnimationDirection.Right:
                        _currentX = EasingFunctions.ApplyEasing(_startX, _endX, _animationProgress, _easingFunction);
                        _component.SetGlobalPosition(new Vector2(_currentX, _component.GlobalPosition.Y));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                switch (_direction)
                {
                    case AnimationDirection.TopIn:
                    case AnimationDirection.TopOut:
                        _currentY = MathHelper.Lerp(_currentY, _endY, _animationSpeed);

                        if (Math.Abs(_currentY - _endY) < 1)
                        {
                            _currentY = _startY;
                            AnimationDone();
                            _component.ToggleAnimating();
                        }

                        _component.SetGlobalPosition(new Vector2(_component.GlobalPosition.X, _currentY));
                        break;
                    case AnimationDirection.Right:
                        _currentX = MathHelper.Lerp(_currentX, _endX, _animationSpeed);

                        if (Math.Abs(_currentX - _endX) < 1)
                        {
                            _currentX = _endX;
                            AnimationDone();
                            _component.ToggleAnimating();

                            // foreach (var (key, (startX, _, endX)) in _children)
                            // {
                            //     _children[key] = (startX, endX, endX);
                            //     key.IsAnimating = false;
                            //
                            //     if (key is Box box)
                            //     {
                            //         if (box.Text is not null)
                            //         {
                            //             box.Text.IsAnimating = false;
                            //         }
                            //     }
                            // }
                        }

                        _component.SetGlobalPosition(new Vector2(_currentX, _component.GlobalPosition.Y));

                        // foreach (var (key, (startX, currentX, endX)) in _children)
                        // {
                        //     _children[key] = (startX, MathHelper.Lerp(currentX, endX, _animationSpeed), endX);
                        //     key.SetGlobalPosition(new Vector2(_children[key].currentX, key.GlobalPosition.Y));
                        //
                        //     if (key is Box box)
                        //     {
                        //         if (box.Text is not null)
                        //         {
                        //             box.Text.SetGlobalPosition(
                        //                 new Vector2(
                        //                     _children[key].currentX + box.Text.LocalPosition.X,
                        //                     key.GlobalPosition.Y + box.Text.LocalPosition.Y
                        //                 )
                        //             );
                        //         }
                        //     }
                        // }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // Logger.Log($"{GetType().FullName}::Update startY= {_startY}, currentY={_currentY}, endY={_endY}");
        }
    }


    private void AnimationDone()
    {
        _isAnimating = false;
        IsAnimationDone = true;
        OnAnimationComplete?.Invoke();
    }
}
