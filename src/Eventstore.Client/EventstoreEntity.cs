namespace Eventstore.Client
{
    internal class EventstoreEntity<T>
    {
        public string id {get; set;}
        public int version {get; set;}
        public T data {get; set;}
    }
}