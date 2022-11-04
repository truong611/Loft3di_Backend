using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting
{
    public class DeleteNotiByIdParameter : BaseParameter
    {
        public Guid NotifiSettingId { get; set; }
    }
}
