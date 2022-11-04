﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class SendEmailVendorQuoteRequest : BaseRequest<SendMailVendorQuoteParameter>
    {
        public List<string> ListEmail { get; set; }

        public List<string> ListEmailCC { get; set; }

        public List<string> ListEmailBcc { get; set; }

        public string TitleEmail { get; set; }
        public string ContentEmail { get; set; }
        public Guid SuggestedSupplierQuoteId { get; set; }
        
        //public string Base64Pdf { get; set; }

        public List<IFormFile> ListFormFile { get; set; }

        public override SendMailVendorQuoteParameter ToParameter()
        {
            return new SendMailVendorQuoteParameter
            {
                ListEmail = ListEmail,
                ListEmailCC = ListEmailCC,
                ListEmailBcc = ListEmailBcc,
                TitleEmail = TitleEmail,
                ContentEmail = ContentEmail,
                SuggestedSupplierQuoteId = SuggestedSupplierQuoteId,
                //Base64Pdf = Base64Pdf,
                ListFormFile = ListFormFile,
                UserId = UserId
            };
        }
    }
}
