using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public struct RenderCommand()
{
    public Texture2D Texture = null;
    public Vector2 Position = default;
    public Rectangle? DestinationRect = null;
    public Rectangle? SourceRect = null;
    public Color Color = Color.Transparent;
    public float Rotation = 0;
    public Vector2 Origin = default;
    public Vector2 Scale = default;
    public SpriteEffects Effects = SpriteEffects.None;
    public float LayerDepth = 0;
    public Effect Shader = null;
    public Matrix Transform = default;

    public SpriteFont? SpriteFont = null;
    public string Text = "";
    public string Id = "";

    public override string ToString()
    {
        return
            $"RenderCommand(Id: {Id}, Texture: {Texture?.Name ?? "null"}, Position: {Position}, DestinationRect: {DestinationRect}, SourceRect: {SourceRect}, Color: {Color}, Rotation: {Rotation}, Origin: {Origin}, Scale: {Scale}, Effects: {Effects}, LayerDepth: {LayerDepth}, Shader: {Shader?.Name ?? "null"}, Transform: {Transform})";
    }
}
