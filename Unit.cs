using JunoEngine.Systems;
using System;

namespace FirstPersonGameEngine
{
    public class Unit
    {
        /// <summary>
        /// The Text Character that represents this unit
        /// </summary>
        public char character;

        public bool IsStill
        {
            get
            {
                return lastPosition == position && lastAngle == angle;
            }
        }

        public void SetStill()
        {
            lastPosition = position;
            lastAngle = angle;
        }

        private Vector3 lastPosition;
        public Vector3 position;
        private float lastAngle;
        public float angle;

        public Unit(Vector3 position, float angle = 0, char character = 'o')
        {
            this.position = position;
            // Calculate an angle based on a circle
            angle %= 360;

            var ang = (360 - angle) / 360;
            ang *= (3.14f * 2);

            this.angle = ang;
            this.character = character;

            SetStill();
        }
    }
}