﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetCauHinhBaoHiemByIdResult : BaseResult
    {
        public CauHinhBaoHiemModel CauHinhBaoHiem { get; set; }
    }
}
