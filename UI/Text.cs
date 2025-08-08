using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.Content;
using System;

namespace ranch_mayhem_engine.UI;

public class Text : UiComponent
{
    private readonly TextOptions _textOptions;
    private SpriteFont _font;

    public Text(
        string id, TextOptions options, UiComponent? parent = null,
        bool scale = true
    ) : base(
        id,
        options,
        parent,
        scale
    )
    {
        _textOptions = options;
        InitializeFont();
    }

    private void InitializeFont()
    {
        var fontWithSize =
            ContentManager.GetFontWithSize(
                RanchMayhemEngineConstants.DefaultFont,
                _textOptions.FontSize
            );
        _font = fontWithSize.font;

        var scale = 1.0f;

        if (fontWithSize.size != _textOptions.FontSize)
        {
            scale = CalculateScale(fontWithSize.size, _textOptions.FontSize);
        }

        Options.Scale = new Vector2(scale) * RanchMayhemEngine.UiManager.GlobalScale;
        Options.Size = _font.MeasureString(_textOptions.Content) * Options.Scale;

        // Logger.Log(
        //     $"{GetType().FullName}::InitializeFont Id={Id} Given size: {_textOptions.FontSize}, Found size: {fontWithSize.size}, Using scale: {scale}",
        //     Logger.LogLevel.Internal);

        UpdateGlobalPosition();
    }

    public override void SetParent(UiComponent parent)
    {
        Parent = parent;
        RecalculateSize();
        UpdateGlobalPosition();
    }

    private void RecalculateSize()
    {
        Options.Size = _font.MeasureString(_textOptions.Content) * Options.Scale;
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        if (_textOptions.Shadow)
        {
            yield return new RenderCommand
            {
                Id = $"{Id}-text",
                SpriteFont = _font,
                Text = _textOptions.Content,
                Position = new Vector2(GlobalPosition.X + 2, GlobalPosition.Y + 2),
                Color = _textOptions.ShadowColor,
                Rotation = 0f,
                Origin = Vector2.Zero,
                Scale = Options.Scale,
                Effects = SpriteEffects.None,
                LayerDepth = 0.5f
            };

            // spriteBatch.DrawString(
            //     _font,
            //     _textOptions.Content,
            //     new Vector2(GlobalPosition.X + 2, GlobalPosition.Y + 2),
            //     _textOptions.ShadowColor,
            //     0f,
            //     Vector2.Zero,
            //     Options.Scale,
            //     SpriteEffects.None,
            //     0.5f
            // );
        }

        yield return new RenderCommand
        {
            SpriteFont = _font,
            Text = _textOptions.Content,
            Position = GlobalPosition,
            Color = _textOptions.FontColor,
            Rotation = 0f,
            Origin = Vector2.Zero,
            Scale = Options.Scale,
            Effects = SpriteEffects.None,
            LayerDepth = 0.5f
        };

        // spriteBatch.DrawString(
        //     _font,
        //     _textOptions.Content,
        //     GlobalPosition,
        //     _textOptions.FontColor,
        //     0f,
        //     Vector2.Zero,
        //     Options.Scale,
        //     SpriteEffects.None,
        //     0.5f
        // );

        foreach (var command in DrawBorder())
        {
            yield return command;
        }

        // DrawBorder(spriteBatch);
    }

    private static float CalculateScale(int from, int to)
    {
        return (float)Math.Pow(1.2, Math.Log(to / (double)from, 1.2));
    }

    public override void Update()
    {
    }

    public void SetContent(string content, bool wrap = false)
    {
        if (_textOptions.Content.Equals(content)) return;

        _textOptions.Content = content;

        RecalculateSize();
        UpdateGlobalPosition();

        if (!FitsParent() && wrap && Parent is not null)
        {
            Logger.Log($"{Id} is too big to fit in parent", LogLevel.Warning);

            // we have 2 ways of dealing wit this
            // - First we try and split the text at spaces
            // - If this doesn't work we make the text smaller
            // - If this doesn't work either we just leave it
            var possibleLines = (int)(Parent!.Options.Size.Y / Options.Size.Y);

            var whiteSpaceAmount = _textOptions.Content.Count(char.IsWhiteSpace);
            var splittingPossible = whiteSpaceAmount > 0 && whiteSpaceAmount <= possibleLines && possibleLines > 1;

            Logger.Log($"{Id} splitting... spacesCount={whiteSpaceAmount} possibleLines={possibleLines}");

            if (splittingPossible)
            {
                Logger.Log($"{Id} splitting possible... trying to split", LogLevel.Internal);
                _textOptions.Content = _textOptions.Content.Replace(" ", "\n");
                RecalculateSize();
                UpdateGlobalPosition();

                if (FitsParent())
                {
                    Logger.Log($"{Id} splitting successful", LogLevel.Internal);
                }
            }
            else
            {
                Logger.Log($"{Id} splitting not possible... trying to scale down", LogLevel.Internal);
            }
        }
    }

    private bool FitsParent()
    {
        if (Parent is null) return true;
        return Parent.Bounds.Contains(
            new Vector2(
                GlobalPosition.X + Options.Size.X,
                GlobalPosition.Y + Options.Size.Y
            )
        );
    }

    public void SetTextColor(Color color)
    {
        _textOptions.FontColor = color;
    }


    public string GetContent() => _textOptions.Content;

    public Vector2 GetSize()
    {
        return _font.MeasureString(_textOptions.Content);
    }

    public class TextOptions : UiComponentOptions
    {
        public string Content = "";
        public int FontSize = 12;

        public Color FontColor = Color.Red;
        public bool Shadow = true;

        public Color ShadowColor = Color.Black;
        // public TextAlignment Alignment = TextAlignment.Center;`
    }
}
