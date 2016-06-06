using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class Attachment : BaseEntity<Attachment>
    {
        public bool Required { get; set; }

        public bool IsExist
        {
            get { return !(RefTableID == null || RefTableID == Guid.Empty.ToString()); }
        }

        public static Attachment Get(string refTableId, Guid requirementId)
        {
            return FirstOrDefault(att => att.RefTableID == refTableId && att.RequirementId == requirementId);
        }
        public static List<Attachment> GetAllAttachmentsIncludeRequire(string refTableName, string projectId, string flowCode, string refTableId = null)
        {
            if (string.IsNullOrEmpty(refTableId))
            {
                refTableId = GetRefTableId(refTableName, projectId);
            }
            //var userRole = ProjectUsers.Get(ClientCookie.UserCode, projectId);
            var attachments = Search(att => att.RefTableName == refTableName && att.RefTableID == refTableId && !att.IsHide).OrderBy(a => a.Sequence).ToList();
            attachments.ForEach(att =>
            {
                if (!string.IsNullOrEmpty(att.Name))
                {
                    att.RequireName = "其他附件";
                    att.FileName = att.Name.EndsWith(att.Extension, StringComparison.CurrentCultureIgnoreCase) ? att.Name : (att.Name + att.Extension);
                }
            });
            var attachsReq = AttachmentRequirement.Search(ar => ar.FlowCode == flowCode).OrderBy(ar => ar.Sequence).AsNoTracking().ToList().Select(ar =>
            {
                Attachment att;
                if (ar.LinkTo != null && ar.LinkTo != Guid.Empty)
                {
                    var arLinkTo = AttachmentRequirement.Get(ar.LinkTo.Value);
                    string linkToRefTableId = GetRefTableId(arLinkTo.RefTableName, projectId);
                    att = FirstOrDefault(at => at.RefTableID == linkToRefTableId && at.RefTableName == arLinkTo.RefTableName && at.RequirementId == arLinkTo.Id && !at.IsHide);
                }
                else
                {
                    att = attachments.FirstOrDefault(a => a.RequirementId == ar.Id);
                }
                if (att == null)
                {
                    att = new Attachment();
                    att.RequirementId = ar.Id;
                    att.RefTableName = refTableName;

                }
                att.RequireName = ar.NameENUS;
                att.Required = ar.Required;
                if (!string.IsNullOrEmpty(att.Name))
                {
                    att.FileName = att.Name.EndsWith(att.Extension, StringComparison.CurrentCultureIgnoreCase) ? att.Name : (att.Name + att.Extension);
                }
                return att;
            }).ToList();
            var attachsOptional = attachments.Where(a => !a.RequirementId.HasValue).ToList();
            return attachsReq.Union(attachsOptional).ToList();
        }

        public static List<Attachment> GetList(string refTableName, string refTableId, string typeCode)
        {
            var list = Search(d => d.RefTableName == refTableName && d.RefTableID == refTableId).AsNoTracking();
            if (!string.IsNullOrEmpty(typeCode))
            {
                list = list.Where(d => d.TypeCode == typeCode);
            }
            list = list.OrderBy(e => e.CreateTime);
            return list.ToList();
        }

        public static Attachment GetAttachment(string refTableName, string refTableId, string typeCode)
        {
            var att = FirstOrDefault(
                 e => e.RefTableName == refTableName && e.RefTableID == refTableId && e.TypeCode == typeCode);
            return att;
        }

        public static int Delete(Guid id)
        {
            var att = Get(id);
            return Delete(att);
        }

        public static int AddList(List<Attachment> att)
        {
            return Add(att.ToArray());
        }

        /// <summary>
        /// 只保存一份附件
        /// </summary>
        /// <param name="attEntity"></param>
        /// <returns></returns>
        public static int SaveSigleFile(Attachment attEntity)
        {
            var att = FirstOrDefault(
                 e => e.RefTableName == attEntity.RefTableName && attEntity.RefTableID == e.RefTableID && e.TypeCode == attEntity.TypeCode);
            if (att != null)
            {

                //attEntity.ID = att.ID;
                att.RefTableName = attEntity.RefTableName;
                att.RefTableID = attEntity.RefTableID;
                att.CreateTime = DateTime.Now;
                att.UpdateTime = DateTime.Now;
                att.InternalName = attEntity.InternalName;
                att.Name = attEntity.Name;
                att.Extension = attEntity.Extension;
                att.Length = attEntity.Length;
                att.TypeCode = attEntity.TypeCode;
                att.CreatorID = attEntity.CreatorID;
                att.RequirementId = attEntity.RequirementId;
                att.CreatorNameENUS = attEntity.CreatorNameENUS;
                att.CreatorNameZHCN = attEntity.CreatorNameZHCN;
                return Update(att);
            }
            else
            {
                attEntity.ID = Guid.NewGuid();
                return Add(attEntity);
            }
        }

        public string FileURL { get; set; }


        public string FileName { get; set; }
        public string RequireName { get; set; }
    }
}
