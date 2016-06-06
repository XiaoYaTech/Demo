using NTTMNC.BPM.Fx.K2.Services.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mcdonalds.AM.DataAccess.Workflow
{
    public class K2Parameter
    {
        public string EmployeeCode { get; set; }
        public string Comment { get; set; }
        public string SerialNumber { get; set; }
        public TaskWork Task { get; set; }
        private List<ProcessDataField> dataFields = new List<ProcessDataField>();
        public List<ProcessDataField> DataFields
        {
            get
            {
                return dataFields;
            }
        }
        public void AddDataField(string key, string value)
        {
            ProcessDataField dataField = new ProcessDataField(key, value);
            dataFields.Add(dataField);
        }
    }
}
