namespace BaseGame
{
    public interface IStaticListener<T> where T : IEvent
    {
        void OnEvent(T e);
    }
}