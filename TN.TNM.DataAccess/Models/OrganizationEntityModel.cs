﻿using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Models
{
    public class OrganizationEntityModel : BaseModel<Databases.Entities.Organization>
    {
        public Guid? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        public string ParentName { get; set; }
        public List<OrganizationEntityModel> OrgChildList { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public bool? IsFinancialIndependence { get; set; }
        public Guid? GeographicalAreaId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? SatelliteId { get; set; }
        public string OrganizationOtherCode { get; set; }
        public bool? IsHR { get; set; }
        public List<Guid?> ListChildOrganizationId { get; set; }
        public bool? HasChildren { get; set; }
        public bool? IsAccess { get; set; }
        public Guid? NguoiPhuTrachId { get; set; }
        public string NguoiPhuTrachName { get; set; }
        public List<EmployeeEntityModel> ListNguoiDanhGiaPhuTrach { get; set; }

        public OrganizationEntityModel()
        {

        }

        public OrganizationEntityModel(Databases.Entities.Organization model)
        {
            Mapper(model, this);
        }

        public override Databases.Entities.Organization ToEntity()
        {
            var entity = new Databases.Entities.Organization();
            Mapper(this, entity);
            return entity;
        }
    }
}
