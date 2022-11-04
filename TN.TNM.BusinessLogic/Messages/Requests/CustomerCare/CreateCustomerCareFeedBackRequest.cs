﻿using TN.TNM.BusinessLogic.Models.CustomerCare;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class CreateCustomerCareFeedBackRequest : BaseRequest<CreateCustomerCareFeedBackParameter>
    {
        public CustomerCareFeedBackEntityModel CustomerCareFeedBack { get; set; }

        public override CreateCustomerCareFeedBackParameter ToParameter()
        {
            return new CreateCustomerCareFeedBackParameter
            {
                CustomerCareFeedBack = this.CustomerCareFeedBack
            };
        }
    }
}
