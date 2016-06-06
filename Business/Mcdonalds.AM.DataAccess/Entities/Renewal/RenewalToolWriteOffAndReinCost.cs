using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Common;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/16/2014 2:34:54 PM
 * FileName     :   RenewalToolWriteOffAndReinCost
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.Core.Json;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalToolWriteOffAndReinCost : BaseEntity<RenewalToolWriteOffAndReinCost>
    {
        public static RenewalToolWriteOffAndReinCost Get(string projectId, Guid toolId,NodeInfo nodeInfo = null)
        {
            Log4netHelper.WriteInfo(
                JsonConvert.SerializeObject(new {desc = "write off and rein cost", projectId, toolId, nodeInfo}));
            var entity = FirstOrDefault(w => w.ToolId == toolId);
            if (entity == null)
            {

                entity = new RenewalToolWriteOffAndReinCost();
                entity.Id = Guid.NewGuid();
                entity.ToolId = toolId;
                entity.Add();
            }

            return entity;
        }
        public void Save()
        {
            if (Any(r => r.Id == this.Id))
            {
                this.Update();
            }
            else
            {
                if (this.Id == Guid.Empty)
                {
                    this.Id = Guid.NewGuid();
                }
                this.Add();
            }
        }
    }
}
