using System;
using System.Threading.Tasks;

namespace Eventstore.Client
{
     public interface IEventstoreClient 
     {
          Task<EventStoreResult<T>> Get<T>(string storename, string id);
          Task<EventStoreResult<T>> GetByVersion<T>(string storename, string id, int version);

          Task<EventStoreResult<T>> Add<T>(string storename, string id, T toAdd);
          Task<EventStoreResult<T>> Save<T>(string storename, string id, T toSave);

     }
}
