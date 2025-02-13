using System;
using System.Net.Http.Headers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public abstract class UIComponent
{
    protected UIComponent Parent { get; set; }
    protected string Id { get; private set; }
    // protected Texture2D _texture;

    // protected readonly Color _color;
    // public Vector2 Size { get; private set; }

    protected Vector2 _localPosition;
    protected Vector2 _globalPosition;
    // protected UIAnchor _uiAnchor;

    private Rectangle _bounds;
    // private Vector2 _scale = Vector2.One;

    public UIComponentOptions _options { get; protected set; } = new();

    protected Action _onClick;
    protected Action _onHover;
    protected Action _offClick;
    protected Action _offHover;

    private bool _isHovered;
    private bool _isClicked;

    protected UIComponent(string id, UIComponentOptions options, UIComponent parent = null)
    {
        Id = id;
        Parent = parent;
        ApplyOptions(options);
    }

    private void ApplyOptions(UIComponentOptions options)
    {
        if (options.Position == Vector2.Zero && options.Size != Vector2.Zero)
        {
            _options.Size = ScaleToGlobal(options.Size);
            _localPosition = options.UiAnchor.CalculatePosition(_options.Size, Parent);
            UpdateBounds(Parent);
        }

        _options.Color = options.Color;
    }

    // protected UIComponent(string id, Color color, Vector2 position, Vector2 size, UIComponent parent = null)
    // {
    //     Id = id;
    //     _color = color;
    //
    //     _localPosition = ScaleToGlobal(position);
    //     Size = ScaleToGlobal(size);
    //     UpdateBounds(parent);
    // }
    //
    // protected UIComponent(string id, Color color, UIAnchor uiAnchor, Vector2 size, UIComponent parent = null)
    // {
    //     Id = id;
    //     _color = color;
    //     Parent = parent;
    //     _uiAnchor = uiAnchor;
    //
    //     Size = ScaleToGlobal(size);
    //     _localPosition = uiAnchor.CalculatePosition(Size, parent);
    //     UpdateBounds(parent);
    // }
    //
    // protected UIComponent(string id, Texture2D texture, Vector2 position, Vector2 size, UIComponent parent = null)
    // {
    //     Id = id;
    //     _texture = texture;
    //     _localPosition = ScaleToGlobal(position);
    //
    //     var scaleX = size.X / texture.Width;
    //     var scaleY = size.Y / texture.Height;
    //     var scale = new Vector2(scaleX, scaleY);
    //
    //     _scale = ScaleToGlobal(scale);
    //     Size = ScaleToGlobal(size);
    //     UpdateBounds(parent);
    // }
    //
    // protected UIComponent(string id, Texture2D texture, UIAnchor uiAnchor, Vector2 size, UIComponent parent = null)
    // {
    //     Id = id;
    //     _texture = texture;
    //     _uiAnchor = uiAnchor;
    //
    //     var scaleX = size.X / texture.Width;
    //     var scaleY = size.Y / texture.Height;
    //     var scale = new Vector2(scaleX, scaleY);
    //     _scale = ScaleToGlobal(scale);
    //
    //     Size = ScaleToGlobal(size);
    //     _localPosition = uiAnchor.CalculatePosition(Size);
    //     UpdateBounds(parent);
    //
    //     Console.WriteLine($"{_localPosition}");
    // }
    //
    // protected UIComponent(string id, Texture2D texture, Vector2 position, float scale, UIComponent parent = null)
    // {
    //     Id = id;
    //     _texture = texture;
    //     _localPosition = ScaleToGlobal(position);
    //     _scale = new Vector2(scale);
    //     UpdateBounds(parent);
    // }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_options.Texture, _localPosition, null, Color.White, 0f, Vector2.Zero,
            _options.Scale, SpriteEffects.None, 0f);
    }

    public void HandleMouse(MouseState mouseState)
    {
        if (!RanchMayhemEngine.IsFocused) return;

        if (mouseState.LeftButton == ButtonState.Pressed &&
            _bounds.Contains(mouseState.Position))
        {
            if (!_isClicked)
            {
                _onClick?.Invoke();
                _isClicked = !_isClicked;
            }
        }

        if (_isClicked && mouseState.LeftButton == ButtonState.Released)
        {
            _offClick?.Invoke();
            _isClicked = !_isClicked;
        }

        if (mouseState is { LeftButton: ButtonState.Released, RightButton: ButtonState.Released } &&
            _bounds.Contains(mouseState.Position))
        {
            if (!_isHovered)
            {
                _onHover?.Invoke();
                _isHovered = !_isHovered;
            }
        }

        if (_isHovered)
        {
            if (_bounds.Contains(mouseState.Position)) return;

            _offHover?.Invoke();
            _isHovered = !_isHovered;
        }
    }

    private static Vector2 ScaleToGlobal(Vector2 position)
    {
        // Console.WriteLine($"scaling from {position} to {position * RanchMayhemEngine.UIManager.GlobalScale}");
        return position * RanchMayhemEngine.UIManager.GlobalScale;
    }

    public void SetParent(UIComponent parent)
    {
        Parent = parent;
        UpdateGlobalPosition();
    }

    private Vector2 CalculateGlobalPosition()
    {
        if (Parent == null) return _localPosition;
        return Parent.CalculateGlobalPosition() + _localPosition;
    }

    private void UpdateGlobalPosition()
    {
        if (Parent == null)
        {
            _globalPosition = _localPosition;
        }
        else
        {
            if (_options.UiAnchor != UIAnchor.None)
            {
                _localPosition = _options.UiAnchor.CalculatePosition(_options.Size, Parent);
            }

            UpdateBounds(Parent);
        }
    }

    private void UpdateBounds(UIComponent parent)
    {
        if (parent == null)
        {
            _bounds = new Rectangle((int)_localPosition.X, (int)_localPosition.Y, (int)_options.Size.X,
                (int)_options.Size.Y);
            _globalPosition = _localPosition;
        }
        else
        {
            _globalPosition = CalculateGlobalPosition();

            _bounds = new Rectangle((int)_globalPosition.X, (int)_globalPosition.Y, (int)_options.Size.X,
                (int)_options.Size.Y);
        }
    }

    public abstract void Update();
}