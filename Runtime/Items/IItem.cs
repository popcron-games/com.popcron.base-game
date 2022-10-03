#nullable enable

namespace BaseGame
{
    public interface IItem : IIdentifiable
    {
        ID? PrefabID { get; }

        void AddedTo(IInventory owner);
        void RemovedFrom(IInventory owner);
    }
}