using System;
using System.Collections.Generic;

namespace EnhancedEvents
{
    public class LayeredEvent<T> where T : EventArgs
    {
        private SortedSet<int> _indexSet = 
            new SortedSet<int>();

        private Dictionary<int, List<EventHandler<T>>> _dict = 
            new Dictionary<int, List<EventHandler<T>>>();

        public void Subscribe(EventHandler<T> callback, int layerIndex = 0)
        {
            _indexSet.Add(layerIndex);
            if (!_dict.ContainsKey(layerIndex))
            {
                _dict.Add(layerIndex, new List<EventHandler<T>>());
            }
            _dict[layerIndex].Add(callback);
        }

        public void Unsubscribe(EventHandler<T> callback)
        {
            foreach (int i in new SortedSet<int>(_indexSet))
            {
                while(_dict[i].Contains(callback))
                {
                    _dict[i].Remove(callback);
                    if(_dict[i].Count == 0) _indexSet.Remove(i);
                }
            }
        }

        public void Publish(object sender, T args)
        {
            foreach(int i in _indexSet)
            {
                foreach(EventHandler<T> callback in _dict[i])
                {
                    callback(sender, args);
                }
            }
        }

        public int SubscriberCount
        {
            get
            {
                int count = 0;
                foreach (List<EventHandler<T>> subList in _dict.Values)
                {
                    count += subList.Count;
                }
                return count;
            }
        }

        public Dictionary<int, List<EventHandler<T>>> Subscribers
        {
            get
            {
                Dictionary<int, List<EventHandler<T>>> newDict =
                    new Dictionary<int, List<EventHandler<T>>>();
                foreach (int layerIndex in _dict.Keys)
                {
                    List<EventHandler<T>> newList = new List<EventHandler<T>>();
                    foreach (EventHandler<T> callback in _dict[layerIndex])
                    {
                        newList.Add(new EventHandler<T>(callback));
                    }
                    newDict[layerIndex] = newList;
                }
                return newDict;
            }
        }

        public bool HasSubscriber(EventHandler<T> callback)
        {
            foreach (List<EventHandler<T>> subList in _dict.Values)
            {
                if (subList.Contains(callback))
                {
                    return true;
                }
            }
            return false;
        }
    }
}