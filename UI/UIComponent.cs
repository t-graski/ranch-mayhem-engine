using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace ranch_mayhem_engine.UI;

public abstract class UIComponent
{
    protected UIComponent Parent { get; set; }
    public string Id { get; private set; }
    public Vector2 LocalPosition;
    public Vector2 GlobalPosition;
    private Rectangle _bounds;

    public UIComponentOptions Options { get; protected set; } = new();

    protected Action OnClick;
    protected Action OnHover;
    protected Action OffClick;
    protected Action OffHover;

    private bool _isHovered;
    private bool _isClicked;

    protected UIComponent(string id, UIComponentOptions options, UIComponent parent = null, bool scale = true)
    {
        Id = id;
        Parent = parent;
        if (scale)
        {
            ApplyOptions(options);
        }
        else
        {
            Options = options;
            LocalPosition = options.Position;
            GlobalPosition = CalculateGlobalPosition();
            UpdateBounds(parent);
        }
    }

    private void ApplyOptions(UIComponentOptions options)
    {
        Options.Size = ScaleToGlobal(options.Size);
        Options.SizeUnit = options.SizeUnit;
        Options.SizePercent = options.SizePercent;
        Options.UiAnchor = options.UiAnchor;

        if (options.Position == Vector2.Zero && options.Size != Vector2.Zero)
        {
            Options.UiAnchor = options.UiAnchor;
            LocalPosition = options.UiAnchor.CalculatePosition(Options.Size, new Vector2(-1), Parent);
            UpdateBounds(Parent);
        }

        if (options.Position != Vector2.Zero)
        {
            LocalPosition = ScaleToGlobal(options.Position);
            UpdateBounds(Parent);
        }

        if (options.Texture != null)
        {
            if (options.Size == Vector2.Zero)
            {
                options.Size.X = options.Texture.Width;
                options.Size.Y = options.Texture.Height;
            }

            var scaleX = options.Size.X / options.Texture.Width;
            var scaleY = options.Size.Y / options.Texture.Height;
            var scale = new Vector2(scaleX, scaleY);
            Options.Scale = ScaleToGlobal(scale);
            Options.Texture = options.Texture;
        }
        else
        {
            Options.Color = options.Color;
        }
    }

    public void RecalculateSize(Vector2 size, Vector2? virtualParent)
    {
        if (Options.SizeUnit == SizeUnit.Pixels)
        {
            Options.Size = size;
        }
        else if (Options.SizeUnit == SizeUnit.Percent)
        {
            var viewport = RanchMayhemEngine.UIManager.GraphicsDevice.Viewport;
            var width = virtualParent?.X ?? viewport.Width;
            var height = virtualParent?.Y ?? viewport.Height;
            var newSize = Vector2.Zero;

            Console.WriteLine($"percent unit: parentWidth: {width} parentHeight: {height}");
            Console.WriteLine($"percent unit: optionsPercent: {Options.SizePercent}");

            if (Options.SizePercent.X != 0 && Options.SizePercent.Y != 0)
            {
                Console.WriteLine("1");
                newSize = new Vector2(width * (Options.SizePercent.X / 100), height * (Options.SizePercent.Y / 100));
            }
            else if (Options.SizePercent.Y == 0)
            {
                Console.WriteLine("2");
                newSize = new Vector2(width * (Options.SizePercent.X / 100));
            }
            else if (Options.SizePercent.X == 0)
            {
                Console.WriteLine("3");
                newSize = new Vector2(height * (Options.SizePercent.Y / 100));
            }

            Console.WriteLine($"newSize: {newSize}");

            Options.Size = newSize;
        }
    }

