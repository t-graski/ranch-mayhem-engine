using System.Buffers.Text;
using Microsoft.Xna.Framework;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine.Pages;

public class MenuBar : Page
{
    public override Page Initialize()
    {
        Id = "menu-bar";
        var contentManager = RanchMayhemEngine.ContentManager;


        var profile = new Box("profile", new UIComponentOptions
        {
            Texture = contentManager.GetTexture("lychee"),
            Size = new Vector2(128),
            // Color = Color.Red,
            UiAnchor = UIAnchor.CenterY | UIAnchor.Left,
        });

        var quickAccess = new Box("menubar-quick-access", new UIComponentOptions
        {
            Texture = contentManager.GetTexture("quick_access"),
            // Color = Color.Red,
            // Size = new Vector2(128),
            SizePercent = new Vector2(0, 75),
            SizeUnit = SizeUnit.Percent,
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        });

        // var crops = new Box("menubar-crops", new UIComponentOptions
        // {
        //     Texture = contentManager.GetTexture("crops"),
        //     SizePercent = new Vector2(0, 75),
        //     SizeUnit = SizeUnit.Percent,
        //     UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY,
        // });

        var crops = new Button("menubar-crops", new UIComponentOptions
        {
            Texture = contentManager.GetTexture("crops"),
            SizePercent = new Vector2(0, 75),
            SizeUnit = SizeUnit.Percent,
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        }, new Button.ButtonOptions
        {
            TextColor = Color.Green,
            Text = "Crops",
            Texture = contentManager.GetTexture("crops"),
            HoverTexture = contentManager.GetTexture("crops"),
            ClickTexture = contentManager.GetTexture("crops")
        })
        {
            OnClick = ToggleInventory
        };

        var crop3 = new Box("crop-3", new UIComponentOptions
        {
            Texture = contentManager.GetTexture("dragon_fruit"),
            Size = new Vector2(128),
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        });

        var menubar = new Grid("menubar", new UIComponentOptions
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
            }, [profile, quickAccess, crops]);

        Components.Add(menubar);
        IsVisible = true;

        return this;
    }

    private void ToggleInventory()
    {
        RanchMayhemEngine.UIManager.GetPage("crops").ToggleVisibility();
    }
}