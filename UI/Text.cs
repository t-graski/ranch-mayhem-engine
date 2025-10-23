using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.Content;

namespace ranch_mayhem_engine.UI;

public class Text : UiComponent
{
    public readonly TextOptions _textOptions;
    public SpriteFont Font;

    private List<DrawRun> _layout = [];
    private bool _dirty = true;

    public Text(
        string id, TextOptions options, UiComponent? parent = null, bool scale = true) : base(id, options, parent,
        scale)
    {
        _textOptions = options;
        InitializeFont();
    }

    private void InitializeFont()
    {
        var fontWithSize =
            ContentManager.GetFontWithSize(RanchMayhemEngineConstants.DefaultFont, _textOptions.FontSize);
        Font = fontWithSize.font;

        var scale = 1.0f;

        if (fontWithSize.size != _textOptions.FontSize)
        {
            scale = CalculateScale(fontWithSize.size, _textOptions.FontSize);
        }

        Options.Scale = new Vector2(scale) * RanchMayhemEngine.UiManager.GlobalScale;

        // Options.Size = Font.MeasureString(_textOptions.Content) * Options.Scale;
        // Logger.Log(
        //     $"{Id} Font.LineSpacing: {Font.LineSpacing}, FonSize: {_textOptions.FontSize}, Scale: {Options.Scale} WSize: {RanchMayhemEngine.WindowedSize}");

        _dirty = true;
        UpdateGlobalPosition();
    }

    public override void SetParent(UiComponent parent)
    {
        Parent = parent;
        _dirty = true;
        // RecalculateSize();
        UpdateGlobalPosition();
    }

    private void EnsureLayout()
    {
        if (!_dirty) return;

        var content = _textOptions.Content;
        var defaultStyle = new Style
        {
            Color = ApplyOpacity(_textOptions.FontColor),
            Bold = false
        };

        var tokens = Tokenize(content);
        var runs = Styleize(tokens, defaultStyle);

        // for (var i = 0; i < tokens.Count; i++)
        // {
        //     if (tokens[i].Kind == TokenKind.LineBreak)
        //     {
        //         runs.Add(new DrawRun { Text = "\n", Style = defaultStyle, IsWhitespace = false });
        //     }
        // }

        var oldSize = Options.Size;
        _layout = Layout(runs, GetMaxWidth(), lineSpacingScale: 1);

        var bounds = MeasureLayoutBounds(_layout);
        var unscaledSize = bounds.Size();
        Options.Size = unscaledSize * Options.Scale;

        if (Options.UiAnchor != UiAnchor.None && Options.Position == Vector2.Zero && Options.Size != Vector2.Zero)
        {
            var virtualParent = new Vector2(-1);
            LocalPosition = Options.UiAnchor.CalculatePosition(Options.Size, virtualParent, Parent);
            if (Options.UiAnchorOffset != Vector2.Zero)
                LocalPosition += Options.UiAnchorOffset;
            // UpdateBounds(Parent);
            UpdateGlobalPosition();
        }

        _dirty = false;
    }

    private float GetMaxWidth()
    {
        if (Parent is null || !_textOptions.AutoWrap) return float.PositiveInfinity;
        return Parent.Options.Size.X / Options.Scale.X;
    }

    private RectangleF MeasureLayoutBounds(List<DrawRun> layout)
    {
        if (layout.Count == 0) return new RectangleF(0, 0, 0, Font.LineSpacing);

        float maxX = 0, maxY = Font.LineSpacing;
        foreach (var r in layout)
        {
            if (r.Text == "\n") continue;
            var size = Font.MeasureString(r.Text);
            var x2 = r.Position.X + size.X;
            var y2 = r.Position.Y + size.Y;
            if (x2 > maxX) maxX = x2;
            if (y2 > maxY) maxY = y2;
        }

        return new RectangleF(0, 0, maxX, maxY);
    }

    private void RecalculateSize()
    {
        Options.Size = Font.MeasureString(_textOptions.Content) * Options.Scale;
    }

