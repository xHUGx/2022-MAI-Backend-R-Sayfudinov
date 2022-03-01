using System.Collections.Generic;
using System.Linq;

namespace ProgramEngineering.Service
{
    public class LRUCache
    {
        private int _numOfCells;
        private Dictionary<string, string> _cache;
        private List<KeyValuePair<string, string>> _orderList;

        public LRUCache(int numberOfCacheCells)
        {
            this._numOfCells = numberOfCacheCells;
            _cache = new Dictionary<string,string>(_numOfCells);
            _orderList = new List<KeyValuePair<string, string>>(_numOfCells);
        }

        public void Set(string key, string value)
        {
            if (_cache.Count == _numOfCells) // the cache is full we need to remove 1
            {
                var toRemove = _orderList[0];
                if (_cache.ContainsKey(key))
                {
                    _cache.Remove(toRemove.Key);
                }
                _orderList.Remove(toRemove);
            }
            _orderList.Add(new KeyValuePair<string, string>(key, value));
            _cache[key] = value;
        }

        public string Get(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                return null;
            }

            //put the key and value to the back of the ordered list
            var tempCacheCell = _orderList.FirstOrDefault(x=>x.Key == key);
            _orderList.Remove(tempCacheCell);
            _orderList.Add(tempCacheCell);
            return _cache[key];
        }

        public void Remove(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                return;
            }

            _orderList.RemoveAll(x=>x.Key == key);
            _cache.Remove(key);
        }
    }
}