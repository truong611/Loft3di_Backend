﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.DataAccess.Messages.Parameters.MenuBuild
{
    public class UpdateMenuBuildParameter: BaseParameter
    {
        public MenuBuildEntityModel MenuBuild { get; set; }
    }
}
