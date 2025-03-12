using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine.Debug;

public class Stats : Page
{
    private Text _fpsText;

    public override Page Initialize()
    {
        IsVisible = true;
        Id = "stats";

        _fpsText = new Text("stats-fps", new Text.TextOptions
        {
            Content = $"FPS: 0",
            FontSize = 16,
            FontColor = Color.Red,
            Position = new Vector2(5)
        });

        Components.Add(_fpsText);

        return this;
    }

    public override void Update(MouseState mouseState)
    {
        // Logger.Log($"{GetType().FullName}::Update");
        _fpsText.SetContent($"FPS: {((int)RanchMayhemEngine.Framerate).ToString()}");
        base.Update(mouseState);
    }
}