using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TN.TNM.DataAccess.Databases;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.Api.RecurringJobs
{
    public class RecurringJobsService : BackgroundService
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IRecurringJobManager _recurringJobs;
        private readonly ILogger<RecurringJobScheduler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public RecurringJobsService(
            [NotNull] IBackgroundJobClient backgroundJobs,
            [NotNull] IRecurringJobManager recurringJobs,
            [NotNull] ILogger<RecurringJobScheduler> logger,
            [NotNull] IServiceScopeFactory scopeFactory)
        {
            _backgroundJobs = backgroundJobs ?? throw new ArgumentNullException(nameof(backgroundJobs));
            _recurringJobs = recurringJobs ?? throw new ArgumentNullException(nameof(recurringJobs));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeFactory = scopeFactory;
        }

        protected override System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _recurringJobs.AddOrUpdate("Cập nhật chức vụ nhân viên",
                    () => UpdateChucVu(),
                    Cron.Daily(1, 0), TimeZoneInfo.Local); //Cập nhật lúc 1h sáng - Cron.Daily(1)

                _recurringJobs.AddOrUpdate("Cập nhật ký hiệu chấm công nghỉ lễ, nghỉ bù",
                    () => CapNhatChamCong(),
                    "0 1 * * *", TimeZoneInfo.Local); //Cập nhật lúc 1h sáng - Cron.Daily(1)

                _recurringJobs.AddOrUpdate("Gửi email thông báo danh sách nhân viên sắp hết hạn hợp đồng",
                   () => ListNhanVienHetHanHD(),
                   "0 2 * * *", TimeZoneInfo.Local); //Cập nhật lúc 2h sáng

                _recurringJobs.AddOrUpdate("Gửi email thông báo danh sách nhân viên sắp đến hạn nộp hồ sơ",
                    () => ListNhanVienDenHanNopHoSo(),
                    "30 2 * * *", TimeZoneInfo.Local); //Cập nhật lúc 2h30p sáng

                _recurringJobs.AddOrUpdate("Tính lại phép của nhân viên",
                    () => TinhLaiPhep(),
                    "0 3 * * *", TimeZoneInfo.Local); //Cập nhật lúc 3h sáng

                _recurringJobs.AddOrUpdate("Gửi email thông báo hạn bảo hành, bảo dưỡng tài sản", //trước 10 ngày
                   () => ListTaiSanCanBaoHanhBaoTri(),
                   "0 10 * * *", TimeZoneInfo.Local); //Quét lúc 10h sáng
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void UpdateChucVu()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<TenantContext>();

                var listEmp = _context.Employee.ToList();
                var listHopDongNhanSu = _context.HopDongNhanSu.ToList();
                var listDeXuat = _context.DeXuatThayDoiChucVu.Where(x => x.TrangThai == 3).ToList(); //lấy đề xuất chức vụ đã duyệt

                var listDeXuatId = listDeXuat.Select(x => x.DeXuatThayDoiChucVuId).ToList();
                var listNhanVienDeXuat = _context.NhanVienDeXuatThayDoiChucVu.Where(x => listDeXuatId.Contains(x.DeXuatThayDoiChucVuId)).ToList();

                List<Employee> listEmployeeUpdate = new List<Employee>();
                listEmp.ForEach(item =>
                {
                    //lấy đề xuất chức vụ đã duyệt mới nhất của nhân viên
                    var deXuatChucVu = (from DX in listDeXuat
                                        join NvDX in listNhanVienDeXuat on DX.DeXuatThayDoiChucVuId equals NvDX.DeXuatThayDoiChucVuId
                                        where NvDX.EmployeeId == item.EmployeeId
                                        select new NhanVienDeXuatThayDoiChucVu
                                        {
                                            ChucVuDeXuatId = NvDX.ChucVuDeXuatId,
                                            CreatedDate = DX.NgayApDung //Ngày áp dụng
                                        }).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                    //lấy hợp đồng mới nhất của nhân viên
                    var hopDongNhanSuNew = listHopDongNhanSu.Where(x => x.EmployeeId == item.EmployeeId).OrderByDescending(x => x.NgayKyHopDong).FirstOrDefault();

                    //lấy hợp đồng cũ nhất của nhân viên
                    var hopDongNhanSuOld = listHopDongNhanSu.Where(x => x.EmployeeId == item.EmployeeId).OrderBy(x => x.NgayKyHopDong).FirstOrDefault();

                    if (hopDongNhanSuNew != null)
                    {
                        if (deXuatChucVu != null && deXuatChucVu.CreatedDate != null)
                        {
                            if (deXuatChucVu.CreatedDate.Value.Date > hopDongNhanSuNew.NgayKyHopDong.Date && deXuatChucVu.CreatedDate.Value.Date < DateTime.Now.Date)
                            {
                                item.PositionId = deXuatChucVu.ChucVuDeXuatId;
                            }
                        } else
                        {
                            item.PositionId = hopDongNhanSuNew.PositionId;
                        }

                        item.StartDateMayChamCong = hopDongNhanSuOld?.NgayBatDauLamViec;
                        item.UpdatedDate = DateTime.Now;
                        listEmployeeUpdate.Add(item);
                    }


                });

                if (listEmployeeUpdate.Count() > 0)
                {
                    _context.Employee.UpdateRange(listEmployeeUpdate);
                }

                _context.SaveChanges();

            }
        }

        public void CapNhatChamCong()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<TenantContext>();

                //Ngày hiện tại
                var nowDate = DateTime.Now;

                //Lấy list tenant
                var listTenant = _context.Tenants.ToList();
                
                //Lấy list cấu hình nghỉ lễ trong năm hiện tại
                var listCauHinhNghiLe = _context.CauHinhNghiLe.Where(x => x.SoNam == nowDate.Year).ToList();
                var listCauHinhNghiLeId = listCauHinhNghiLe.Select(y => y.NghiLeId).ToList();
                var listCauHinhNghiLeChiTiet = _context.CauHinhNghiLeChiTiet
                    .Where(x => listCauHinhNghiLeId.Contains(x.NghiLeId) && 
                                (x.LoaiNghiLe == 1 || x.LoaiNghiLe == 2)).ToList();

                //Lấy list nhân viên đang làm việc và có mã chấm công
                var listEmp = _context.Employee
                    .Join(_context.User,
                        emp => emp.EmployeeId,
                        user => user.EmployeeId,
                        (emp, user) => new { Emp = emp, User = user }) // selection
                    .Where(x => !String.IsNullOrWhiteSpace(x.Emp.CodeMayChamCong) &&
                                x.Emp.Active == true)
                    .Select(y => new Employee
                    {
                        EmployeeId = y.Emp.EmployeeId,
                        CodeMayChamCong = y.Emp.CodeMayChamCong,
                        EmployeeCode = y.Emp.EmployeeCode,
                        EmployeeName = y.Emp.EmployeeName,
                        TenantId = y.Emp.TenantId
                    }).ToList();
                var listEmpId = listEmp.Select(y => y.EmployeeId).ToList();

                //List ký hiệu chấm công
                var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong");
                var kyHieuNghiLe = listKyHieuChamCong.FirstOrDefault(x => x.Value == 2);
                var kyHieuNghiBu = listKyHieuChamCong.FirstOrDefault(x => x.Value == 3);

                var listChamCong = _context.ChamCong.Where(x =>
                    listEmpId.Contains(x.EmployeeId.Value) &&
                    x.NgayChamCong.Date == nowDate.Date).ToList();
                var listThongKeDiMuonVeSom = _context.ThongKeDiMuonVeSom.Where(x =>
                    listEmpId.Contains(x.EmployeeId) &&
                    x.NgayDmvs.Date == nowDate.Date).ToList();

                var listChamCongNew = new List<ChamCong>();
                var listChamCongUpdate = new List<ChamCong>();
                var listThongKeDiMuonVeSomRemove = new List<ThongKeDiMuonVeSom>();

                //Lấy dữ liệu theo tenant
                listTenant.ForEach(tenant =>
                {
                    var cauHinhNghiLe = listCauHinhNghiLe.FirstOrDefault(x => x.TenantId == tenant.TenantId);
                    var _listCauHinhNghiLeChiTiet = listCauHinhNghiLeChiTiet
                        .Where(x => x.TenantId == tenant.TenantId &&
                                    x.NghiLeId == cauHinhNghiLe.NghiLeId).ToList();
                    var _listEmp = listEmp.Where(x => x.TenantId == tenant.TenantId).ToList();
                    var _listChamCong = listChamCong.Where(x => x.TenantId == tenant.TenantId).ToList();
                    var _listThongKeDiMuonVeSom =
                        listThongKeDiMuonVeSom.Where(x => x.TenantId == tenant.TenantId).ToList();

                    if (_listCauHinhNghiLeChiTiet.Count > 0 && _listEmp.Count > 0)
                    {
                        //Nếu ngày hiện tại có trong cấu hình ngày nghỉ
                        var exitst = _listCauHinhNghiLeChiTiet.FirstOrDefault(x => x.Ngay.Date == nowDate.Date);

                        if (exitst != null)
                        {
                            var kyHieu = new TrangThaiGeneral();
                            //Nếu là nghỉ lễ
                            if (exitst.LoaiNghiLe == 1)
                            {
                                kyHieu = kyHieuNghiLe;
                            }
                            //Nếu là nghỉ bù
                            else if (exitst.LoaiNghiLe == 2)
                            {
                                kyHieu = kyHieuNghiBu;
                            }

                            #region Thêm dữ liệu vào bảng chấm công
                            
                            _listEmp.ForEach(emp =>
                            {
                                var _chamCong = _listChamCong.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);

                                //Create
                                if (_chamCong == null)
                                {
                                    var chamCong = new ChamCong();
                                    chamCong.EmployeeId = emp.EmployeeId;
                                    chamCong.NgayChamCong = nowDate.Date;
                                    chamCong.KyHieuVaoSang = kyHieu.Value;
                                    chamCong.KyHieuRaSang = kyHieu.Value;
                                    chamCong.KyHieuVaoChieu = kyHieu.Value;
                                    chamCong.KyHieuRaChieu = kyHieu.Value;
                                    chamCong.CreatedDate = DateTime.Now;
                                    chamCong.TenantId = tenant.TenantId;

                                    listChamCongNew.Add(chamCong);
                                }
                                //Update
                                else
                                {
                                    #region Xóa thống kê đi muộn về sớm

                                    var listThongKeDmvs = _listThongKeDiMuonVeSom
                                        .Where(x => x.EmployeeId == emp.EmployeeId).ToList();
                                    listThongKeDiMuonVeSomRemove.AddRange(listThongKeDmvs);

                                    #endregion

                                    _chamCong.KyHieuVaoSang = kyHieu.Value;
                                    _chamCong.KyHieuRaSang = kyHieu.Value;
                                    _chamCong.KyHieuVaoChieu = kyHieu.Value;
                                    _chamCong.KyHieuRaChieu = kyHieu.Value;

                                    listChamCongUpdate.Add(_chamCong);
                                }
                            });

                            #endregion
                        }
                    }
                });

                _context.ChamCong.AddRange(listChamCongNew);
                _context.ChamCong.UpdateRange(listChamCongUpdate);
                _context.ThongKeDiMuonVeSom.RemoveRange(listThongKeDiMuonVeSomRemove);
                _context.SaveChanges();
            }
        }

        //danh sách nhân viên sắp hết hạn thử việc/hợp đồng lao động
        public void ListNhanVienHetHanHD()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<TNTN8Context>();

                var categoryType = _context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS")?.CategoryTypeId;

                var loaiHopDong1T = _context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryType && x.CategoryCode == "HĐĐT1")?.CategoryId;
                var loaiHopDong2T = _context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryType && x.CategoryCode == "HĐĐT2")?.CategoryId;
                var loaiHopDong = _context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryType && x.CategoryId != loaiHopDong1T && x.CategoryId != loaiHopDong2T)?.CategoryId;

                var listHD1TId = new List<int>();//list id hợp đồng thử việc 1 tháng sắp hết hạn
                var listHD2TId = new List<int>();//list id hợp đồng thử việc 2 tháng sắp hết hạn 
                var listHDId = new List<int>();//list id hợp đồng sắp lao động hết hạn 

                //list hợp đồng mới nhất của tất cả nhân viên
                var listHopDongNhanSu = _context.HopDongNhanSu.OrderByDescending(x => x.NgayKyHopDong).GroupBy(x => x.EmployeeId).Select(g => g.FirstOrDefault()).ToList();

                //list hợp đồng sắp hết hạn của tất cả nhân viên
                listHopDongNhanSu.ForEach(item =>
                {
                    if (item.LoaiHopDongId == loaiHopDong1T)
                    {
                        //thông báo trước 5 ngày đối với nhân viên thử việc 1 tháng
                        if (item.NgayKetThucHopDong != null && item.NgayKetThucHopDong < DateTime.Today.AddDays(6) && item.NgayKetThucHopDong >= DateTime.Today.AddDays(5))
                        {
                            listHD1TId.Add(item.HopDongNhanSuId);
                        }
                    }
                    else if (item.LoaiHopDongId == loaiHopDong2T)
                    {
                        //thông báo trước 10 ngày đối với nhân viên thử việc 2 tháng
                        if (item.NgayKetThucHopDong != null && item.NgayKetThucHopDong < DateTime.Today.AddDays(11) && item.NgayKetThucHopDong >= DateTime.Today.AddDays(10))
                        {
                            listHD2TId.Add(item.HopDongNhanSuId);
                        }
                    }
                    else
                    {
                        //thông báo trước 30 ngày đối với nhân viên hợp đồng lao động
                        if (item.NgayKetThucHopDong != null && item.NgayKetThucHopDong < DateTime.Today.AddDays(31) && item.NgayKetThucHopDong >= DateTime.Today.AddDays(30))
                        {
                            listHDId.Add(item.HopDongNhanSuId);
                        }
                    }

                });

                //list nhân viên sắp hết hạn thử việc 1 tháng
                var listHD1T = listHopDongNhanSu.Where(x => listHD1TId.Contains(x.HopDongNhanSuId)).ToList();
                if (listHD1T.Count() > 0)
                {
                    //gửi mail tới HR
                    NotificationHelper.AccessNotification(_context, "EMPLOYEE_INFOR", "PROBATION_OVER", new Employee(), listHD1T, true);
                }

                //list nhân viên sắp hết hạn thử việc 2 tháng
                var listHD2T = listHopDongNhanSu.Where(x => listHD2TId.Contains(x.HopDongNhanSuId)).ToList();
                if (listHD2T.Count() > 0)
                {
                    //gửi mail tới HR
                    NotificationHelper.AccessNotification(_context, "EMPLOYEE_INFOR", "PROBATION_OVER", new Employee(), listHD2T, true);
                }

                //list nhân viên sắp hết hạn hợp đồng lao động
                var listHD = listHopDongNhanSu.Where(x => listHDId.Contains(x.HopDongNhanSuId)).ToList();
                if (listHD.Count() > 0)
                {
                    //gửi mail tới HR
                    NotificationHelper.AccessNotification(_context, "EMPLOYEE_INFOR", "CONTRACT_EXPIRED", new Employee(), listHD, true);
                }

                _context.SaveChanges();

            }
        }

        //danh sách nhân viên sắp đến hạn nộp hồ sơ
        public void ListNhanVienDenHanNopHoSo()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<TNTN8Context>();

                var listCauHinhChecklist = _context.CauHinhChecklist.ToList();
                var listAllTaiLieu = _context.TaiLieuNhanVien.ToList();
                var listTaiLieu = new List<TaiLieuNhanVien>();//list tài liệu sắp đến hạn nộp của tất cả nhân viên

                listAllTaiLieu.ForEach(item =>
                {
                    //Thông báo hạn nộp hồ sơ trước 2 ngày so với hạn nộp
                    if (item.NgayHen != null && item.NgayHen < DateTime.Today.AddDays(3) && item.NgayHen >= DateTime.Today.AddDays(2))
                    {
                        item.TenTaiLieu = listCauHinhChecklist.FirstOrDefault(x => x.CauHinhChecklistId == item.CauHinhChecklistId)?.TenTaiLieu;
                        listTaiLieu.Add(item);
                    }

                });
                
                if (listTaiLieu.Count() > 0)
                {
                    //gửi mail tới HR
                    NotificationHelper.AccessNotification(_context, "EMPLOYEE_INFOR", "DEADLINE_SUBMISSION", new Employee(), listTaiLieu, true);
                }

                _context.SaveChanges();

            }
        }

        public void ListTaiSanCanBaoHanhBaoTri()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<TNTN8Context>();
                var listTaiSan = _context.TaiSan.Where(x => x.NgayVaoSo != null && x.ThoiHanBaoHanh != null && x.BaoDuongDinhKy != null)
                .Select(x => new AssetEntityModel {
                    TaiSanId = x.TaiSanId,
                    TenTaiSan = x.TenTaiSan,
                    MaTaiSan = x.MaTaiSan,
                    NgayVaoSo = x.NgayVaoSo,
                    ThoiHanBaoHanh = x.ThoiHanBaoHanh,
                    BaoDuongDinhKy = x.BaoDuongDinhKy,
                    NgayHetHanBaoHanh = x.NgayVaoSo.Value.AddMonths(x.BaoDuongDinhKy.Value),
                }).ToList();
                var listTaiSanThongBao = new List<AssetEntityModel>();
                var ngayHienTai = DateTime.Now;
                //Lọc tài sản
                var soNgayTrongThang = 30;
                listTaiSan.ForEach(item =>
                {
                    if(item.BaoDuongDinhKy != 0)
                    {
                        var soLanBaoHanh = item.ThoiHanBaoHanh / item.BaoDuongDinhKy;
                        //Lấy các mốc thời điểm bảo hành của tài sản
                        for (var i = 0; i < soLanBaoHanh; i++)
                        {
                            var thoiDiemBaoHanh = item.NgayVaoSo.Value.Date.AddDays(i * soNgayTrongThang);
                            //Nếu gần đến hạn bảo trì/bảo dưỡng thì thông báo trước 10 ngày
                            if ((ngayHienTai - thoiDiemBaoHanh).TotalDays <= 10 && (ngayHienTai - thoiDiemBaoHanh).TotalDays >= 0)
                            {
                                listTaiSanThongBao.Add(item);
                            }
                        };
                    }
                });
                if (listTaiSanThongBao.Count() > 0)
                {
                    NotificationHelper.AccessNotification(_context, "TAISAN_INFOR", "TAISAN_BD", new Employee(), listTaiSanThongBao, true);
                }
            }
        }

        public void TinhLaiPhep()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<TenantContext>();

                //Ngày hiện tại
                var nowDate = DateTime.Now;

                //Lấy list tenant
                var listTenant = _context.Tenants.ToList();

                listTenant.ForEach(tenant =>
                {
                    //Cấu hình ngày reset phép năm
                    var cauHinhNgayResetPhepNam =
                        _context.SystemParameter
                            .FirstOrDefault(x => x.SystemKey == "NgayResetPhepNam" && x.TenantId == tenant.TenantId)
                            ?.SystemValueString;

                    //Cấu hình ngày hết hạn phép năm cũ
                    var cauHinhNgayHetHanPhepNamCu =
                        _context.SystemParameter
                            .FirstOrDefault(x => x.SystemKey == "NgayHetHanPhepNamCu" && x.TenantId == tenant.TenantId)
                            ?.SystemValueString;

                    if (!String.IsNullOrWhiteSpace(cauHinhNgayResetPhepNam))
                    {
                        var list = cauHinhNgayResetPhepNam.Split("/");
                        int ngaySo = Int32.Parse(list.First());
                        int thangSo = Int32.Parse(list.Last());

                        var ngayResetPhepNam = new DateTime(nowDate.Year, thangSo, ngaySo);

                        if (nowDate.Date == ngayResetPhepNam.Date)
                        {
                            var listEmp = _context.Employee.Where(x => x.TenantId == tenant.TenantId).ToList();

                            listEmp.ForEach(item =>
                            {
                                item.SoNgayDaNghiPhep = 0;
                                item.SoNgayPhepConLai = 12 + (item.SoNgayPhepConLai ?? 0);
                            });

                            _context.Employee.UpdateRange(listEmp);
                            _context.SaveChanges();
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(cauHinhNgayHetHanPhepNamCu))
                    {
                        var list = cauHinhNgayHetHanPhepNamCu.Split("/");
                        int ngaySo = Int32.Parse(list.First());
                        int thangSo = Int32.Parse(list.Last());

                        var ngayHetHanPhepNamCu = new DateTime(nowDate.Year, thangSo, ngaySo);

                        if (nowDate.Date == ngayHetHanPhepNamCu.Date)
                        {
                            var listEmp = _context.Employee.Where(x => x.TenantId == tenant.TenantId).ToList();

                            listEmp.ForEach(item =>
                            {
                                if (item.SoNgayPhepConLai > 12)
                                {
                                    item.SoNgayPhepConLai = 12;
                                }
                            });

                            _context.Employee.UpdateRange(listEmp);
                            _context.SaveChanges();
                        }
                    }
                });
            }
        }
    }
}
