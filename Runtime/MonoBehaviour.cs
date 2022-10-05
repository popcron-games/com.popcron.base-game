#nullable enable

namespace BaseGame
{
    public class MonoBehaviour : SealedMonoBehaviour, IComponent, IUpdateLoop
    {
        private Log? log;

        public Log Log
        {
            get
            {
                if (log is null)
                {
                    log = new(name);
                }

                return log;
            }
        }

        protected override sealed void Awake() => OnAwake();
        protected override sealed void Start() => OnStart();

        protected override sealed void OnEnable()
        {
            PlayerLoop.Add(this);
            OnEnabled();
        }

        protected override sealed void OnDisable()
        {
            PlayerLoop.Remove(this);
            OnDisabled();
        }

        protected override sealed void OnValidate()
        {
            Validator.PerformValidation(this);
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnEnabled() { }
        protected virtual void OnDisabled() { }
        protected virtual void OnReset() { }
        protected virtual void OnUpdate(float delta) { }
        protected virtual void OnFixedUpdate(float delta) { }
        protected virtual void OnLateUpdate(float delta) { }

        void IUpdateLoop.OnUpdate(float delta) => OnUpdate(delta);
    }
}