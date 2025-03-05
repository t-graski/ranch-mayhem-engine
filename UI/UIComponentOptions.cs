using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class UIComponentOptions
{
    public Color Color;
    public Texture2D Texture;
    public Vector2 Position;
    public UIAnchor UiAnchor;
    public Vector2 Size;
    public Vector2 SizePercent;
    public SizeUnit SizeUnit = SizeUnit.Pixels;
    public Vector2 Scale;

    public override string ToString()
    {
        return
            $"Color={Color}, Texture={Texture}, Position={Position}, UiAnchor={UiAnchor}, Size={Size}, SizePercent={SizePercent}, SizeUnit={SizeUnit}, Scale={Scale}";
    }
}

public enum SizeUnit
{
    Pixels,
    Percent
}