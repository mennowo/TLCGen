using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Threading;

namespace TLCGen.Messaging
{
    /// <summary>
    /// Manages sending and distributing of events between objects (mostly ViewModels) in TLCGen.
    /// This class is loosely based on the Messenger from the MVVM Light Toolkit (http://www.mvvmlight.net)
    /// </summary>
    public class MessageManager
    {
        private static readonly object _Locker = new object();

        private static bool _IsCleaningUp;

        private static MessageManager _Instance;
        public static MessageManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_Locker)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new MessageManager();
                        }
                    }
                }
                return _Instance;
            }
        }

        public Dictionary<Type, List<Subscriber>> Events { get; set; }

        public void Subscribe<T>(object subscriber, Action<T> action)
        {
            if (Events.Keys.Contains(typeof(T)))
            {
                List<Subscriber> subscribers;
                Events.TryGetValue(typeof(T), out subscribers);
                subscribers.Add(new Subscriber(subscriber, new WeakAction<T>(subscriber, action)));
            }
            else
            {
                List<Subscriber> subscribers = new List<Subscriber>();
                subscribers.Add(new Subscriber(subscriber, new WeakAction<T>(subscriber, action)));
                Events.Add(typeof(T), subscribers);
            }
            RequestCleanup();
        }

        public void Unsubscribe<T>(object subscriber)
        {
            if (Events.Keys.Contains(typeof(T)))
            {
                List<Subscriber> subscribers;
                Events.TryGetValue(typeof(T), out subscribers);
                Subscriber s = null;
                foreach (object o in subscribers)
                {
                    if (o is Subscriber)
                    {
                        if (subscriber == (o as Subscriber))
                        {
                            s = (o as Subscriber);
                        }
                    }
                }
                if (s != null)
                {
                    subscribers.Remove(s);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            RequestCleanup();
        }

        public void Send<T>(T myevent)
        {
            if (Events.Keys.Contains(typeof(T)))
            {
                List<Subscriber> subscribers;
                if (Events.TryGetValue(typeof(T), out subscribers) && subscribers.Count > 0)
                {
                    foreach (Subscriber s in subscribers)
                    {
                        var action = (WeakAction<T>)s.MyAction;
                        action.ExecuteWithObject(myevent);
                    }
                }
            }
        }

        public T SendWithRespons<T>(T myevent)
        {
            if (Events.Keys.Contains(typeof(T)))
            {
                List<Subscriber> subscribers;
                if (Events.TryGetValue(typeof(T), out subscribers) && subscribers.Count > 0)
                {
                    foreach (Subscriber s in subscribers)
                    {
                        var action = (WeakAction<T>)s.MyAction;
                        action.ExecuteWithObject(myevent);
                    }
                }
            }
            return myevent;
        }

        public void Cleanup()
        {
            CleanupList(Events);
            _IsCleaningUp = false;
        }

        private void RequestCleanup()
        {
            if (!_IsCleaningUp)
            {
                Action cleanupAction = Cleanup;

                Dispatcher.CurrentDispatcher.BeginInvoke(
                    cleanupAction,
                    DispatcherPriority.ApplicationIdle,
                    null);

                _IsCleaningUp = true;
            }
        }

        private static void CleanupList(IDictionary<Type, List<Subscriber>> lists)
        {
            if (lists == null)
            {
                return;
            }

            lock (lists)
            {
                var listsToRemove = new List<Type>();
                foreach (var list in lists)
                {
                    var recipientsToRemove = list.Value
                        .Where(item => item.MyAction == null || !item.MyAction.IsAlive)
                        .ToList();

                    foreach (var recipient in recipientsToRemove)
                    {
                        list.Value.Remove(recipient);
                    }

                    if (list.Value.Count == 0)
                    {
                        listsToRemove.Add(list.Key);
                    }
                }

                foreach (var key in listsToRemove)
                {
                    lists.Remove(key);
                }
            }
        }

        private MessageManager()
        {
            Events = new Dictionary<Type, List<Subscriber>>();
        }
    }
}