    public override IEnumerable<RenderCommand> Draw()
    {
        // if (_textOptions.Shadow)
        // {
        //     yield return new RenderCommand
        //     {
        //         Id = $"{Id}-text-shadow",
        //         SpriteFont = Font,
        //         Text = _textOptions.Content,
        //         Position = new Vector2(GlobalPosition.X + 2, GlobalPosition.Y + 2),
        //         Color = _textOptions.ShadowColor,
        //         Rotation = 0f,
        //         Origin = Vector2.Zero,
        //         Scale = Options.Scale,
        //         Effects = SpriteEffects.None,
        //         LayerDepth = 0.5f
        //     };
        // }
        //
        // yield return new RenderCommand
        // {
        //     Id = $"{Id}-text",
        //     SpriteFont = Font,
        //     Text = _textOptions.Content,
        //     Position = GlobalPosition,
        //     Color = ApplyOpacity(_textOptions.FontColor),
        //     Rotation = 0f,
        //     Origin = Vector2.Zero,
        //     Scale = Options.Scale,
        //     Effects = SpriteEffects.None,
        //     LayerDepth = 0.5f,
        //     Shader = RenderShader
        // };
        //

        EnsureLayout();

        if (_textOptions.Shadow)
        {
            foreach (var run in _layout)
            {
                yield return new RenderCommand
                {
                    Id = $"{Id}-shadow-{run.Id}",
                    SpriteFont = Font,
                    Text = run.Text,
                    Position = new Vector2(GlobalPosition.X + (2 * Options.Scale.X),
                        GlobalPosition.Y + (2 * Options.Scale.Y)) + (run.Position * Options.Scale),
                    Color = _textOptions.ShadowColor,
                    Rotation = 0f,
                    Origin = Vector2.Zero,
                    Scale = Options.Scale,
                    Effects = SpriteEffects.None,
                    LayerDepth = 0.5f
                };
            }
        }

        foreach (var run in _layout)
        {
            yield return MakeCommand(run, GlobalPosition, Options.Scale, 0, 0);
        }

        foreach (var command in DrawBorder())
        {
            yield return command;
        }
    }

    private RenderCommand MakeCommand(DrawRun run, Vector2 globalPos, Vector2 scale, float ox, float oy)
    {
        return new RenderCommand
        {
            Id = $"{Id}-run-{run.Id}-{ox}-{oy}",
            SpriteFont = Font,
            Text = run.Text,
            Position = new Vector2(globalPos.X + (ox * scale.X), globalPos.Y + (oy * scale.Y)) +
                       (run.Position * scale),
            Color = run.Style.Color,
            Rotation = 0f,
            Origin = Vector2.Zero,
            Scale = scale,
            Effects = SpriteEffects.None,
            LayerDepth = 0.5f,
            Shader = RenderShader
        };
    }

    private static float CalculateScale(int from, int to)
    {
        // return (float)Math.Pow(1.2, Math.Log(to / (double)from, 1.2));
        // var f = to / (float)from;
        // var k = Math.Max(1, (int)MathF.Round(f));
        // return k;
        return to / (float)from;
    }

    public override void Update()
    {
    }

    public void SetContent(string content, bool wrap = false)
    {
#if DEBUG
        if (string.IsNullOrEmpty(content))
        {
            Logger.Log(
                $"{GetType().FullName}::SetContent Id={Id} Consider using Text::ClearContent to set an empty string as content",
                LogLevel.Warning
            );
        }
#endif
        if (_textOptions.Content.Equals(content)) return;

        _textOptions.Content = content;
        if (wrap) _textOptions.AutoWrap = true;

        // RecalculateSize();
        _dirty = true;
        UpdateGlobalPosition();

        // if (wrap && !FitsParent() && Parent is not null)
        // {
        //     Logger.Log($"{Id} is too big to fit in parent Content={content}", LogLevel.Warning);
        //
        //     // we have 2 ways of dealing wit this
        //     // - First we try and split the text at spaces
        //     // - If this doesn't work we make the text smaller
        //     // - If this doesn't work either we just leave it
        //     var possibleLines = (int)Math.Round(Parent!.Options.Size.Y / Options.Size.Y);
        //
        //     var whiteSpaceAmount = _textOptions.Content.Count(char.IsWhiteSpace);
        //     var splittingPossible = whiteSpaceAmount > 0 && whiteSpaceAmount <= possibleLines && possibleLines > 1;
        //
        //     Logger.Log($"{Id} splitting... spacesCount={whiteSpaceAmount} possibleLines={possibleLines}");
        //
        //     if (splittingPossible)
        //     {
        //         Logger.Log($"{Id} splitting possible... trying to split", LogLevel.Internal);
        //         _textOptions.Content = _textOptions.Content.Replace(" ", "\n");
        //         RecalculateSize();
        //         UpdateGlobalPosition();
        //
        //         if (FitsParent())
        //         {
        //             Logger.Log($"{Id} splitting successful", LogLevel.Internal);
        //         }
        //     }
        //     else
        //     {
        //         Logger.Log($"{Id} splitting not possible... trying to scale down", LogLevel.Internal);
        //     }
        // }
    }


