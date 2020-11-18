using System;

using UnityEngine;

namespace AvalonStudios.Additions.Attributes
{
    /// <summary>
    /// If the field is a property then it will show the name without k__BackingField.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class IsProperty : PropertyAttribute
    {
        public IsProperty() { }
    }
}
