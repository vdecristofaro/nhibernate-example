
using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using nhibernate_console.Entities;

namespace nhibernate_console.Mapping {
    internal class CustomerConfiguration : ClassMapping<Customer> {
        public CustomerConfiguration() {
            Table( "Customers" );

            Id( x => x.EntityID, im => {
                im.Column( "CustomerID" );
                im.Generator( Generators.Identity );
            } );

            Property( x => x.Name, pm => {
                pm.NotNullable( true );
                pm.Length( 255 );
            } );
            Property( x => x.LegalName, pm => {
                pm.NotNullable( true );
                pm.Length( 255 );
            } );
            Property( x => x.VATCode, pm => {
                pm.NotNullable( true );
                pm.Length( 20 );
            } );
            Property( x => x.Created, pm => {
                pm.NotNullable( true );
            } );

            Set<Site>( property => property.Sites,
                collection => {
                    collection.Cascade( Cascade.Persist.Include( Cascade.DeleteOrphans ) );
                    collection.Inverse( true );
                    collection.Key( keyMapping => {
                        keyMapping.Column( "CustomerID" );
                        keyMapping.NotNullable( true );
                    } );
                },
                mapping => {
                    mapping.OneToMany( relationalMapping => {
                        relationalMapping.Class( typeof( Site ) );
                    } );
                } );
        }
    }
}
