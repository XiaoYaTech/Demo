using System;
using System.Reflection;
using System.Configuration;

namespace MoyeBuy.Com.MessagingFactory
{
    public sealed class QueueAccess
    {
        private static readonly string path = ConfigurationManager.AppSettings["OrderMessaging"];
        public static MoyeBuy.Com.IMessaging.IOrder CreateOrder()
        {
            string className = path + ".MoyeBuyOrder";
            return (MoyeBuy.Com.IMessaging.IOrder)Assembly.Load(path).CreateInstance(className);
        }
    }

}
