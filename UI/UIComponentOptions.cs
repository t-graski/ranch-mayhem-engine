using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class UIComponentOptions
{
    public Color Color;
    public Texture2D? Texture;
    public Texture2D? TextureOverlay;
    public Vector2 Position;
    public UiAnchor UiAnchor;
    public Vector2 UiAnchorOffset;
    public Vector2 Size;
    public Vector2 SizePercent;
    public SizeUnit SizeUnit = SizeUnit.Pixels;
    public Vector2 Scale;

    public Color BorderColor;
    public Texture2D? BorderTexture;
    public Texture2D? BorderCornerTexture;
    public int BorderSize;
    public BorderOrientation BorderOrientation = BorderOrientation.Outside;
    public BorderPosition BorderPosition = BorderPosition.All;
}