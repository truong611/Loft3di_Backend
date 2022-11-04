﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class UpdateOrganizationByIdRequest : BaseRequest<UpdateOrganizationByIdParameter>
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationPhone { get; set; }
        public string OrganizationAddress { get; set; }
        public bool IsFinancialIndependence { get; set; }
        public Guid? GeographicalAreaId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? SatelliteId { get; set; }
        public string OrganizationOtherCode { get; set; }
        public List<ThanhVienPhongBanModel> ListThanhVienPhongBan { get; set; }

        public override UpdateOrganizationByIdParameter ToParameter()
        {
            return new UpdateOrganizationByIdParameter()
            {
                UserId = UserId,
                OrganizationId = OrganizationId,
                OrganizationName = OrganizationName,
                OrganizationCode = OrganizationCode,
                OrganizationPhone = OrganizationPhone,
                OrganizationAddress = OrganizationAddress,
                IsFinancialIndependence = IsFinancialIndependence,
                GeographicalAreaId = GeographicalAreaId,
                ProvinceId = ProvinceId,
                DistrictId = DistrictId,
                WardId = WardId,
                SatelliteId = SatelliteId,
                OrganizationOtherCode = OrganizationOtherCode,
                ListThanhVienPhongBan = ListThanhVienPhongBan
            };
        }
    }
}
