using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class ChangeNgayNopTaiLieuParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public DateTime? NgayNop { get; set; }
        public DateTime? NgayHenNop { get; set; }
    }
}
