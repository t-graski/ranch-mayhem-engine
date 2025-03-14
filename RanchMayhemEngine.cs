using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.Content;
using ranch_mayhem_engine.Debug;
using ranch_mayhem_engine.Pages;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine;

public sealed class RanchMayhemEngine : Game
{
    private const bool IsFullScreen = false;
    public const int Width = 1280;
    public const int Height = 720;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static UIManager UIManager { get; private set; }
    public static ContentManager ContentManager { get; private set; }
    public static KeyboardManager KeyboardManager { get; private set; }
    public static MouseState MouseState { get; private set; }
    public static bool IsFocused { get; private set; } = true;
    private static bool WasFocused { get; set; } = false;
    public static double Framerate { get; private set; }

    public RanchMayhemEngine()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 30d);
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = IsFullScreen;
        _graphics.PreferredBackBufferWidth = Width;
        _graphics.PreferredBackBufferHeight = Height;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        UIManager = new UIManager(_spriteBatch.GraphicsDevice, _spriteBatch);
        UIManager.Initialize();

        ContentManager = new ContentManager();
        ContentManager.LoadContent(Content);
        ContentManager.LoadFromTextureAtlas(Content);

        KeyboardManager = new KeyboardManager();

        UIManager.AddComponent(new MenuBar().Initialize());
        UIManager.AddComponent(new Crops().Initialize());
        UIManager.AddComponent(new QuickAccess().Initialize());
        var console = new Pages.Console().Initialize();
        UIManager.AddComponent(console);
        var stats = new Stats().Initialize();
        UIManager.AddComponent(stats);

        KeyboardManager.RegisterBinding(Keys.OemQuestion, console);
        KeyboardManager.RegisterBinding(Keys.S, stats);

        UIManager.SetBackground(ContentManager.GetTexture("spring_background"));
    }

    protected override void Update(GameTime gameTime)
    {
        Framerate = (1 / gameTime.ElapsedGameTime.TotalSeconds);

        MouseState = Mouse.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        UIManager.UpdateComponents(MouseState);

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
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGoldenrodYellow);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        UIManager.RenderBackground();
        UIManager.RenderComponents();

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}