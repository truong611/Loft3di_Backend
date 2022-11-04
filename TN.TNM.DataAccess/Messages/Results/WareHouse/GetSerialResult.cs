﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetSerialResult:BaseResult
    {
        public List<SerialEntityModel> lstSerial { get; set;}
    }
}
