﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class SendEmailCustomerQuoteParameter : BaseParameter
    {
        public List<string> ListEmail { get; set; }

        public List<string> ListEmailCC { get; set; }

        public List<string> ListEmailBcc { get; set; }

        public string TitleEmail { get; set; }
        public string ContentEmail { get; set; }
        public Guid QuoteId { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
    }
}
