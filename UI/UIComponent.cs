using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.Content;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ranch_mayhem_engine.UI;

public abstract class UiComponent
{
    public string Id { get; }
    public UiComponent? Parent { get; protected set; }
    public bool IsVisible { get; set; } = true;
    public Vector2 LocalPosition { get; private set; }
    public Vector2 GlobalPosition { get; private set; }
    public Rectangle Bounds { get; protected set; }
    public UiComponentOptions Options { get; } = new();
    protected Effect RenderShader { get; set; }
    public bool HasHoverShader { get; set; }
    protected static Effect DefaultHoverShader { get; set; } = ContentManager.GetShader("Outline");
    protected Effect HoverShader { get; set; } = DefaultHoverShader;
    private UiComponent? HoverItem { get; set; }
    private bool _hasBorder = false;
    public bool IsAnimating;

    public bool AllowClickThrough { get; set; } = false;
    public bool AllowRightClickThrough { get; set; } = false;

    public Action? OnClick;
    public Action? OffClick;
    public Action? OnRightClick;
    public Action? OffRightClick;
    public Action? OnHover;
    public Action? OffHover;

    public Action? OnScrollUp;
    public Action? OnScrollDown;

    protected bool IsHovered;
    protected bool IsClicked;
    protected bool IsRightClicked;
    protected bool IsActive;

    public bool CanTriggerClick;
    public bool CanTriggerRightClick;

    protected event Action<Vector2>? OnPositionChange;

