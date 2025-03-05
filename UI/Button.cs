using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Button : UIComponent
{
    private readonly ButtonOptions _buttonOptions;

    public Button(string id, ButtonOptions options,
        UIComponent parent = null) : base(
        id, options, parent)
    {
        OnHover = HandleOnHover;
        OffHover = HandleOffHover;

        OnClick = HandleOnClick;
        OffClick = HandleOffClick;


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

        Logger.Log($"{GetType().FullName}::HandleOnHover Id={Id}", Logger.LogLevel.Internal);
        Options.Texture = _buttonOptions.HoverTexture;
    }

    private void HandleOnClick()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOnClick Id={Id}", Logger.LogLevel.Internal);
        Options.Texture = _buttonOptions.ClickTexture;
    }

    private void HandleOffHover()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOffHover Id={Id}", Logger.LogLevel.Internal);
        Options.Texture = _buttonOptions.Texture;
    }

    private void HandleOffClick()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Logger.Log($"{GetType().FullName}::HandleOffClick Id={Id}", Logger.LogLevel.Internal);
        Options.Texture = _buttonOptions.HoverTexture;
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

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }

    public override void Update()
    {
    }

    public class ButtonOptions : UIComponentOptions
    {
        public Texture2D HoverTexture;
        public Texture2D ClickTexture;
        public ButtonState State = ButtonState.Normal;
    }

    public enum ButtonState
    {
        Normal,
        Disabled
    }
}