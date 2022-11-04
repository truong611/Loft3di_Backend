using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class GetDataExportOTParameter: BaseParameter
    {
        public int KyLuongId { get; set; }
        public int BaoCaoNumber { get; set; }
    }
}
