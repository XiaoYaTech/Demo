using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

namespace MoyeBuy.Com.MSMQMessaging
{
    public class MoyeBuyComQueue : IDisposable
    {
        protected MessageQueueTransactionType transactionType = MessageQueueTransactionType.Automatic;
        protected MessageQueue queue;
        protected TimeSpan timeout;

        public MoyeBuyComQueue(string queuePath, int timeoutSeconds)
        {
            queue = new MessageQueue(queuePath);
            timeout = TimeSpan.FromSeconds(Convert.ToDouble(timeoutSeconds));

            queue.DefaultPropertiesToSend.AttachSenderId = false;
            queue.DefaultPropertiesToSend.UseAuthentication = false;
            queue.DefaultPropertiesToSend.UseEncryption = false;
            queue.DefaultPropertiesToSend.AcknowledgeType = AcknowledgeTypes.None;
            queue.DefaultPropertiesToSend.UseJournalQueue = false;
        }
        public virtual object Receive()
        {
            try
            {
                using (Message message = queue.Receive(timeout, transactionType))
                    return message;
            }
            catch (MessageQueueException mqex)
            {
                if (mqex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    throw new TimeoutException();

                throw;
            }
        }
        public virtual void Send(object msg)
        {
            queue.Send(msg, transactionType);
        }

        public void Dispose()
        {
            queue.Dispose();
        }
    }
}
