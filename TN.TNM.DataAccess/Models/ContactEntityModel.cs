﻿using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models
{
    public class ContactEntityModel : BaseModel<DataAccess.Databases.Entities.Contact>
    {
        public Guid ContactId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string WorkPhone { get; set; }
        public string OtherPhone { get; set; }
        public string Email { get; set; }
        public string WorkEmail { get; set; }
        public string OtherEmail { get; set; }
        public string IdentityId { get; set; }
        public string AvatarUrl { get; set; }
        public string Address { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public string PostCode { get; set; }
        public string WebsiteUrl { get; set; }
        public string SocialUrl { get; set; }
        public string Note { get; set; }
        public string Role { get; set; }
        public string TaxCode { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? MaritalStatusId { get; set; }
        public string Job { get; set; }
        public string Agency { get; set; }
        public string Birthplace { get; set; }
        public string HoKhauThuongTruTA { get; set; }
        public string HoKhauThuongTruTV { get; set; }
        public string AddressTiengAnh { get; set; }
        public DateTime? IdentityIddateOfIssue { get; set; }
        public string IdentityIdplaceOfIssue { get; set; }
        public DateTime? IdentityIddateOfParticipation { get; set; }
        public string WorkPermitNumber { get; set; }
        public string VisaNumber { get; set; }
        public DateTime? VisaDateOfIssue { get; set; }
        public DateTime? VisaExpirationDate { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public DateTime? SocialInsuranceDateOfIssue { get; set; }
        public DateTime? SocialInsuranceDateOfParticipation { get; set; }
        public string HealthInsuranceNumber { get; set; }
        public DateTime? HealthInsuranceDateOfIssue { get; set; }
        public DateTime? HealthInsuranceDateOfParticipation { get; set; }
        public TimeSpan? WorkHourOfStart { get; set; }
        public TimeSpan? WorkHourOfEnd { get; set; }
        public Guid? TypePaid { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public Guid? CustomerPosition { get; set; }
        public bool? Active { get; set; }
        public string Other { get; set; }
        public string PotentialCustomerPosition { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public Guid? AreaId { get; set; }
        public Guid? GeographicalAreaId { get; set; }
        public string CustomerName { get; set; }
        public string LinkFace { get; set; }
        public string EvaluateContactPeople { get; set; }
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public Guid? BankId { get; set; }
        public string BranchName { get; set; }
        public Guid? DefaultAccount { get; set; }
        public decimal? MoneyLimit { get; set; }
        public string OptionPosition { get; set; }
        public string RelationShip { get; set; }
        public Guid? TermsPayment { get; set; }
        public string BankAddress { get; set; }
        public string BankOwnerName { get; set; }
        public string BankAccount { get; set; }


        public Country Country { get; set; }
        public District District { get; set; }
        public DataAccess.Databases.Entities.Category MaritalStatus { get; set; }
        public Province Province { get; set; }
        public Ward Ward { get; set; }
        public Databases.Entities.GeographicalArea Area { get; set; }
        public string StatusCustomer { get; set; }  //HDO: khách hàng, MOI: khách hàng tiềm năng
        public Guid? QuanHeId { get; set; }
        public string TenQuanHe { get; set; }
        public bool? PhuThuoc { get; set; }
        public DateTime? PhuThuocTuNgay { get; set; }
        public DateTime? PhuThuocDenNgay { get; set; }

        public ContactEntityModel()
        {
        }

        public ContactEntityModel(DataAccess.Databases.Entities.Contact entity)
        {
            Mapper(entity, this);
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.Contact ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Contact();
            Mapper(this, entity);
            return entity;
        }
    }
}
