﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetMasterSearchKeHoachOtParameter : BaseParameter
    {
        public List<byte> ListTrangThai { get; set; }
    }
}
