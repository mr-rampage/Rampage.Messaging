namespace Rampage.Messaging
{
    public interface IService
    {
        void Start(IMessageBus messageBus);
        void Stop();
    }

    public interface IHub : IService
    {
        IHub Deploy(IService service);
        IHub Undeploy(IService service);
    }
}