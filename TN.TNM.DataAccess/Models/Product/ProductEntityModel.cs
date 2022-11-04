﻿using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Product
{
    public class ProductEntityModel : BaseModel<Databases.Entities.Product>
    {
        public Guid ProductId { get; set; }
        public Guid ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal? Price1 { get; set; }
        public decimal? Price2 { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public decimal? Quantity { get; set; }
        public Guid? ProductUnitId { get; set; }
        public string ProductUnitName { get; set; }
        public string ProductDescription { get; set; }
        public decimal? Vat { get; set; }
        public decimal? MinimumInventoryQuantity { get; set; }
        public Guid? ProductMoneyUnitId { get; set; }
        public string ProductCategoryName { get; set; }
        public string ListVendorName { get; set; }
        public int? Guarantee { get; set; }
        public int? GuaranteeTime { get; set; }
        public int CountProductInformation { get; set; }
        public decimal? ExWarehousePrice { get; set; }
        public Guid? CalculateInventoryPricesId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? WarehouseAccountId { get; set; }
        public Guid? RevenueAccountId { get; set; }
        public Guid? PayableAccountId { get; set; }
        public decimal? ImportTax { get; set; }
        public Guid? CostPriceAccountId { get; set; }
        public Guid? AccountReturnsId { get; set; }
        public bool? FolowInventory { get; set; }
        public bool? ManagerSerialNumber { get; set; }
        public string ProductCodeName { get; set; }
        public string PropertyName { get; set; }
        public string CalculateInventoryPricesName { get; set; }
        public decimal? FixedPrice { get; set; }

        public Guid? LoaiKinhDoanh { get; set; }
        public string LoaiKinhDoanhCode { get; set; }


        public List<Models.ProductAttributeCategory.ProductAttributeCategoryEntityModel> ListProductAttributeCategory { get; set; }

        public ProductEntityModel()
        {
            ListProductAttributeCategory = new List<ProductAttributeCategory.ProductAttributeCategoryEntityModel>();
        }
        public ProductEntityModel(Databases.Entities.Product entity)
        {
            Mapper(entity, this);
        }

        public override Databases.Entities.Product ToEntity()
        {
            var entity = new Databases.Entities.Product();
            Mapper(this, entity);
            return entity;
        }
    }
}
