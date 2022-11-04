﻿using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeRequestEntityModel:BaseModel<EmployeeRequest>
    {
        
        public Guid EmployeeRequestId { get; set; }
        public string EmployeeRequestCode { get; set; }
        public Guid? CreateEmployeeId { get; set; }
        public string CreateEmployeeCode { get; set; }
        public Guid OfferEmployeeId { get; set; }
        public string OfferEmployeeCode { get; set; }
        public Guid? TypeRequest { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EnDate { get; set; }
        public Guid? StartTypeTime { get; set; }
        public Guid? EndTypeTime { get; set; }
        public DateTime? RequestDate { get; set; }
        public Guid? TypeReason { get; set; }
        public string Detail { get; set; }
        public Guid? ManagerId { get; set; }
        public Guid? ApproverId { get; set; }
        public string NotifyList { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

        public string OfferEmployeeName { get; set; }
        public string Organization { get; set; }
        public Guid? OrganizationId { get; set; }
        public string OrganizationCode { get; set; }
        public string ApproverName { get; set; }
        public string TypeRequestName { get; set; }
        public string ShiftName { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }
        public string CreateEmployeeName { get; set; }
        public Guid? ContactId { get; set; }
        public string BackgroupStatusColor { get; set; }

        public Guid? TenantId { get; set; }
        public int? StepNumber { get; set; }

        public EmployeeRequestEntityModel(EmployeeRequest employeeRequest)
        {
            Mapper(employeeRequest, this);

            //EmployeeRequestId = employeeRequest.EmployeeRequestId;
            //EmployeeRequestCode = employeeRequest.EmployeeRequestCode;
            //CreateEmployeeId = employeeRequest.CreateEmployeeId;
            //CreateEmployeeCode = employeeRequest.CreateEmployeeCode;
            //OfferEmployeeId = employeeRequest.OfferEmployeeId;
            //OfferEmployeeCode = employeeRequest.OfferEmployeeCode;
            //TypeRequest = employeeRequest.TypeRequest;
            //StatusId = employeeRequest.StatusId;
            //StartDate = employeeRequest.StartDate;
            //EnDate = employeeRequest.EnDate;
            //StartTypeTime = employeeRequest.StartTypeTime;
            //EndTypeTime = employeeRequest.EndTypeTime;
            //RequestDate = employeeRequest.RequestDate;
            //TypeReason = employeeRequest.TypeReason;
            //Detail = employeeRequest.Detail;
            //ManagerId = employeeRequest.ManagerId;
            //ApproverId = employeeRequest.ApproverId;
            //NotifyList = employeeRequest.NotifyList;
            //CreateById = employeeRequest.CreateById;
            //CreateDate = employeeRequest.CreateDate;
            //UpdateDate = employeeRequest.UpdateDate;
            //UpdateById = employeeRequest.UpdateById;
        }

        public EmployeeRequestEntityModel()
        {
        }

        public override EmployeeRequest ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new EmployeeRequest();
            Mapper(this, entity);
            return entity;
        }
    }
}