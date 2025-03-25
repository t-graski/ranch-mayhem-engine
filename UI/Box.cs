using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.UI;

public class Box : UIComponent
{
    private Text _text;

    public Box(string id, BoxOptions options, UIComponent parent = null, bool scale = true) : base(id, options,
        parent, scale)
    {
    }

    public class BoxOptions : UIComponentOptions
    {
    }

    public void SetText(string text, Color color)
    {
        if (text.Length == 0)
        {
            Logger.Log($"Setting text 1");
            _text = null;
            return;
        }

        if (_text == null)
        {
            Logger.Log($"Setting text 2");
            _text = new TextBuilder($"{Id}-inner-text")
                .SetContent(text)
                .SetUiAnchor(UIAnchor.CenterX | UIAnchor.CenterY)
                .SetFontColor(color)
                .SetFontSize(16)
                .Build();

            _text.SetParent(this);

            Logger.Log($"text global pos {_text.GlobalPosition} size {_text.GetSize()} parent global {GlobalPosition}");

            return;
        }

        if (_text.GetContent().Equals(text))
        {
            Logger.Log($"Setting text 3");
            return;
        }

        Logger.Log($"Setting text 4");
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