using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Dev90Lib.Observer
{
    public class EventDispatcher : MonoBehaviour
    {

        #region Singleton
        private static EventDispatcher s_instance;
        public static EventDispatcher Instance
        {
            get
            {
                // if instance_s not exist create  new one 
                if (s_instance == null)
                {
                    // Creat game object and add  EventDispatcher component
                    GameObject go = new GameObject();
                    go.name = "Singleton EventDispatcher";
                    s_instance = go.AddComponent<EventDispatcher>();
                }
                return s_instance;
            }

            private set { }
        }

        void Awake()
        {
            // Check other instance exist in scene
            if (s_instance != null && s_instance.GetInstanceID() != this.GetInstanceID())
            {
                Destroy(gameObject);    // Destroy it
            }
            else
            {
                s_instance = this as EventDispatcher;
            }
        }
        #endregion

        /// Store all "listener"
        Dictionary<EventID, List<Action<Component, object>>> _listenersDict
        = new Dictionary<EventID, List<Action<Component, object>>>();

        // Register EventID, callback will be invoke when event with be raise
        public void RegisterListener(EventID eventID, Action<Component, object> callback)
        {
            // check if listener exist in dictionary
            if (_listenersDict.ContainsKey(eventID))
            {
                // add callback to our collection
                _listenersDict[eventID].Add(callback);
            }
            else
            {
                // add new key-value pair
                var newList = new List<Action<Component, object>>();
                newList.Add(callback);
                _listenersDict.Add(eventID, newList);
            }
        }

        // Post event, This will notify all listener which register to listen for eventID
        public void PostEvent(EventID eventID, Component sender, object param = null)
        {

            List<Action<Component, object>> actionList;
            if (_listenersDict.TryGetValue(eventID, out actionList))
            {
                for (int i = 0, amount = actionList.Count; i < amount; i++)
                {
                    try
                    {
                        actionList[i](sender, param);
                    }
                    catch (Exception e)
                    {
                        // remove listener at i - that cause the exception
                        actionList.RemoveAt(i);
                        if (actionList.Count == 0)
                        {
                            // no listener remain, then delete this key
                            _listenersDict.Remove(eventID);
                        }
                        // reduce amount and index for the next loop
                        amount--;
                        i--;
                    }
                }
            }
            else
            {
                // if not exist, just warning, don't throw exceptoin

            }
        }

        // Use for Unregister, not listen for event anymore
        public void RemoveListener(EventID eventID, Action<Component, object> callback)
        {
            List<Action<Component, object>> actionList;
            if (_listenersDict.TryGetValue(eventID, out actionList))
            {
                if (actionList.Contains(callback))
                {
                    actionList.Remove(callback);
                    if (actionList.Count == 0)// no listener remain for this event
                    {
                        _listenersDict.Remove(eventID);
                    }
                }
            }
            else
            {
                // the listeners not exist

            }
        }

    }
    // An Extension class, declare some "shortcut" for using EventPatcher
    #region Extension Class
    public static class EventDispatcherExtension
    {
        public static void RegisterListener(this MonoBehaviour sender, EventID eventID, Action<Component, object> callback)
        {
            EventDispatcher.Instance.RegisterListener(eventID, callback);
        }

        // Post event with param
        public static void PostEvent(this MonoBehaviour sender, EventID eventID, object param = null)
        {
            EventDispatcher.Instance.PostEvent(eventID, sender, param);
        }

        // Post vent without param

        public static void PostEvent(this MonoBehaviour sender, EventID eventID)
        {
            EventDispatcher.Instance.PostEvent(eventID, sender, null);
        }
    }
    # endregion
}
