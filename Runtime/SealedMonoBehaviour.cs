#nullable enable

namespace BaseGame
{
    public class SealedMonoBehaviour : UnityEngine.MonoBehaviour
    {
        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void OnValidate() { }
    }
}