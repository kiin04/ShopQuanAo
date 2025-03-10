using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models
{
    public sealed class DatabaseContextSingleton
    {
        private static readonly Lazy<ShopQuanAoEntities> _instance =
            new Lazy<ShopQuanAoEntities>(() => new ShopQuanAoEntities());

        private DatabaseContextSingleton() { }

        public static ShopQuanAoEntities Instance => _instance.Value;
    }

}