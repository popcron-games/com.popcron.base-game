#nullable enable

namespace BaseGame
{
    public readonly struct ManagerHasInitialized : IEvent
    {
        public readonly IManager manager;
        
        public ManagerHasInitialized(IManager manager)
        {
            this.manager = manager;
        }
    }
}