#nullable enable
using System;

namespace BaseGame
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ItemCategoryAttribute : Attribute
    {
        public string Category { get; }
        
        public ItemCategoryAttribute(string category)
        {
            Category = category;
        }
    }
}