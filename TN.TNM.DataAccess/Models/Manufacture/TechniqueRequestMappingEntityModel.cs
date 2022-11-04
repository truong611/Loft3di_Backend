﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class TechniqueRequestMappingEntityModel
    {
        public Guid TechniqueRequestMappingId { get; set; }
        public Guid ProductionOrderMappingId { get; set; }
        public Guid TechniqueRequestId { get; set; }
        public Guid? ParentId { get; set; }
        public byte? Rate { get; set; }
        public string TechniqueName { get; set; }
        public byte TechniqueOrder { get; set; }
        public double? TechniqueValue { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public bool? Thin { get; set; }
        public bool? Thick { get; set; }
        public bool? EspeciallyThin { get; set; }
        public bool? EspeciallyThick { get; set; }
        public bool? BoreholeThin { get; set; }
        public bool? BoreholeThick { get; set; }
        public bool? OriginalThin { get; set; }
        public bool? OriginalThick { get; set; }

        public double? UnitQuantity { get; set; }
    }
}
