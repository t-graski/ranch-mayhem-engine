using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.UI;

public class TypeWriterText : UiComponent
{
    private readonly TypeWriterTextOptions _typeWriterTextOptions;
    public bool IsWriting = false;
    public bool IsDoneWriting = false;
    public bool WasForceCompleted = false;
    private string _originalText;
    private string _fullText = string.Empty;
    private int _visibleCharCount = 0;
    private float _typeWriterTimer = 0f;
    private float _typeWriterHoldMs;

    private Text _text;
    private Container _container;

    public TypeWriterText(
        string id, TypeWriterTextOptions options, UiComponent? parent = null, bool scale = true, Effect? renderShader = null
    ) : base(id, options, parent, scale, renderShader)
    {
        _typeWriterTextOptions = options;
        _originalText = _typeWriterTextOptions.Content;
        Initialize();
    }

    private void Initialize()
    {
        _text = new TextBuilder($"{Id}-inner-text")
            .SetContent(_typeWriterTextOptions.Content)
            .SetFontColor(_typeWriterTextOptions.FontColor)
            .SetFontSize(_typeWriterTextOptions.FontSize)
            .SetPosition(new Vector2(4, 4))
            .Build();

        var lines = _typeWriterTextOptions.Content.Split('\n');
        var longestLineSize = new Vector2(0, 0);
        foreach (var line in lines)
        {
            var lineSize = _text.Font.MeasureString(line);
            if (lineSize.X > longestLineSize.X)
            {
                longestLineSize = lineSize;
            }
        }

        _container = new ContainerBuilder($"{Id}-inner-container")
            .SetSize(new Vector2(longestLineSize.X + 8, (longestLineSize.Y * (lines.Length - 1)) + 8))
            .SetColor(_typeWriterTextOptions.Color)
            .SetBorderColor(_typeWriterTextOptions.BorderColor)
            .SetBorderPosition(_typeWriterTextOptions.BorderPosition)
            .SetBorderSize(_typeWriterTextOptions.BorderSize)
            .SetUiAnchor(_typeWriterTextOptions.UiAnchor)
            .SetUiAnchorOffSet(_typeWriterTextOptions.UiAnchorOffset)
            .SetChildren([_text])
            .Build();

        Options.Size = _container.Options.Size;
        Bounds = new Rectangle((int)_container.LocalPosition.X, (int)_container.LocalPosition.Y, (int)Options.Size.X, (int)Options.Size.Y);
    }

    public void StartWriting()
    {
        ResetWriter();
        IsWriting = true;
    }

    private void ResetWriter()
    {
        _fullText = _originalText;
        _text.ClearContent();
        _visibleCharCount = 0;
        _typeWriterTimer = 0f;
        IsDoneWriting = false;
        WasForceCompleted = false;
    }

    public void Complete()
    {
        _text.SetContent(_fullText);
        IsDoneWriting = true;
        IsWriting = false;
        WasForceCompleted = true;
    }

    public override void Update()
    {
        HandleMouse(RanchMayhemEngine.MouseState);

        if (!IsWriting) return;

        var deltaMs = (float)RanchMayhemEngine.GameTime.ElapsedGameTime.TotalMilliseconds;

        if (_typeWriterTextOptions.PauseAfterSentence && _typeWriterHoldMs > 0f)
        {
            _typeWriterHoldMs -= deltaMs;
            if (_typeWriterHoldMs < 0f)
            {
                _typeWriterHoldMs = 0f;
            }
        }
        else
        {
            _typeWriterTimer += deltaMs;
            while (_typeWriterTimer >= _typeWriterTextOptions.TypeWriterInterval && _visibleCharCount < _fullText.Length)
            {
                _typeWriterTimer -= _typeWriterTextOptions.TypeWriterInterval;
                _visibleCharCount++;
                _text.SetContent(_fullText[.._visibleCharCount]);

                var justRevealed = _fullText[_visibleCharCount - 1];
                if (_typeWriterTextOptions.PauseAfterSentence && justRevealed == '.')
                {
                    _typeWriterHoldMs = _typeWriterTextOptions.SentencePauseMs;
                    break;
                }
            }
        }

        if (IsWriting && _visibleCharCount == _fullText.Length)
        {
            IsWriting = false;
            IsDoneWriting = true;
        }
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        return _container.Draw();
    }
}

public sealed class TypeWriterTextOptions : Text.TextOptions
{
    public int TypeWriterInterval = 25;
    public bool PauseAfterSentence = false;
    public int SentencePauseMs = 300;
}
