using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine.UI;

public class TextBox : UIComponent
{
    private StringBuilder _text;
    private bool _isActive;
    private SpriteFont _font;
    private Texture2D _background;
    private Action<string> _onSubmit;


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
                if (KeyboardInput.IsNewKeyPress(key))
                {
                    HandleKeyPress(key);
                }
            }
        }
    }

    private void HandleKeyPress(Keys key)
    {
        if (key == Keys.Back && _text.Length > 0)
        {
            _text.Remove(_text.Length - 1, 1);
        }
        else if (key == Keys.Enter)
        {
            _onSubmit.Invoke(_text.ToString());
            _text.Clear();
        }
        else
        {
            var character = KeyboardInput.GetCharFromKey(key);
            if (character.HasValue)
            {
                _text.Append(character.Value);
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.DrawString(_font, _text.ToString(), new Vector2(_bounds.X + 5, _bounds.Y + 5), Color.Red);
    }
}