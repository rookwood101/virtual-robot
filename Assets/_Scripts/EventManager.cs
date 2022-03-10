#nullable enable
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed class EventType {
    public readonly Type EventParameterType;
    public EventType(Type eventParameterType) {
        this.EventParameterType = eventParameterType;
    }
}

public sealed class Null {
    private Null() { }
}

public class EventManager
{
    private Dictionary<EventType, UnityEvent<object?>> eventDictionary = new Dictionary<EventType, UnityEvent<object?>>();
    private Dictionary<EventType, TaskCompletionSource<object?>> eventCompleteAwaiters = new Dictionary<EventType, TaskCompletionSource<object?>>();

    public void AddListener(EventType eventType, UnityAction<object?> listener)
    {
        UnityEvent<object?> thisEvent;
        if (eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<object?>();
            thisEvent.AddListener(listener);
            eventDictionary.Add(eventType, thisEvent);
        }
    }

    public void RemoveListener(EventType eventType, UnityAction<object?> listener)
    {
        UnityEvent<object?> thisEvent;
        if (eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(EventType eventType, object? parameter)
    {
        if (!eventType.EventParameterType.IsInstanceOfType(parameter) && !(parameter is null)) {

            throw new ArgumentException($"Event parameter of type {parameter.GetType()} is not compatible with event type");
        }

        UnityEvent<object?> thisEvent;
        if (eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.Invoke(parameter);
        }
        TaskCompletionSource<object?> taskCompletion;
        if (eventCompleteAwaiters.TryGetValue(eventType, out taskCompletion))
        {
            eventCompleteAwaiters.Remove(eventType);
            taskCompletion.SetResult(parameter);
        }
    }

    public async Task<object?> WaitForEvent(EventType eventType)
    {
        TaskCompletionSource<object?> taskCompletion;
        if (!eventCompleteAwaiters.TryGetValue(eventType, out taskCompletion))
        {
            taskCompletion = new TaskCompletionSource<object?>();
            eventCompleteAwaiters[eventType] = taskCompletion;
        }

        return await taskCompletion.Task;
    }

    public async Task<object?> WaitForEventUntil(EventType eventType, object? value)
    {
        object? eventValue;
        do
        {
            eventValue = await WaitForEvent(eventType);
        } while (!object.Equals(eventValue, value));

        return eventValue;
    }
}