#nullable enable

using UnityEngine;

namespace BaseGame
{
    public readonly struct InputState
    {
        public static readonly InputState None = new();

        public readonly bool isPressed;
        public readonly float value;
        public readonly Vector3 vector;

        public readonly bool IsActive
        {
            get
            {
                const float threshold = 0.5f;
                return isPressed || value > threshold || vector.sqrMagnitude > threshold;
            }
        }

        public InputState(bool isPressed = false, float value = 0f, Vector3 vector = default)
        {
            this.isPressed = isPressed;
            this.value = value;
            this.vector = vector;
        }
    }
}