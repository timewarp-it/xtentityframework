using System;
namespace xtEntityFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchableAttribute : Attribute
    {
        public bool CascadingSearchEnabled { get; set; }

        public SearchableAttribute(bool CascasingSearchEnabled = false)
        {
            this.CascadingSearchEnabled = CascasingSearchEnabled;
        }
    }
}
