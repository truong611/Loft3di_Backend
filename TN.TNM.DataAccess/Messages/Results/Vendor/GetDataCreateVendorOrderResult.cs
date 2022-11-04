﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.PurchaseOrderStatus;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataCreateVendorOrderResult: BaseResult
    {
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<PurchaseOrderStatusEntityModel> ListOrderStatus { get; set; }
        public List<BankAccountEntityModel> ListBankAccount { get; set; }
        public List<Models.Employee.EmployeeEntityModel> ListEmployeeModel { get; set; }
        public List<Models.Vendor.VendorCreateOrderEntityModel> VendorCreateOrderModel { get; set; }
        public List<ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
        //public List<ContractEntityModel> ListContract { get; set; }
        public List<WareHouseEntityModel> ListWareHouse { get; set; }
        public List<CategoryEntityModel> ListVendorGroup { get; set; }
    }
}
