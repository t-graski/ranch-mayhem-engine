using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;
using ranch_mayhem_engine.Utils;

namespace ranch_mayhem_engine;

public class RanchMayhemEngine : Game
{
    private const bool IsFullScreen = true;
    public static readonly LogLevel LogLevel = LogLevel.Error;

    private Point _windowedSize = new(1280, 720);
    private bool _isBorderlessFullscreen = false;

    public const int Width = 1920;
    public const int Height = 1080;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static UiManager UiManager { get; private set; }
    public static MouseState MouseState { get; private set; }
    public static bool IsFocused { get; private set; } = true;
    private static bool WasFocused { get; set; } = false;
    public static double Framerate { get; set; }

    public static readonly NumberFormatter Nf = new();

    public static GameTime GameTime { get; private set; }

    protected RanchMayhemEngine()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            // !!!
            // Might cause problems later with weird pixels
            PreferHalfPixelOffset = false,
            SynchronizeWithVerticalRetrace = true,
            IsFullScreen = false,
            HardwareModeSwitch = false,

            PreferredBackBufferWidth = _windowedSize.X,
            PreferredBackBufferHeight = _windowedSize.Y,
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        // _graphics.HardwareModeSwitch = false;
        // _graphics.IsFullScreen = IsFullScreen;
        // _graphics.PreferredBackBufferWidth = Width;
        // _graphics.PreferredBackBufferHeight = Height;
        _graphics.ApplyChanges();

        UiManager = new UiManager(GraphicsDevice, new SpriteBatch(GraphicsDevice));

        Window.ClientSizeChanged += OnClientSizeChanged;

        base.Initialize();
    }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        if (_isBorderlessFullscreen)
        {
            return;
        }

        if (_graphics.IsFullScreen || Window.IsBorderless)
        {
            return;
        }

        var bounds = Window.ClientBounds;
        if (bounds is { Width: > 0, Height: > 0 })
        {
            _graphics.PreferredBackBufferWidth = bounds.Width;
            _graphics.PreferredBackBufferHeight = bounds.Height;
            _graphics.ApplyChanges();
            _windowedSize = new Point(bounds.Width, bounds.Height);

            UiManager.FullRebuild();
        }
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // _spriteBatch.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        // UiManager = new UiManager(_spriteBatch.GraphicsDevice, _spriteBatch);
        UiManager.Initialize();
    }


    public void ToggleFullscreen()
    {
        _isBorderlessFullscreen = !_isBorderlessFullscreen;

        if (_isBorderlessFullscreen)
        {
            var cb = Window.ClientBounds;
            _windowedSize = new Point(cb.Width, cb.Height);

            var b = SDL2Display.GetCurrentDisplayBounds(Window.Handle);

            var dm = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            Window.IsBorderless = false;
            _graphics.HardwareModeSwitch = false;
            _graphics.IsFullScreen = false;

            Window.Position = new Point(b.X, b.Y);
            _graphics.PreferredBackBufferWidth = b.Width;
            _graphics.PreferredBackBufferHeight = b.Height;

            Window.IsBorderless = true;
        }
        else
        {
            Window.IsBorderless = false;
            Window.Position = new Point(100, 100);
            _graphics.HardwareModeSwitch = false;
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = _windowedSize.X;
            _graphics.PreferredBackBufferHeight = _windowedSize.Y;
        }

        _graphics.ApplyChanges();
        UiManager.FullRebuild();
    }



    protected override void Update(GameTime gameTime)
    {
#if DEBUG
        DebugUtils.BeginFrame(gameTime);
#endif
        Framerate = (1 / gameTime.ElapsedGameTime.TotalSeconds);
        GameTime = gameTime;

        MouseState = Mouse.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (KeyboardInput.CurrentState.IsKeyDown(Keys.F11) && !KeyboardInput.PreviousState.IsKeyDown(Keys.F11))
        {
            ToggleFullscreen();
        }


        UiManager.UpdateComponents(MouseState);

        if (!IsActive && !WasFocused)
        {
            Logger.Log("Lost focus");
            IsFocused = false;
            WasFocused = true;
        }

        if (WasFocused && IsActive)
        {
            Logger.Log("Regained focus");
            IsFocused = true;
            WasFocused = false;
        }


        KeyboardInput.Update();
        MouseInput.Update();
        KeyboardManager.Update();
        base.Update(gameTime);

#if DEBUG
        DebugUtils.EndFrame(gameTime);
#endif
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGoldenrodYellow);

        base.Draw(gameTime);
    }
}
