#nullable enable
using UnityEngine;

namespace BaseGame
{
    public class IdentifiableAsset : ScriptableObject, IIdentifiable
    {
        [SerializeField]
        private SerializedID id = new();

        private Log? log;

        public ID ID => id.ID;
        
        protected Log Log
        {
            get
            {
                if (log is null)
                {
                    log = new Log(ToString());
                }

                return log;
            }
        }
    }
}