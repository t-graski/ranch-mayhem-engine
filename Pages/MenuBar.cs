using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine.Pages;

public class MenuBar : Page
{
    private ProgressBar _levelProgress;

    public override Page Initialize()
    {
        Id = "menu-bar";
        IsVisible = true;
        var contentManager = RanchMayhemEngine.ContentManager;

        var profile = new Box("menubar-profile", new Box.BoxOptions
        {
            // Texture = contentManager.GetTexture("quick_access"),
            SizePercent = new Vector2(0, 75),
            SizeUnit = SizeUnit.Percent,
            Color = Color.WhiteSmoke,
            UiAnchor = UIAnchor.CenterY | UIAnchor.Left,

            BorderColor = Color.Goldenrod,
            BorderSize = 4,
            BorderOrientation = BorderOrientation.Inside
        });

        var quickAccess = new Box("menubar-quick-access", new Box.BoxOptions
        {
            Texture = contentManager.GetTexture("quick_access"),
            // Color = Color.Red,
            // Size = new Vector2(128),
            SizePercent = new Vector2(0, 75),
            SizeUnit = SizeUnit.Percent,
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY,

            BorderColor = Color.Goldenrod,
            BorderSize = 4,
            BorderOrientation = BorderOrientation.Inside
        })
        {
            OnClick = ToggleQuickAccess
        };

        var crops = new Button("menubar-crops", new Button.ButtonOptions
        {
            Texture = contentManager.GetTexture("crops"),
            SizePercent = new Vector2(0, 75),
            SizeUnit = SizeUnit.Percent,
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY,
            HoverTexture = contentManager.GetTexture("crops"),
            ClickTexture = contentManager.GetTexture("crops"),

            BorderColor = Color.Goldenrod,
            BorderSize = 4,
            BorderOrientation = BorderOrientation.Inside
        })
        {
            OnClick = ToggleInventory
        };

        var hoverTest = new Text("hover", new Text.TextOptions
        {
            FontColor = Color.MediumSpringGreen,
            FontSize = 16,
            Content = "Test hovering\nfoo\nbar",

            BorderColor = Color.Red,
            BorderOrientation = BorderOrientation.Outside,
            BorderSize = 2
        });

        _levelProgress = new ProgressBar("level-progress-bar", new ProgressBar.ProgressBarOptions
        {
            Fraction = 0,
            Size = new Vector2(400, 40),
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY,
            Content = "LVL: 32 (0%)",
            Thresholds = new Dictionary<float, Color>
            {
                { 0f, Color.Red },
                { 0.5f, Color.Yellow },
                { 0.9f, Color.Green }
            },

            BorderColor = Color.Aqua,
            BorderSize = 2,
            BorderOrientation = BorderOrientation.Outside
        });

        var progressContainer = new Container("progress-bar-container", new UIComponentOptions
        {
        }, [_levelProgress])
        {
            OnClick = () => { _levelProgress.SetFraction(_levelProgress.GetFraction() + 0.05f); }
        };

        var menubar = new Grid("menubar", new Grid.GridOptions()
        {
            Color = Color.Gray * 0.95f,
            UiAnchor = UIAnchor.Bottom,
            Size = new Vector2(1920, 216),
            Columns = [1, 1, 1, 1],
            ColumnGap = 10,
            Rows = [1],
            RowGap = 0,
            Padding = new Vector4(5, 5, 5, 20),

            BorderColor = Color.DarkGray,
            BorderOrientation = BorderOrientation.Inside,
            BorderSize = 2
        }, [profile, progressContainer, quickAccess, crops]);

        AddComponent(menubar);

        return this;
    }

    public override void Update(MouseState mouseState)
    {
        _levelProgress.SetContent($"LVL: 32 ({_levelProgress.GetFraction()})");
        _levelProgress.Update();
        base.Update(mouseState);
    }

    private static void ToggleInventory()
    {
        RanchMayhemEngine.UIManager.GetPage("crops").ToggleVisibility();
    }

    private static void ToggleQuickAccess()
    {
        RanchMayhemEngine.UIManager.GetPage("quick-access").ToggleVisibility();
    }
}