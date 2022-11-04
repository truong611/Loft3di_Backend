﻿using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class SearchCustomerParameter : BaseParameter
    {
        public bool NoPic { get; set; }

        public bool IsBusinessCus { get; set; }

        public bool IsPersonalCus { get; set; }
        public bool IsAgentCus { get; set; }

        public Guid? StatusCareId { get; set; }

        public List<Guid?> CustomerGroupIdList { get; set; }

        public List<Guid?> PersonInChargeIdList { get; set; }

        public Guid? NhanVienChamSocId { get; set; }

        public Guid? AreaId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public bool KhachDuAn { get; set; }

        public bool KhachBanLe { get; set; }
    }
}
