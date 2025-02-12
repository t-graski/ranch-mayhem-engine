using System;
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
    public static Vector2 CalculatePosition(this UIAnchor uiAnchor, Vector2 size)
    {
        var viewport = RanchMayhemEngine.UIManager.GraphicsDevice.Viewport;
        float x = 0;
        float y = 0;

        if (uiAnchor.HasFlag(UIAnchor.Top))
        {
            // Do nothing
        }

        if (uiAnchor.HasFlag(UIAnchor.Bottom))
        {
            y = viewport.Height - size.Y;
        }

        if (uiAnchor.HasFlag(UIAnchor.Right))
        {
            x = viewport.Width - size.X;
        }

        if (uiAnchor.HasFlag(UIAnchor.CenterX))
        {
            x = (viewport.Width - size.X) / 2;
        }

        if (uiAnchor.HasFlag(UIAnchor.CenterY))
        {
            y = (viewport.Height - size.Y) / 2;
        }

        return new Vector2(x, y);
    }
}