using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ranch_mayhem_engine.UI;

public class ProgressBar : UiComponent
{
    private readonly ProgressBarOptions _progressBarOptions;
    private Color _currentColor;
    private string _currentText;
    private readonly Text _progressText;

    private double _currentFraction = 0;


    public ProgressBar(string id, ProgressBarOptions options, UiComponent? parent = null, bool scale = true) : base(
        id,
        options,
        parent,
        scale
    )
    {
        _progressBarOptions = options;
        _currentFraction = _progressBarOptions.Fraction;
        UpdateColor(_progressBarOptions.Fraction);

        _progressText = new Text(
            $"{Id}-text",
            new Text.TextOptions
            {
                FontColor = Color.White,
                FontSize = 12,
                UiAnchor = UiAnchor.CenterX | UiAnchor.CenterY
            }
        );

        _progressText.SetParent(this);
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        var texture = new Texture2D(RanchMayhemEngine.UiManager.GraphicsDevice, 1, 1);
        texture.SetData([_currentColor]);
        yield return new RenderCommand
        {
            Id = $"{Id}-progress-bar",
            Texture = texture,
            DestinationRect = new Rectangle(
                (int)GlobalPosition.X,
                (int)GlobalPosition.Y,
                (int)Options.Size.X,
                (int)Options.Size.Y
            ),
            Color = _currentColor
        };

        // spriteBatch.Draw(
        //     texture,
        //     new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, (int)Options.Size.X, (int)Options.Size.Y),
        //     _currentColor
        // );

        foreach (var command in DrawTiledTexture(
                     _progressBarOptions.ProgressTexture,
                     GlobalPosition,
                     new Vector2(Options.Size.X * (float)_currentFraction, Options.Size.Y),
                     true,
                     true
                 ))
        {
            yield return command;
        }

        foreach (var command in _progressText.Draw())
        {
            yield return command;
        }

        // _progressText.Draw(spriteBatch);

        foreach (var command in DrawBorder())
        {
            yield return command;
        }

        // DrawBorder(spriteBatch);
    }

    public override void SetParent(UiComponent parent)
    {
        base.SetParent(parent);
        _progressText.SetParent(this);
    }

    public void SetFraction(float fraction)
    {
        _progressBarOptions.Fraction = MathHelper.Clamp(fraction, 0, 1);
        if (!_progressBarOptions.SmoothUpdate) _currentFraction = _progressBarOptions.Fraction;
        UpdateColor(fraction);
        UpdateText(fraction);
    }

    private void UpdateColor(double fraction)
    {
        foreach (var (key, value) in _progressBarOptions.ColorThresholds.Reverse())
        {
            if (fraction >= key)
            {
                _currentColor = value;
                return;
            }
        }

        _currentColor = Color.Black;
    }

    private void UpdateText(double fraction)
    {
        foreach (var (key, value) in _progressBarOptions.TextThresholds.Reverse())
        {
            if (fraction >= key)
            {
                _currentText = value;
                return;
            }
        }
    }

    public double GetFraction() => _currentFraction;

    public void SetContent(string content) => _progressBarOptions.Content = content;

    public void SetTextThresholds(Dictionary<float, string> thresholds) => _progressBarOptions.TextThresholds = thresholds;
    public void SetColorThresholds(Dictionary<float, Color> thresholds) => _progressBarOptions.ColorThresholds = thresholds;

    public override void Update()
    {
        _progressText.SetContent(_currentText ?? "");

        if (!_progressBarOptions.SmoothUpdate) return;

        _currentFraction = MathHelper.Lerp(
            (float)_currentFraction,
            (float)_progressBarOptions.Fraction,
            _progressBarOptions.SmoothUpdateSpeed
        );

        if (Math.Abs(_currentFraction - _progressBarOptions.Fraction) < 0.001f)
        {
            _currentFraction = _progressBarOptions.Fraction;
        }
    }

    public class ProgressBarOptions : UiComponentOptions
    {
        public double Fraction;
        public Dictionary<float, Color> ColorThresholds;
        public Dictionary<float, string> TextThresholds;
        public string? Content;

        public Texture2D ProgressTexture;
        public bool SmoothUpdate;
        public float SmoothUpdateSpeed;
    }
}
