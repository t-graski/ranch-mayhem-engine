using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public abstract class UiComponent
{
    public UiComponent? Parent { get; set; }
    public bool IsVisible { get; set; } = true;
    public string Id { get; }
    public Vector2 LocalPosition { get; private set; }
    public Vector2 GlobalPosition { get; set; }
    private Rectangle _bounds;
    public UiComponentOptions Options { get; set; } = new();
    private UiComponent? HoverItem { get; set; }
    private bool _hasBorder;
    public bool IsAnimating;

    public Action? OnClick;
    protected Action? OnHover;
    protected Action? OffHover;

    protected bool IsHovered;
    protected bool IsClicked;
    protected bool IsActive;

    public bool CanTriggerClick;

    protected UiComponent(string id, UiComponentOptions options, UiComponent? parent = null, bool scale = true)
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

    private void ParseOptions(UiComponentOptions options)
    {
        var prefix = $"{GetType().FullName}::ctor Id={Id}";

        if (options.SizeUnit == SizeUnit.Pixels && options.Size == Vector2.Zero && this is not Text or Container)
        {
            // Logger.Log($"{prefix} SizeUnit is set to Pixels and Size is {Vector2.Zero}.", Logger.LogLevel.Warning);
        }

        if (options.SizeUnit == SizeUnit.Percent && options.SizePercent == Vector2.Zero)
        {
            if (options.Size == Vector2.Zero)
            {
                Logger.Log($"{prefix} SizeUnit is set to Percent and SizePercent is {Vector2.Zero}",
                    LogLevel.Warning);
            }
            else
            {
                Logger.Log($"{prefix} SizeUnit is set to Percent but 'Size' is being used instead of 'SizePercent'",
                    LogLevel.Warning);
            }
        }

        if (options.Scale.X < 0 || options.Scale.Y < 0)
        {
            Logger.Log($"{prefix} Scale is negative ({options.Scale})", LogLevel.Warning);
        }

        // TODO: check for border options to be either all null or none null.
    }

    protected void ApplyOptions(UiComponentOptions options)
    {
        Options.Size = ScaleToGlobal(options.Size);
        Options.SizeUnit = options.SizeUnit;
        Options.SizePercent = options.SizePercent;
        Options.UiAnchor = options.UiAnchor;
        Options.UiAnchorOffset = options.UiAnchorOffset;

        if (options.BorderSize != 0)
        {
            Options.BorderColor = options.BorderColor;
            Options.BorderSize = options.BorderSize;
            Options.BorderOrientation = options.BorderOrientation;
            Options.BorderTexture = options.BorderTexture;
            Options.BorderCornerTexture = options.BorderCornerTexture ?? Options.BorderTexture;
            Options.BorderPosition = options.BorderPosition;

            _hasBorder = true;
        }

        if (options.Position == Vector2.Zero && options.Size != Vector2.Zero)
        {
            Options.UiAnchor = options.UiAnchor;
            LocalPosition = options.UiAnchor.CalculatePosition(Options.Size, new Vector2(-1), Parent);

            if (options.UiAnchorOffset != Vector2.Zero)
            {
                LocalPosition += options.UiAnchorOffset;
            }

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
            Options.TextureOverlay = options.TextureOverlay;
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
            var viewport = RanchMayhemEngine.UiManager.GraphicsDevice.Viewport;
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

            CalculateScale();
        }
    }

    public void UpdatePosition(Vector2 position, Vector2 size, UiComponent? parent, Vector2? virtualParent)
    {
        LocalPosition = position;
        GlobalPosition = CalculateGlobalPosition();

        if (Options.SizeUnit == SizeUnit.Pixels)
        {
            Options.Size = size;
        }
        else if (Options.SizeUnit == SizeUnit.Percent)
        {
            var viewport = RanchMayhemEngine.UiManager.GraphicsDevice.Viewport;
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

        CalculateScale();
        UpdateBounds(parent);
    }

    private void CalculateScale()
    {
        if (Options.Texture != null)
        {
            var scaleX = Options.Size.X / Options.Texture?.Width;
            var scaleY = Options.Size.Y / Options.Texture?.Height;
            var scale = new Vector2(scaleX ?? 1, scaleY ?? 1);

            Options.Scale = scale;
        }
    }

    public void SetTextureOverlay(Texture2D texture)
    {
        Options.TextureOverlay = texture;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Options.Texture != null)
        {
            spriteBatch.Draw(Options.Texture, GlobalPosition, null, Color.White, 0f, Vector2.Zero,
                Options.Scale, SpriteEffects.None, 0f);

            if (Options.TextureOverlay != null)
            {
                spriteBatch.Draw(Options.TextureOverlay, GlobalPosition, null, Color.White, 0f, Vector2.Zero,
                    Options.Scale, SpriteEffects.None, 0f);
            }
        }
        else
        {
            // Logger.Log($"{GetType().FullName}::Draw Id={Id} Drawing at {GlobalPosition}");
            var texture = new Texture2D(RanchMayhemEngine.UiManager.GraphicsDevice, 1, 1);
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

        if (_hasBorder)
        {
            DrawBorder(spriteBatch);
        }

        if (HoverItem != null && IsHovered)
        {
            DrawHoverItem(spriteBatch, RanchMayhemEngine.MouseState);
        }
    }

    protected void DrawBorder(SpriteBatch spriteBatch)
    {
        if (Options.BorderTexture == null)
        {
            if (Options.BorderOrientation == BorderOrientation.Inside)
            {
                var top = GlobalPosition;
                var topSize = new Vector2(Options.Size.X, Options.BorderSize);

                if (Options.BorderPosition.HasFlag(BorderPosition.Top))
                {
                    DrawRectangle(spriteBatch, top, topSize, Options.BorderColor);
                }

                var left = new Vector2(GlobalPosition.X, GlobalPosition.Y + Options.BorderSize);
                var leftSize = new Vector2(Options.BorderSize, Options.Size.Y - 2 * Options.BorderSize);

                if (Options.BorderPosition.HasFlag(BorderPosition.Left))
                {
                    DrawRectangle(spriteBatch, left, leftSize, Options.BorderColor);
                }

                var right = new Vector2(GlobalPosition.X + Options.Size.X - Options.BorderSize, left.Y);

                if (Options.BorderPosition.HasFlag(BorderPosition.Right))
                {
                    DrawRectangle(spriteBatch, right, leftSize, Options.BorderColor);
                }


                var bottom = new Vector2(GlobalPosition.X,
                    GlobalPosition.Y + leftSize.Y + Options.BorderSize);
                if (Options.BorderPosition.HasFlag(BorderPosition.Top))
                {
                    DrawRectangle(spriteBatch, bottom, topSize, Options.BorderColor);
                }
            }

            if (Options.BorderOrientation == BorderOrientation.Outside)
            {
                var top = new Vector2(GlobalPosition.X - Options.BorderSize, GlobalPosition.Y - Options.BorderSize);
                var topSize = new Vector2(Options.Size.X + Options.BorderSize * 2, Options.BorderSize);

                if (Options.BorderPosition.HasFlag(BorderPosition.Top))
                {
                    DrawRectangle(spriteBatch, top, topSize, Options.BorderColor);
                }

                var left = new Vector2(top.X, top.Y + Options.BorderSize);
                var leftSize = new Vector2(Options.BorderSize, Options.Size.Y);
                if (Options.BorderPosition.HasFlag(BorderPosition.Left))
                {
                    DrawRectangle(spriteBatch, left, leftSize, Options.BorderColor);
                }

                var right = new Vector2(left.X + Options.Size.X + Options.BorderSize, left.Y);

                if (Options.BorderPosition.HasFlag(BorderPosition.Right))
                {
                    DrawRectangle(spriteBatch, right, leftSize, Options.BorderColor);
                }

                var bottom = new Vector2(top.X, top.Y + Options.Size.Y + Options.BorderSize - 1);

                if (Options.BorderPosition.HasFlag(BorderPosition.Bottom))
                {
                    DrawRectangle(spriteBatch, bottom, topSize, Options.BorderColor);
                }
            }
        }
        else
        {
            var topLeftCorner = new Vector2(GlobalPosition.X - Options.BorderSize,
                GlobalPosition.Y - Options.BorderSize);

            var bottomLeftCorner = new Vector2(topLeftCorner.X,
                topLeftCorner.Y + Options.Size.Y + Options.BorderSize - 1);

            var topRightCorner =
                new Vector2(topLeftCorner.X + Options.Size.X + Options.BorderSize, topLeftCorner.Y);

            var bottomRightCorner = new Vector2(topRightCorner.X,
                topRightCorner.Y + Options.Size.Y + Options.BorderSize - 1);

            var top = new Vector2(GlobalPosition.X, GlobalPosition.Y - Options.BorderSize);
            var topSize = new Vector2(Options.Size.X, Options.BorderSize);

            var left = new Vector2(topLeftCorner.X, topLeftCorner.Y + Options.BorderSize);
            var leftSize = new Vector2(Options.BorderSize, Options.Size.Y);

            var right = new Vector2(left.X + Options.Size.X + Options.BorderSize, left.Y);
            var bottom = new Vector2(top.X, top.Y + Options.Size.Y + Options.BorderSize - 1);


            if (Options.BorderPosition.HasFlag(BorderPosition.Top))
            {
                DrawTiledTexture(spriteBatch, Options.BorderCornerTexture, topLeftCorner,
                    new Vector2(Options.BorderSize));
                DrawTiledTexture(spriteBatch, Options.BorderCornerTexture, topRightCorner,
                    new Vector2(Options.BorderSize));
                DrawTiledTexture(spriteBatch, Options.BorderTexture, top, topSize);
            }

            if (Options.BorderPosition.HasFlag(BorderPosition.Left))
            {
                DrawTiledTexture(spriteBatch, Options.BorderTexture, left, leftSize, false);
            }

            if (Options.BorderPosition.HasFlag(BorderPosition.Right))
            {
                DrawTiledTexture(spriteBatch, Options.BorderCornerTexture, bottomLeftCorner,
                    new Vector2(Options.BorderSize));
                DrawTiledTexture(spriteBatch, Options.BorderTexture, right, leftSize, false);
            }

            if (Options.BorderPosition.HasFlag(BorderPosition.Bottom))
            {
                DrawTiledTexture(spriteBatch, Options.BorderCornerTexture, bottomRightCorner,
                    new Vector2(Options.BorderSize));
                DrawTiledTexture(spriteBatch, Options.BorderTexture, bottom, topSize);
            }
        }
    }

    private void DrawRectangle(SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color)
    {
        var texture = new Texture2D(RanchMayhemEngine.UiManager.GraphicsDevice, 1, 1);
        texture.SetData([color]);

        spriteBatch.Draw(texture,
            new Rectangle((int)Math.Floor(position.X), (int)Math.Floor(position.Y), (int)Math.Floor(size.X),
                (int)Math.Floor(size.Y)),
            color);
    }

    private const int MousePadding = 5;

    private void DrawHoverItem(SpriteBatch spriteBatch, MouseState mouseState)
    {
        var position = new Vector2(mouseState.Position.X - HoverItem!.Options.Size.X / 2,
            mouseState.Position.Y + MousePadding);

        if (mouseState.Position.Y + MousePadding + HoverItem.Options.Size.Y >= RanchMayhemEngine.Height)
        {
            position.Y = mouseState.Position.Y - MousePadding - HoverItem.Options.Size.Y;
        }

        HoverItem.SetPosition(position);
        HoverItem.Draw(spriteBatch);
    }

    private void DrawTiledTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 size,
        bool horizontal = true)
    {
        var rect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        var tileSize = texture.Width;

        if (horizontal)
        {
            for (var x = rect.Left; x < rect.Right; x += tileSize)
            {
                var width = Math.Min(tileSize, rect.Right - x);
                spriteBatch.Draw(texture, new Rectangle(x, rect.Top, width, Options.BorderSize),
                    new Rectangle(0, 0, width, Options.BorderSize),
                    Color.White);
            }
        }
        else
        {
            for (var y = rect.Top; y < rect.Bottom; y += tileSize)
            {
                var height = Math.Min(tileSize, rect.Bottom - y);

                spriteBatch.Draw(texture, new Rectangle(rect.Left, y, Options.BorderSize, height),
                    new Rectangle(0, 0, Options.BorderSize, height), Color.White);
            }
        }
    }

    public void HandleMouse(MouseState mouseState)
    {
        if (!RanchMayhemEngine.IsFocused || !IsVisible) return;

        if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
            _bounds.Contains(mouseState.Position))
        {
            if (!IsClicked)
            {
                IsClicked = !IsClicked;
                IsActive = true;
            }
        }

        if (IsClicked && mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
        {
            OnClick?.Invoke();
            IsClicked = !IsClicked;
            CanTriggerClick = true;
        }

        if (mouseState is
            {
                LeftButton: Microsoft.Xna.Framework.Input.ButtonState.Released,
                RightButton: Microsoft.Xna.Framework.Input.ButtonState.Released
            } &&
            _bounds.Contains(mouseState.Position))
        {
            if (!IsHovered)
            {
                OnHover?.Invoke();
                IsHovered = !IsHovered;
            }
        }

        if (IsActive && mouseState is
            {
                LeftButton: Microsoft.Xna.Framework.Input.ButtonState.Pressed,
                RightButton: Microsoft.Xna.Framework.Input.ButtonState.Released
            } &&
            !_bounds.Contains(mouseState.Position))
        {
            IsActive = false;
            OffActive();
        }

        if (IsHovered)
        {
            if (_bounds.Contains(mouseState.Position)) return;

            OffHover?.Invoke();
            IsHovered = !IsHovered;
        }
    }

    protected virtual void OffActive()
    {
    }

    protected static Vector2 ScaleToGlobal(Vector2 position)
    {
        return position * RanchMayhemEngine.UiManager.GlobalScale;
    }

    protected static Vector4 ScaleToGlobal(Vector4 position)
    {
        var globalScale = RanchMayhemEngine.UiManager.GlobalScale;
        return new Vector4(position.X * globalScale.Y, position.Y * globalScale.X, position.Z * globalScale.Y,
            position.W * globalScale.X);
    }

    public virtual void SetParent(UiComponent parent)
    {
        // Logger.Log(
        //     $"{GetType().FullName}::SetParent Id:{Id} parent:{parent.Id} parent global pos: {parent.GlobalPosition}");
        Parent = parent;
        RecalculateSize(Options.Size, Parent.Options.Size);
        UpdateGlobalPosition();
        // Logger.Log($"{GetType().FullName}::SetParent Id:{Id} global pos:{GlobalPosition}");
    }

    private void SetPosition(Vector2 position)
    {
        LocalPosition = position;
        GlobalPosition = position;
    }

    public void SetHoverItem(UiComponent item)
    {
        HoverItem = item;
    }

    private Vector2 CalculateGlobalPosition()
    {
        if (Parent == null) return GlobalPosition;
        // Logger.Log(
        //     $"{GetType().FullName}::CalculateGlobalPosition Id:{Id} global pos: {GlobalPosition} local pos: {LocalPosition} parent global: {Parent.CalculateGlobalPosition()}");
        return Parent.CalculateGlobalPosition() + LocalPosition;
    }

    protected void UpdateGlobalPosition()
    {
        if (IsAnimating) return;

        if (Parent == null)
        {
            GlobalPosition = LocalPosition;
        }
        else
        {
            if (Options.UiAnchor != UiAnchor.None)
            {
                LocalPosition = Options.UiAnchor.CalculatePosition(Options.Size, new Vector2(-1), Parent);
                // Logger.Log($"{GetType().FullName}::UpdateGlobalPosition Id:{Id} local pos: {LocalPosition} parent: {Parent.Id}");

                if (Options.UiAnchorOffset != Vector2.Zero)
                {
                    LocalPosition += Options.UiAnchorOffset;
                }

                GlobalPosition = CalculateGlobalPosition();
            }
            else
            {
                GlobalPosition = CalculateGlobalPosition();
            }

            UpdateBounds(Parent);
        }
    }

    private void UpdateBounds(UiComponent? parent)
    {
        if (parent is null)
        {
            _bounds = new Rectangle((int)LocalPosition.X, (int)LocalPosition.Y, (int)Options.Size.X,
                (int)Options.Size.Y);
            GlobalPosition = LocalPosition;
        }
        else
        {
            _bounds = new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, (int)Options.Size.X,
                (int)Options.Size.Y);
        }
    }

    public void SetTexture(Texture2D texture)
    {
        if (Options.Texture != texture)
        {
            Options.Texture = texture;
            CalculateScale();
        }
    }

    public void SetColor(Color color)
    {
        Options.Color = color;
    }

    public void SetOnClick(Action clickAction)
    {
        OnClick = clickAction;
    }

    public abstract void Update();
}
