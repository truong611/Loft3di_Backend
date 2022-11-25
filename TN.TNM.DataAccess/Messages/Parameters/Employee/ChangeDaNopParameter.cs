using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class ChangeDaNopParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public int? TaiLieuNhanVienId { get; set; }
        public int CauHinhChecklistId { get; set; }
        public bool IsDaNop { get; set; }
    }
}
