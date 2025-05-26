using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ranch_mayhem_engine.UI;

public class ProgressBar : UiComponent
{
    private readonly ProgressBarOptions _progressBarOptions;
    private Color _currentColor;
    private readonly Text _progressText;

    public ProgressBar(string id, ProgressBarOptions options, UiComponent? parent = null, bool scale = true) : base(id,
        options, parent, scale)
    {
        _progressBarOptions = options;
        UpdateColor(_progressBarOptions.Fraction);

        _progressText = new Text($"{Id}-text", new Text.TextOptions
        {
            FontColor = Color.Black,
            FontSize = 12,
            Content = _progressBarOptions.Content,
            UiAnchor = UiAnchor.CenterX | UiAnchor.CenterY
        });
        _progressText.SetParent(this);
    }

    // public override void Draw(SpriteBatch spriteBatch)
    // {
    //     var texture = new Texture2D(RanchMayhemEngine.UIManager.GraphicsDevice, 1, 1);
    //     texture.SetData([_currentColor]);
    //
    //     spriteBatch.Draw(texture,
    //         new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y,
    //             (int)(Options.Size.X * MathHelper.Clamp(_progressBarOptions.Fraction, 0, 1)), (int)Options.Size.Y),
    //         _currentColor);
    //
    //     _progressText.Draw(spriteBatch);
    //
    //     DrawBorder(spriteBatch);
    // }

    public override void SetParent(UiComponent parent)
    {
        base.SetParent(parent);
        _progressText.SetParent(this);
    }

    public void SetFraction(float fraction)
    {
        _progressBarOptions.Fraction = MathHelper.Clamp(fraction, 0, 1);
        UpdateColor(fraction);
    }

    private void UpdateColor(float fraction)
    {
        foreach (var (key, value) in _progressBarOptions.Thresholds.Reverse())
        {
            if (fraction >= key)
            {
                _currentColor = value;
                return;
            }
        }

        _currentColor = Color.Black;
    }

    public float GetFraction() => _progressBarOptions.Fraction;

    public void SetContent(string content) => _progressBarOptions.Content = content;

    public override void Update()
    {
        _progressText.SetContent(_progressBarOptions.Content ?? "");
    }


    public class ProgressBarOptions : UiComponentOptions
    {
        public float Fraction;
        public Dictionary<float, Color> Thresholds;
        public string? Content;
    }
}