    protected UiComponent(
        string id, UiComponentOptions options, UiComponent? parent = null, bool scale = true,
        Effect? renderShader = null
    )
    {
        Id = id;
        Parent = parent;

        if (Parent is not null)
        {
            Parent.OnPositionChange += HandleParentGlobalPositionChange;
            RenderShader = Parent.RenderShader;
        }
        else
        {
            RenderShader = renderShader;
        }

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

        DefaultHoverShader.Parameters["OutlineColor"].SetValue(Color.White.ToVector4());
        DefaultHoverShader.Parameters["AlphaThreshold"].SetValue(0.1f);
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
                Logger.Log(
                    $"{prefix} SizeUnit is set to Percent and SizePercent is {Vector2.Zero}",
                    LogLevel.Warning
                );
            }
            else
            {
                Logger.Log(
                    $"{prefix} SizeUnit is set to Percent but 'Size' is being used instead of 'SizePercent'",
                    LogLevel.Warning
                );
            }
        }

        if (options.Scale.X < 0 || options.Scale.Y < 0)
        {
            Logger.Log($"{prefix} Scale is negative ({options.Scale})", LogLevel.Warning);
        }

        // TODO: check for border options to be either all null or none null.
    }

    private void ApplyOptions(UiComponentOptions options)
    {
        Options.Size = ScaleToGlobal(options.Size);
        Options.SizeUnit = options.SizeUnit;
        Options.SizePercent = options.SizePercent;
        Options.UiAnchor = options.UiAnchor;
        Options.UiAnchorOffset = ScaleToGlobal(options.UiAnchorOffset);

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
            // Options.TextureOverlay = options.TextureOverlay;
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
            CalculateScale();
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

    public void SetSizePixels(Vector2 size)
    {
        if (Options.SizeUnit == SizeUnit.Percent)
        {
            var prefix = $"{GetType().FullName}::ctor Id={Id}";
            Logger.Log(
                $"{prefix} Unable to set size pixels for SizeUnit Percent",
                LogLevel.Warning
            );
            return;
        }

        Options.Size = ScaleToGlobal(size);
        RecalculateSize(ScaleToGlobal(size), null);
        // UpdateGlobalPosition();
    }

    public void UpdatePosition(Vector2 position, Vector2 size, UiComponent? parent, Vector2? virtualParent)
    {
        LocalPosition = position;
        GlobalPosition = CalculateGlobalPosition();
        // OnPositionChange?.Invoke(GlobalPosition);

        // if (Options.SizeUnit == SizeUnit.Pixels)
        // {
        //     Options.Size = size;
        // }
        // else if (Options.SizeUnit == SizeUnit.Percent)
        // {
        //     var viewport = RanchMayhemEngine.UiManager.GraphicsDevice.Viewport;
        //     var width = virtualParent?.X ?? viewport.Width;
        //     var height = virtualParent?.Y ?? viewport.Height;
        //     var newSize = Vector2.Zero;
        //
        //     if (Options.SizePercent.X != 0 && Options.SizePercent.Y != 0)
        //     {
        //         newSize = new Vector2(width * (Options.SizePercent.X / 100), height * (Options.SizePercent.Y / 100));
        //     }
        //     else if (Options.SizePercent.Y == 0)
        //     {
        //         newSize = new Vector2(width * (Options.SizePercent.X / 100));
        //     }
        //     else if (Options.SizePercent.X == 0)
        //     {
        //         newSize = new Vector2(height * (Options.SizePercent.Y / 100));
        //     }
        //
        //     Options.Size = newSize;
        // }

        RecalculateSize(size, virtualParent);
        CalculateScale();
        UpdateBounds(parent);
    }

    protected void CalculateScale()
    {
        if (Options.Texture != null)
        {
            var scaleX = Options.Size.X / Options.Texture?.Width;
            var scaleY = Options.Size.Y / Options.Texture?.Height;
            var scale = new Vector2(scaleX ?? 1, scaleY ?? 1);

            Options.Scale = scale;
        }
    }

    public void ChangeTexture(Texture2D texture)
    {
        if (Options.Texture == texture) return;
        Options.Texture = texture;
        CalculateScale();
    }

    public void SetTextureOverlay(Texture2D texture)
    {
        Options.TextureOverlay = texture;
    }

    public virtual IEnumerable<RenderCommand> Draw()
    {
        if (this is Container { Background: not null } container)
        {
            yield return new RenderCommand
            {
                Id = $"{Id}-container-background",
                Texture = container.Background,
                Position = GlobalPosition,
                SourceRect = null,
                Color = Color.White,
                Rotation = 0f,
                Origin = Vector2.Zero,
                Scale = Options.Scale,
                Effects = SpriteEffects.None,
                LayerDepth = 0f,
                Shader = RenderShader
            };

            // spriteBatch.Draw(
            //     container.Background,
            //     GlobalPosition,
            //     null,
            //     Color.White,
            //     0f,
            //     Vector2.Zero,
            //     Options.Scale,
            //     SpriteEffects.None,
            //     0f
            // );
        }

        if (Options.Texture != null)
        {
            yield return new RenderCommand
            {
                Id = $"{Id}-texture-not-null",
                Texture = Options.Texture,
                Position = GlobalPosition,
                SourceRect = null,
                Color = Color.White,
                Rotation = 0f,
                Origin = Vector2.Zero,
                Scale = Options.Scale,
                Effects = SpriteEffects.None,
                LayerDepth = 0f,
                Shader = RenderShader
            };

            // spriteBatch.Draw(
            //     Options.Texture,
            //     GlobalPosition,
            //     null,
            //     Color.White,
            //     0f,
            //     Vector2.Zero,
            //     Options.Scale,
            //     SpriteEffects.None,
            //     0f
            // );

            // if (Options.TextureOverlay != null)
            // {
            //     spriteBatch.Draw(
            //         Options.TextureOverlay,
            //         GlobalPosition,
            //         null,
            //         Color.White,
            //         0f,
            //         Vector2.Zero,
            //         Options.Scale,
            //         SpriteEffects.None,
            //         0f
            //     );
            // }
        }
        else
        {
            // if (Parent is null)
            // {
            var texture = new Texture2D(RanchMayhemEngine.UiManager.GraphicsDevice, 1, 1);
            texture.SetData([Options.Color]);
            yield return new RenderCommand
            {
                Id = $"{Id}-texture-null",
                Texture = texture,
                DestinationRect = new Rectangle(
                    (int)GlobalPosition.X,
                    (int)GlobalPosition.Y,
                    (int)Options.Size.X,
                    (int)Options.Size.Y
                ),
                Color = Options.Color,
                Shader = RenderShader
            };

            // spriteBatch.Draw(
            //     UiManager.Pixel,
            //     new Rectangle(
            //         (int)GlobalPosition.X,
            //         (int)GlobalPosition.Y,
            //         (int)Options.Size.X,
            //         (int)Options.Size.Y
            //     ),
            //     Options.Color
            // );
            // }
            // else
            // {
            // spriteBatch.Draw(
            //     UiManager.Pixel,
            //     new Rectangle(
            //         (int)GlobalPosition.X,
            //         (int)GlobalPosition.Y,
            //         (int)Options.Size.X,
            //         (int)Options.Size.Y
            //     ),
            //     Options.Color
            // );
            // }
        }

        if (_hasBorder)
        {
            foreach (var command in DrawBorder())
            {
                yield return command;
            }
        }

        if (HoverItem != null && IsHovered)
        {
            DrawHoverItem(RanchMayhemEngine.MouseState);
        }
    }

    protected IEnumerable<RenderCommand> DrawBorder()
    {
        if (!_hasBorder) yield break;

        if (Options.BorderTexture == null)
        {
            if (Options.BorderOrientation == BorderOrientation.Inside)
            {
                var top = GlobalPosition;
                var topSize = new Vector2(Options.Size.X, Options.BorderSize);

                if (Options.BorderPosition.HasFlag(BorderPosition.Top))
                {
                    yield return DrawRectangle(top, topSize, Options.BorderColor);
                }

                var left = new Vector2(GlobalPosition.X, GlobalPosition.Y + Options.BorderSize);
                var leftSize = new Vector2(Options.BorderSize, Options.Size.Y - 2 * Options.BorderSize);

                if (Options.BorderPosition.HasFlag(BorderPosition.Left))
                {
                    yield return DrawRectangle(left, leftSize, Options.BorderColor);
                }

                var right = new Vector2(GlobalPosition.X + Options.Size.X - Options.BorderSize, left.Y);

                if (Options.BorderPosition.HasFlag(BorderPosition.Right))
                {
                    yield return DrawRectangle(right, leftSize, Options.BorderColor);
                }


                var bottom = new Vector2(
                    GlobalPosition.X,
                    GlobalPosition.Y + leftSize.Y + Options.BorderSize
                );
                if (Options.BorderPosition.HasFlag(BorderPosition.Top))
                {
                    yield return DrawRectangle(bottom, topSize, Options.BorderColor);
                }
            }

            if (Options.BorderOrientation == BorderOrientation.Outside)
            {
                var top = new Vector2(GlobalPosition.X - Options.BorderSize, GlobalPosition.Y - Options.BorderSize);
                var topSize = new Vector2(Options.Size.X + Options.BorderSize * 2, Options.BorderSize);

                if (Options.BorderPosition.HasFlag(BorderPosition.Top))
                {
                    yield return DrawRectangle(top, topSize, Options.BorderColor);
                }

                var left = new Vector2(top.X, top.Y + Options.BorderSize);
                var leftSize = new Vector2(Options.BorderSize, Options.Size.Y);
                if (Options.BorderPosition.HasFlag(BorderPosition.Left))
                {
                    yield return DrawRectangle(left, leftSize, Options.BorderColor);
                }

                var right = new Vector2(left.X + Options.Size.X + Options.BorderSize, left.Y);

                if (Options.BorderPosition.HasFlag(BorderPosition.Right))
                {
                    yield return DrawRectangle(right, leftSize, Options.BorderColor);
                }

                var bottom = new Vector2(top.X, top.Y + Options.Size.Y + Options.BorderSize - 1);

                if (Options.BorderPosition.HasFlag(BorderPosition.Bottom))
                {
                    yield return DrawRectangle(bottom, topSize, Options.BorderColor);
                }
            }
        }
        else
        {
            var topLeftCorner = new Vector2(
                GlobalPosition.X - Options.BorderSize,
                GlobalPosition.Y - Options.BorderSize
            );

            var bottomLeftCorner = new Vector2(
                topLeftCorner.X,
                topLeftCorner.Y + Options.Size.Y + Options.BorderSize - 1
            );

            var topRightCorner =
                new Vector2(topLeftCorner.X + Options.Size.X + Options.BorderSize, topLeftCorner.Y);

            var bottomRightCorner = new Vector2(
                topRightCorner.X,
                topRightCorner.Y + Options.Size.Y + Options.BorderSize - 1
            );

            var top = new Vector2(GlobalPosition.X, GlobalPosition.Y - Options.BorderSize);
            var topSize = new Vector2(Options.Size.X, Options.BorderSize);

            var left = new Vector2(topLeftCorner.X, topLeftCorner.Y + Options.BorderSize);
            var leftSize = new Vector2(Options.BorderSize, Options.Size.Y);

            var right = new Vector2(left.X + Options.Size.X + Options.BorderSize, left.Y);
            var bottom = new Vector2(top.X, top.Y + Options.Size.Y + Options.BorderSize - 1);


            if (Options.BorderPosition.HasFlag(BorderPosition.Top))
            {
                foreach (var command in DrawTiledTexture(Options.BorderCornerTexture, topLeftCorner,
                             new Vector2(Options.BorderSize)))
                {
                    yield return command;
                }

                // yield return DrawTiledTexture(
                //     Options.BorderCornerTexture,
                //     topLeftCorner,
                //     new Vector2(Options.BorderSize)
                // );
                // DrawTiledTexture(
                //     spriteBatch,
                //     Options.BorderCornerTexture,
                //     topRightCorner,
                //     new Vector2(Options.BorderSize)
                // );

                foreach (var command in DrawTiledTexture(Options.BorderCornerTexture, topRightCorner,
                             new Vector2(Options.BorderSize)))
                {
                    yield return command;
                }

                foreach (var command in DrawTiledTexture(Options.BorderTexture, top, topSize))
                {
                    yield return command;
                }

                // DrawTiledTexture(spriteBatch, Options.BorderTexture, top, topSize);
            }

            if (Options.BorderPosition.HasFlag(BorderPosition.Left))
            {
                foreach (var command in DrawTiledTexture(Options.BorderTexture, left, leftSize, false))
                {
                    yield return command;
                }

                // DrawTiledTexture(spriteBatch, Options.BorderTexture, left, leftSize, false);
            }

            if (Options.BorderPosition.HasFlag(BorderPosition.Right))
            {
                foreach (var command in DrawTiledTexture(Options.BorderCornerTexture, bottomLeftCorner,
                             new Vector2(Options.BorderSize)))
                {
                    yield return command;
                }

                foreach (var command in DrawTiledTexture(Options.BorderTexture, right, leftSize, false))
                {
                    yield return command;
                }

                // DrawTiledTexture(
                //     spriteBatch,
                //     Options.BorderCornerTexture,
                //     bottomLeftCorner,
                //     new Vector2(Options.BorderSize)
                // );
                // DrawTiledTexture(spriteBatch, Options.BorderTexture, right, leftSize, false);
            }

            if (Options.BorderPosition.HasFlag(BorderPosition.Bottom))
            {
                foreach (var command in DrawTiledTexture(Options.BorderCornerTexture, bottomRightCorner,
                             new Vector2(Options.BorderSize)))
                {
                    yield return command;
                }

                // DrawTiledTexture(
                //     spriteBatch,
                //     Options.BorderCornerTexture,
                //     bottomRightCorner,
                //     new Vector2(Options.BorderSize)
                // );

                foreach (var command in DrawTiledTexture(Options.BorderTexture, bottom, topSize))
                {
                    yield return command;
                }

                // DrawTiledTexture(spriteBatch, Options.BorderTexture, bottom, topSize);
            }
        }
    }

    private RenderCommand DrawRectangle(Vector2 position, Vector2 size, Color color)
    {
        return new RenderCommand
        {
            Id = $"{Id}-draw-rectangle",
            Texture = UiManager.Pixel,
            DestinationRect = new Rectangle(
                (int)Math.Floor(position.X),
                (int)Math.Floor(position.Y),
                (int)Math.Floor(size.X),
                (int)Math.Floor(size.Y)
            ),
            Color = color
        };
        //
        // var texture = new Texture2D(RanchMayhemEngine.UiManager.GraphicsDevice, 1, 1);
        // texture.SetData([color]);
        //
        // spriteBatch.Draw(
        //     texture,
        //     new Rectangle(
        //         (int)Math.Floor(position.X),
        //         (int)Math.Floor(position.Y),
        //         (int)Math.Floor(size.X),
        //         (int)Math.Floor(size.Y)
        //     ),
        //     color
        // );
    }

    private const int MousePadding = 16;

    private void DrawHoverItem(MouseState mouseState)
    {
        var position = new Vector2(
            mouseState.Position.X - HoverItem!.Options.Size.X / 2,
            mouseState.Position.Y + MousePadding
        );

        if (mouseState.Position.Y + MousePadding + HoverItem.Options.Size.Y >= RanchMayhemEngine.Height)
        {
            position.Y = mouseState.Position.Y - MousePadding - HoverItem.Options.Size.Y;
        }

        HoverItem.SetPosition(position);
        UiManager.Enqueue(HoverItem.Draw(), UiManager.OverlayQueue);
    }

    protected IEnumerable<RenderCommand> DrawTiledTexture(
        Texture2D texture, Vector2 position, Vector2 size,
        bool horizontal = true, bool stretch = false
    )
    {
        var rect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        var tileSize = texture.Width;

        if (horizontal)
        {
            var tileWidth = texture.Width;
            var tileHeight = texture.Height;
            var sourceHeight = Options.BorderSize;
            var destinationHeight = Options.BorderSize;

            if (stretch)
            {
                sourceHeight = texture.Height;
                destinationHeight = (int)size.Y;
            }


            // for (var x = rect.Left; x < rect.Right; x += tileSize)
            for (var x = 0; x < rect.Width; x += tileWidth)
            {
                var drawWidth = tileWidth;
                var remaining = rect.Width - x;

                var destRect = new Rectangle(rect.X + x, rect.Y, drawWidth, destinationHeight);
                var srcRect = new Rectangle(0, 0, tileWidth, tileHeight);

                if (remaining < tileWidth)
                {
                    drawWidth = remaining;
                    destRect.Width = drawWidth;
                    srcRect.Width = drawWidth;
                }

                yield return new RenderCommand
                {
                    Id = $"{Id}-draw-tiled-texture",
                    Texture = texture,
                    DestinationRect = destRect,
                    SourceRect = srcRect,
                    Color = Color.White
                };

                // spriteBatch.Draw(texture, destRect, srcRect, Color.White);

                // var width = Math.Min(tileSize, rect.Right - x);
                // spriteBatch.Draw(
                //     texture,
                //     new Rectangle(x, rect.Top, width, destinationHeight),
                //     new Rectangle(0, 0, width, sourceHeight),
                //     Color.White
                // );
            }
        }
        else
        {
            for (var y = rect.Top; y < rect.Bottom; y += tileSize)
            {
                var height = Math.Min(tileSize, rect.Bottom - y);

                yield return new RenderCommand
                {
                    Id = $"{Id}-draw-tiled-texture",
                    Texture = texture,
                    DestinationRect = new Rectangle(rect.Left, y, Options.BorderSize, height),
                    SourceRect = new Rectangle(0, 0, Options.BorderSize, height),
                    Color = Color.White
                };

                // spriteBatch.Draw(
                //     texture,
                //     new Rectangle(rect.Left, y, Options.BorderSize, height),
                //     new Rectangle(0, 0, Options.BorderSize, height),
                //     Color.White
                // );
            }
        }
    }

    public void HandleMouse(MouseState mouseState)
    {
        if (!RanchMayhemEngine.IsFocused || !IsVisible) return;

        if (RanchMayhemEngine.UiManager.IsInputDisabled && !RanchMayhemEngine.UiManager.InputExceptions.Contains(Id))
        {
            return;
        }

        if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
            (MouseInput.PreviousState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released ||
             AllowClickThrough) &&
            Bounds.Contains(mouseState.Position))
        {
            if (!IsClicked)
            {
                OnClick?.Invoke();
                IsClicked = !IsClicked;
                IsActive = true;
            }
        }

        if (IsClicked && mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
        {
            if (Bounds.Contains(mouseState.Position))
            {
                OffClick?.Invoke();
            }

            IsClicked = !IsClicked;
            CanTriggerClick = true;
        }

        if (mouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
            (MouseInput.PreviousState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released ||
             AllowRightClickThrough) &&
            Bounds.Contains(mouseState.Position))
        {
            if (!IsRightClicked)
            {
                IsRightClicked = !IsRightClicked;
                IsActive = true;
            }
        }

        if (IsRightClicked && mouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
        {
            if (Bounds.Contains(mouseState.Position))
            {
                OnRightClick?.Invoke();
            }

            IsRightClicked = !IsRightClicked;
            CanTriggerRightClick = true;
        }

        if (mouseState is
            {
                LeftButton: Microsoft.Xna.Framework.Input.ButtonState.Released,
                RightButton: Microsoft.Xna.Framework.Input.ButtonState.Released
            } &&
            Bounds.Contains(mouseState.Position))
        {
            if (!IsHovered)
            {
                OnHover?.Invoke();
                IsHovered = !IsHovered;

                if (HasHoverShader)
                {
                    SetRenderShader(HoverShader);
                }
            }
        }

        if (IsActive && mouseState is
            {
                LeftButton: Microsoft.Xna.Framework.Input.ButtonState.Pressed,
                RightButton: Microsoft.Xna.Framework.Input.ButtonState.Released
            } &&
            !Bounds.Contains(mouseState.Position))
        {
            IsActive = false;
            OffActive();
        }

        if (IsHovered)
        {
            // check for scrolling
            var scrollDelta = MouseInput.CurrentState.ScrollWheelValue - MouseInput.PreviousState.ScrollWheelValue;
            if (scrollDelta > 0)
            {
                OnScrollUp?.Invoke();
            }
            else if (scrollDelta < 0)
            {
                OnScrollDown?.Invoke();
            }
        }

        if (IsHovered)
        {
            if (Bounds.Contains(mouseState.Position)) return;

            OffHover?.Invoke();
            IsHovered = !IsHovered;

            if (HasHoverShader)
            {
                SetRenderShader(null);
            }
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
        return new Vector4(
            position.X * globalScale.Y,
            position.Y * globalScale.X,
            position.Z * globalScale.Y,
            position.W * globalScale.X
        );
    }

    public virtual void SetParent(UiComponent parent)
    {
        Parent = parent;
        RecalculateSize(Options.Size, Parent.Options.Size);
        UpdateGlobalPosition();
        Parent.OnPositionChange += HandleParentGlobalPositionChange;
    }

    public virtual void SetPosition(Vector2 position)
    {
        LocalPosition = position;
        GlobalPosition = position;
        // OnPositionChange?.Invoke(GlobalPosition);

        if (this is Container container)
        {
            container.HandleParentGlobalPositionChange(GlobalPosition);
        }
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
        // if (IsAnimating) return;

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

        // OnPositionChange?.Invoke(GlobalPosition);
    }

    private void UpdateBounds(UiComponent? parent)
    {
        if (parent is null)
        {
            Bounds = new Rectangle(
                (int)LocalPosition.X,
                (int)LocalPosition.Y,
                (int)Options.Size.X,
                (int)Options.Size.Y
            );
            GlobalPosition = LocalPosition;
        }
        else
        {
            Bounds = new Rectangle(
                (int)GlobalPosition.X,
                (int)GlobalPosition.Y,
                (int)Options.Size.X,
                (int)Options.Size.Y
            );
        }
    }

    public virtual void SetTexture(Texture2D texture)
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

    public virtual void SetRenderShader(Effect shader)
    {
        RenderShader = shader;
    }

    public virtual void SetHoverShader(Effect? shader)
    {
        if (!HasHoverShader) return;

        HoverShader = shader;
        HoverShader?.Parameters["TexelSize"].SetValue(new Vector2(1f / Options.Size.X, 1f / Options.Size.Y));
    }

    public abstract void Update();


    public void SetGlobalPosition(Vector2 position)
    {
        GlobalPosition = position;

        // TODO: refactor this to adjust for future UiComponents
        if (this is Container container)
        {
            container.HandleParentGlobalPositionChange(GlobalPosition);
        }

        OnPositionChange?.Invoke(GlobalPosition);
    }

    public virtual void ToggleAnimating()
    {
        IsAnimating = !IsAnimating;
    }

    public virtual void HandleParentGlobalPositionChange(Vector2 position)
    {
        Logger.Log($"{GetType().FullName}::HandleParentGlobalPositionChange Id={Id} Parent={Parent?.Id}",
            LogLevel.Internal);
        GlobalPosition = CalculateGlobalPosition();
    }
}