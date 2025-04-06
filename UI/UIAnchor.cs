using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI;

public static partial class UiAnchorExtension
{
    public static Vector2 CalculatePosition(this UiAnchor uiAnchor, Vector2 size, Vector2 virtualParent,
        UiComponent? parent = null)
    {
        var viewport = RanchMayhemEngine.UiManager.GraphicsDevice.Viewport;
        var parentWidth = parent?.Options.Size.X ?? viewport.Width;
        var parentHeight = parent?.Options.Size.Y ?? viewport.Height;

        if (virtualParent.X >= 0)
        {
            parentWidth = virtualParent.X;
            parentHeight = virtualParent.Y;
        }


        float x = 0;
        float y = 0;

        if (uiAnchor.HasFlag(UiAnchor.Top))
        {
            // Do nothing
        }

        if (uiAnchor.HasFlag(UiAnchor.Bottom))
        {
            y = parentHeight - size.Y;
        }

        if (uiAnchor.HasFlag(UiAnchor.Right))
        {
            x = parentWidth - size.X;
        }

        if (uiAnchor.HasFlag(UiAnchor.CenterX))
        {
            x = (parentWidth - size.X) / 2;
        }

        if (uiAnchor.HasFlag(UiAnchor.CenterY))
        {
            y = (parentHeight - size.Y) / 2;
        }

        return new Vector2(x, y);
    }
}