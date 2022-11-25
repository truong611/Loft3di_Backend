using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class GetTkDiMuonVeSomParameter : BaseParameter
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
        public int IsShowOption { get; set; }
        public List<int> ListOption { get; set; } /* 1: Đi muộn về sớm, 2: Chấm công thiếu ca */
    }
}
