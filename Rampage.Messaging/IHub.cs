namespace Rampage.Messaging
{
    public interface IServiceNode
    {
        void Start(IMessageBus messageBus);
        void Stop();
    }

    public interface IHub : IServiceNode
    {
        IHub Deploy(IServiceNode serviceNode);
        IHub Undeploy(IServiceNode serviceNode);
    }
}