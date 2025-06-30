using System;
using Interface;
using System.Collections.Generic;

public class MessageBus
{
    private static MessageBus _instance;
    public static MessageBus Instance => _instance ??= new MessageBus();

    private readonly Dictionary<Type, List<Action<IMessage>>> _subscribers = new();

    public void Subscribe<T>(Action<T> callback) where T : IMessage
    {
        Type type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Action<IMessage>>();

        _subscribers[type].Add(msg => callback((T)msg));
    }

    public void Unsubscribe<T>(Action<T> callback) where T : IMessage
    {
        Type type = typeof(T);
        if (_subscribers.TryGetValue(type, out var list))
        {
            list.RemoveAll(action => action.Equals((Action<IMessage>)(msg => callback((T)msg))));
        }
    }

    public void Publish<T>(T message) where T : IMessage
    {
        Type type = typeof(T);
        if (_subscribers.TryGetValue(type, out var list))
        {
            foreach (var callback in list)
                callback.Invoke(message);
        }
    }
}