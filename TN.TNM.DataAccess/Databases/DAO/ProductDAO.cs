using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Product;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;
using System.Net;
using TN.TNM.DataAccess.Models.ProductAttributeCategory;
using TN.TNM.DataAccess.Models.ProductAttributeCategoryValue;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ProductDAO : BaseDAO, IProductDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }
        public SearchProductResult SearchProduct(SearchProductParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.PRODUCT, "Search product", parameter.UserId);
                var commonProductCategory = context.ProductCategory.ToList();
                var commonProduct = context.Product.ToList();
                var commonProductVendorMapping = context.ProductVendorMapping.ToList();
                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();

                var productUnitTypeId = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DNH")?.CategoryTypeId;
                var listAllProductUnit = commonCategory.Where(c => c.CategoryTypeId == productUnitTypeId).ToList() ?? new List<Category>();

                var propertyTypeId = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TC")?.CategoryTypeId;
                var listProperty = commonCategory.Where(c => c.CategoryTypeId == propertyTypeId).ToList() ?? new List<Category>();

                var caculatorInventoryPriceTypeId = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "GTK")?.CategoryTypeId;
                var listCaculatorInventoryPrice = commonCategory.Where(c => c.CategoryTypeId == caculatorInventoryPriceTypeId).ToList() ?? new List<Category>();

                #region Kiểm tra các references của Product
                var vendorOrderDetails = context.VendorOrderDetail.ToList();
                var customerOrderDetails = context.CustomerOrderDetail.ToList();
                var quoteDetails = context.QuoteDetail.ToList();
                var procurementRequestItems = context.ProcurementRequestItem.ToList();

                #endregion

                if (parameter.ListProductCategory.Count > 0)
                {
                    List<Guid> listGuidTemp = parameter.ListProductCategory;
                    for (int i = 0; i < listGuidTemp.Count; ++i)
                    {
                        ListChildProductCategory(listGuidTemp[i], parameter.ListProductCategory, commonProductCategory);
                    }
                }
                var listVendorMappingSearch = context.ProductVendorMapping.Where(c => parameter.ListVendor.Count == 0 || parameter.ListVendor == null ||
                                                    parameter.ListVendor.Contains(c.VendorId)).Select(c => c.ProductId).ToList();


                var productList = commonProduct.Where(c => c.Active == true && (parameter.ListVendor == null || parameter.ListVendor.Count == 0 || listVendorMappingSearch.Contains(c.ProductId)) &&
                                 (parameter.ProductName == null || parameter.ProductName == "" || c.ProductName.ToLower().Contains(parameter.ProductName.ToLower().Trim())) &&
                                 (parameter.ProductCode == null || parameter.ProductCode == "" || c.ProductCode.ToLower().Contains(parameter.ProductCode.ToLower().Trim())) &&
                                 (parameter.ListProductCategory.Contains(c.ProductCategoryId) || parameter.ListProductCategory.Count == 0))
                                  .Select(m => new ProductEntityModel
                                  {
                                      ProductId = m.ProductId,
                                      ProductCategoryId = m.ProductCategoryId,
                                      ProductName = m.ProductName,
                                      ProductCode = m.ProductCode,
                                      ProductDescription = m.ProductDescription,
                                      ProductUnitId = m.ProductUnitId,
                                      Quantity = m.Quantity,
                                      Price1 = m.Price1,
                                      Price2 = m.Price2,
                                      Active = m.Active,
                                      CreatedById = m.CreatedById,
                                      CreatedDate = m.CreatedDate,
                                      UpdatedById = m.UpdatedById,
                                      UpdatedDate = m.UpdatedDate,
                                      LoaiKinhDoanh = m.LoaiKinhDoanh,
                                      ProductCategoryName = commonProductCategory.FirstOrDefault(c => c.ProductCategoryId == m.ProductCategoryId)?.ProductCategoryName ?? "",
                                      MinimumInventoryQuantity = m.MinimumInventoryQuantity,
                                      GuaranteeTime = m.GuaranteeTime,
                                      ProductUnitName = listAllProductUnit.FirstOrDefault(c => c.CategoryId == m.ProductUnitId)?.CategoryName ?? "",
                                      PropertyName = listProperty.FirstOrDefault(c => c.CategoryId == m.PropertyId)?.CategoryName ?? "",
                                      CalculateInventoryPricesName = listCaculatorInventoryPrice.FirstOrDefault(c => c.CategoryId == m.CalculateInventoryPricesId)?.CategoryName ?? "",

                                      CountProductInformation = CountProductInformation(
                                                                       m.ProductId,
                                                                       vendorOrderDetails,
                                                                       customerOrderDetails,
                                                                       quoteDetails,
                                                                       procurementRequestItems),
                                      //ListVendorName= getListNameVendor(m.ProductId),

                                  }).ToList();
                var resultGroup = productList.GroupBy(x => x.ProductId).Select(y => y.First()).ToList();
                resultGroup.ForEach(item =>
                {
                    item.ListVendorName = getListNameVendor(item.ProductId);
                });

                if (parameter.ListKieuHinhKinhDoanh.Count > 0)
                {
                    resultGroup = resultGroup.Where(x => parameter.ListKieuHinhKinhDoanh.Contains(x.LoaiKinhDoanh)).ToList();
                }


                //sort by created date desc
                resultGroup = resultGroup.OrderByDescending(w => w.CreatedDate).ToList();
                return new SearchProductResult
                {
                    StatusCode = HttpStatusCode.OK,
                    ProductList = resultGroup
                };
            }
            catch (Exception ex)
            {
                return new SearchProductResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }
        public void ListChildProductCategory(Guid ProductCategoryID, List<Guid> listResult, List<ProductCategory> commonProductCategory)
        {
            var listProductCategoryChil = commonProductCategory.Where(item => item.ParentId == ProductCategoryID).ToList();
            if (listProductCategoryChil.Count > 0)
            {
                for (int i = 0; i < listProductCategoryChil.Count; ++i)
                {
                    listResult.Add(listProductCategoryChil[i].ProductCategoryId);
                    ListChildProductCategory(listProductCategoryChil[i].ProductCategoryId, listResult, commonProductCategory);
                }
            }
        }
        public CreateProductResult CreateProduct(CreateProductParameter parameter)
        {
            try
            {
                //Check trùng mã sản phẩm
                var existsCode = context.Product.FirstOrDefault(x =>
                    x.Active == true && x.ProductCode == parameter.Product.ProductCode.Trim());

                if (existsCode != null)
                {
                    return new CreateProductResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã sản phẩm đã tồn tại"
                    };
                }

                #region Add Product
                var productId = Guid.NewGuid();
                var newProduct = new Databases.Entities.Product
                {
                    ProductId = productId,
                    ProductCategoryId = parameter.Product.ProductCategoryId,
                    ProductName = parameter.Product.ProductName.Trim(),
                    ProductCode = parameter.Product.ProductCode.Trim(),
                    Price1 = parameter.Product.Price1,
                    CreatedDate = DateTime.Now,
                    ProductUnitId = parameter.Product.ProductUnitId,
                    ProductDescription = parameter.Product.ProductDescription?.Trim(),
                    Vat = parameter.Product.Vat,
                    ProductMoneyUnitId = parameter.Product.ProductMoneyUnitId,
                    GuaranteeTime = parameter.Product.GuaranteeTime,
                    ExWarehousePrice = parameter.Product.ExWarehousePrice,
                    CreatedById = parameter.UserId,
                    //default values
                    UpdatedById = null,
                    Price2 = 0,
                    UpdatedDate = null,
                    Active = true,
                    Quantity = 0,
                    Guarantee = null,
                    GuaranteeDatetime = null,
                    MinimumInventoryQuantity = 0, //trường số lượng tồn kho tối thiểu chuyển qua dùng ở bảng InventoryReport, trường QuantityMinimun
                    CalculateInventoryPricesId = parameter.Product.CalculateInventoryPricesId,
                    PropertyId = parameter.Product.PropertyId,
                    WarehouseAccountId = parameter.Product.WarehouseAccountId,
                    RevenueAccountId = parameter.Product.RevenueAccountId,
                    PayableAccountId = parameter.Product.PayableAccountId,
                    ImportTax = parameter.Product.ImportTax,
                    CostPriceAccountId = parameter.Product.CostPriceAccountId,
                    AccountReturnsId = parameter.Product.AccountReturnsId,
                    FolowInventory = parameter.Product.FolowInventory,
                    ManagerSerialNumber = parameter.Product.ManagerSerialNumber,
                    LoaiKinhDoanh = parameter.Product.LoaiKinhDoanh,
                };
                context.Product.Add(newProduct);

                var productResponse = new DataAccess.Models.Product.ProductEntityModel()
                {
                    ProductId = newProduct.ProductId,
                    ProductName = newProduct.ProductName,
                    ProductCode = newProduct.ProductCode,

                };

                #endregion

                #region Add Mapping Product and Vendors
                if (parameter.ListProductVendorMapping.Count > 0)
                {
                    var listProductVendorMapping = new List<ProductVendorMapping>();
                    parameter.ListProductVendorMapping.ForEach(vendor =>
                    {
                        var productVendorObj = new ProductVendorMapping
                        {
                            ProductVendorMappingId = Guid.NewGuid(),
                            ProductId = productId,
                            VendorId = vendor.VendorId,
                            VendorProductName = vendor.VendorProductName,
                            MiniumQuantity = vendor.MiniumQuantity,
                            UnitPriceId = vendor.MoneyUnitId,
                            Price = vendor.Price,
                            FromDate = vendor.FromDate,
                            ToDate = vendor.ToDate,
                            OrderNumber = vendor.OrderNumber,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null,
                            Active = true
                        };
                        listProductVendorMapping.Add(productVendorObj);
                    });
                    context.ProductVendorMapping.AddRange(listProductVendorMapping);
                }
                #endregion

                #region Add Product Attribute             
                if (parameter.ListProductAttributeCategory.Count > 0)
                {
                    var listAttributeCategory = new List<Databases.Entities.ProductAttributeCategory>();
                    var listProductAttributeCategoryValue = new List<Databases.Entities.ProductAttributeCategoryValue>();
                    var listProductAttribute = new List<Databases.Entities.ProductAttribute>();
                    parameter.ListProductAttributeCategory.ForEach(attribute =>
                    {
                        //định nghĩa product attribute category
                        var newAttributeCategoryId = Guid.NewGuid();
                        var attributeCategoryObj = new Databases.Entities.ProductAttributeCategory
                        {
                            ProductAttributeCategoryId = newAttributeCategoryId,
                            ProductAttributeCategoryName = attribute.ProductAttributeCategoryName?.Trim(),
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null,
                            Active = true
                        };
                        listAttributeCategory.Add(attributeCategoryObj);
                        //gắn category với sản phẩm
                        var productAttribute = new Databases.Entities.ProductAttribute
                        {
                            ProductAttributeId = Guid.NewGuid(),
                            ProductId = productId,
                            ProductAttributeCategoryId = newAttributeCategoryId,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = null,
                            Active = true,
                            UpdatedBy = null,
                            CreatedBy = parameter.UserId
                        };
                        listProductAttribute.Add(productAttribute);
                        //định nghĩa product attribute value
                        if (attribute.ProductAttributeCategoryValue.Count > 0)
                        {
                            attribute.ProductAttributeCategoryValue.ForEach(attriButeValue =>
                            {
                                var attributeValue = new Databases.Entities.ProductAttributeCategoryValue
                                {
                                    ProductAttributeCategoryValueId = Guid.NewGuid(),
                                    ProductAttributeCategoryValue1 = attriButeValue.ProductAttributeCategoryValue1?.Trim(),
                                    ProductAttributeCategoryId = newAttributeCategoryId,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = null,
                                    UpdatedById = null,
                                    Active = true
                                };
                                listProductAttributeCategoryValue.Add(attributeValue);
                            });
                        }
                    });
                    context.ProductAttributeCategory.AddRange(listAttributeCategory);
                    context.ProductAttributeCategoryValue.AddRange(listProductAttributeCategoryValue);
                    context.ProductAttribute.AddRange(listProductAttribute);
                }
                #endregion

                #region Add Product BOM
                var listProductBOM = new List<ProductBillOfMaterials>();

                parameter.ListProductBillOfMaterials?.ForEach(bom =>
                {
                    listProductBOM.Add(new ProductBillOfMaterials()
                    {
                        ProductBillOfMaterialId = Guid.NewGuid(),
                        ProductId = newProduct.ProductId, //lấy theo id sản phẩm vừa tạo
                        ProductMaterialId = bom.ProductMaterialId,
                        Quantity = bom.Quantity,
                        EffectiveFromDate = bom.EffectiveFromDate,
                        EffectiveToDate = bom.EffectiveToDate,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now
                    });
                });

                context.ProductBillOfMaterials.AddRange(listProductBOM);
                #endregion

                #region Thêm vào bảng Báo cáo tồn kho

                var SERIAL_STATUS_CODE = "TSE";
                var serialStatusId = context.CategoryType
                    .FirstOrDefault(w => w.CategoryTypeCode == SERIAL_STATUS_CODE)?.CategoryTypeId;
                var NEW_SERIAL_STATUS_CODE = "CXU"; //Mặc định trạng thái mới của serial: Chưa xuất;
                var statusId = context.Category.FirstOrDefault(w =>
                        w.CategoryTypeId == serialStatusId && w.CategoryCode == NEW_SERIAL_STATUS_CODE)?
                    .CategoryId;

                if (parameter.ListInventoryReport.Count > 0)
                {
                    var listInventoryReport = new List<InventoryReport>();
                    var listSerial = new List<Serial>();
                    parameter.ListInventoryReport.ForEach(item =>
                    {
                        var newInventoryReportObj = new InventoryReport
                        {
                            InventoryReportId = Guid.NewGuid(),
                            WarehouseId = item.WarehouseId,
                            ProductId = productId,
                            Quantity = 0, //mặc định 0
                            QuantityMinimum = item.QuantityMinimum,
                            StartQuantity = item.StartQuantity ?? 0,
                            QuantityMaximum = item.QuantityMaximum,
                            OpeningBalance = item.OpeningBalance ?? 0,
                            Note = item.Note,
                            Active = true,
                            CreatedDate = DateTime.Now,
                            CreatedById = parameter.UserId
                        };
                        listInventoryReport.Add(newInventoryReportObj);

                        #region Add Serial 

                        if (item.ListSerial.Count > 0)
                        {
                            item.ListSerial.ForEach(serial =>
                            {
                                var newSerial = new Serial
                                {
                                    SerialId = Guid.NewGuid(),
                                    SerialCode = serial.SerialCode?.Trim(),
                                    ProductId = productId,
                                    StatusId = statusId.Value,
                                    WarehouseId = item.WarehouseId,
                                    CreatedDate = DateTime.Now,
                                    Active = true,
                                    CreatedById = parameter.UserId,
                                    UpdatedDate = null,
                                    UpdatedById = null,
                                };
                                listSerial.Add(newSerial);
                            });
                        }

                        #endregion
                    });

                    context.InventoryReport.AddRange(listInventoryReport);
                    context.Serial.AddRange(listSerial);
                }

                #endregion

                #region Add Product Image

                if (parameter.ListProductImage.Count > 0)
                {
                    var listProductImage = new List<ProductImage>();
                    string folderName = "ProductImage";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    parameter.ListProductImage.ForEach(image =>
                    {
                        listProductImage.Add(new ProductImage
                        {
                            ProductImageId = Guid.NewGuid(),
                            ProductId = productId,
                            ImageName = image.ImageName.Trim(),
                            ImageSize = image.ImageSize,
                            ImageUrl = Path.Combine(newPath, image.ImageName),
                            //default values 
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                        });
                    });
                    context.ProductImage.AddRange(listProductImage);
                }

                #endregion

                context.SaveChanges();

                #region Lưu nhật ký hệ thống

                LogHelper.AuditTrace(context, ActionName.Create, ObjectName.PRODUCT, productId, parameter.UserId);

                #endregion

                return new CreateProductResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Tạo sản phẩm/dịch vụ thành công",
                    ProductId = productId,
                    NewProduct = productResponse
                };
            }
            catch (Exception ex)
            {
                return new CreateProductResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public ImportProductResult ImportProduct(ImportProductParameter parameter)
        {
            var products = new List<Product>();
            parameter.ListProduct.ForEach(item =>
            {
                var newProduct = new Databases.Entities.Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductCategoryId = item.ProductCategoryId,
                    ProductName = item.ProductName.Trim(),
                    ProductCode = item.ProductCode.Trim(),
                    Price1 = item.Price1,
                    CreatedDate = DateTime.Now,
                    ProductUnitId = item.ProductUnitId,
                    ProductDescription = item.ProductDescription?.Trim(),
                    Vat = item.Vat,
                    ProductMoneyUnitId = item.ProductMoneyUnitId,
                    GuaranteeTime = item.GuaranteeTime,
                    ExWarehousePrice = item.ExWarehousePrice,
                    CreatedById = parameter.UserId,
                    //default values
                    UpdatedById = null,
                    Price2 = 0,
                    UpdatedDate = null,
                    Active = true,
                    Quantity = 0,
                    Guarantee = null,
                    GuaranteeDatetime = null,
                    MinimumInventoryQuantity = 0, //trường số lượng tồn kho tối thiểu chuyển qua dùng ở bảng InventoryReport, trường QuantityMinimun
                    CalculateInventoryPricesId = item.CalculateInventoryPricesId,
                    PropertyId = item.PropertyId,
                    WarehouseAccountId = item.WarehouseAccountId,
                    RevenueAccountId = item.RevenueAccountId,
                    PayableAccountId = item.PayableAccountId,
                    ImportTax = item.ImportTax,
                    CostPriceAccountId = item.CostPriceAccountId,
                    AccountReturnsId = item.AccountReturnsId,
                    FolowInventory = item.FolowInventory,
                    ManagerSerialNumber = item.ManagerSerialNumber,
                };
                products.Add(newProduct);
            });
            context.Product.AddRange(products);
            context.SaveChanges();

            return new ImportProductResult
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = CommonMessage.Product.IMPORT_SUCCESS,
            };
        }

        public GetProductByIDResult GetProductByID(GetProductByIDParameter parameter)
        {
            try
            {
                #region Add by Dung

                var productResponse = new ProductEntityModel();
                var Product = context.Product.Where(m => m.ProductId == parameter.ProductId).FirstOrDefault();
                if (Product == null)
                {
                    return new GetProductByIDResult
                    {
                        MessageCode = "Sản phẩm không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                if (Product != null)
                {
                    productResponse.ProductId = Product.ProductId;
                    productResponse.ProductCategoryId = Product.ProductCategoryId;
                    productResponse.ProductName = Product.ProductName;
                    productResponse.ProductCode = Product.ProductCode;
                    productResponse.Price1 = Product.Price1;
                    productResponse.Price2 = Product.Price2;
                    productResponse.ProductDescription = Product.ProductDescription;
                    productResponse.Vat = Product.Vat;
                    productResponse.ProductMoneyUnitId = Product.ProductMoneyUnitId;
                    productResponse.ProductUnitId = Product.ProductUnitId;
                    productResponse.GuaranteeTime = Product.GuaranteeTime;
                    productResponse.ExWarehousePrice = Product.ExWarehousePrice ?? 0;
                    productResponse.CalculateInventoryPricesId = Product.CalculateInventoryPricesId;
                    productResponse.PropertyId = Product.PropertyId;
                    productResponse.WarehouseAccountId = Product.WarehouseAccountId;
                    productResponse.RevenueAccountId = Product.RevenueAccountId;
                    productResponse.PayableAccountId = Product.PayableAccountId;
                    productResponse.ImportTax = Product.ImportTax;
                    productResponse.CostPriceAccountId = Product.CostPriceAccountId;
                    productResponse.AccountReturnsId = Product.AccountReturnsId;
                    productResponse.FolowInventory = Product.FolowInventory;
                    productResponse.ManagerSerialNumber = Product.ManagerSerialNumber;
                    productResponse.LoaiKinhDoanh = Product.LoaiKinhDoanh;
                }

                var listVendorMappingResult = context.ProductVendorMapping
                    .Where(c => c.ProductId == parameter.ProductId).OrderBy(x => x.OrderNumber).ToList();
                //chuyển sang entitymodel
                var listVendorMapping = new List<ProductVendorMappingEntityModel>();
                listVendorMappingResult.ForEach(item =>
                {
                    listVendorMapping.Add(new ProductVendorMappingEntityModel(item));
                });

                var listProductAttributeCategory = new List<ProductAttributeCategoryEntityModel>();
                //get mapping product with attribute
                var listAttributeId = context.ProductAttribute.Where(w => w.ProductId == parameter.ProductId)
                    .Select(w => w.ProductAttributeCategoryId).ToList();

                if (listAttributeId.Count > 0)
                {
                    //get name of attribute 
                    var listProductAttributeCategoryEntity = context.ProductAttributeCategory
                        .Where(w => listAttributeId.Contains(w.ProductAttributeCategoryId)).ToList();
                    var listAttributeCategoryIdEntity = listProductAttributeCategoryEntity
                        .Select(w => w.ProductAttributeCategoryId).ToList();
                    var listAttributeValueEntity = context.ProductAttributeCategoryValue
                        .Where(w => listAttributeCategoryIdEntity.Contains(w.ProductAttributeCategoryId)).ToList();

                    //get value of attribute
                    listProductAttributeCategoryEntity.ForEach(productAttributeCategory =>
                    {
                        var newListAttribute = new List<ProductAttributeCategoryValueEntityModel>();
                        var listAttributeValueByCategory = listAttributeValueEntity.Where(w =>
                                w.ProductAttributeCategoryId == productAttributeCategory.ProductAttributeCategoryId)
                            .ToList();
                        listAttributeValueByCategory.ForEach(attValue =>
                        {
                            newListAttribute.Add(new ProductAttributeCategoryValueEntityModel
                            {
                                ProductAttributeCategoryValueId = attValue.ProductAttributeCategoryValueId,
                                ProductAttributeCategoryValue1 = attValue.ProductAttributeCategoryValue1,
                                ProductAttributeCategoryId = attValue.ProductAttributeCategoryId
                            });
                        });

                        listProductAttributeCategory.Add(new ProductAttributeCategoryEntityModel
                        {
                            ProductAttributeCategoryId = productAttributeCategory.ProductAttributeCategoryId,
                            ProductAttributeCategoryName = productAttributeCategory.ProductAttributeCategoryName,
                            ProductAttributeCategoryValue = newListAttribute
                        });
                    });
                }

                #endregion

                #region Get Customer Order
                var listCustomerOrder = new List<CustomerOrderEntityModel>();
                var customerOrderDetail = context.CustomerOrderDetail.Where(w => w.ProductId == parameter.ProductId).ToList();
                if (customerOrderDetail != null)
                {
                    //get list order by order detail
                    var listOrderDetailId = customerOrderDetail.Select(w => w.OrderId).ToList();
                    var listOrder = context.CustomerOrder.Where(w => listOrderDetailId.Contains(w.OrderId)).ToList();
                    var listCustomerOrderId = listOrder.Select(w => w.CustomerId).ToList();
                    var listCustomer = context.Customer.Where(w => listCustomerOrderId.Contains(w.CustomerId)).ToList();

                    customerOrderDetail.ForEach(orderdetail =>
                    {
                        var order = listOrder.Where(w => w.OrderId == orderdetail.OrderId).FirstOrDefault();
                        var customer = listCustomer.Where(w => w.CustomerId == order.CustomerId).FirstOrDefault();

                        listCustomerOrder.Add(new CustomerOrderEntityModel
                        {
                            OrderId = orderdetail.OrderId,
                            CustomerName = customer.CustomerName?.Trim(),
                            CustomerId = customer.CustomerId,
                            OrderCode = order.OrderCode?.Trim(),
                            OrderDate = order.OrderDate,
                            //default values
                            Description = null,
                            Note = null,
                            PaymentMethod = Guid.Empty,
                            DaysAreOwed = null,
                            MaxDebt = null,
                            ReceivedDate = null,
                            ReceivedHour = null,
                            RecipientName = null,
                            LocationOfShipment = null,
                            ShippingNote = null,
                            RecipientPhone = null,
                            RecipientEmail = null,
                            PlaceOfDelivery = null,
                            Amount = 0,
                            DiscountValue = null,
                            StatusId = null,
                            CreatedById = Guid.Empty,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null,
                            Active = null
                        });
                    });
                }
                #endregion

                #region Get Product Image
                var ListProductImage = new List<ProductImageEntityModel>();

                var productImageEntity = context.ProductImage.Where(w => w.Active == true && w.ProductId == parameter.ProductId).ToList();
                if (productImageEntity != null)
                {
                    productImageEntity.ForEach(option =>
                    {
                        ListProductImage.Add(new ProductImageEntityModel
                        {
                            ProductImageId = option.ProductImageId,
                            ProductId = option.ProductId,
                            ImageName = option.ImageName,
                            ImageSize = option.ImageSize,
                            ImageUrl = option.ImageUrl,
                        });
                    });
                }

                #endregion

                #region Lấy tồn kho đầu kỳ và tồn kho tối thiểu

                var listInventory = new List<InventoryReportByProductIdEntityModel>();

                var inventoryByProductId =
                    context.InventoryReport.Where(w => w.ProductId == parameter.ProductId).ToList();

                inventoryByProductId?.ForEach(inventory =>
                {
                    listInventory.Add(new InventoryReportByProductIdEntityModel
                    {
                        InventoryReportId = inventory.InventoryReportId,
                        WarehouseId = inventory.WarehouseId,
                        ProductId = inventory.ProductId,
                        Quantity = inventory.Quantity,
                        QuantityMinimum = inventory.QuantityMinimum,
                        QuantityMaximum = inventory.QuantityMaximum,
                        StartQuantity = inventory.StartQuantity,
                        OpeningBalance = inventory.OpeningBalance,
                        Note = inventory.Note,
                        ListSerial = new List<SerialEntityModel>()
                    });
                });

                if (listInventory.Count > 0)
                {
                    var listAllSerial = context.Serial.ToList();

                    listInventory.ForEach(item =>
                    {
                        var listSerial = listAllSerial
                            .Where(x => x.WarehouseId == item.WarehouseId && x.ProductId == item.ProductId).Select(y =>
                                new SerialEntityModel
                                {
                                    SerialId = y.SerialId,
                                    SerialCode = y.SerialCode,
                                    ProductId = y.ProductId,
                                    WarehouseId = y.WarehouseId,
                                    StatusId = y.StatusId,
                                    CreatedDate = y.CreatedDate
                                }).ToList();

                        item.ListSerial = listSerial;
                    });
                }

                #endregion

                #region Lấy Product BOM
                var productBOMEntity = context.ProductBillOfMaterials.Where(w => w.Active == true && w.ProductId == parameter.ProductId).ToList();
                var listProductEntity = context.Product.ToList();
                var unitTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH").CategoryTypeId;
                var listProductUnit = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitTypeId).ToList();

                var listProductBOM = new List<DataAccess.Models.Product.ProductBillOfMaterialsEntityModel>();
                productBOMEntity?.ForEach(bom =>
                {
                    var productMaterial = listProductEntity.FirstOrDefault(f => f.ProductId == bom.ProductMaterialId);
                    var productName = productMaterial?.ProductName ?? "";
                    var productCode = productMaterial?.ProductCode ?? "";
                    var productUnitName = listProductUnit.FirstOrDefault(f => productMaterial.ProductUnitId == f.CategoryId)?.CategoryName ?? "";

                    listProductBOM.Add(new ProductBillOfMaterialsEntityModel()
                    {
                        ProductBillOfMaterialId = bom.ProductBillOfMaterialId,
                        ProductId = bom.ProductId,
                        ProductMaterialId = bom.ProductMaterialId,
                        Quantity = bom.Quantity,
                        EffectiveFromDate = bom.EffectiveFromDate,
                        EffectiveToDate = bom.EffectiveToDate,
                        ProductName = productName,
                        ProductCode = productCode,
                        ProductUnitName = productUnitName
                    });
                });
                #endregion

                #region Kiểm tra điều kiện xóa sản phẩm
                var CanDelete = true;
                var checkVendorOrderDetail = context.VendorOrderDetail.FirstOrDefault(f => f.ProductId == parameter.ProductId);
                var checkCustomerOrderDetail = context.CustomerOrderDetail.FirstOrDefault(f => f.ProductId == parameter.ProductId);
                var checkQuoteDetail = context.QuoteDetail.FirstOrDefault(f => f.ProductId == parameter.ProductId);
                var checkProcurementRequestItem = context.ProcurementRequestItem.FirstOrDefault(f => f.ProductId == parameter.ProductId);

                if (checkVendorOrderDetail != null || checkCustomerOrderDetail != null || checkQuoteDetail != null || checkProcurementRequestItem != null)
                {
                    CanDelete = false;
                }
                #endregion

                return new GetProductByIDResult
                {
                    Product = productResponse,
                    lstProductAttributeCategory = listProductAttributeCategory,
                    LstProductVendorMapping = listVendorMapping,
                    lstCustomerOrder = listCustomerOrder.OrderByDescending(w => w.OrderDate).ToList(),
                    ListProductImage = ListProductImage,
                    ListInventory = listInventory,
                    StatusCode = HttpStatusCode.OK,
                    CanDelete = CanDelete,
                    ListProductBillOfMaterials = listProductBOM
                    //CountProductInformation = countProductInformation
                };
            }
            catch (Exception ex)
            {
                return new GetProductByIDResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString(),
                };

            }
        }

        public UpdateProductResult UpdateProduct(UpdateProductParameter parameter)
        {
            try
            {
                #region Update Product
                var product = context.Product.Where(w => w.ProductId == parameter.Product.ProductId).FirstOrDefault();
                if (product != null)
                {
                    product.ProductCategoryId = parameter.Product.ProductCategoryId;
                    product.ProductCode = parameter.Product.ProductCode.Trim();
                    product.ProductName = parameter.Product.ProductName.Trim();
                    product.Price1 = parameter.Product.Price1;
                    product.ExWarehousePrice = parameter.Product.ExWarehousePrice;
                    product.ProductUnitId = parameter.Product.ProductUnitId;
                    product.ProductMoneyUnitId = parameter.Product.ProductMoneyUnitId;
                    product.Vat = parameter.Product.Vat;
                    product.GuaranteeTime = parameter.Product.GuaranteeTime;
                    product.ProductDescription = parameter.Product.ProductDescription?.Trim();
                    product.UpdatedById = parameter.UserId;
                    product.UpdatedDate = DateTime.Now;
                    product.MinimumInventoryQuantity = 0; //trường số lượng tồn kho tối thiểu chuyển qua dùng ở bảng InventoryReport, trường QuantityMinimun
                    product.CalculateInventoryPricesId = parameter.Product.CalculateInventoryPricesId;
                    product.PropertyId = parameter.Product.PropertyId;
                    product.WarehouseAccountId = parameter.Product.WarehouseAccountId;
                    product.RevenueAccountId = parameter.Product.RevenueAccountId;
                    product.PayableAccountId = parameter.Product.PayableAccountId;
                    product.ImportTax = parameter.Product.ImportTax;
                    product.CostPriceAccountId = parameter.Product.CostPriceAccountId;
                    product.AccountReturnsId = parameter.Product.AccountReturnsId;
                    product.FolowInventory = parameter.Product.FolowInventory;
                    product.ManagerSerialNumber = parameter.Product.ManagerSerialNumber;
                    product.LoaiKinhDoanh = parameter.Product.LoaiKinhDoanh;

                    context.Product.Update(product);
                }
                #endregion

                #region Update Product Mapping Vendor
                //delete old records
                var oldList = context.ProductVendorMapping.Where(w => w.ProductId == parameter.Product.ProductId).ToList();
                if (oldList != null)
                {
                    context.ProductVendorMapping.RemoveRange(oldList);
                }
                var newList = new List<ProductVendorMapping>();
                parameter.ListProductVendorMapping.ForEach(vendor =>
                {
                    newList.Add(new ProductVendorMapping
                    {
                        ProductVendorMappingId = Guid.NewGuid(),
                        ProductId = parameter.Product.ProductId,
                        VendorId = vendor.VendorId,
                        VendorProductName = vendor.VendorProductName,
                        MiniumQuantity = vendor.MiniumQuantity,
                        UnitPriceId = vendor.MoneyUnitId,
                        Price = vendor.Price,
                        FromDate = vendor.FromDate,
                        ToDate = vendor.ToDate,
                        OrderNumber = vendor.OrderNumber,
                        ExchangeRate = vendor.ExchangeRate,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null,
                        Active = true
                    });
                });
                context.ProductVendorMapping.AddRange(newList);
                #endregion

                #region Update Product BOM
                //delete old product BOM
                var listOldBOM = context.ProductBillOfMaterials.Where(w => w.ProductId == parameter.Product.ProductId).ToList();
                context.ProductBillOfMaterials.RemoveRange(listOldBOM);
                //add new product BOM
                var listProductBOM = new List<ProductBillOfMaterials>();
                parameter.ListProductBillOfMaterials?.ForEach(bom =>
                {
                    listProductBOM.Add(new ProductBillOfMaterials()
                    {
                        ProductBillOfMaterialId = Guid.NewGuid(),
                        ProductId = parameter.Product.ProductId, //lấy theo id sản phẩm vừa update
                        ProductMaterialId = bom.ProductMaterialId,
                        Quantity = bom.Quantity,
                        EffectiveFromDate = bom.EffectiveFromDate,
                        EffectiveToDate = bom.EffectiveToDate,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now
                    });
                });
                context.ProductBillOfMaterials.AddRange(listProductBOM);
                #endregion

                #region Add Product Image
                //remove old img
                var listOldImg = context.ProductImage.Where(x => x.ProductId == product.ProductId).ToList();
                if (listOldImg.Count > 0)
                {
                    context.ProductImage.RemoveRange(listOldImg);
                }

                // Add list new image
                if (parameter.ListProductImage.Count > 0)
                {
                    var listProductImage = new List<ProductImage>();
                    string folderName = "ProductImage";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    parameter.ListProductImage.ForEach(image =>
                    {
                        listProductImage.Add(new ProductImage
                        {
                            ProductImageId = Guid.NewGuid(),
                            ProductId = product.ProductId,
                            ImageName = image.ImageName.Trim(),
                            ImageSize = image.ImageSize,
                            ImageUrl = Path.Combine(newPath, image.ImageName),
                            //default values 
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                        });
                    });
                    context.ProductImage.AddRange(listProductImage);
                }

                #endregion

                context.SaveChanges();

                #region Lưu nhật ký hệ thống

                LogHelper.AuditTrace(context, ActionName.UPDATE, ObjectName.PRODUCT, product.ProductId, parameter.UserId);

                #endregion

                return new UpdateProductResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Chỉnh sửa sản phẩm/dịch vụ thành công",
                    ProductId = parameter.Product.ProductId
                };
            }
            catch (Exception ex)
            {
                return new UpdateProductResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public string getListNameVendor(Guid ProductId)
        {
            string Result = string.Empty;
            var listVendorId = context.ProductVendorMapping.Where(c => c.ProductId == ProductId)?.Select(c => c.VendorId).ToList() ?? new List<Guid>();
            if (listVendorId.Count != 0)
            {
                var listVendor = context.Vendor.Where(c => listVendorId.Contains(c.VendorId)).Select(c => c.VendorName).ToList();
                Result = string.Join(";", listVendor);
            }
            else
            {
                Result = "";
            }

            return Result;
        }

        public GetProductByVendorIDResult GetProductByVendorID(GetProductByVendorIDParameter parameter)
        {
            try
            {
                var listProductResult = (from product in context.Product
                                   join productvendormapping in context.ProductVendorMapping on product.ProductId equals
                                       productvendormapping.ProductId
                                   where productvendormapping.VendorId == parameter.VendorId && product.Active == true
                                   select product).ToList();
                var listProduct = new List<ProductEntityModel>();
                listProductResult.ForEach(item =>
                {
                    listProduct.Add(new ProductEntityModel(item));
                });
                return new GetProductByVendorIDResult
                {
                    lstProduct = listProduct,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetProductByVendorIDResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetProductAttributeByProductIDResult GetProductAttributeByProductID(GetProductAttributeByProductIDParameter parameter)
        {
            try
            {
                var lstProductAtribute = (from pattributecategory in context.ProductAttributeCategory
                                          join productattribute in context.ProductAttribute on pattributecategory.ProductAttributeCategoryId equals productattribute.ProductAttributeCategoryId
                                          where productattribute.ProductId == parameter.ProductId
                                          select new ProductAttributeCategoryEntityModel
                                          {
                                              Active = pattributecategory.Active,
                                              CreatedById = pattributecategory.CreatedById,
                                              CreatedDate = pattributecategory.CreatedDate,
                                              ProductAttributeCategoryId = pattributecategory.ProductAttributeCategoryId,
                                              ProductAttributeCategoryName = pattributecategory.ProductAttributeCategoryName,
                                              UpdatedById = pattributecategory.UpdatedById,
                                              UpdatedDate = pattributecategory.UpdatedDate,
                                              ProductAttributeCategoryValue = (context.ProductAttributeCategoryValue.Where(m => m.ProductAttributeCategoryId == pattributecategory.ProductAttributeCategoryId))
                                              .Select(y => new ProductAttributeCategoryValueEntityModel
                                              {
                                                  ProductAttributeCategoryValueId = y.ProductAttributeCategoryValueId,
                                                  ProductAttributeCategoryValue1 = y.ProductAttributeCategoryValue1,
                                                  ProductAttributeCategoryId = y.ProductAttributeCategoryId,
                                                  CreatedById = y.CreatedById,
                                                  CreatedDate = y.CreatedDate,
                                                  UpdatedById = y.UpdatedById,
                                                  UpdatedDate = y.UpdatedDate,
                                              }).ToList(),
                                          }).ToList();

                return new GetProductAttributeByProductIDResult
                {
                    lstProductAttributeCategory = lstProductAtribute,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetProductAttributeByProductIDResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetAllProductCodeResult GetAllProductCode(GetAllProductCodeParameter parameter)
        {
            try
            {
                var ListCode = context.Product.Select(item => new { code = item.ProductCode.ToLower() }).ToList();
                List<string> result = new List<string>();
                foreach (var item in ListCode)
                {
                    result.Add(item.code.Trim());
                }
                return new GetAllProductCodeResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListProductCode = result
                };
            }
            catch (Exception ex)
            {

                return new GetAllProductCodeResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public UpdateActiveProductResult UpdateActiveProduct(UpdateActiveProductParameter parameter)
        {
            try
            {
                var productUpdate = context.Product.FirstOrDefault(item => item.ProductId == parameter.ProductId);
                productUpdate.Active = false;
                productUpdate.UpdatedById = parameter.UserId;
                productUpdate.UpdatedDate = DateTime.Now;

                context.Product.Update(productUpdate);
                context.SaveChanges();

                #region Lưu nhật ký hệ thống

                LogHelper.AuditTrace(context, ActionName.DELETE, ObjectName.PRODUCT, productUpdate.ProductId, parameter.UserId);

                #endregion

                return new UpdateActiveProductResult
                {
                    MessageCode = CommonMessage.ProductCategory.DELETE_SUCCESS,
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {

                return new UpdateActiveProductResult
                {
                    MessageCode = CommonMessage.ProductCategory.DELETE_FAIL,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public int CountProductInformation(
            Guid productId,
            List<VendorOrderDetail> vendorOrderDetails,
            //List<VendorOrderProductDetailProductAttributeValue> vendorOrderProductDetailProductAttributeValues,
            List<CustomerOrderDetail> customerOrderDetails,
            //List<OrderProductDetailProductAttributeValue> orderProductDetailProductAttributeValues,
            List<QuoteDetail> quoteDetails,
            //List<QuoteProductDetailProductAttributeValue> quoteProductDetailProductAttributeValues,
            List<ProcurementRequestItem> procurementRequestItems)
        //List<ProductAttribute> productAttributes)
        {
            int count = vendorOrderDetails.Where(q => q.ProductId == productId).Count(); //đơn đặt hàng nhà cung cấp
            //count += vendorOrderProductDetailProductAttributeValues.Where(q => q.ProductId == productId).Count();
            count += customerOrderDetails.Where(q => q.ProductId == productId).Count(); //đơn hàng
            //count += orderProductDetailProductAttributeValues.Where(q => q.ProductId == productId).Count();
            count += quoteDetails.Where(q => q.ProductId == productId).Count();//báo giá
            //count += quoteProductDetailProductAttributeValues.Where(q => q.ProductId == productId).Count();
            count += procurementRequestItems.Where(q => q.ProductId == productId).Count(); //đề xuất mua hàng   
            //count += productAttributes.Where(q => q.ProductId == productId).Count();
            //count += productVendorMappings.Where(q => q.ProductId == productId).Count(); Comment by Dung
            //tìm theo điều kiện đơn đặt hàng nhà cung cấp
            //count += vendorOrderDetail.Where(w => w.ProductId == productId).Count();
            return count;
        }

        public GetListProductResult GetListProduct(GetListProductParameter parameter)
        {
            try
            {
                var listProductCategory = new List<ProductCategoryEntityModel>();
                var listVendor = new List<VendorEntityModel>();
                var listUnitEntity = new List<CategoryEntityModel>();
                var listPriceInventoryEntity = new List<CategoryEntityModel>();
                var listPropertyEntity = new List<CategoryEntityModel>();

                var unitCode = "DNH"; //đơn vị tính
                var propertyCode = "TC";
                var priceInvetoryCode = "GTK";

                var unitId = context.CategoryType.Where(w => w.CategoryTypeCode == unitCode).FirstOrDefault().CategoryTypeId;
                var propertyId = context.CategoryType.Where(c => c.CategoryTypeCode == propertyCode).FirstOrDefault().CategoryTypeId;
                var priceInventoryId = context.CategoryType.Where(c => c.CategoryTypeCode == priceInvetoryCode).FirstOrDefault().CategoryTypeId;

                var listUnitEntityResult = context.Category.Where(w => w.CategoryTypeId == unitId).ToList();
                listUnitEntityResult.ForEach(item =>
                {
                    listUnitEntity.Add(new CategoryEntityModel(item));
                });

                var listPropertyEntityResult = context.Category.Where(c => c.CategoryTypeId == propertyId).ToList();
                listPropertyEntityResult.ForEach(item =>
                {
                    listPropertyEntity.Add(new CategoryEntityModel(item));
                });

                var listPriceInventoryEntityResult = context.Category.Where(c => c.CategoryTypeId == priceInventoryId).ToList();
                listPriceInventoryEntityResult.ForEach(item =>
                {
                    listPriceInventoryEntity.Add(new CategoryEntityModel(item));
                });


                var listProductCategoryEntity = context.ProductCategory.Where(w => w.Active == true).ToList();
                var listVendorEntity = context.Vendor.Where(w => w.Active == true).ToList();


                listProductCategoryEntity?.ForEach(e =>
                {
                    listProductCategory.Add(new ProductCategoryEntityModel
                    {
                        ProductCategoryId = e.ProductCategoryId,
                        ProductCategoryName = e.ProductCategoryName,
                    });
                });

                listVendorEntity?.ForEach(e =>
                {
                    listVendor.Add(new VendorEntityModel
                    {
                        VendorId = e.VendorId,
                        VendorName = e.VendorName
                    });
                });

                // lấy list loại hình kinh doanh: Chỉ bán ra, chỉ mua vào và cả 2.
                var loaiHinhTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HHKD")?.CategoryTypeId;
                var listLoaiHinh = context.Category.Where(x => x.CategoryTypeId == loaiHinhTypeId).Select(c => new CategoryEntityModel()
                {
                    CategoryTypeId = c.CategoryTypeId,
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryCode = c.CategoryCode,
                }).ToList();

                return new GetListProductResult
                {
                    StatusCode = HttpStatusCode.OK,
                    ListProductCategory = listProductCategory,
                    ListVendor = listVendor,
                    ListUnit = listUnitEntity,
                    ListPriceInventory = listPriceInventoryEntity,
                    ListProperty = listPropertyEntity,
                    ListLoaiHinh = listLoaiHinh,
                };
            }
            catch (Exception ex)
            {

                return new GetListProductResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetMasterdataCreateProductResult GetMasterdataCreateProduct(GetMasterdataCreateProductParameter parameter)
        {
            try
            {
                var ListProductMoneyUnitEntityModel = new List<CategoryEntityModel>();
                var ListProductUnitEntityModel = new List<CategoryEntityModel>();
                var ListVendorEntityModel = new List<VendorEntityModel>();
                var ListWarehouseEntityModel = new List<WareHouseEntityModel>();
                var ListPropertyEntityModel = new List<CategoryEntityModel>();
                var ListPriceInventoryEntityModel = new List<CategoryEntityModel>();
                var listProductCode = new List<string>();
                var listProductUnitName = new List<string>();

                #region Get data from Database
                var moneyUnitCode = "DTI"; //đơn vị tiền
                var unitCode = "DNH"; //đơn vị tính
                var propertyCode = "TC";
                var priceInvetoryCode = "GTK";

                var moneyUnitId = context.CategoryType.Where(w => w.CategoryTypeCode == moneyUnitCode).FirstOrDefault().CategoryTypeId;
                var unitId = context.CategoryType.Where(w => w.CategoryTypeCode == unitCode).FirstOrDefault().CategoryTypeId;
                var propertyId = context.CategoryType.Where(c => c.CategoryTypeCode == propertyCode).FirstOrDefault().CategoryTypeId;
                var priceInventoryId = context.CategoryType.Where(c => c.CategoryTypeCode == priceInvetoryCode).FirstOrDefault().CategoryTypeId;

                var listMoneyUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == moneyUnitId).ToList();
                var listUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitId).ToList();
                var listPropertyEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == propertyId).ToList();
                var listPriceInventoryEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == priceInventoryId).ToList();

                var warehouseEntity = context.Warehouse.Where(w => w.Active == true).ToList();
                var vendorEntity = context.Vendor.Where(w => w.Active == true).OrderBy(w => w.VendorName).ToList();

                var productCodeEntity = context.Product.Select(w => new { w.ProductCode }).ToList();

                // lấy list loại hình kinh doanh: Chỉ bán ra, chỉ mua vào và cả 2.
                var loaiHinhTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HHKD")?.CategoryTypeId;
                var listLoaiHinh = context.Category.Where(x => x.CategoryTypeId == loaiHinhTypeId).Select(c => new CategoryEntityModel()
                {
                    CategoryTypeId = c.CategoryTypeId,
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryCode = c.CategoryCode,
                }).ToList();

                #endregion

                #region Patch to Response
                listPropertyEntity?.ForEach(e =>
                {
                    ListPropertyEntityModel.Add(new CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                        IsDefault = e.IsDefauld
                    });
                });
                listPriceInventoryEntity?.ForEach(e =>
                {
                    ListPriceInventoryEntityModel.Add(new CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                        IsDefault = e.IsDefauld
                    });
                });
                listMoneyUnitEntity?.ForEach(e =>
                {
                    ListProductMoneyUnitEntityModel.Add(new CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                        IsDefault = e.IsDefauld
                    });
                });

                listUnitEntity?.ForEach(e =>
                {
                    ListProductUnitEntityModel.Add(new CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                        IsDefault = e.IsDefauld
                    });
                });

                warehouseEntity?.ForEach(e =>
                {
                    ListWarehouseEntityModel.Add(new WareHouseEntityModel
                    {
                        WarehouseId = e.WarehouseId,
                        WarehouseCode = e.WarehouseCode,
                        WarehouseName = e.WarehouseName,
                        WarehouseParent = e.WarehouseParent
                    });
                });

                ListWarehouseEntityModel = ListWarehouseEntityModel.OrderBy(w => w.WarehouseName).ToList();

                vendorEntity?.ForEach(e =>
                {
                    var vendor = new VendorEntityModel();
                    vendor.VendorId = e.VendorId;
                    vendor.VendorName = e.VendorName;
                    vendor.VendorCode = e.VendorCode;
                    ListVendorEntityModel.Add(vendor);
                });

                ListVendorEntityModel = ListVendorEntityModel.OrderBy(w => w.VendorName).ToList();

                productCodeEntity?.ForEach(productCode =>
                {
                    listProductCode.Add(productCode.ProductCode?.Trim());
                });
                #endregion

                var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "DNH");
                listProductUnitName = context.Category.Where(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.Active == true).Select(c => c.CategoryName).ToList();
                                        
                return new GetMasterdataCreateProductResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListProductMoneyUnit = ListProductMoneyUnitEntityModel,
                    ListProductUnit = ListProductUnitEntityModel,
                    ListVendor = ListVendorEntityModel,
                    ListWarehouse = ListWarehouseEntityModel,
                    ListProductCode = listProductCode,
                    ListProductUnitName = listProductUnitName,
                    ListProperty = ListPropertyEntityModel,
                    ListPriceInventory = ListPriceInventoryEntityModel,
                    ListLoaiHinh = listLoaiHinh,
                };
            }
            catch (Exception ex)
            {

                return new GetMasterdataCreateProductResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }
        }

        public AddSerialNumberResult AddSerialNumber(AddSerialNumberParameter parameter)
        {
            try
            {
                var listSerialEntity = context.Serial.Where(x => x.ProductId != parameter.ProductId)
                    .Select(w => new { w.SerialCode }).ToList();
                var ListSerialNumber = new List<string>();

                listSerialEntity.ForEach(serial =>
                {
                    ListSerialNumber.Add(serial.SerialCode?.Trim());
                });

                return new AddSerialNumberResult
                {
                    ListSerialNumber = ListSerialNumber,
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {

                return new AddSerialNumberResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetMasterDataVendorDialogResult GetMasterDataVendorDialog(GetMasterDataVendorDialogParameter parameter)
        {
            var ListProductMoneyUnit = new List<CategoryEntityModel>();
            var ListVendor = new List<VendorEntityModel>();
            var listProduct = new List<ProductEntityModel>();
            var listSuggestedSupplierQuote = new List<SuggestedSupplierQuotesEntityModel>();

            var moneyUnitCode = "DTI"; //đơn vị tiền
            var moneyUnitId = context.CategoryType.Where(w => w.Active == true && w.CategoryTypeCode == moneyUnitCode).FirstOrDefault().CategoryTypeId; ;
            var listMoneyUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == moneyUnitId).ToList();
            var vendorEntity = context.Vendor.Where(w => w.Active == true).OrderBy(w => w.VendorName).ToList();


            var listProductResult = context.Product.Where(w => w.Active == true).OrderBy(w => w.ProductName).ToList();
            listProductResult.ForEach(item =>
            {
                listProduct.Add(new ProductEntityModel(item));
            });

            var listSuggestedSupplierQuoteResult = context.SuggestedSupplierQuotes.Where(c => c.Active == true).OrderBy(w => w.SuggestedSupplierQuote).ToList();
            listSuggestedSupplierQuoteResult.ForEach(item =>
            {
                listSuggestedSupplierQuote.Add(new SuggestedSupplierQuotesEntityModel(item));
            });


            listMoneyUnitEntity?.ForEach(e =>
            {
                ListProductMoneyUnit.Add(new CategoryEntityModel
                {
                    CategoryId = e.CategoryId,
                    CategoryName = e.CategoryName,
                    CategoryCode = e.CategoryCode,
                    IsDefauld = e.IsDefauld
                });
            });

            vendorEntity?.ForEach(e =>
            {
                ListVendor.Add(new VendorEntityModel
                {
                    VendorId = e.VendorId,
                    VendorName = e.VendorName,
                    VendorCode = e.VendorCode
                });
            });

            ListVendor = ListVendor.OrderBy(w => w.VendorName).ToList();

            return new GetMasterDataVendorDialogResult
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = "",
                ListProductMoneyUnit = ListProductMoneyUnit,
                ListVendor = ListVendor,
                ListProduct = listProduct,
                ListSuggestedSupplierQuote = listSuggestedSupplierQuote,
            };
        }

        public DownloadTemplateProductServiceResult DownloadTemplateProductService(DownloadTemplateProductServiceParameter parameter)
        {
            try
            {
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_Import_Product.xlsx";

                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateProductServiceResult
                {
                    TemplateExcel = data,
                    MessageCode = string.Format("Đã dowload file Template_Import_Product"),
                    FileName = "Template_Import_Product",
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception)
            {
                return new DownloadTemplateProductServiceResult
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình download",
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetMasterDataPriceProductResult GetMasterDataPriceList(GetMasterDataPriceProductParameter parameter)
        {
            try
            {
                var groupCustomerId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NHA").CategoryTypeId;
                var groupCustomerAll = context.Category.Where(c => c.CategoryTypeId == groupCustomerId).ToList();
                var groupCustomer = groupCustomerAll.Where(c => c.Active == true && c.CategoryTypeId == groupCustomerId).ToList();
                var listCategoryCustomer = new List<CategoryEntityModel>();
                groupCustomer.ForEach(item =>
                {
                    var newCategoryCustomer = new CategoryEntityModel()
                    {
                        CategoryId = item.CategoryId,
                        CategoryName = item.CategoryName,
                        CategoryCode = item.CategoryCode,
                    };
                    listCategoryCustomer.Add(newCategoryCustomer);
                });
                var listProduct = context.Product.ToList();
                var products = listProduct.Where(c => c.Active == true).ToList();
                var listProductEntityModel = new List<ProductEntityModel>();
                products.ForEach(item =>
                {
                    var newProduct = new ProductEntityModel()
                    {
                        ProductId = item.ProductId,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                    };
                    listProductEntityModel.Add(newProduct);
                });
                var listPrice = context.PriceProduct.Where(c => c.Active == true).OrderByDescending(x => x.EffectiveDate).ToList();
                var listPriceEntityModel = new List<PriceProductEntityModel>();

                listPrice.ForEach(item =>
                {
                    var newPriceProduct = new PriceProductEntityModel
                    {
                        PriceProductId = item.PriceProductId,
                        ProductId = item.ProductId,
                        ProductCode = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductCode ?? "",
                        ProductName = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductName ?? "",
                        EffectiveDate = item.EffectiveDate,
                        PriceVnd = item.PriceVnd,
                        MinQuantity = item.MinQuantity,
                        PriceForeignMoney = item.PriceForeignMoney,
                        CustomerGroupCategory = item.CustomerGroupCategory,
                        CustomerGroupCategoryName = groupCustomerAll.FirstOrDefault(c => c.CategoryId == item.CustomerGroupCategory)?.CategoryName ?? "",
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                        TiLeChietKhau = item.TiLeChietKhau,
                        NgayHetHan = item.NgayHetHan
                    };

                    listPriceEntityModel.Add(newPriceProduct);
                });
                listPriceEntityModel = listPriceEntityModel.OrderByDescending(c => c.EffectiveDate).ToList();

                return new GetMasterDataPriceProductResult
                {
                    ListProduct = listProductEntityModel,
                    ListPrice = listPriceEntityModel,
                    ListCategory = listCategoryCustomer,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "success"
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataPriceProductResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }

        }

        public CreateOrUpdatePriceProductResult CreateOrUpdatePriceProduct(CreateOrUpdatePriceProductParameter parameter)
        {
            try
            {
                var priceProduct = context.PriceProduct.FirstOrDefault(c => c.PriceProductId == parameter.PriceProduct.PriceProductId);

                var groupCustomerId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NHA").CategoryTypeId;
                var groupCustomerAll = context.Category.Where(c => c.CategoryTypeId == groupCustomerId).ToList();

                var listProduct = context.Product.ToList();
                var products = listProduct.Where(c => c.Active == true).ToList();
                string message = "";
                if (priceProduct == null)
                {
                    var newPriceProduct = new PriceProduct
                    {
                        PriceProductId = Guid.NewGuid(),
                        ProductId = parameter.PriceProduct.ProductId,
                        EffectiveDate = parameter.PriceProduct.EffectiveDate,
                        PriceVnd = parameter.PriceProduct.PriceVnd,
                        MinQuantity = parameter.PriceProduct.MinQuantity,
                        PriceForeignMoney = parameter.PriceProduct.PriceForeignMoney,
                        NgayHetHan = parameter.PriceProduct.NgayHetHan,
                        TiLeChietKhau = parameter.PriceProduct.TiLeChietKhau.Value,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        CustomerGroupCategory = parameter.PriceProduct.CustomerGroupCategory,
                        UpdatedById = null,
                        UpdatedDate = null
                    };
                    context.PriceProduct.Add(newPriceProduct);
                    message = Common.CommonMessage.PriceProduct.CREATE_SUCCESS;
                }
                else
                {
                    priceProduct.ProductId = parameter.PriceProduct.ProductId;
                    priceProduct.EffectiveDate = parameter.PriceProduct.EffectiveDate;
                    priceProduct.PriceVnd = parameter.PriceProduct.PriceVnd;
                    priceProduct.MinQuantity = parameter.PriceProduct.MinQuantity;
                    priceProduct.PriceForeignMoney = parameter.PriceProduct.PriceForeignMoney;
                    priceProduct.CustomerGroupCategory = parameter.PriceProduct.CustomerGroupCategory;
                    priceProduct.UpdatedById = parameter.UserId;
                    priceProduct.UpdatedDate = DateTime.Now;
                    priceProduct.TiLeChietKhau = parameter.PriceProduct.TiLeChietKhau.Value;
                    priceProduct.NgayHetHan = parameter.PriceProduct.NgayHetHan;
                    context.PriceProduct.Update(priceProduct);
                    message = Common.CommonMessage.PriceProduct.UPDATE_SUCCESS;
                }
                context.SaveChanges();

                var listPrice = context.PriceProduct.Where(c => c.Active == true).OrderByDescending(x => x.EffectiveDate).ToList();
                var listPriceEntityModel = new List<PriceProductEntityModel>();
                listPrice.ForEach(item =>
                {
                    var newPriceProduct = new PriceProductEntityModel
                    {
                        PriceProductId = item.PriceProductId,
                        ProductId = item.ProductId,
                        ProductCode = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductCode ?? "",
                        ProductName = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId)?.ProductName ?? "",
                        EffectiveDate = item.EffectiveDate,
                        PriceVnd = item.PriceVnd,
                        MinQuantity = item.MinQuantity,
                        NgayHetHan = item.NgayHetHan,
                        TiLeChietKhau = item.TiLeChietKhau,
                        PriceForeignMoney = item.PriceForeignMoney,
                        CustomerGroupCategory = item.CustomerGroupCategory,
                        CustomerGroupCategoryName = groupCustomerAll.FirstOrDefault(c => c.CategoryId == item.CustomerGroupCategory)?.CategoryName ?? "",
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate,
                    };

                    listPriceEntityModel.Add(newPriceProduct);
                });
                listPriceEntityModel = listPriceEntityModel.OrderByDescending(c => c.EffectiveDate).ToList();

                return new CreateOrUpdatePriceProductResult
                {
                    ListPrice = listPriceEntityModel,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = message
                };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdatePriceProductResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public DeletePriceProductResult DeletePriceProduct(DeletePriceProductParameter parameter)
        {
            try
            {
                var priceProduct = context.PriceProduct.FirstOrDefault(c => c.PriceProductId == parameter.PriceProductId);
                if (priceProduct == null)
                {
                    return new DeletePriceProductResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.PriceProduct.DELETE_FAIL
                    };
                }
                else
                {
                    priceProduct.Active = false;
                    context.PriceProduct.Update(priceProduct);
                    context.SaveChanges();
                    return new DeletePriceProductResult
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        MessageCode = CommonMessage.PriceProduct.DELETE_SUCCESS
                    };
                }
            }
            catch (Exception ex)
            {
                return new DeletePriceProductResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetDataQuickCreateProductResult GetDataQuickCreateProduct(GetDataQuickCreateProductParameter parameter)
        {
            try
            {
                //var listProductCode = new List<string>();
                //var ListProductUnit = new List<DataAccess.Databases.Entities.Category>();

                //var unitCode = "DNH"; //đơn vị tính
                //var priceInvetoryCode = "GTK";
                //var propertyCode = "TC";

                //var unitId = context.CategoryType.Where(w => w.CategoryTypeCode == unitCode).FirstOrDefault().CategoryTypeId;

                //var priceInventoryId = context.CategoryType.Where(c => c.CategoryTypeCode == priceInvetoryCode).FirstOrDefault().CategoryTypeId;

                //var productCodeEntity = context.Product.Select(w => new { w.ProductCode }).ToList();
                //var listUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitId).ToList();


                //productCodeEntity?.ForEach(productCode =>
                //{
                //    listProductCode.Add(productCode.ProductCode?.Trim());
                //});

                //listUnitEntity?.ForEach(e =>
                //{
                //    ListProductUnit.Add(new Category
                //    {
                //        CategoryId = e.CategoryId,
                //        CategoryName = e.CategoryName,
                //        CategoryCode = e.CategoryCode,
                //        IsDefauld = e.IsDefauld
                //    });
                //});

                //var propertyId = context.CategoryType.Where(c => c.CategoryTypeCode == propertyCode).FirstOrDefault().CategoryTypeId;
                //var listPriceInventoryEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == priceInventoryId).ToList();
                //var listPropertyEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == propertyId).ToList();

                #region Mã sản phẩm
                var listProductCode = context.Product.Where(w => w.Active == true).Select(w => w.ProductCode).ToList() ?? new List<string>();
                #endregion

                #region Đơn vị tính
                var listProductUnit = new List<DataAccess.Models.CategoryEntityModel>();

                var unitCode = "DNH"; //đơn vị tính
                var unitId = context.CategoryType.Where(w => w.CategoryTypeCode == unitCode).FirstOrDefault().CategoryTypeId;
                var listUnitEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitId).ToList();
                listUnitEntity?.ForEach(e =>
                {
                    listProductUnit.Add(new DataAccess.Models.CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                    });
                });
                #endregion

                #region Cách tính tồn kho
                var listPriceInventory = new List<DataAccess.Models.CategoryEntityModel>();

                var priceInvetoryCode = "GTK";
                var priceInventoryId = context.CategoryType.Where(c => c.CategoryTypeCode == priceInvetoryCode).FirstOrDefault().CategoryTypeId;
                var listPriceInventoryEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == priceInventoryId).ToList();
                listPriceInventoryEntity?.ForEach(e =>
                {
                    listPriceInventory.Add(new DataAccess.Models.CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                    });
                });
                #endregion

                #region Tính chất
                var listProperty = new List<DataAccess.Models.CategoryEntityModel>();

                var propertyCode = "TC";
                var propertyId = context.CategoryType.Where(c => c.CategoryTypeCode == propertyCode).FirstOrDefault().CategoryTypeId;
                var listPropertyEntity = context.Category.Where(c => c.Active == true && c.CategoryTypeId == propertyId).ToList();
                listPropertyEntity?.ForEach(e =>
                {
                    listProperty.Add(new DataAccess.Models.CategoryEntityModel
                    {
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        CategoryCode = e.CategoryCode,
                    });
                });
                #endregion

                // lấy list loại hình kinh doanh: Chỉ bán ra, chỉ mua vào và cả 2.
                var loaiHinhTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HHKD")?.CategoryTypeId;
                var listLoaiHinh = context.Category.Where(x => x.CategoryTypeId == loaiHinhTypeId).Select(c => new CategoryEntityModel()
                {
                    CategoryTypeId = c.CategoryTypeId,
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryCode = c.CategoryCode,
                }).ToList();

                return new GetDataQuickCreateProductResult
                {
                    ListProductCode = listProductCode,
                    ListProductUnit = listProductUnit,
                    ListPriceInventory = listPriceInventory,
                    ListProperty = listProperty,
                    ListLoaiHinh = listLoaiHinh,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = ""
                };
            }
            catch (Exception ex)
            {
                return new GetDataQuickCreateProductResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetDataCreateUpdateBOMResult GetDataCreateUpdateBOM(GetDataCreateUpdateBOMParameter parameter)
        {
            try
            {
                #region Lấy danh sách sản phẩm
                var listProductEntity = context.Product.Where(w => w.Active == true).ToList();
                var unitTypeCodeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH").CategoryTypeId;
                var listProductUnitEntity = context.Category.Where(w => w.CategoryTypeId == unitTypeCodeId && w.Active == true).ToList();

                var listProduct = new List<DataAccess.Models.Product.ProductEntityModel>();

                listProductEntity?.ForEach(product =>
                {
                    var productUnitName = listProductUnitEntity.FirstOrDefault(f => f.CategoryId == product.ProductUnitId)?.CategoryName ?? "";

                    listProduct.Add(new ProductEntityModel()
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductCode = product.ProductCode,
                        ProductUnitId = product.ProductUnitId,
                        ProductUnitName = productUnitName
                    });
                });
                #endregion

                return new GetDataCreateUpdateBOMResult
                {
                    ListProduct = listProduct,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = ""
                };
            }
            catch (Exception ex)
            {
                return new GetDataCreateUpdateBOMResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public DownloadPriceProductTemplateResult DownloadPriceProductTemplate(DownloadPriceProductTemplateParameter parameter)
        {
            try
            {
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_import_Bng_gi_bn.xlsx";

                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadPriceProductTemplateResult
                {
                    TemplateExcel = data,
                    MessageCode = string.Format("Đã dowload file Template_import_Bng_gi_bn"),
                    FileName = "Template_import_Bng_gi_bn",
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new DownloadPriceProductTemplateResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public ImportPriceProductResult ImportPriceProduct(ImportPriceProductParamter parameter)
        {
            try
            {
                if (parameter.ListPriceProduct == null)
                {
                    return new ImportPriceProductResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "not foud",
                    };
                }
                var list = new List<PriceProduct>();
                parameter.ListPriceProduct.ForEach(item =>
                {
                    var priceProduct = new PriceProduct
                    {
                        PriceProductId = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        EffectiveDate = item.EffectiveDate,
                        MinQuantity = item.MinQuantity,
                        PriceVnd = item.PriceVnd,
                        PriceForeignMoney = item.PriceForeignMoney,
                        CustomerGroupCategory = item.CustomerGroupCategory,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null
                    };
                    list.Add(priceProduct);
                });
                context.PriceProduct.AddRange(list);
                context.SaveChanges();

                return new ImportPriceProductResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception ex)
            {
                return new ImportPriceProductResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }
        }

        public CreateThuocTinhSanPhamResult CreateThuocTinhSanPham(CreateThuocTinhSanPhamParameter parameter)
        {
            try
            {
                var listProductAttributeCategoryValue = new List<ProductAttributeCategoryValue>();
                var listProductAttribute = new List<ProductAttribute>();

                //định nghĩa product attribute category
                var attributeCategoryObj = new ProductAttributeCategory
                {
                    ProductAttributeCategoryId = Guid.NewGuid(),
                    ProductAttributeCategoryName = parameter.ThuocTinh.ProductAttributeCategoryName?.Trim(),
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    Active = true
                };

                //gắn category với sản phẩm
                var productAttribute = new ProductAttribute
                {
                    ProductAttributeId = Guid.NewGuid(),
                    ProductId = parameter.ProductId,
                    ProductAttributeCategoryId = attributeCategoryObj.ProductAttributeCategoryId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    Active = true,
                    UpdatedBy = null,
                    CreatedBy = parameter.UserId
                };
                listProductAttribute.Add(productAttribute);

                //định nghĩa product attribute value
                if (parameter.ThuocTinh.ProductAttributeCategoryValue.Count > 0)
                {
                    List<ProductAttributeCategoryValueEntityModel> listAttributeValue =
                        parameter.ThuocTinh.ProductAttributeCategoryValue.ToList();

                    listAttributeValue.ForEach(attriButeValue =>
                    {
                        var attributeValue = new ProductAttributeCategoryValue
                        {
                            ProductAttributeCategoryValueId = Guid.NewGuid(),
                            ProductAttributeCategoryValue1 =
                                attriButeValue.ProductAttributeCategoryValue1?.Trim(),
                            ProductAttributeCategoryId = attributeCategoryObj.ProductAttributeCategoryId,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            Active = true
                        };
                        listProductAttributeCategoryValue.Add(attributeValue);
                    });
                }

                context.ProductAttributeCategory.Add(attributeCategoryObj);
                context.ProductAttributeCategoryValue.AddRange(listProductAttributeCategoryValue);
                context.ProductAttribute.AddRange(listProductAttribute);
                context.SaveChanges();

                return new CreateThuocTinhSanPhamResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ProductAttributeCategoryId = attributeCategoryObj.ProductAttributeCategoryId
                };
            }
            catch (Exception e)
            {
                return new CreateThuocTinhSanPhamResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteThuocTinhSanPhamResult DeleteThuocTinhSanPham(DeleteThuocTinhSanPhamParameter parameter)
        {
            try
            {
                var thuocTinh = context.ProductAttributeCategory.FirstOrDefault(x =>
                    x.ProductAttributeCategoryId == parameter.ProductAttributeCategoryId);

                if (thuocTinh == null)
                {
                    return new DeleteThuocTinhSanPhamResult()
                    {
                        MessageCode = "Thuộc tính không tồn tại trên hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var attribute = context.ProductAttribute.FirstOrDefault(x =>
                    x.ProductAttributeCategoryId == parameter.ProductAttributeCategoryId);
                var listGiaTriThuocTinh = context.ProductAttributeCategoryValue
                    .Where(x => x.ProductAttributeCategoryId == parameter.ProductAttributeCategoryId).ToList();

                #region Kiểm tra thuộc tính đã được sử dụng hay chưa

                var count_order = context.OrderProductDetailProductAttributeValue.Count(x =>
                    x.ProductAttributeCategoryId == thuocTinh.ProductAttributeCategoryId);

                if (count_order > 0)
                {
                    return new DeleteThuocTinhSanPhamResult()
                    {
                        MessageCode = "Không thể xóa vì Thuộc tính đã được sử dụng",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var count_quote = context.QuoteProductDetailProductAttributeValue.Count(x =>
                    x.ProductAttributeCategoryId == thuocTinh.ProductAttributeCategoryId);

                if (count_quote > 0)
                {
                    return new DeleteThuocTinhSanPhamResult()
                    {
                        MessageCode = "Không thể xóa vì Thuộc tính đã được sử dụng",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var count_contract = context.ContractDetailProductAttribute.Count(x =>
                    x.ProductAttributeCategoryId == thuocTinh.ProductAttributeCategoryId);

                if (count_contract > 0)
                {
                    return new DeleteThuocTinhSanPhamResult()
                    {
                        MessageCode = "Không thể xóa vì Thuộc tính đã được sử dụng",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var count_lead = context.LeadProductDetailProductAttributeValue.Count(x =>
                    x.ProductAttributeCategoryId == thuocTinh.ProductAttributeCategoryId);

                if (count_lead > 0)
                {
                    return new DeleteThuocTinhSanPhamResult()
                    {
                        MessageCode = "Không thể xóa vì Thuộc tính đã được sử dụng",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var count_vendor_order = context.VendorOrderProductDetailProductAttributeValue.Count(x =>
                    x.ProductAttributeCategoryId == thuocTinh.ProductAttributeCategoryId);

                if (count_vendor_order > 0)
                {
                    return new DeleteThuocTinhSanPhamResult()
                    {
                        MessageCode = "Không thể xóa vì Thuộc tính đã được sử dụng",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var count_hst = context.SaleBiddingDetailProductAttribute.Count(x =>
                    x.ProductAttributeCategoryId == thuocTinh.ProductAttributeCategoryId);

                if (count_hst > 0)
                {
                    return new DeleteThuocTinhSanPhamResult()
                    {
                        MessageCode = "Không thể xóa vì Thuộc tính đã được sử dụng",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var count_hoa_don = context.BillOfSaleCostProductAttribute.Count(x =>
                    x.ProductAttributeCategoryId == thuocTinh.ProductAttributeCategoryId);

                if (count_hoa_don > 0)
                {
                    return new DeleteThuocTinhSanPhamResult()
                    {
                        MessageCode = "Không thể xóa vì Thuộc tính đã được sử dụng",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #endregion

                context.ProductAttribute.Remove(attribute);
                context.ProductAttributeCategoryValue.RemoveRange(listGiaTriThuocTinh);
                context.ProductAttributeCategory.Remove(thuocTinh);
                context.SaveChanges();

                return new DeleteThuocTinhSanPhamResult()
                {
                    MessageCode = "Xóa thành công",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new DeleteThuocTinhSanPhamResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
