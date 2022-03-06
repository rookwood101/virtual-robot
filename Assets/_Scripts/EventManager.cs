using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EventManager : MonoBehaviour
{
    private Dictionary<CustomEventType, UnityEvent<object>> eventDictionary;
    private Dictionary<CustomEventType, TaskCompletionSource<object>> eventCompleteAwaiters;

    private static EventManager eventManager;

    public static EventManager Instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<CustomEventType, UnityEvent<object>>();
            eventCompleteAwaiters = new Dictionary<CustomEventType, TaskCompletionSource<object>>();
        }
    }

    public static void AddListener(CustomEventType eventName, UnityAction<object> listener)
    {
        UnityEvent<object> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<object>();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void RemoveListener(CustomEventType eventName, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        UnityEvent<object> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(CustomEventType eventName, object parameter)
    {
        UnityEvent<object> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(parameter);
        }
        TaskCompletionSource<object> taskCompletion;
        if (Instance.eventCompleteAwaiters.TryGetValue(eventName, out taskCompletion))
        {
            Instance.eventCompleteAwaiters.Remove(eventName);
            taskCompletion.SetResult(parameter);
        }
    }

    public static async Task<object> WaitForEvent(CustomEventType eventName)
    {
        TaskCompletionSource<object> taskCompletion;
        if (!Instance.eventCompleteAwaiters.TryGetValue(eventName, out taskCompletion))
        {
            taskCompletion = new TaskCompletionSource<object>();
            Instance.eventCompleteAwaiters[eventName] = taskCompletion;
        }

        return await taskCompletion.Task;
    }

    public static async Task<T> WaitForEventUntil<T>(CustomEventType eventName, T value) where T : class
    {
        T eventValue;
        do
        {
            eventValue = (T)await EventManager.WaitForEvent(eventName);
        } while (!eventValue.Equals(value));

        return eventValue;
    }
}