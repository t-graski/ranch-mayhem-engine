using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.UI;

public class Box : UiComponent
{
    private Text? _text;

    public Box(string id, BoxOptions options, UiComponent? parent = null, bool scale = true) : base(id, options,
        parent, scale)
    {
    }

    public class BoxOptions : UiComponentOptions
    {
    }

    public void SetText(string text, Color color)
    {
        if (text.Length == 0)
        {
            _text = null;
            return;
        }

        if (_text == null)
        {
            _text = new TextBuilder($"{Id}-inner-text")
                .SetContent(text)
                .SetUiAnchor(UiAnchor.CenterX | UiAnchor.CenterY)
                .SetFontColor(color)
                .SetFontSize(16)
                .Build();

            _text.SetParent(this);
            return;
        }

        if (_text.GetContent().Equals(text))
        {
            return;
        }

        _text.SetContent(text);
        _text.SetColor(color);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        _text?.Draw(spriteBatch);
    }

    public override void Update()
    {
    }
}