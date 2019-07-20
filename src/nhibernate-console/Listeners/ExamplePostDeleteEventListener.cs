
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHibernate.Event;

namespace nhibernate_console.Listeners {
    public class ExamplePostDeleteEventListener : IPostDeleteEventListener {

        private JsonSerializerSettings settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        public ExamplePostDeleteEventListener() {
        }

        public void OnPostDelete( PostDeleteEvent @event ) {
            Console.WriteLine( $"Received PostDelete event for entity {JsonConvert.SerializeObject( @event.Entity, Formatting.None, settings )}" );
        }

        public async Task OnPostDeleteAsync( PostDeleteEvent @event, CancellationToken cancellationToken ) {
            Console.WriteLine( $"Received PostDelete event for entity {JsonConvert.SerializeObject( @event.Entity, Formatting.None, settings )}" );
            await Task.CompletedTask;
        }
    }
}
