﻿using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class MenuBuild
    {
        public Guid MenuBuildId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CodeParent { get; set; }
        public byte Level { get; set; }
        public string Path { get; set; }
        public string NameIcon { get; set; }
        public short IndexOrder { get; set; }
        public bool? IsPageDetail { get; set; }
        public Guid? TenantId { get; set; }
        public bool? IsShow { get; set; }
    }
}
