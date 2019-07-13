
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHibernate;
using nhibernate_console.Entities;

namespace nhibernate_console {
    public class NhibernateService : IHostedService, IDisposable {

        private readonly ILogger _logger;
        private Timer _timer;
        private static Random random = new Random();

        private readonly ISessionFactory _sessionFactory;

        public NhibernateService( ILogger<NhibernateService> logger, ISessionFactory sessionFactory ) {
            _logger = logger;
            _sessionFactory = sessionFactory;
        }

        public Task StartAsync( CancellationToken cancellationToken ) {
            _logger.LogInformation( "Starting" );

            _timer = new Timer( DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds( 15 ) );

            return Task.CompletedTask;
        }

        private void DoWork( object state ) {
            using ( var session = _sessionFactory.OpenSession() ) {
                using ( var tx = session.BeginTransaction() ) {
                    //session.FlushMode = FlushMode.Commit;
                    var customer = new Customer() {
                        Name = $"Customer 1",
                        LegalName = $"Customer 1 LLC",
                        VATCode = "xxxxxxxxx",
                        Created = DateTime.UtcNow
                    };
                    customer.Sites.Add( new Site() {
                        Address = $"Address Customer 1",
                        City = $"City Customer 1",
                        Country = $"Country Customer 1",
                        Customer = customer
                    } );
                    session.Save( customer );
                    tx.Commit();
                }
            }
        }

        public Task StopAsync( CancellationToken cancellationToken ) {
            _logger.LogInformation( "Stopping." );

            _timer?.Change( Timeout.Infinite, 0 );

            return Task.CompletedTask;
        }

        public void Dispose() {
            _timer?.Dispose();
        }

    }
}
