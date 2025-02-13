using System;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI;

[Flags]
public enum UIAnchor
{
    None = 0,
    Top = 1 << 0,
    Bottom = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
    CenterX = 1 << 4,
    CenterY = 1 << 5
}

public static partial class UIAnchorExtension
{
    public static Vector2 CalculatePosition(this UIAnchor uiAnchor, Vector2 size, Vector2 virtualParent,
        UIComponent parent = null)
    {
        var viewport = RanchMayhemEngine.UIManager.GraphicsDevice.Viewport;
        var parentWidth = parent?.Options.Size.X ?? viewport.Width;
        var parentHeight = parent?.Options.Size.Y ?? viewport.Height;

        if (virtualParent.X >= 0)
        {
            Console.WriteLine("Entering virtual parent mode");
            parentWidth = virtualParent.X;
            parentHeight = virtualParent.Y;
        }

        float x = 0;
        float y = 0;

        if (uiAnchor.HasFlag(UIAnchor.Top))
        {
            // Do nothing
        }

        if (uiAnchor.HasFlag(UIAnchor.Bottom))
        {
            y = parentHeight - size.Y;
        }

        if (uiAnchor.HasFlag(UIAnchor.Right))
        {
            x = parentWidth - size.X;
        }

        if (uiAnchor.HasFlag(UIAnchor.CenterX))
        {
            x = (parentWidth - size.X) / 2;
            // Console.WriteLine($"x = {x}");
        }

        if (uiAnchor.HasFlag(UIAnchor.CenterY))
        {
            y = (parentHeight - size.Y) / 2;
            // Console.WriteLine($"y = {y}");
        }

        return new Vector2(x, y);
    }
}