using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.UI;

public class Box : UiComponent
{
    public Text? Text;

    public Box(string id, BoxOptions options, UiComponent? parent = null, bool scale = true) : base(
        id,
        options,
        parent,
        scale
    )
    {
    }

    public class BoxOptions : UiComponentOptions
    {
    }

    public void SetText(string text, Color color, int size = 16)
    {
        if (text.Length == 0)
        {
            Text = null;
            return;
        }

        if (Text == null)
        {
            Text = new TextBuilder($"{Id}-inner-text")
                .SetContent(text)
                .SetUiAnchor(UiAnchor.CenterX | UiAnchor.CenterY)
                .SetFontColor(color)
                .SetFontSize(size)
                .Build();

            Text.SetParent(this);
            return;
        }

        if (Text.GetContent().Equals(text))
        {
            return;
        }

        Text.SetContent(text);
        Text.SetTextColor(color);
    }

    public void SetTextColor(Color color)
    {
        Text?.SetTextColor(color);
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        foreach (var command in base.Draw())
        {
            yield return command;
        }

        if (Text is null) yield break;
        foreach (var command in Text?.Draw())
        {
            yield return command;
        }
    }

    public override void Update()
    {
    }
}
