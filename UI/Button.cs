using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.UI;

public class Button : UiComponent
{
    private readonly ButtonOptions _buttonOptions;
    private Text? _text;
    private float _transitionProgress = 0f;
    private const float TransitionSpeed = 0.3f;

    public Button(
        string id, ButtonOptions options,
        UiComponent? parent = null
    ) : base(
        id,
        options,
        parent
    )
    {
        OnHover = InternalHandleOnHover;
        OffHover = InternalHandleOffHover;

        OnClick = InternalHandleOnClick;
        OffClick = InternalHandleOffClick;

#if DEBUG
        ParseOptions(options);
#endif
        _buttonOptions = options;
    }

    private void ParseOptions(ButtonOptions buttonOptions)
    {
        var prefix = $"{GetType().FullName}::ctor Id={Id}";

        if (buttonOptions.Texture == null)
        {
            Logger.Log($"{prefix} Texture is null");
        }

        if (buttonOptions.HoverTexture == null)
        {
            Logger.Log($"{prefix} HoverTexture is null");
        }

        if (buttonOptions.ClickTexture == null)
        {
            Logger.Log($"{prefix} ClickTexture is null");
        }
    }

    private void InternalHandleOnHover()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOnHover Id={Id}", LogLevel.Internal);

        if (_buttonOptions.HoverTexture == null) return;

        Options.Texture = _buttonOptions.HoverTexture;
    }

    private void InternalHandleOffHover()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOffHover Id={Id}", LogLevel.Internal);
        Options.Texture = _buttonOptions.Texture;
    }

    private void InternalHandleOnClick()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOnClick Id={Id}", LogLevel.Internal);

        if (_buttonOptions.ClickTexture == null) return;

        Options.Texture = _buttonOptions.ClickTexture;
    }


    private void InternalHandleOffClick()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOffClick Id={Id}", LogLevel.Internal);

        Options.Texture = _buttonOptions.Texture;
    }

    public void SetText(string text, Color color, int size = 16)
    {
        if (text.Length == 0)
        {
            _text = null;
            return;
        }

        if (_text == null)
        {
            _text = new TextBuilder($"{Id}-inner-text")
                .SetContent(text)
                .SetUiAnchor(UiAnchor.CenterX | UiAnchor.CenterY)
                .SetFontColor(color)
                .SetFontSize(size)
                .Build();

            _text.SetParent(this);
            return;
        }

        if (_text.GetContent().Equals(text))
        {
            return;
        }

        _text.SetContent(text);
        _text.SetTextColor(color);
    }

    public void AddOnClickAction(Action handler)
    {
        var previous = InternalHandleOnClick;
        OnClick = () =>
        {
            previous?.Invoke();
            handler?.Invoke();
        };
    }

    public void AddOffClickAction(Action handler)
    {
        var previous = InternalHandleOffClick;
        OffClick = () =>
        {
            previous?.Invoke();
            handler?.Invoke();
        };
    }

    public void AddOnHoverAction(Action handler)
    {
        var previous = InternalHandleOnHover;
        OnHover = () =>
        {
            previous?.Invoke();
            handler?.Invoke();
        };
    }

    public void AddOffHoverAction(Action handler)
    {
        var previous = InternalHandleOffHover;
        OffHover = () =>
        {
            previous?.Invoke();
            handler?.Invoke();
        };
    }

    public void ToggleDisabled()
    {
        if (_buttonOptions.State == ButtonState.Disabled)
        {
            _buttonOptions.State = ButtonState.Normal;
        }
        else
        {
            _buttonOptions.State = ButtonState.Disabled;
        }
    }

    public override void Update()
    {
        var target = IsHovered ? 1f : 0f;
        _transitionProgress = MathHelper.Lerp(_transitionProgress, target, TransitionSpeed);

        if (Math.Abs(_transitionProgress - target) < 0.1f)
        {
            _transitionProgress = target;
        }
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        foreach (var command in base.Draw())
        {
            yield return command;
        }

        if (IsHovered && _buttonOptions.HoverTexture is not null)
        {
            yield return new RenderCommand
            {
                Id = $"{Id}-button-hover",
                Texture = _buttonOptions.HoverTexture,
                Position = GlobalPosition,
                SourceRect = null,
                Color = Color.White * _transitionProgress,
                Rotation = 0f,
                Origin = Vector2.Zero,
                Scale = Options.Scale,
                Effects = SpriteEffects.None,
                LayerDepth = 0f
            };

            // spriteBatch.Draw(
            //     _buttonOptions.HoverTexture,
            //     GlobalPosition,
            //     null,
            //     Color.White * _transitionProgress,
            //     0f,
            //     Vector2.Zero,
            //     Options.Scale,
            //     SpriteEffects.None,
            //     0f
            // );
        }

        if (_text is null) yield break;
        foreach (var command in _text?.Draw())
        {
            yield return command;
        }

        // _text?.Draw(spriteBatch);
    }

    public class ButtonOptions : UiComponentOptions
    {
        public Texture2D? HoverTexture;
        public Texture2D? ClickTexture;
        public ButtonState State = ButtonState.Normal;
    }
}
