using System;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI;


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
        }

        if (uiAnchor.HasFlag(UIAnchor.CenterY))
        {
            y = (parentHeight - size.Y) / 2;
        }

        return new Vector2(x, y);
    }
}