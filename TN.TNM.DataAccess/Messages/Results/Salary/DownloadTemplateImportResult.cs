using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class DownloadTemplateImportResult : BaseResult
    {
        public byte[] TemplateExcel { get; set; }
        public string FileName { get; set; }
    }
}