    public void UpdatePosition(Vector2 position, Vector2 size, UIComponent parent, Vector2? virtualParent)
    {
        LocalPosition = position;
        GlobalPosition = CalculateGlobalPosition();
        Console.WriteLine($"{Id} local: {LocalPosition} global: {GlobalPosition}");

        if (Options.SizeUnit == SizeUnit.Pixels)
        {
            Options.Size = size;
        }
        else if (Options.SizeUnit == SizeUnit.Percent)
        {
            var viewport = RanchMayhemEngine.UIManager.GraphicsDevice.Viewport;
            var width = virtualParent?.X ?? viewport.Width;
            var height = virtualParent?.Y ?? viewport.Height;
            var newSize = Vector2.Zero;

            Console.WriteLine($"percent unit: parentWidth: {width} parentHeight: {height}");
            Console.WriteLine($"percent unit: optionsPercent: {Options.SizePercent}");

            if (Options.SizePercent.X != 0 && Options.SizePercent.Y != 0)
            {
                Console.WriteLine("1");
                newSize = new Vector2(width * (Options.SizePercent.X / 100), height * (Options.SizePercent.Y / 100));
            }
            else if (Options.SizePercent.Y == 0)
            {
                Console.WriteLine("2");
                newSize = new Vector2(width * (Options.SizePercent.X / 100));
            }
            else if (Options.SizePercent.X == 0)
            {
                Console.WriteLine("3");
                newSize = new Vector2(height * (Options.SizePercent.Y / 100));
            }

            Console.WriteLine($"newSize: {newSize}");

            Options.Size = newSize;
        }

        if (Options.Texture != null)
        {
            var scaleX = Options.Size.X / Options.Texture?.Width;
            var scaleY = Options.Size.Y / Options.Texture?.Height;
            var scale = new Vector2(scaleX ?? 1, scaleY ?? 1);

            Options.Scale = scale;
        }


        UpdateBounds(parent);
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
        spriteBatch.Draw(Options.Texture, GlobalPosition, null, Color.White, 0f, Vector2.Zero,
            Options.Scale, SpriteEffects.None, 0f);
    }

    public void HandleMouse(MouseState mouseState)
    {
        if (!RanchMayhemEngine.IsFocused) return;

        if (mouseState.LeftButton == ButtonState.Pressed &&
            _bounds.Contains(mouseState.Position))
        {
            if (!_isClicked)
            {
                OnClick?.Invoke();
                _isClicked = !_isClicked;
            }
        }

        if (_isClicked && mouseState.LeftButton == ButtonState.Released)
        {
            OffClick?.Invoke();
            _isClicked = !_isClicked;
        }

        if (mouseState is { LeftButton: ButtonState.Released, RightButton: ButtonState.Released } &&
            _bounds.Contains(mouseState.Position))
        {
            if (!_isHovered)
            {
                OnHover?.Invoke();
                _isHovered = !_isHovered;
            }
        }

        if (_isHovered)
        {
            if (_bounds.Contains(mouseState.Position)) return;

            OffHover?.Invoke();
            _isHovered = !_isHovered;
        }
    }

    protected static Vector2 ScaleToGlobal(Vector2 position)
    {
        return position * RanchMayhemEngine.UIManager.GlobalScale;
    }

    protected static Vector4 ScaleToGlobal(Vector4 position)
    {
        var globalScale = RanchMayhemEngine.UIManager.GlobalScale;
        return new Vector4(position.X * globalScale.Y, position.Y * globalScale.X, position.Z * globalScale.Y,
            position.W * globalScale.X);
    }

    public void SetParent(UIComponent parent)
    {
        Parent = parent;
        UpdateGlobalPosition();
    }

    private Vector2 CalculateGlobalPosition()
    {
        if (Parent == null) return LocalPosition;
        return Parent.CalculateGlobalPosition() + LocalPosition;
    }

    private void UpdateGlobalPosition()
    {
        if (Parent == null)
        {
            GlobalPosition = LocalPosition;
        }
        else
        {
            if (Options.UiAnchor != UIAnchor.None)
            {
                LocalPosition = Options.UiAnchor.CalculatePosition(Options.Size, new Vector2(-1), Parent);
            }

            UpdateBounds(Parent);
        }
    }

    private void UpdateBounds(UIComponent parent)
    {
        if (parent == null)
        {
            _bounds = new Rectangle((int)LocalPosition.X, (int)LocalPosition.Y, (int)Options.Size.X,
                (int)Options.Size.Y);
            GlobalPosition = LocalPosition;
        }
        else
        {
            GlobalPosition = CalculateGlobalPosition();

            _bounds = new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, (int)Options.Size.X,
                (int)Options.Size.Y);
        }
    }

    public abstract void Update();
}