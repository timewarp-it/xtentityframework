using System;
namespace xtEntityFramework.Models
{
    public class Filter<TEntity> : IEquatable<Filter<TEntity>>
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public Comparison Comparison { get; set; }

        public bool Equals(Filter<TEntity>? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Name == other.Name && Value == other.Value && Comparison == other.Comparison;
        }

        public override bool Equals(object? obj)
        {

            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() == this.GetType()) return false;

            return Equals((Filter<TEntity>)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Value, Comparison);
        }
    }

    public enum Comparison
    {
        Eq = 0,
        Neq = 1,
        Gt = 2,
        Gte = 3,
        Lt = 4,
        Lte = 5,
        Contains = 6,
        StartsWith = 7,
        EndsWith = 8
    }
}
