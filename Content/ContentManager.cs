using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace ranch_mayhem_engine.Content;

public class ContentManager
{
    private readonly Dictionary<string, Texture2D> _contents =  [];
    private readonly Dictionary<string, Dictionary<int, SpriteFont>> _fonts =  [];

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
    {
        var itemsToLoad = LoadJson();

        foreach (var item in itemsToLoad)
        {
            switch (item.Type)
            {
                case "sprite":
                    LoadSprite(item, content);
                    break;
                case "font":
                    LoadFont(item, content);
                    break;
                case "unknown":
                default:
                    Logger.Log(
                        $"{GetType().FullName}::LoadContent Trying to load content with unknown type, name={item.Name}",
                        Logger.LogLevel.Warning);
                    break;
            }
        }

        Logger.Log($"Loaded {_contents.Count} sprites");
        Logger.Log($"Loaded {_fonts.Count} fonts");
    }

    private void LoadSprite(ContentItem item, Microsoft.Xna.Framework.Content.ContentManager content)
    {
        _contents.Add(item.Name, content.Load<Texture2D>(item.Name));
    }

    private void LoadFont(ContentItem item, Microsoft.Xna.Framework.Content.ContentManager content)
    {
        if (item.Sizes == null || item.Sizes.Count < 1)
        {
            Logger.Log($"{GetType().FullName}::LoadFont No sizes defined for font={item.Name}");
            return;
        }

        foreach (var size in item.Sizes)
        {
            var font = content.Load<SpriteFont>($"{item.Name}{size}");

            if (!_fonts.TryGetValue(item.Name, out var fontSizes))
            {
                fontSizes = new Dictionary<int, SpriteFont>();
                _fonts[item.Name] = fontSizes;
            }

            fontSizes[size] = font;
        }
    }

    public Texture2D GetTexture(string name) => _contents[name];

    public int GetClosestSize(string name, int size)
    {
        // available: 12, 16, 32
        // want: 20
        // should: 16

        var diff = 0;
        var closestSize = _fonts[name].First().Key;

        foreach (var (key, value) in _fonts[name])
        {
            if (key == size) return key;

            if (diff == 0)
            {
                diff = Math.Abs(size - key);
            }

            if (Math.Abs(size - key) < diff)
            {
                diff = Math.Abs(size - key);
                closestSize = key;
            }
        }

        return closestSize;
    }

    private bool HasSize(string name, int size)
    {
        return _fonts[name].ContainsKey(size);
    }

    public SpriteFont GetFont(string name, int size)
    {
        if (!HasSize(name, size))
        {
            return _fonts[name][GetClosestSize(name, size)];
        }

        return _fonts[name][size];
    }

    public (SpriteFont font, int size) GetFontWithSize(string name, int size)
    {
        if (!HasSize(name, size))
        {
            var closest = GetClosestSize(name, size);
            return (_fonts[name][closest], closest);
        }

        return (_fonts[name][size], size);
    }

    private static List<ContentItem> LoadJson()
    {
        using var file = File.OpenText("./content/content.json");
        var serializer = new JsonSerializer();
        var items = (List<ContentItem>)serializer.Deserialize(file, typeof(List<ContentItem>));
        return items;
    }

    private class ContentItem
    {
        public string Name;
        public string Type = "unknown";
        public List<int> Sizes;
    }
}