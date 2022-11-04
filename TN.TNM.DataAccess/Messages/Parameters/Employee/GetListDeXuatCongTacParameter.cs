﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetListDeXuatCongTacParameter : BaseParameter    {
        
        public Guid? NguoiDeXuatId { get; set; }

        public DateTime? ThoiGianDeXuat { get; set; }

        public int? TrangThaiDeXuat { get; set; }
    }
}