    public void ClearContent()
    {
        _textOptions.Content = string.Empty;
        _dirty = true;
    }

    private bool FitsParent()
    {
        if (Parent is null) return true;
        return Parent.Bounds.Contains(
            new Vector2(
                GlobalPosition.X + Options.Size.X,
                GlobalPosition.Y + Options.Size.Y
            )
        );
    }

    public void SetTextColor(Color color)
    {
        _textOptions.FontColor = color;
    }


    public string GetContent() => _textOptions.Content;

    public Vector2 GetSize()
    {
        EnsureLayout();
        return Options.Size;
    }

    public Vector2 GetUnscaledSize()
    {
        EnsureLayout();
        var bounds = MeasureLayoutBounds(_layout);
        return bounds.Size();
    }

    public class TextOptions : UiComponentOptions
    {
        public string Content = "";
        public int FontSize = 12;

        public Color FontColor = Color.Red;
        public bool Shadow = true;

        public Color ShadowColor = Color.Black;

        public bool AutoWrap = false;
        // public TextAlignment Alignment = TextAlignment.Center;`
    }

    private readonly struct Style
    {
        public Color Color { get; init; }
        public bool Bold { get; init; }

        public Style WithColor(Color c) => new Style { Color = c, Bold = Bold };
        public Style WithBold(bool b) => new Style { Color = Color, Bold = b };
    }

    private sealed class DrawRun
    {
        private static int _nextId;
        public string Id { get; } = (++_nextId).ToString();
        public string Text = string.Empty;
        public Vector2 Position;
        public Style Style;
        public bool IsWhitespace;
    }

    private enum TokenKind
    {
        Word,
        Space,
        Tag,
        ScopedTag,
        LineBreak
    }

    private readonly struct Token
    {
        public TokenKind Kind { get; init; }
        public string Text { get; init; }
        public string TagPayload { get; init; }
    }

    private static List<Token> Tokenize(string s)
    {
        var result = new List<Token>(64);
        int i = 0;
        while (i < s.Length)
        {
            // hard break
            if (s[i] == '\n')
            {
                result.Add(new Token { Kind = TokenKind.LineBreak, Text = "\n" });
                i++;
                continue;
            }

            if (s[i] == '{')
            {
                int end = s.IndexOf('}', i + 1);
                if (end > i)
                {
                    var inside = s.Substring(i + 1, end - i - 1);
                    var scopedSep = inside.IndexOf(':');
                    if (scopedSep >= 0)
                    {
                        var payload = inside.Substring(0, scopedSep).Trim();
                        var scopedText = inside.Substring(scopedSep + 1);
                        result.Add(new Token { Kind = TokenKind.ScopedTag, TagPayload = payload, Text = scopedText });
                        i = end + 1;
                        continue;
                    }
                    else
                    {
                        result.Add(new Token { Kind = TokenKind.Tag, TagPayload = inside.Trim(), Text = "" });
                        i = end + 1;
                        continue;
                    }
                }
            }

            if (char.IsWhiteSpace(s[i]))
            {
                // treat only ' ' as space runs; '\t' etc. collapse to single space
                if (s[i] == ' ')
                {
                    int j = i + 1;
                    while (j < s.Length && s[j] == ' ') j++;
                    result.Add(new Token { Kind = TokenKind.Space, Text = " " }); // collapse multi-spaces
                    i = j;
                }
                else
                {
                    // other whitespace -> single space
                    result.Add(new Token { Kind = TokenKind.Space, Text = " " });
                    i++;
                }

                continue;
            }

            int k = i + 1;
            while (k < s.Length && s[k] != '{' && !char.IsWhiteSpace(s[k]) && s[k] != '\n') k++;
            result.Add(new Token { Kind = TokenKind.Word, Text = s.Substring(i, k - i) });
            i = k;
        }

        return result;
    }

