using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JPNSample.API.Core.Cache
{
    public class CacheItem<T>
        where T : class
    {
        public T Item { get; set; }
        public DateTime UtcDateCreated { get; set; }
        public DateTime? UtcDateExpired { get; set; }
        public IDictionary<string, object> ExtendedProperties { get; set; }

        public CacheItem()
        { }

        public CacheItem(T item, DateTime? utcDateExpired)
        {
            this.Item = item ?? throw new ArgumentException(nameof(item));
            this.UtcDateCreated = DateTime.UtcNow;
            this.UtcDateExpired = UtcDateExpired;
            this.ExtendedProperties = new Dictionary<string, object>();
        }

        public CacheItem(T item)
            : this(item, null)
        { }
    }
}
