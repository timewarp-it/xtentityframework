using System;
namespace xtEntityFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultOrderAttribute : Attribute
    {
        public Order Order;
        public bool BaseClass { get; set; }

        public DefaultOrderAttribute(Order Order = Order.Ascending, bool BaseClass = false)
        {
            this.Order = Order;
            this.BaseClass = BaseClass;
        }
    }

    public enum Order
    {
        Ascending = 0,
        Descending = 1
    }
}
