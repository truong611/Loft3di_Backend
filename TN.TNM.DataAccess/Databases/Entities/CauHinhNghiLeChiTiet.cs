﻿using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHinhNghiLeChiTiet
    {
        public int NghiLeChiTietId { get; set; }
        public int NghiLeId { get; set; }
        public int? LoaiNghiLe { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime Ngay { get; set; }
    }
}
