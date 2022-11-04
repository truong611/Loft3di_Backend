﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Category;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetDataCreatePotentialCustomerResult: BaseResult
    {
        public List<CategoryEntityModel> ListInvestFund { get; set; }
        public List<EmployeeEntityModel> ListEmployeeModel { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<ProvinceEntityModel> ListProvinceEntityModel { get; set; }
        public List<DistrictEntityModel> ListDistrictEntityModel { get; set; }
        public List<WardEntityModel> ListWardEntityModel { get; set; }
        public List<CategoryEntityModel> ListGroupCustomer { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<EmployeeEntityModel> ListEmployeeTakeCare { get; set; }
    }
}
