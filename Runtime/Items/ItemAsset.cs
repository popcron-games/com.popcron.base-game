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
                    return item;
                }
                else
                {
                    throw ExceptionBuilder.Format("No item assigned to {0}", this);
                }
            }
        }

        bool IValidate.Validate()
        {
            if (item is not null)
            {
                if (item.GetFieldValue("id") is int id)
                {
                    int assetId = ID.GetHashCode();
                    if (id != assetId)
                    {
                        item.SetFieldValue("id", assetId);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Log.LogErrorFormat("Item {0} does not have an ID field", item);
                    return false;
                }
            }
            else
            {
                Log.LogErrorFormat("No item assigned to {0}", this);
                return false;
            }
        }
    }
}
