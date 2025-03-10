using System.Threading.Tasks.Dataflow;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine.Pages;

public class Console : Page
{
    private Animator _windowAnimator;

    public override Page Initialize()
    {
        IsVisible = false;

        var consoleWindow = new TextBox("console-window", new TextBox.TextBoxOptions
        {
            Color = Color.MediumSpringGreen,
            Size = new Vector2(1920, 100),
            UiAnchor = UIAnchor.CenterY,
            FontColor = Color.DarkRed,
            FontSize = 52,

            BorderSize = 10,
            BorderOrientation = BorderOrientation.Inside,
            BorderColor = Color.Red
        }, s => { Logger.Log("Submit"); });

        Components.Add(consoleWindow);

        _windowAnimator = new Animator(consoleWindow, 0.33f);

        return this;
    }

    public override void Update(MouseState mouseState)
    {
        _windowAnimator.Update();
        base.Update(mouseState);
    }

    public override void ToggleVisibility()
    {
        if (!IsVisible)
        {
            _windowAnimator.Reset();
            _windowAnimator.StartAnimation();
        }

        base.ToggleVisibility();
    }
}