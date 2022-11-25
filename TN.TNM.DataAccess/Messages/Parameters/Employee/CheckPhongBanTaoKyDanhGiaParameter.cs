using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CheckPhongBanTaoKyDanhGiaParameter : BaseParameter
    {
        public List<Guid> ListOrgId { get;set;}
        public Guid OrgAddId  { get;set;}
    }
}
