﻿using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.BankBook;
using TN.TNM.DataAccess.Messages.Results.BankBook;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class BankBookDAO : BaseDAO, IBankBookDataAccess
    {
        public BankBookDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetMaterDataSearchBankBookResult GetMasterDataSearchBankBook(GetMasterDataSearchBankBookParameter parameter)
        {
            try
            {
                var listBankCount = context.BankAccount.Where(c => c.ObjectType == ObjectType.COMPANY).OrderBy(c => c.BankName).ToList();
                var listBankCountResult = new List<BankAccountEntityModel>();
                listBankCount.ForEach(item =>
                {
                    listBankCountResult.Add(new BankAccountEntityModel(item));
                });

                var listEmployee = context.Employee.Where(c => c.Active == true).OrderBy(c => c.EmployeeName).ToList();
                var listEmployeeResult = new List<EmployeeEntityModel>();
                listEmployee.ForEach(item =>
                {
                    listEmployeeResult.Add(new EmployeeEntityModel(item));
                });

                return new GetMaterDataSearchBankBookResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListBankAccount = listBankCountResult,
                    ListEmployee = listEmployeeResult
                };
            }
            catch (Exception ex)
            {
                return new GetMaterDataSearchBankBookResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public SearchBankBookResult SearchBankBook(SearchBankBookParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.BANKBOOK, "Search Bank Book", parameter.UserId);

                if (parameter.BankAccountId.Count > 0)
                {
                    decimal SumOpeningBalance = 0;
                    decimal SumClosingBalance = 0;

                    parameter.BankAccountId.ForEach(item =>
                    {
                        decimal itemOpeningBalance = 0;
                        decimal itemClosingBalance = 0;

                        var objectBankBookAmount = (from bb in context.BankBook
                                                    where bb.CreateDate == (from bb1 in context.BankBook
                                                                            where (parameter.FromPaidDate == null || parameter.FromPaidDate == DateTime.MinValue ||
                                                                            bb1.CreateDate < parameter.FromPaidDate.Value)
                                                                            && bb1.BankAccountId == item
                                                                            select bb1.CreateDate).Max().GetValueOrDefault()
                                                    && bb.BankAccountId == item
                                                    select bb.Amount).FirstOrDefault();
                        if (objectBankBookAmount.HasValue)
                        {
                            itemOpeningBalance = objectBankBookAmount.Value;
                        }

                    var objectBankBookAmountClosingBalance = (from bb in context.BankBook
                                                              where bb.CreateDate == (from bb1 in context.BankBook
                                                                                      where (parameter.FromPaidDate == null || parameter.FromPaidDate == DateTime.MinValue ||
                                                                                      bb1.CreateDate >= parameter.FromPaidDate.Value)
                                                                                      && (parameter.ToPaidDate == null || parameter.ToPaidDate == DateTime.MinValue ||
                                                                                      bb1.CreateDate <= parameter.ToPaidDate.Value)
                                                                                          && bb1.BankAccountId == item
                                                                                          select bb1.CreateDate).Max().GetValueOrDefault()
                                                                  && bb.BankAccountId == item
                                                                  select bb.Amount).FirstOrDefault();

                        if (objectBankBookAmountClosingBalance.HasValue)
                        {
                            itemClosingBalance = objectBankBookAmountClosingBalance.Value;
                        }
                        else
                        {
                            itemClosingBalance = itemOpeningBalance;
                        }
                        SumOpeningBalance = SumOpeningBalance + itemOpeningBalance;
                        SumClosingBalance = SumClosingBalance + itemClosingBalance;

                    });
                    return new SearchBankBookResult
                    {
                        OpeningBalance = SumOpeningBalance,
                        ClosingBalance = SumClosingBalance,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                else
                {
                    var listBankAccount = (from BA in context.BankAccount
                                           where BA.ObjectType == "COM"
                                           select BA.BankAccountId).ToList();
                    decimal SumOpeningBalance = 0;
                    decimal SumClosingBalance = 0;

                    listBankAccount.ForEach(item =>
                    {
                        decimal itemOpeningBalance = 0;
                        decimal itemClosingBalance = 0;

                        var objectBankBookAmount = (from bb in context.BankBook
                                                    where bb.CreateDate == (from bb1 in context.BankBook
                                                                            where (parameter.FromPaidDate == null || parameter.FromPaidDate == DateTime.MinValue||
                                                                            bb1.CreateDate < parameter.FromPaidDate.Value)
                                                                            && bb1.BankAccountId == item
                                                                            select bb1.CreateDate).Max().GetValueOrDefault()
                                                    && bb.BankAccountId == item
                                                    select bb.Amount).FirstOrDefault();
                        if (objectBankBookAmount.HasValue)
                        {
                            itemOpeningBalance = objectBankBookAmount.Value;
                        }


                    var objectBankBookAmountClosingBalance = (from bb in context.BankBook
                                                              where bb.CreateDate == (from bb1 in context.BankBook
                                                                                      where (parameter.FromPaidDate == null || parameter.FromPaidDate == DateTime.MinValue ||
                                                                                      bb1.CreateDate >= parameter.FromPaidDate.Value)
                                                                                          && (parameter.ToPaidDate == null || parameter.ToPaidDate == DateTime.MinValue ||
                                                                                          bb1.CreateDate <= parameter.ToPaidDate.Value)
                                                                                          && bb1.BankAccountId == item
                                                                                          select bb1.CreateDate).Max().GetValueOrDefault()
                                                                  && bb.BankAccountId == item
                                                                  select bb.Amount).FirstOrDefault();

                        if (objectBankBookAmountClosingBalance.HasValue)
                        {
                            itemClosingBalance = objectBankBookAmountClosingBalance.Value;
                        }
                        else
                        {
                            itemClosingBalance = itemOpeningBalance;
                        }
                        SumOpeningBalance = SumOpeningBalance + itemOpeningBalance;
                        SumClosingBalance = SumClosingBalance + itemClosingBalance;
                    });
                    return new SearchBankBookResult
                    {
                        OpeningBalance = SumOpeningBalance,
                        ClosingBalance = SumClosingBalance,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };

                }
            }
            catch (Exception ex)
            {

                return new SearchBankBookResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }


    }
}
