#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BaseGame
{
    [Serializable]
    public class DefaultUserBehaviour : UserBehaviour
    {
        public DefaultUserBehaviour(User user) : base(user) { }
        
        public override bool TryGetInputState(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out InputState state)
        {
            if (name == "mousePosition")
            {
                state = new InputState(vector: Input.mousePosition);
                return true;
            }
            else if (name == "mouseDelta")
            {
                state = new InputState(vector: new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
                return true;
            }
            else if (name == "attack")
            {
                state = new InputState(isPressed: Input.GetMouseButton(0));
                return true;
            }
            else if (name == "jump")
            {
                state = new InputState(isPressed: Input.GetKey(KeyCode.Space));
                return true;
            }
            else if (name == "moveDirection")
            {
                Vector2 wasd = Vector2.zero;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    wasd.x -= 1f;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    wasd.x += 1f;
                }

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    wasd.y += 1f;
                }
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    wasd.y -= 1f;
                }

                state = new InputState(vector: wasd.normalized);
                return true;
            }

            state = default;
            return false;
        }
    }
}