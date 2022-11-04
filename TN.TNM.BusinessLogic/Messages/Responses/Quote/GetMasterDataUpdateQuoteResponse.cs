﻿using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Promotion;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.QuyTrinh;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetMasterDataUpdateQuoteResponse : BaseResponse
    {
        public List<CategoryEntityModel> ListInvestFund { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<CategoryEntityModel> ListQuoteStatus { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<CustomerEntityModel> ListCustomerNew { get; set; }
        public List<LeadEntityModel> ListAllLead { get; set; }
        public List<SaleBiddingEntityModel> ListAllSaleBidding { get; set; }
        public List<EmployeeEntityModel> ListParticipant { get; set; }
        public QuoteEntityModel Quote { get; set; }
        public List<QuoteDetailEntityModel> ListQuoteDetail { get; set; }
        public List<QuoteDocumentEntityModel> ListQuoteDocument { get; set; }
        public List<AdditionalInformationEntityModel> ListAdditionalInformation { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<QuoteCostDetailEntityModel> ListQuoteCostDetail { get; set; }
        public bool IsApproval { get; set; }
        public List<Guid> ListParticipantId { get; set; }
        public bool IsParticipant { get; set; }
        public List<PromotionObjectApplyEntityModel> ListPromotionObjectApply { get; set; }
        public List<QuotePlanEntityModel> ListQuotePlans { get; set; }
        public List<QuoteScopeEntityModel> ListQuoteScopes { get; set; }
        public List<QuotePaymentTermEntityModel> ListQuotePaymentTerm { get; set; }
        public bool IsShowGuiPheDuyet { get; set; }
        public bool IsShowPheDuyet { get; set; }
        public bool IsShowTuChoi { get; set; }
    }
}
