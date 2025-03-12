using Microsoft.Xna.Framework;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine.Pages;

public class MenuBar : Page
{
    public override Page Initialize()
    {
        Id = "menu-bar";
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
        });

        var crops = new Button("menubar-crops", new Button.ButtonOptions
        {
            Texture = contentManager.GetTexture("quick_access"),
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

        var menubar = new Grid("menubar", new Grid.GridOptions()
        {
            Color = Color.White,
            UiAnchor = UIAnchor.Bottom,
            Size = new Vector2(1920, 216),
            Columns = [2, 1, 1, 1],
            ColumnGap = 10,
            Rows = [1],
            RowGap = 0,
            Padding = new Vector4(5),
            
            BorderColor = Color.DarkGray,
            BorderOrientation = BorderOrientation.Inside,
            BorderSize = 2
        }, [profile, quickAccess, crops]);

        Components.Add(menubar);
        IsVisible = true;

        return this;
    }

    private static void ToggleInventory()
    {
        RanchMayhemEngine.UIManager.GetPage("crops").ToggleVisibility();
    }
}