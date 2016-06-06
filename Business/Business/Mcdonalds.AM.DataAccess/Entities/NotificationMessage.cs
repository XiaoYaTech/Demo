using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class NotificationMessage : BaseEntity<NotificationMessage>
    {
        public Guid SaveMessage()
        {
            Guid msgId = Guid.Empty;
            if (Id == Guid.Empty)
            {
                msgId = Guid.NewGuid();
                Id = msgId;
                Add();
            }
            else
            {
                Update();
            }
            return msgId;
        }
    }
}
