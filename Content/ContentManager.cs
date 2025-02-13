using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace ranch_mayhem_engine.Content;

public class ContentManager
{
    private readonly Dictionary<string, Texture2D> _contents = [];

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
    {
        var itemsToLoad = LoadJson();

        foreach (var item in itemsToLoad)
        {
            _contents.Add(item.name, content.Load<Texture2D>(item.name));
        }
    }

    public Texture2D GetTexture(string name) => _contents[name];

    private static List<ContentItem> LoadJson()
    {
        using var file = File.OpenText("./content/content.json");
        var serializer = new JsonSerializer();
        var items = (List<ContentItem>)serializer.Deserialize(file, typeof(List<ContentItem>));
        return items;
    }

    private class ContentItem
    {
        public string name;
    }
}