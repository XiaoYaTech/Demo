using Mcdonalds.AM.DataAccess.Common.Excel;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/9/2014 4:50:13 PM
 * FileName     :   RenewalLLNegotiationRecord
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalLLNegotiationRecord : BaseEntity<RenewalLLNegotiationRecord>
    {
        public static List<RenewalLLNegotiationRecord> GetRecords(Guid negotiationId)
        {
            return Search(r => r.RenewalLLNegotiationId == negotiationId && r.Valid).OrderByDescending(r=>r.CreateTime).ToList();
        }
        public static List<RenewalLLNegotiationRecord> GetRecords(Guid negotiationId, int pageIndex, int pageSize,out int totalItems)
        {
            totalItems = Count(r => r.RenewalLLNegotiationId == negotiationId && r.Valid);
            return Search(r => r.RenewalLLNegotiationId == negotiationId && r.Valid).OrderByDescending(r => r.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        public static string ExportRecords(string projectId)
        {
            var nego = RenewalLLNegotiation.Get(projectId);
            var records = GetRecords(nego.Id);
            var templateName = HttpContext.Current.Server.MapPath("~/Template/Renewal_LLNegotiationRecord_Template.xlsx");
            string fileName = string.Concat(HttpContext.Current.Server.MapPath("~/Temp/"), Guid.NewGuid(), ".xlsx");
            File.Copy(templateName, fileName);
            FileInfo file = new FileInfo(fileName);
            ExcelDataInputDirector excelDirector = new ExcelDataInputDirector(file, ExcelDataInputType.RenewalLLNegotiationRecord);
            List<ExcelInputDTO> datas = records.Select(r => new ExcelInputDTO
            {
                McdParticipants =r.McdParticipants,
                Content = r.Content,
                LLParticipants = r.LLParticipants,
                Topic = r.Topic,
                Location = r.Location,
                MeetingDate = r.Date.Value.ToString("yyyy-MM-dd"),
                CreateDate = r.CreateTime.ToString("yyyy-MM-dd")
            }).ToList();
            excelDirector.ListInput(datas);
            return fileName;
        }
    }
}
