using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public class UiManager
{
    public static Texture2D Pixel;

    public Page? CurrentPage;
    private List<Page> _pages;

    private const int ReferenceWidth = 1920;
    private const int ReferenceHeight = 1080;

    private readonly float _globalScaleX;
    private readonly float _globalScaleY;

    public Vector2 GlobalScale;
    private List<Texture2D> Backgrounds { get; } = [];
    private int _backgroundIndex = 0;
    private int _backgroundTimer;
    private const int BackgroundInterval = 20;

    public GraphicsDevice GraphicsDevice { get; }
    public SpriteBatch SpriteBatch { get; }

    public static readonly List<RenderCommand> UiQueue = [];
    public static readonly List<RenderCommand> BackgroundQueue = [];
    public static readonly List<RenderCommand> PopUpQueue = [];
    public static readonly List<RenderCommand> OverlayQueue = [];

    public static Page Overlay;

    public UiManager(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        Logger.Log($"{GetType().FullName}::ctor", LogLevel.Internal);
        GraphicsDevice = graphicsDevice;
        SpriteBatch = spriteBatch;

        var viewport = GraphicsDevice.Viewport;

        _globalScaleX = (float)viewport.Width / ReferenceWidth;
        _globalScaleY = (float)viewport.Height / ReferenceHeight;
        GlobalScale = new Vector2(_globalScaleX, _globalScaleY);

        Pixel = new Texture2D(GraphicsDevice, 1, 1);
        Pixel.SetData([Color.White]);
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
        Logger.Log($"loading background image {texture.Name} {texture.Width}x{texture.Height}");
    }

    public void SetBackgrounds(List<Texture2D> textures)
    {
        Backgrounds.AddRange(textures);
    }

    private Page? GetPage(string id)
    {
        return _pages.Find(component => component.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
    }

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
                    DestinationRect = new Rectangle(0, 0, RanchMayhemEngine.Width, RanchMayhemEngine.Height),
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
        Overlay.Draw();
    }

    public void SetActivePage(string id)
    {
        Logger.Log(
            $"{GetType().FullName}::SetActivePage Current active page: {CurrentPage?.Id ?? "None"} new: {id}",
            LogLevel.Internal
        );

        if (CurrentPage is not null)
        {
            CurrentPage.IsVisible = false;
        }

        if (id.Equals(CurrentPage?.Id))
        {
            CloseActivePage();
            return;
        }

        CurrentPage = GetPage(id);
        CurrentPage?.ToggleVisibility();
    }

    public void CloseActivePage()
    {
        Logger.Log($"{GetType().FullName}::CloseActivePage Current active page: {CurrentPage?.Id ?? "None"}", LogLevel.Internal);

        if (CurrentPage is not null)
        {
            CurrentPage.IsVisible = false;
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

    private static void Flush(SpriteBatch sb, IEnumerable<RenderCommand> q)
    {
        Effect? currentShader = null;
        var begun = false;

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
        }

        foreach (var cmd in q)
        {
            if (!begun || !ReferenceEquals(cmd.Shader, currentShader))
            {
                if (begun) sb.End();
                currentShader = cmd.Shader;
                BeginWith(currentShader);
            }

            switch (cmd)
            {
                case { DestinationRect: not null, SourceRect: null }:
                    sb.Draw(
                        texture: cmd.Texture,
                        destinationRectangle: cmd.DestinationRect.Value,
                        color: cmd.Color
                    );
                    break;
                case { DestinationRect: not null, SourceRect: not null }:
                    sb.Draw(
                        texture: cmd.Texture,
                        destinationRectangle: cmd.DestinationRect.Value,
                        sourceRectangle: cmd.SourceRect.Value,
                        color: cmd.Color
                    );
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
                    break;
            }
        }

        if (begun) sb.End();
        // if (shader is not null)
        // {
        //     foreach (var technique in shader.Techniques)
        //     {
        //         shader.CurrentTechnique = technique;
        //         foreach (var pass in technique.Passes)
        //         {
        //             pass.Apply();
        //         }
        //     }
        // }
    }

    public static void Flush(SpriteBatch sb)
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
}
