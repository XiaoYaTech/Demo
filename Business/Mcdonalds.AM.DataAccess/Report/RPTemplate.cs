using Mcdonalds.AM.DataAccess.DataTransferObjects.Report;
using Mcdonalds.AM.DataAccess.Report;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RPTemplate : BaseEntity<RPTemplate>
    {
        #region 属性
        public List<RPTemplateFieldDetail> Details { get; set; }
        #endregion

        #region 查询
        public static List<RPTemplate> GetByEID(bool deepend = false)
        {
            var eid = ClientCookie.UserCode;
            if (deepend)
            {
                var context = PrepareDb();
                var result = context.RPTemplate.GroupJoin(context.RPTemplateFieldDetail, h => h.ID, d => d.TemplateID, (h, d) => new { head = h, details = d })
                    .Where(r => r.head.CreateBy == eid || r.head.IsCommon == true);
                List<RPTemplate> list = new List<RPTemplate>();

                foreach (var item in result)
                {
                    item.head.Details = item.details.ToList();

                    list.Add(item.head);
                }
                return list;
            }
            else
            {
                return Search(p => p.CreateBy == eid || p.IsCommon == true).ToList();
            }

        }

        public static RPTemplate Get(int Id)
        {
            return Search(p => p.ID == Id).FirstOrDefault();
        }

        #endregion

        #region 操作

        public static void Save(int templateId, List<TemplateTable> Tables)
        {
            if (Tables == null || Tables.Count() == 0)
            {
                throw new Exception("模板字段不能为空");
            }
            var template = Get(templateId);
            if (template == null)
            {
                throw new Exception("模板不存在");
            }
            else if (template.IsCommon == true)
            {
                throw new Exception("系统模板禁止修改");
            }

            using (var scope = new TransactionScope())
            {
                RPTemplateFieldDetail.Delete(p => p.TemplateID == templateId);
                RPTemplateTableDetail.Delete(p => p.TemplateID == templateId);
                var checkTs = Tables.Where(t => t.Checked).ToList();
                foreach (var tb in checkTs)
                {
                    var templateTable = new RPTemplateTableDetail()
                    {
                        TableID = tb.ID,
                        TemplateID = template.ID
                    };
                    RPTemplateTableDetail.Add(templateTable);
                    var checkFs = tb.Fields.Where(t => t.Checked).ToList();
                    foreach (TemplateField field in checkFs)
                    {
                        var fieldDetail = new RPTemplateFieldDetail()
                        {
                            TemplateID = template.ID,
                            TableID = tb.ID,
                            FieldID = field.ID,
                            ConditionText = field.ConditionText,
                            IsOrderBy = field.IsOrderBy,
                            IsDESC = field.IsDESC
                        };
                        RPTemplateFieldDetail.Add(fieldDetail);
                    }
                }
                scope.Complete();
            }
        }

        public static int SaveAs(string templateName, List<TemplateTable> Tables)
        {
            var currentUserCode = ClientCookie.UserCode;
            if (Tables == null || Tables.Count() == 0)
            {
                throw new Exception("模板字段不能为空");
            }

            if (string.IsNullOrEmpty(templateName))
            {
                templateName = "个人模板_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                //throw new Exception("模板名称不能为空");
            }
            using (var scope = new TransactionScope())
            {
                var newTemplate = new RPTemplate()
                {
                    TName = templateName,
                    IsCommon = false,
                    CreateBy = currentUserCode,
                    CreateDate = DateTime.Now
                };
                RPTemplate.Add(newTemplate);
                var checks = Tables.Where(t => t.Checked).ToList();
                foreach (var tb in checks)
                {
                    var templateTable = new RPTemplateTableDetail()
                    {
                        TableID = tb.ID,
                        TemplateID = newTemplate.ID
                    };
                    RPTemplateTableDetail.Add(templateTable);
                    var checkFs = tb.Fields.Where(t => t.Checked).ToList();
                    foreach (TemplateField field in checkFs)
                    {
                        var fieldDetail = new RPTemplateFieldDetail()
                        {
                            TemplateID = newTemplate.ID,
                            TableID = tb.ID,
                            FieldID = field.ID,
                            ConditionText = field.ConditionText,
                            IsOrderBy = field.IsOrderBy,
                            IsDESC = field.IsDESC
                        };
                        RPTemplateFieldDetail.Add(fieldDetail);
                    }
                }
                scope.Complete();
                return newTemplate.ID;
            }
        }

        public static void Delete(int templateId)
        {
            RPTemplateFieldDetail.Delete(p => p.TemplateID == templateId);
            RPTemplateTableDetail.Delete(p => p.TemplateID == templateId);
            RPTemplate.Delete(p => p.ID == templateId);
        }
        #endregion
    }
}
