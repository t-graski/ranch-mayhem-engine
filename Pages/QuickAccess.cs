using Microsoft.Xna.Framework;
using ranch_mayhem_engine.UI;
using ranch_mayhem_engine.UI.Helper;

namespace ranch_mayhem_engine.Pages;

public class QuickAccess : Page
{
    public override Page Initialize()
    {
        Id = "quick-access";
        IsVisible = false;

        // var automationText = new Text("quick-access-automation-text", new Text.TextOptions
        // {
        //     FontColor = Color.White,
        //     FontSize = 16,
        //     Content = "Automation",
        //     UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        // });

        var automationText = new TextBuilder($"{Id}-automation-text")
            .SetContent("Automation")
            .SetFontSize(16)
            .SetFontColor(Color.White)
            .CenterXY()
            .Build();

        var automationContainer = new Container("quick-access-automation-container", new UIComponentOptions
        {
            BorderColor = Color.Orange,
            BorderSize = 2,
            BorderOrientation = BorderOrientation.Outside
        }, [automationText]);

        var cropShopText = new Text("quick-access-crop-shop-text", new Text.TextOptions
        {
            FontColor = Color.White,
            FontSize = 16,
            Content = "Crop Shop",
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        });

        var cropShopContainer = new Container("quick-access-crop-shop-container", new UIComponentOptions
        {
            BorderColor = Color.Orange,
            BorderSize = 2,
            BorderOrientation = BorderOrientation.Outside
        }, [cropShopText]);

        var shopText = new Text("quick-access-shop-text", new Text.TextOptions
        {
            FontColor = Color.White,
            FontSize = 16,
            Content = "Shop",
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        });

        var shopContainer = new Container("quick-access-shop-container", new UIComponentOptions
        {
            BorderColor = Color.Orange,
            BorderSize = 2,
            BorderOrientation = BorderOrientation.Outside
        }, [shopText]);

        var questText = new Text("quick-access-quest-text", new Text.TextOptions
        {
            FontColor = Color.White,
            FontSize = 16,
            Content = "Quests",
            UiAnchor = UIAnchor.CenterX | UIAnchor.CenterY
        });

        var questContainer = new Container("quick-access-quest-container", new UIComponentOptions
        {
            BorderColor = Color.Orange,
            BorderSize = 2,
            BorderOrientation = BorderOrientation.Outside
        }, [questText]);


        var itemGrid = new Grid("quick-access-grid", new Grid.GridOptions
        {
            Color = Color.MediumAquamarine,
            Position = new Vector2(360, 100),
            Size = new Vector2(1200, 664),
            Columns = [1, 1, 1],
            ColumnGap = 15,
            Rows = [1, 1, 1, 1, 1],
            RowGap = 25,
            Padding = new Vector4(10),

            BorderTexture = RanchMayhemEngine.ContentManager.GetTexture("planks_oak"),
            BorderSize = 6,
            BorderOrientation = BorderOrientation.Outside
        }, [automationContainer, cropShopContainer, shopContainer, questContainer]);

        AddComponent(itemGrid);


        return this;
    }
}