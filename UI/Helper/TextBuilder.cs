using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI.Helper;

public class TextBuilder(string id)
{
    private readonly Text.TextOptions _textOptions = new();
    private UiComponent _parent;

    public TextBuilder SetParent(UiComponent parent)
    {
        _parent = parent;
        return this;
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

    public TextBuilder SetUiAnchor(UiAnchor anchor)
    {
        _textOptions.UiAnchor = anchor;
        return this;
    }

    public TextBuilder SetPosition(Vector2 position)
    {
        _textOptions.Position = position;
        return this;
    }

    public TextBuilder SetPosition(float x, float y) => SetPosition(new Vector2(x, y));

    public TextBuilder SetUiAnchorOffset(Vector2 offset)
    {
        _textOptions.UiAnchorOffset = offset;
        return this;
    }

    public TextBuilder SetUiAnchorOffSet(float x, float y) => SetUiAnchorOffset(new Vector2(x, y));

    public TextBuilder CenterXY()
    {
        _textOptions.UiAnchor = UiAnchor.CenterX | UiAnchor.CenterY;
        return this;
    }

    public Text Build()
    {
        return new Text(id, _textOptions, _parent);
    }
}