using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace ranch_mayhem_engine.Content;

public class SpriteSheet
{
    public Texture2D Texture { get; }
    private readonly Dictionary<string, Rectangle> _regions;

    public SpriteSheet(Texture2D texture, string filePath)
    {
        Texture = texture;
        _regions = new Dictionary<string, Rectangle>();

        var json = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<SpriteSheetData>(json);

        foreach (var (key, value) in data.Frames)
        {
            var frame = value.FrameInfo;
            _regions[key] = new Rectangle(frame.X, frame.Y, frame.W, frame.H);
        }
    }


    public Rectangle GetRegion(string name)
    {
        return _regions.TryGetValue(name, out var region) ? region : Rectangle.Empty;
    }

    private class SpriteSheetData
    {
        public Dictionary<string, SpriteSheetFrame> Frames { get; set; }
    }

    private class SpriteSheetFrame
    {
        public SpriteSheetFrameInfo FrameInfo { get; set; }
    }

    private class SpriteSheetFrameInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
    }
}