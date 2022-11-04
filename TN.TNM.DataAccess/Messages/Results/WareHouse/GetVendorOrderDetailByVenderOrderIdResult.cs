﻿using System.Collections.Generic;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetVendorOrderDetailByVenderOrderIdResult : BaseResult
    {
        public List<GetVendorOrderDetailByVenderOrderIdEntityModel> ListOrderProduct { get; set; }
    }
}
