﻿using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class UpdateCustomerCareResult:BaseResult
    {
        public Guid CustomerCareId { get; set; }
        public List<Guid> CustomerCareCustomer { get; set; }
    }
}
