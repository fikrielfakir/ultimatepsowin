using System;

namespace UltimatePOS.Core.Interfaces;

/// <summary>
/// Simple message bus for pub/sub communication between components
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Subscribe to messages of a specific type
    /// </summary>
    void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : class;

    /// <summary>
    /// Subscribe to messages with a token for selective unsubscription
    /// </summary>
    IDisposable Subscribe<TMessage>(Action<TMessage> handler, object token) where TMessage : class;

    /// <summary>
    /// Unsubscribe from messages of a specific type
    /// </summary>
    void Unsubscribe<TMessage>(Action<TMessage> handler) where TMessage : class;

    /// <summary>
    /// Unsubscribe all handlers for a specific token
    /// </summary>
    void Unsubscribe(object token);

    /// <summary>
    /// Publish a message to all subscribers
    /// </summary>
    void Publish<TMessage>(TMessage message) where TMessage : class;
}
