using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.UI;

public class Button : UiComponent
{
    private readonly ButtonOptions _buttonOptions;
    public Text? Text;
    private float _transitionProgress = 0f;
    private const float TransitionSpeed = 0.3f;

    public Button(string id, ButtonOptions options,
        UiComponent? parent = null) : base(
        id, options, parent)
    {
        OnHover = HandleOnHover;
        OffHover = HandleOffHover;

        OnClick = HandleOnClick;


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

    private void HandleOnHover()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOnHover Id={Id}", LogLevel.Internal);
        // Options.Texture = _buttonOptions.HoverTexture;
    }

    public void AddText(string content, Color color, int size = 16)
    {
        Text = new TextBuilder($"{Id}-text")
            .SetParent(this)
            .SetContent(content)
            .CenterXY()
            .SetFontColor(color)
            .SetFontSize(size)
            .Build();
    }

    private void HandleOnClick()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOnClick Id={Id}", LogLevel.Internal);
        Options.Texture = _buttonOptions.ClickTexture;
    }

    private void HandleOffHover()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOffHover Id={Id}", LogLevel.Internal);
        Options.Texture = _buttonOptions.Texture;
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

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (IsHovered && _buttonOptions.HoverTexture is not null)
        {
            spriteBatch.Draw(_buttonOptions.HoverTexture, GlobalPosition, null, Color.White * _transitionProgress, 0f,
                Vector2.Zero, Options.Scale, SpriteEffects.None, 0f);
        }

        Text?.Draw(spriteBatch);
    }

    public class ButtonOptions : UiComponentOptions
    {
        public Texture2D? HoverTexture;
        public Texture2D? ClickTexture;
        public ButtonState State = ButtonState.Normal;
    }
}