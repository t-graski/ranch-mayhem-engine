using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Button : UIComponent
{
    private readonly ButtonOptions _buttonOptions;

    public Button(string id, UIComponentOptions options, ButtonOptions buttonOptions, UIComponent parent = null) : base(
        id, options, parent)
    {
        OnHover = HandleOnHover;
        OffHover = HandleOffHover;

        OnClick = HandleOnClick;
        OffClick = HandleOffClick;

        _buttonOptions = buttonOptions;
    }

    private void HandleOnHover()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Console.WriteLine($"{GetType().FullName}::HandleOnHover {Id}");
        Options.Texture = _buttonOptions.HoverTexture;
    }

    private void HandleOnClick()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Console.WriteLine($"{GetType().FullName}::HandleOnClick {Id}");
        Options.Texture = _buttonOptions.ClickTexture;
    }

    private void HandleOffHover()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Console.WriteLine($"{GetType().FullName}::HandleOffHover {Id}");
        Options.Texture = _buttonOptions.Texture;
    }

    private void HandleOffClick()
    {
        if (_buttonOptions.State == ButtonState.Disabled) return;

        Console.WriteLine($"{GetType().FullName}::HandleOffClick {Id}");
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

    public override void Update()
    {
    }

    public class ButtonOptions
    {
        public Texture2D Texture;
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