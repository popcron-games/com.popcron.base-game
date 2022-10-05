#nullable enable
using UnityEngine;

namespace BaseGame
{
    [CreateAssetMenu]
    public class ItemAsset : IdentifiableAsset, IValidate
    {
        [SerializeReference, SerializeReferenceButton]
        private IItem? item;

        public IItem PrefabItem
        {
            get
            {
                if (item is not null)
                {
                    _ = ID; //to make sure that a string id is present for the item
                    EnsureItemIDIsSet();
                    return item;
                }
                else
                {
                    throw ExceptionBuilder.Format("No item assigned to {0}", this);
                }
            }
        }

        public IItem Create(ID? newId = null)
        {
            return PrefabItem.Clone(newId);
        }

        public T Create<T>(ID? newId = null) where T : IItem
        {
            return (T)PrefabItem.Clone(newId);
        }

        private void EnsureItemIDIsSet()
        {
            if (item is not null)
            {
                if (item.GetFieldValue("id") is int id)
                {
                    int assetId = ID.GetHashCode();
                    if (id != assetId)
                    {
                        item.SetFieldValue("id", assetId);
                    }
                }
                else
                {
                    Log.LogErrorFormat("Item {0} does not have an ID field", item);
                }
            }
            else
            {
                Log.LogErrorFormat("No item assigned to {0}", this);
            }
        }

        bool IValidate.Validate()
        {
            bool changed = false;
            if (item is not null)
            {
                ID previousId = item.ID;
                EnsureItemIDIsSet();
                return previousId != item.ID;
            }
            else
            {
                Log.LogErrorFormat("No item assigned to {0}", this);
            }

            return changed;
        }
    }
}
