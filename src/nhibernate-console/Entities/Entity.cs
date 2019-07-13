
using System;
using System.Reflection;

namespace nhibernate_console.Entities {
    [Serializable]
    public abstract class Entity {

        public virtual int EntityID { get; set; }

        /// <summary>
        /// Checks if this entity is transient (it has not an Id).
        /// </summary>
        /// <returns>True, if this entity is transient</returns>
        public virtual bool IsTransient() {
            return EntityID <= 0;
        }

        /// <inheritdoc/>
        public override bool Equals( object obj ) {
            if ( obj == null || !( obj is Entity ) ) {
                return false;
            }

            //Same instances must be considered as equal
            if ( ReferenceEquals( this, obj ) ) {
                return true;
            }

            //Transient objects are not considered as equal
            var other = (Entity)obj;
            if ( IsTransient() && other.IsTransient() ) {
                return false;
            }

            //Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if ( !typeOfThis.GetTypeInfo().IsAssignableFrom( typeOfOther ) && !typeOfOther.GetTypeInfo().IsAssignableFrom( typeOfThis ) ) {
                return false;
            }

            return EntityID.Equals( other.EntityID );
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            var hashCode = default( int );
            if ( !IsTransient() ) {
                // XOR for random distribution 
                // (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                hashCode = this.EntityID.GetHashCode() ^ 31;
            }
            else {
                hashCode = base.GetHashCode();
            }
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==( Entity left, Entity right ) {
            if ( Equals( left, null ) ) {
                return Equals( right, null );
            }

            return left.Equals( right );
        }

        /// <inheritdoc/>
        public static bool operator !=( Entity left, Entity right ) {
            return !( left == right );
        }

        /// <inheritdoc/>
        public override string ToString() {
            return $"[{GetType().Name} {EntityID}]";
        }
    }
}
