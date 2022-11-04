﻿using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Cost;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetMasterDataCreateCostResult : BaseResult
    {
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<CostEntityModel> ListCost { get; set; }
    }
}
