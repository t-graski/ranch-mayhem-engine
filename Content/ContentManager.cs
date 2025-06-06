﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ranch_mayhem_engine.UI;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace ranch_mayhem_engine.Content;

public static class ContentManager
{
    private static readonly Dictionary<string, Texture2D> Contents = [];
    private static readonly Dictionary<string, Dictionary<int, SpriteFont>> Fonts = [];

    private static readonly Dictionary<string, AtlasItem> Sprites = [];
    private static Texture2D _textureAtlas;

    public static void LoadContentFile(Microsoft.Xna.Framework.Content.ContentManager content, string path)
    {
        var itemsToLoad = LoadJson<ContentItem>(path);

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
                        $"{typeof(ContentManager)}::LoadContent Trying to load content with unknown type, name={item.Name}",
                        LogLevel.Warning);
                    break;
            }
        }

        Logger.Log($"Loaded {Contents.Count} sprites");
        Logger.Log($"Loaded {Fonts.Count} fonts");
    }

    public static void LoadTextureAtlas(Microsoft.Xna.Framework.Content.ContentManager content, string path,
        string atlasName)
    {
        var items = LoadJson<AtlasItem>(path);

        foreach (var atlasItem in items)
        {
            Sprites.Add(atlasItem.Name, atlasItem);
        }

        _textureAtlas = content.Load<Texture2D>(atlasName);
        Logger.Log($"Loaded {Sprites.Count} atlas sprites");
    }

    public static Texture2D? GetAtlasSprite(string name)
    {
        if (Sprites.TryGetValue(name, out var item))
        {
            var extracted = new Texture2D(RanchMayhemEngine.UiManager.GraphicsDevice, item.Width, item.Height);

            var data = new Color[item.Width * item.Height];
            _textureAtlas.GetData(0, new Rectangle((int)item.Position.X, (int)item.Position.Y, item.Width, item.Height),
                data, 0, data.Length);

            extracted.SetData(data);

            return extracted;
        }

        return null;
    }

    private static void LoadSprite(ContentItem item, Microsoft.Xna.Framework.Content.ContentManager content)
    {
        Contents.Add(item.Name, content.Load<Texture2D>(item.Name));
    }

    private static void LoadFont(ContentItem item, Microsoft.Xna.Framework.Content.ContentManager content)
    {
        if (item.Sizes == null || item.Sizes.Count == 0)
        {
            Logger.Log($"{typeof(ContentManager)}::LoadFont No sizes defined for font={item.Name}");
            return;
        }

        foreach (var size in item.Sizes)
        {
            var font = content.Load<SpriteFont>($"{item.Name}{size}");

            if (!Fonts.TryGetValue(item.Name, out var fontSizes))
            {
                fontSizes = new Dictionary<int, SpriteFont>();
                Fonts[item.Name] = fontSizes;
            }

            fontSizes[size] = font;
        }
    }

    public static Texture2D GetTexture(string name) => Contents[name];

    private static int GetClosestSize(string name, int size)
    {
        // available: 12, 16, 32
        // want: 20
        // should: 16

        var diff = 0;
        var closestSize = Fonts[name].First().Key;

        foreach (var (key, value) in Fonts[name])
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

    private static bool HasSize(string name, int size)
    {
        return Fonts[name].ContainsKey(size);
    }

    public static SpriteFont GetFont(string name, int size)
    {
        if (!HasSize(name, size))
        {
            return Fonts[name][GetClosestSize(name, size)];
        }

        return Fonts[name][size];
    }

    public static (SpriteFont font, int size) GetFontWithSize(string name, int size)
    {
        if (!HasSize(name, size))
        {
            var closest = GetClosestSize(name, size);
            return (Fonts[name][closest], closest);
        }

        return (Fonts[name][size], size);
    }

    private static List<T> LoadJson<T>(string path)
    {
        using var file = File.OpenText(path);
        var serializer = new JsonSerializer();
        var items = (List<T>)serializer.Deserialize(file, typeof(List<T>))!;
        return items;
    }

    private class ContentItem
    {
        public string Name;
        public string Type = "unknown";
        public List<int> Sizes;
    }

    private class AtlasItem
    {
        public string Name;
        public Vector2 Position;
        public int Width;
        public int Height;
    }
}
