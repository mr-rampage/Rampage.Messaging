namespace Rampage.Messaging
{
    public interface IServiceNode<T>
    {
        void Start(IMessageBus<T> messageBus);
        void Stop();
    }

    public interface IHub<T> : IServiceNode<T>
    {
        IHub<T> Deploy(IServiceNode<T> serviceNode);
        IHub<T> Undeploy(IServiceNode<T> serviceNode);
    }
}