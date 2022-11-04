using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class JobParameter
    {
        public long JobId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public Job Job { get; set; }
    }
}
