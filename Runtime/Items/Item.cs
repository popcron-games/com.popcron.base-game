#nullable enable
using System;
using UnityEngine;

namespace BaseGame
{
    [Serializable]
    public class Item : IItem
    {
        [SerializeField, FixedString]
        private int id;

        [SerializeField, FixedString]
        private int prefabId;

        private Log? log;

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

        public ID ID
        {
            get => new ID(id);
            protected set => id = value.GetHashCode();
        }

        public ID? PrefabID
        {
            get
            {
                if (prefabId != 0)
                {
                    return new ID(prefabId);
                }
                else
                {
                    return null;
                }
            }
        }

        public Item(ID id, ID? prefabId = null)
        {
            ID = id;
            this.prefabId = prefabId?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            if (prefabId != 0)
            {
                return ValueStringBuilder.Format("{0} ({2}:{1})", GetType().Name, ID, new ID(prefabId)).ToString();
            }
            else
            {
                return ValueStringBuilder.Format("{0} ({1})", GetType().Name, ID).ToString();
            }
        }

        protected virtual void AddedTo(IInventory owner) { }
        protected virtual void RemovedFrom(IInventory owner) { }

        void IItem.AddedTo(IInventory owner)
        {
            AddedTo(owner);
        }

        void IItem.RemovedFrom(IInventory owner)
        {
            RemovedFrom(owner);
        }
    }
}