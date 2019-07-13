
using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using nhibernate_console.Entities;

namespace nhibernate_console.Mapping {
    internal class SiteConfiguration : ClassMapping<Site> {
        public SiteConfiguration() {
            Table( "Sites" );

            Id( x => x.EntityID, im => {
                im.Column( "SiteID" );
                im.Generator( Generators.Identity );
            } );

            Property( x => x.Address, pm => {
                pm.NotNullable( true );
                pm.Length( 255 );
            } );
            Property( x => x.City, pm => {
                pm.NotNullable( false );
                pm.Length( 255 );
            } );
            Property( x => x.Country, pm => {
                pm.NotNullable( false );
                pm.Length( 20 );
            } );

            ManyToOne( x => x.Customer, mm => {
                mm.Column( "CustomerID" );
                mm.NotNullable( true );
            } );
        }
    }
}
