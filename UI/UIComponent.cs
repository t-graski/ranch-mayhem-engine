using System;
using System.Net.Http.Headers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public abstract class UIComponent
{
    private UIComponent _parent;
    protected string Id { get; }
    private readonly Texture2D _texture;

    protected readonly Color _color;
    // protected readonly Vector2 _size;

    protected Vector2 _position;
    protected UIAnchor _uiAnchor;

    private Rectangle _bounds;
    private Vector2 _scale = Vector2.One;

    protected Action _onClick;
    protected Action _onHover;
    private bool _isHovered;
    private bool _isClicked;

    protected UIComponent(string id, Color color, Vector2 position, Vector2 size)
    {
        Id = id;
        _color = color;

        Console.WriteLine($"original pos: {position}");
        _position = ScaleToGlobal(position);
        Console.WriteLine($"adjusted pos: {_position}");
        size = ScaleToGlobal(size);
        _bounds = new Rectangle((int)_position.X, (int)_position.Y, (int)size.X, (int)size.Y);
    }

    protected UIComponent(string id, Color color, UIAnchor uiAnchor, Vector2 size)
    {
        Id = id;
        _color = color;

        _position = ScaleToGlobal(uiAnchor.CalculatePosition(size));
        size = ScaleToGlobal(size);

        _bounds = new Rectangle((int)_position.X, (int)_position.Y, (int)size.X, (int)size.Y);
    }

    protected UIComponent(string id, Texture2D texture, Vector2 position, Vector2 size)
    {
        Id = id;
        _texture = texture;
        _position = ScaleToGlobal(position);

        // 448 / 900 = 0.5 | 450 / 600 = 0.75
        // 900 / 448 = 2   | 600 / 450 = 1.333

        // 450 / 900 = 0.5
        // -> < 1 = +1
        // -> > 1 = -1

        var scaleX = size.X / texture.Width;
        var scaleY = size.Y / texture.Height;
        var scale = new Vector2(scaleX, scaleY);

        _scale = ScaleToGlobal(scale);
        Console.WriteLine($"original size: {size}");
        size = ScaleToGlobal(size);
        Console.WriteLine($"adjusted size: {size}");
        _bounds = new Rectangle((int)_position.X, (int)_position.Y, (int)size.X, (int)size.Y);

        Console.WriteLine($"{GetType().FullName}::ctor");
    }

    protected UIComponent(string id, Texture2D texture, UIAnchor uiAnchor, Vector2 size)
    {
        Id = id;
        _texture = texture;
        _uiAnchor = uiAnchor;

        _position = ScaleToGlobal(uiAnchor.CalculatePosition(size));
        _bounds = new Rectangle((int)_position.X, (int)_position.Y, (int)size.X, (int)size.Y);
    }

    protected UIComponent(string id, Texture2D texture, Vector2 position, float scale)
    {
        Id = id;
        _texture = texture;
        _position = ScaleToGlobal(position);
        _scale = new Vector2(scale);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        var globalScale = RanchMayhemEngine.UIManager.GlobalScale;
        spriteBatch.Draw(_texture, _position, null, Color.White, 0f, Vector2.Zero,
            _scale, SpriteEffects.None, 0f);
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

            _isHovered = !_isHovered;
        }
    }

    private static Vector2 ScaleToGlobal(Vector2 position)
    {
        return position * RanchMayhemEngine.UIManager.GlobalScale;
    }

    public abstract void Update();
}