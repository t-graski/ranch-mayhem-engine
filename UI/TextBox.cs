using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public class TextBox : UIComponent
{
    private readonly StringBuilder _text;
    private bool _isActive;
    private readonly SpriteFont _font;
    private Texture2D _background;
    private readonly Action<string> _onSubmit;


    public TextBox(string id, UIComponentOptions options, Action<string> onSubmit, UIComponent parent = null,
        bool scale = true) : base(id,
        options, parent, scale)
    {
        _onSubmit = onSubmit;
        _text = new StringBuilder();
        _isActive = false;
        _background = options.Texture;
        _font = RanchMayhemEngine.ContentManager.GetFont("Joystix", 16);
    }

    public override void Update()
    {
        if (IsClicked)
        {
            _isActive = true;
        }

        if (_isActive)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var key in keys)
            {
                Logger.Log($"new key = {KeyboardInput.IsNewKeyPress(key)}");
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
                _text.Append(' ');
                break;
            default:
            {
                var character = KeyboardInput.GetCharFromKey(key);
                if (character.HasValue)
                {
                    _text.Append(character.Value);
                }

                break;
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.DrawString(_font, _text.ToString(), new Vector2(Bounds.X + 5, Bounds.Y + 5), Color.Red);
    }
}