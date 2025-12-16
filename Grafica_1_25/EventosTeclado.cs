using OpenTK.Input;

namespace Grafica_1_25
{
    class EventosTeclado
    {
        public void CambioDeEscala(KeyboardState input, ref float scale)
        {
            if (input.IsKeyDown(Key.Plus) || input.IsKeyDown(Key.KeypadPlus))
            {
                scale += 0.5f;
            }
            else if (input.IsKeyDown(Key.Minus) || input.IsKeyDown(Key.KeypadMinus))
            {
                scale = System.Math.Max(0.1f, scale - 0.5f);
            }
        }
    }
}