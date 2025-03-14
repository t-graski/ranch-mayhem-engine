using Microsoft.Xna.Framework;

namespace ranch_mayhem_engine.UI.Helper;

public class TextBuilder
{
    private Text _text;
    private Text.TextOptions _textOptions;
    private string _id;
    private UIComponent _parent;

    public TextBuilder(string id)
    {
        _id = id;
        _textOptions = new Text.TextOptions();
    }

    public TextBuilder SetParent(UIComponent parent)
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

    public TextBuilder SetUiAnchor(UIAnchor anchor)
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
        _textOptions.UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY;
        return this;
    }

    public Text Build()
    {
        return new Text(_id, _textOptions, _parent);
    }
}