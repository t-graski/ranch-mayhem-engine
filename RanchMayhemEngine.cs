using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.Content;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine;

public sealed class RanchMayhemEngine : Game
{
    private const bool IsFullScreen = false;
    private const int Width = 1280;
    private const int Height = 720;


    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _louis;
    public static UIManager UIManager { get; private set; }
    public static ContentManager ContentManager { get; private set; }

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
        _louis = Content.Load<Texture2D>("louis");

        UIManager = new UIManager(_spriteBatch.GraphicsDevice, _spriteBatch);
        UIManager.Initialize();

        ContentManager = new ContentManager();
        ContentManager.LoadContent(Content);

        UIManager.AddComponent(
            new Button("button", ContentManager.GetTexture("button"), UIAnchor.CenterX | UIAnchor.CenterY,
                new Vector2(300, 300)));

        // var testbox3 = new Box("test-3", Color.Orange, UIAnchor.Bottom | UIAnchor.CenterX,
        //     new Vector2(1920, 250), null);
        // var testbox1 = new Box("test-1", Color.Green, UIAnchor.CenterX | UIAnchor.CenterY,
        //     new Vector2(175, 175), testbox3);
        // var testbox2 = new Box("test-2", Color.Blue, UIAnchor.CenterY | UIAnchor.CenterX,
        //     new Vector2(75, 75), testbox1);
        //
        // UIManager.AddComponent(testbox3);
        // UIManager.AddComponent(testbox1);
        // UIManager.AddComponent(testbox2);
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        UIManager.UpdateComponents(mouseState);

        // TODO: Add your update logic here

        if (!IsActive && !WasFocused)
        {
            Console.WriteLine("Lost focus");
            IsFocused = false;
            WasFocused = true;
        }

        if (WasFocused && IsActive)
        {
            Console.WriteLine("Regained focus");
            IsFocused = true;
            WasFocused = false;
        }


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGoldenrodYellow);

        _spriteBatch.Begin();

        UIManager.RenderComponents();

        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}