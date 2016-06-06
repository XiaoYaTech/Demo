using MoyeBuy.Com.Model;

namespace MoyeBuy.Com.IMessaging
{
    public interface IOrder
    {
        OrderInfo Receive();
        OrderInfo Receive(int timeout);
        void Send(OrderInfo orderMessage);
    }
}
