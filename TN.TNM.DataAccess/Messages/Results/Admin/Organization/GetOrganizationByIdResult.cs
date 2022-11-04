﻿using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class GetOrganizationByIdResult : BaseResult
    {
        public OrganizationEntityModel Organization { get; set; }
        public List<ThanhVienPhongBanModel> ListThanhVienPhongBan { get; set; }
    }
}
