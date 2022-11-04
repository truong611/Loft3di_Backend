﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.DynamicColumnTable;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetDataBaoCaoAllowancesResult : BaseResult
    {
        public List<string> ListHeader1 { get; set; }
        public List<string> ListHeader2 { get; set; }
        public List<List<DataRowModel>> ListData { get; set; }
        public int TongSoCotDong { get; set; }
    }
}
