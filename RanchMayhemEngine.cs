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
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        _graphics.HardwareModeSwitch = false;
        _graphics.IsFullScreen = IsFullScreen;
        _graphics.PreferredBackBufferWidth = Width;
        _graphics.PreferredBackBufferHeight = Height;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _spriteBatch.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        UiManager = new UiManager(_spriteBatch.GraphicsDevice, _spriteBatch);
        UiManager.Initialize();
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
