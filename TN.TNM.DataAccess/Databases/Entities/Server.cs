using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Server
    {
        public string Id { get; set; }
        public string Data { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}
