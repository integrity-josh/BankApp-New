using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp.Domain.Infrastructure
{
    // public abstract class Entity // <TId> : IComparable, ICompareableEntity<TId>, ....
    // {

        // if we didn't care about checking for equality, we could just make entity class and it'd be pretty much empty and used as a marker,
            // only thing in it would be the entity property
            
        // entity base class logic here, such as equality checks, etc. - this is the base class for all entities in the domain, including aggregate roots (customer)

        // logic that will be here comes from Phil's finding that
            // for a lot of clients, their DB's Id's tend to be a mess of strings, ints, guids, etc
            // so here we can have logic that takes all those types of Id's and handles them, so we don't have to worry about it in each individual entity
        
        // protected constructor

        // public override bool Equals(object? obj) - equality checks based on Id, so that if two entities have the same Id, they are considered equal, even if they are different instances in memory

            // getunproxiedtype method - if we are using an ORM like Entity Framework, it creates proxy classes that inherit from our entities, so we need to get the actual type of the entity, not the proxy type, when doing equality checks
                // for ex: get down to the actual type name of the entity as opposed to the proxy class version
        
        // IsTransient method - checks if the entity is transient, meaning it has not been persisted to the database yet, which we can determine by checking if the Id is the default value (0 for int, null for string, etc)
            // (have i saved it to the DB yet?)

        // public static bool operator ==(Entity<TId> a, ...b)
            // defines equality operators for the entity, so that we can use == and != to compare entities based on their Ids, rather than their reference in memory

        // public override int GetHashCode method - generates a hash code based on the entity's Id, so that entities with the same Id have the same hash code
            // by default .NET is going to check for reference equality
            // if you have two entities and you're checking for equality, don't stop with reference equality, check to see if the Id's are equal
        
    // }


    //boiler code:
    public abstract class Entity<TId> : IComparable, IComparable<Entity<TId>>
        where TId : IComparable<TId>
    {
        public virtual TId Id { get; protected set; }

        protected Entity() { }

        protected Entity(TId id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetUnproxiedType(this) != GetUnproxiedType(other))
                return false;

            if (IsTransient() || other.IsTransient())
                return false;

            return Id.Equals(other.Id);
        }

        private bool IsTransient()
        {
            return Id is null || Id.Equals(default(TId));
        }

        public static bool operator ==(Entity<TId> a, Entity<TId> b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity<TId> a, Entity<TId> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetUnproxiedType(this).ToString() + Id).GetHashCode();
        }

        public virtual int CompareTo(Entity<TId>? other)
        {
            if (other is null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return Id.CompareTo(other.Id);
        }

        public virtual int CompareTo(object? other)
        {
            return CompareTo(other as Entity<TId>);
        }

        internal static Type GetUnproxiedType(object obj)
        {
            const string EFCoreProxyPrefix = "Castle.Proxies.";
            const string NHibernateProxyPostfix = "Proxy";

            var type = obj.GetType();
            var typeString = type.ToString();

            if (typeString.Contains(EFCoreProxyPrefix) || typeString.EndsWith(NHibernateProxyPostfix))
                return type.BaseType ?? type;

            return type;
        }
    }

    public abstract class Entity : Entity<int>
    {
        protected Entity() { }

        protected Entity(int id) : base(id) { }
    }
}