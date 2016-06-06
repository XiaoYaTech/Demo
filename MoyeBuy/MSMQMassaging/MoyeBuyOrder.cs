using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Messaging;
using MoyeBuy.Com.Model;

namespace MoyeBuy.Com.MSMQMessaging
{
    public class MoyeBuyOrder : MoyeBuyComQueue, MoyeBuy.Com.IMessaging.IOrder
    {
        // Path example - FormatName:DIRECT=OS:MyMachineName\Private$\OrderQueueName
        private static readonly string queuePath = ConfigurationManager.AppSettings["OrderQueuePath"];
        private static int queueTimeout = 20;

        public MoyeBuyOrder()
            : base(queuePath, queueTimeout)
        {
            queue.Formatter = new BinaryMessageFormatter();
        }

        public new MoyeBuy.Com.Model.OrderInfo Receive()
        {
            base.transactionType = MessageQueueTransactionType.Automatic;
            return (OrderInfo)((Message)base.Receive()).Body;
        }

        public MoyeBuy.Com.Model.OrderInfo Receive(int timeout)
        {
            base.timeout = TimeSpan.FromSeconds(Convert.ToDouble(timeout));
            return Receive();
        }

        public void Send(MoyeBuy.Com.Model.OrderInfo orderMessage)
        {
            base.transactionType = MessageQueueTransactionType.Single;
            base.Send(orderMessage);
        }
    }
}
