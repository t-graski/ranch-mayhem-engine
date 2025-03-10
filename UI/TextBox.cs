using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public class TextBox : UIComponent
{
    private readonly StringBuilder _text;
    private readonly SpriteFont _font;
    private Texture2D _background;

    private readonly Action<string> _onSubmit;
    private readonly TextBoxOptions _textBoxOptions;

    public TextBox(string id, TextBoxOptions options, Action<string> onSubmit, UIComponent parent = null,
        bool scale = true) : base(id,
        options, parent, scale)
    {
        _onSubmit = onSubmit;
        _text = new StringBuilder();
        _background = options.Texture;
        _font = RanchMayhemEngine.ContentManager.GetFont(RanchMayhemEngineConstants.DefaultFont, options.FontSize);

        _textBoxOptions = options;
    }

    public override void Update()
    {
        if (IsClicked)
        {
            RanchMayhemEngine.KeyboardManager.IsInTextBox = true;
        }

        if (IsActive)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var key in keys)
            {
                if (KeyboardInput.IsNewKeyPress(key))
                {
                    HandleKeyPress(key);
                }
            }
        }
    }

    private void HandleKeyPress(Keys key)
    {
        switch (key)
        {
            case Keys.Back when _text.Length > 0:
                _text.Remove(_text.Length - 1, 1);
                break;
            case Keys.Enter:
                _onSubmit.Invoke(_text.ToString());
                _text.Clear();
                break;
            case Keys.Space:
                if (_text.Length == _textBoxOptions.MaxLength) break;
                _text.Append(' ');
                break;
            default:
            {
                if (_text.Length == _textBoxOptions.MaxLength) break;
                var character = KeyboardInput.GetCharFromKey(key);
                if (character.HasValue)
                {
                    _text.Append(character.Value);
                }

                break;
            }
        }
    }

    public override void OffActive()
    {
        RanchMayhemEngine.KeyboardManager.IsInTextBox = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.DrawString(_font, _text.ToString(), new Vector2(Bounds.X + 5, Bounds.Y + 5),
            _textBoxOptions.FontColor);
    }

    public class TextBoxOptions : UIComponentOptions
    {
        public int FontSize { get; set; } = RanchMayhemEngineConstants.DefaultFontSize;
        public Color FontColor { get; set; } = RanchMayhemEngineConstants.DefaultFontColor;
        public int MaxLength { get; set; } = 100;
    }
}