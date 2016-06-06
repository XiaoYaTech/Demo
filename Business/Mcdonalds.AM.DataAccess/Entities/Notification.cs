using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class Notification : BaseEntity<Notification>
    {
        public string Message { get; set; }
        public string PositionENUS { get; set; }
        public string PositionZHCN { get; set; }
        public string SenderZHCN { get; set; }
        public string SenderENUS { get; set; }
        public string ReceiverENUS { get; set; }
        public string ReceiverZHCN { get; set; }
        public static void Send(NotificationMsg message)
        {
            using (var scope = new TransactionScope())
            {
                Guid? msgId = null;
                if (!string.IsNullOrEmpty(message.Message))
                {
                    var msg = new NotificationMessage { Id = Guid.Empty, MessageContent = message.Message };
                    msgId = msg.SaveMessage();
                }
                Mapper.CreateMap<NotificationMsg, Notification>();

                var notification = Mapper.Map<Notification>(message);
                notification.Title = string.IsNullOrEmpty(message.Title) ? "无主题" : message.Title;
                notification.CreateTime = DateTime.Now;
                notification.CreateUserAccount = ClientCookie.UserCode;
                notification.HasRead = false;
                if (msgId.HasValue)
                {
                    notification.ContentId = msgId;
                }
                notification.IsSendEmail = message.IsSendEmail;

                //var flowOwnerCode = BaseWFEntity.GetProcessOwnerCode(message.FlowCode, message.ProjectId);

                if (message.ReceiverCodeList != null && message.ReceiverCodeList.Count != 0)
                {
                    //if (!string.IsNullOrEmpty(flowOwnerCode) && !message.ReceiverCodeList.Contains(flowOwnerCode))
                    //{
                    //    message.ReceiverCodeList.Add(flowOwnerCode);
                    //}
                    foreach (var receiverCode in message.ReceiverCodeList)
                    {
                        if (string.IsNullOrEmpty(receiverCode))
                            continue;
                        notification.ReceiverCode = receiverCode;
                        notification.Add();
                    }
                }

                scope.Complete();
            }
        }

        public static IQueryable<Notification> Query(NotificationSearchCondition searchCondition, out int totalSize)
        {
            var predicate = PredicateBuilder.True<Notification>();

            if (!string.IsNullOrEmpty(searchCondition.Title))
            {
                predicate = predicate.And(e => e.Title.Contains(searchCondition.Title));
            }

            if (!string.IsNullOrEmpty(searchCondition.SenderZHCN))
            {
                var employeeList = Employee.Search(e => e.NameZHCN.Contains(searchCondition.SenderZHCN));
                var employeeCodeList = employeeList.Select(e => e.Code).ToList();

                predicate = predicate.And(e => employeeCodeList.Contains(e.SenderCode));
            }

            if (!string.IsNullOrEmpty(searchCondition.SenderENUS))
            {
                var employeeList = Employee.Search(e => e.NameENUS.Contains(searchCondition.SenderENUS));
                var employeeCodeList = employeeList.Select(e => e.Code).ToList();

                predicate = predicate.And(e => employeeCodeList.Contains(e.SenderCode));
            }

            if (!string.IsNullOrEmpty(searchCondition.ReceiverAccount))
            {
                predicate = predicate.And(e => e.ReceiverCode == searchCondition.ReceiverAccount);
            }

            //从多少页开始取数据
            var notificationList = Search(predicate, e => e.CreateTime, searchCondition.PageIndex, searchCondition.PageSize,
                out totalSize, true);

            foreach (var notification in notificationList)
            {
                var notificationLocal = notification;
                var sender = Employee.FirstOrDefault(e => e.Code == notificationLocal.SenderCode);
                var receiver = Employee.FirstOrDefault(e => e.Code == notificationLocal.ReceiverCode);

                notification.SenderZHCN = sender != null ? sender.NameZHCN : string.Empty;
                notification.SenderENUS = sender != null ? sender.NameENUS : string.Empty;

                notification.ReceiverZHCN = receiver != null ? receiver.NameZHCN : string.Empty;
                notification.ReceiverENUS = receiver != null ? receiver.NameENUS : string.Empty;

                var message = NotificationMessage.FirstOrDefault(e => e.Id == notificationLocal.ContentId);
                if (message != null)
                {
                    notification.Message = message.MessageContent;
                }

            }

            return notificationList;
        }

        public static int Count(string receiverAccount, bool hasRead = false)
        {
            return Count(e => e.ReceiverCode == receiverAccount && e.HasRead == hasRead);
        }

        public static IQueryable<Notification> List(NotificationMsg message)
        {
            var predicate = PredicateBuilder.True<Notification>();

            if (!string.IsNullOrEmpty(message.FlowCode))
            {
                predicate = predicate.And(e => e.FlowCode == message.FlowCode);
            }

            if (!string.IsNullOrEmpty(message.ProjectId))
            {
                predicate = predicate.And(e => e.ProjectId == message.ProjectId);
            }

            if (!string.IsNullOrEmpty(message.SenderCode))
            {
                predicate = predicate.And(e => e.SenderCode == message.SenderCode);
            }

            if (message.ReceiverCodeList.Count > 0)
            {
                predicate = predicate.And(e => message.ReceiverCodeList.Contains(e.ReceiverCode));
            }

            if (message.RefId != Guid.Empty)
            {
                predicate = predicate.And(e => e.RefId == message.RefId);
            }

            var notifications = Search(predicate);

            return notifications;
        }

        public static void Read(NotificationMsg message)
        {
            var notifications = List(message);

            foreach (var notification in notifications)
            {
                notification.HasRead = true;
            }

        }

        public void Read(List<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                if(notification.ReceiverCode==ClientCookie.UserCode)
                    notification.HasRead = true;
            }
        }

        public List<Notification> GetNotificationBySenderCode(string senderCode, string projectId)
        {
            var context = GetDb();
            var listNotification = (from N in context.Notification
                                    join M in context.NotificationMessage
                                    on N.ContentId equals M.Id
                                    join E in context.Employee
                                    on N.ReceiverCode equals E.Code
                                    where N.SenderCode == senderCode
                                    && N.ProjectId == projectId
                                    orderby N.CreateTime descending
                                    select new
                                    {
                                        Id = N.Id,
                                        RefId = N.RefId,
                                        ProjectId = N.ProjectId,
                                        SenderCode = N.SenderCode,
                                        ReceiverCode = N.ReceiverCode,
                                        CreateTime = N.CreateTime,
                                        LastUpdateTime = N.LastUpdateTime,
                                        CreateUserAccount = N.CreateUserAccount,
                                        IsSendEmail = N.IsSendEmail,
                                        HasRead = N.HasRead,
                                        FlowCode = N.FlowCode,
                                        PositionENUS = E.PositionENUS,
                                        PositionZHCN = E.PositionZHCN,
                                        NameENUS = E.NameENUS,
                                        NameZHCN = E.NameZHCN,
                                        Message = M.MessageContent
                                    }).Distinct().ToList().Select(e => new Notification()
                                    {
                                        Id = e.Id,
                                        RefId = e.RefId,
                                        ProjectId = e.ProjectId,
                                        SenderCode = e.SenderCode,
                                        ReceiverCode = e.ReceiverCode,
                                        CreateTime = e.CreateTime,
                                        LastUpdateTime = e.LastUpdateTime,
                                        CreateUserAccount = e.CreateUserAccount,
                                        IsSendEmail = e.IsSendEmail,
                                        HasRead = e.HasRead,
                                        FlowCode = e.FlowCode,
                                        PositionENUS = e.PositionENUS,
                                        PositionZHCN = e.PositionZHCN,
                                        ReceiverZHCN = e.NameZHCN,
                                        ReceiverENUS = e.NameENUS,
                                        Message = e.Message
                                    }).ToList();

            return listNotification;

        }
        public List<Notification> GetNotificationByReceiverCode(string receiverCode, string projectId)
        {
            var context = GetDb();
            var listNotification = (from N in context.Notification
                                    join M in context.NotificationMessage
                                    on N.ContentId equals M.Id
                                    join E in context.Employee
                                    on N.SenderCode equals E.Code
                                    where N.ReceiverCode == receiverCode
                                    && N.ProjectId == projectId
                                    orderby N.CreateTime descending
                                    select new
                                    {
                                        Id = N.Id,
                                        RefId = N.RefId,
                                        ProjectId = N.ProjectId,
                                        SenderCode = N.SenderCode,
                                        ReceiverCode = N.ReceiverCode,
                                        CreateTime = N.CreateTime,
                                        LastUpdateTime = N.LastUpdateTime,
                                        CreateUserAccount = N.CreateUserAccount,
                                        IsSendEmail = N.IsSendEmail,
                                        HasRead = N.HasRead,
                                        FlowCode = N.FlowCode,
                                        PositionENUS = E.PositionENUS,
                                        PositionZHCN = E.PositionZHCN,
                                        NameENUS = E.NameENUS,
                                        NameZHCN = E.NameZHCN,
                                        Message = M.MessageContent,
                                        ContentId = N.ContentId,
                                        Title = N.Title
                                    }).ToList().Select(e => new Notification()
                                    {
                                        Id = e.Id,
                                        RefId = e.RefId,
                                        ProjectId = e.ProjectId,
                                        SenderCode = e.SenderCode,
                                        ReceiverCode = e.ReceiverCode,
                                        CreateTime = e.CreateTime,
                                        LastUpdateTime = e.LastUpdateTime,
                                        CreateUserAccount = e.CreateUserAccount,
                                        IsSendEmail = e.IsSendEmail,
                                        HasRead = e.HasRead,
                                        FlowCode = e.FlowCode,
                                        PositionENUS = e.PositionENUS,
                                        PositionZHCN = e.PositionZHCN,
                                        SenderZHCN = e.NameZHCN,
                                        SenderENUS = e.NameENUS,
                                        Message = e.Message,
                                        ContentId = e.ContentId,
                                        Title = e.Title
                                    }).ToList();

            Read(listNotification);
            Update(listNotification.ToArray());
            return listNotification;

        }

        public List<Notification> GetNotificationByProjectId(string projectId,string flowCode)
        {
            var context = GetDb();
            var listNotification = (from N in context.Notification
                                    join M in context.NotificationMessage
                                    on N.ContentId equals M.Id
                                    join E in context.Employee
                                    on N.SenderCode equals E.Code
                                    join EM in context.Employee
                                    on N.ReceiverCode equals EM.Code
                                    into TEMPEM
                                    from tem in TEMPEM.DefaultIfEmpty()
                                    where  N.ProjectId == projectId
                                    && N.FlowCode == flowCode
                                    orderby N.CreateTime descending
                                    select new
                                    {
                                        Id = N.Id,
                                        RefId = N.RefId,
                                        ProjectId = N.ProjectId,
                                        SenderCode = N.SenderCode,
                                        ReceiverCode = N.ReceiverCode,
                                        CreateTime = N.CreateTime,
                                        LastUpdateTime = N.LastUpdateTime,
                                        CreateUserAccount = N.CreateUserAccount,
                                        IsSendEmail = N.IsSendEmail,
                                        HasRead = N.HasRead,
                                        FlowCode = N.FlowCode,
                                        PositionENUS = E.PositionENUS,
                                        PositionZHCN = E.PositionZHCN,
                                        SenderNameENUS = E.NameENUS,
                                        SenderNameZHCN = E.NameZHCN,
                                        ReceiverNameENUS = tem.NameENUS,
                                        ReceiverNameZHCN = tem.NameZHCN,
                                        Message = M.MessageContent,
                                        ContentId = N.ContentId,
                                        Title = N.Title
                                    }).ToList().Select(e => new Notification()
                                    {
                                        Id = e.Id,
                                        RefId = e.RefId,
                                        ProjectId = e.ProjectId,
                                        SenderCode = e.SenderCode,
                                        ReceiverCode = e.ReceiverCode,
                                        CreateTime = e.CreateTime,
                                        LastUpdateTime = e.LastUpdateTime,
                                        CreateUserAccount = e.CreateUserAccount,
                                        IsSendEmail = e.IsSendEmail,
                                        HasRead = e.HasRead,
                                        FlowCode = e.FlowCode,
                                        PositionENUS = e.PositionENUS,
                                        PositionZHCN = e.PositionZHCN,
                                        SenderZHCN = e.SenderNameZHCN,
                                        SenderENUS = e.SenderNameENUS,
                                        ReceiverZHCN = e.ReceiverNameZHCN,
                                        ReceiverENUS = e.ReceiverNameENUS,
                                        Message = e.Message,
                                        ContentId = e.ContentId,
                                        Title = e.Title
                                    }).ToList();

            Read(listNotification);
            Update(listNotification.ToArray());
            return listNotification;
        }

        public List<FlowInfo> GetFlowList(string parentCode)
        {
            return FlowInfo.GetFlowList(parentCode).ToList();
        }
        public List<SimpleEmployee> GetCreatorList()
        {
            List<SimpleEmployee> listEmp = null;
            var entity = BaseWFEntity.GetWorkflowEntity(ProjectId, FlowCode);
            Guid? entityId = null;
            string souceCode = "";
            string tableName = "";
            if (entity != null)
            {
                souceCode = entity.WorkflowCode.Split('_')[0];
                entityId = entity.EntityId;
            }
            using (var context = GetDb())
            {
                listEmp = (from N in context.Notification
                           join E in context.Employee on N.SenderCode equals E.Code
                           where N.ProjectId == ProjectId
                           select new SimpleEmployee()
                           {
                               Code = N.SenderCode,
                               NameZHCN = E.NameZHCN,
                               NameENUS = E.NameENUS
                           }).Union(
                            from PC in context.ProjectComment
                            where PC.RefTableId==entityId && PC.SourceCode==souceCode
                            select new SimpleEmployee()
                            {
                                Code = PC.UserAccount,
                                NameZHCN = PC.UserNameZHCN,
                                NameENUS = PC.UserNameENUS
                            }
                           ).Distinct().ToList();
            }

            return listEmp;
        }

        public static List<NotificationDTO> GetNotificationList(NotificationSearchCondition condition, out int totalSize)
        {
            var context = PrepareDb();
            if (condition == null) condition = new NotificationSearchCondition();
            var entity = BaseWFEntity.GetWorkflowEntity(condition.ProjectId, condition.FlowCode);
            Guid? entityId=null;
            string souceCode = "";
            string tableName = "";
            if (entity != null)
            {
                souceCode = entity.WorkflowCode.Split('_')[0];
                entityId = entity.EntityId;
            }
            
            int pageIndex = condition.PageIndex;
            int pageSize = condition.PageSize;
            totalSize = 0;
            var data = (from N in context.Notification
                join M in context.NotificationMessage
                    on N.ContentId equals M.Id
                join E in context.Employee
                    on N.SenderCode equals E.Code
                join R in context.Employee
                    on N.ReceiverCode equals R.Code
                into TEMPEM
                from tem in TEMPEM.DefaultIfEmpty()
                    where (string.IsNullOrEmpty(condition.Title) || M.MessageContent.Contains(condition.Title))
                    && N.ProjectId==condition.ProjectId
                    && N.FlowCode==condition.FlowCode
                    && (string.IsNullOrEmpty(condition.SenderCode) || N.SenderCode==condition.SenderCode)
                    orderby N.CreateTime descending
                        select new NotificationDTO
                        {
                            Id = N.Id,
                            RefId = N.RefId,
                            ProjectId = N.ProjectId,
                            SenderCode = N.SenderCode,
                            ReceiverCode = N.ReceiverCode,
                            CreateTime = N.CreateTime,
                            LastUpdateTime = N.LastUpdateTime,
                            CreateUserAccount = N.CreateUserAccount,
                            IsSendEmail = N.IsSendEmail,
                            HasRead = N.HasRead,
                            FlowCode = N.FlowCode,
                            PositionENUS = E.PositionENUS,
                            PositionZHCN = E.PositionZHCN,
                            SenderENUS = E.NameENUS,
                            SenderZHCN = E.NameZHCN,
                            ReceiverENUS = tem.NameENUS,
                            ReceiverZHCN = tem.NameZHCN,
                            Message = M.MessageContent
                        }
                ).Union(
                    from PC in context.ProjectComment 
                    where PC.Status==ProjectCommentStatus.Submit
                    && entityId !=null
                    && PC.RefTableId==entityId
                    && PC.SourceCode ==souceCode
                    && (string.IsNullOrEmpty(condition.Title) || PC.Content.Contains(condition.Title))
                    && (string.IsNullOrEmpty(condition.SenderCode) || PC.CreateUserAccount == condition.SenderCode)
                    orderby PC.CreateTime descending
                    select new NotificationDTO
                    {
                        Id = PC.Num,
                        RefId = PC.RefTableId,
                        ProjectId = condition.ProjectId,
                        SenderCode = PC.CreateUserAccount,
                        ReceiverCode = "",
                        CreateTime = PC.CreateTime,
                        LastUpdateTime = null,
                        CreateUserAccount = PC.CreateUserAccount,
                        IsSendEmail = false,
                        HasRead = true,
                        FlowCode = condition.FlowCode,
                        PositionENUS = "",
                        PositionZHCN = "",
                        SenderENUS = string.IsNullOrEmpty(PC.CreateUserNameENUS)?PC.UserNameENUS:PC.CreateUserNameENUS,
                        SenderZHCN = string.IsNullOrEmpty(PC.CreateUserNameZHCN) ? PC.UserNameZHCN : PC.CreateUserNameZHCN,
                        ReceiverENUS = "",
                        ReceiverZHCN = "",
                        Message = PC.Content
                    }
                );
            
            IQueryable<NotificationDTO> filterData = null;
            if (condition.CreateDate.HasValue && condition.EndDate.HasValue)
            {
                var endDate = condition.EndDate.Value.AddDays(1);
                filterData =
                        data.Where(
                            e => e.CreateTime >= condition.CreateDate.Value && e.CreateTime <= endDate);
            }
            else
                filterData = data;

            totalSize = filterData.Count();
            var result = filterData
                .OrderByDescending(e=>e.CreateTime)
                .Skip(pageSize*(pageIndex - 1))
                .Take(pageSize)
                .ToList()
                .Select(e => new NotificationDTO()
                {
                    Id = e.Id,
                    RefId = e.RefId,
                    ProjectId = e.ProjectId,
                    SenderCode = e.SenderCode,
                    ReceiverCode = e.ReceiverCode,
                    CreateTime = e.CreateTime,
                    LastUpdateTime = e.LastUpdateTime,
                    CreateUserAccount = e.CreateUserAccount,
                    IsSendEmail = e.IsSendEmail,
                    HasRead = e.HasRead,
                    FlowCode = e.FlowCode,
                    PositionENUS = e.PositionENUS,
                    PositionZHCN = e.PositionZHCN,
                    SenderZHCN = e.SenderZHCN,
                    SenderENUS = e.SenderENUS,
                    ReceiverENUS = e.ReceiverENUS,
                    ReceiverZHCN = e.ReceiverZHCN,
                    Message = e.Message
                }).ToList();
            return result;
        }
    }
}
