using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI.Helper;

public class TextBuilder : UiComponentBuilder<TextBuilder>
{
    private readonly Text.TextOptions _textOptions;

    public TextBuilder(string id) : base(id, new Text.TextOptions())
    {
        _textOptions = (Text.TextOptions)_componentOptions;
    }

    public TextBuilder SetContent(string content)
    {
        _textOptions.Content = content;
        return this;
    }

    public TextBuilder SetFontSize(int fontSize)
    {
        _textOptions.FontSize = fontSize;
        return this;
    }

    public TextBuilder SetFontColor(Color fontColor)
    {
        _textOptions.FontColor = fontColor;
        return this;
    }

    public Text Build()
    {
        return new Text(_id, _textOptions, _parent);
    }
}