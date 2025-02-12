using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine;

public class RanchMayhemEngine : Game
{
    private const bool IsFullScreen = false;
    private const int Width = 1280;
    private const int Height = 720;


    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _louis;

    private UIManager _uiManager;

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

        _uiManager = new UIManager(_spriteBatch);
        _uiManager.Initialize();

        _uiManager.AddComponent(new Button(_louis, new Vector2(100, 100)));

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _uiManager.UpdateComponents(mouseState);

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGoldenrodYellow);

        _spriteBatch.Begin();

        _uiManager.RenderComponents();

        // _spriteBatch.Draw(_louis, new Vector2(100, 100), Color.White);

        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}