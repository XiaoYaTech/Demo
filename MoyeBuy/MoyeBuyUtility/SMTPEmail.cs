using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using MoyeBuy.Com.IUtility;

namespace MoyeBuy.Com.MoyeBuyUtility
{
    public class SMTPEmail : IEmail
    {
        private MailMessage msg = new MailMessage();
        private SmtpClient client = null;

        public SMTPEmail(string strHost)
        {
            client = new SmtpClient(strHost);
        }
        public void SendEmail(string strSendTo, string strSendFrom, string strSendCC, string strTitle, string strMsgBody,string strUID)
        {
            ArrayList arrSendTo=Gadget.Split(strSendTo,";");
            ArrayList arrSendCC=Gadget.Split(strSendCC,";");
            for(int i=0;i<arrSendTo.Count;i++)
            {
                msg.CC.Add(arrSendTo[i].ToString());
            }
            for(int i=0;i<arrSendTo.Count;i++)
            {
                msg.To.Add(arrSendCC[i].ToString());
            }
            msg.From = new MailAddress(strSendFrom);
            msg.Subject = strTitle;
            msg.Body = strMsgBody;
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.High;
            msg.BodyEncoding = ASCIIEncoding.Unicode;
            try
            {
                //client.Credentials = new NetworkCredential("clay.wang@moyebuy.com", "");
                client.Credentials = CredentialCache.DefaultNetworkCredentials ;
                client.Timeout = 0;
                client.EnableSsl = true;
                client.Send(msg);
            }
            catch(Exception ex)
            {
                msg.Dispose();
                client.Dispose();

                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "MoyeBuy.Com.MoyeBuyUtility.SendEmail()", UtilityFactory.LogType.LogToFile);
            }
           
        }
    }
}
