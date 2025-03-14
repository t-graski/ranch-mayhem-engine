﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class Text : UIComponent
{
    private readonly TextOptions _textOptions;
    private SpriteFont _font;

    public Text(string id, TextOptions options, UIComponent parent = null,
        bool scale = true) : base(id, options,
        parent, scale)
    {
        _textOptions = options;
        InitializeFont();
    }

    private void InitializeFont()
    {
        var fontWithSize =
            RanchMayhemEngine.ContentManager.GetFontWithSize(RanchMayhemEngineConstants.DefaultFont,
                _textOptions.FontSize);
        _font = fontWithSize.font;

        var scale = 1.0f;

        if (fontWithSize.size != _textOptions.FontSize)
        {
            scale = CalculateScale(fontWithSize.size, _textOptions.FontSize);
        }

        Options.Scale = new Vector2(scale);
        Options.Size = _font.MeasureString(_textOptions.Content);


        // Logger.Log(
        //     $"{GetType().FullName}::InitializeFont Id={Id} Given size: {_textOptions.FontSize}, Found size: {fontWithSize.size}, Using scale: {scale}",
        //     Logger.LogLevel.Internal);
    }

    public override void SetParent(UIComponent parent)
    {
        Parent = parent;
        RecalculateSize();
        UpdateGlobalPosition();
    }

    private void RecalculateSize()
    {
        Options.Size = _font.MeasureString(_textOptions.Content);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, _textOptions.Content, GlobalPosition, _textOptions.FontColor, 0f, Vector2.Zero,
            Options.Scale,
            SpriteEffects.None, 0.5f);

        DrawBorder(spriteBatch);
    }

    private static float CalculateScale(int from, int to)
    {
        return (float)Math.Pow(1.2, Math.Log(to / (double)from, 1.2));
    }

    public override void Update()
    {
    }

    public void SetContent(string content)
    {
        if (_textOptions.Content.Equals(content)) return;

        _textOptions.Content = content;

        RecalculateSize();
        UpdateGlobalPosition();
    }

    public Vector2 GetSize()
    {
        return _font.MeasureString(_textOptions.Content);
    }

    public class TextOptions : UIComponentOptions
    {
        public string Content = "";
        public int FontSize = 12;

        public Color FontColor = Color.Red;
        // public TextAlignment Alignment = TextAlignment.Center;`
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }
}