using System;
using BaseGame;
using BaseGame.Events;

public static class EventExtensions
{
    public static void Dispatch<T>(this T e) where T : IEvent
    {
        GlobalEventBus<T>.Dispatch(e);
    }

    public static void ListenTo<_, E>(this _ obj, Action<E> callback) where E : IEvent
    {
        GlobalEventBus<E>.AddListener(callback);
    }
}