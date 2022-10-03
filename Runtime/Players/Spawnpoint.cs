#nullable enable

using UnityEngine;

namespace BaseGame
{
    public class Spawnpoint : MonoBehaviour
    {
        public virtual Vector3 Position => transform.position;
        public virtual Quaternion Rotation => transform.rotation;
        public virtual bool CanUseSpawnpoint(User user) => true;
    }
}
