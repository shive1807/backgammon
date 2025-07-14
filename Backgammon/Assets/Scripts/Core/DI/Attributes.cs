using System;

namespace Core.DI
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
        public string Id { get; set; }
        public bool Optional { get; set; }

        public InjectAttribute(string id = null, bool optional = false)
        {
            Id = id;
            Optional = optional;
        }
    }
}