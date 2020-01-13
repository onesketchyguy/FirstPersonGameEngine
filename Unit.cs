using JunoEngine.Systems;

namespace FirstPersonGameEngine
{
    public class Unit
    {
        public char character;

        public bool IsStill
        {
            get
            {
                return lastPosition == position && lastAngle == angle;
            }
        }

        private Vector3 lastPosition;
        public Vector3 position;
        private float lastAngle;
        public float angle;

        public void SetStill()
        {
            lastPosition = position;
            lastAngle = angle;
        }

        public Unit(Vector3 position, float angle = 0, char character = 'o')
        {
            this.position = position;
            this.angle = angle;
            this.character = character;

            SetStill();
        }
    }
}