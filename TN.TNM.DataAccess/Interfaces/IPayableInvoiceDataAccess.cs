﻿using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;
using TN.TNM.DataAccess.Messages.Results.PayableInvoice;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IPayableInvoiceDataAccess
    {
        CreatePayableInvoiceResult CreatePayableInvoice(CreatePayableInvoiceParameter parameter);
        GetPayableInvoiceByIdResult GetPayableInvoiceById(GetPayableInvoiceByIdParameter parameter);
        SearchPayableInvoiceResult SearchPayableInvoice(SearchPayableInvoiceParameter parameter);
        CreateBankPayableInvoiceResult CreateBankPayableInvoice(CreateBankPayableInvoiceParameter parameter);
        SearchBankPayableInvoiceResult SearchBankPayableInvoice(SearchBankPayableInvoiceParameter parameter);
        GetBankPayableInvoiceByIdResult GetBankPayableInvoiceById(GetBankPayableInvoiceByIdParameter parameter);
        ExportBankPayableInvoiceResult ExportBankPayableInvoice(ExportBankPayableInvoiceParameter parameter);
        ExportPayableInvoiceResult ExportPayableInvoice(ExportPayableInvoiceParameter parameter);
        SearchBankBookPayableInvoiceResult SearchBankBookPayableInvoice(SearchBankBookPayableInvoiceParameter parameter);
        SearchCashBookPayableInvoiceResult SearchCashBookPayableInvoice(SearchCashBookPayableInvoiceParameter parameter);
        GetMasterDataPayableInvoiceResult GetMasterDataPayableInvoice(GetMasterDataPayableInvoiceParameter parameter);
        GetMasterDataBankPayableInvoiceResult GetMasterDataBankPayableInvoice(GetMasterDataBankPayableInvoiceParameter parameter);
        GetMasterDataPayableInvoiceSearchResult GetMasterDataPayableInvoiceSearch(GetMasterDataPayableInvoiceSearchParameter parameter);
        GetMasterDataSearchBankPayableInvoiceResult GetMasterDataSearchBankPayableInvoice(GetMasterDataSearchBankPayableInvoiceParameter parameter);
    }
}
