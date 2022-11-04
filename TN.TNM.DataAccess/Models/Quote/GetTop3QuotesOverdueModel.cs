﻿using System;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class GetTop3QuotesOverdueModel
    {
        public Guid QuoteId { get; set; }
        public string QuoteCode { get; set; }
        public string QuoteName { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? SendQuoteDate { get; set; }
        public DateTime? IntendedQuoteDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal Amount { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? EffectiveQuoteDate { get; set; }

        public decimal?  TotalAmountAfterVat { get; set; }
        
        public decimal?  TotalAmount { get; set; }
        
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }


    }
}
