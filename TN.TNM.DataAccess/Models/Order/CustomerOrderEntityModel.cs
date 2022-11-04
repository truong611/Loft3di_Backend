﻿using System;

namespace TN.TNM.DataAccess.Models.Order
{
    public class CustomerOrderEntityModel : BaseModel<Databases.Entities.CustomerOrder>
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid? Seller { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerContactId { get; set; }
        public Guid? PaymentMethod { get; set; }
        public bool? DiscountType { get; set; }
        public Guid? BankAccountId { get; set; }
        public int? DaysAreOwed { get; set; }
        public decimal? MaxDebt { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public TimeSpan? ReceivedHour { get; set; }
        public string RecipientName { get; set; }
        public string LocationOfShipment { get; set; }
        public string ShippingNote { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientEmail { get; set; }
        public string PlaceOfDelivery { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? ReceiptInvoiceAmount { get; set; }
        public Guid? StatusId { get; set; }
        public string StatusCode { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool? CanDelete { get; set; }

        public Guid? SellerContactId { get; set; }
        public string SellerName { get; set; }
        public string SellerFirstName { get; set; }
        public string SellerLastName { get; set; }
        public string SellerAvatarUrl { get; set; }
        public string OrderStatusName { get; set; }
        public string CustomerName { get; set; }
        public int? TypeAccount { get; set; }
        public string ListOrderDetail { get; set; }
        public string ReasonCancel { get; set; }
        public bool? IsAutoGenReceiveInfor { get; set; }
        public string CustomerAddress { get; set; }
        public Guid? OrderContractId { get; set; }
        public Guid? WarehouseId { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? PersonInChargeIdOfCus { get; set; }
        public CustomerOrderEntityModel(Databases.Entities.CustomerOrder entity)
        {
            Mapper(entity, this);
        }
        public CustomerOrderEntityModel()
        {
            
        }
        public override Databases.Entities.CustomerOrder ToEntity()
        {
            var entity = new Databases.Entities.CustomerOrder();
            Mapper(this, entity);
            return entity;
        }
    }
}
