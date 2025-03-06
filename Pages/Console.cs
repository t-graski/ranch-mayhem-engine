using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine.Pages;

public class Console : Page
{
    private Animator windowAnimator;

    public override Page Initialize()
    {
        IsVisible = true;

        var consoleWindow = new Box("console-window", new Box.BoxOptions
        {
            Color = Color.MediumSpringGreen,
            Size = new Vector2(1920, 100),
            UiAnchor = UIAnchor.CenterY
        });

        Components.Add(consoleWindow);

        windowAnimator = new Animator(consoleWindow, 0.2f);
        windowAnimator.StartAnimation();

        return this;
    }

    public override void Update(MouseState mouseState)
    {
        windowAnimator.Update();
        base.Update(mouseState);
    }
}