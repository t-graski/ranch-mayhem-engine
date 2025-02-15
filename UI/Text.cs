namespace ranch_mayhem_engine.UI;

public class Text : UIComponent
{
    public Text(string id, UIComponentOptions options, UIComponent parent = null, bool scale = true) : base(id, options,
        parent, scale)
    {
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }

    public class TextOptions()
    {
        public string Content = "";
        public int Size = 12;
        public TextAlignment Alignment = TextAlignment.Center;
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }
}