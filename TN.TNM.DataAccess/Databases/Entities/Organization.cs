﻿using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Organization
    {
        public Organization()
        {
            BankPayableInvoice = new HashSet<BankPayableInvoice>();
            BankReceiptInvoice = new HashSet<BankReceiptInvoice>();
            Employee = new HashSet<Employee>();
            InverseParent = new HashSet<Organization>();
            PayableInvoice = new HashSet<PayableInvoice>();
            ReceiptInvoice = new HashSet<ReceiptInvoice>();
        }

        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public bool? IsFinancialIndependence { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? GeographicalAreaId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? SatelliteId { get; set; }
        public string OrganizationOtherCode { get; set; }
        public bool? IsHr { get; set; }
        public bool? IsAccess { get; set; }

        public Organization Parent { get; set; }
        public ICollection<BankPayableInvoice> BankPayableInvoice { get; set; }
        public ICollection<BankReceiptInvoice> BankReceiptInvoice { get; set; }
        public ICollection<Employee> Employee { get; set; }
        public ICollection<Organization> InverseParent { get; set; }
        public ICollection<PayableInvoice> PayableInvoice { get; set; }
        public ICollection<ReceiptInvoice> ReceiptInvoice { get; set; }
    }
}
