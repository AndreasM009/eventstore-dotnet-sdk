namespace Eventstore.Client
{
    public class EventStoreResult<T>
    {
        public T Resource {get; set;}
        public int Version {get; set;}
        public int StatusCode {get; set;}
    }
}