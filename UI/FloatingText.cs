using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.UI;

public class FloatingText : UiComponent
{
    private Text _text;
    private FloatingTextOptions _options;


    public bool Done => _options.Age >= _options.Duration;

    public FloatingText(string id, FloatingTextOptions options, UiComponent? parent = null, bool scale = true,
        Effect? renderShader = null) : base(id, options, parent, scale, renderShader)
    {
        _options = options;

        _text = new TextBuilder(id)
            .SetContent(_options.Content)
            .SetFontColor(_options.FontColor)
            .SetPosition(Bezier(_options.Start, _options.Control, _options.End, 0f))
            .SetFontSize(16)
            .Build();
    }

    private static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        var u = 1f - t;
        return (u * u) * a + 2f * u * t * b + (t * t) * c;
    }

    public override void Update()
    {
        IsVisible = true;
        _options.Age += (float)RanchMayhemEngine.GameTime.ElapsedGameTime.TotalMilliseconds;
        var t = Math.Clamp(_options.Age / _options.Duration, 0f, 1f);

        var pos = Bezier(_options.Start, _options.Control, _options.End, t);
        _text.SetPosition(pos);

        var alpha = 1f - t;
        var color = _text._textOptions.FontColor;

        _text.SetTextColor(new Color(color.R, color.G, color.B, (byte)(255 * (1f - t))));
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        if (IsVisible)
        {
            UiManager.Enqueue(_text.Draw());
        }

        return base.Draw();
    }
}

public sealed class FloatingTextOptions : Text.TextOptions
{
    public Vector2 Start;
    public Vector2 End;
    public Vector2 Control;
    public float Age;
    public float Duration;
    public float StartScale = 1.0f;
    public float EndScale = 0.8f;
}