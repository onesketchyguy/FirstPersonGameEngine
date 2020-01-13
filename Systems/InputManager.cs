using System.Collections;
using System.Windows.Forms;

namespace JunoEngine.Systems
{
    public delegate void InputCall(Keys keypressed);

    public static class Input
    {
        public static InputCall keyPressed;
        public static InputCall keyReleased;

        public static Axis horizontal = new Axis(new Keys[] { Keys.Left, Keys.A }, new Keys[] { Keys.Right, Keys.D });
        public static Axis vertical = new Axis(new Keys[] { Keys.Down, Keys.S }, new Keys[] { Keys.Up, Keys.W });

        // Load a list of available keyboard buttons
        private static readonly Hashtable keytable_Down = new Hashtable();

        // Check if a particular button has been pressed
        public static bool KeyDown(Keys key)
        {
            if (keytable_Down[key] == null)
            {
                keytable_Down.Add(key, false);

                return false;
            }
            var val = (bool)keytable_Down[key];

            return val;
        }

        // Detect if a keyboard button is pressed
        public static void ChangeState(Keys key, bool state)
        {
            keytable_Down[key] = state;

            if (state == true)
            {
                if (keyPressed != null)
                    keyPressed.Invoke(key);
            }
            else
            {
                if (keyReleased != null)
                    keyReleased.Invoke(key);
            }
        }
    }

    public class Axis
    {
        private float value;

        private readonly Hashtable negativeKeytable = new Hashtable();
        private readonly Hashtable posativeKeytable = new Hashtable();

        private void CheckForKeyDown(Keys key)
        {
            if (negativeKeytable.ContainsKey(key))
            {
                // Negate value
                value = -1;
            }
            if (posativeKeytable.ContainsKey(key))
            {
                // Apply to value
                value = 1;
            }
        }

        private void CheckForKeyUp(Keys key)
        {
            if (negativeKeytable.ContainsKey(key))
            {
                if (value < 0)
                {
                    value = 0;
                }
            }
            if (posativeKeytable.ContainsKey(key))
            {
                if (value > 0)
                {
                    value = 0;
                }
            }
        }

        public Axis(Keys[] negative, Keys[] posative)
        {
            foreach (var item in negative)
            {
                negativeKeytable.Add(item, false);
            }

            foreach (var item in posative)
            {
                posativeKeytable.Add(item, false);
            }

            value = 0;

            Input.keyPressed += CheckForKeyDown;
            Input.keyReleased += CheckForKeyUp;
        }

        public static implicit operator int(Axis axis)
        {
            return (int)axis.value;
        }

        public static implicit operator float(Axis axis)
        {
            return axis.value;
        }
    }
}