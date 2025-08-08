using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class UiComponentOptions
{
    public Color Color = Color.Transparent;
    public Texture2D? Texture;

    [Obsolete("This attribute is not yet implemented, but will be in the future.")]
    public Texture2D? TextureOverlay;

    public Vector2 Position = Vector2.Zero;
    public UiAnchor UiAnchor = UiAnchor.None;
    public Vector2 UiAnchorOffset = Vector2.Zero;
    public Vector2 Size = Vector2.One;
    public Vector2 SizePercent = new(100);
    public SizeUnit SizeUnit = SizeUnit.Pixels;
    public Vector2 Scale = Vector2.One;

    public Color BorderColor = Color.Transparent;
    public Texture2D? BorderTexture;
    public Texture2D? BorderCornerTexture;
    public int BorderSize = 8;
    public BorderOrientation BorderOrientation = BorderOrientation.Outside;
    public BorderPosition BorderPosition = BorderPosition.All;
}
