using System;
using System.Collections.Generic;
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
        _louis = Content.Load<Texture2D>("louis");

        UIManager = new UIManager(_spriteBatch.GraphicsDevice, _spriteBatch);
        UIManager.Initialize();

        ContentManager = new ContentManager();
        ContentManager.LoadContent(Content);


        // UIManager.AddComponent(
        //     new Button("button", ContentManager.GetTexture("button"), UIAnchor.CenterX | UIAnchor.CenterY,
        //         new Vector2(300, 300)));

        //
        // UIManager.AddComponent(testbox3);
        // UIManager.AddComponent(testbox1);
        // UIManager.AddComponent(testbox2);

        // var container = new Container("container-1", Color.BlueViolet, new Vector2(100, 100), new Vector2(960, 540),
        //     null,
        //     [testbox1, testbox2]);
        //
        // UIManager.AddComponent(container);


        // var container = new Container("container-1", new UIComponentOptions
        // {
        //     Color = Color.BlueViolet,
        //     Position = new Vector2(100, 100),
        //     Size = new Vector2(960, 540)
        // }, [testbox4]);

        // UIManager.AddComponent(container);

        // var button = new Button("button-1", new UIComponentOptions
        //     {
        //         Texture = ContentManager.GetTexture("button"),
        //         Position = new Vector2(900, 600),
        //         Size = new Vector2(200, 20)
        //     },
        //     new Button.ButtonOptions
        //     {
        //         Texture = ContentManager.GetTexture("button"),
        //         ClickTexture = ContentManager.GetTexture("button_click"),
        //         HoverTexture = ContentManager.GetTexture("button_hover")
        //     });

        // UIManager.AddComponent(button);


        // var button2 = new Button("button-2", new UIComponentOptions
        //     {
        //         Texture = ContentManager.GetTexture("button"),
        //         UiAnchor = UIAnchor.CenterX | UIAnchor.Bottom,
        //         Size = new Vector2(200, 20)
        //     },
        //     new Button.ButtonOptions
        //     {
        //         Texture = ContentManager.GetTexture("button"),
        //         ClickTexture = ContentManager.GetTexture("button_click"),
        //         HoverTexture = ContentManager.GetTexture("button_hover"),
        //     });
        //
        // var testbox4 = new Box("test-4", new UIComponentOptions
        // {
        //     Color = Color.Blue,
        //     UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY,
        //     Size = new Vector2(75, 75)
        // });
        //
        // var container = new Container("container-1", new UIComponentOptions
        // {
        //     Color = Color.Orange,
        // }, [testbox4, button2]);

        // UIManager.AddComponent(button);
        // UIManager.AddComponent(button2);
        // var testbox3 = new Box("test-3", new UIComponentOptions
        // {
        //     Color = Color.Blue,
        // });
        // var testbox2 = new Box("test-2", new UIComponentOptions
        // {
        //     Color = Color.Green,
        //     Size = new Vector2(150, 150),
        //     UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        // });
        //
        // var grid1 = new Grid("grid-1", new UIComponentOptions
        //     {
        //         Color = Color.Red,
        //         Position = new Vector2(100, 100),
        //         Size = new Vector2(900, 600)
        //     },
        //     new Grid.GridOptions
        //     {
        //         Columns = [2, 1, 1],
        //         ColumnGap = 0,
        //         Rows = [1, 1, 1],
        //         RowGap = 0,
        //     }, [testbox3, testbox2, container]);

        // UIManager.AddComponent(grid1);
        // UIManager.AddComponent(container);

        var profile = new Box("profile", new UIComponentOptions
        {
            Texture = ContentManager.GetTexture("lychee"),
            Size = new Vector2(128),
            // Color = Color.Red,
            UiAnchor = UIAnchor.CenterY | UIAnchor.Left,
        });

        var crop1 = new Box("crop-1", new UIComponentOptions
        {
            Texture = ContentManager.GetTexture("carrot"),
            // Color = Color.Red,
            // Size = new Vector2(128),
            SizePercent = new Vector2(0, 100),
            SizeUnit = SizeUnit.Percent,
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        });

        var crop2 = new Box("crop-2", new UIComponentOptions
        {
            Texture = ContentManager.GetTexture("corn"),
            Size = new Vector2(128),
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        });

        var crop3 = new Box("crop-3", new UIComponentOptions
        {
            Texture = ContentManager.GetTexture("dragon_fruit"),
            Size = new Vector2(128),
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        });

        var menubar = new Grid("mennubar", new UIComponentOptions
            {
                Color = Color.White,
                UiAnchor = UIAnchor.Bottom,
                Size = new Vector2(1920, 216)
            },
            new Grid.GridOptions
            {
                Columns = [2, 1, 1, 1],
                ColumnGap = 10,
                Rows = [1],
                RowGap = 0,
                Padding = new Vector4(5)
            }, [profile, crop1, crop2, crop3]);

        UIManager.AddComponent(menubar);

        var container = new Container("test-container", new UIComponentOptions
        {
            Color = Color.Red,
            Size = new Vector2(900, 450),
            UiAnchor = UIAnchor.Top | UIAnchor.CenterX
        }, []);

        UIManager.AddComponent(container);


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

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        UIManager.RenderComponents();

        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}