using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SFLibs.Core.Basis
{
    public class KeyedList<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        private Func<TItem, TKey> getKeyFunc;

        public KeyedList(Func<TItem, TKey> getKeyFunc)
            : base()
        {
            this.getKeyFunc = getKeyFunc;
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return this.getKeyFunc(item);
        }

        public bool TryAdd(TItem item)
        {
            if (!Contains(GetKeyForItem(item)))
            {
                Add(item);
                return true;
            }

            return false;
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// シリアライズのために書いておく。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new TItem this[int index]
        {
            get { return ((IList<TItem>)this)[index]; }
        }

        public bool TryGetItem(TKey key, out TItem item)
        {
            if (Contains(key))
            {
                item = this[key];
                return true;
            }
            else
            {
                item = default(TItem);
                return false;
            }
        }
    }
}
