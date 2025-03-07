﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public abstract class UIComponent
{
    protected UIComponent Parent { get; private set; }
    public string Id { get; }
    protected Vector2 LocalPosition;
    public Vector2 GlobalPosition { get; set; }
    protected Rectangle Bounds;

    public UIComponentOptions Options { get; set; } = new();

    public Action OnClick;
    protected Action OnHover;
    protected Action OffClick;
    protected Action OffHover;

    private bool _isHovered;
    protected bool IsClicked;

    protected UIComponent(string id, UIComponentOptions options, UIComponent parent = null, bool scale = true)
    {
        Id = id;
        Parent = parent;

#if DEBUG
        ParseOptions(options);
#endif

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

    private void ParseOptions(UIComponentOptions options)
    {
        var prefix = $"{GetType().FullName}::ctor Id={Id}";

        if (options.SizeUnit == SizeUnit.Pixels && options.Size == Vector2.Zero && this is not Text)
        {
            Logger.Log($"{prefix} SizeUnit is set to Pixels and Size is {Vector2.Zero}.", Logger.LogLevel.Warning);
        }

        if (options.SizeUnit == SizeUnit.Percent && options.SizePercent == Vector2.Zero)
        {
            if (options.Size == Vector2.Zero)
            {
                Logger.Log($"{prefix} SizeUnit is set to Percent and SizePercent is {Vector2.Zero}",
                    Logger.LogLevel.Warning);
            }
            else
            {
                Logger.Log($"{prefix} SizeUnit is set to Percent but 'Size' is being used instead of 'SizePercent'",
                    Logger.LogLevel.Warning);
            }
        }

        if (options.Scale.X < 0 || options.Scale.Y < 0)
        {
            Logger.Log($"{prefix} Scale is negative ({options.Scale})", Logger.LogLevel.Warning);
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

    public void RecalculateSize(Vector2 size, Vector2? virtualParentSize)
    {
        if (Options.SizeUnit == SizeUnit.Pixels)
        {
            Options.Size = size;
        }
        else if (Options.SizeUnit == SizeUnit.Percent)
        {
            var viewport = RanchMayhemEngine.UIManager.GraphicsDevice.Viewport;
            var width = virtualParentSize?.X ?? viewport.Width;
            var height = virtualParentSize?.Y ?? viewport.Height;
            var newSize = Vector2.Zero;

            if (Options.SizePercent.X != 0 && Options.SizePercent.Y != 0)
            {
                newSize = new Vector2(width * (Options.SizePercent.X / 100), height * (Options.SizePercent.Y / 100));
            }
            else if (Options.SizePercent.Y == 0)
            {
                newSize = new Vector2(width * (Options.SizePercent.X / 100));
            }
            else if (Options.SizePercent.X == 0)
            {
                newSize = new Vector2(height * (Options.SizePercent.Y / 100));
            }

            Options.Size = newSize;
        }
    }

    public void UpdatePosition(Vector2 position, Vector2 size, UIComponent parent, Vector2? virtualParent)
    {
        LocalPosition = position;
        GlobalPosition = CalculateGlobalPosition();

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

            if (Options.SizePercent.X != 0 && Options.SizePercent.Y != 0)
            {
                newSize = new Vector2(width * (Options.SizePercent.X / 100), height * (Options.SizePercent.Y / 100));
            }
            else if (Options.SizePercent.Y == 0)
            {
                newSize = new Vector2(width * (Options.SizePercent.X / 100));
            }
            else if (Options.SizePercent.X == 0)
            {
                newSize = new Vector2(height * (Options.SizePercent.Y / 100));
            }

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

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Options.Texture != null)
        {
            spriteBatch.Draw(Options.Texture, GlobalPosition, null, Color.White, 0f, Vector2.Zero,
                Options.Scale, SpriteEffects.None, 0f);
        }
        else
        {
            // Logger.Log($"{GetType().FullName}::Draw Id={Id} Drawing at {GlobalPosition}");
            var texture = new Texture2D(RanchMayhemEngine.UIManager.GraphicsDevice, 1, 1);
            texture.SetData([Options.Color]);
            if (Parent is null)
            {
                spriteBatch.Draw(texture,
                    new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, (int)Options.Size.X,
                        (int)Options.Size.Y), Options.Color);
            }
            else
            {
                spriteBatch.Draw(texture,
                    new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, (int)Options.Size.X,
                        (int)Options.Size.Y), Options.Color);
            }
        }
    }

    public void HandleMouse(MouseState mouseState)
    {
        if (!RanchMayhemEngine.IsFocused) return;

        if (mouseState.LeftButton == ButtonState.Pressed &&
            Bounds.Contains(mouseState.Position))
        {
            if (!IsClicked)
            {
                OnClick?.Invoke();
                IsClicked = !IsClicked;
            }
        }

        if (IsClicked && mouseState.LeftButton == ButtonState.Released)
        {
            OffClick?.Invoke();
            IsClicked = !IsClicked;
        }

        if (mouseState is { LeftButton: ButtonState.Released, RightButton: ButtonState.Released } &&
            Bounds.Contains(mouseState.Position))
        {
            if (!_isHovered)
            {
                OnHover?.Invoke();
                _isHovered = !_isHovered;
            }
        }

        if (_isHovered)
        {
            if (Bounds.Contains(mouseState.Position)) return;

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
                GlobalPosition = CalculateGlobalPosition();
            }

            UpdateBounds(Parent);
        }
    }

    private void UpdateBounds(UIComponent parent)
    {
        if (parent == null)
        {
            Bounds = new Rectangle((int)LocalPosition.X, (int)LocalPosition.Y, (int)Options.Size.X,
                (int)Options.Size.Y);
            GlobalPosition = LocalPosition;
        }
        else
        {
            Bounds = new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, (int)Options.Size.X,
                (int)Options.Size.Y);
        }
    }

    public abstract void Update();
}