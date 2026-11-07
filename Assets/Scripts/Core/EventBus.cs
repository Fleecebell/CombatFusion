using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventBus
{
    private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();
    public static EventBus Instance { get; } = new EventBus();
    public void Subscribe<T>(Action<T> handler) where T : struct
    {
        var type = typeof(T);
        _handlers.TryGetValue(type, out var existing);
        _handlers[type] = Delegate.Combine(existing, handler);
    }
    public void Unsubscribe<T>(Action<T> handler) where T : struct
    {
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var existing)) return;

        var remaining = Delegate.Remove(existing, handler);
        if (remaining == null) _handlers.Remove(type);
        else _handlers[type] = remaining;
    }
    public void Publish<T>(T evt) where T : struct
    {
        if (!_handlers.TryGetValue(typeof(T), out var raw)) return;

        var callbacks = raw as Action<T>;
        if (callbacks == null) return;

        callbacks(evt); //广播
    }

    public void Clear() => _handlers.Clear();
}