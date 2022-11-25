using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.DataAccess.Databases
{
    public partial class TNTN8Context : DbContext
    {
        public TNTN8Context()
        {
        }

        private Guid _tenantId;


        public TNTN8Context(DbContextOptions<TNTN8Context> options) : base(options)
        {

        }

        public TNTN8Context(DbContextOptions<TNTN8Context> options, ITenantProvider tenantProvider) : base(options)
        {
            _tenantId = tenantProvider.GetTenantId();
        }

        public virtual DbSet<ActionResource> ActionResource { get; set; }
        public virtual DbSet<AdditionalInformation> AdditionalInformation { get; set; }
        public virtual DbSet<RelateTaskMapping> RelateTaskMapping { get; set; }
        public virtual DbSet<Quiz> Quiz { get; set; }
        public virtual DbSet<AuditTrace> AuditTrace { get; set; }
        public virtual DbSet<BankAccount> BankAccount { get; set; }
        public virtual DbSet<BankBook> BankBook { get; set; }
        public virtual DbSet<BankPayableInvoice> BankPayableInvoice { get; set; }
        public virtual DbSet<BankPayableInvoiceMapping> BankPayableInvoiceMapping { get; set; }
        public virtual DbSet<BankReceiptInvoice> BankReceiptInvoice { get; set; }
        public virtual DbSet<BankReceiptInvoiceMapping> BankReceiptInvoiceMapping { get; set; }
        public virtual DbSet<BillOfSale> BillOfSale { get; set; }
        public virtual DbSet<BillOfSaleCost> BillOfSaleCost { get; set; }
        public virtual DbSet<BillOfSaleCostProductAttribute> BillOfSaleCostProductAttribute { get; set; }
        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<OverviewCandidate> OverviewCandidate { get; set; }
        public virtual DbSet<TaiLieuNhanVien> TaiLieuNhanVien { get; set; }
        public virtual DbSet<BillOfSaleDetail> BillOfSaleDetail { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<BusinessGoals> BusinessGoals { get; set; }
        public virtual DbSet<BusinessGoalsDetail> BusinessGoalsDetail { get; set; }
        public virtual DbSet<Case> Case { get; set; }
        public virtual DbSet<CaseActivities> CaseActivities { get; set; }
        public virtual DbSet<CashBook> CashBook { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryType> CategoryType { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyConfiguration> CompanyConfiguration { get; set; }
        public virtual DbSet<ConfigurationRule> ConfigurationRule { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<ContractCostDetail> ContractCostDetail { get; set; }
        public virtual DbSet<ContractDetail> ContractDetail { get; set; }
        public virtual DbSet<ContractDetailProductAttribute> ContractDetailProductAttribute { get; set; }
        public virtual DbSet<Cost> Cost { get; set; }
        public virtual DbSet<CostsQuote> CostsQuote { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<HopDongNhanSu> HopDongNhanSu { get; set; }
        public virtual DbSet<CustomerAdditionalInformation> CustomerAdditionalInformation { get; set; }
        public virtual DbSet<CustomerCare> CustomerCare { get; set; }
        public virtual DbSet<CustomerCareCustomer> CustomerCareCustomer { get; set; }
        public virtual DbSet<CustomerCareFeedBack> CustomerCareFeedBack { get; set; }
        public virtual DbSet<CustomerCareFilter> CustomerCareFilter { get; set; }
        public virtual DbSet<CustomerMeeting> CustomerMeeting { get; set; }
        public virtual DbSet<CustomerOrder> CustomerOrder { get; set; }
        public virtual DbSet<CustomerOrderDetail> CustomerOrderDetail { get; set; }
        public virtual DbSet<CustomerOrderLocalPointMapping> CustomerOrderLocalPointMapping { get; set; }
        public virtual DbSet<CustomerServiceLevel> CustomerServiceLevel { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
        public virtual DbSet<CandidateAssessment> CandidateAssessment { get; set; }
        public virtual DbSet<CandidateAssessmentDetail> CandidateAssessmentDetail { get; set; }
        public virtual DbSet<CandidateAssessmentMapping> CandidateAssessmentMapping { get; set; }
        public virtual DbSet<CandidateVacanciesMapping> CandidateVacanciesMapping { get; set; }
        public virtual DbSet<InterviewSchedule> InterviewSchedule { get; set; }
        public virtual DbSet<InterviewScheduleMapping> InterviewScheduleMapping { get; set; }
        public virtual DbSet<Vacancies> Vacancies { get; set; }
        public virtual DbSet<VacanciesDocument> VacanciesDocument { get; set; }
        public virtual DbSet<EmailTemplateCcvalue> EmailTemplateCcvalue { get; set; }
        public virtual DbSet<EmailTemplateToken> EmailTemplateToken { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeAllowance> EmployeeAllowance { get; set; }
        public virtual DbSet<EmployeeAssessment> EmployeeAssessment { get; set; }
        public virtual DbSet<EmployeeInsurance> EmployeeInsurance { get; set; }
        public virtual DbSet<EmployeeMonthySalary> EmployeeMonthySalary { get; set; }
        public virtual DbSet<EmployeeRequest> EmployeeRequest { get; set; }
        public virtual DbSet<EmployeeSalary> EmployeeSalary { get; set; }
        public virtual DbSet<EmployeeTimesheet> EmployeeTimesheet { get; set; }
        public virtual DbSet<ExternalUser> ExternalUser { get; set; }
        public virtual DbSet<FeatureNote> FeatureNote { get; set; }
        public virtual DbSet<FeatureWorkFlowProgress> FeatureWorkFlowProgress { get; set; }
        public virtual DbSet<FileInFolder> FileInFolder { get; set; }
        public virtual DbSet<Folder> Folder { get; set; }
        public virtual DbSet<GeographicalArea> GeographicalArea { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<GroupUser> GroupUser { get; set; }
        public virtual DbSet<InforScreen> InforScreen { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<InventoryDeliveryVoucher> InventoryDeliveryVoucher { get; set; }
        public virtual DbSet<InventoryDeliveryVoucherMapping> InventoryDeliveryVoucherMapping { get; set; }
        public virtual DbSet<InventoryDeliveryVoucherSerialMapping> InventoryDeliveryVoucherSerialMapping { get; set; }
        public virtual DbSet<InventoryDetail> InventoryDetail { get; set; }
        public virtual DbSet<InventoryReceivingVoucher> InventoryReceivingVoucher { get; set; }
        public virtual DbSet<InventoryReceivingVoucherMapping> InventoryReceivingVoucherMapping { get; set; }
        public virtual DbSet<InventoryReceivingVoucherSerialMapping> InventoryReceivingVoucherSerialMapping { get; set; }
        public virtual DbSet<InventoryReport> InventoryReport { get; set; }
        public virtual DbSet<Invoices> Invoices { get; set; }
        public virtual DbSet<Lead> Lead { get; set; }
        public virtual DbSet<LeadCare> LeadCare { get; set; }
        public virtual DbSet<LeadCareFeedBack> LeadCareFeedBack { get; set; }
        public virtual DbSet<LeadCareFilter> LeadCareFilter { get; set; }
        public virtual DbSet<LeadCareLead> LeadCareLead { get; set; }
        public virtual DbSet<LeadDetail> LeadDetail { get; set; }
        public virtual DbSet<LeadInterestedGroupMapping> LeadInterestedGroupMapping { get; set; }
        public virtual DbSet<LeadMeeting> LeadMeeting { get; set; }
        public virtual DbSet<LeadProductDetailProductAttributeValue> LeadProductDetailProductAttributeValue { get; set; }
        public virtual DbSet<LinkOfDocument> LinkOfDocument { get; set; }
        public virtual DbSet<LocalAddress> LocalAddress { get; set; }
        public virtual DbSet<LocalPoint> LocalPoint { get; set; }
        public virtual DbSet<LoginAuditTrace> LoginAuditTrace { get; set; }
        public virtual DbSet<MenuBuild> MenuBuild { get; set; }
        public virtual DbSet<MinusItemMapping> MinusItemMapping { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<NoteDocument> NoteDocument { get; set; }
        public virtual DbSet<NotifiAction> NotifiAction { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<NotifiCondition> NotifiCondition { get; set; }
        public virtual DbSet<NotifiSetting> NotifiSetting { get; set; }
        public virtual DbSet<NotifiSettingCondition> NotifiSettingCondition { get; set; }
        public virtual DbSet<NotifiSettingToken> NotifiSettingToken { get; set; }
        public virtual DbSet<NotifiSpecial> NotifiSpecial { get; set; }
        public virtual DbSet<OrderCostDetail> OrderCostDetail { get; set; }
        public virtual DbSet<OrderProductDetailProductAttributeValue> OrderProductDetailProductAttributeValue { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<OrderTechniqueMapping> OrderTechniqueMapping { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<PartItemMapping> PartItemMapping { get; set; }
        public virtual DbSet<PayableInvoice> PayableInvoice { get; set; }
        public virtual DbSet<PayableInvoiceMapping> PayableInvoiceMapping { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<PermissionMapping> PermissionMapping { get; set; }
        public virtual DbSet<PermissionSet> PermissionSet { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<PotentialCustomerProduct> PotentialCustomerProduct { get; set; }
        public virtual DbSet<PriceProduct> PriceProduct { get; set; }
        public virtual DbSet<PriceSuggestedSupplierQuotesMapping> PriceSuggestedSupplierQuotesMapping { get; set; }
        public virtual DbSet<ProcurementPlan> ProcurementPlan { get; set; }
        public virtual DbSet<ProcurementRequest> ProcurementRequest { get; set; }
        public virtual DbSet<ProcurementRequestItem> ProcurementRequestItem { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttribute { get; set; }
        public virtual DbSet<ProductAttributeCategory> ProductAttributeCategory { get; set; }
        public virtual DbSet<ProductAttributeCategoryValue> ProductAttributeCategoryValue { get; set; }
        public virtual DbSet<ProductBillOfMaterials> ProductBillOfMaterials { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductImage> ProductImage { get; set; }
        public virtual DbSet<ProductionOrder> ProductionOrder { get; set; }
        public virtual DbSet<ProductionOrderHistory> ProductionOrderHistory { get; set; }
        public virtual DbSet<ProductionOrderMapping> ProductionOrderMapping { get; set; }
        public virtual DbSet<ProductOrderWorkflow> ProductOrderWorkflow { get; set; }
        public virtual DbSet<ProductVendorMapping> ProductVendorMapping { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectContractMapping> ProjectContractMapping { get; set; }
        public virtual DbSet<ProjectCostReport> ProjectCostReport { get; set; }
        public virtual DbSet<ProjectEmployeeMapping> ProjectEmployeeMapping { get; set; }
        public virtual DbSet<ProjectMilestone> ProjectMilestone { get; set; }
        public virtual DbSet<ProjectObjective> ProjectObjective { get; set; }
        public virtual DbSet<ProjectResource> ProjectResource { get; set; }
        public virtual DbSet<ProjectScope> ProjectScope { get; set; }
        public virtual DbSet<ProjectScopeMapping> ProjectScopeMapping { get; set; }
        public virtual DbSet<ProjectTask> ProjectTask { get; set; }
        public virtual DbSet<ProjectVendor> ProjectVendor { get; set; }
        public virtual DbSet<Promotion> Promotion { get; set; }
        public virtual DbSet<PromotionMapping> PromotionMapping { get; set; }
        public virtual DbSet<PromotionObjectApply> PromotionObjectApply { get; set; }
        public virtual DbSet<PromotionObjectApplyMapping> PromotionObjectApplyMapping { get; set; }
        public virtual DbSet<PromotionProductMapping> PromotionProductMapping { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<PurchaseOrderStatus> PurchaseOrderStatus { get; set; }
        public virtual DbSet<RecruitmentCampaign> RecruitmentCampaign { get; set; }
        public virtual DbSet<Queue> Queue { get; set; }
        public virtual DbSet<Quote> Quote { get; set; }
        public virtual DbSet<QuoteApproveDetailHistory> QuoteApproveDetailHistory { get; set; }
        public virtual DbSet<QuoteApproveHistory> QuoteApproveHistory { get; set; }
        public virtual DbSet<QuoteCostDetail> QuoteCostDetail { get; set; }
        public virtual DbSet<QuoteDetail> QuoteDetail { get; set; }
        public virtual DbSet<QuoteDocument> QuoteDocument { get; set; }
        public virtual DbSet<QuoteParticipantMapping> QuoteParticipantMapping { get; set; }
        public virtual DbSet<QuotePaymentTerm> QuotePaymentTerm { get; set; }
        public virtual DbSet<QuotePlan> QuotePlan { get; set; }
        public virtual DbSet<QuoteProductDetailProductAttributeValue> QuoteProductDetailProductAttributeValue { get; set; }
        public virtual DbSet<QuoteScope> QuoteScope { get; set; }
        public virtual DbSet<ReceiptInvoice> ReceiptInvoice { get; set; }
        public virtual DbSet<ReceiptInvoiceMapping> ReceiptInvoiceMapping { get; set; }
        public virtual DbSet<ReceiptOrderHistory> ReceiptOrderHistory { get; set; }
        public virtual DbSet<RememberItem> RememberItem { get; set; }
        public virtual DbSet<RequestPayment> RequestPayment { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleAndMenuBuild> RoleAndMenuBuild { get; set; }
        public virtual DbSet<RoleAndPermission> RoleAndPermission { get; set; }
        public virtual DbSet<SaleBidding> SaleBidding { get; set; }
        public virtual DbSet<SaleBiddingDetail> SaleBiddingDetail { get; set; }
        public virtual DbSet<SaleBiddingDetailProductAttribute> SaleBiddingDetailProductAttribute { get; set; }
        public virtual DbSet<SaleBiddingEmployeeMapping> SaleBiddingEmployeeMapping { get; set; }
        public virtual DbSet<Satellite> Satellite { get; set; }
        public virtual DbSet<Screen> Screen { get; set; }
        public virtual DbSet<Serial> Serial { get; set; }
        public virtual DbSet<SoKho> SoKho { get; set; }
        public virtual DbSet<StockTake> StockTake { get; set; }
        public virtual DbSet<StockTakeProductMapping> StockTakeProductMapping { get; set; }
        public virtual DbSet<SuggestedSupplierQuotes> SuggestedSupplierQuotes { get; set; }
        public virtual DbSet<SuggestedSupplierQuotesDetail> SuggestedSupplierQuotesDetail { get; set; }
        public virtual DbSet<SynchonizedHistory> SynchonizedHistory { get; set; }
        public virtual DbSet<SystemFeature> SystemFeature { get; set; }
        public virtual DbSet<SystemParameter> SystemParameter { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<TaskConstraint> TaskConstraint { get; set; }
        public virtual DbSet<TaskDocument> TaskDocument { get; set; }
        public virtual DbSet<TaskMilestonesMapping> TaskMilestonesMapping { get; set; }
        public virtual DbSet<TaskResourceMapping> TaskResourceMapping { get; set; }
        public virtual DbSet<TechniqueRequest> TechniqueRequest { get; set; }
        public virtual DbSet<TechniqueRequestMapping> TechniqueRequestMapping { get; set; }
        public virtual DbSet<Template> Template { get; set; }
        public virtual DbSet<Tenants> Tenants { get; set; }
        public virtual DbSet<TermsOfPayment> TermsOfPayment { get; set; }
        public virtual DbSet<TimeSheet> TimeSheet { get; set; }
        public virtual DbSet<TimeSheetDetail> TimeSheetDetail { get; set; }
        public virtual DbSet<TotalProductionOrder> TotalProductionOrder { get; set; }
        public virtual DbSet<TotalProductionOrderMapping> TotalProductionOrderMapping { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<VendorOrder> VendorOrder { get; set; }
        public virtual DbSet<VendorOrderCostDetail> VendorOrderCostDetail { get; set; }
        public virtual DbSet<VendorOrderDetail> VendorOrderDetail { get; set; }
        public virtual DbSet<VendorOrderProcurementRequestMapping> VendorOrderProcurementRequestMapping { get; set; }
        public virtual DbSet<VendorOrderProductDetailProductAttributeValue> VendorOrderProductDetailProductAttributeValue { get; set; }
        public virtual DbSet<Ward> Ward { get; set; }
        public virtual DbSet<Warehouse> Warehouse { get; set; }
        public virtual DbSet<WorkFlows> WorkFlows { get; set; }
        public virtual DbSet<WorkFlowSteps> WorkFlowSteps { get; set; }
        public virtual DbSet<ThanhVienPhongBan> ThanhVienPhongBan { get; set; }
        public virtual DbSet<QuyTrinh> QuyTrinh { get; set; }
        public virtual DbSet<CauHinhQuyTrinh> CauHinhQuyTrinh { get; set; }
        public virtual DbSet<CacBuocQuyTrinh> CacBuocQuyTrinh { get; set; }
        public virtual DbSet<PhongBanTrongCacBuocQuyTrinh> PhongBanTrongCacBuocQuyTrinh { get; set; }
        public virtual DbSet<CacBuocApDung> CacBuocApDung { get; set; }
        public virtual DbSet<PhongBanApDung> PhongBanApDung { get; set; }
        public virtual DbSet<LichSuPheDuyet> LichSuPheDuyet { get; set; }
        public virtual DbSet<KichHoatTinhHuong> KichHoatTinhHuong { get; set; }
        public virtual DbSet<KichHoatTinhHuongChiTiet> KichHoatTinhHuongChiTiet { get; set; }
        public virtual DbSet<CaLamViec> CaLamViec { get; set; }
        public virtual DbSet<CaLamViecChiTiet> CaLamViecChiTiet { get; set; }
        public virtual DbSet<CauHinhGiamTru> CauHinhGiamTru { get; set; }
        public virtual DbSet<CauHinhNghiLe> CauHinhNghiLe { get; set; }
        public virtual DbSet<CauHinhNghiLeChiTiet> CauHinhNghiLeChiTiet { get; set; }
        public virtual DbSet<CauHinhOt> CauHinhOt { get; set; }
        public virtual DbSet<LichSuCauHinhBaoHiem> LichSuCauHinhBaoHiem { get; set; }
        public virtual DbSet<LichSuCauHinhBaoHiemLoftCare> LichSuCauHinhBaoHiemLoftCare { get; set; }
        public virtual DbSet<TroCap> TroCap { get; set; }
        public virtual DbSet<TroCapChucVuMapping> TroCapChucVuMapping { get; set; }
        public virtual DbSet<DeXuatThayDoiChucVu> DeXuatThayDoiChucVu { get; set; }
        public virtual DbSet<NhanVienDeXuatThayDoiChucVu> NhanVienDeXuatThayDoiChucVu { get; set; }
        public virtual DbSet<DeXuatTangLuong> DeXuatTangLuong { get; set; }
        public virtual DbSet<DeXuatTangLuongNhanVien> DeXuatTangLuongNhanVien { get; set; }
        public virtual DbSet<TaiSan> TaiSan { get; set; }
        public virtual DbSet<BaoDuongTaiSan> BaoDuongTaiSan { get; set; }
        public virtual DbSet<ThongTinCapPhatTaiSan> ThongTinCapPhatTaiSan { get; set; }
        public virtual DbSet<CapPhatTaiSan> CapPhatTaiSan { get; set; }
        public virtual DbSet<CauHinhBaoHiem> CauHinhBaoHiem { get; set; }
        public virtual DbSet<CauHinhBaoHiemLoftCare> CauHinhBaoHiemLoftCare { get; set; }
        public virtual DbSet<DoiTuongPhuThuocMapping> DoiTuongPhuThuocMapping { get; set; }
        public virtual DbSet<MucHuongBaoHiemLoftCare> MucHuongBaoHiemLoftCare { get; set; }
        public virtual DbSet<KeHoachOt> KeHoachOt { get; set; }
        public virtual DbSet<KeHoachOtThanhVien> KeHoachOtThanhVien { get; set; }
        public virtual DbSet<YeuCauCapPhatTaiSan> YeuCauCapPhatTaiSan { get; set; }
        public virtual DbSet<YeuCauCapPhatTaiSanChiTiet> YeuCauCapPhatTaiSanChiTiet { get; set; }
        public virtual DbSet<KeHoachOtPhongBan> KeHoachOtPhongBan { get; set; }
        public virtual DbSet<DeXuatCongTac> DeXuatCongTac { get; set; }
        public virtual DbSet<ChiTietDeXuatCongTac> ChiTietDeXuatCongTac { get; set; }
        public virtual DbSet<HoSoCongTac> HoSoCongTac { get; set; }
        public virtual DbSet<KinhPhiCongDoan> KinhPhiCongDoan { get; set; }
        public virtual DbSet<CongThucTinhLuong> CongThucTinhLuong { get; set; }
        public virtual DbSet<DeNghiTamHoanUng> DeNghiTamHoanUng { get; set; }
        public virtual DbSet<DeNghiTamHoanUngChiTiet> DeNghiTamHoanUngChiTiet { get; set; }
        public virtual DbSet<ChucVuBaoHiemLoftCare> ChucVuBaoHiemLoftCare { get; set; }
        public virtual DbSet<MucDanhGia> MucDanhGia { get; set; }
        public virtual DbSet<CauHoiMucDanhGiaMapping> CauHoiMucDanhGiaMapping { get; set; }
        public virtual DbSet<MucDanhGiaDanhGiaMapping> MucDanhGiaDanhGiaMapping { get; set; }
        public virtual DbSet<ChamCong> ChamCong { get; set; }
        public virtual DbSet<ThongKeDiMuonVeSom> ThongKeDiMuonVeSom { get; set; }
        public virtual DbSet<CauHinhChecklist> CauHinhChecklist { get; set; }
        public virtual DbSet<QuyLuong> QuyLuong { get; set; }
        public virtual DbSet<PhieuDanhGia> PhieuDanhGia { get; set; }
        public virtual DbSet<CauHoiPhieuDanhGiaMapping> CauHoiPhieuDanhGiaMapping { get; set; }
        public virtual DbSet<CauHoiPhieuDanhGiaMappingDanhMucItem> CauHoiPhieuDanhGiaMappingDanhMucItem { get; set; }
        public virtual DbSet<KyDanhGia> KyDanhGia { get; set; }
        public virtual DbSet<DeXuatXinNghi> DeXuatXinNghi { get; set; }
        public virtual DbSet<DeXuatXinNghiChiTiet> DeXuatXinNghiChiTiet { get; set; }
        public virtual DbSet<CauHinhChamCongOt> CauHinhChamCongOt { get; set; }
        public virtual DbSet<ChamCongOt> ChamCongOt { get; set; }
        public virtual DbSet<TongHopChamCongOt> TongHopChamCongOt { get; set; }
        public virtual DbSet<CauHinhOtCaNgay> CauHinhOtCaNgay { get; set; }
        public virtual DbSet<CauHinhThueTncn> CauHinhThueTncn { get; set; }
        public virtual DbSet<NhanVienKyDanhGia> NhanVienKyDanhGia { get; set; }
        public virtual DbSet<NoiDungKyDanhGia> NoiDungKyDanhGia { get; set; }
        public virtual DbSet<DanhGiaNhanVien> DanhGiaNhanVien { get; set; }
        public virtual DbSet<ChiTietDanhGiaNhanVien> ChiTietDanhGiaNhanVien { get; set; }
        public virtual DbSet<DanhMucCauTraLoiDanhGiaMapping> DanhMucCauTraLoiDanhGiaMapping { get; set; }
        public virtual DbSet<LichSuThanhToanBaoHiem> LichSuThanhToanBaoHiem { get; set; }
        public virtual DbSet<NhomBaoHiemLoftCare> NhomBaoHiemLoftCare { get; set; }
        public virtual DbSet<QuyenLoiBaoHiemLoftCare> QuyenLoiBaoHiemLoftCare { get; set; }
        public virtual DbSet<TroCapDieuKienHuongMapping> TroCapDieuKienHuongMapping { get; set; }
        public virtual DbSet<TroCapLoaiHopDongMapping> TroCapLoaiHopDongMapping { get; set; }
        public virtual DbSet<MucHuongDmvs> MucHuongDmvs { get; set; }
        public virtual DbSet<MucHuongTheoNgayNghi> MucHuongTheoNgayNghi { get; set; }
        public virtual DbSet<KyLuong> KyLuong { get; set; }
        public virtual DbSet<LuongCtBaoHiem> LuongCtBaoHiem { get; set; }
        public virtual DbSet<LuongCtCtyDong> LuongCtCtyDong { get; set; }
        public virtual DbSet<LuongCtDieuKienTroCapCoDinh> LuongCtDieuKienTroCapCoDinh { get; set; }
        public virtual DbSet<LuongCtDieuKienTroCapKhac> LuongCtDieuKienTroCapKhac { get; set; }
        public virtual DbSet<LuongCtGiamTruSauThue> LuongCtGiamTruSauThue { get; set; }
        public virtual DbSet<LuongCtGiamTruTruocThue> LuongCtGiamTruTruocThue { get; set; }
        public virtual DbSet<LuongCtHoanLaiSauThue> LuongCtHoanLaiSauThue { get; set; }
        public virtual DbSet<LuongCtLoaiTroCapCoDinh> LuongCtLoaiTroCapCoDinh { get; set; }
        public virtual DbSet<LuongCtLoaiTroCapKhac> LuongCtLoaiTroCapKhac { get; set; }
        public virtual DbSet<LuongCtLoaiTroCapOt> LuongCtLoaiTroCapOt { get; set; }
        public virtual DbSet<LuongCtOther> LuongCtOther { get; set; }
        public virtual DbSet<LuongCtThuNhapTinhThue> LuongCtThuNhapTinhThue { get; set; }
        public virtual DbSet<LuongCtTroCapCoDinh> LuongCtTroCapCoDinh { get; set; }
        public virtual DbSet<LuongCtTroCapKhac> LuongCtTroCapKhac { get; set; }
        public virtual DbSet<LuongCtTroCapOt> LuongCtTroCapOt { get; set; }
        public virtual DbSet<LuongTongHop> LuongTongHop { get; set; }
        public virtual DbSet<TongHopChamCong> TongHopChamCong { get; set; }
        public virtual DbSet<LuongCtEmpDktcKhac> LuongCtEmpDktcKhac { get; set; }
        public virtual DbSet<PhieuLuong> PhieuLuong { get; set; }
        public virtual DbSet<DotKiemKe> DotKiemKe { get; set; }
        public virtual DbSet<DotKiemKeChiTiet> DotKiemKeChiTiet { get; set; }
        public virtual DbSet<PhongBanPheDuyetDoiTuong> PhongBanPheDuyetDoiTuong { get; set; }
        public virtual DbSet<EmailNhanSu> EmailNhanSu { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HopDongNhanSu>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NgayBatDauLamViec).HasColumnType("datetime");

                entity.Property(e => e.NgayKetThucHopDong).HasColumnType("datetime");

                entity.Property(e => e.NgayKyHopDong).HasColumnType("datetime");

                entity.Property(e => e.SoHopDong)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.SoPhuLuc)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TroCap>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MucTroCap).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TypeId)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NhanVienDeXuatThayDoiChucVu>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TrangThai).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatThayDoiChucVu>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.NgayApDung).HasColumnType("datetime");

                entity.Property(e => e.NgayDeXuat).HasColumnType("datetime");

                entity.Property(e => e.TenDeXuat).IsRequired();

                entity.Property(e => e.TrangThai).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TroCapChucVuMapping>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LichSuCauHinhBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MucDongCu).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.SoNamLamViecCu).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TenQuyenLoiCu).IsRequired();

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ActionResource>(entity =>
            {
                entity.Property(e => e.ActionResourceId).ValueGeneratedNever();

                entity.Property(e => e.ActionResource1)
                    .IsRequired()
                    .HasColumnName("ActionResource")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<LichSuCauHinhBaoHiem>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MucDongToiDaCu)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TiLePhanBoMucDongBhtncuaNldcu)
                    .HasColumnName("TiLePhanBoMucDongBHTNCuaNLDCu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TiLePhanBoMucDongBhtncuaNsdldcu)
                    .HasColumnName("TiLePhanBoMucDongBHTNCuaNSDLDCu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TiLePhanBoMucDongBhxhcuaNldcu)
                    .HasColumnName("TiLePhanBoMucDongBHXHCuaNLDCu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TiLePhanBoMucDongBhxhcuaNsdldcu)
                    .HasColumnName("TiLePhanBoMucDongBHXHCuaNSDLDCu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TiLePhanBoMucDongBhytcuaNldcu)
                    .HasColumnName("TiLePhanBoMucDongBHYTCuaNLDCu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TiLePhanBoMucDongBhytcuaNsdldcu)
                    .HasColumnName("TiLePhanBoMucDongBHYTCuaNSDLDCu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<AdditionalInformation>(entity =>
            {
                entity.Property(e => e.AdditionalInformationId).ValueGeneratedNever();

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CandidateAssessmentMapping>(entity =>
            {
                entity.Property(e => e.CandidateAssessmentMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AuditTrace>(entity =>
            {
                entity.HasKey(e => e.TraceId);

                entity.Property(e => e.TraceId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ActionName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ObjectName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.Property(e => e.BankAccountId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.AccountName).HasMaxLength(250);

                entity.Property(e => e.AccountNumber).HasMaxLength(50);

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BankDetail).HasMaxLength(250);

                entity.Property(e => e.BankName).HasMaxLength(250);

                entity.Property(e => e.BranchName).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankBook>(entity =>
            {
                entity.Property(e => e.BankBookId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankPayableInvoice>(entity =>
            {
                entity.Property(e => e.BankPayableInvoiceId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.BankPayableInvoiceAmount).HasColumnType("money");

                entity.Property(e => e.BankPayableInvoiceAmountText).HasMaxLength(250);

                entity.Property(e => e.BankPayableInvoiceCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BankPayableInvoiceDetail).HasMaxLength(500);

                entity.Property(e => e.BankPayableInvoiceExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.BankPayableInvoicePaidDate).HasColumnType("datetime");

                entity.Property(e => e.BankPayableInvoicePrice).HasColumnType("money");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ReceiveAccountName).HasMaxLength(250);

                entity.Property(e => e.ReceiveAccountNumber).HasMaxLength(250);

                entity.Property(e => e.ReceiveBankName).HasMaxLength(250);

                entity.Property(e => e.ReceiveBranchName).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VouchersDate).HasColumnType("datetime");

                entity.HasOne(d => d.BankPayableInvoiceBankAccount)
                    .WithMany(p => p.BankPayableInvoice)
                    .HasForeignKey(d => d.BankPayableInvoiceBankAccountId)
                    .HasConstraintName("FK__BankPayab__BankP__17B8652E");

                entity.HasOne(d => d.BankPayableInvoicePriceCurrencyNavigation)
                    .WithMany(p => p.BankPayableInvoiceBankPayableInvoicePriceCurrencyNavigation)
                    .HasForeignKey(d => d.BankPayableInvoicePriceCurrency)
                    .HasConstraintName("FK__BankPayab__BankP__15D01CBC");

                entity.HasOne(d => d.BankPayableInvoiceReasonNavigation)
                    .WithMany(p => p.BankPayableInvoiceBankPayableInvoiceReasonNavigation)
                    .HasForeignKey(d => d.BankPayableInvoiceReason)
                    .HasConstraintName("FK__BankPayab__BankP__16C440F5");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.BankPayableInvoice)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK__BankPayab__Organ__19A0ADA0");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.BankPayableInvoiceStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__BankPayab__Statu__1A94D1D9");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankPayableInvoiceMapping>(entity =>
            {
                entity.Property(e => e.BankPayableInvoiceMappingId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.BankPayableInvoice)
                    .WithMany(p => p.BankPayableInvoiceMapping)
                    .HasForeignKey(d => d.BankPayableInvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BankPayab__BankP__241E3C13");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankReceiptInvoice>(entity =>
            {
                entity.Property(e => e.BankReceiptInvoiceId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.BankReceiptInvoiceAmount).HasColumnType("money");

                entity.Property(e => e.BankReceiptInvoiceAmountText).HasMaxLength(250);

                entity.Property(e => e.BankReceiptInvoiceCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BankReceiptInvoiceDetail).HasMaxLength(500);

                entity.Property(e => e.BankReceiptInvoiceExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.BankReceiptInvoicePaidDate).HasColumnType("datetime");

                entity.Property(e => e.BankReceiptInvoicePrice).HasColumnType("money");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VouchersDate).HasColumnType("datetime");

                entity.HasOne(d => d.BankReceiptInvoiceBankAccount)
                    .WithMany(p => p.BankReceiptInvoice)
                    .HasForeignKey(d => d.BankReceiptInvoiceBankAccountId)
                    .HasConstraintName("FK__BankRecei__BankR__1F5986F6");

                entity.HasOne(d => d.BankReceiptInvoicePriceCurrencyNavigation)
                    .WithMany(p => p.BankReceiptInvoiceBankReceiptInvoicePriceCurrencyNavigation)
                    .HasForeignKey(d => d.BankReceiptInvoicePriceCurrency)
                    .HasConstraintName("FK__BankRecei__BankR__1D713E84");

                entity.HasOne(d => d.BankReceiptInvoiceReasonNavigation)
                    .WithMany(p => p.BankReceiptInvoiceBankReceiptInvoiceReasonNavigation)
                    .HasForeignKey(d => d.BankReceiptInvoiceReason)
                    .HasConstraintName("FK__BankRecei__BankR__1E6562BD");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.BankReceiptInvoice)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK__BankRecei__Organ__204DAB2F");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.BankReceiptInvoiceStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__BankRecei__Statu__2141CF68");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankReceiptInvoiceMapping>(entity =>
            {
                entity.Property(e => e.BankReceiptInvoiceMappingId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.BankReceiptInvoice)
                    .WithMany(p => p.BankReceiptInvoiceMapping)
                    .HasForeignKey(d => d.BankReceiptInvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BankRecei__BankR__26FAA8BE");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BillOfSale>(entity =>
            {
                entity.Property(e => e.BillOfSaLeId).ValueGeneratedNever();

                entity.Property(e => e.BillDate).HasColumnType("datetime");

                entity.Property(e => e.BillOfSaLeCode)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerAddress).HasMaxLength(250);

                entity.Property(e => e.CustomerName).HasMaxLength(250);

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.InvoiceSymbol).HasMaxLength(250);

                entity.Property(e => e.Mst)
                    .HasColumnName("MST")
                    .HasMaxLength(50);

                entity.Property(e => e.PercentAdvance).HasColumnType("money");

                entity.Property(e => e.PercentAdvanceType)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 6)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BillOfSaleCost>(entity =>
            {
                entity.Property(e => e.BillOfSaleCostId).ValueGeneratedNever();

                entity.Property(e => e.CostCode).HasMaxLength(50);

                entity.Property(e => e.CostName).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BillOfSaleCostProductAttribute>(entity =>
            {
                entity.Property(e => e.BillOfSaleCostProductAttributeId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BillOfSaleDetail>(entity =>
            {
                entity.Property(e => e.BillOfSaleDetailId).ValueGeneratedNever();

                entity.Property(e => e.ActualInventory).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.BusinessInventory).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.IncurredUnit).HasMaxLength(50);

                entity.Property(e => e.MoneyForGoods).HasColumnType("money");

                entity.Property(e => e.PriceInitial).HasColumnType("money");

                entity.Property(e => e.ProductName).HasMaxLength(500);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitLaborPrice).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.Property(e => e.BranchId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.BranchCode).HasMaxLength(100);

                entity.Property(e => e.BranchName).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BusinessGoals>(entity =>
            {
                entity.Property(e => e.BusinessGoalsId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Year).HasMaxLength(10);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BusinessGoalsDetail>(entity =>
            {
                entity.Property(e => e.BusinessGoalsDetailId).ValueGeneratedNever();

                entity.Property(e => e.April)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.August)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BusinessGoalsType).HasMaxLength(10);

                entity.Property(e => e.December)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.February)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.January)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.July)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.June)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.March)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.May)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.November)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.October)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.September)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Case>(entity =>
            {
                entity.Property(e => e.CaseId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ObjectType).HasMaxLength(50);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CaseActivities>(entity =>
            {
                entity.Property(e => e.CaseActivitiesId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Case)
                    .WithMany(p => p.CaseActivities)
                    .HasForeignKey(d => d.CaseId)
                    .HasConstraintName("FK_CaseActivities_Case");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CashBook>(entity =>
            {
                entity.Property(e => e.CashBookId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.PaidDate).HasColumnType("date");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CategoryCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(600);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("date");

                entity.HasOne(d => d.CategoryType)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.CategoryTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Category_CategoryType");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CategoryType>(entity =>
            {
                entity.Property(e => e.CategoryTypeId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CategoryTypeCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CategoryTypeName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });



            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.CompanyId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CompanyConfiguration>(entity =>
            {
                entity.HasKey(e => e.CompanyId);

                entity.Property(e => e.CompanyId).ValueGeneratedNever();

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ContactName).HasMaxLength(250);

                entity.Property(e => e.ContactRole).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.HasOne(d => d.BankAccount)
                    .WithMany(p => p.CompanyConfiguration)
                    .HasForeignKey(d => d.BankAccountId)
                    .HasConstraintName("FK__CompanyCo__BankA__12F3B011");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ConfigurationRule>(entity =>
            {
                entity.Property(e => e.ConfigurationRuleId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Money).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.ContactId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.AccountNumber).HasMaxLength(255);

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.AddressTiengAnh).HasMaxLength(255);

                entity.Property(e => e.Agency).HasMaxLength(100);

                entity.Property(e => e.BankAccount).HasMaxLength(255);

                entity.Property(e => e.BankAddress).HasMaxLength(255);

                entity.Property(e => e.BankCode).HasMaxLength(50);

                entity.Property(e => e.BankName).HasMaxLength(500);

                entity.Property(e => e.BankOwnerName).HasMaxLength(255);

                entity.Property(e => e.Birthplace).HasMaxLength(100);

                entity.Property(e => e.BranchName).HasMaxLength(255);

                entity.Property(e => e.CompanyAddress).HasMaxLength(500);

                entity.Property(e => e.CompanyName).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.EvaluateContactPeople).HasMaxLength(2000);

                entity.Property(e => e.FirstName).HasMaxLength(250);

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.HealthInsuranceDateOfIssue).HasColumnType("date");

                entity.Property(e => e.HealthInsuranceDateOfParticipation).HasColumnType("date");

                entity.Property(e => e.HealthInsuranceNumber).HasMaxLength(50);

                entity.Property(e => e.HoKhauThuongTruTa)
                    .HasColumnName("HoKhauThuongTruTA")
                    .HasMaxLength(255);

                entity.Property(e => e.HoKhauThuongTruTv)
                    .HasColumnName("HoKhauThuongTruTV")
                    .HasMaxLength(255);

                entity.Property(e => e.IdentityId)
                    .HasColumnName("IdentityID")
                    .HasMaxLength(20);

                entity.Property(e => e.IdentityIddateOfIssue)
                    .HasColumnName("IdentityIDDateOfIssue")
                    .HasColumnType("date");

                entity.Property(e => e.IdentityIddateOfParticipation)
                    .HasColumnName("IdentityIDDateOfParticipation")
                    .HasColumnType("date");

                entity.Property(e => e.IdentityIdplaceOfIssue)
                    .HasColumnName("IdentityIDPlaceOfIssue")
                    .HasMaxLength(100);

                entity.Property(e => e.Job).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(250);

                entity.Property(e => e.Latitude).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.LinkFace).HasMaxLength(2000);

                entity.Property(e => e.Longitude).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.MaTheBhLoftCare).HasMaxLength(255);

                entity.Property(e => e.MoneyLimit).HasColumnType("money");

                entity.Property(e => e.NguyenQuan).HasMaxLength(255);

                entity.Property(e => e.NoiCapCmndtiengAnh)
                    .HasColumnName("NoiCapCMNDTiengAnh")
                    .HasMaxLength(255);

                entity.Property(e => e.NoiSinh).HasMaxLength(255);

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.OptionPosition).HasMaxLength(500);

                entity.Property(e => e.Other).HasMaxLength(500);

                entity.Property(e => e.OtherEmail).HasMaxLength(100);

                entity.Property(e => e.OtherPhone).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.PhuThuoc).HasDefaultValueSql("((0))");

                entity.Property(e => e.PhuThuocDenNgay).HasColumnType("datetime");

                entity.Property(e => e.PhuThuocTuNgay).HasColumnType("datetime");

                entity.Property(e => e.PostCode).HasMaxLength(10);

                entity.Property(e => e.PotentialCustomerPosition).HasMaxLength(100);

                entity.Property(e => e.Role).HasMaxLength(100);

                entity.Property(e => e.SocialInsuranceDateOfIssue).HasColumnType("date");

                entity.Property(e => e.SocialInsuranceDateOfParticipation).HasColumnType("date");

                entity.Property(e => e.SocialInsuranceNumber).HasMaxLength(50);

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VisaDateOfIssue).HasColumnType("datetime");

                entity.Property(e => e.VisaExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.VisaNumber).HasMaxLength(50);

                entity.Property(e => e.WorkEmail).HasMaxLength(100);

                entity.Property(e => e.WorkPermitNumber).HasMaxLength(50);

                entity.Property(e => e.WorkPhone).HasMaxLength(50);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK__Contact__Country");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Contact_District");

                entity.HasOne(d => d.MaritalStatus)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.MaritalStatusId)
                    .HasConstraintName("FK__Contact__Category");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Contact_Province");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_Contact_Ward");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.Property(e => e.ContractId).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.ContractCode)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ContractDescription).HasMaxLength(500);

                entity.Property(e => e.ContractName).HasMaxLength(500);

                entity.Property(e => e.ContractNote).HasMaxLength(500);

                entity.Property(e => e.ContractTimeUnit).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiredDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.ValueContract).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ContractCostDetail>(entity =>
            {
                entity.Property(e => e.ContractCostDetailId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CostName).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.ContractCostDetail)
                    .HasForeignKey(d => d.ContractId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContractCostDetail_Contract");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ContractDetail>(entity =>
            {
                entity.Property(e => e.ContractDetailId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.GuaranteeTime).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.IncurredUnit).HasMaxLength(50);

                entity.Property(e => e.PriceInitial).HasColumnType("money");

                entity.Property(e => e.Quantity).HasColumnType("money");

                entity.Property(e => e.QuantityOdered).HasColumnType("money");

                entity.Property(e => e.Tax).HasColumnType("money");

                entity.Property(e => e.UnitLaborPrice).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ContractDetailProductAttribute>(entity =>
            {
                entity.Property(e => e.ContractDetailProductAttributeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Cost>(entity =>
            {
                entity.Property(e => e.CostId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DonGia).HasColumnType("money");

                entity.Property(e => e.NgayHetHan).HasColumnType("datetime");

                entity.Property(e => e.NgayHieuLuc)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SoLuongToiThieu)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CostsQuote>(entity =>
            {
                entity.Property(e => e.CostsQuoteId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.IncurredUnit).HasMaxLength(50);

                entity.Property(e => e.ProductName).HasMaxLength(250);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitLaborPrice).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CountryId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BusinessRegistrationDate).HasColumnType("date");

                entity.Property(e => e.ContactDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CustomerName).HasMaxLength(500);

                entity.Property(e => e.EvaluateCompany).HasMaxLength(2000);

                entity.Property(e => e.MaximumDebtValue).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.NearestDateTransaction).HasColumnType("datetime");

                entity.Property(e => e.PayPoint).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Point).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.SalesUpdate).HasMaxLength(2000);

                entity.Property(e => e.SalesUpdateAfterMeeting).HasMaxLength(2000);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CustomerServiceLevel)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.CustomerServiceLevelId)
                    .HasConstraintName("FK__Customer__CustomerServiceLevel");

                entity.HasOne(d => d.Field)
                    .WithMany(p => p.CustomerField)
                    .HasForeignKey(d => d.FieldId)
                    .HasConstraintName("FK__Customer__FieldId");

                entity.HasOne(d => d.Scale)
                    .WithMany(p => p.CustomerScale)
                    .HasForeignKey(d => d.ScaleId)
                    .HasConstraintName("FK__Customer__ScaleId");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.CustomerStatus)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Status");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerAdditionalInformation>(entity =>
            {
                entity.Property(e => e.CustomerAdditionalInformationId).ValueGeneratedNever();

                entity.Property(e => e.Answer).HasMaxLength(4000);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Question).HasMaxLength(4000);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerAdditionalInformation)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerAdditionalInformation_Customer");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerCare>(entity =>
            {
                entity.Property(e => e.CustomerCareId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ActiveDate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerCareCode).HasMaxLength(50);

                entity.Property(e => e.CustomerCareContentSms).HasColumnName("CustomerCareContentSMS");

                entity.Property(e => e.CustomerCareVoucher)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DiscountAmount).HasColumnType("money");

                entity.Property(e => e.EffecttiveFromDate).HasColumnType("datetime");

                entity.Property(e => e.EffecttiveToDate).HasColumnType("datetime");

                entity.Property(e => e.ExpectedAmount).HasColumnType("money");

                entity.Property(e => e.PercentDiscountAmount).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.SendDate).HasColumnType("date");

                entity.Property(e => e.SendEmailDate).HasColumnType("date");

                entity.Property(e => e.TypeCustomer).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });


            modelBuilder.Entity<RelateTaskMapping>(entity =>
            {
                entity.Property(e => e.RelateTaskMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CustomerCareCustomer>(entity =>
            {
                entity.Property(e => e.CustomerCareCustomerId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.CustomerCare)
                    .WithMany(p => p.CustomerCareCustomer)
                    .HasForeignKey(d => d.CustomerCareId)
                    .HasConstraintName("FK_CustomerCareCustomer_CustomerCare");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerCareCustomer)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_CustomerCareCustomer_Customer");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerCareFeedBack>(entity =>
            {
                entity.Property(e => e.CustomerCareFeedBackId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.FeedBackFromDate).HasColumnType("datetime");

                entity.Property(e => e.FeedBackToDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.CustomerCare)
                    .WithMany(p => p.CustomerCareFeedBack)
                    .HasForeignKey(d => d.CustomerCareId)
                    .HasConstraintName("FK_CustomerCareFeedBack_CustomerCare");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerCareFeedBack)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_CustomerCareFeedBack_Customer");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerCareFilter>(entity =>
            {
                entity.Property(e => e.CustomerCareFilterId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.CustomerCare)
                    .WithMany(p => p.CustomerCareFilter)
                    .HasForeignKey(d => d.CustomerCareId)
                    .HasConstraintName("FK_CustomerCareFilter_CustomerCare");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerMeeting>(entity =>
            {
                entity.Property(e => e.CustomerMeetingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerEmail).HasMaxLength(500);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Participants).IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.OrderId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ApprovedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerAddress).HasMaxLength(500);

                entity.Property(e => e.CustomerCode).HasMaxLength(250);

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CustomerNumber).HasMaxLength(250);

                entity.Property(e => e.CustomerPhone).HasMaxLength(50);

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ManufactureDate).HasColumnType("datetime");

                entity.Property(e => e.MaxDebt).HasColumnType("money");

                entity.Property(e => e.OrderCode).HasMaxLength(20);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PercentAdvance).HasColumnType("money");

                entity.Property(e => e.PercentAdvanceType)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ReasonCancel).HasMaxLength(50);

                entity.Property(e => e.ReceiptInvoiceAmount).HasColumnType("money");

                entity.Property(e => e.ReceivedDate).HasColumnType("date");

                entity.Property(e => e.RecipientEmail).HasMaxLength(50);

                entity.Property(e => e.RecipientName).HasMaxLength(250);

                entity.Property(e => e.RecipientPhone).HasMaxLength(50);

                entity.Property(e => e.StatusId).HasDefaultValueSql("(N'BOOKING')");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 6)");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.CustomerOrder)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__CustomerO__Statu__013F142A");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerOrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);

                entity.Property(e => e.OrderDetailId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.GuaranteeDatetime).HasColumnType("datetime");

                entity.Property(e => e.IncurredUnit).HasMaxLength(50);

                entity.Property(e => e.PriceInitial).HasColumnType("money");

                entity.Property(e => e.ProductCode).HasMaxLength(250);

                entity.Property(e => e.ProductColor).HasMaxLength(300);

                entity.Property(e => e.ProductColorCode).HasMaxLength(300);

                entity.Property(e => e.ProductGroupCode).HasMaxLength(255);

                entity.Property(e => e.ProductName).HasMaxLength(500);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitLaborPrice).HasColumnType("money");

                entity.Property(e => e.UnitName).HasMaxLength(250);

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 6)");

                entity.Property(e => e.VendorId).HasColumnName("VendorID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.CustomerOrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerOrderLocalPointMapping>(entity =>
            {
                entity.Property(e => e.CustomerOrderLocalPointMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerServiceLevel>(entity =>
            {
                entity.Property(e => e.CustomerServiceLevelId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerServiceLevelCode).HasMaxLength(20);

                entity.Property(e => e.CustomerServiceLevelName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.Property(e => e.DistrictId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DistrictCode).HasMaxLength(5);

                entity.Property(e => e.DistrictName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DistrictType).HasMaxLength(20);

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.District)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_District_Province");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.DocumentId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Extension).HasMaxLength(10);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.Property(e => e.EmailTemplateId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailTemplateName).HasMaxLength(100);

                entity.Property(e => e.EmailTemplateTitle).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmailTemplateCcvalue>(entity =>
            {
                entity.ToTable("EmailTemplateCCValue");

                entity.Property(e => e.EmailTemplateCcvalueId)
                    .HasColumnName("EmailTemplateCCValueId")
                    .ValueGeneratedNever();

                entity.Property(e => e.Ccto)
                    .HasColumnName("CCTo")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmailTemplateToken>(entity =>
            {
                entity.Property(e => e.EmailTemplateTokenId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TokenCode).HasMaxLength(30);

                entity.Property(e => e.TokenLabel).HasMaxLength(30);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.EmployeeId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BienSo).HasMaxLength(255);

                entity.Property(e => e.ChiPhiTheoGio).HasColumnType("money");

                entity.Property(e => e.ChuyenNganhHoc).HasMaxLength(255);

                entity.Property(e => e.CodeMayChamCong).HasMaxLength(255);

                entity.Property(e => e.ContractEndDate).HasColumnType("date");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DanToc).HasMaxLength(255);

                entity.Property(e => e.DiaDiemLamviec).HasMaxLength(255);

                entity.Property(e => e.DiemTest).HasMaxLength(255);

                entity.Property(e => e.EmployeeCode).HasMaxLength(50);

                entity.Property(e => e.EmployeeName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.GradeTesting).HasMaxLength(255);

                entity.Property(e => e.HoTenTiengAnh).HasMaxLength(255);

                entity.Property(e => e.IsXacNhanTaiLieu)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LoaiChuyenNganhHoc).HasMaxLength(2);

                entity.Property(e => e.LoaiXe).HasMaxLength(255);

                entity.Property(e => e.MaSoThueCaNhan).HasMaxLength(255);

                entity.Property(e => e.MaTest).HasMaxLength(255);

                entity.Property(e => e.NgayHenNop).HasColumnType("datetime");

                entity.Property(e => e.NgayNghiViec).HasColumnType("datetime");

                entity.Property(e => e.NgayNop).HasColumnType("datetime");

                entity.Property(e => e.ProbationEndDate).HasColumnType("datetime");

                entity.Property(e => e.ProbationStartDate).HasColumnType("datetime");

                entity.Property(e => e.QuocTich).HasMaxLength(255);

                entity.Property(e => e.StartDateMayChamCong).HasColumnType("datetime");

                entity.Property(e => e.StartedDate).HasColumnType("datetime");

                entity.Property(e => e.TenMayChamCong).HasMaxLength(255);

                entity.Property(e => e.TenTruongHocCaoNhat).HasMaxLength(255);

                entity.Property(e => e.ThangNopDangKyGiamTru).HasColumnType("datetime");

                entity.Property(e => e.TomTatHocVan).HasMaxLength(4000);

                entity.Property(e => e.TonGiao).HasMaxLength(255);

                entity.Property(e => e.TrainingStartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.ViTriLamViec).HasMaxLength(255);

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Orgranization_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeAllowance>(entity =>
            {
                entity.Property(e => e.EmployeeAllowanceId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAllowance)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_AllowanceSalary_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeAssessment>(entity =>
            {
                entity.Property(e => e.EmployeeAssessmentId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAssessment)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeAssessment_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeInsurance>(entity =>
            {
                entity.Property(e => e.EmployeeInsuranceId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.HealthInsurancePercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.HealthInsuranceSupportPercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.SocialInsurancePercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.SocialInsuranceSupportPercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.UnemploymentinsurancePercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.UnemploymentinsuranceSupportPercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeInsurance)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeInsurance_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeMonthySalary>(entity =>
            {
                entity.Property(e => e.EmployeeMonthySalaryId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ActualWorkingDay).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.BankAccountCode).HasMaxLength(500);

                entity.Property(e => e.BankAccountName).HasMaxLength(500);

                entity.Property(e => e.BranchOfBank).HasMaxLength(500);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.EmployeeBranch).HasMaxLength(500);

                entity.Property(e => e.EmployeeCode).HasMaxLength(50);

                entity.Property(e => e.EmployeeName).HasMaxLength(500);

                entity.Property(e => e.EmployeeUnit).HasMaxLength(500);

                entity.Property(e => e.Overtime).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.PostionName).HasMaxLength(500);

                entity.Property(e => e.UnPaidLeaveDay).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.VacationDay).HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeMonthySalary)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeMonthSalary_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeRequest>(entity =>
            {
                entity.Property(e => e.EmployeeRequestId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreateEmployeeCode).HasMaxLength(50);

                entity.Property(e => e.EmployeeRequestCode).HasMaxLength(50);

                entity.Property(e => e.EnDate).HasColumnType("datetime");

                entity.Property(e => e.OfferEmployeeCode).HasMaxLength(50);

                entity.Property(e => e.RequestDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.OfferEmployee)
                    .WithMany(p => p.EmployeeRequest)
                    .HasForeignKey(d => d.OfferEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeRequest_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeSalary>(entity =>
            {
                entity.Property(e => e.EmployeeSalaryId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.EmployeeSalaryBase).HasColumnType("money");

                entity.Property(e => e.ResponsibilitySalary).HasColumnType("money");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeSalary)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_BasedSalary_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeTimesheet>(entity =>
            {
                entity.Property(e => e.EmployeeTimesheetId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ActualWorkingDay).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Center).HasMaxLength(500);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Overtime).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.TimesheetDate).HasColumnType("date");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeTimesheet)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeTimesheet_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<FeatureNote>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Note).IsRequired();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<FeatureWorkFlowProgress>(entity =>
            {
                entity.Property(e => e.FeatureWorkflowProgressId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ChangeStepDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.ProgressStatus).HasMaxLength(250);

                entity.Property(e => e.RecordStatus).HasMaxLength(50);

                entity.Property(e => e.SystemFeatureCode).HasMaxLength(50);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<FileInFolder>(entity =>
            {
                entity.Property(e => e.FileInFolderId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FileExtension)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ObjectType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Size)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.Property(e => e.FolderId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Url).HasColumnName("URL");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InterviewSchedule>(entity =>
            {
                entity.Property(e => e.InterviewScheduleId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.InterviewDate).HasColumnType("datetime");

                entity.Property(e => e.InterviewTitle).IsRequired();

                entity.Property(e => e.TypeOfInterview)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CandidateVacanciesMapping>(entity =>
            {
                entity.HasKey(e => e.CandidateId);

                entity.Property(e => e.CandidateId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<GeographicalArea>(entity =>
            {
                entity.Property(e => e.GeographicalAreaId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.GeographicalAreaCode).HasMaxLength(500);

                entity.Property(e => e.GeographicalAreaName).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Groups>(entity =>
            {
                entity.HasKey(e => e.GroupId);

                entity.Property(e => e.GroupId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<GroupUser>(entity =>
            {
                entity.Property(e => e.GroupUserId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("date");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupUser)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GroupUser_Group");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.GroupUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GroupUser_User");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InforScreen>(entity =>
            {
                entity.Property(e => e.InforScreenId).ValueGeneratedNever();

                entity.Property(e => e.InforScreenCode)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.InforScreenName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.Property(e => e.InventoryId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.InventoryDetail).HasMaxLength(500);

                entity.Property(e => e.InventoryTotalAmount).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.ProductService)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InventoryStatusNavigation)
                    .WithMany(p => p.Inventory)
                    .HasForeignKey(d => d.InventoryStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductSe__Produ__11558062");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Inventory)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductSe__Vendo__7F36D027");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryDeliveryVoucher>(entity =>
            {
                entity.Property(e => e.InventoryDeliveryVoucherId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.InventoryDeliveryVoucherCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.InventoryDeliveryVoucherDate).HasColumnType("datetime");

                entity.Property(e => e.Reason).HasMaxLength(500);

                entity.Property(e => e.Receiver).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryDeliveryVoucherMapping>(entity =>
            {
                entity.Property(e => e.InventoryDeliveryVoucherMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(17, 4)");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PriceProduct).HasColumnType("money");

                entity.Property(e => e.QuantityActual).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantityInventory).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantityRequest).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantityReservation).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat).HasColumnType("decimal(10, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryDeliveryVoucherSerialMapping>(entity =>
            {
                entity.Property(e => e.InventoryDeliveryVoucherSerialMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryDetail>(entity =>
            {
                entity.Property(e => e.InventoryDetailId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.InventoryDetailInputDate).HasColumnType("datetime");

                entity.Property(e => e.InventoryDetailOutputDate).HasColumnType("datetime");

                entity.Property(e => e.InventoryDetailProductPrice).HasColumnType("money");

                entity.Property(e => e.InventoryDetailProductQuantity).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InventoryDetailProduct)
                    .WithMany(p => p.InventoryDetail)
                    .HasForeignKey(d => d.InventoryDetailProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InventoryDetail_Product");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.InventoryDetailNavigation)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InventoryDetail_Inventory");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryReceivingVoucher>(entity =>
            {
                entity.Property(e => e.InventoryReceivingVoucherId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.ExpectedDate).HasColumnType("datetime");

                entity.Property(e => e.InventoryReceivingVoucherCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.InventoryReceivingVoucherDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(1000);

                entity.Property(e => e.ShiperName).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryReceivingVoucherMapping>(entity =>
            {
                entity.Property(e => e.InventoryReceivingVoucherMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(17, 4)");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PriceProduct).HasColumnType("money");

                entity.Property(e => e.QuantityActual).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantityRequest).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantityReservation).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantitySerial).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat).HasColumnType("decimal(10, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryReceivingVoucherSerialMapping>(entity =>
            {
                entity.Property(e => e.InventoryReceivingVoucherSerialMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryReport>(entity =>
            {
                entity.Property(e => e.InventoryReportId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.OpeningBalance).HasColumnType("money");

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantityMaximum).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantityMinimum).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.StartQuantity).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Invoices>(entity =>
            {
                entity.HasKey(e => e.InvoiceId);

                entity.Property(e => e.InvoiceId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Amount).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.InvoiceDate).HasColumnType("datetime");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Tax).HasColumnType("decimal(2, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Lead>(entity =>
            {
                entity.Property(e => e.LeadId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedById)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpectedSale).HasColumnType("money");

                entity.Property(e => e.ForecastSales).HasColumnType("money");

                entity.Property(e => e.LeadCode).HasMaxLength(30);

                entity.Property(e => e.Role).HasMaxLength(50);

                entity.Property(e => e.UpdatedById).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Lead)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Lead_Company");

                entity.HasOne(d => d.InterestedGroup)
                    .WithMany(p => p.LeadInterestedGroup)
                    .HasForeignKey(d => d.InterestedGroupId)
                    .HasConstraintName("FK_Lead_GroupProduct");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.LeadPaymentMethod)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .HasConstraintName("FK_Lead_PaymentMethod");

                entity.HasOne(d => d.Potential)
                    .WithMany(p => p.LeadPotential)
                    .HasForeignKey(d => d.PotentialId)
                    .HasConstraintName("FK_Lead_Potential");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.LeadStatus)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lead_Status");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadCare>(entity =>
            {
                entity.Property(e => e.LeadCareId).ValueGeneratedNever();

                entity.Property(e => e.ActiveDate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountAmount).HasColumnType("money");

                entity.Property(e => e.EffecttiveFromDate).HasColumnType("datetime");

                entity.Property(e => e.EffecttiveToDate).HasColumnType("datetime");

                entity.Property(e => e.LeadCareCode).HasMaxLength(50);

                entity.Property(e => e.LeadCareContentSms).HasColumnName("LeadCareContentSMS");

                entity.Property(e => e.LeadCareVoucher)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PercentDiscountAmount).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.SendDate).HasColumnType("date");

                entity.Property(e => e.SendEmailDate).HasColumnType("date");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadCareFeedBack>(entity =>
            {
                entity.Property(e => e.LeadCareFeedBackId).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.FeedBackFromDate).HasColumnType("datetime");

                entity.Property(e => e.FeedBackToDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadCareFilter>(entity =>
            {
                entity.Property(e => e.LeadCareFilterId).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadCareLead>(entity =>
            {
                entity.Property(e => e.LeadCareLeadId).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadDetail>(entity =>
            {
                entity.Property(e => e.LeadDetailId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.IncurredUnit).HasMaxLength(50);

                entity.Property(e => e.ProductName).HasMaxLength(200);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitLaborPrice).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 6)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadInterestedGroupMapping>(entity =>
            {
                entity.Property(e => e.LeadInterestedGroupMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadMeeting>(entity =>
            {
                entity.Property(e => e.LeadMeetingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Participant).IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadProductDetailProductAttributeValue>(entity =>
            {
                entity.HasKey(e => e.LeadProductDetailProductAttributeValue1);

                entity.Property(e => e.LeadProductDetailProductAttributeValue1)
                    .HasColumnName("LeadProductDetailProductAttributeValue")
                    .ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LinkOfDocument>(entity =>
            {
                entity.Property(e => e.LinkOfDocumentId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LocalAddress>(entity =>
            {
                entity.Property(e => e.LocalAddressId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.LocalAddressCode).HasMaxLength(100);

                entity.Property(e => e.LocalAddressName).HasMaxLength(100);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LocalPoint>(entity =>
            {
                entity.Property(e => e.LocalPointId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.LocalPointCode).HasMaxLength(100);

                entity.Property(e => e.LocalPointName).HasMaxLength(100);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LoginAuditTrace>(entity =>
            {
                entity.Property(e => e.LoginAuditTraceId).ValueGeneratedNever();

                entity.Property(e => e.LoginDate).HasColumnType("datetime");

                entity.Property(e => e.LogoutDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MenuBuild>(entity =>
            {
                entity.Property(e => e.MenuBuildId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CodeParent)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IsShow)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.NameIcon).HasMaxLength(100);

                entity.Property(e => e.Path).HasMaxLength(2000);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.Property(e => e.NoteId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.NoteTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ObjectType)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('LD')");

                entity.Property(e => e.Type)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NoteDocument>(entity =>
            {
                entity.Property(e => e.NoteDocumentId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentName).HasMaxLength(100);

                entity.Property(e => e.DocumentSize).HasMaxLength(20);

                entity.Property(e => e.DocumentUrl).HasMaxLength(300);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Note)
                    .WithMany(p => p.NoteDocument)
                    .HasForeignKey(d => d.NoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NoteDocum__NoteI__4FBCC72F");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiAction>(entity =>
            {
                entity.Property(e => e.NotifiActionId).ValueGeneratedNever();

                entity.Property(e => e.NotifiActionCode)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.NotifiActionName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Notifications>(entity =>
            {
                entity.HasKey(e => e.NotificationId);

                entity.Property(e => e.NotificationId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.NotificationType)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Notificat__Recei__29E1370A");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CandidateAssessmentDetail>(entity =>
            {
                entity.Property(e => e.CandidateAssessmentDetailId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<NotifiCondition>(entity =>
            {
                entity.Property(e => e.NotifiConditionId).ValueGeneratedNever();

                entity.Property(e => e.NotifiConditionName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiSetting>(entity =>
            {
                entity.Property(e => e.NotifiSettingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerEmailTitle).HasMaxLength(1000);

                entity.Property(e => e.CustomerSmsTitle).HasMaxLength(1000);

                entity.Property(e => e.EmailTitle).HasMaxLength(1000);

                entity.Property(e => e.NotifiSettingName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.SmsTitle).HasMaxLength(1000);

                entity.Property(e => e.SystemTitle).HasMaxLength(1000);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiSettingCondition>(entity =>
            {
                entity.Property(e => e.NotifiSettingConditionId).ValueGeneratedNever();

                entity.Property(e => e.DateValue).HasColumnType("datetime");

                entity.Property(e => e.NumberValue).HasColumnType("decimal(18, 4)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiSettingToken>(entity =>
            {
                entity.Property(e => e.NotifiSettingTokenId).ValueGeneratedNever();

                entity.Property(e => e.TokenCode).HasMaxLength(100);

                entity.Property(e => e.TokenLabel).HasMaxLength(100);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiSpecial>(entity =>
            {
                entity.Property(e => e.NotifiSpecialId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OrderCostDetail>(entity =>
            {
                entity.Property(e => e.OrderCostDetailId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CostName).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderCostDetail)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderCostDetail_Order");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OrderProductDetailProductAttributeValue>(entity =>
            {
                entity.Property(e => e.OrderProductDetailProductAttributeValueId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.OrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetailProductAttributeValue_CustomerOrderDetail");

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.OrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetailProductAttributeValue_ProductAttributeCategory");

                entity.HasOne(d => d.ProductAttributeCategoryValue)
                    .WithMany(p => p.OrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryValueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetailProductAttributeValue_ProductAttributeCategoryValue");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetailProductAttributeValue_Product");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.Property(e => e.OrderStatusId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.OrderStatusCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RecruitmentCampaign>(entity =>
            {
                entity.Property(e => e.RecruitmentCampaignId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDateDate).HasColumnType("datetime");

                entity.Property(e => e.RecruitmentCampaignName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.OrganizationId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.IsAccess).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsHr)
                    .HasColumnName("IsHR")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrganizationCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.OrganizationName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.OrganizationOtherCode).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Department_Department");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });


            modelBuilder.Entity<PayableInvoice>(entity =>
            {
                entity.Property(e => e.PayableInvoiceId).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AmountText).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.PaidDate).HasColumnType("datetime");

                entity.Property(e => e.PayableInvoiceCode).HasMaxLength(50);

                entity.Property(e => e.PayableInvoiceDetail).HasMaxLength(500);

                entity.Property(e => e.PayableInvoicePrice).HasColumnType("money");

                entity.Property(e => e.RecipientName).HasMaxLength(250);

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VouchersDate).HasColumnType("datetime");

                entity.HasOne(d => d.CurrencyUnitNavigation)
                    .WithMany(p => p.PayableInvoiceCurrencyUnitNavigation)
                    .HasForeignKey(d => d.CurrencyUnit)
                    .HasConstraintName("FK__PayableIn__Curre__22A007F5");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.PayableInvoice)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK__PayableIn__Organ__3489AE06");

                entity.HasOne(d => d.PayableInvoicePriceCurrencyNavigation)
                    .WithMany(p => p.PayableInvoicePayableInvoicePriceCurrencyNavigation)
                    .HasForeignKey(d => d.PayableInvoicePriceCurrency)
                    .HasConstraintName("FK__PayableInvoice_Category");

                entity.HasOne(d => d.PayableInvoiceReasonNavigation)
                    .WithMany(p => p.PayableInvoicePayableInvoiceReasonNavigation)
                    .HasForeignKey(d => d.PayableInvoiceReason)
                    .HasConstraintName("FK__PayableIn__Payab__1ECF7711");

                entity.HasOne(d => d.RegisterTypeNavigation)
                    .WithMany(p => p.PayableInvoiceRegisterTypeNavigation)
                    .HasForeignKey(d => d.RegisterType)
                    .HasConstraintName("FK__PayableIn__Regis__1FC39B4A");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.PayableInvoiceStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__PayableIn__Statu__21ABE3BC");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PayableInvoiceMapping>(entity =>
            {
                entity.Property(e => e.PayableInvoiceMappingId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.PayableInvoice)
                    .WithMany(p => p.PayableInvoiceMapping)
                    .HasForeignKey(d => d.PayableInvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PayableIn__Payab__257C74A0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.PermissionId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.IconClass).HasMaxLength(200);

                entity.Property(e => e.PermissionCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PermissionName).IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PermissionMapping>(entity =>
            {
                entity.Property(e => e.PermissionMappingId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UseFor)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.PermissionMapping)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_PermissionMapping_Group");

                entity.HasOne(d => d.PermissionSet)
                    .WithMany(p => p.PermissionMapping)
                    .HasForeignKey(d => d.PermissionSetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermisisonSetMapping");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PermissionMapping)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_PermissionMapping_User");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PermissionSet>(entity =>
            {
                entity.Property(e => e.PermissionSetId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PermissionSetCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.PermissionSetDescription)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PermissionSetName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(e => e.PositionId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PositionCode).HasMaxLength(50);

                entity.Property(e => e.PositionName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Position_Position");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PotentialCustomerProduct>(entity =>
            {
                entity.Property(e => e.PotentialCustomerProductId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductFixedPrice).HasColumnType("money");

                entity.Property(e => e.ProductName).HasMaxLength(100);

                entity.Property(e => e.ProductUnit).HasMaxLength(20);

                entity.Property(e => e.ProductUnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PriceProduct>(entity =>
            {
                entity.Property(e => e.PriceProductId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.MinQuantity)
                    .HasColumnType("decimal(12, 6)")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NgayHetHan).HasColumnType("datetime");

                entity.Property(e => e.PriceForeignMoney).HasColumnType("money");

                entity.Property(e => e.PriceVnd)
                    .HasColumnName("PriceVND")
                    .HasColumnType("money");

                entity.Property(e => e.TiLeChietKhau).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PriceSuggestedSupplierQuotesMapping>(entity =>
            {
                entity.Property(e => e.PriceSuggestedSupplierQuotesMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProcurementPlan>(entity =>
            {
                entity.Property(e => e.ProcurementPlanId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ProcurementAmount).HasColumnType("money");

                entity.Property(e => e.ProcurementPlanCode).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.ProcurementPlan)
                    .HasForeignKey(d => d.ProductCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Procureme__Produ__3B0BC30C");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProcurementRequest>(entity =>
            {
                entity.Property(e => e.ProcurementRequestId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmployeePhone).HasMaxLength(50);

                entity.Property(e => e.ProcurementCode).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Approver)
                    .WithMany(p => p.ProcurementRequestApprover)
                    .HasForeignKey(d => d.ApproverId)
                    .HasConstraintName("FK_ProcurementRequest_EmployeeApprove");

                entity.HasOne(d => d.RequestEmployee)
                    .WithMany(p => p.ProcurementRequestRequestEmployee)
                    .HasForeignKey(d => d.RequestEmployeeId)
                    .HasConstraintName("FK_ProcurementRequest_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProcurementRequestItem>(entity =>
            {
                entity.Property(e => e.ProcurementRequestItemId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.IncurredUnit).HasMaxLength(200);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.QuantityApproval).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 6)");

                entity.HasOne(d => d.ProcurementPlan)
                    .WithMany(p => p.ProcurementRequestItem)
                    .HasForeignKey(d => d.ProcurementPlanId)
                    .HasConstraintName("FK_ProcurementRequestItem_ProcurementPlan");

                entity.HasOne(d => d.ProcurementRequest)
                    .WithMany(p => p.ProcurementRequestItem)
                    .HasForeignKey(d => d.ProcurementRequestId)
                    .HasConstraintName("FK_ProcurementRequestItem_ProcurementRequest");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProcurementRequestItem)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProcurementRequestItem_Product");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.ProcurementRequestItem)
                    .HasForeignKey(d => d.VendorId)
                    .HasConstraintName("FK_ProcurementRequestItem_Vendor");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExWarehousePrice).HasColumnType("money");

                entity.Property(e => e.GuaranteeDatetime).HasColumnType("datetime");

                entity.Property(e => e.ImportTax).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.MinimumInventoryQuantity).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.Price1).HasColumnType("money");

                entity.Property(e => e.Price2).HasColumnType("money");

                entity.Property(e => e.ProductCode).HasMaxLength(25);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 2)");

                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.ProductCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_ProductCategory");

                entity.HasOne(d => d.ProductNavigation)
                    .WithOne(p => p.InverseProductNavigation)
                    .HasForeignKey<Product>(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Product");

                entity.HasOne(d => d.ProductMoneyUnit)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.ProductMoneyUnitId)
                    .HasConstraintName("FK__Product__Category");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.Property(e => e.ProductAttributeId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.ProductAttribute)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductAttribute__ProductAttributeCategory");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductAttribute)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductAttribute__Product");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductAttributeCategory>(entity =>
            {
                entity.Property(e => e.ProductAttributeCategoryId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductAttributeCategoryName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductAttributeCategoryValue>(entity =>
            {
                entity.Property(e => e.ProductAttributeCategoryValueId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductAttributeCategoryValue1)
                    .IsRequired()
                    .HasColumnName("ProductAttributeCategoryValue")
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.ProductAttributeCategoryValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductAt__Produ__467D75B8");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductBillOfMaterials>(entity =>
            {
                entity.HasKey(e => e.ProductBillOfMaterialId);

                entity.Property(e => e.ProductBillOfMaterialId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EffectiveFromDate).HasColumnType("datetime");

                entity.Property(e => e.EffectiveToDate).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.Property(e => e.ProductCategoryId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductCategoryCode).HasMaxLength(500);

                entity.Property(e => e.ProductCategoryName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_ProductCategory_ProductCategory");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.Property(e => e.ProductImageId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ImageName).HasMaxLength(100);

                entity.Property(e => e.ImageSize).HasMaxLength(20);

                entity.Property(e => e.ImageUrl).HasMaxLength(300);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductVendorMapping>(entity =>
            {
                entity.Property(e => e.ProductVendorMappingId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.MiniumQuantity).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ToDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VendorProductCode).HasMaxLength(250);

                entity.Property(e => e.VendorProductName).HasMaxLength(250);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductVendorMapping)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductVendorMapping__Product");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.ProductVendorMapping)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductVendorMapping__Vendor");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OverviewCandidate>(entity =>
            {
                entity.Property(e => e.OverviewCandidateId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EducationAndWorkExpName).IsRequired();

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Type).HasMaxLength(20);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.ProjectId).ValueGeneratedNever();

                entity.Property(e => e.ActualEndDate).HasColumnType("datetime");

                entity.Property(e => e.ActualStartDate).HasColumnType("datetime");

                entity.Property(e => e.BudgetNgayCong).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.BudgetUsd)
                    .HasColumnName("BudgetUSD")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.BudgetVnd)
                    .HasColumnName("BudgetVND")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Butget).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.GiaBanTheoGio).HasColumnType("money");

                entity.Property(e => e.LastChangeActivityDate).HasColumnType("datetime");

                entity.Property(e => e.NgayKyNghiemThu).HasColumnType("datetime");

                entity.Property(e => e.ProjectCode).HasMaxLength(500);

                entity.Property(e => e.ProjectEndDate).HasColumnType("datetime");

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ProjectStartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectContractMapping>(entity =>
            {
                entity.HasKey(e => e.ProjectContractId);

                entity.Property(e => e.ProjectContractId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectCostReport>(entity =>
            {
                entity.Property(e => e.ProjectCostReportId).ValueGeneratedNever();

                entity.Property(e => e.Ac)
                    .HasColumnName("AC")
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Ev)
                    .HasColumnName("EV")
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Pv)
                    .HasColumnName("PV")
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ProjectEmployeeMapping>(entity =>
            {
                entity.HasKey(e => e.ProjectResourceMappingId);

                entity.Property(e => e.ProjectResourceMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InterviewScheduleMapping>(entity =>
            {
                entity.Property(e => e.InterviewScheduleMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectMilestone>(entity =>
            {
                entity.HasKey(e => e.ProjectMilestonesId);

                entity.Property(e => e.ProjectMilestonesId).ValueGeneratedNever();

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(500);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectObjective>(entity =>
            {
                entity.HasKey(e => e.ProjectObjectId);

                entity.Property(e => e.ProjectObjectId).ValueGeneratedNever();

                entity.Property(e => e.ProjectObjectName).HasMaxLength(500);

                entity.Property(e => e.ProjectObjectValue).HasColumnType("decimal(18, 0)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectResource>(entity =>
            {
                entity.Property(e => e.ProjectResourceId).ValueGeneratedNever();

                entity.Property(e => e.ChiPhiTheoGio).HasColumnType("money");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectScope>(entity =>
            {
                entity.Property(e => e.ProjectScopeId).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ProjectScopeCode).HasMaxLength(50);

                entity.Property(e => e.ProjectScopeName).HasMaxLength(250);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.HasKey(e => e.TaskId);

                entity.Property(e => e.TaskId).ValueGeneratedNever();

                entity.Property(e => e.ActualEndTime).HasColumnType("datetime");

                entity.Property(e => e.ActualHour).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ActualStartTime).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Hour).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.TaskCode).HasMaxLength(24);

                entity.Property(e => e.TaskName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.Property(e => e.PromotionId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.PromotionCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PromotionName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PromotionMapping>(entity =>
            {
                entity.Property(e => e.PromotionMappingId).ValueGeneratedNever();

                entity.Property(e => e.GiaTri).HasColumnType("money");

                entity.Property(e => e.SoLuongMua).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.SoLuongTang).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.SoTienTu).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PromotionObjectApply>(entity =>
            {
                entity.Property(e => e.PromotionObjectApplyId).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.GiaTri).HasColumnType("money");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SoLuongTang).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.SoTienTu).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PromotionObjectApplyMapping>(entity =>
            {
                entity.Property(e => e.PromotionObjectApplyMappingId).ValueGeneratedNever();

                entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PromotionProductMapping>(entity =>
            {
                entity.Property(e => e.PromotionProductMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.Property(e => e.ProvinceId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ProvinceCode).HasMaxLength(5);

                entity.Property(e => e.ProvinceName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProvinceType).HasMaxLength(20);
            });

            modelBuilder.Entity<PurchaseOrderStatus>(entity =>
            {
                entity.Property(e => e.PurchaseOrderStatusId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.PurchaseOrderStatusCode).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Queue>(entity =>
            {
                entity.Property(e => e.QueueId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Bcc)
                    .HasColumnName("BCC")
                    .HasMaxLength(500);

                entity.Property(e => e.Cc)
                    .HasColumnName("CC")
                    .HasMaxLength(500);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.FromTo).HasMaxLength(200);

                entity.Property(e => e.Method).HasMaxLength(5);

                entity.Property(e => e.SenDate).HasColumnType("datetime");

                entity.Property(e => e.SendTo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.Property(e => e.QuoteId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.ConstructionTime).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExpirationDate).HasColumnType("date");

                entity.Property(e => e.IntendedQuoteDate).HasColumnType("datetime");

                entity.Property(e => e.MaxDebt).HasColumnType("money");

                entity.Property(e => e.ObjectType).HasMaxLength(50);

                entity.Property(e => e.PercentAdvance).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.PercentAdvanceType)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.QuoteCode).HasMaxLength(20);

                entity.Property(e => e.QuoteDate).HasColumnType("datetime");

                entity.Property(e => e.ReceivedDate).HasColumnType("date");

                entity.Property(e => e.RecipientEmail).HasMaxLength(50);

                entity.Property(e => e.RecipientName).HasMaxLength(250);

                entity.Property(e => e.RecipientPhone).HasMaxLength(50);

                entity.Property(e => e.SendQuoteDate).HasColumnType("datetime");

                entity.Property(e => e.StatusId).HasDefaultValueSql("(N'BOOKING')");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 6)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteApproveDetailHistory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.IncurredUnit).HasMaxLength(50);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 6)");

                entity.Property(e => e.VendorId).HasColumnName("VendorID");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteApproveHistory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AmountPriceInitial).HasColumnType("money");

                entity.Property(e => e.AmountPriceProfit).HasColumnType("money");

                entity.Property(e => e.DiscountValue).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.QuoteCode).HasMaxLength(20);

                entity.Property(e => e.SendApproveDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteCostDetail>(entity =>
            {
                entity.Property(e => e.QuoteCostDetailId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CostName).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.QuoteCostDetail)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteCostDetail_Quote");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteDetail>(entity =>
            {
                entity.Property(e => e.QuoteDetailId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.IncurredUnit).HasMaxLength(50);

                entity.Property(e => e.PriceInitial).HasColumnType("money");

                entity.Property(e => e.ProductName).HasMaxLength(200);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitLaborPrice).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 6)");

                entity.Property(e => e.VendorId).HasColumnName("VendorID");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.QuoteDetail)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteDetail_Quote");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteDocument>(entity =>
            {
                entity.Property(e => e.QuoteDocumentId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentName).HasMaxLength(100);

                entity.Property(e => e.DocumentSize).HasMaxLength(20);

                entity.Property(e => e.DocumentUrl).HasMaxLength(300);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.QuoteDocument)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuoteDocum__QuoteI__4FBCC72F");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteParticipantMapping>(entity =>
            {
                entity.Property(e => e.QuoteParticipantMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuotePaymentTerm>(entity =>
            {
                entity.HasKey(e => e.PaymentTermId);

                entity.Property(e => e.PaymentTermId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VacanciesDocument>(entity =>
            {
                entity.Property(e => e.VacanciesDocumentId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Vacancies>(entity =>
            {
                entity.Property(e => e.VacanciesId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Currency).HasMaxLength(20);

                entity.Property(e => e.SalaryFrom).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.SalaryTo).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VacanciesName).IsRequired();
            });

            modelBuilder.Entity<QuotePlan>(entity =>
            {
                entity.HasKey(e => e.PlanId);

                entity.Property(e => e.PlanId).ValueGeneratedNever();

                entity.Property(e => e.Tt).HasColumnName("TT");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteProductDetailProductAttributeValue>(entity =>
            {
                entity.Property(e => e.QuoteProductDetailProductAttributeValueId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.QuoteProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteProductDetailProductAttributeValue_ProductAttributeCategory");

                entity.HasOne(d => d.ProductAttributeCategoryValue)
                    .WithMany(p => p.QuoteProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryValueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteProductDetailProductAttributeValue_ProductAttributeCategoryValue1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.QuoteProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteProductDetailProductAttributeValue_Product");

                entity.HasOne(d => d.QuoteDetail)
                    .WithMany(p => p.QuoteProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.QuoteDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteProductDetailProductAttributeValue_QuoteDetail");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteScope>(entity =>
            {
                entity.HasKey(e => e.ScopeId);

                entity.Property(e => e.ScopeId)
                    .HasColumnName("scopeId")
                    .ValueGeneratedNever();

                entity.Property(e => e.Category).HasColumnName("category");

                //entity.Property(e => e.CreatedDate)
                //    .HasColumnName("createdDate")
                //    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.ParentId).HasColumnName("parentId");

                entity.Property(e => e.QuoteId).HasColumnName("quoteId");

                entity.Property(e => e.Tt)
                    .IsRequired()
                    .HasColumnName("TT")
                    .HasMaxLength(10);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ReceiptInvoice>(entity =>
            {
                entity.Property(e => e.ReceiptInvoiceId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AmountText).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.ReceiptDate).HasColumnType("datetime");

                entity.Property(e => e.ReceiptInvoiceCode).HasMaxLength(50);

                entity.Property(e => e.ReceiptInvoiceDetail).HasMaxLength(500);

                entity.Property(e => e.RecipientName).HasMaxLength(250);

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VouchersDate).HasColumnType("datetime");

                entity.HasOne(d => d.CurrencyUnitNavigation)
                    .WithMany(p => p.ReceiptInvoiceCurrencyUnitNavigation)
                    .HasForeignKey(d => d.CurrencyUnit)
                    .HasConstraintName("FK__ReceiptIn__Curre__47D18CA4");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ReceiptInvoice)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK__ReceiptIn__Organ__3B36AB95");

                entity.HasOne(d => d.ReceiptInvoiceReasonNavigation)
                    .WithMany(p => p.ReceiptInvoiceReceiptInvoiceReasonNavigation)
                    .HasForeignKey(d => d.ReceiptInvoiceReason)
                    .HasConstraintName("FK__ReceiptIn__Recei__4400FBC0");

                entity.HasOne(d => d.RegisterTypeNavigation)
                    .WithMany(p => p.ReceiptInvoiceRegisterTypeNavigation)
                    .HasForeignKey(d => d.RegisterType)
                    .HasConstraintName("FK__ReceiptIn__Regis__44F51FF9");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ReceiptInvoiceStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__ReceiptIn__Statu__46DD686B");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ReceiptInvoiceMapping>(entity =>
            {
                entity.Property(e => e.ReceiptInvoiceMappingId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ReceiptInvoice)
                    .WithMany(p => p.ReceiptInvoiceMapping)
                    .HasForeignKey(d => d.ReceiptInvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReceiptIn__Recei__4AADF94F");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CandidateAssessment>(entity =>
            {
                entity.Property(e => e.CandidateAssessmentId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            });
            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.Property(e => e.CandidateId).ValueGeneratedNever();

                entity.Property(e => e.ApplicationDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FullName).IsRequired();

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ReceiptOrderHistory>(entity =>
            {
                entity.Property(e => e.ReceiptOrderHistoryId).ValueGeneratedNever();

                entity.Property(e => e.AmountCollected).HasColumnType("money");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RequestPayment>(entity =>
            {
                entity.Property(e => e.RequestPaymentId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.RequestEmployeePhone).HasMaxLength(50);

                entity.Property(e => e.RequestPaymentCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RequestPaymentCreateDate).HasColumnType("date");

                entity.Property(e => e.RequestPaymentNote).HasMaxLength(500);

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.TotalAmount).HasColumnType("money");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.RoleValue).HasMaxLength(500);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RoleAndMenuBuild>(entity =>
            {
                entity.Property(e => e.RoleAndMenuBuildId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Path).HasMaxLength(2000);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RoleAndPermission>(entity =>
            {
                entity.Property(e => e.RoleAndPermissionId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SaleBidding>(entity =>
            {
                entity.Property(e => e.SaleBiddingId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.BidStartDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.FormOfBid)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.SaleBiddingCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SaleBiddingName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.ValueBid).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SaleBiddingDetail>(entity =>
            {
                entity.Property(e => e.SaleBiddingDetailId).ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SaleBiddingDetailProductAttribute>(entity =>
            {
                entity.Property(e => e.SaleBiddingDetailProductAttributeId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SaleBiddingEmployeeMapping>(entity =>
            {
                entity.Property(e => e.SaleBiddingEmployeeMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Satellite>(entity =>
            {
                entity.Property(e => e.SatelliteId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.SatelliteCode).HasMaxLength(500);

                entity.Property(e => e.SatelliteName).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Screen>(entity =>
            {
                entity.Property(e => e.ScreenId).ValueGeneratedNever();

                entity.Property(e => e.ScreenCode)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ScreenName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.Property(e => e.QuizId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.QuizName).IsRequired();

                entity.Property(e => e.Score).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });


            modelBuilder.Entity<Serial>(entity =>
            {
                entity.Property(e => e.SerialId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.SerialCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SoKho>(entity =>
            {
                entity.Property(e => e.SoKhoId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Gia).HasColumnType("money");

                entity.Property(e => e.NgayChungTu).HasColumnType("datetime");

                entity.Property(e => e.SoChungTu)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SoLuong).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.ThanhTien).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });
            modelBuilder.Entity<ExternalUser>(entity =>
            {
                entity.Property(e => e.ExternalUserId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.ResetCode).HasMaxLength(50);

                entity.Property(e => e.ResetCodeDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });


            modelBuilder.Entity<StockTake>(entity =>
            {
                entity.Property(e => e.StockTakeId).ValueGeneratedNever();

                entity.Property(e => e.BalanceDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.StockTakeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StockTakeDate).HasColumnType("datetime");

                entity.Property(e => e.TotalMoneyActual).HasColumnType("money");

                entity.Property(e => e.TotalMoneyDeflectionIncreases).HasColumnType("money");

                entity.Property(e => e.TotalMoneyDeviationDecreases).HasColumnType("money");

                entity.Property(e => e.TotalMoneyDeviationDeviation).HasColumnType("money");

                entity.Property(e => e.TotalQuantityActual).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TotalQuantityDeflectionIncreases).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TotalQuantityDeviation).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TotalQuantityDeviationDecreases).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<StockTakeProductMapping>(entity =>
            {
                entity.Property(e => e.StockTakeProductMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.PriceDeviation).HasColumnType("money");

                entity.Property(e => e.QuantityActual).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantityDeviation).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.QuantityInventory).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SuggestedSupplierQuotes>(entity =>
            {
                entity.HasKey(e => e.SuggestedSupplierQuoteId);

                entity.Property(e => e.SuggestedSupplierQuoteId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ObjectType).HasMaxLength(50);

                entity.Property(e => e.QuoteTermDate).HasColumnType("datetime");

                entity.Property(e => e.RecommendedDate).HasColumnType("datetime");

                entity.Property(e => e.SuggestedSupplierQuote)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SuggestedSupplierQuotesDetail>(entity =>
            {
                entity.HasKey(e => e.SuggestedSupplierQuoteDetailId);

                entity.Property(e => e.SuggestedSupplierQuoteDetailId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SystemFeature>(entity =>
            {
                entity.Property(e => e.SystemFeatureId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.SystemFeatureCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SystemFeatureName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SystemParameter>(entity =>
            {
                entity.Property(e => e.SystemParameterId).ValueGeneratedNever();

                entity.Property(e => e.SystemDescription).HasMaxLength(500);

                entity.Property(e => e.SystemGroupCode).HasMaxLength(20);

                entity.Property(e => e.SystemGroupDesc).HasMaxLength(100);

                entity.Property(e => e.SystemKey)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.TaskId).ValueGeneratedNever();

                entity.Property(e => e.ActualEndTime).HasColumnType("datetime");

                entity.Property(e => e.ActualHour).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ActualStartTime).HasColumnType("datetime");

                entity.Property(e => e.CompleteDate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EstimateCost).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.EstimateHour).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.PlanEndTime).HasColumnType("datetime");

                entity.Property(e => e.PlanStartTime).HasColumnType("datetime");

                entity.Property(e => e.TaskCode).HasMaxLength(24);

                entity.Property(e => e.TaskName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaskConstraint>(entity =>
            {
                entity.Property(e => e.TaskConstraintId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DelayTime).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaskDocument>(entity =>
            {
                entity.Property(e => e.TaskDocumentId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentName).HasMaxLength(100);

                entity.Property(e => e.DocumentSize).HasMaxLength(20);

                entity.Property(e => e.DocumentUrl).HasMaxLength(300);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaskMilestonesMapping>(entity =>
            {
                entity.HasKey(e => e.ProjectMilestonesMappingId);

                entity.Property(e => e.ProjectMilestonesMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaskResourceMapping>(entity =>
            {
                entity.Property(e => e.TaskResourceMappingId).ValueGeneratedNever();

                entity.Property(e => e.Hours).HasColumnType("decimal(18, 4)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.Property(e => e.TemplateId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.TemplateContent).IsRequired();

                entity.Property(e => e.TemplateTitle)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Tenants>(entity =>
            {
                entity.HasKey(e => e.TenantId);

                entity.Property(e => e.TenantId).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenantHost)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TenantMode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenantName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TermsOfPayment>(entity =>
            {
                entity.Property(e => e.TermsOfPaymentId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TimeSheet>(entity =>
            {
                entity.Property(e => e.TimeSheetId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.SpentHour).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.ToDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TimeSheetDetail>(entity =>
            {
                entity.Property(e => e.TimeSheetDetailId).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.SpentHour).HasColumnType("decimal(18, 6)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.ResetCode).HasMaxLength(50);

                entity.Property(e => e.ResetCodeDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_User_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.UserRoleId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.Property(e => e.VendorId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.NearestDateTransaction).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VendorCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.VendorName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.VendorPayment)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Vendor__Payment__76A18A26");

                entity.HasOne(d => d.VendorGroup)
                    .WithMany(p => p.VendorVendorGroup)
                    .HasForeignKey(d => d.VendorGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Vendor__VendorGr__75AD65ED");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrder>(entity =>
            {
                entity.Property(e => e.VendorOrderId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ReceivedDate).HasColumnType("date");

                entity.Property(e => e.RecipientEmail).HasMaxLength(50);

                entity.Property(e => e.RecipientName).HasMaxLength(250);

                entity.Property(e => e.RecipientPhone).HasMaxLength(50);

                entity.Property(e => e.ShipperName).HasMaxLength(300);

                entity.Property(e => e.TypeCost).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VendorOrderCode).HasMaxLength(20);

                entity.Property(e => e.VendorOrderDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrderCostDetail>(entity =>
            {
                entity.Property(e => e.VendorOrderCostDetailId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CostName).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.VendorOrder)
                    .WithMany(p => p.VendorOrderCostDetail)
                    .HasForeignKey(d => d.VendorOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderCostDetail_VendorOrder");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrderDetail>(entity =>
            {
                entity.Property(e => e.VendorOrderDetailId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Cost).HasColumnType("money");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.IncurredUnit).HasMaxLength(50);

                entity.Property(e => e.PriceValueWarehouse).HasColumnType("money");

                entity.Property(e => e.PriceWarehouse).HasColumnType("money");

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 6)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(12, 6)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.VendorOrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_VendorOrderDetail_Product");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.VendorOrderDetail)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK__VendorOrd__UnitI__71C7C670");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.VendorOrderDetail)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderDetail_Vendor");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrderProcurementRequestMapping>(entity =>
            {
                entity.Property(e => e.VendorOrderProcurementRequestMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrderProductDetailProductAttributeValue>(entity =>
            {
                entity.HasKey(e => e.OrderProductDetailProductAttributeValueId);

                entity.Property(e => e.OrderProductDetailProductAttributeValueId).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.VendorOrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderProductDetailProductAttributeValue_ProductAttributeCategory");

                entity.HasOne(d => d.ProductAttributeCategoryValue)
                    .WithMany(p => p.VendorOrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryValueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderProductDetailProductAttributeValue_ProductAttributeCategoryValue");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.VendorOrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderProductDetailProductAttributeValue_Product");

                entity.HasOne(d => d.VendorOrderDetail)
                    .WithMany(p => p.VendorOrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.VendorOrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderProductDetailProductAttributeValue_VendorOrderDetail");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.Property(e => e.WardId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.WardCode).HasMaxLength(10);

                entity.Property(e => e.WardName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.WardType).HasMaxLength(20);

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Ward)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ward_District");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.Property(e => e.WarehouseId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WarehouseAddress).HasMaxLength(500);

                entity.Property(e => e.WarehouseCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.WarehouseDescription).HasColumnType("text");

                entity.Property(e => e.WarehouseName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.WarehousePhone).HasMaxLength(50);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<WorkFlows>(entity =>
            {
                entity.HasKey(e => e.WorkFlowId);

                entity.Property(e => e.WorkFlowId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WorkflowCode).HasMaxLength(50);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<WorkFlowSteps>(entity =>
            {
                entity.HasKey(e => e.WorkflowStepId);

                entity.Property(e => e.WorkflowStepId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ApprovedText)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NotApprovedText).HasMaxLength(100);

                entity.Property(e => e.RecordStatus).HasMaxLength(50);

                entity.HasOne(d => d.Workflow)
                    .WithMany(p => p.WorkFlowSteps)
                    .HasForeignKey(d => d.WorkflowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkFlowSteps_WorkFlows");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OrderTechniqueMapping>(entity =>
            {
                entity.Property(e => e.OrderTechniqueMappingId).ValueGeneratedNever();

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Rate).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MinusItemMapping>(entity =>
            {
                entity.Property(e => e.MinusItemMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductionOrder>(entity =>
            {
                entity.Property(e => e.ProductionOrderId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerName).HasMaxLength(100);

                entity.Property(e => e.CustomerNumber).HasMaxLength(250);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Especially).HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductionOrderCode).HasMaxLength(100);

                entity.Property(e => e.ReceivedDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductionOrderHistory>(entity =>
            {
                entity.Property(e => e.ProductionOrderHistoryId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.IsError).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductionOrderMapping>(entity =>
            {
                entity.Property(e => e.ProductionOrderMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductCode).HasMaxLength(250);

                entity.Property(e => e.ProductColor).HasMaxLength(300);

                entity.Property(e => e.ProductColorCode).HasMaxLength(300);

                entity.Property(e => e.ProductGroupCode).HasMaxLength(255);

                entity.Property(e => e.ProductName).HasMaxLength(300);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductOrderWorkflow>(entity =>
            {
                entity.Property(e => e.ProductOrderWorkflowId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.IsDefault).HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RememberItem>(entity =>
            {
                entity.Property(e => e.RememberItemId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TechniqueRequest>(entity =>
            {
                entity.Property(e => e.TechniqueRequestId).ValueGeneratedNever();

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TechniqueName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TechniqueRequestCode).HasMaxLength(255);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TechniqueRequestMapping>(entity =>
            {
                entity.Property(e => e.TechniqueRequestMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TechniqueName).HasMaxLength(300);

                entity.Property(e => e.TechniqueValue).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TotalProductionOrder>(entity =>
            {
                entity.Property(e => e.TotalProductionOrderId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TotalProductionOrderMapping>(entity =>
            {
                entity.Property(e => e.TotalProductionOrderMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ThanhVienPhongBan>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuyTrinh>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MaQuyTrinh)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenQuyTrinh)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhQuyTrinh>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.LoaiCauHinh).HasDefaultValueSql("((1))");

                entity.Property(e => e.QuyTrinh).HasMaxLength(1000);

                entity.Property(e => e.SoTienTu).HasColumnType("money");

                entity.Property(e => e.TenCauHinh)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CacBuocQuyTrinh>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PhongBanTrongCacBuocQuyTrinh>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CacBuocApDung>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PhongBanApDung>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LichSuPheDuyet>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.LyDo).HasMaxLength(3000);

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.ObjectNumber).HasDefaultValueSql("((0))");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KichHoatTinhHuong>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.NoiDung)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Session).HasMaxLength(255);

                entity.Property(e => e.ThoiDiemKichHoat).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KichHoatTinhHuongChiTiet>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.HoVaTen).HasMaxLength(50);

                entity.Property(e => e.NoiDung)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PhanHoi).HasMaxLength(255);

                entity.Property(e => e.Sdt)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Session).HasMaxLength(255);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CaLamViec>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CaLamViecChiTiet>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhGiamTru>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MucGiamTru).HasColumnType("money");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhNghiLe>(entity =>
            {
                entity.HasKey(e => e.NghiLeId);

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhNghiLeChiTiet>(entity =>
            {
                entity.HasKey(e => e.NghiLeChiTietId);

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Ngay).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhOt>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TyLeHuong).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaiSan>(entity =>
            {
                entity.Property(e => e.TaiSanId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BaoDuongTaiSan>(entity =>
            {
                entity.Property(e => e.BaoDuongTaiSanId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ThongTinCapPhatTaiSan>(entity =>
            {
                entity.Property(e => e.ThongTinCapPhatTaiSanId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CapPhatTaiSan>(entity =>
            {
                entity.Property(e => e.CapPhatTaiSanId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.NamCauHinh).HasMaxLength(255);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhBaoHiem>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MucDong).HasColumnType("money");

                entity.Property(e => e.MucDongToiDa).HasColumnType("money");

                entity.Property(e => e.TiLePhanBoMucDongBhtncuaNld).HasColumnName("TiLePhanBoMucDongBHTNCuaNLD");

                entity.Property(e => e.TiLePhanBoMucDongBhtncuaNsdld).HasColumnName("TiLePhanBoMucDongBHTNCuaNSDLD");

                entity.Property(e => e.TiLePhanBoMucDongBhtnnncuaNld).HasColumnName("TiLePhanBoMucDongBHTNNNCuaNLD");

                entity.Property(e => e.TiLePhanBoMucDongBhtnnncuaNsdld).HasColumnName("TiLePhanBoMucDongBHTNNNCuaNSDLD");

                entity.Property(e => e.TiLePhanBoMucDongBhxhcuaNld).HasColumnName("TiLePhanBoMucDongBHXHCuaNLD");

                entity.Property(e => e.TiLePhanBoMucDongBhxhcuaNsdld).HasColumnName("TiLePhanBoMucDongBHXHCuaNSDLD");

                entity.Property(e => e.TiLePhanBoMucDongBhytcuaNld).HasColumnName("TiLePhanBoMucDongBHYTCuaNLD");

                entity.Property(e => e.TiLePhanBoMucDongBhytcuaNsdld).HasColumnName("TiLePhanBoMucDongBHYTCuaNSDLD");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DoiTuongPhuThuocMapping>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MucHuongBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.DoiTuongHuong).HasDefaultValueSql("((1))");

                entity.Property(e => e.LePhi).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MucGiam).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MucHuong).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PhiCoDinh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PhiTheoLuong).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaiLieuNhanVien>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NgayHen).HasColumnType("datetime");

                entity.Property(e => e.NgayNop).HasColumnType("datetime");

                entity.Property(e => e.TenTaiLieu).HasMaxLength(255);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KeHoachOt>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiaDiem).HasMaxLength(500);

                entity.Property(e => e.GhiChu).HasMaxLength(3000);

                entity.Property(e => e.HanDangKy).HasColumnType("datetime");

                entity.Property(e => e.HanPheDuyetDangKy).HasColumnType("datetime");

                entity.Property(e => e.HanPheDuyetKeHoach).HasColumnType("datetime");

                entity.Property(e => e.LyDo).HasMaxLength(200);

                entity.Property(e => e.NgayBatDau).HasColumnType("datetime");

                entity.Property(e => e.NgayDeXuat).HasColumnType("datetime");

                entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");

                entity.Property(e => e.TenKeHoach).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KeHoachOtThanhVien>(entity =>
            {
                entity.HasKey(e => e.ThanVienOtId);

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KeHoachOtPhongBan>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<YeuCauCapPhatTaiSan>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MaYeuCau)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NgayDeXuat).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<YeuCauCapPhatTaiSanChiTiet>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LyDo).HasMaxLength(500);

                entity.Property(e => e.NgayBatDau).HasColumnType("datetime");

                entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");

                entity.Property(e => e.SoLuong).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatCongTac>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiaDiem).HasMaxLength(100);

                entity.Property(e => e.DonVi).HasMaxLength(100);

                entity.Property(e => e.NgayDeXuat).HasColumnType("datetime");

                entity.Property(e => e.NhiemVu).HasMaxLength(200);

                entity.Property(e => e.PhuongTien).HasMaxLength(100);

                entity.Property(e => e.TenDeXuat)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ThoiGianBatDau).HasColumnType("datetime");

                entity.Property(e => e.ThoiGianKetThuc).HasColumnType("datetime");

                entity.Property(e => e.TrangThai).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChiTietDeXuatCongTac>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LyDo).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<HoSoCongTac>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KinhPhiCongDoan>(entity =>
            {
                entity.Property(e => e.MucDongCongTy).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MucDongNhanVien).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CongThucTinhLuong>(entity =>
            {
                entity.Property(e => e.CongThuc)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeNghiTamHoanUng>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeNghiTamHoanUngChiTiet>(entity =>
            {
                entity.Property(e => e.ChiPhiKhac)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DinhKemCt).HasColumnName("DinhKemCT");

                entity.Property(e => e.KhachSan)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NgayThang).HasColumnType("datetime");

                entity.Property(e => e.SoHoaDon).HasMaxLength(100);

                entity.Property(e => e.TienDonDn)
                    .HasColumnName("TienDonDN")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TienDonHnnb)
                    .HasColumnName("TienDonHNNB")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TienSauVat)
                    .HasColumnName("TienSauVAT")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TongTienTruocVat)
                    .HasColumnName("TongTienTruocVAT")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VanChuyenXm)
                    .HasColumnName("VanChuyenXM")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChucVuBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.SoNamKinhNghiem).HasDefaultValueSql("((0))");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MucDanhGia>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiemDanhGia)
                    .HasColumnType("decimal(18, 4)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NgayApDung).HasColumnType("datetime");

                entity.Property(e => e.TenMucDanhGia)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHoiMucDanhGiaMapping>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MucDanhGiaDanhGiaMapping>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DiemDen).HasDefaultValueSql("((0))");

                entity.Property(e => e.DiemTu)
                    .HasColumnType("decimal(18, 4)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MucDanhGiaId).HasColumnName("MucDanhGiaID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuyLuong>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");


                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.QuyLuong1)
                    .HasColumnName("QuyLuong")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChamCong>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.NgayChamCong).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ThongKeDiMuonVeSom>(entity =>
            {
                entity.Property(e => e.NgayDmvs).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PhieuDanhGia>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TenPhieuDanhGia).IsRequired();

                entity.Property(e => e.TrangThaiPhieuDanhGia).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHoiPhieuDanhGiaMapping>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.NoiDungCauHoi).HasMaxLength(2000);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhChecklist>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TenTaiLieu).HasMaxLength(255);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KyDanhGia>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.TenKyDanhGia)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ThoiGianBatDau).HasColumnType("datetime");

                entity.Property(e => e.ThoiGianKetThuc).HasColumnType("datetime");

                entity.Property(e => e.TrangThaiDanhGia).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatXinNghi>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LyDoTuChoi).HasMaxLength(3000);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatXinNghiChiTiet>(entity =>
            {
                entity.Property(e => e.Ngay).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhChamCongOt>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChamCongOt>(entity =>
            {
                entity.Property(e => e.NgayChamCong).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TongHopChamCongOt>(entity =>
            {
                entity.Property(e => e.NgayChamCong).HasColumnType("datetime");

                entity.Property(e => e.SoPhut).HasColumnType("decimal(18, 0)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhOtCaNgay>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhThueTncn>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });


            modelBuilder.Entity<NhanVienKyDanhGia>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.HasQueryFilter(e => e.TenantId == _tenantId);

            });


            modelBuilder.Entity<NoiDungKyDanhGia>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
                entity.HasQueryFilter(e => e.TenantId == _tenantId);

            });

            modelBuilder.Entity<DanhGiaNhanVien>(entity =>
            {
                entity.Property(e => e.MucLuongDeXuatQuanLy).HasColumnName("MucLuongDeXuat_QuanLy");

                entity.Property(e => e.MucLuongDeXuatTruongPhong).HasColumnName("MucLuongDeXuat_TruongPhong");

                entity.Property(e => e.NhanXetTruongPhong).HasColumnName("NhanXet_TruongPhong");
                entity.HasQueryFilter(e => e.TenantId == _tenantId);

            });

            modelBuilder.Entity<ChiTietDanhGiaNhanVien>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LichSuThanhToanBaoHiem>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TroCapDieuKienHuongMapping>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TroCapLoaiHopDongMapping>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MucHuongDmvs>(entity =>
            {
                entity.Property(e => e.MucTru).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoLanDen).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoLanTu).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuyenLoiBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NhomBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MucHuongTheoNgayNghi>(entity =>
            {
                entity.Property(e => e.MucHuongPhanTram).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoNgayDen).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoNgayTu).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KyLuong>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DenNgay).HasColumnType("date");

                entity.Property(e => e.LyDoTuChoi).HasMaxLength(3000);

                entity.Property(e => e.SoNgayLamViec).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TenKyLuong)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TuNgay).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtBaoHiem>(entity =>
            {
                entity.Property(e => e.BaseBhtn).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BaseBhxh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Bhtn).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Bhtnnn).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Bhxh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Bhyt).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Other).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtCtyDong>(entity =>
            {
                entity.Property(e => e.BaseBhtn).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BaseBhxh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Bhtn).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Bhtnnn).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Bhxh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Bhyt).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FundOct)
                    .HasColumnName("Fund_Oct")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.KinhPhiCongDoan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Other).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtGiamTruSauThue>(entity =>
            {
                entity.Property(e => e.KinhPhiCongDoan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Other).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.QuyetToanThueTncn).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtGiamTruTruocThue>(entity =>
            {
                entity.Property(e => e.GiamTruCaNhan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.GiamTruKhac).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.GiamTruNguoiPhuThuoc).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TienDongBaoHiem).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtHoanLaiSauThue>(entity =>
            {
                entity.Property(e => e.Other).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ThueTncn).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtLoaiTroCapCoDinh>(entity =>
            {
                entity.Property(e => e.MucTroCap).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtLoaiTroCapKhac>(entity =>
            {
                entity.Property(e => e.MucTroCap).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtLoaiTroCapOt>(entity =>
            {
                entity.Property(e => e.MucTroCap).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoGioOt).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtOther>(entity =>
            {
                entity.Property(e => e.KhauTruHoanLaiTruocThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.KhoanBuTruThangTruoc).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongTamUng).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapKhacKhongTinhThue).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtThuNhapTinhThue>(entity =>
            {
                entity.Property(e => e.Gift).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Month13).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NetToGross).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Other).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtTroCapOt>(entity =>
            {
                entity.Property(e => e.MucLuongHienTai).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongTongHop>(entity =>
            {
                entity.Property(e => e.CacKhoanCtyPhaiTraKhac).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CacKhoanGiamTruSauThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CacKhoanHoanLaiSauThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.GiamTruTruocThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.KhauTruHoanLaiTruocThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.KhoanBuTruThangTruoc).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongCoBanDongBh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongConLai).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongOtKhongTinhThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongOtTinhThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongTamUng).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongThucTe).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MucLuongCu).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MucLuongHienTai).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoNgayMucLuongCu)
                    .HasColumnName("SoNgay_MucLuongCu")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ThuNhapChiDuaVaoTinhThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ThuNhapThucNhan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ThuNhapThucNhanTruocKhiBuTruThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ThueTncnNld).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongChiPhiCtyPhaiTra).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongNgayTinhLuong).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongThuNhapBaoGomThueVaKhongThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongThuNhapChiuThueSauGiamTru).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongThuNhapSauKhiBoCacKhoanKhongTinhThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongThueTncnCtyVaNld).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongTienBhCtyPhaiDong).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongTienBhNldPhaiDong).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongTroCapCoDinh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongTroCapKhac).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapKhacKhongTinhThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TongHopChamCong>(entity =>
            {
                entity.Property(e => e.CongTac).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DaoTaoHoiThao).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NgayLamViecThucTe).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NghiBu).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NghiCheDo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NghiHuongBhxh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NghiHuongLuongKhac).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NghiKhongLuong).HasColumnType("decimal(18, 1)");

                entity.Property(e => e.NghiKhongPhep).HasColumnType("decimal(18, 1)");

                entity.Property(e => e.NghiLe).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NghiPhep).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoLanTruChuyenCan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongNgayDmvs).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtTroCapCoDinh>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtDieuKienTroCapKhac>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtTroCapKhac>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtEmpDktcKhac>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PhieuLuong>(entity =>
            {
                entity.Property(e => e.BaoHiem).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CauHinhGiamTruCaNhan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CauHinhGiamTruNguoiPhuThuoc).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CauHinhTroCapCc).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CauHinhTroCapDmvs).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CtyTraBh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DuocHuongTroCapChuyenCan)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DuocHuongTroCapKpi)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.GiamTruGiaCanh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.KinhPhiCongDoan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongCoBan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongCoBanSau).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongThang13).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LuongTheoNgayHocViec).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MucDieuChinh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NamBatDauKyLuong)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.NamKetThucKyLuong)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.NamTheoThangTruoc)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.NgayBatDauKyLuong)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.NgayDmvs).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NgayKetThucKyLuong)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.NgayKhongHuongChuyenCan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NgayLamViecThucTe).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NgayNghiKhongLuong).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NgayNghiLe).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NgayNghiPhep).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NghiKhongPhep).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OtKhongTinhThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OtTinhThue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PhanTramBaoHiemCty).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PhanTramBaoHiemNld).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PhanTramKinhPhiCongDoanCty).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PhieuLuongCode)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.QuaBocTham).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoLanTruChuyenCan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoLuongDkGiamTruGiaCanh).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoNgayLamViec).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ThangBatDauKyLuong)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ThangKetThucKyLuong)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ThangTruoc)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ThangTruocTiengAnh)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ThucNhan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ThuongCuoiNam).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ThuongKpi).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongChiPhiNhanVien).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongSoNgayKhongTinhLuong).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongThueTncn).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapAnTrua).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapCcncTheoNgayLvtt).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapChuyenCan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapChuyenCanDmvs).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapDiLai).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapDienThoai).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapHocViec).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapNhaO).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TroCapTrachNhiem).HasColumnType("decimal(18, 2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DotKiemKe>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DotKiemKeChiTiet>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PhongBanPheDuyetDoiTuong>(entity =>
            {
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmailNhanSu>(entity =>
            {
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });
        }

        /// <summary>  
        /// Overriding Save Changes  
        /// </summary>  
        /// <returns></returns>  
        public override int SaveChanges()
        {
            // Get entities that are added
            var changedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

            // Run through entities and look for a property named TentantId
            changedEntities.ForEach(e =>
            {
                // Tro to get property "TenantId"
                var tentantId = e.Entity.GetType().GetProperty("TenantId");
                // If the entity has this property then set it
                if (tentantId != null)
                {
                    // Set to current _tentantId
                    tentantId.SetValue(e.Entity, _tenantId);
                }
            });

            return base.SaveChanges();
        }
    }
}