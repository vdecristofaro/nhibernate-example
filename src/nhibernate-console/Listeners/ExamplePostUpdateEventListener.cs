

using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHibernate.Event;

namespace nhibernate_console.Listeners {
    public class ExamplePostUpdateEventListener : IPostUpdateEventListener {
        private JsonSerializerSettings settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public ExamplePostUpdateEventListener() {
        }

        public void OnPostUpdate( PostUpdateEvent @event ) {
            Console.WriteLine( $"Received PostUpdate event for entity {JsonConvert.SerializeObject( @event.Entity, Formatting.None, settings )}" );
        }

        public async Task OnPostUpdateAsync( PostUpdateEvent @event, CancellationToken cancellationToken ) {
            Console.WriteLine( $"Received PostUpdate event for entity {JsonConvert.SerializeObject( @event.Entity, Formatting.None, settings )}" );
            await Task.CompletedTask;
        }
    }
}
