﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.QuyTrinh
{
    public class GetDuLieuQuyTrinhParameter : BaseParameter
    {
        public Guid ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }
        public int? ObjectNumber { get; set; }
    }
}
