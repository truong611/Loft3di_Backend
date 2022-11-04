using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class State
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Data { get; set; }

        public Job Job { get; set; }
    }
}
