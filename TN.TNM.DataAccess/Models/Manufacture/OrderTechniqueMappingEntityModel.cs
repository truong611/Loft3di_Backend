using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class OrderTechniqueMappingEntityModel
    {
        public Guid OrderTechniqueMappingId { get; set; }
        public Guid ProductOrderWorkflowId { get; set; }
        public Guid TechniqueRequestId { get; set; }
        public byte? TechniqueOrder { get; set; }
        public byte? Rate { get; set; }
        public bool? IsDefault { get; set; }
        public bool? Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public string TechniqueName { get; set; }
    }
}
