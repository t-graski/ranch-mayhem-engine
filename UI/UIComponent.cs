using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public abstract class UIComponent
{
    private UIComponent _parent;
    private string Id { get; }
    private readonly Texture2D _texture;
    private Vector2 _position;
    private Rectangle _bounds;
    private Vector2 _scale = Vector2.One;

    protected Action _onClick;
    protected Action _onHover;
    private bool _isHovered;
    private bool _isClicked;

    protected UIComponent(string id, Texture2D texture, Vector2 position)
    {
        Id = id;
        _texture = texture;
        _position = position;
        _bounds = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
    }

    protected UIComponent(string id, Texture2D texture, Vector2 position, Vector2 scale)
    {
        Id = id;
        _texture = texture;
        _position = position;
        _scale = scale;
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _position, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
    }

    public void HandleMouse(MouseState mouseState)
    {
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

    public abstract void Update();
}