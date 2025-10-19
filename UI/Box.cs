using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.Content;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.UI;

public class Box : UiComponent
{
    private Text? _text;

    public Box(string id, BoxOptions options, UiComponent? parent = null, bool scale = true,
        Effect? renderShader = null) : base(
        id,
        options,
        parent,
        scale,
        renderShader
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
            _text = null;
            return;
        }

        if (_text == null)
        {
            _text = new TextBuilder($"{Id}-inner-text")
                .SetContent(text)
                .SetUiAnchor(UiAnchor.CenterX | UiAnchor.CenterY)
                .SetFontColor(color)
                .SetFontSize(size)
                .Build();

            _text.SetParent(this);
            return;
        }

        if (_text.GetContent().Equals(text))
        {
            return;
        }

        _text.SetContent(text);
        _text.SetTextColor(color);
    }

    public void SetTextColor(Color color)
    {
        _text?.SetTextColor(color);
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        foreach (var command in base.Draw())
        {
            yield return command;
        }

        if (_text is null) yield break;
        foreach (var command in _text?.Draw())
        {
            yield return command;
        }
    }

    public override void ToggleAnimating()
    {
        base.ToggleAnimating();
        _text?.ToggleAnimating();
    }

    public override void SetRenderShader(Effect shader)
    {
        base.SetRenderShader(shader);
        _text?.SetRenderShader(shader);
    }

    public override void HandleParentGlobalPositionChange(Vector2 position)
    {
        base.HandleParentGlobalPositionChange(position);
        _text?.HandleParentGlobalPositionChange(position);
    }

    public override void Update()
    {
    }
}