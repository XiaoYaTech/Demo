using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expose178.Com.IUtility
{
    public interface IEmail
    {
        void SendEmail(string strSendTo, string strSendFrom, string strSendCC, string strTitle, string strMsgBody);
    }
}
