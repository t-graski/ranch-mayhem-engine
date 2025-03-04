using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.Content;
using ranch_mayhem_engine.Pages;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine;

public sealed class RanchMayhemEngine : Game
{
    private const bool IsFullScreen = false;
    private const int Width = 1280;
    private const int Height = 720;


    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static UIManager UIManager { get; private set; }
    public static ContentManager ContentManager { get; private set; }
    public static SpriteFont MainFont { get; private set; }

    public static MouseState MouseState { get; private set; }

    public static bool IsFocused { get; private set; } = true;
    private static bool WasFocused { get; set; } = false;

    public RanchMayhemEngine()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
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

        // MainFont = Content.Load<SpriteFont>("MainFont");

        UIManager = new UIManager(_spriteBatch.GraphicsDevice, _spriteBatch);
        UIManager.Initialize();

        ContentManager = new ContentManager();
        ContentManager.LoadContent(Content);

        UIManager.AddComponent(new MenuBar().Initialize());
        UIManager.AddComponent(new Crops().Initialize());

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        MouseState = Mouse.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        UIManager.UpdateComponents(MouseState);

        // TODO: Add your update logic here

        if (!IsActive && !WasFocused)
        {
            Logger.Log("Lost focus");
            IsFocused = false;
            WasFocused = true;
        }

        if (WasFocused && IsActive)
        {
            Logger.Log("Regained focus");
            KeyboardInput.Update();
            IsFocused = true;
            WasFocused = false;
        }


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGoldenrodYellow);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        UIManager.RenderComponents();

        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}