    private List<DrawRun> Styleize(List<Token> tokens, Style defaultStyle)
    {
        var runs = new List<DrawRun>(tokens.Count);
        var pendingStyleForNextWord = (Style?)null;

        foreach (var t in tokens)
        {
            switch (t.Kind)
            {
                case TokenKind.LineBreak:
                    runs.Add(new DrawRun
                    {
                        Text = "\n",
                        Style = defaultStyle,
                        IsWhitespace = false
                    });
                    break;
                case TokenKind.ScopedTag:
                {
                    var st = ParseStylePayload(t.TagPayload, defaultStyle);
                    if (!string.IsNullOrEmpty(t.Text))
                    {
                        runs.Add(new DrawRun
                        {
                            Text = t.Text,
                            Style = st,
                            IsWhitespace = false
                        });
                    }

                    break;
                }
                case TokenKind.Tag:
                {
                    var payload = t.TagPayload.ToLowerInvariant();
                    if (payload == "reset")
                    {
                        pendingStyleForNextWord = defaultStyle;
                    }
                    else
                    {
                        pendingStyleForNextWord = ParseStylePayload(payload, defaultStyle);
                    }

                    break;
                }
                case TokenKind.Space:
                {
                    runs.Add(new DrawRun
                    {
                        Text = t.Text,
                        Style = defaultStyle, // spaces don't carry color; draw as transparent? We'll just not recolor
                        IsWhitespace = true
                    });
                    break;
                }
                case TokenKind.Word:
                {
                    var st = pendingStyleForNextWord ?? defaultStyle;
                    runs.Add(new DrawRun
                    {
                        Text = t.Text,
                        Style = st,
                        IsWhitespace = false
                    });
                    pendingStyleForNextWord = null; // consumed
                    break;
                }
            }
        }

        // Merge adjacent spaces into single space to avoid odd wrapping with multi-spaces
        var merged = new List<DrawRun>(runs.Count);
        foreach (var t in runs)
        {
            if (t is { IsWhitespace: true, Text.Length: > 1 })
            {
                t.Text = " ";
            }

            merged.Add(t);
        }

        return merged;
    }

    private Style ParseStylePayload(string payload, Style baseDefault)
    {
        // payload: color[,flags]
        // flags: b (bold)
        var color = baseDefault.Color;
        var bold = false;

        var parts = payload.Split(',');
        if (parts.Length >= 1)
        {
            var cName = parts[0].Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(cName))
                color = ResolveColor(cName, baseDefault.Color);
        }

        if (parts.Length >= 2)
        {
            var flags = parts[1].Trim().ToLowerInvariant();
            bold = flags.Contains('b');
        }

        return new Style { Color = color, Bold = bold };
    }

    private Color ResolveColor(string name, Color fallback)
    {
        return name switch
        {
            "black" => Color.Black,
            "white" => Color.White,
            "gray" => Color.Gray,
            "red" => Color.Red,
            "green" => Color.Green,
            "blue" => Color.Blue,
            "yellow" => Color.Yellow,
            "cyan" => Color.Cyan,
            "magenta" => Color.Magenta,
            "orange" => Color.Orange,
            "purple" => Color.Purple,
            "brown" => Color.Brown,
            "pink" => Color.Pink,
            "lime" => Color.Lime,
            "teal" => Color.Teal,
            "navy" => Color.Navy,
            "olive" => Color.Olive,
            "maroon" => new Color(128, 0, 0),
            "silver" => Color.Silver,
            "gold" => Color.Gold,
            _ => fallback
        };
    }

    private List<DrawRun> Layout(List<DrawRun> runs, float maxWidth, double lineSpacingScale)
    {
        var laid = new List<DrawRun>(runs.Count);
        float x = 0f;
        float y = 0f;

        // var lineHeight = Font.LineSpacing;
        var lineHeight = Font.LineSpacing * (float)lineSpacingScale;

        bool AtLineStart() => Math.Abs(x) < 0.0001f;

        foreach (var r in runs)
        {
            // hard line break support
            if (r.Text is "\n" or "\\n") // (in case content passed with escaped newline)
            {
                x = 0f;
                y += lineHeight;
                continue;
            }

            if (r.IsWhitespace)
            {
                // don't place a space at start of line
                if (!AtLineStart())
                {
                    var w = Font.MeasureString(" ").X;
                    laid.Add(new DrawRun
                        { Text = " ", Style = r.Style, IsWhitespace = true, Position = new Vector2(x, y) });
                    x += w;
                }

                continue;
            }

            var wordWidth = Font.MeasureString(r.Text).X;

            // wrap if this word would overflow and we're not at line start
            if (!float.IsInfinity(maxWidth) && !AtLineStart() && (x + wordWidth) > maxWidth)
            {
                x = 0f;
                y += lineHeight;
            }

            // place word
            laid.Add(new DrawRun
            {
                Text = r.Text,
                Style = r.Style,
                IsWhitespace = false,
                Position = new Vector2(x, y)
            });

            x += wordWidth;
        }

        return laid;
    }

    private readonly struct RectangleF(float x, float y, float w, float h)
    {
        public readonly float X = x, Y = y, W = w, H = h;
        public Vector2 Size() => new Vector2(W, H);
    }
}