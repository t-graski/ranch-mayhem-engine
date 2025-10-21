using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public class UiManager
{
    public static Texture2D Pixel;
    public static Texture2D TransparentPixel;

    public Page? CurrentPage;
    private List<Page> _pages;

    private readonly Dictionary<string, UiComponent> _byId = new(StringComparer.Ordinal);

    private Func<IEnumerable<Page>> _pageFactory = Array.Empty<Page>;

    public void SetPageFactory(Func<IEnumerable<Page>> pageFactory) => _pageFactory = pageFactory;
    private bool _rebuilding;

    private const int ReferenceWidth = 1920;
    private const int ReferenceHeight = 1080;

    private float _globalScaleX;
    private float _globalScaleY;

    public Vector2 GlobalScale;
    private List<Texture2D> Backgrounds { get; } = [];
    private int _backgroundIndex = 0;
    private int _backgroundTimer;
    private const int BackgroundInterval = 20;

    public GraphicsDevice GraphicsDevice { get; }
    public SpriteBatch SpriteBatch { get; }

    private RenderTarget2D _uiTarget;
    private Rectangle _presentDest;
    private float _presentScale;
    private Point _presentOffset;

    public static readonly List<RenderCommand> UiQueue = [];
    public static readonly List<RenderCommand> BackgroundQueue = [];
    public static readonly List<RenderCommand> PopUpQueue = [];
    public static readonly List<RenderCommand> OverlayQueue = [];

    public static Page Overlay;

    public bool IsInputDisabled;
    public List<string> InputExceptions = [];

    public UiManager(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        Logger.Log($"{GetType().FullName}::ctor", LogLevel.Internal);
        GraphicsDevice = graphicsDevice;
        SpriteBatch = spriteBatch;

        _uiTarget = new RenderTarget2D(
            GraphicsDevice,
            ReferenceWidth,
            ReferenceHeight,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            0,
            RenderTargetUsage.DiscardContents
        );

        var viewport = GraphicsDevice.Viewport;

        _globalScaleX = (float)viewport.Width / ReferenceWidth;
        _globalScaleY = (float)viewport.Height / ReferenceHeight;
        GlobalScale = new Vector2(_globalScaleX, _globalScaleY);

        Pixel = new Texture2D(GraphicsDevice, 1, 1);
        Pixel.SetData([Color.White]);
        TransparentPixel = new Texture2D(GraphicsDevice, 1, 1);
        TransparentPixel.SetData([Color.Transparent]);
    }

    public void FullRebuild()
    {
        if (_rebuilding) return;
        EnabledInput();
        RecalculateScale();
        _rebuilding = true;

        try
        {
            var keepVisibleId = CurrentPage?.Id;
            var tutorialWasVisible = GetPage("tutorial")?.IsVisible ?? false;

            foreach (var p in _pages)
            {
                try
                {
                    p.Dispose();
                }
                catch
                {
                    // ignored
                }
            }

            _pages.Clear();
            _byId.Clear();
            ClearQueues();

            var fresh = _pageFactory?.Invoke() ?? [];

            foreach (var page in fresh)
            {
                _pages.Add(page);
                var root = page.Initialize();
                // AddComponent(root);
                page.ToggleVisibility(forceInvisible: true);
            }

            Logger.Log($"current pages: {string.Join(", ", _pages.Select(p => p.Id))}");

            CurrentPage = _pages.FirstOrDefault(p => p.Id == keepVisibleId) ?? _pages.FirstOrDefault();
            CurrentPage?.ToggleVisibility(forceInvisible: false);

            // TODO: add a hook to - for now this is fine
            // Action? UiRebuilt;

            if (tutorialWasVisible)
                GetPage("tutorial")?.SetVisibility(true);

            GetPage("menu-bar").SetVisibility(true);
        }
        finally
        {
            _rebuilding = false;
        }
    }

    private void RecalculateScale()
    {
        var vp = GraphicsDevice.Viewport;
        _globalScaleX = (float)vp.Width / ReferenceWidth;
        _globalScaleY = (float)vp.Height / ReferenceHeight;
        GlobalScale = new Vector2(_globalScaleX, _globalScaleY);
    }

    public void Initialize()
    {
        _pages = [];
    }

    public void AddComponent(Page page)
    {
        _pages.Add(page);
    }

    public void SetBackground(Texture2D texture)
    {
        Backgrounds.Add(texture);
    }

    public void SetBackgrounds(List<Texture2D> textures)
    {
        Backgrounds.AddRange(textures);
    }

    [Obsolete("Use GetPage<T> instead")]
    public Page? GetPage(string id)
    {
        return _pages.Find(component => component.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
    }

    public T? GetPage<T>() where T : Page => _pages.OfType<T>().FirstOrDefault();

    public void UpdateComponents(MouseState mouseState)
    {
        _backgroundTimer++;

        if (_backgroundTimer >= BackgroundInterval)
        {
            _backgroundTimer = 0;
            _backgroundIndex = (_backgroundIndex + 1) % Backgrounds.Count;
        }

        foreach (var page in _pages)
        {
            page.Update(mouseState);
        }
    }

    public void RenderBackground()
    {
        if (Backgrounds.Count > 0)
        {
            Enqueue(
                new RenderCommand
                {
                    Texture = Backgrounds[_backgroundIndex],
                    DestinationRect = new Rectangle(
                        0,
                        0,
                        (int)(RanchMayhemEngine.Width * _globalScaleX),
                        (int)(RanchMayhemEngine.Height * _globalScaleY)
                    ),
                    Color = Color.White,
                },
                BackgroundQueue
            );
        }
    }

    public void RenderComponents()
    {
        // if (!RanchMayhemEngine.IsFocused) return;

        foreach (var page in _pages)
        {
            page.Draw();
        }
    }

    public void RenderPopUp()
    {
    }

    public void RenderOverlay()
    {
        // Overlay.Draw();
    }

    public void SetActivePage(string id)
    {
        Logger.Log(
            $"{GetType().FullName}::SetActivePage Current active page: {CurrentPage?.Id ?? "None"} new: {id}",
            LogLevel.Internal
        );

        if (CurrentPage is not null)
        {
            CurrentPage.ToggleVisibility(forceInvisible: true);
        }

        if (id.Equals(CurrentPage?.Id))
        {
            CloseActivePage();
            return;
        }

        CurrentPage = GetPage(id);
        CurrentPage?.ToggleVisibility();
        CurrentPage?.OnAppear();
    }

    public void CloseActivePage()
    {
        Logger.Log($"{GetType().FullName}::CloseActivePage Current active page: {CurrentPage?.Id ?? "None"}",
            LogLevel.Internal);

        if (CurrentPage is not null)
        {
            CurrentPage.ToggleVisibility(forceInvisible: true);
            CurrentPage = null;
        }
    }

    private static void ClearQueue(List<RenderCommand> q) => q.Clear();

    public static void ClearQueues()
    {
        ClearQueue(BackgroundQueue);
        ClearQueue(UiQueue);
        ClearQueue(PopUpQueue);
        ClearQueue(OverlayQueue);
    }


    public static void Enqueue(RenderCommand cmd, List<RenderCommand>? q = null)
    {
        if (q == null)
        {
            UiQueue.Add(cmd);
        }
        else
        {
            q.Add(cmd);
        }
    }

    public static void Enqueue(IEnumerable<RenderCommand> cmds, List<RenderCommand>? q = null)
    {
        if (q == null)
        {
            UiQueue.AddRange(cmds);
        }
        else
        {
            q.AddRange(cmds);
        }
    }

    private static readonly RasterizerState ScissorRaster = new RasterizerState { ScissorTestEnable = true };

    private static int _metricsCounter = 0;
    private static RenderMetrics _renderMetrics = new RenderMetrics();

    public static void Flush(SpriteBatch sb, IEnumerable<RenderCommand> q)
    {
        Effect? currentShader = null;
        var begun = false;

        var scissorsStack = new Stack<Rectangle>();
        var gd = sb.GraphicsDevice;
        var defaultScissor = gd.Viewport.Bounds;

        gd.ScissorRectangle = defaultScissor;
        var metrics = new RenderMetrics();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        void BeginWith(Effect? shader)
        {
            sb.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: DepthStencilState.None,
                rasterizerState: new RasterizerState { ScissorTestEnable = true },
                effect: shader
            );

            begun = true;
            metrics.BatchCount++;
        }

        foreach (var cmd in q)
        {
            metrics.TotalCommands++;
            if (cmd is { ScissorPush: true, ScissorRect: not null })
            {
                if (begun) sb.End();

                var rect = cmd.ScissorRect.Value;
                scissorsStack.Push(gd.ScissorRectangle);
                gd.ScissorRectangle = rect;

                BeginWith(currentShader);
                metrics.ScissorChanges++;
                continue;
            }

            if (cmd.ScissorPop)
            {
                if (begun) sb.End();

                gd.ScissorRectangle = scissorsStack.Count > 0 ? scissorsStack.Pop() : defaultScissor;

                BeginWith(currentShader);
                metrics.ScissorChanges++;
                continue;
            }

            if (!begun || !ReferenceEquals(cmd.Shader, currentShader))
            {
                if (begun) sb.End();
                currentShader = cmd.Shader;
                BeginWith(currentShader);
                metrics.ShaderChanges++;
            }

            switch (cmd)
            {
                case { DestinationRect: not null, SourceRect: null }:
                    sb.Draw(
                        texture: cmd.Texture,
                        destinationRectangle: cmd.DestinationRect.Value,
                        color: cmd.Color
                    );
                    metrics.TextureDraws++;
                    break;
                case { DestinationRect: not null, SourceRect: not null }:
                    sb.Draw(
                        texture: cmd.Texture,
                        destinationRectangle: cmd.DestinationRect.Value,
                        sourceRectangle: cmd.SourceRect.Value,
                        color: cmd.Color
                    );
                    metrics.TextureDraws++;
                    break;
                case { SpriteFont: not null }:
                    sb.DrawString(
                        spriteFont: cmd.SpriteFont,
                        text: cmd.Text,
                        position: new Vector2(MathF.Round(cmd.Position.X), MathF.Round(cmd.Position.Y)),
                        color: cmd.Color,
                        rotation: 0f,
                        origin: Vector2.Zero,
                        scale: cmd.Scale,
                        effects: cmd.Effects,
                        layerDepth: cmd.LayerDepth
                    );
                    metrics.TextDraws++;
                    metrics.TotalCharacters += cmd.Text?.Length ?? 0;
                    break;
                default:
                    sb.Draw(
                        texture: cmd.Texture,
                        position: cmd.Position,
                        sourceRectangle: cmd.SourceRect,
                        color: cmd.Color,
                        rotation: cmd.Rotation,
                        origin: cmd.Origin,
                        scale: cmd.Scale,
                        effects: cmd.Effects,
                        layerDepth: cmd.LayerDepth
                    );
                    metrics.TextureDraws++;
                    break;
            }
        }

        if (begun) sb.End();

        stopwatch.Stop();
        metrics.ElapsedMs = stopwatch.Elapsed.TotalMilliseconds;

        _renderMetrics.TotalCommands += metrics.TotalCommands;
        _renderMetrics.BatchCount += metrics.BatchCount;
        _renderMetrics.TextDraws += metrics.TextDraws;
        _renderMetrics.TotalCharacters += metrics.TotalCharacters;
        _renderMetrics.TextureDraws += metrics.TextureDraws;
        _renderMetrics.ShaderChanges += metrics.ShaderChanges;
        _renderMetrics.ScissorChanges += metrics.ScissorChanges;
        _renderMetrics.ElapsedMs += metrics.ElapsedMs;

        _frameCounter++;

        if (_frameCounter >= 60)
        {
            // Logger.Log(
            //     $"[RENDER METRICS] Commands: {metrics.TotalCommands} | Batches: {metrics.BatchCount} | TextDraws: {metrics.TextDraws} ({metrics.TotalCharacters} chars) | TextureDraws: {metrics.TextureDraws} | ShaderChanges: {metrics.ShaderChanges} | ScissorChanges: {metrics.ScissorChanges} | Elapsed: {metrics.ElapsedMs:F2} ms");
            var avg = _renderMetrics;

            // Logger.Log(
            //     $"[RENDER METRICS - 60 FRAMES AVERAGE] Commands: {avg.TotalCommands / 60} | Batches: {avg.BatchCount / 60} | TextDraws: {avg.TextDraws / 60} ({avg.TotalCharacters / 60} chars) | TextureDraws: {avg.TextureDraws / 60} | ShaderChanges: {avg.ShaderChanges / 60} | ScissorChanges: {avg.ScissorChanges / 60} | Elapsed: {avg.ElapsedMs / 60:F2} ms");
            _frameCounter = 0;
            _renderMetrics = new RenderMetrics();
        }
    }

    private static int _frameCounter = 0;

    private struct RenderMetrics
    {
        public int TotalCommands;
        public int BatchCount;
        public int TextDraws;
        public int TotalCharacters;
        public int TextureDraws;
        public int ShaderChanges;
        public int ScissorChanges;
        public double ElapsedMs;
    }

    private static void Flush(SpriteBatch sb)
    {
        // Logger.Log($"BackgroundQ={BackgroundQueue.Count}");
        // Logger.Log($"iUiQ={UiQueue.Count}");
        // Logger.Log($"PopUpQ={PopUpQueue.Count}");
        // Logger.Log($"OverlayQ={OverlayQueue.Count}");
        Flush(sb, BackgroundQueue);
        Flush(sb, UiQueue);
        Flush(sb, PopUpQueue);
        Flush(sb, OverlayQueue);
    }

    public static int RenderQueueLength() => UiQueue.Count;

    public void DisableInputExcept(IReadOnlyList<string> exceptions)
    {
        IsInputDisabled = true;
        InputExceptions = exceptions.ToList();
    }

    public void EnabledInput()
    {
        IsInputDisabled = false;
        InputExceptions.Clear();
    }
}