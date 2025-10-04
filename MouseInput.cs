using Microsoft.Xna.Framework.Input;

namespace ranch_mayhem_engine;

public static class MouseInput
{
    public static MouseState PreviousState;
    public static MouseState CurrentState;

    public static void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }
}
