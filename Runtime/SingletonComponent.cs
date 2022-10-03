#nullable enable

namespace BaseGame
{
    public class SingletonComponent<T> : MonoBehaviour where T : SingletonComponent<T>
    {
        protected static Log log;
        private static T? instance;

        public static T Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance is null)
                    {
                        throw ExceptionBuilder.Format("No instance of {0} found in scene.", typeof(T));
                    }
                }

                return instance;
            }
        }

        static SingletonComponent()
        {
            log = new Log(typeof(T).Name);
        }

        protected override void OnEnabled()
        {
            instance = this as T;
        }

        protected override void OnDisabled()
        {
            instance = null;
        }
    }
}