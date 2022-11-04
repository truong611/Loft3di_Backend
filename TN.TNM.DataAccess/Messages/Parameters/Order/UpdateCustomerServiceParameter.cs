﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class UpdateCustomerServiceParameter : BaseParameter
    {
        public Guid OrderId { get; set; }
        public List<CustomerOrderDetailEntityModel> ListCustomerOrderDetail { get; set; }
    }
}
