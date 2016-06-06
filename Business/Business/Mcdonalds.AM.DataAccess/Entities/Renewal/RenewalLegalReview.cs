/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/28/2014 4:25:58 PM
 * FileName     :   Renewal_LegalReview
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services.Entity;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalLegalReview : BaseWFEntity<RenewalLegalReview>
    {
        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Renewal\LegalReview"; } }

        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Renewal_LR"; }
        }


        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            RenewalInfo info = RenewalInfo.Get(this.ProjectId);
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_LegalUser",null),
                new ProcessDataField("dest_GeneralCounsel",null),
                new ProcessDataField("ProcessCode", WorkflowProcessCode)
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }
        public static void Create(string projectId)
        {

        }
    }
}
