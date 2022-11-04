﻿using System;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class GetMasterDataPayableInvoiceRequest : BaseRequest<GetMasterDataPayableInvoiceParameter>
    {
        public Guid? VendorOrderId { get; set; }

        public override GetMasterDataPayableInvoiceParameter ToParameter() => new GetMasterDataPayableInvoiceParameter
        {
            VendorOrderId = VendorOrderId,
            UserId = UserId
        };
    }
}
