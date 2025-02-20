using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Text : UIComponent
{
    private TextOptions _textOptions;
    private SpriteFont _font;

    public Text(string id, UIComponentOptions options, TextOptions textOptions, UIComponent parent = null,
        bool scale = true) : base(id, options,
        parent, scale)
    {
        _textOptions = textOptions;
        InitializeFont();
    }

    private void InitializeFont()
    {
        var size = RanchMayhemEngine.ContentManager.GetClosestSize("Arial", _textOptions.Size);
        _font = RanchMayhemEngine.ContentManager.GetFont("Arial", size);
        var scale = 1.0f;

        if (size != _textOptions.Size)
        {
            scale = CalculateScale(size, _textOptions.Size);
        }

        Logger.Log(
            $"{GetType().FullName}::InitializeFont Id={Id} Given size: {_textOptions.Size}, Found size: {size}, Using scale: {scale}",
            Logger.LogLevel.Internal);

        Options.Scale = new Vector2(scale);
        Options.Size = _font.MeasureString(_textOptions.Content);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Logger.Log($"drawing button {Id} pos:{GlobalPosition} scale:{Options.Scale} size:{Options.Size}");
        spriteBatch.DrawString(_font, _textOptions.Content, GlobalPosition, Color.Red, 0f, Vector2.Zero,
            Options.Scale,
            SpriteEffects.None, 0.5f);
    }

    private static float CalculateScale(int from, int to)
    {
        return (float)Math.Pow(1.2, Math.Log(to / (double)from, 1.2));
    }

    public override void Update()
    {
    }

    public Vector2 GetSize()
    {
        return _font.MeasureString(_textOptions.Content);
    }

    public class TextOptions()
    {
        public string Content = "";
        public int Size = 12;

        public Color Color;
        // public TextAlignment Alignment = TextAlignment.Center;`
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }
}