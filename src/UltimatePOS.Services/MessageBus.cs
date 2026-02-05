using System;
using System.Collections.Generic;
using System.Linq;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

/// <summary>
/// Simple message bus implementation using weak references
/// </summary>
public class MessageBus : IMessageBus
{
    private readonly Dictionary<Type, List<Subscription>> _subscriptions = new();
    private readonly object _lock = new();

    public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : class
    {
        Subscribe(handler, Guid.NewGuid());
    }

    public IDisposable Subscribe<TMessage>(Action<TMessage> handler, object token) where TMessage : class
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));
        if (token == null) throw new ArgumentNullException(nameof(token));

        var messageType = typeof(TMessage);
        lock (_lock)
        {
            if (!_subscriptions.ContainsKey(messageType))
            {
                _subscriptions[messageType] = new List<Subscription>();
            }

            var subscription = new Subscription(handler, token);
            _subscriptions[messageType].Add(subscription);

            return new Unsubscriber<TMessage>(this, handler);
        }
    }

    public void Unsubscribe<TMessage>(Action<TMessage> handler) where TMessage : class
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(TMessage);
        lock (_lock)
        {
            if (_subscriptions.TryGetValue(messageType, out var subscriptions))
            {
                subscriptions.RemoveAll(s => s.HandlerReference.Target as Action<TMessage> == handler);
            }
        }
    }

    public void Unsubscribe(object token)
    {
        if (token == null) throw new ArgumentNullException(nameof(token));

        lock (_lock)
        {
            foreach (var subscriptions in _subscriptions.Values)
            {
                subscriptions.RemoveAll(s => s.Token.Equals(token));
            }
        }
    }

    public void Publish<TMessage>(TMessage message) where TMessage : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        var messageType = typeof(TMessage);
        List<Subscription> subscriptionsCopy;

        lock (_lock)
        {
            if (!_subscriptions.TryGetValue(messageType, out var subscriptions))
            {
                return;
            }

            // Remove dead references and get a copy
            subscriptions.RemoveAll(s => !s.HandlerReference.IsAlive);
            subscriptionsCopy = subscriptions.ToList();
        }

        // Invoke handlers outside the lock to avoid deadlocks
        foreach (var subscription in subscriptionsCopy)
        {
            if (subscription.HandlerReference.Target is Action<TMessage> handler)
            {
                try
                {
                    handler(message);
                }
                catch
                {
                    // Swallow exceptions to prevent one handler from breaking others
                    // In production, you might want to log this
                }
            }
        }
    }

    private class Subscription
    {
        public WeakReference HandlerReference { get; }
        public object Token { get; }

        public Subscription(object handler, object token)
        {
            HandlerReference = new WeakReference(handler);
            Token = token;
        }
    }

    private class Unsubscriber<TMessage> : IDisposable where TMessage : class
    {
        private readonly MessageBus _messageBus;
        private readonly Action<TMessage> _handler;
        private bool _disposed;

        public Unsubscriber(MessageBus messageBus, Action<TMessage> handler)
        {
            _messageBus = messageBus;
            _handler = handler;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _messageBus.Unsubscribe(_handler);
                _disposed = true;
            }
        }
    }
}
