using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mcdonalds.AM.DataAccess;

namespace Mcdonalds.AM.Services.Common
{
    public class UserPositionHandler
    {
//        //法人Code
//        public const string LegalCounselCode = "suoya450007";
//        //财务Code
//        public const string FinanceTeamCode = "suoya303057";
//        public const string IT = "suoya625647";
//        public const string MCCL_Construction_Mgr = "suoya612026";

//        private McdAMEntities _db = new McdAMEntities();
//        private ClosureInfo _closureInfoHandler = new ClosureInfo();
//        /// <summary>
//        /// 根据UserPositionList获取账号
//        /// </summary>
//        /// <param name="projectId"></param>
//        /// <param name="userPositionList"></param>
//        /// <returns></returns>
//        public  string GetAccounts(string projectId, string positionCode)
//        {
//            string accounts = string.Empty;
//            var closureInfo = _closureInfoHandler.GetByProjectId(projectId);
//            var userPositionList = SearchUserPosition(closureInfo.USCode, positionCode);
//            foreach (var item in userPositionList)
//            {
//                accounts += item.Code + ";";
//            }
//            accounts = accounts.Trim(';');
//            return accounts;
//        }


//        public string GetAccounts( List<SPosition> userPositionList)
//        {
//            string accounts = string.Empty;
         
//            foreach (var item in userPositionList)
//            {
//                accounts += item.Code + ";";
//            }
//            accounts = accounts.Trim(';');
//            return accounts;
//        }
//        public string GetAccounts(List<UserPosition> userPositionList)
//        {
//            string accounts = string.Empty;

//            foreach (var item in userPositionList)
//            {
//                //accounts += item.UserCode + ";";
//            }
//            accounts = accounts.Trim(';');
//            return accounts;
//        }

//        public string GetReportToAccounts(string userCode)
//        {
      
//           var list = _db.UserPosition.Where(e => e.UserCode == userCode);
//            var accounts = GetAccounts(list.ToList());
//            return accounts;
//        }

//        public string GetReportToAccounts(List<SPosition> userPositionList)
//        {
//            string[] reportToIds;

//            List<string> reportToIdList = new List<string>();

//            foreach (var item in userPositionList)
//            {
//                reportToIdList.Add(item.ReportTo);
//            }
//            reportToIds = reportToIdList.ToArray();

//            IQueryable<Employee> employeeList = _db.Employee.Where(e => reportToIds.Contains(e.Id.ToString()));
//            string accounts = string.Empty;
//            foreach (var employee in employeeList)
//            {
//                accounts += employee + ";";
//            }
//            accounts = accounts.Trim(';');
//            return accounts;
//        }

    

//        public class SPosition
//        {
//            public string Code { get; set; }
//            public string NameENUS { get; set; }
//            public string PositionName { get; set; }
//            public string PositionCode { get; set; }
//            public string ReportTo { get; set; }
//        }

//        public List<SPosition> SearchUserPosition(string storeCode, string[] positionCodes)
//        {
//            string codes = string.Empty;
//            foreach (var item in positionCodes)
//            {
//                codes += "'"+item + "',";
//            }
//            codes = codes.Trim(',');

//            string sql = string.Format(@"SELECT tb_employee.Code,tb_employee.NameENUS,tb_position.NameENUS as PositionName,tb_position.PositionCode,tb_position.ReportTo
//                FROM dbo.Store tb_store
//                INNER JOIN dbo.UserMiniMarket tb_mini
//                ON tb_mini.MiniMarketCode = tb_store.MiniMarketCode
//                INNER JOIN dbo.UserPosition tb_position
//                ON tb_position.Id = tb_mini.PositionId
//                INNER JOIN dbo.Employee tb_employee
//                ON tb_employee.Id = tb_position.UserId
//                WHERE tb_store.code = '{0}' AND tb_position.PositionCode in ({1})
//                  AND tb_employee.Status = 1
//                ORDER BY PositionCode", storeCode,codes);



//            var list = _db.Database.SqlQuery<SPosition>(sql);
//            var s = list.ToList();
//            return s;
//        }

//        public List<SPosition> SearchUserPosition(string storeCode, string positionCode)
//        {
           

//            string sql = string.Format(@"SELECT tb_employee.Code,tb_employee.NameENUS,tb_position.NameENUS as PositionName,tb_position.PositionCode 
//                FROM dbo.Store tb_store
//                INNER JOIN dbo.UserMiniMarket tb_mini
//                ON tb_mini.MiniMarketCode = tb_store.MiniMarketCode
//                INNER JOIN dbo.UserPosition tb_position
//                ON tb_position.Id = tb_mini.PositionId
//                INNER JOIN dbo.Employee tb_employee
//                ON tb_employee.Id = tb_position.UserId
//                WHERE tb_store.code = '{0}' AND tb_position.PositionCode = '{1}'
//                AND tb_employee.Status = 1
//                ORDER BY PositionCode", storeCode, positionCode);
//            //AND tb_employee.Status = 1
//            var list = _db.Database.SqlQuery<SPosition>(sql);
//            var s = list.ToList();
//            return s;
//        }

//        public List<SPosition> SearchUserAccounts(string projectId, string positionCode)
//        {
//            var closureInfo = _closureInfoHandler.GetByProjectId(projectId);
//            string usCode = closureInfo.USCode;

//            string sql = string.Format(@"SELECT tb_employee.Code,tb_employee.NameENUS,tb_position.NameENUS as PositionName,tb_position.PositionCode 
//                FROM dbo.Store tb_store
//                INNER JOIN dbo.UserMiniMarket tb_mini
//                ON tb_mini.MiniMarketCode = tb_store.MiniMarketCode
//                INNER JOIN dbo.UserPosition tb_position
//                ON tb_position.Id = tb_mini.PositionId
//                INNER JOIN dbo.Employee tb_employee
//                ON tb_employee.Id = tb_position.UserId
//                WHERE tb_store.code = '{0}' AND tb_position.PositionCode = '{1}'
//                AND tb_employee.Status = 1
//                ORDER BY PositionCode", usCode, positionCode);
//            //AND tb_employee.Status = 1
//            var list = _db.Database.SqlQuery<SPosition>(sql);
//            var s = list.ToList();
//            return s;
//        }

    }
}