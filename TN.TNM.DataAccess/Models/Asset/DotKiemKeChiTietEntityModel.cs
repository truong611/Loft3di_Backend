using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Asset
{
    public class DotKiemKeChiTietEntityModel
    {
        public int? DotKiemKeChiTietId { get; set; }
        public int? DotKiemKeId { get; set; }
        public int? TaiSanId { get; set; }
        public bool? UserConfirm { get; set; }
        public Guid? NguoiKiemKeId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public string NguoiKiemKeName { get; set; }
        public string MaTaiSan { get; set; }
        public string TenTaiSan { get; set; }
        public string KhuVucName { get; set; }
        public string TinhTrangName { get; set; }
        public decimal? GiaTriTinhKhauHao { get; set; }
        public decimal? KhauHaoLuyKe { get; set; }
        public decimal? GiaTriConLai { get; set; }
        public string  MoTaTaiSan { get; set; }
        public Guid? ProvincecId { get; set; }
        public Guid? PhanLoaiTaiSanId { get; set; }
        public int? HienTrangTaiSan { get; set; }


        public string SerialNumber { get; set; }
        public string OwnerShip { get; set; }
        public string LegalCode { get; set; }
        public string AssetType { get; set; }
        public string NguoiDungName { get; set; }
        public string NguoiDungUserName { get; set; }
        public string NguoiDungOrg { get; set; }
        public string ExpenseUnit { get; set; }
        public decimal? GiaTriNguyenGia { get; set; }
        public decimal? TgianKhauHao { get; set; }
        public decimal? TgianDaKhauHao { get; set; }
        public decimal? TgianKhauHaoCL { get; set; }
        public decimal? KhauHaoKyHt { get; set; }
        public DateTime? NgayKetThucKhauHao { get; set; }
        public DateTime? NgayNhapKho { get; set; }
        public string Tang { get; set; }
        public string VỉTriTs { get; set; }
        public string MucDichTS { get; set; }
    }
}
