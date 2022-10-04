#nullable enable
using System;

namespace BaseGame
{
    public abstract class PlayerItem : Item, IUnityLifecycle
    {
        private int playerId;
        private IPlayer? player;

        public IPlayer Player
        {
            get
            {
                if (player is null)
                {
                    player = PlayerLoop.Get<IPlayer>(new ID(playerId));
                }

                return player;
            }
        }

        public PlayerItem(ID id) : base(id) { }

        protected virtual void OnEnabled(IPlayer player) { }
        protected virtual void OnDisabled(IPlayer player) { }
        protected virtual void AddedTo(IPlayer player) { }
        protected virtual void RemovedFrom(IPlayer player) { }

        protected sealed override void AddedTo(IInventory owner)
        {
            foreach (IPlayer possiblePlayer in PlayerLoop.GetAll<IPlayer>())
            {
                if (possiblePlayer.Inventory == owner)
                {
                    playerId = possiblePlayer.ID.GetHashCode();
                    player = possiblePlayer;
                    AddedTo(player);
                    return;
                }
            }

            throw new Exception("Ability added to an inventory that is not owned by any player.");
        }

        protected sealed override void RemovedFrom(IInventory owner)
        {
            if (Player is IPlayer player)
            {
                RemovedFrom(player);
            }

            playerId = 0;
        }

        void IUnityLifecycle.OnEnabled() => OnEnabled(Player);
        void IUnityLifecycle.OnDisabled() => OnDisabled(Player);
    }
}
