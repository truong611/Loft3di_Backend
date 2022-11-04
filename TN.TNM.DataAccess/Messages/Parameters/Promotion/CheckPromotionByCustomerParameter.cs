﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class CheckPromotionByCustomerParameter : BaseParameter
    {
        public Guid CustomerId { get; set; }

        public bool? IsSendEmail { get; set; }
    }
}
