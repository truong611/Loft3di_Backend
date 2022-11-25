using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.ConstType.Contact;
using TN.TNM.DataAccess.ConstType.Note;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Salary;
using TN.TNM.DataAccess.Messages.Results.Salary;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.CauHinhNghiLe;
using TN.TNM.DataAccess.Models.CauHinhOtMođel;
using TN.TNM.DataAccess.Models.CauHinhThue;
using TN.TNM.DataAccess.Models.ChamCong;
using TN.TNM.DataAccess.Models.DynamicColumnTable;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.GiamTru;
using TN.TNM.DataAccess.Models.Salary;
using TongHopChamCongModel = TN.TNM.DataAccess.Models.Salary.TongHopChamCongModel;
using TN.TNM.DataAccess.Messages.Results;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Net.Mime;
using System.IO;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class SalaryDAO : BaseDAO, ISalaryDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public SalaryDAO(TNTN8Context _content, IHostingEnvironment _hostingEnvironment)
        {
            this.context = _content;
            this.hostingEnvironment = _hostingEnvironment;
        }

        public CreateOrUpdateCaLamViecResult CreateOrUpdateCaLamViec(CreateOrUpdateCaLamViecParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    //Create
                    if (parameter.CaLamViec.CaLamViecId == null || parameter.CaLamViec.CaLamViecId == 0)
                    {
                        var caLamViec = parameter.CaLamViec.ToEntity();
                        context.CaLamViec.Add(caLamViec);
                        context.SaveChanges();

                        var listCaLamViecChiTiet = new List<CaLamViecChiTiet>();
                        parameter.CaLamViec.ListCaLamViecChiTiet.ForEach(item =>
                        {
                            var caLamViecChiTiet = new CaLamViecChiTiet();
                            caLamViecChiTiet.CaLamViecId = caLamViec.CaLamViecId;
                            caLamViecChiTiet.NgayTrongTuan = item.NgayTrongTuan;

                            listCaLamViecChiTiet.Add(caLamViecChiTiet);
                        });

                        context.CaLamViecChiTiet.AddRange(listCaLamViecChiTiet);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var caLamViec =
                            context.CaLamViec.FirstOrDefault(x => x.CaLamViecId == parameter.CaLamViec.CaLamViecId);

                        if (caLamViec == null)
                        {
                            return new CreateOrUpdateCaLamViecResult()
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = "Ca làm việc không tồn tại trên hệ thống"
                            };
                        }

                        caLamViec.LoaiCaLamViecId = parameter.CaLamViec.LoaiCaLamViecId;
                        caLamViec.GioVao = parameter.CaLamViec.GioVao;
                        caLamViec.GioRa = parameter.CaLamViec.GioRa;
                        caLamViec.ThoiGianKetThucCa = parameter.CaLamViec.ThoiGianKetThucCa;

                        /* Xóa list cũ */
                        var listCaLamViecChiTietOld = context.CaLamViecChiTiet
                            .Where(x => x.CaLamViecId == caLamViec.CaLamViecId).ToList();
                        context.CaLamViecChiTiet.RemoveRange(listCaLamViecChiTietOld);
                        context.SaveChanges();

                        /* Thêm list mới */
                        var listCaLamViecChiTiet = new List<CaLamViecChiTiet>();
                        parameter.CaLamViec.ListCaLamViecChiTiet.ForEach(item =>
                        {
                            var caLamViecChiTiet = new CaLamViecChiTiet();
                            caLamViecChiTiet.CaLamViecId = caLamViec.CaLamViecId;
                            caLamViecChiTiet.NgayTrongTuan = item.NgayTrongTuan;

                            listCaLamViecChiTiet.Add(caLamViecChiTiet);
                        });

                        context.CaLamViecChiTiet.AddRange(listCaLamViecChiTiet);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCaLamViecResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCaLamViecResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhGiamTruResult CreateOrUpdateCauHinhGiamTru(CreateOrUpdateCauHinhGiamTruParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    //Create
                    if (parameter.CauHinhGiamTru.CauHinhGiamTruId == null || parameter.CauHinhGiamTru.CauHinhGiamTruId == 0)
                    {
                        var exists = context.CauHinhGiamTru.FirstOrDefault(x =>
                            x.LoaiGiamTruId == parameter.CauHinhGiamTru.LoaiGiamTruId);

                        if (exists != null)
                        {
                            return new CreateOrUpdateCauHinhGiamTruResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Loại giảm trừ đã tồn tại trên hệ thống"
                            };
                        }

                        var cauHinhGiamTru = parameter.CauHinhGiamTru.ToEntity();
                        context.CauHinhGiamTru.Add(cauHinhGiamTru);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var cauHinhGiamTru = context.CauHinhGiamTru
                            .FirstOrDefault(x => x.CauHinhGiamTruId == parameter.CauHinhGiamTru.CauHinhGiamTruId);

                        if (cauHinhGiamTru == null)
                        {
                            return new CreateOrUpdateCauHinhGiamTruResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        var exists = context.CauHinhGiamTru.FirstOrDefault(x =>
                            x.LoaiGiamTruId == parameter.CauHinhGiamTru.LoaiGiamTruId &&
                            x.CauHinhGiamTruId != parameter.CauHinhGiamTru.CauHinhGiamTruId);

                        if (exists != null)
                        {
                            return new CreateOrUpdateCauHinhGiamTruResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Loại giảm trừ đã tồn tại trên hệ thống"
                            };
                        }

                        cauHinhGiamTru.LoaiGiamTruId = parameter.CauHinhGiamTru.LoaiGiamTruId;
                        cauHinhGiamTru.MucGiamTru = parameter.CauHinhGiamTru.MucGiamTru;
                        context.CauHinhGiamTru.Update(cauHinhGiamTru);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCauHinhGiamTruResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhGiamTruResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhNghiLeResult CreateOrUpdateCauHinhNghiLe(CreateOrUpdateCauHinhNghiLeParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    //Create
                    if (parameter.CauHinhNghiLe.NghiLeId == null || parameter.CauHinhNghiLe.NghiLeId == 0)
                    {
                        var existsSoNam =
                            context.CauHinhNghiLe.FirstOrDefault(x => x.SoNam == parameter.CauHinhNghiLe.SoNam);

                        if (existsSoNam != null)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Số năm đã tồn tại, hãy nhập số năm khác"
                            };
                        }

                        bool isTrungNgay = false;
                        parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.ForEach(item =>
                        {
                            var count = parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.Count(x =>
                                x.Ngay.Date == item.Ngay.Date);

                            if (count > 1) isTrungNgay = true;
                        });

                        if (isTrungNgay)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Ngày nghỉ lễ, Ngày nghỉ bù, Ngày làm bù không được trùng nhau"
                            };
                        }

                        var cauHinhNghiLe = parameter.CauHinhNghiLe.ToEntity();
                        context.CauHinhNghiLe.Add(cauHinhNghiLe);
                        context.SaveChanges();

                        var listCauHinhNghiLeChiTiet = new List<CauHinhNghiLeChiTiet>();
                        parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.ForEach(item =>
                        {
                            var cauHinhNghiLeChiTiet = new CauHinhNghiLeChiTiet();
                            cauHinhNghiLeChiTiet.NghiLeId = cauHinhNghiLe.NghiLeId;
                            cauHinhNghiLeChiTiet.LoaiNghiLe = item.LoaiNghiLe;
                            cauHinhNghiLeChiTiet.Ngay = item.Ngay;

                            listCauHinhNghiLeChiTiet.Add(cauHinhNghiLeChiTiet);
                        });

                        context.CauHinhNghiLeChiTiet.AddRange(listCauHinhNghiLeChiTiet);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var cauHinhNghiLe =
                            context.CauHinhNghiLe.FirstOrDefault(x => x.NghiLeId == parameter.CauHinhNghiLe.NghiLeId);

                        if (cauHinhNghiLe == null)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        var existsSoNam =
                            context.CauHinhNghiLe.FirstOrDefault(x => x.SoNam == parameter.CauHinhNghiLe.SoNam &&
                                                                      x.NghiLeId != cauHinhNghiLe.NghiLeId);

                        if (existsSoNam != null)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Số năm đã tồn tại, hãy nhập số năm khác"
                            };
                        }

                        bool isTrungNgay = false;
                        parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.ForEach(item =>
                        {
                            var count = parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.Count(x =>
                                x.Ngay.Date == item.Ngay.Date);

                            if (count > 1) isTrungNgay = true;
                        });

                        if (isTrungNgay)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Ngày nghỉ lễ, Ngày nghỉ bù, Ngày làm bù không được trùng nhau"
                            };
                        }

                        cauHinhNghiLe.SoNam = parameter.CauHinhNghiLe.SoNam;

                        /* Xóa list cũ */
                        var listCauHinhNghiLeChiTietOld = context.CauHinhNghiLeChiTiet
                            .Where(x => x.NghiLeId == cauHinhNghiLe.NghiLeId).ToList();
                        context.CauHinhNghiLeChiTiet.RemoveRange(listCauHinhNghiLeChiTietOld);
                        context.SaveChanges();

                        /* Thêm list mới */
                        var listCauHinhNghiLeChiTiet = new List<CauHinhNghiLeChiTiet>();
                        parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.ForEach(item =>
                        {
                            var cauHinhNghiLeChiTiet = new CauHinhNghiLeChiTiet();
                            cauHinhNghiLeChiTiet.NghiLeId = cauHinhNghiLe.NghiLeId;
                            cauHinhNghiLeChiTiet.LoaiNghiLe = item.LoaiNghiLe;
                            cauHinhNghiLeChiTiet.Ngay = item.Ngay;

                            listCauHinhNghiLeChiTiet.Add(cauHinhNghiLeChiTiet);
                        });

                        context.CauHinhNghiLeChiTiet.AddRange(listCauHinhNghiLeChiTiet);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCauHinhNghiLeResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhNghiLeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhOtResult CreateOrUpdateCauHinhOt(CreateOrUpdateCauHinhOtParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    //Create
                    if (parameter.CauHinhOt.CauHinhOtId == null || parameter.CauHinhOt.CauHinhOtId == 0)
                    {
                        var exists = context.CauHinhOt.FirstOrDefault(x => x.LoaiOtId == parameter.CauHinhOt.LoaiOtId);

                        if (exists != null)
                        {
                            return new CreateOrUpdateCauHinhOtResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Loại OT đã tồn tại trên hệ thống"
                            };
                        }

                        var cauHinhOt = parameter.CauHinhOt.ToEntity();
                        context.CauHinhOt.Add(cauHinhOt);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var cauHinhOt = context.CauHinhOt.FirstOrDefault(x => x.CauHinhOtId == parameter.CauHinhOt.CauHinhOtId);

                        if (cauHinhOt == null)
                        {
                            return new CreateOrUpdateCauHinhOtResult
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        var exists = context.CauHinhOt
                            .FirstOrDefault(x => x.LoaiOtId == parameter.CauHinhOt.LoaiOtId &&
                                                 x.CauHinhOtId != parameter.CauHinhOt.CauHinhOtId);

                        if (exists != null)
                        {
                            return new CreateOrUpdateCauHinhOtResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Loại OT đã tồn tại trên hệ thống"
                            };
                        }

                        cauHinhOt.LoaiOtId = parameter.CauHinhOt.LoaiOtId;
                        cauHinhOt.TyLeHuong = parameter.CauHinhOt.TyLeHuong;
                        context.CauHinhOt.Update(cauHinhOt);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCauHinhOtResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCongThucTinhLuongResult CreateOrUpdateCongThucTinhLuong(CreateOrUpdateCongThucTinhLuongParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";
                    var isValid = CheckSyntaxCongThuc(parameter.CongThucTinhLuong.CongThuc);

                    //Nếu công thức sai cú pháp
                    if (!isValid)
                    {
                        return new CreateOrUpdateCongThucTinhLuongResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Công thức không đúng định dạng"
                        };
                    }

                    //Create
                    if (parameter.CongThucTinhLuong.CongThucTinhLuongId == null || parameter.CongThucTinhLuong.CongThucTinhLuongId == 0)
                    {
                        var congThucTinhLuong = parameter.CongThucTinhLuong.ToEntity();
                        context.CongThucTinhLuong.Add(congThucTinhLuong);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var congThucTinhLuong = context.CongThucTinhLuong
                                .FirstOrDefault(x => x.CongThucTinhLuongId == parameter.CongThucTinhLuong.CongThucTinhLuongId);

                        if (congThucTinhLuong == null)
                        {
                            return new CreateOrUpdateCongThucTinhLuongResult
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        congThucTinhLuong.CongThuc = parameter.CongThucTinhLuong.CongThuc;
                        context.CongThucTinhLuong.Update(congThucTinhLuong);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCongThucTinhLuongResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCongThucTinhLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetTkDiMuonVeSomResult GetTkDiMuonVeSom(GetTkDiMuonVeSomParameter parameter)
        {
            try
            {
                if (parameter.TuNgay.Date > parameter.DenNgay.Date)
                {
                    return new GetTkDiMuonVeSomResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Bộ lọc Từ ngày phải trước Đến ngày"
                    };
                }

                var listData = new List<List<DataRowModel>>();
                var listColorChamCong = new List<ColorChamCongModel>();
                
                var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= parameter.TuNgay.Date &&
                                                               x.NgayChamCong.Date <= parameter.DenNgay.Date &&
                                                               (parameter.ListEmployeeId.Count == 0 ||
                                                                parameter.ListEmployeeId.Contains(x.EmployeeId.Value)))
                    .OrderBy(z => z.NgayChamCong).ToList();
                var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
                var listEmp = new List<Employee>();

                //Nếu hiển thị tất cả nhân viên 
                if (parameter.IsShowOption == 0)
                {
                    listEmp = context.Employee
                        .Where(x => !String.IsNullOrWhiteSpace(x.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.EmployeeId)))
                        .OrderBy(z => z.CodeMayChamCong)
                        .ToList();
                }
                //Nếu chỉ hiển thị nhân viên đang làm việc
                else if (parameter.IsShowOption == 1)
                {
                    listEmp = context.Employee
                        .Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user }) // selection
                        .Where(x => !String.IsNullOrWhiteSpace(x.Emp.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.Emp.EmployeeId)) &&
                                     x.Emp.Active == true)
                        .Select(y => new Employee
                        {
                            EmployeeId = y.Emp.EmployeeId,
                            CodeMayChamCong = y.Emp.CodeMayChamCong,
                            EmployeeCode = y.Emp.EmployeeCode,
                            EmployeeName = y.Emp.EmployeeName,
                        })
                        .OrderBy(z => z.CodeMayChamCong)
                        .ToList();
                }
                //Nếu chỉ hiển thị nhân viên đã nghỉ viêc
                else if (parameter.IsShowOption == 2)
                {
                    listEmp = context.Employee
                        .Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user }) // selection
                        .Where(x => !String.IsNullOrWhiteSpace(x.Emp.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.Emp.EmployeeId)) &&
                                    x.Emp.Active == false && x.User.Active == false)
                        .Select(y => new Employee
                        {
                            EmployeeId = y.Emp.EmployeeId,
                            CodeMayChamCong = y.Emp.CodeMayChamCong,
                            EmployeeCode = y.Emp.EmployeeCode,
                            EmployeeName = y.Emp.EmployeeName,
                        })
                        .OrderBy(z => z.CodeMayChamCong)
                        .ToList();
                }

                var listDate = new List<DateTime>();
                var startDate = parameter.TuNgay.Date;
                var endDate = parameter.DenNgay.Date;
                while (startDate <= endDate)
                {
                    listDate.Add(startDate);
                    startDate = startDate.AddDays(1).Date;
                }
                var listDmvs = context.ThongKeDiMuonVeSom
                    .Where(x => listEmpId.Contains(x.EmployeeId) && 
                                x.NgayDmvs.Date >= parameter.TuNgay.Date &&
                                x.NgayDmvs.Date <= parameter.DenNgay.Date).ToList();

                //Nếu có lọc theo Chấm công bất thường: 1: Đi muộn về sớm, 2: Chấm công thiếu ca
                if (parameter.ListOption.Count > 0)
                {
                    var existsOptionDmvs = parameter.ListOption.Count(x => x == 1);
                    var existsOptionCctc = parameter.ListOption.Count(x => x == 2);

                    var _listId = new List<Guid>();
                    var _listId_1 = new List<Guid>();
                    var _listId_2 = new List<Guid>();

                    //Nếu lọc theo Đi muộn về sớm
                    if (existsOptionDmvs > 0)
                    {
                        _listId_1 = listDmvs.Select(y => y.EmployeeId).Distinct().ToList();
                    }

                    //Nếu lọc theo Chấm công thiếu ca
                    if (existsOptionCctc > 0)
                    {
                        _listId_2 = listChamCong.Where(x => (x.KyHieuVaoSang == null &&
                                                             (x.VaoSang == null && x.RaSang != null) ||
                                                             (x.VaoSang != null && x.RaSang == null)) ||
                                                            (x.KyHieuVaoChieu == null &&
                                                             (x.VaoChieu == null && x.RaChieu != null) ||
                                                             (x.VaoChieu != null && x.RaChieu == null)))
                            .Select(y => y.EmployeeId.Value).Distinct().ToList();
                    }

                    _listId.AddRange(_listId_1);
                    _listId.AddRange(_listId_2);
                    _listId = _listId.Distinct().ToList();

                    listEmp = listEmp.Join(_listId,
                            emp => emp.EmployeeId,
                            empId => empId,
                            (emp, empId) => new {Emp = emp, EmpId = empId}) // selection
                        .Select(y => new Employee
                        {
                            EmployeeId = y.Emp.EmployeeId,
                            CodeMayChamCong = y.Emp.CodeMayChamCong,
                            EmployeeCode = y.Emp.EmployeeCode,
                            EmployeeName = y.Emp.EmployeeName,
                        })
                        .OrderBy(z => z.CodeMayChamCong)
                        .ToList();
                }

                var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");

                #region Data

                int stt = 0;
                listEmp.ForEach(item =>
                {
                    stt++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "60px";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "code";
                    dataRow2.ColumnValue = item.CodeMayChamCong;
                    dataRow2.Width = "60px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "name";
                    dataRow3.ColumnValue = item.EmployeeCode + " - " + item.EmployeeName;
                    dataRow3.Width = "250px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    #region Phần tổng kết

                    int tongSoPhutDmvs = CommonHelper.GetSoPhutDmvs(item.EmployeeId, listDmvs);
                    decimal tongSoNgayDmvs = Math.Round((decimal)tongSoPhutDmvs / 480, 2);
                    int tongSoLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);
                    double tongSoNgayNghi = GetTongSoNgayNghi(item.EmployeeId, listChamCong);

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "soPhutDmvs";
                    dataRow4.ColumnValue = tongSoPhutDmvs.ToString();
                    dataRow4.Width = "85px";
                    dataRow4.TextAlign = "right";
                    dataRow4.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow4);

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "soNgayDmvs";
                    dataRow5.ColumnValue = tongSoNgayDmvs.ToString();
                    dataRow5.Width = "85px";
                    dataRow5.TextAlign = "right";
                    dataRow5.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow5);

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "soNgayNghi";
                    dataRow6.ColumnValue = tongSoNgayNghi.ToString();
                    dataRow6.Width = "85px";
                    dataRow6.TextAlign = "right";
                    dataRow6.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow6);

                    var dataRow7 = new DataRowModel();
                    dataRow7.ColumnKey = "soLanDmvs";
                    dataRow7.ColumnValue = tongSoLanDmvs.ToString();
                    dataRow7.Width = "85px";
                    dataRow7.TextAlign = "right";
                    dataRow7.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow7);

                    #endregion

                    int index = 0;
                    listDate.ForEach(data =>
                    {
                        //Lấy dữ liệu chấm công của nhân viên theo ngày
                        var chamCong = listChamCong.FirstOrDefault(x =>
                            x.NgayChamCong == data && x.EmployeeId == item.EmployeeId);

                        //Nếu nhân viên có dữ liệu chấm công theo ngày này
                        if (chamCong != null)
                        {
                            #region Vào sáng

                            index++;
                            var _vaoSang = new DataRowModel();
                            _vaoSang.ColumnKey = "index_" + index;
                            _vaoSang.ColumnValue = HienThiKyHieu(chamCong, 1);
                            _vaoSang.Width = "60px";
                            _vaoSang.TextAlign = "center";

                            listDataRow.Add(_vaoSang);

                            #region Màu hiển thị của dữ liệu

                            var _colorVaoSang = new ColorChamCongModel();
                            _colorVaoSang.Code = item.CodeMayChamCong;
                            _colorVaoSang.NgayChamCong = data.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            _colorVaoSang.Index = _vaoSang.ColumnKey;
                            _colorVaoSang.ColorCode = HienThiMau(chamCong, 1, listDmvs);

                            listColorChamCong.Add(_colorVaoSang);

                            #endregion

                            #endregion

                            #region Ra sáng

                            index++;
                            var _raSang = new DataRowModel();
                            _raSang.ColumnKey = "index_" + index;
                            _raSang.ColumnValue = HienThiKyHieu(chamCong, 2);
                            _raSang.Width = "60px";
                            _raSang.TextAlign = "center";

                            listDataRow.Add(_raSang);

                            #region Màu hiển thị của dữ liệu

                            var _colorRaSang = new ColorChamCongModel();
                            _colorRaSang.Code = item.CodeMayChamCong;
                            _colorRaSang.NgayChamCong = data.ToString("dd/MM/yyyy");
                            _colorRaSang.Index = _raSang.ColumnKey;
                            _colorRaSang.ColorCode = HienThiMau(chamCong, 2, listDmvs);

                            listColorChamCong.Add(_colorRaSang);

                            #endregion

                            #endregion

                            #region Vào chiều

                            index++;
                            var _vaoChieu = new DataRowModel();
                            _vaoChieu.ColumnKey = "index_" + index;
                            _vaoChieu.ColumnValue = HienThiKyHieu(chamCong, 3);
                            _vaoChieu.Width = "60px";
                            _vaoChieu.TextAlign = "center";

                            listDataRow.Add(_vaoChieu);

                            #region Màu hiển thị của dữ liệu

                            var _colorVaoChieu = new ColorChamCongModel();
                            _colorVaoChieu.Code = item.CodeMayChamCong;
                            _colorVaoChieu.NgayChamCong = data.ToString("dd/MM/yyyy");
                            _colorVaoChieu.Index = _vaoChieu.ColumnKey;
                            _colorVaoChieu.ColorCode = HienThiMau(chamCong, 3, listDmvs);

                            listColorChamCong.Add(_colorVaoChieu);

                            #endregion

                            #endregion

                            #region Ra chiều

                            index++;
                            var _raChieu = new DataRowModel();
                            _raChieu.ColumnKey = "index_" + index;
                            _raChieu.ColumnValue = HienThiKyHieu(chamCong, 4);
                            _raChieu.Width = "60px";
                            _raChieu.TextAlign = "center";

                            listDataRow.Add(_raChieu);

                            #region Màu hiển thị của dữ liệu

                            var _colorRaChieu = new ColorChamCongModel();
                            _colorRaChieu.Code = item.CodeMayChamCong;
                            _colorRaChieu.NgayChamCong = data.ToString("dd/MM/yyyy");
                            _colorRaChieu.Index = _raChieu.ColumnKey;
                            _colorRaChieu.ColorCode = HienThiMau(chamCong, 4, listDmvs);

                            listColorChamCong.Add(_colorRaChieu);

                            #endregion

                            #endregion
                        }
                        //Nếu nhân viên không có dữ liệu chấm công
                        else
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                index++;
                                var _dataRow = new DataRowModel();
                                _dataRow.ColumnKey = "index_" + index;
                                _dataRow.ColumnValue = "--";
                                _dataRow.Width = "60px";
                                _dataRow.TextAlign = "center";

                                listDataRow.Add(_dataRow);
                            }
                        }
                    });

                    listData.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeader = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "40px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "code",
                        ColumnValue = "Code",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "120px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "name",
                        ColumnValue = "Họ tên",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                };

                listHeader1.Add(new DataHeaderModel()
                {
                    ColumnKey = "soPhutDmvs",
                    ColumnValue = "Số phút đi muộn/về sớm",
                    Rowspan = 3,
                    Colspan = 0,
                    Width = "85px",
                });

                listHeader1.Add(new DataHeaderModel()
                {
                    ColumnKey = "soNgayDmvs",
                    ColumnValue = "Số ngày đi muộn/về sớm",
                    Rowspan = 3,
                    Colspan = 0,
                    Width = "85px",
                });

                listHeader1.Add(new DataHeaderModel()
                {
                    ColumnKey = "soNgayNghi",
                    ColumnValue = "Tổng số ngày nghỉ",
                    Rowspan = 3,
                    Colspan = 0,
                    Width = "85px",
                });

                listHeader1.Add(new DataHeaderModel()
                {
                    ColumnKey = "soLanDmvs",
                    ColumnValue = "Số lần đi muộn/về sớm",
                    Rowspan = 3,
                    Colspan = 0,
                    Width = "85px",
                });

                int _index = 0;
                listDate.ForEach(item =>
                {
                    string ngayTrongTuan = listNgayLamViecTrongTuan
                        .FirstOrDefault(x => x.Value == ConvertNgayTrongTuan(item.DayOfWeek))?.Name;

                    _index++;
                    var _dataHeader = new DataHeaderModel();
                    _dataHeader.ColumnKey = "index_ngay_" + _index;
                    _dataHeader.ColumnValue = item.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + ngayTrongTuan;
                    _dataHeader.Rowspan = 0;
                    _dataHeader.Colspan = 4;
                    _dataHeader.Width = "240px";

                    listHeader1.Add(_dataHeader);
                });

                listDataHeader.Add(listHeader1);

                /* tr2 */
                var listHeader2 = new List<DataHeaderModel>();
                int _indexCa = 0;
                listDate.ForEach(date =>
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        _indexCa++;
                        var _dataHeader = new DataHeaderModel();
                        _dataHeader.ColumnKey = "index_ca_" + _indexCa;
                        _dataHeader.ColumnValue = i % 2 != 0 ? "Sáng": "Chiều";
                        _dataHeader.Rowspan = 0;
                        _dataHeader.Colspan = 2;
                        _dataHeader.Width = "120px";

                        listHeader2.Add(_dataHeader);
                    }
                });

                listDataHeader.Add(listHeader2);

                /* tr3 */
                var listHeader3 = new List<DataHeaderModel>();
                listDate.ForEach(date =>
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        var _dataHeader = new DataHeaderModel();
                        _dataHeader.ColumnKey = i.ToString();
                        _dataHeader.ColumnValue = i % 2 != 0 ? "Vào" : "Ra";
                        _dataHeader.Rowspan = 0;
                        _dataHeader.Colspan = 0;
                        _dataHeader.Width = "60px";

                        listHeader3.Add(_dataHeader);
                    }
                });

                listDataHeader.Add(listHeader3);

                #endregion

                return new GetTkDiMuonVeSomResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListData = listData,
                    ListDataHeader = listDataHeader,
                    ListColorChamCong = listColorChamCong
                };
            }
            catch (Exception e)
            {
                return new GetTkDiMuonVeSomResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ImportChamCongResult ImportChamCong(ImportChamCongParameter parameter)
        {
            try
            {
                var listEmp = context.Employee.ToList();

                var listDataConvert = new List<DataChamCongConvertModel>();

                for (int i = 0; i < parameter.ListData.Count; i++)
                {
                    var item = parameter.ListData[i];
                    item.NoteError = "";

                    var emp = listEmp.FirstOrDefault(x =>
                        x.CodeMayChamCong != null && x.CodeMayChamCong.Trim() == item.No.Trim());

                    if (emp == null)
                    {
                        item.IsError = true;
                        item.NoteError = "<p>- Code trên máy chấm công không tồn tại trên hệ thống</p>";
                    }

                    DateTime thoiGianChamCong;
                    if (!DateTime.TryParseExact(item.ThoiGianChamCong, "M/d/yyyy h:mm:ss tt",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out thoiGianChamCong))
                    {
                        item.IsError = true;
                        item.NoteError += "<p>- Định dạng thời gian không hợp lệ</p>";
                    }

                    if (item.IsError != true)
                    {
                        var ngayChamCong = thoiGianChamCong.Date;

                        var dataConvert = new DataChamCongConvertModel();
                        dataConvert.EmployeeId = emp.EmployeeId;
                        dataConvert.NgayChamCong = ngayChamCong;
                        dataConvert.ThoiGianChamCong = thoiGianChamCong.TimeOfDay;

                        listDataConvert.Add(dataConvert);
                    }
                }

                var existsError = parameter.ListData.Count(x => x.IsError == true);
                if (existsError > 0)
                {
                    return new ImportChamCongResult()
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        MessageCode = "Có dữ liệu không hợp lệ",
                        ListDataError = parameter.ListData
                    };
                }

                var listCaLamViecChiTiet = context.CaLamViecChiTiet.ToList();
                var listCaLamViec = context.CaLamViec
                    .Select(y => new CaLamViecModel
                    {
                        CaLamViecId = y.CaLamViecId,
                        LoaiCaLamViecId = y.LoaiCaLamViecId,
                        GioVao = y.GioVao,
                        GioRa = y.GioRa,
                        ThoiGianKetThucCa = y.ThoiGianKetThucCa,
                        ListCaLamViecChiTiet = listCaLamViecChiTiet.Where(x => x.CaLamViecId == y.CaLamViecId)
                            .Select(z => new CaLamViecChiTietModel(z)).ToList()
                    }).ToList();

                var listCauHinhNghiLeChiTiet = context.CauHinhNghiLeChiTiet.ToList();
                var listCauHinhNghiLe = context.CauHinhNghiLe.Select(y => new CauHinhNghiLeModel
                {
                    NghiLeId = y.NghiLeId,
                    SoNam = y.SoNam,
                    ListCauHinhNghiLeChiTiet = listCauHinhNghiLeChiTiet.Where(x => x.NghiLeId == y.NghiLeId)
                        .Select(z => new CauHinhNghiLeChiTietModel(z)).ToList()
                }).ToList();

                var listKeHoachOt = context.KeHoachOt.Where(x => (x.TrangThai == 5 ||
                                                                  x.TrangThai == 6 ||
                                                                  x.TrangThai == 7) &&
                                                                 x.Active == true).ToList();
                var listKeHoachOtId = listKeHoachOt.Select(y => y.KeHoachOtId).ToList();
                var listKeHoachOtThanhVien = context.KeHoachOtThanhVien
                    .Where(x => x.KeHoachOtId != null && listKeHoachOtId.Contains(x.KeHoachOtId.Value)).ToList();
                var cauHinhChamCongOt = context.CauHinhChamCongOt.FirstOrDefault();
                var cauHinhOtCaNgay = context.CauHinhOtCaNgay.FirstOrDefault();

                // Lọc ra các ngày nghỉ
                var listAllNgay = listDataConvert.Select(y => y.NgayChamCong).Distinct().OrderBy(z => z).ToList();

                var listEmpId = listDataConvert.Select(y => y.EmployeeId).Distinct().ToList();
                var listChamCong = new List<ChamCong>();
                var listThongKeDiMuonVeSom = new List<ThongKeDiMuonVeSom>();
                var listChamCongOt = new List<ChamCongOt>();
                var listTongHopChamCongOt = new List<TongHopChamCongOt>();

                listAllNgay.ForEach(ngay =>
                {
                    var cauHinhNghiLe = listCauHinhNghiLe.FirstOrDefault(x => x.SoNam == ngay.Year);

                    bool isNgayNghi = false;
                    if (cauHinhNghiLe != null)
                    {
                        var listNgayNghi = cauHinhNghiLe.ListCauHinhNghiLeChiTiet
                            .Where(x => x.LoaiNghiLe == 1 || x.LoaiNghiLe == 2).Select(y => y.Ngay.Date)
                            .ToList();

                        var count = listNgayNghi.Count(x => x == ngay);
                        if (count > 0) isNgayNghi = true;
                    }

                    if (!isNgayNghi)
                    {
                        #region Chấm công thường

                        listEmpId.ForEach(empId =>
                        {
                            var listLanChamCongTrongNgay = listDataConvert
                                .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                            var listThoiGianChamCong = listLanChamCongTrongNgay.Select(y => y.ThoiGianChamCong).ToList();

                            //Nếu có dữ liệu chấm công
                            if (listThoiGianChamCong.Count > 0)
                            {
                                var ngayTrongTuan = ConvertNgayTrongTuan(ngay.DayOfWeek);
                                var _chamCong = new ChamCong();
                                var _listThongKeDiMuonVeSom = new List<ThongKeDiMuonVeSom>();
                                GetGioChamCong(empId, ngay, listThoiGianChamCong, listCaLamViec, ngayTrongTuan,
                                    out bool isNoData,
                                    out _chamCong, out _listThongKeDiMuonVeSom);

                                if (!isNoData)
                                {
                                    var chamCong = new ChamCong();
                                    chamCong.NgayChamCong = ngay;
                                    chamCong.CreatedDate = DateTime.Now;
                                    chamCong.CreatedById = parameter.UserId;
                                    chamCong.EmployeeId = empId;
                                    chamCong.VaoSang = _chamCong.VaoSang;
                                    chamCong.RaSang = _chamCong.RaSang;
                                    chamCong.VaoChieu = _chamCong.VaoChieu;
                                    chamCong.RaChieu = _chamCong.RaChieu;

                                    listChamCong.Add(chamCong);

                                    if (_listThongKeDiMuonVeSom.Count > 0)
                                    {
                                        listThongKeDiMuonVeSom.AddRange(_listThongKeDiMuonVeSom);
                                    }
                                }
                            }
                        });

                        #endregion
                    }

                    #region Chấm công OT

                    //List kế hoạch OT có: Ngày bắt đầu <= ngày <= Ngày kết thúc 
                    var _listKeHoachOt = listKeHoachOt.Where(x => x.NgayBatDau.Value.Date <= ngay &&
                                                                  x.NgayKetThuc.Value.Date >= ngay).ToList();

                    _listKeHoachOt.ForEach(keHoach =>
                    {
                        TimeSpan gioBatDauTinhChamCongOt;
                        TimeSpan gioBatDau;
                        TimeSpan gioKetThuc;

                        var _listKeHoachOtThanhVien = listKeHoachOtThanhVien
                            .Where(x => x.KeHoachOtId == keHoach.KeHoachOtId &&
                                        x.TrangThai == 3).ToList();

                        var listDangKyOtEmpId =
                            _listKeHoachOtThanhVien.Select(y => y.EmployeeId).Distinct().ToList();

                        //Nếu là Ca sáng
                        if (keHoach.LoaiCaId == 1 && cauHinhOtCaNgay != null)
                        {
                            gioBatDau = cauHinhOtCaNgay.GioVaoSang;
                            gioKetThuc = cauHinhOtCaNgay.GioRaSang;

                            listDangKyOtEmpId.ForEach(empId =>
                            {
                                var chamCongOt = new ChamCongOt();

                                var listLanChamCongTrongNgay = listDataConvert
                                    .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                                var listThoiGianChamCong = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong <= cauHinhOtCaNgay.GioKetThucSang)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCong.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoSang = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;

                                    listChamCongOt.Add(chamCongOt);
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCong.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoSang = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaSang = listThoiGianChamCong.Last() < gioKetThuc
                                        ? listThoiGianChamCong.Last()
                                        : gioKetThuc;

                                    listChamCongOt.Add(chamCongOt);
                                }
                            });
                        }
                        //Nếu là Ca chiều
                        else if (keHoach.LoaiCaId == 2 && cauHinhOtCaNgay != null)
                        {
                            gioBatDau = cauHinhOtCaNgay.GioVaoChieu;
                            gioKetThuc = cauHinhOtCaNgay.GioRaChieu;

                            listDangKyOtEmpId.ForEach(empId =>
                            {
                                var chamCongOt = new ChamCongOt();

                                var listLanChamCongTrongNgay = listDataConvert
                                    .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                                var listThoiGianChamCong = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong >= cauHinhOtCaNgay.GioKetThucSang &&
                                                x.ThoiGianChamCong <= cauHinhOtCaNgay.GioKetThucChieu)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCong.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoChieu = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;

                                    listChamCongOt.Add(chamCongOt);
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCong.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoChieu = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaChieu = listThoiGianChamCong.Last() < gioKetThuc
                                        ? listThoiGianChamCong.Last()
                                        : gioKetThuc;

                                    listChamCongOt.Add(chamCongOt);
                                }
                            });
                        }
                        //Nếu là Ca tối
                        else if (keHoach.LoaiCaId == 3)
                        {
                            gioBatDau = keHoach.GioBatDau.Value;
                            gioKetThuc = keHoach.GioKetThuc.Value;

                            //Nếu có cấu hình chấm công OT
                            if (cauHinhChamCongOt != null)
                            {
                                gioBatDauTinhChamCongOt =
                                    TimeSpan.FromMinutes(gioBatDau.TotalMinutes - (double) cauHinhChamCongOt.SoPhut);
                            }
                            else
                            {
                                gioBatDauTinhChamCongOt = gioBatDau;
                            }

                            listDangKyOtEmpId.ForEach(empId =>
                            {
                                var listLanChamCongTrongNgay = listDataConvert
                                    .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                                var listThoiGianChamCong = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong > gioBatDauTinhChamCongOt)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                var chamCongOt = new ChamCongOt();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCong.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoToi = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaToi = null;

                                    listChamCongOt.Add(chamCongOt);
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCong.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoToi = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaToi = listThoiGianChamCong.Last() < gioKetThuc
                                        ? listThoiGianChamCong.Last()
                                        : gioKetThuc;

                                    listChamCongOt.Add(chamCongOt);
                                }
                            });
                        }
                        //Nếu là Cả ngày 
                        else if (keHoach.LoaiCaId == 4 && cauHinhOtCaNgay != null)
                        {
                            gioBatDau = cauHinhOtCaNgay.GioVaoSang;
                            gioKetThuc = cauHinhOtCaNgay.GioRaChieu;

                            listDangKyOtEmpId.ForEach(empId =>
                            {
                                var chamCongOt = new ChamCongOt();

                                var listLanChamCongTrongNgay = listDataConvert
                                    .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                                #region Ca sáng

                                var listThoiGianChamCongSang = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong <= cauHinhOtCaNgay.GioKetThucSang)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCongSang.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoSang = listThoiGianChamCongSang.First() > gioBatDau
                                        ? listThoiGianChamCongSang.First()
                                        : gioBatDau;
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCongSang.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoSang = listThoiGianChamCongSang.First() > gioBatDau
                                        ? listThoiGianChamCongSang.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaSang = listThoiGianChamCongSang.Last() < gioKetThuc
                                        ? listThoiGianChamCongSang.Last()
                                        : gioKetThuc;
                                }

                                #endregion

                                #region Ca chiều

                                var listThoiGianChamCongChieu = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong >= cauHinhOtCaNgay.GioKetThucSang &&
                                                x.ThoiGianChamCong <= cauHinhOtCaNgay.GioKetThucChieu)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCongChieu.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoChieu = listThoiGianChamCongChieu.First() > gioBatDau
                                        ? listThoiGianChamCongChieu.First()
                                        : gioBatDau;
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCongChieu.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoChieu = listThoiGianChamCongChieu.First() > gioBatDau
                                        ? listThoiGianChamCongChieu.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaChieu = listThoiGianChamCongChieu.Last() < gioKetThuc
                                        ? listThoiGianChamCongChieu.Last()
                                        : gioKetThuc;
                                }

                                #endregion

                                if (chamCongOt.GioVaoSang != null || chamCongOt.GioVaoChieu != null)
                                {
                                    listChamCongOt.Add(chamCongOt);
                                }
                            });
                        }
                    });

                    #endregion
                });

                listChamCongOt = listChamCongOt.GroupBy(g => new {g.EmployeeId, g.NgayChamCong, g.LoaiOtId})
                    .Select(y => new ChamCongOt
                    {
                        EmployeeId = y.Key.EmployeeId,
                        NgayChamCong = y.Key.NgayChamCong,
                        LoaiOtId = y.Key.LoaiOtId,
                        GioVaoSang = y.FirstOrDefault(x => x.GioVaoSang != null)?.GioVaoSang,
                        GioRaSang = y.FirstOrDefault(x => x.GioRaSang != null)?.GioRaSang,
                        GioVaoChieu = y.FirstOrDefault(x => x.GioVaoChieu != null)?.GioVaoChieu,
                        GioRaChieu = y.FirstOrDefault(x => x.GioRaChieu != null)?.GioRaChieu,
                        GioVaoToi = y.FirstOrDefault(x => x.GioVaoToi != null)?.GioVaoToi,
                        GioRaToi = y.FirstOrDefault(x => x.GioRaToi != null)?.GioRaToi,
                    }).ToList();

                listTongHopChamCongOt = TinhTongHopChamCongOt(listChamCongOt);

                //Nếu chưa có cấu hình ca làm việc
                if (listChamCong.Count == 0 && listChamCongOt.Count == 0)
                {
                    return new ImportChamCongResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Chưa có cấu hình ca làm việc"
                    };
                }

                var listChamCongBatThuong = new List<DuLieuChamCongBatThuongModel>();

                using (var trans = context.Database.BeginTransaction())
                {
                    #region Chấm công thường

                    #region Xóa dữ liệu cũ

                    var listOldChamCong = (from a in context.ChamCong
                        join b in listChamCong on
                            new { a.EmployeeId, a.NgayChamCong } equals
                            new { b.EmployeeId, b.NgayChamCong }
                        select a).ToList();

                    var listOldThongKeDiMuonVeSom = (from a in context.ThongKeDiMuonVeSom
                        join b in listChamCong on
                            new { EmployeeId = a.EmployeeId, NgayChamCong = a.NgayDmvs } equals
                            new { EmployeeId = b.EmployeeId.Value, NgayChamCong = b.NgayChamCong }
                        select a).ToList();

                    context.ChamCong.RemoveRange(listOldChamCong);
                    context.ThongKeDiMuonVeSom.RemoveRange(listOldThongKeDiMuonVeSom);
                    context.SaveChanges();

                    #endregion

                    context.ChamCong.AddRange(listChamCong);
                    context.ThongKeDiMuonVeSom.AddRange(listThongKeDiMuonVeSom);
                    context.SaveChanges();

                    #endregion

                    #region Chấm công OT

                    #region Xóa dữ liệu cũ

                    var listOldChamCongOt = (from a in context.ChamCongOt
                        join b in listChamCongOt on
                            new {a.EmployeeId, a.NgayChamCong, a.LoaiOtId} equals
                            new {b.EmployeeId, b.NgayChamCong, b.LoaiOtId}
                        select a).ToList();

                    var listOldTongHopChamCongOt = (from a in context.TongHopChamCongOt
                        join b in listTongHopChamCongOt on
                            new {a.EmployeeId, a.NgayChamCong, a.LoaiOtId} equals
                            new {b.EmployeeId, b.NgayChamCong, b.LoaiOtId}
                        select a).ToList();

                    context.ChamCongOt.RemoveRange(listOldChamCongOt);
                    context.TongHopChamCongOt.RemoveRange(listOldTongHopChamCongOt);
                    context.SaveChanges();

                    #endregion

                    context.ChamCongOt.AddRange(listChamCongOt);
                    context.TongHopChamCongOt.AddRange(listTongHopChamCongOt);
                    context.SaveChanges();

                    #endregion

                    #region Lưu lịch sử vào dòng thòi gian

                    var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                    var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                    var contentNote = "";
                    contentNote = "<p>- <strong>" + emp.EmployeeCode + " - " + emp.EmployeeName +
                                  "</strong> đã import file chấm công</p>";

                    Note note = new Note
                    {
                        NoteId = Guid.NewGuid(),
                        Description = contentNote,
                        Type = "ADD",
                        ObjectId = Guid.Empty,
                        ObjectNumber = 1,
                        ObjectType = NoteObjectType.TKCCT,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        NoteTitle = "đã thêm ghi chú"
                    };

                    context.Note.Add(note);
                    context.SaveChanges();

                    #endregion

                    #region Kiểm tra dữ liệu

                    //Nếu có bản ghi có dữ liệu vào nhưng không có dữ liệu ra thì gửi email theo cấu hình
                    var _listChamCongBatThuong = listChamCong.Where(x =>
                            (x.VaoSang != null && x.RaSang == null) || (x.VaoChieu != null && x.RaChieu == null))
                        .Select(y => new DuLieuChamCongBatThuongModel
                        {
                            EmployeeId = y.EmployeeId.Value,
                            NgayChamCong = y.NgayChamCong,
                            VaoSang = y.VaoSang,
                            RaSang = y.RaSang,
                            VaoChieu = y.VaoChieu,
                            RaChieu = y.RaChieu,
                        }).ToList();

                    var listChamCongOtBatThuong = listChamCongOt.Where(x =>
                            (x.GioVaoSang != null && x.GioRaSang == null) ||
                            (x.GioVaoChieu != null && x.GioRaChieu == null) ||
                            (x.GioVaoToi != null && x.GioRaToi == null))
                        .Select(y => new DuLieuChamCongBatThuongModel
                        {
                            EmployeeId = y.EmployeeId,
                            NgayChamCong = y.NgayChamCong,
                            VaoSang = y.GioVaoSang,
                            RaSang = y.GioRaSang,
                            VaoChieu = y.GioVaoChieu,
                            RaChieu = y.GioRaChieu,
                            VaoToi = y.GioVaoToi,
                            RaToi = y.GioRaToi
                        })
                        .ToList();

                    _listChamCongBatThuong.AddRange(listChamCongOtBatThuong);

                    _listChamCongBatThuong.ForEach(item =>
                    {
                        if (item.VaoSang != null && item.RaSang == null)
                        {
                            var newItem = new DuLieuChamCongBatThuongModel
                            {
                                EmployeeId = item.EmployeeId,
                                NgayChamCong = item.NgayChamCong,
                                VaoSang = item.VaoSang,
                                RaSang = item.RaSang,
                                VaoChieu = item.VaoChieu,
                                RaChieu = item.RaChieu,
                                VaoToi = item.VaoToi,
                                RaToi = item.RaToi,
                                CaChamCong = 1
                            };

                            listChamCongBatThuong.Add(newItem);
                        }

                        if (item.VaoChieu != null && item.RaChieu == null)
                        {
                            var newItem = new DuLieuChamCongBatThuongModel
                            {
                                EmployeeId = item.EmployeeId,
                                NgayChamCong = item.NgayChamCong,
                                VaoSang = item.VaoSang,
                                RaSang = item.RaSang,
                                VaoChieu = item.VaoChieu,
                                RaChieu = item.RaChieu,
                                VaoToi = item.VaoToi,
                                RaToi = item.RaToi,
                                CaChamCong = 2
                            };

                            listChamCongBatThuong.Add(newItem);
                        }

                        if (item.VaoToi != null && item.RaToi == null)
                        {
                            var newItem = new DuLieuChamCongBatThuongModel
                            {
                                EmployeeId = item.EmployeeId,
                                NgayChamCong = item.NgayChamCong,
                                VaoSang = item.VaoSang,
                                RaSang = item.RaSang,
                                VaoChieu = item.VaoChieu,
                                RaChieu = item.RaChieu,
                                VaoToi = item.VaoToi,
                                RaToi = item.RaToi,
                                CaChamCong = 3
                            };

                            listChamCongBatThuong.Add(newItem);
                        }
                    });

                    #endregion

                    trans.Commit();
                }

                return new ImportChamCongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Import thành công",
                    ListChamCongBatThuong = listChamCongBatThuong
                };
            }
            catch (Exception e)
            {
                return new ImportChamCongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterThongKeChamCongResult GetMasterThongKeChamCong(GetMasterThongKeChamCongParameter parameter)
        {
            try
            {
                var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong")
                    .Where(x => x.Value != 12 && x.Value != 13).ToList();

                var listEmployee = context.Employee
                    .Where(x => !String.IsNullOrWhiteSpace(x.CodeMayChamCong))
                    .Select(y => new EmployeeEntityModel
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeCode = y.EmployeeCode,
                        EmployeeName = y.EmployeeName,
                        EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                    }).OrderBy(z => z.EmployeeName).ToList();

                return new GetMasterThongKeChamCongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListKyHieuChamCong = listKyHieuChamCong,
                    ListEmployee = listEmployee,
                };
            }
            catch (Exception e)
            {
                return new GetMasterThongKeChamCongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateChamCongResult CreateOrUpdateChamCong(CreateOrUpdateChamCongParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //Lấy list ngày không được phép sửa: Là những ngày nằm trong các kỳ lương có trạng thái khác Mới
                var listKyLuong = context.KyLuong.Where(x => x.TrangThai != 1).ToList();
                var listDateNotUpdate = GetListNgayKhongDuocPhepSuaChamCong(listKyLuong);

                var exists = listDateNotUpdate.FirstOrDefault(x => x == parameter.NgayChamCong.Date);

                //Nếu thuộc ngày không được phép sửa thì báo lỗi
                if (exists != null && exists != DateTime.MinValue && exists != DateTime.MaxValue)
                {
                    return new CreateOrUpdateChamCongResult()
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        MessageCode = "Thông tin chấm công hiện tại thuộc kỳ lương đã khóa tính năng cập nhật dữ liệu"
                    };
                }

                using (var trans = context.Database.BeginTransaction())
                {
                    var listCaLamViecChiTiet = context.CaLamViecChiTiet.ToList();
                    var listCaLamViec = context.CaLamViec
                        .Select(y => new CaLamViecModel
                        {
                            CaLamViecId = y.CaLamViecId,
                            LoaiCaLamViecId = y.LoaiCaLamViecId,
                            GioVao = y.GioVao,
                            GioRa = y.GioRa,
                            ThoiGianKetThucCa = y.ThoiGianKetThucCa,
                            ListCaLamViecChiTiet = listCaLamViecChiTiet.Where(x => x.CaLamViecId == y.CaLamViecId)
                                .Select(z => new CaLamViecChiTietModel(z)).ToList()
                        }).ToList();

                    var listEmpKyHieu = new List<string>();
                    var oldKyHieu = "--";
                    var caKyHieu = "";

                    if (parameter.TypeUpdate == "CA")
                    {
                        //Ca sáng
                        if (parameter.TypeField == 1)
                        {
                            caKyHieu = "ca sáng";
                        }
                        //Ca chiều
                        else if (parameter.TypeField == 2)
                        {
                            caKyHieu = "ca chiều";
                        }
                    }
                    else if (parameter.TypeUpdate == "NGAY")
                    {
                        caKyHieu = "ca sáng và ca chiều";
                    }

                    var listKyHieu = GeneralList.GetTrangThais("KyHieuChamCong");
                    var newKyHieu = (parameter.MaKyHieu == null || parameter.MaKyHieu == 0)
                        ? "--"
                        : listKyHieu.FirstOrDefault(x => x.Value == parameter.MaKyHieu).ValueText;
                    var ngayKyHieu = parameter.NgayChamCong.ToString("dd/MM/yyyy");

                    bool isKhongDuPhep = false;

                    for (int i = 0; i < parameter.ListEmployeeId.Count; i++)
                    {
                        var EmployeeId = parameter.ListEmployeeId[i];
                        var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == EmployeeId);
                        employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep ?? 0;
                        employee.SoNgayPhepConLai = employee.SoNgayPhepConLai ?? 0;

                        var chamCong = context.ChamCong
                            .FirstOrDefault(x => x.EmployeeId == EmployeeId &&
                                                 x.NgayChamCong.Date == parameter.NgayChamCong.Date);

                        var ngayTrongTuan = ConvertNgayTrongTuan(parameter.NgayChamCong.DayOfWeek);
                        var _chamCong = new ChamCong();
                        var _listThongKeDiMuonVeSom = new List<ThongKeDiMuonVeSom>();
                        var listThoiGianChamCong = new List<TimeSpan>();

                        //Nếu là Giờ
                        if (parameter.TypeUpdate == "GIO")
                        {
                            int loaiCaLamViecId = 0;
                            if (parameter.TypeField == 1 || parameter.TypeField == 2)
                            {
                                loaiCaLamViecId = 1;
                            }
                            else if (parameter.TypeField == 3 || parameter.TypeField == 4)
                            {
                                loaiCaLamViecId = 2;
                            }

                            var caLamViec =
                                CheckExistsCauHinhChamCong(listCaLamViec, ngayTrongTuan, loaiCaLamViecId);

                            if (caLamViec == null)
                            {
                                return new CreateOrUpdateChamCongResult()
                                {
                                    StatusCode = HttpStatusCode.NotFound,
                                    MessageCode = "Chưa có cấu hình ca làm việc"
                                };
                            }

                            var oldThoiGian = "--";
                            var newThoiGian = parameter.ThoiGian == null ? "--" : parameter.ThoiGian.Value.ToString(@"hh\:mm");
                            var ngay = parameter.NgayChamCong.ToString("dd/MM/yyyy");

                            //Create
                            if (chamCong == null)
                            {
                                //Ra sáng hoặc Ra chiều
                                if (parameter.TypeField == 2 || parameter.TypeField == 4)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(null, parameter.ThoiGian);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }
                                }

                                if (parameter.ThoiGian != null)
                                {
                                    #region Tính lại thống kê đi muộn về sớm

                                    var thongKeDmvs = new ThongKeDiMuonVeSom();
                                    GetThongKeDmvsTheoThoiGian(caLamViec, parameter.TypeField,
                                        parameter.ThoiGian.Value, out thongKeDmvs);

                                    //Nếu đi muộn về sớm
                                    if (thongKeDmvs.ThoiGian > 0)
                                    {
                                        thongKeDmvs.EmployeeId = EmployeeId;
                                        thongKeDmvs.NgayDmvs = parameter.NgayChamCong.Date;
                                        context.ThongKeDiMuonVeSom.Add(thongKeDmvs);
                                        context.SaveChanges();
                                    }

                                    #endregion

                                    //Vào sáng
                                    if (parameter.TypeField == 1)
                                    {
                                        _chamCong.VaoSang = parameter.ThoiGian;
                                    }
                                    //Ra sáng
                                    else if (parameter.TypeField == 2)
                                    {
                                        _chamCong.RaSang = parameter.ThoiGian;
                                    }
                                    //Vào chiều
                                    else if (parameter.TypeField == 3)
                                    {
                                        _chamCong.VaoChieu = parameter.ThoiGian;
                                    }
                                    //Ra chiều
                                    else if (parameter.TypeField == 4)
                                    {
                                        _chamCong.RaChieu = parameter.ThoiGian;
                                    }

                                    _chamCong.NgayChamCong = parameter.NgayChamCong.Date;
                                    _chamCong.CreatedDate = DateTime.Now;
                                    _chamCong.CreatedById = parameter.UserId;
                                    _chamCong.EmployeeId = EmployeeId;

                                    context.ChamCong.Add(_chamCong);
                                    context.SaveChanges();
                                }
                            }
                            //Update
                            else
                            {
                                #region Xóa thống kê đi muộn về sớm

                                var oldThongKeDmvs = context.ThongKeDiMuonVeSom.FirstOrDefault(x =>
                                    x.EmployeeId == EmployeeId &&
                                    x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                    x.LoaiDmvs == parameter.TypeField);

                                if (oldThongKeDmvs != null)
                                {
                                    context.ThongKeDiMuonVeSom.Remove(oldThongKeDmvs);
                                    context.SaveChanges();
                                }

                                #endregion

                                #region Tính lại thống kê đi muộn về sớm

                                if (parameter.ThoiGian != null)
                                {
                                    var thongKeDmvs = new ThongKeDiMuonVeSom();
                                    GetThongKeDmvsTheoThoiGian(caLamViec, parameter.TypeField,
                                        parameter.ThoiGian.Value, out thongKeDmvs);

                                    //Nếu đi muộn về sớm
                                    if (thongKeDmvs.ThoiGian > 0)
                                    {
                                        thongKeDmvs.EmployeeId = EmployeeId;
                                        thongKeDmvs.NgayDmvs = parameter.NgayChamCong.Date;
                                        context.ThongKeDiMuonVeSom.Add(thongKeDmvs);
                                        context.SaveChanges();
                                    }
                                }

                                #endregion

                                //Vào sáng
                                if (parameter.TypeField == 1)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(parameter.ThoiGian, chamCong.RaSang);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }

                                    oldThoiGian = chamCong.VaoSang == null
                                        ? "--"
                                        : chamCong.VaoSang.Value.ToString(@"hh\:mm");

                                    chamCong.VaoSang = parameter.ThoiGian;

                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();
                                }
                                //Ra sáng
                                else if (parameter.TypeField == 2)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(chamCong.VaoSang, parameter.ThoiGian);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }

                                    oldThoiGian = chamCong.RaSang == null
                                        ? "--"
                                        : chamCong.RaSang.Value.ToString(@"hh\:mm");

                                    chamCong.RaSang = parameter.ThoiGian;

                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();
                                }
                                //Vào chiều
                                else if (parameter.TypeField == 3)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(parameter.ThoiGian, chamCong.RaChieu);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }

                                    oldThoiGian = chamCong.VaoChieu == null
                                        ? "--"
                                        : chamCong.VaoChieu.Value.ToString(@"hh\:mm");

                                    chamCong.VaoChieu = parameter.ThoiGian;

                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();
                                }
                                //Ra chiều
                                else if (parameter.TypeField == 4)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(chamCong.VaoChieu, parameter.ThoiGian);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }

                                    oldThoiGian = chamCong.RaChieu == null
                                        ? "--"
                                        : chamCong.RaChieu.Value.ToString(@"hh\:mm");

                                    chamCong.RaChieu = parameter.ThoiGian;

                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();
                                }
                            }

                            #region Lưu lịch sử vào dòng thòi gian
                            
                            //var _emp = context.Employee.FirstOrDefault(x => x.EmployeeId == EmployeeId);
                            var ca = "";

                            //Vào sáng
                            if (parameter.TypeField == 1)
                            {
                                ca = "vào sáng";
                            }
                            //Ra sáng
                            else if (parameter.TypeField == 2)
                            {
                                ca = "ra sáng";
                            }
                            //Vào chiều
                            else if (parameter.TypeField == 3)
                            {
                                ca = "vào chiều";
                            }
                            //Ra chiều
                            else if (parameter.TypeField == 4)
                            {
                                ca = "ra chiều";
                            }

                            var contentNote = "";
                            contentNote = "<p>- <strong>" + emp.EmployeeCode + " - " + emp.EmployeeName +
                                          "</strong> đã thay đổi giờ " + ca +
                                          " của <strong>" + employee.EmployeeCode + " - " + employee.EmployeeName +
                                          "</strong> từ <strong>\"" + oldThoiGian + "\"</strong> sang <strong>\"" + newThoiGian +
                                          "\"</strong> trong ngày <strong>" + ngay + "</strong></p>";

                            Note note = new Note
                            {
                                NoteId = Guid.NewGuid(),
                                Description = contentNote,
                                Type = "ADD",
                                ObjectId = Guid.Empty,
                                ObjectNumber = 1,
                                ObjectType = NoteObjectType.TKCCT,
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                NoteTitle = "đã thêm ghi chú"
                            };

                            context.Note.Add(note);
                            context.SaveChanges();

                            #endregion
                        }
                        //Nếu là Ca
                        else if (parameter.TypeUpdate == "CA")
                        {
                            //Create
                            if (chamCong == null)
                            {
                                //Nếu là Nghỉ phép thì sẽ kiểm tra xem nhân viên còn phép hay không
                                if (parameter.MaKyHieu == 1)
                                {
                                    //Nếu không còn phép thì bỏ qua
                                    if ((decimal) 0.5 > employee.SoNgayPhepConLai)
                                    {
                                        isKhongDuPhep = true;
                                        continue;
                                    }
                                    //Nếu còn phép thì trừ đi số ngày phép còn lại
                                    else
                                    {
                                        employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + (decimal) 0.5;
                                        employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - (decimal) 0.5;
                                        context.Employee.Update(employee);
                                    }
                                }

                                _chamCong.EmployeeId = EmployeeId;
                                _chamCong.NgayChamCong = parameter.NgayChamCong.Date;
                                _chamCong.CreatedById = parameter.UserId;
                                _chamCong.CreatedDate = DateTime.Now;

                                //Ca sáng
                                if (parameter.TypeField == 1)
                                {
                                    _chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                    _chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                }
                                //Ca chiều
                                else if (parameter.TypeField == 2)
                                {
                                    _chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                    _chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                }

                                context.ChamCong.Add(_chamCong);
                                context.SaveChanges();

                                #region Lưu lịch sử vào dòng thòi gian

                                listEmpKyHieu.Add("<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                  "</strong> từ <strong>\"" + oldKyHieu + "\"</strong> sang <strong>\"" +
                                                  newKyHieu + "\"</strong></p>");

                                #endregion
                            }
                            //Update
                            else
                            {
                                //Nếu xóa ký hiệu
                                if (parameter.MaKyHieu == null)
                                {
                                    #region Tính lại thống kê đi muộn về sớm

                                    if (chamCong.VaoSang != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.VaoSang.Value);
                                    }

                                    if (chamCong.RaSang != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.RaSang.Value);
                                    }

                                    if (chamCong.VaoChieu != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.VaoChieu.Value);
                                    }

                                    if (chamCong.RaChieu != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.RaChieu.Value);
                                    }

                                    GetGioChamCong(EmployeeId, parameter.NgayChamCong.Date,
                                        listThoiGianChamCong,
                                        listCaLamViec, ngayTrongTuan,
                                        out bool isNoData,
                                        out _chamCong, out _listThongKeDiMuonVeSom);

                                    //Ca sáng
                                    if (parameter.TypeField == 1)
                                    {
                                        //Nếu ký hiệu cũ là nghỉ phép thì sẽ cộng thêm lại vào tổng số ngày phép còn lại của nhân viên
                                        if (chamCong.KyHieuVaoSang == 1)
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal) 0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal) 0.5;
                                            context.Employee.Update(employee);
                                        }

                                        oldKyHieu = chamCong.KyHieuVaoSang == null ? "--" : listKyHieu.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)
                                            .ValueText;

                                        chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                        chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                        context.ChamCong.Update(chamCong);
                                        context.SaveChanges();

                                        #region Xóa thống kê đi muộn về sớm

                                        var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                            x.EmployeeId == EmployeeId &&
                                            x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                            (x.LoaiDmvs == 1 || x.LoaiDmvs == 2)).ToList();

                                        if (oldThongKeDmvs.Count > 0)
                                        {
                                            context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                            context.SaveChanges();
                                        }

                                        #endregion

                                        //Nếu là đi muộn về sớm thì
                                        if (_listThongKeDiMuonVeSom.Count > 0)
                                        {
                                            var listTk = _listThongKeDiMuonVeSom
                                                .Where(x => x.LoaiDmvs == 1 || x.LoaiDmvs == 2).ToList();

                                            if (listTk.Count > 0)
                                            {
                                                context.ThongKeDiMuonVeSom.AddRange(listTk);
                                                context.SaveChanges();
                                            }
                                        }

                                        #region Lưu lịch sử vào dòng thòi gian
                                        
                                        listEmpKyHieu.Add("<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                          "</strong> từ <strong>\"" + oldKyHieu + "\"</strong> sang <strong>\"" +
                                                          newKyHieu + "\"</strong></p>");

                                        #endregion
                                    }
                                    //Ca chiều
                                    else
                                    {
                                        //Nếu ký hiệu cũ là nghỉ phép thì sẽ cộng thêm lại vào tổng số ngày phép còn lại của nhân viên
                                        if (chamCong.KyHieuVaoChieu == 1)
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal)0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal)0.5;
                                            context.Employee.Update(employee);
                                        }

                                        oldKyHieu = chamCong.KyHieuVaoChieu == null ? "--" : listKyHieu.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)
                                            .ValueText;

                                        chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                        chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                        context.ChamCong.Update(chamCong);
                                        context.SaveChanges();

                                        #region Xóa thống kê đi muộn về sớm

                                        var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                            x.EmployeeId == EmployeeId &&
                                            x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                            (x.LoaiDmvs == 3 || x.LoaiDmvs == 4)).ToList();

                                        if (oldThongKeDmvs.Count > 0)
                                        {
                                            context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                            context.SaveChanges();
                                        }

                                        #endregion

                                        //Nếu là đi muộn về sớm thì
                                        if (_listThongKeDiMuonVeSom.Count > 0)
                                        {
                                            var listTk = _listThongKeDiMuonVeSom
                                                .Where(x => x.LoaiDmvs == 3 || x.LoaiDmvs == 4).ToList();

                                            if (listTk.Count > 0)
                                            {
                                                context.ThongKeDiMuonVeSom.AddRange(listTk);
                                                context.SaveChanges();
                                            }
                                        }

                                        #region Lưu lịch sử vào dòng thòi gian
                                        
                                        listEmpKyHieu.Add("<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                          "</strong> từ <strong>\"" + oldKyHieu + "\"</strong> sang <strong>\"" +
                                                          newKyHieu + "\"</strong></p>");

                                        #endregion
                                    }

                                    #endregion
                                }
                                //Nếu sửa ký hiệu
                                else
                                {
                                    //Ca sáng
                                    if (parameter.TypeField == 1)
                                    {
                                        //Nếu ký hiệu cũ là nghỉ phép và ký hiệu mới khác nghỉ phép thì sẽ cộng thêm lại vào tổng số ngày phép còn lại của nhân viên
                                        if (chamCong.KyHieuVaoSang == 1 && parameter.MaKyHieu != 1)
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal)0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal)0.5;
                                            context.Employee.Update(employee);
                                        }
                                        //Nếu ký hiệu cũ khác Nghỉ phép và ký hiệu mới là Nghỉ phép thì sẽ trừ đi số ngày phép còn lại của nhân viên
                                        else if (chamCong.KyHieuVaoSang != 1 && parameter.MaKyHieu == 1)
                                        {
                                            //Nếu không còn phép thì bỏ qua
                                            if ((decimal)0.5 > employee.SoNgayPhepConLai)
                                            {
                                                isKhongDuPhep = true;
                                                continue;
                                            }
                                            //Nếu còn phép thì trừ đi số ngày phép còn lại
                                            else
                                            {
                                                employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + (decimal)0.5;
                                                employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - (decimal)0.5;
                                                context.Employee.Update(employee);
                                            }
                                        }

                                        #region Xóa thống kê đi muộn về sớm

                                        var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                            x.EmployeeId == EmployeeId &&
                                            x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                            (x.LoaiDmvs == 1 || x.LoaiDmvs == 2)).ToList();

                                        if (oldThongKeDmvs.Count > 0)
                                        {
                                            context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                            context.SaveChanges();
                                        }

                                        #endregion

                                        oldKyHieu = chamCong.KyHieuVaoSang == null ? "--" : listKyHieu.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)
                                            .ValueText;

                                        chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                        chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                        context.ChamCong.Update(chamCong);
                                        context.SaveChanges();

                                        #region Lưu lịch sử vào dòng thòi gian
                                        
                                        listEmpKyHieu.Add("<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                          "</strong> từ <strong>\"" + oldKyHieu + "\"</strong> sang <strong>\"" +
                                                          newKyHieu + "\"</strong></p>");

                                        #endregion
                                    }
                                    //Ca chiều
                                    else
                                    {
                                        //Nếu ký hiệu cũ là nghỉ phép và ký hiệu mới khác nghỉ phép thì sẽ cộng thêm lại vào tổng số ngày phép còn lại của nhân viên
                                        if (chamCong.KyHieuVaoChieu == 1 && parameter.MaKyHieu != 1)
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal)0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal)0.5;
                                            context.Employee.Update(employee);
                                        }
                                        //Nếu ký hiệu cũ khác Nghỉ phép và ký hiệu mới là Nghỉ phép thì sẽ trừ đi số ngày phép còn lại của nhân viên
                                        else if (chamCong.KyHieuVaoChieu != 1 && parameter.MaKyHieu == 1)
                                        {
                                            //Nếu không còn phép thì bỏ qua
                                            if ((decimal)0.5 > employee.SoNgayPhepConLai)
                                            {
                                                isKhongDuPhep = true;
                                                continue;
                                            }
                                            //Nếu còn phép thì trừ đi số ngày phép còn lại
                                            else
                                            {
                                                employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + (decimal)0.5;
                                                employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - (decimal)0.5;
                                                context.Employee.Update(employee);
                                            }
                                        }

                                        #region Xóa thống kê đi muộn về sớm

                                        var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                            x.EmployeeId == EmployeeId &&
                                            x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                            (x.LoaiDmvs == 3 || x.LoaiDmvs == 4)).ToList();

                                        if (oldThongKeDmvs.Count > 0)
                                        {
                                            context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                            context.SaveChanges();
                                        }

                                        #endregion

                                        oldKyHieu = chamCong.KyHieuVaoChieu == null ? "--" : listKyHieu.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)
                                            .ValueText;

                                        chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                        chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                        context.ChamCong.Update(chamCong);
                                        context.SaveChanges();

                                        #region Lưu lịch sử vào dòng thòi gian
                                        
                                        listEmpKyHieu.Add("<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                          "</strong> từ <strong>\"" + oldKyHieu + "\"</strong> sang <strong>\"" +
                                                          newKyHieu + "\"</strong></p>");

                                        #endregion
                                    }
                                }
                            }
                        }
                        //Nếu là Ngày
                        else if (parameter.TypeUpdate == "NGAY")
                        {
                            //Create
                            if (chamCong == null)
                            {
                                //Nếu là Nghỉ phép thì sẽ kiểm tra xem nhân viên còn phép hay không
                                if (parameter.MaKyHieu == 1)
                                {
                                    //Nếu không còn phép thì bỏ qua
                                    if (1 > employee.SoNgayPhepConLai)
                                    {
                                        isKhongDuPhep = true;
                                        continue;
                                    }
                                    //Nếu còn phép thì trừ đi số ngày phép còn lại
                                    else
                                    {
                                        employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + 1;
                                        employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - 1;
                                        context.Employee.Update(employee);
                                    }
                                }

                                _chamCong.EmployeeId = EmployeeId;
                                _chamCong.NgayChamCong = parameter.NgayChamCong.Date;
                                _chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                _chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                _chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                _chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                _chamCong.CreatedById = parameter.UserId;
                                _chamCong.CreatedDate = DateTime.Now;

                                context.ChamCong.Add(_chamCong);
                                context.SaveChanges();

                                #region Lưu lịch sử vào dòng thòi gian

                                listEmpKyHieu.Add("<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                  "</strong> từ <strong>\"" + oldKyHieu + "\"</strong> sang <strong>\"" +
                                                  newKyHieu + "\"</strong></p>");

                                #endregion
                            }
                            //Update 
                            else
                            {
                                #region Kiểm tra số ngày phép

                                //Nếu ký hiệu mới là Nghỉ phép 
                                if (parameter.MaKyHieu == 1)
                                {
                                    //Nếu ký hiệu cũ khác Nghỉ phép và ký hiệu mới là Nghỉ phép (cả ngày)
                                    if (chamCong.KyHieuVaoSang != 1 && chamCong.KyHieuVaoChieu != 1)
                                    {
                                        //Nếu không còn phép thì bỏ qua
                                        if (1 > employee.SoNgayPhepConLai)
                                        {
                                            isKhongDuPhep = true;
                                            continue;
                                        }
                                        //Nếu còn phép thì trừ đi số ngày phép còn lại
                                        else
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + 1;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - 1;
                                            context.Employee.Update(employee);
                                        }
                                    }
                                    //Nếu ký hiệu cũ khác Nghỉ phép và ký hiệu mới là Nghỉ phép (nửa ngày)
                                    else if ((chamCong.KyHieuVaoSang == 1 && chamCong.KyHieuVaoChieu != 1) ||
                                              (chamCong.KyHieuVaoSang != 1 && chamCong.KyHieuVaoChieu == 1))
                                    {
                                        //Nếu không còn phép thì bỏ qua
                                        if ((decimal) 0.5 > employee.SoNgayPhepConLai)
                                        {
                                            isKhongDuPhep = true;
                                            continue;
                                        }
                                        //Nếu còn phép thì trừ đi số ngày phép còn lại
                                        else
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + (decimal) 0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - (decimal) 0.5;
                                            context.Employee.Update(employee);
                                        }
                                    }
                                }
                                //Nếu ký hiệu mới không phải nghỉ phép
                                else
                                {
                                    //Nếu ký hiệu cũ là Nghỉ phép (cả ngày)
                                    if (chamCong.KyHieuVaoSang == 1 && chamCong.KyHieuVaoChieu == 1)
                                    {
                                        employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - 1;
                                        employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + 1;
                                        context.Employee.Update(employee);
                                    }
                                    //Nếu ký hiệu cũ có ca ngày hoặc ca sáng là Nghỉ phép (nửa ngày)
                                    else if ((chamCong.KyHieuVaoSang == 1 && chamCong.KyHieuVaoChieu != 1) ||
                                             chamCong.KyHieuVaoSang != 1 && chamCong.KyHieuVaoChieu == 1)
                                    {
                                        employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal)0.5;
                                        employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal) 0.5;
                                        context.Employee.Update(employee);
                                    }
                                }

                                #endregion

                                #region Xóa thống kê đi muộn về sớm

                                var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                    x.EmployeeId == EmployeeId &&
                                    x.NgayDmvs.Date == parameter.NgayChamCong.Date).ToList();

                                if (oldThongKeDmvs.Count > 0)
                                {
                                    context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                    context.SaveChanges();
                                }

                                #endregion

                                //Nếu xóa ký hiệu
                                if (parameter.MaKyHieu == null)
                                {
                                    #region Tính lại thống kê đi muộn về sớm

                                    if (chamCong.VaoSang != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.VaoSang.Value);
                                    }

                                    if (chamCong.RaSang != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.RaSang.Value);
                                    }

                                    if (chamCong.VaoChieu != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.VaoChieu.Value);
                                    }

                                    if (chamCong.RaChieu != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.RaChieu.Value);
                                    }

                                    GetGioChamCong(EmployeeId, parameter.NgayChamCong.Date,
                                        listThoiGianChamCong,
                                        listCaLamViec, ngayTrongTuan,
                                        out bool isNoData,
                                        out _chamCong, out _listThongKeDiMuonVeSom);

                                    string oldKyHieuSang = chamCong.KyHieuVaoSang == null
                                        ? "--"
                                        : listKyHieu.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)
                                            .ValueText;
                                    string oldKyHieuChieu = chamCong.KyHieuVaoSang == null
                                        ? "--"
                                        : listKyHieu.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)
                                            .ValueText;

                                    chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                    chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                    chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                    chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();

                                    //Nếu là đi muộn về sớm thì
                                    if (_listThongKeDiMuonVeSom.Count > 0)
                                    {
                                        context.ThongKeDiMuonVeSom.AddRange(_listThongKeDiMuonVeSom);
                                        context.SaveChanges();
                                    }

                                    #region Lưu lịch sử vào dòng thòi gian

                                    listEmpKyHieu.Add("<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                      "</strong> ca sáng từ <strong>\"" + oldKyHieuSang + "\"</strong> sang <strong>\"" +
                                                      newKyHieu + "\"</strong></p>" +
                                                      "<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                      "</strong> ca chiều từ <strong>\"" + oldKyHieuChieu + "\"</strong> sang <strong>\"" +
                                                      newKyHieu + "\"</strong></p>");

                                    #endregion

                                    #endregion
                                }
                                //Nếu sửa ký hiệu
                                else
                                {
                                    string oldKyHieuSang = chamCong.KyHieuVaoSang == null
                                        ? "--"
                                        : listKyHieu.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)
                                            .ValueText;
                                    string oldKyHieuChieu = chamCong.KyHieuVaoSang == null
                                        ? "--"
                                        : listKyHieu.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)
                                            .ValueText;

                                    chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                    chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                    chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                    chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();

                                    #region Lưu lịch sử vào dòng thòi gian

                                    listEmpKyHieu.Add("<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                      "</strong> ca sáng từ <strong>\"" + oldKyHieuSang + "\"</strong> sang <strong>\"" +
                                                      newKyHieu + "\"</strong></p>" +
                                                      "<p><strong>+ " + employee.EmployeeCode + " - " + employee.EmployeeName +
                                                      "</strong> ca chiều từ <strong>\"" + oldKyHieuChieu + "\"</strong> sang <strong>\"" +
                                                      newKyHieu + "\"</strong></p>");

                                    #endregion
                                }
                            }
                        }
                    }

                    if (isKhongDuPhep)
                    {
                        return new CreateOrUpdateChamCongResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Nhân viên được chọn đã hết ngày nghỉ phép"
                        };
                    }

                    if (listEmpKyHieu.Count > 0)
                    {
                        var cacNhanVien = listEmpKyHieu.Join("");
                        var contentNoteKyHieu = "<p>- <strong>" + emp.EmployeeCode + " - " + emp.EmployeeName +
                                      "</strong> đã thay đổi ký hiệu " + caKyHieu +
                                      " trong ngày <strong>" + ngayKyHieu + "</strong> của:</p><p></p>" + cacNhanVien;

                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            Description = contentNoteKyHieu,
                            Type = "ADD",
                            ObjectId = Guid.Empty,
                            ObjectNumber = 1,
                            ObjectType = NoteObjectType.TKCCT,
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = "đã thêm ghi chú"
                        };

                        context.Note.Add(note);
                        context.SaveChanges();
                    }

                    trans.Commit();

                    return new CreateOrUpdateChamCongResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateChamCongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhChamCongOtResult CreateOrUpdateCauHinhChamCongOt(
            CreateOrUpdateCauHinhChamCongOtParameter parameter)
        {
            try
            {
                string mess = "";

                //Create
                if (parameter.CauHinhChamCongOt.CauHinhChamCongOtId == null)
                {
                    var cauHinh = parameter.CauHinhChamCongOt;
                    cauHinh.CreatedById = parameter.UserId;

                    context.CauHinhChamCongOt.Add(cauHinh.ToEntity());
                    context.SaveChanges();

                    mess = "Thêm mới thành công";
                }
                //Update
                else
                {
                    var cauHinh = context.CauHinhChamCongOt.FirstOrDefault(x =>
                        x.CauHinhChamCongOtId == parameter.CauHinhChamCongOt.CauHinhChamCongOtId);
                    if (cauHinh == null)
                    {
                        return new CreateOrUpdateCauHinhChamCongOtResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Cấu hình không tồn tại trên hệ thống"
                        };
                    }

                    cauHinh.SoPhut = parameter.CauHinhChamCongOt.SoPhut;

                    context.CauHinhChamCongOt.Update(cauHinh);
                    context.SaveChanges();

                    mess = "Lưu thành công";
                }

                return new CreateOrUpdateCauHinhChamCongOtResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = mess
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhChamCongOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetTkThoiGianOtResult GetTkThoiGianOt(GetTkThoiGianOtParameter parameter)
        {
            try
            {
                var listData = new List<List<DataRowModel>>();

                var listEmp = new List<Employee>();

                //Nếu hiển thị tất cả nhân viên 
                if (parameter.IsShowOption == 0)
                {
                    listEmp = context.Employee
                        .Where(x => !String.IsNullOrWhiteSpace(x.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.EmployeeId)))
                        .OrderBy(z => z.EmployeeName)
                        .ToList();
                }
                //Nếu chỉ hiển thị nhân viên đang làm việc
                else if (parameter.IsShowOption == 1)
                {
                    listEmp = context.Employee
                        .Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user }) // selection
                        .Where(x => !String.IsNullOrWhiteSpace(x.Emp.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.Emp.EmployeeId)) &&
                                     x.Emp.Active == true)
                        .Select(y => new Employee
                        {
                            EmployeeId = y.Emp.EmployeeId,
                            CodeMayChamCong = y.Emp.CodeMayChamCong,
                            EmployeeCode = y.Emp.EmployeeCode,
                            EmployeeName = y.Emp.EmployeeName,
                        })
                        .OrderBy(z => z.EmployeeName)
                        .ToList();
                }
                //Nếu chỉ hiển thị nhân viên đã nghỉ viêc
                else if (parameter.IsShowOption == 2)
                {
                    listEmp = context.Employee
                        .Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user }) // selection
                        .Where(x => !String.IsNullOrWhiteSpace(x.Emp.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.Emp.EmployeeId)) &&
                                    x.Emp.Active == false && x.User.Active == false)
                        .Select(y => new Employee
                        {
                            EmployeeId = y.Emp.EmployeeId,
                            CodeMayChamCong = y.Emp.CodeMayChamCong,
                            EmployeeCode = y.Emp.EmployeeCode,
                            EmployeeName = y.Emp.EmployeeName,
                        })
                        .OrderBy(z => z.EmployeeName)
                        .ToList();
                }

                //Lấy list các loại OT
                var otCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT")?.CategoryTypeId;
                var listLoaiOt = context.Category
                    .Where(x => x.CategoryTypeId == otCategoryTypeId)
                    .Select(y => new CategoryEntityModel(y)).ToList();

                var listEmpId = listEmp.Select(y => y.EmployeeId).ToList();
                var listTongHopChamCongOt = context.TongHopChamCongOt
                    .Where(x => (parameter.TuNgay == null || x.NgayChamCong.Date >= parameter.TuNgay.Date) && 
                                (parameter.DenNgay == null || x.NgayChamCong.Date <= parameter.DenNgay.Date) && 
                                listEmpId.Contains(x.EmployeeId))
                    .GroupBy(g => new {g.EmployeeId, g.LoaiOtId})
                    .Select(y => new TongHopChamCongOtModel()
                    {
                        EmployeeId = y.Key.EmployeeId,
                        LoaiOtId = y.Key.LoaiOtId,
                        SoPhut = y.Sum(s => s.SoPhut),
                        SoGio = y.Sum(s => s.SoGio),
                    }).OrderBy(z => z.LoaiOtId).ToList();

                var listLoaiOtId = listLoaiOt.Select(y => y.CategoryId).Distinct().OrderBy(z => z).ToList();

                #region Data

                int stt = 0;
                listEmp.ForEach(item =>
                {
                    stt++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "60px";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "code";
                    dataRow2.ColumnValue = item.CodeMayChamCong;
                    dataRow2.Width = "60px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "name";
                    dataRow3.ColumnValue = item.EmployeeCode + " - " + item.EmployeeName;
                    dataRow3.Width = "250px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    int index = 0;
                    listLoaiOtId.ForEach(loaiOtId =>
                    {
                        index++;
                        var data = listTongHopChamCongOt.FirstOrDefault(x => x.EmployeeId == item.EmployeeId &&
                                                                             x.LoaiOtId == loaiOtId);
                        decimal soGio = 0;
                        if (data != null) soGio = data.SoGio;

                        var _dataRow = new DataRowModel();
                        _dataRow.ColumnKey = "index_" + index;
                        _dataRow.ColumnValue = soGio.ToString();
                        _dataRow.ValueType = ValueTypeEnum.NUMBER;
                        _dataRow.Width = "150px";
                        _dataRow.TextAlign = "right";
                        listDataRow.Add(_dataRow);
                    });

                    listData.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeader = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "60px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "code",
                        ColumnValue = "Code",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "60px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "name",
                        ColumnValue = "Họ tên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "250px",
                    },
                };

                int _index = 0;
                listLoaiOtId.ForEach(loaiOtId =>
                {
                    string headerName = listLoaiOt
                        .FirstOrDefault(x => x.CategoryId == loaiOtId)?.CategoryName;

                    _index++;
                    var _dataHeader = new DataHeaderModel();
                    _dataHeader.ColumnKey = "index_" + _index;
                    _dataHeader.ColumnValue = "Tổng số giờ OT " + headerName;
                    _dataHeader.Rowspan = 0;
                    _dataHeader.Colspan = 0;
                    _dataHeader.Width = "150px";

                    listHeader1.Add(_dataHeader);
                });

                listDataHeader.Add(listHeader1);

                #endregion

                return new GetTkThoiGianOtResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListData = listData,
                    ListDataHeader = listDataHeader
                };
            }
            catch (Exception e)
            {
                return new GetTkThoiGianOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhOtCaNgayResult CreateOrUpdateCauHinhOtCaNgay(CreateOrUpdateCauHinhOtCaNgayParameter parameter)
        {
            try
            {
                string mess = "";
                var cauHinhOtCaNgay = parameter.CauHinhOtCaNgay;

                //Create
                if (cauHinhOtCaNgay.CauHinhOtCaNgayId == null)
                {
                    context.CauHinhOtCaNgay.Add(cauHinhOtCaNgay.ToEntity());
                    context.SaveChanges();

                    mess = "Tạo thành công";
                }
                //Update
                else
                {
                    var cauHinh = context.CauHinhOtCaNgay.FirstOrDefault(x =>
                        x.CauHinhOtCaNgayId == cauHinhOtCaNgay.CauHinhOtCaNgayId);
                    if (cauHinh == null)
                    {
                        return new CreateOrUpdateCauHinhOtCaNgayResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Cấu hình không tồn tại trên hệ thống"
                        };
                    }

                    cauHinh.GioVaoSang = cauHinhOtCaNgay.GioVaoSang;
                    cauHinh.GioRaSang = cauHinhOtCaNgay.GioRaSang;
                    cauHinh.GioKetThucSang = cauHinhOtCaNgay.GioKetThucSang;
                    cauHinh.GioVaoChieu = cauHinhOtCaNgay.GioVaoChieu;
                    cauHinh.GioRaChieu = cauHinhOtCaNgay.GioRaChieu;
                    cauHinh.GioKetThucChieu = cauHinhOtCaNgay.GioKetThucChieu;

                    context.CauHinhOtCaNgay.Update(cauHinh);
                    context.SaveChanges();

                    mess = "Lưu thành công";
                }

                return new CreateOrUpdateCauHinhOtCaNgayResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = mess
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhOtCaNgayResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhThueTncnResult CreateOrUpdateCauHinhThueTncn(CreateOrUpdateCauHinhThueTncnParameter parameter)
        {
            try
            {
                string mess = "";
                var cauHinhThueTncn = parameter.CauHinhThueTncn;

                //Create
                if (cauHinhThueTncn.CauHinhThueTncnId == null)
                {
                    context.CauHinhThueTncn.Add(cauHinhThueTncn.ToEntity());
                    context.SaveChanges();

                    mess = "Tạo thành công";
                }
                //Update
                else
                {
                    var cauHinh = context.CauHinhThueTncn.FirstOrDefault(x =>
                        x.CauHinhThueTncnId == cauHinhThueTncn.CauHinhThueTncnId);
                    if (cauHinh == null)
                    {
                        return new CreateOrUpdateCauHinhThueTncnResult()
                        {
                            StatusCode = HttpStatusCode.OK,
                            MessageCode = "Cấu hình không tồn tại trên hệ thống"
                        };
                    }

                    cauHinh.SoTienTu = cauHinhThueTncn.SoTienTu;
                    cauHinh.SoTienDen = cauHinhThueTncn.SoTienDen;
                    cauHinh.PhanTramThue = cauHinhThueTncn.PhanTramThue;
                    cauHinh.SoBiTruTheoCongThuc = cauHinhThueTncn.SoBiTruTheoCongThuc;

                    context.CauHinhThueTncn.Update(cauHinh);
                    context.SaveChanges();

                    mess = "Lưu thành công";
                }

                return new CreateOrUpdateCauHinhThueTncnResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = mess
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhThueTncnResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhThueTncnResult DeleteCauHinhThueTncn(DeleteCauHinhThueTncnParameter parameter)
        {
            try
            {
                var cauHinh =
                    context.CauHinhThueTncn.FirstOrDefault(x => x.CauHinhThueTncnId == parameter.CauHinhThueTncnId);
                if (cauHinh == null)
                {
                    return new DeleteCauHinhThueTncnResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình không tồn tại trên hệ thống"
                    };
                }

                context.CauHinhThueTncn.Remove(cauHinh);
                context.SaveChanges();

                return new DeleteCauHinhThueTncnResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa thành công"
                };
            }
            catch (Exception e)
            {
                return new DeleteCauHinhThueTncnResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetChamCongOtByEmpIdResult GetChamCongOtByEmpId(GetChamCongOtByEmpIdParameter parameter)
        {
            try
            {
                if (parameter.TuNgay.Date > parameter.DenNgay.Date)
                {
                    return new GetChamCongOtByEmpIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Bộ lọc Từ ngày phải trước Đến ngày"
                    };
                }

                var listData = new List<List<DataRowModel>>();
                var listDataHeader = new List<List<DataHeaderModel>>();

                //Lấy list các loại OT
                var otCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT")?.CategoryTypeId;
                var listLoaiOt = context.Category
                    .Where(x => x.CategoryTypeId == otCategoryTypeId)
                    .Select(y => new CategoryEntityModel(y)).ToList();

                var listChamCongOt = context.ChamCongOt.Where(x =>
                        (parameter.TuNgay == null || x.NgayChamCong.Date >= parameter.TuNgay.Date) &&
                        (parameter.DenNgay == null || x.NgayChamCong.Date <= parameter.DenNgay.Date) &&
                        x.EmployeeId == parameter.EmployeeId)
                    .OrderByDescending(z => z.NgayChamCong).ToList();

                var listLoaiOtId = listLoaiOt.Select(y => y.CategoryId).Distinct().OrderBy(z => z).ToList();

                var listDate = new List<DateTime>();
                var startDate = parameter.TuNgay;
                var endDate = parameter.DenNgay;
                while (startDate <= endDate)
                {
                    listDate.Add(startDate);
                    startDate = startDate.AddDays(1).Date;
                }

                listDate = listDate.OrderByDescending(z => z).ToList();

                #region Data

                listDate.ForEach(item =>
                {
                    var listDataRow = new List<DataRowModel>();

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "ngay";
                    dataRow.ColumnValue = item.ToString("dd/MM/yyyy");
                    dataRow.Width = "90px";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    int index = 0;
                    listLoaiOtId.ForEach(loaiOtId =>
                    {
                        var dataChamCongOt = listChamCongOt.FirstOrDefault(x => x.NgayChamCong.Date == item &&
                                                                                x.LoaiOtId == loaiOtId);

                        index++;
                        var dataRowVaoSang = new DataRowModel();
                        dataRowVaoSang.ColumnKey = "index_vs_" + loaiOtId + "_" + index;
                        dataRowVaoSang.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioVaoSang);
                        dataRowVaoSang.Width = "50px";
                        dataRowVaoSang.TextAlign = "center";
                        listDataRow.Add(dataRowVaoSang);

                        index++;
                        var dataRowRaSang = new DataRowModel();
                        dataRowRaSang.ColumnKey = "index_rs_" + loaiOtId + "_" + index;
                        dataRowRaSang.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioRaSang);
                        dataRowRaSang.Width = "50px";
                        dataRowRaSang.TextAlign = "center";
                        listDataRow.Add(dataRowRaSang);

                        index++;
                        var dataRowVaoChieu = new DataRowModel();
                        dataRowVaoChieu.ColumnKey = "index_vc_" + loaiOtId + "_" + index;
                        dataRowVaoChieu.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioVaoChieu);
                        dataRowVaoChieu.Width = "50px";
                        dataRowVaoChieu.TextAlign = "center";
                        listDataRow.Add(dataRowVaoChieu);

                        index++;
                        var dataRowRaChieu = new DataRowModel();
                        dataRowRaChieu.ColumnKey = "index_rc_" + loaiOtId + "_" + index;
                        dataRowRaChieu.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioRaChieu);
                        dataRowRaChieu.Width = "50px";
                        dataRowRaChieu.TextAlign = "center";
                        listDataRow.Add(dataRowRaChieu);

                        index++;
                        var dataRowVaoToi = new DataRowModel();
                        dataRowVaoToi.ColumnKey = "index_vt_" + loaiOtId + "_" + index;
                        dataRowVaoToi.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioVaoToi);
                        dataRowVaoToi.Width = "50px";
                        dataRowVaoToi.TextAlign = "center";
                        listDataRow.Add(dataRowVaoToi);

                        index++;
                        var dataRowRaToi = new DataRowModel();
                        dataRowRaToi.ColumnKey = "index_rt_" + loaiOtId + "_" + index;
                        dataRowRaToi.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioRaToi);
                        dataRowRaToi.Width = "50px";
                        dataRowRaToi.TextAlign = "center";
                        listDataRow.Add(dataRowRaToi);
                    });

                    listData.Add(listDataRow);
                });

                #endregion

                #region Header

                /* tr1 */
                var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "ngay",
                        ColumnValue = "Ngày",
                        Rowspan = 2,
                        Colspan = 0,
                        Width = "90px",
                    },
                };

                int _index = 0;
                listLoaiOtId.ForEach(item =>
                {
                    _index++;
                    var _dataHeader = new DataHeaderModel();
                    _dataHeader.ColumnKey = "index_loaiOt_" + _index;
                    _dataHeader.ColumnValue = "Loại OT " + listLoaiOt.FirstOrDefault(x => x.CategoryId == item)?.CategoryName;
                    _dataHeader.Rowspan = 0;
                    _dataHeader.Colspan = 6;
                    _dataHeader.Width = "300px";

                    listHeader1.Add(_dataHeader);
                });

                listDataHeader.Add(listHeader1);

                /* tr2 */
                var listHeader2 = new List<DataHeaderModel>();
                int _indexCa = 0;
                listLoaiOtId.ForEach(item =>
                {
                    //Vào sáng
                    _indexCa++;
                    var _dataHeader1 = new DataHeaderModel();
                    _dataHeader1.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader1.ColumnValue = "Vào sáng";
                    _dataHeader1.Rowspan = 0;
                    _dataHeader1.Colspan = 0;
                    _dataHeader1.Width = "50px";

                    listHeader2.Add(_dataHeader1);

                    //Ra sáng
                    _indexCa++;
                    var _dataHeader2 = new DataHeaderModel();
                    _dataHeader2.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader2.ColumnValue = "Ra sáng";
                    _dataHeader2.Rowspan = 0;
                    _dataHeader2.Colspan = 0;
                    _dataHeader2.Width = "50px";

                    listHeader2.Add(_dataHeader2);

                    //Vào chiều
                    _indexCa++;
                    var _dataHeader3 = new DataHeaderModel();
                    _dataHeader3.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader3.ColumnValue = "Vào chiều";
                    _dataHeader3.Rowspan = 0;
                    _dataHeader3.Colspan = 0;
                    _dataHeader3.Width = "50px";

                    listHeader2.Add(_dataHeader3);

                    //Ra chiều
                    _indexCa++;
                    var _dataHeader4 = new DataHeaderModel();
                    _dataHeader4.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader4.ColumnValue = "Ra chiều";
                    _dataHeader4.Rowspan = 0;
                    _dataHeader4.Colspan = 0;
                    _dataHeader4.Width = "50px";

                    listHeader2.Add(_dataHeader4);

                    //Vào tối
                    _indexCa++;
                    var _dataHeader5 = new DataHeaderModel();
                    _dataHeader5.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader5.ColumnValue = "Vào tối";
                    _dataHeader5.Rowspan = 0;
                    _dataHeader5.Colspan = 0;
                    _dataHeader5.Width = "50px";

                    listHeader2.Add(_dataHeader5);

                    //Ra tối
                    _indexCa++;
                    var _dataHeader6 = new DataHeaderModel();
                    _dataHeader6.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader6.ColumnValue = "Ra tối";
                    _dataHeader6.Rowspan = 0;
                    _dataHeader6.Colspan = 0;
                    _dataHeader6.Width = "50px";

                    listHeader2.Add(_dataHeader6);
                });

                listDataHeader.Add(listHeader2);

                #endregion

                return new GetChamCongOtByEmpIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListData = listData,
                    ListDataHeader = listDataHeader,
                    ListLoaiOt = listLoaiOt
                };
            }
            catch (Exception e)
            {
                return new GetChamCongOtByEmpIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateChamCongOtResult CreateOrUpdateChamCongOt(CreateOrUpdateChamCongOtParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var chamCongOt = context.ChamCongOt.FirstOrDefault(x => x.EmployeeId == parameter.EmployeeId &&
                                                                        x.NgayChamCong.Date ==
                                                                        parameter.NgayChamCong.Date &&
                                                                        x.LoaiOtId == parameter.LoaiOtId);

                    var otCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT")?.CategoryTypeId;
                    var listLoaiOt = context.Category
                        .Where(x => x.CategoryTypeId == otCategoryTypeId)
                        .Select(y => new CategoryEntityModel(y)).ToList();

                    var oldThoiGian = "--";
                    var newThoiGian = parameter.ThoiGian == null ? "--" : parameter.ThoiGian.Value.ToString(@"hh\:mm");
                    var loaiOt = listLoaiOt.FirstOrDefault(x => x.CategoryId == parameter.LoaiOtId)?.CategoryName;
                    var ngay = parameter.NgayChamCong.ToString("dd/MM/yyyy");

                    //Create
                    if (chamCongOt == null)
                    {
                        var _chamCongOt = new ChamCongOt();
                        _chamCongOt.EmployeeId = parameter.EmployeeId;
                        _chamCongOt.NgayChamCong = parameter.NgayChamCong.Date;
                        _chamCongOt.LoaiOtId = parameter.LoaiOtId;

                        if (parameter.Type == "vs") _chamCongOt.GioVaoSang = parameter.ThoiGian;
                        else if (parameter.Type == "rs") _chamCongOt.GioRaSang = parameter.ThoiGian;
                        else if (parameter.Type == "vc") _chamCongOt.GioVaoChieu = parameter.ThoiGian;
                        else if (parameter.Type == "rc") _chamCongOt.GioRaChieu = parameter.ThoiGian;
                        else if (parameter.Type == "vt") _chamCongOt.GioVaoToi = parameter.ThoiGian;
                        else if (parameter.Type == "rt") _chamCongOt.GioRaToi = parameter.ThoiGian;

                        context.ChamCongOt.Add(_chamCongOt);
                        context.SaveChanges();
                    }
                    //Update
                    else
                    {
                        if (parameter.Type == "vs")
                        {
                            oldThoiGian = chamCongOt.GioVaoSang == null
                                ? "--"
                                : chamCongOt.GioVaoSang.Value.ToString(@"hh\:mm");

                            chamCongOt.GioVaoSang = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "rs")
                        {
                            oldThoiGian = chamCongOt.GioRaSang == null
                                ? "--"
                                : chamCongOt.GioRaSang.Value.ToString(@"hh\:mm");

                            chamCongOt.GioRaSang = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "vc")
                        {
                            oldThoiGian = chamCongOt.GioVaoChieu == null
                                ? "--"
                                : chamCongOt.GioVaoChieu.Value.ToString(@"hh\:mm");

                            chamCongOt.GioVaoChieu = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "rc")
                        {
                            oldThoiGian = chamCongOt.GioRaChieu == null
                                ? "--"
                                : chamCongOt.GioRaChieu.Value.ToString(@"hh\:mm");

                            chamCongOt.GioRaChieu = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "vt")
                        {
                            oldThoiGian = chamCongOt.GioVaoToi == null
                                ? "--"
                                : chamCongOt.GioVaoToi.Value.ToString(@"hh\:mm");

                            chamCongOt.GioVaoToi = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "rt")
                        {
                            oldThoiGian = chamCongOt.GioRaToi == null
                                ? "--"
                                : chamCongOt.GioRaToi.Value.ToString(@"hh\:mm");

                            chamCongOt.GioRaToi = parameter.ThoiGian;
                        }

                        context.ChamCongOt.Update(chamCongOt);
                        context.SaveChanges();
                    }

                    #region Tính lại giờ OT

                    var tongHopChamCongOt = context.TongHopChamCongOt.FirstOrDefault(x =>
                        x.EmployeeId == parameter.EmployeeId &&
                        x.LoaiOtId == parameter.LoaiOtId &&
                        x.NgayChamCong.Date == parameter.NgayChamCong.Date);

                    if (tongHopChamCongOt != null)
                    {
                        context.TongHopChamCongOt.Remove(tongHopChamCongOt);
                    }

                    var newChamCongOt = context.ChamCongOt
                        .Where(x => x.EmployeeId == parameter.EmployeeId &&
                                    x.NgayChamCong.Date == parameter.NgayChamCong.Date &&
                                    x.LoaiOtId == parameter.LoaiOtId).ToList();

                    var listTongHopChamCongOt = TinhTongHopChamCongOt(newChamCongOt);

                    context.TongHopChamCongOt.AddRange(listTongHopChamCongOt);
                    context.SaveChanges();

                    #endregion

                    #region Lưu lịch sử vào dòng thời gian

                    var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                    var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                    var _emp = context.Employee.FirstOrDefault(x => x.EmployeeId == parameter.EmployeeId);

                    var ca = "";
                    if (parameter.Type == "vs") ca = "vào sáng";
                    else if (parameter.Type == "rs") ca = "ra sáng";
                    else if (parameter.Type == "vc") ca = "vào chiều";
                    else if (parameter.Type == "rc") ca = "ra chiều";
                    else if (parameter.Type == "vt") ca = "vào tối";
                    else if (parameter.Type == "rt") ca = "ra tối";

                    var contentNote = "";
                    contentNote = "<p>- <strong>" + emp.EmployeeCode + " - " + emp.EmployeeName +
                                  "</strong> đã thay đổi giờ " + ca +
                                  " của <strong>" + _emp.EmployeeCode + " - " + _emp.EmployeeName +
                                  "</strong> từ <strong>\"" + oldThoiGian + "\"</strong> sang <strong>\"" + newThoiGian +
                                  "\"</strong> với loại OT: <strong> " + loaiOt + 
                                  "</strong> trong ngày <strong>" + ngay + "</strong></p>";

                    Note note = new Note
                    {
                        NoteId = Guid.NewGuid(),
                        Description = contentNote,
                        Type = "ADD",
                        ObjectId = Guid.Empty,
                        ObjectNumber = 1,
                        ObjectType = NoteObjectType.TKCCOT,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        NoteTitle = "đã thêm ghi chú"
                    };

                    context.Note.Add(note);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();

                    return new CreateOrUpdateChamCongOtResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateChamCongOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataTroCapResult GetMasterDataTroCap(GetMasterDataTroCapParameter parameter)
        {
            try
            {
                var listLoaiNgayNghi = GeneralList.GetTrangThais("TroCap_LoaiNgayNghi");
                var listHinhThucTru = GeneralList.GetTrangThais("TroCap_HinhThucTru");

                #region Master data

                var troCapCoDinhType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
                var listLoaiTroCapCoDinh = context.Category
                    .Where(x => x.CategoryTypeId == troCapCoDinhType.CategoryTypeId &&
                                x.CategoryCode != "TCCCNC" && x.CategoryCode != "TCCCDMVS")
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var listLoaiTroCapChuyenCanNgayCong = context.Category
                    .Where(x => x.CategoryTypeId == troCapCoDinhType.CategoryTypeId &&
                                x.CategoryCode == "TCCCNC")
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var listLoaiTroCapChuyenCanDmvs = context.Category
                    .Where(x => x.CategoryTypeId == troCapCoDinhType.CategoryTypeId &&
                                x.CategoryCode == "TCCCDMVS")
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var troCapKhacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCK");
                var listLoaiTroCapKhac = context.Category
                    .Where(x => x.CategoryTypeId == troCapKhacType.CategoryTypeId)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var listPosition = context.Position.Select(y => new PositionModel
                {
                    PositionId = y.PositionId,
                    PositionName = y.PositionName
                }).OrderBy(z => z.PositionName).ToList();

                var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
                var listLoaiHopDong = context.Category
                    .Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var dieuKienHuongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC");
                var listDieuKienHuong = context.Category
                    .Where(x => x.CategoryTypeId == dieuKienHuongType.CategoryTypeId)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Trợ cấp cố định

                var listTroCapCoDinh = context.TroCap.Where(x => x.TypeId == 1).Select(y => new TroCapModel(y)).ToList();
                var listTroCapCoDinhId = listTroCapCoDinh.Select(y => y.TroCapId).ToList();

                var listChucVuTccd = context.TroCapChucVuMapping
                    .Where(x => listTroCapCoDinhId.Contains(x.TroCapId))
                    .Select(y => new TroCapChucVuMappingModel(y)).ToList();
                var listLoaiHopDongTccd = context.TroCapLoaiHopDongMapping
                    .Where(x => listTroCapCoDinhId.Contains(x.TroCapId))
                    .Select(y => new TroCapLoaiHopDongMappingModel(y)).ToList();
                var listDieuKienHuongTccd = context.TroCapDieuKienHuongMapping
                    .Where(x => listTroCapCoDinhId.Contains(x.TroCapId))
                    .Select(y => new TroCapDieuKienHuongMappingModel(y)).ToList();

                listTroCapCoDinh.ForEach(item =>
                {
                    var loaiTroCap = listLoaiTroCapCoDinh.FirstOrDefault(x => x.CategoryId == item.LoaiTroCapId);
                    item.LoaiTroCap = loaiTroCap?.CategoryName;

                    item.ListChucVu = listChucVuTccd.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listChucVuId = item.ListChucVu.Select(y => y.PositionId).ToList();
                    var listChucVu = listPosition.Where(x => listChucVuId.Contains(x.PositionId))
                        .OrderBy(z => z.PositionName).Select(y => y.PositionName).ToList();
                    listChucVu.ForEach(cv => { item.ChucVuText += "<p>+ " + cv + "</p>"; });

                    item.ListLoaiHopDong = listLoaiHopDongTccd.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listLoaiHopDongId = item.ListLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                    var _listLoaiHopDong = listLoaiHopDong
                        .Where(x => listLoaiHopDongId.Contains(x.CategoryId.Value))
                        .OrderBy(z => z.CategoryName).Select(y => y.CategoryName).ToList();
                    _listLoaiHopDong.ForEach(lhd => { item.LoaiHopDongText += "<p>+ " + lhd + "</p>"; });

                    item.ListDieuKienHuong = listDieuKienHuongTccd.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listDieuKienHuongId = item.ListDieuKienHuong.Select(y => y.DieuKienHuongId).ToList();
                    var _listDieuKienHuong = listDieuKienHuong.Where(x => listDieuKienHuongId.Contains(x.CategoryId.Value))
                        .OrderBy(z => z.CategoryName).Select(y => y.CategoryName).ToList();
                    _listDieuKienHuong.ForEach(dkh => { item.DieuKienHuongText += "<p>+ " + dkh + "</p>"; });
                });

                #endregion

                #region Trợ cấp theo chuyên cần ngày công

                var troCapTheoChuyenCanNgayCong = context.TroCap.Where(x => x.TypeId == 2)
                    .Select(y => new TroCapModel(y)).FirstOrDefault();

                if (troCapTheoChuyenCanNgayCong != null)
                {
                    var loaiTroCapChuyenCanNgayCong = listLoaiTroCapCoDinh
                        .FirstOrDefault(x => x.CategoryId == troCapTheoChuyenCanNgayCong.LoaiTroCapId);
                    troCapTheoChuyenCanNgayCong.LoaiTroCap = loaiTroCapChuyenCanNgayCong?.CategoryName;

                    troCapTheoChuyenCanNgayCong.ListChucVu = context.TroCapChucVuMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanNgayCong.TroCapId)
                        .Select(y => new TroCapChucVuMappingModel(y)).ToList();

                    troCapTheoChuyenCanNgayCong.ListLoaiHopDong = context.TroCapLoaiHopDongMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanNgayCong.TroCapId)
                        .Select(y => new TroCapLoaiHopDongMappingModel(y)).ToList();

                    troCapTheoChuyenCanNgayCong.ListDieuKienHuong = context.TroCapDieuKienHuongMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanNgayCong.TroCapId)
                        .Select(y => new TroCapDieuKienHuongMappingModel(y)).ToList();

                    troCapTheoChuyenCanNgayCong.ListMucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi
                        .Where(x => x.TroCapId == troCapTheoChuyenCanNgayCong.TroCapId)
                        .Select(y => new MucHuongTheoNgayNghiModel(y)).OrderByDescending(z => z.MucHuongPhanTram).ToList();
                }

                #endregion

                #region Trợ cấp theo chuyên cần đi muộn về sớm

                var troCapTheoChuyenCanDmvs = context.TroCap.Where(x => x.TypeId == 3)
                    .Select(y => new TroCapModel(y)).FirstOrDefault();

                if (troCapTheoChuyenCanDmvs != null)
                {
                    var loaiTroCapChuyenCanDmvs = listLoaiTroCapCoDinh
                        .FirstOrDefault(x => x.CategoryId == troCapTheoChuyenCanDmvs.LoaiTroCapId);
                    troCapTheoChuyenCanDmvs.LoaiTroCap = loaiTroCapChuyenCanDmvs?.CategoryName;

                    troCapTheoChuyenCanDmvs.ListChucVu = context.TroCapChucVuMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanDmvs.TroCapId)
                        .Select(y => new TroCapChucVuMappingModel(y)).ToList();

                    troCapTheoChuyenCanDmvs.ListLoaiHopDong = context.TroCapLoaiHopDongMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanDmvs.TroCapId)
                        .Select(y => new TroCapLoaiHopDongMappingModel(y)).ToList();

                    troCapTheoChuyenCanDmvs.ListDieuKienHuong = context.TroCapDieuKienHuongMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanDmvs.TroCapId)
                        .Select(y => new TroCapDieuKienHuongMappingModel(y)).ToList();

                    troCapTheoChuyenCanDmvs.ListMucHuongDmvs = context.MucHuongDmvs
                        .Where(x => x.TroCapId == troCapTheoChuyenCanDmvs.TroCapId)
                        .Select(y => new MucHuongDmvsModel(y)).ToList();
                }

                #endregion

                #region Trợ cấp khác

                var listTroCapKhac = context.TroCap.Where(x => x.TypeId == 4).Select(y => new TroCapModel(y)).ToList();
                var listTroCapKhacId = listTroCapKhac.Select(y => y.TroCapId).ToList();

                var listChucVuTck = context.TroCapChucVuMapping
                    .Where(x => listTroCapKhacId.Contains(x.TroCapId))
                    .Select(y => new TroCapChucVuMappingModel(y)).ToList();
                var listLoaiHopDongTck = context.TroCapLoaiHopDongMapping
                    .Where(x => listTroCapKhacId.Contains(x.TroCapId))
                    .Select(y => new TroCapLoaiHopDongMappingModel(y)).ToList();
                var listDieuKienHuongTck = context.TroCapDieuKienHuongMapping
                    .Where(x => listTroCapKhacId.Contains(x.TroCapId))
                    .Select(y => new TroCapDieuKienHuongMappingModel(y)).ToList();

                listTroCapKhac.ForEach(item =>
                {
                    var loaiTroCap = listLoaiTroCapKhac.FirstOrDefault(x => x.CategoryId == item.LoaiTroCapId);
                    item.LoaiTroCap = loaiTroCap?.CategoryName;

                    item.ListChucVu = listChucVuTck.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listChucVuId = item.ListChucVu.Select(y => y.PositionId).ToList();
                    var listChucVu = listPosition.Where(x => listChucVuId.Contains(x.PositionId))
                        .OrderBy(z => z.PositionName).Select(y => y.PositionName).ToList();
                    listChucVu.ForEach(cv => { item.ChucVuText += "<p>+ " + cv + "</p>"; });

                    item.ListLoaiHopDong = listLoaiHopDongTck.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listLoaiHopDongId = item.ListLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                    var _listLoaiHopDong = listLoaiHopDong
                        .Where(x => listLoaiHopDongId.Contains(x.CategoryId.Value))
                        .OrderBy(z => z.CategoryName).Select(y => y.CategoryName).ToList();
                    _listLoaiHopDong.ForEach(lhd => { item.LoaiHopDongText += "<p>+ " + lhd + "</p>"; });

                    item.ListDieuKienHuong = listDieuKienHuongTck.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listDieuKienHuongId = item.ListDieuKienHuong.Select(y => y.DieuKienHuongId).ToList();
                    var _listDieuKienHuong = listDieuKienHuong.Where(x => listDieuKienHuongId.Contains(x.CategoryId.Value))
                        .OrderBy(z => z.CategoryName).Select(y => y.CategoryName).ToList();
                    _listDieuKienHuong.ForEach(dkh => { item.DieuKienHuongText += "<p>+ " + dkh + "</p>"; });
                });

                #endregion

                return new GetMasterDataTroCapResult()
                {
                    ListTroCapCoDinh = listTroCapCoDinh,
                    TroCapTheoChuyenCanNgayCong = troCapTheoChuyenCanNgayCong ?? new TroCapModel(),
                    TroCapTheoChuyenCanDmvs = troCapTheoChuyenCanDmvs ?? new TroCapModel(),
                    ListTroCapKhac = listTroCapKhac,
                    ListLoaiTroCapCoDinh = listLoaiTroCapCoDinh,
                    ListLoaiTroCapChuyenCanNgayCong = listLoaiTroCapChuyenCanNgayCong,
                    ListLoaiTroCapChuyenCanDmvs = listLoaiTroCapChuyenCanDmvs,
                    ListLoaiTroCapKhac = listLoaiTroCapKhac,
                    ListPosition = listPosition,
                    ListLoaiHopDong = listLoaiHopDong,
                    ListDieuKienHuong = listDieuKienHuong,
                    ListLoaiNgayNghi = listLoaiNgayNghi,
                    ListHinhThucTru = listHinhThucTru,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataTroCapResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateTroCapResult CreateOrUpdateTroCap(CreateOrUpdateTroCapParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    int troCapId;

                    var troCapParameter = parameter.TroCap;

                    //Create
                    if (troCapParameter.TroCapId == null)
                    {
                        if (troCapParameter.TypeId == 1 || troCapParameter.TypeId == 4)
                        {
                            var exists =
                                context.TroCap.FirstOrDefault(x => x.LoaiTroCapId == troCapParameter.LoaiTroCapId &&
                                                                   x.TypeId == troCapParameter.TypeId);

                            if (exists != null)
                            {
                                return new CreateOrUpdateTroCapResult()
                                {
                                    StatusCode = HttpStatusCode.Conflict,
                                    MessageCode = "Loại Trợ cấp này đã tồn tại trên hệ thống, bạn không thể thêm"
                                };
                            }
                        }

                        var troCap = troCapParameter.ToEntity();
                        context.TroCap.Add(troCap);

                        context.SaveChanges();

                        troCapId = troCap.TroCapId;
                    }
                    //Update
                    else
                    {
                        var troCap = context.TroCap.FirstOrDefault(x => x.TroCapId == troCapParameter.TroCapId);
                        if (troCap == null)
                        {
                            return new CreateOrUpdateTroCapResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Trợ cấp không tồn tại trên hệ thống"
                            };
                        }

                        if (troCapParameter.TypeId == 1 || troCapParameter.TypeId == 4)
                        {
                            var exists =
                                context.TroCap.FirstOrDefault(x => x.LoaiTroCapId == troCapParameter.LoaiTroCapId &&
                                                                   x.TypeId == troCapParameter.TypeId &&
                                                                   x.TroCapId != troCap.TroCapId);

                            if (exists != null)
                            {
                                return new CreateOrUpdateTroCapResult()
                                {
                                    StatusCode = HttpStatusCode.Conflict,
                                    MessageCode = "Loại Trợ cấp này đã tồn tại trên hệ thống, bạn không thể thêm"
                                };
                            }
                        }

                        troCap.LoaiTroCapId = troCapParameter.LoaiTroCapId;
                        troCap.MucTroCap = troCapParameter.MucTroCap;

                        context.TroCap.Update(troCap);
                        context.SaveChanges();

                        troCapId = troCap.TroCapId;

                        #region Xóa các list mapping

                        var listChucVu = context.TroCapChucVuMapping.Where(x => x.TroCapId == troCapId).ToList();
                        var listLoaiHopDong = context.TroCapLoaiHopDongMapping.Where(x => x.TroCapId == troCapId).ToList();
                        var listDieuKienHuong = context.TroCapDieuKienHuongMapping.Where(x => x.TroCapId == troCapId).ToList();
                        var listMucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi.Where(x => x.TroCapId == troCapId).ToList();
                        var listMucHuongDmvs = context.MucHuongDmvs.Where(x => x.TroCapId == troCapId).ToList();

                        context.TroCapChucVuMapping.RemoveRange(listChucVu);
                        context.TroCapLoaiHopDongMapping.RemoveRange(listLoaiHopDong);
                        context.TroCapDieuKienHuongMapping.RemoveRange(listDieuKienHuong);
                        context.MucHuongTheoNgayNghi.RemoveRange(listMucHuongTheoNgayNghi);
                        context.MucHuongDmvs.RemoveRange(listMucHuongDmvs);
                        context.SaveChanges();

                        #endregion
                    }

                    #region Thêm các list mapping

                    //List chức vụ
                    troCapParameter.ListChucVu.ForEach(item =>
                    {
                        item.TroCapId = troCapId;
                        context.TroCapChucVuMapping.Add(item.ToEntity());
                        context.SaveChanges();
                    });

                    //List loại hợp đồng
                    troCapParameter.ListLoaiHopDong.ForEach(item =>
                    {
                        item.TroCapId = troCapId;
                        context.TroCapLoaiHopDongMapping.Add(item.ToEntity());
                        context.SaveChanges();
                    });

                    //List điều kiện hưởng
                    troCapParameter.ListDieuKienHuong.ForEach(item =>
                    {
                        item.TroCapId = troCapId;
                        context.TroCapDieuKienHuongMapping.Add(item.ToEntity());
                        context.SaveChanges();
                    });

                    //Trợ cấp theo chuyên cần ngày công 
                    if (troCapParameter.TypeId == 2)
                    {
                        troCapParameter.ListMucHuongTheoNgayNghi.ForEach(item =>
                        {
                            item.TroCapId = troCapId;
                            context.MucHuongTheoNgayNghi.Add(item.ToEntity());
                            context.SaveChanges();
                        });
                    }
                    //Trợ cấp theo chuyên cần đi muộn về sớm
                    else if (troCapParameter.TypeId == 3)
                    {
                        troCapParameter.ListMucHuongDmvs.ForEach(item =>
                        {
                            item.TroCapId = troCapId;
                            context.MucHuongDmvs.Add(item.ToEntity());
                            context.SaveChanges();
                        });
                    }

                    #endregion

                    trans.Commit();

                    return new CreateOrUpdateTroCapResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateTroCapResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteTroCapResult DeleteTroCap(DeleteTroCapParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    //Trợ cấp cố định, Trợ cấp khác
                    if (parameter.TypeId == 1 || parameter.TypeId == 4)
                    {
                        var troCap = context.TroCap.FirstOrDefault(x => x.TroCapId == parameter.ObjectId);
                        if (troCap == null)
                        {
                            return new DeleteTroCapResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Trợ cấp không tồn tại trên hệ thống"
                            };
                        }

                        var listChucVu = context.TroCapChucVuMapping.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listLoaiHopDong = context.TroCapLoaiHopDongMapping.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listDieuKienHuong = context.TroCapDieuKienHuongMapping.Where(x => x.TroCapId == troCap.TroCapId).ToList();

                        context.TroCapChucVuMapping.RemoveRange(listChucVu);
                        context.TroCapLoaiHopDongMapping.RemoveRange(listLoaiHopDong);
                        context.TroCapDieuKienHuongMapping.RemoveRange(listDieuKienHuong);
                        context.TroCap.Remove(troCap);

                        context.SaveChanges();
                    }
                    //Trợ cấp chuyên cần ngày công
                    else if (parameter.TypeId == 2)
                    {
                        var mucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi
                            .FirstOrDefault(x => x.MucHuongTheoNgayNghiId == parameter.ObjectId);
                        if (mucHuongTheoNgayNghi == null)
                        {
                            return new DeleteTroCapResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Trợ cấp không tồn tại trên hệ thống"
                            };
                        }

                        context.MucHuongTheoNgayNghi.Remove(mucHuongTheoNgayNghi);

                        context.SaveChanges();
                    }
                    //Trợ cấp chuyên cần dmvs
                    else if (parameter.TypeId == 3)
                    {
                        var mucHuongDmvs = context.MucHuongDmvs
                            .FirstOrDefault(x => x.MucHuongDmvsId == parameter.ObjectId);
                        if (mucHuongDmvs == null)
                        {
                            return new DeleteTroCapResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Trợ cấp không tồn tại trên hệ thống"
                            };
                        }

                        context.MucHuongDmvs.Remove(mucHuongDmvs);

                        context.SaveChanges();
                    }

                    trans.Commit();

                    return new DeleteTroCapResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteTroCapResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListKyLuongResult GetListKyLuong(GetListKyLuongParameter parameter)
        {
            try
            {
                var listAllUser = context.User.ToList();
                var user = listAllUser.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetListKyLuongResult
                    {
                        Message = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var listAllEmp = context.Employee.ToList();
                var employee = listAllEmp.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetListKyLuongResult
                    {
                        Message = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var listStatus = GeneralList.GetTrangThais("TrangThaiKyLuong");

                var listKyLuong = context.KyLuong.Where(x =>
                        (String.IsNullOrWhiteSpace(parameter.TenKyLuong) || x.TenKyLuong.ToLower().Trim()
                             .Contains(parameter.TenKyLuong.ToLower().Trim())) &&
                        parameter.ListTrangThai.Count == 0 || parameter.ListTrangThai.Contains(x.TrangThai))
                    .Select(y => new KyLuongModel(y)).ToList();

                #region Phân quyền dữ liệu theo quy trình phê duyệt

                var listThanhVienPhongBan =
                    context.ThanhVienPhongBan.Where(x => x.EmployeeId == employee.EmployeeId).ToList();

                var thongTinDeXuatTL = new List<DeXuatChucVuEntityModel>();
                var isAccess = context.Organization.FirstOrDefault(x => x.OrganizationId == employee.OrganizationId)?.IsAccess;
                var listEmpIdCungPhongBan = listAllEmp.Where(x => x.OrganizationId == employee.OrganizationId).Select(x => x.EmployeeId).ToList();
                var listUserIdCungPhongBan = listAllUser.Where(x => listEmpIdCungPhongBan.Contains(x.EmployeeId.Value)).Select(x => x.UserId).ToList();

                var listData = new List<KyLuongModel>();

                if (isAccess == false)
                {
                    var _listData = listKyLuong.Where(x => x.CreatedById == user.UserId).ToList(); // Theo người tạo
                    listData.AddRange(_listData);

                    listThanhVienPhongBan.ForEach(item =>
                    {
                        if (item.IsManager == 1)
                        {
                            //Lấy ra list đối tượng id mà người dùng phụ trách phê duyệt
                            var listId = context.PhongBanPheDuyetDoiTuong
                                .Where(x => x.DoiTuongApDung == 14 &&
                                            x.OrganizationId == item.OrganizationId).Select(y => y.ObjectNumber)
                                .ToList();

                            //Lọc ra các Id các bản ghi đã được lấy ở trên
                            var listIgnoreId = listData.Select(y => y.KyLuongId).ToList();

                            var listData_CanPheDuyet = listData.Where(x =>
                                    (listIgnoreId.Count == 0 || !listIgnoreId.Contains(x.KyLuongId)) &&
                                    (listId.Contains(x.KyLuongId) || // cần phê duyệt
                                    (listUserIdCungPhongBan.Contains(x.CreatedById.Value) && x.TrangThai != 1))) // cùng phòng ban tt khác 1
                                .ToList();

                            listData.AddRange(listData_CanPheDuyet);
                        }
                    });
                }
                else
                {
                    listData = listKyLuong.Where(x =>
                                        x.CreatedById == user.UserId || // Theo người tạo
                                        (x.CreatedById != user.UserId && x.TrangThai != 1) // Người khác tạo và trajgn thái khác mới: 1
                    ).ToList(); 
                }
                #endregion

                return new GetListKyLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListData = listData,
                    ListStatus = listStatus
                };
            }
            catch (Exception e)
            {
                return new GetListKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateKyLuongResult CreateOrUpdateKyLuong(CreateOrUpdateKyLuongParameter parameter)
        {
            try
            {
                #region Kiểm tra điều kiện trước khi tạo kỳ lương

                if (parameter.KyLuong.TuNgay.Date > parameter.KyLuong.DenNgay.Date)
                {
                    return new CreateOrUpdateKyLuongResult()
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        MessageCode = "Ngày bắt đầu phải nhỏ hơn Ngày kết thúc"
                    };
                }

                //Lấy dữ liệu chấm công trong khoảng Từ ngày => Đến ngày
                var countError = context.ChamCong.Count(x => x.NgayChamCong.Date >= parameter.KyLuong.TuNgay.Date &&
                                                             x.NgayChamCong.Date <= parameter.KyLuong.DenNgay.Date &&
                                                             x.KyHieuVaoSang == null && x.KyHieuRaSang == null &&
                                                             x.KyHieuVaoChieu == null && x.KyHieuRaChieu == null &&
                                                             ((x.VaoSang == null && x.RaSang != null) ||
                                                              (x.VaoSang != null && x.RaSang == null) ||
                                                              (x.VaoChieu == null && x.RaChieu != null) ||
                                                              (x.VaoChieu != null && x.RaChieu == null)));

                //var listTest = context.ChamCong.Join(context.User,
                //        cc => cc.EmployeeId,
                //        user => user.EmployeeId,
                //        (cc, user) => new {Cc = cc, User = user})
                //    .Where(x => x.User.Disabled != true &&
                //                x.Cc.NgayChamCong.Date >= parameter.KyLuong.TuNgay.Date &&
                //                x.Cc.NgayChamCong.Date <= parameter.KyLuong.DenNgay.Date &&
                //                x.Cc.KyHieuVaoSang == null && x.Cc.KyHieuRaSang == null &&
                //                x.Cc.KyHieuVaoChieu == null && x.Cc.KyHieuRaChieu == null &&
                //                ((x.Cc.VaoSang == null && x.Cc.RaSang != null) ||
                //                 (x.Cc.VaoSang != null && x.Cc.RaSang == null) ||
                //                 (x.Cc.VaoChieu == null && x.Cc.RaChieu != null) ||
                //                 (x.Cc.VaoChieu != null && x.Cc.RaChieu == null))).ToList();

                if (countError > 0)
                {
                    return new CreateOrUpdateKyLuongResult()
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        MessageCode = "Dữ liệu chấm công chưa đúng, bạn vui lòng kiểm tra lại"
                    };
                }

                #endregion

                int kyLuongId = 0;

                using (var trans = context.Database.BeginTransaction())
                {
                    //Create
                    if (parameter.KyLuong.KyLuongId == null)
                    {
                        var exists = context.KyLuong.OrderByDescending(z => z.TuNgay).FirstOrDefault();

                        if (exists != null && parameter.KyLuong.TuNgay.Date != exists.DenNgay.AddDays(1).Date)
                        {
                            return new CreateOrUpdateKyLuongResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Ngày bắt đầu kỳ lương phải liền kề với ngày kết thúc kỳ lương trước"
                            };
                        }

                        parameter.KyLuong.TuNgay = parameter.KyLuong.TuNgay.Date;
                        parameter.KyLuong.DenNgay = parameter.KyLuong.DenNgay.Date;
                        parameter.KyLuong.CreatedById = parameter.UserId;
                        parameter.KyLuong.TrangThai = 1;

                        var kyLuong = parameter.KyLuong.ToEntity();

                        context.KyLuong.Add(kyLuong);
                        context.SaveChanges();

                        kyLuongId = kyLuong.KyLuongId;
                    }
                    //Update
                    else
                    {
                        var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuong.KyLuongId);
                        if (kyLuong == null)
                        {
                            return new CreateOrUpdateKyLuongResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                            };
                        }

                        var exists = context.KyLuong.Where(x => x.KyLuongId != kyLuong.KyLuongId)
                            .OrderByDescending(z => z.TuNgay).FirstOrDefault();

                        if (exists != null && parameter.KyLuong.TuNgay.Date != exists.DenNgay.AddDays(1).Date)
                        {
                            return new CreateOrUpdateKyLuongResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Ngày bắt đầu kỳ lương phải liền kề với ngày kết thúc kỳ lương trước"
                            };
                        }

                        if (kyLuong.TrangThai != 1)
                        {
                            return new CreateOrUpdateKyLuongResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Không thể cập nhật do Kỳ lương không phải ở trạng thái Mới"
                            };
                        }

                        kyLuong.TenKyLuong = parameter.KyLuong.TenKyLuong.Trim();
                        kyLuong.SoNgayLamViec = parameter.KyLuong.SoNgayLamViec;
                        kyLuong.TuNgay = parameter.KyLuong.TuNgay.Date;
                        kyLuong.DenNgay = parameter.KyLuong.DenNgay.Date;

                        context.KyLuong.Update(kyLuong);
                        context.SaveChanges();

                        kyLuongId = kyLuong.KyLuongId;
                    }

                    SaveTongHopChamCong(kyLuongId, parameter.KyLuong.TuNgay.Date, parameter.KyLuong.DenNgay.Date);

                    //Create
                    if (parameter.KyLuong.KyLuongId == null)
                    {
                        var countTongHopChamCong = context.TongHopChamCong.Count(x => x.KyLuongId == kyLuongId);

                        //Nếu không có dữ liệu tổng hợp chấm công thì báo lỗi
                        if (countTongHopChamCong == 0)
                        {
                            return new CreateOrUpdateKyLuongResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Không có dữ liệu chấm công trong khoảng từ Ngày bắt đầu đến Ngày kết thúc"
                            };
                        }
                    }
                    
                    SaveLuongCt_TroCap(kyLuongId, parameter.KyLuong.SoNgayLamViec, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_TroCapOt(kyLuongId, parameter.KyLuong.SoNgayLamViec, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_ThuNhapTinhThue(kyLuongId, parameter.KyLuong.TuNgay.Date, parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_BaoHiem(kyLuongId, parameter.KyLuong.TuNgay.Date, parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_GiamTruTruocThue(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_GiamTruSauThue(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_HoanLaiSauThue(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_CtyDong(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_Other(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongTongHop(kyLuongId, parameter.KyLuong.SoNgayLamViec, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    trans.Commit();
                }

                return new CreateOrUpdateKyLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công",
                    KyLuongId = kyLuongId
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteKyLuongResult DeleteKyLuong(DeleteKyLuongParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new DeleteKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    if (kyLuong.TrangThai != 1)
                    {
                        return new DeleteKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Không thể xóa do Kỳ lương không phải ở trạng thái Mới"
                        };
                    }

                    var listTongHopChamCong =
                        context.TongHopChamCong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtTroCapCoDinh =
                        context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtTroCapKhac =
                        context.LuongCtTroCapKhac.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtDieuKienTroCapKhac = context.LuongCtDieuKienTroCapKhac
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtEmpDktcKhac =
                        context.LuongCtEmpDktcKhac.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtTroCapOt =
                        context.LuongCtTroCapOt.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapOt =
                        context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtBaoHiem =
                        context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtGiamTruSauThue = context.LuongCtGiamTruSauThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtHoanLaiSauThue = context.LuongCtHoanLaiSauThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtCtyDong =
                        context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtOther = context.LuongCtOther.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                    context.KyLuong.Remove(kyLuong);
                    context.TongHopChamCong.RemoveRange(listTongHopChamCong);
                    context.LuongCtTroCapCoDinh.RemoveRange(listLuongCtTroCapCoDinh);
                    context.LuongCtLoaiTroCapCoDinh.RemoveRange(listLuongCtLoaiTroCapCoDinh);
                    context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listLuongCtDieuKienTroCapCoDinh);
                    context.LuongCtTroCapKhac.RemoveRange(listLuongCtTroCapKhac);
                    context.LuongCtLoaiTroCapKhac.RemoveRange(listLuongCtLoaiTroCapKhac);
                    context.LuongCtDieuKienTroCapKhac.RemoveRange(listLuongCtDieuKienTroCapKhac);
                    context.LuongCtEmpDktcKhac.RemoveRange(listLuongCtEmpDktcKhac);
                    context.LuongCtTroCapOt.RemoveRange(listLuongCtTroCapOt);
                    context.LuongCtLoaiTroCapOt.RemoveRange(listLuongCtLoaiTroCapOt);
                    context.LuongCtThuNhapTinhThue.RemoveRange(listLuongCtThuNhapTinhThue);
                    context.LuongCtBaoHiem.RemoveRange(listLuongCtBaoHiem);
                    context.LuongCtGiamTruTruocThue.RemoveRange(listLuongCtGiamTruTruocThue);
                    context.LuongCtGiamTruSauThue.RemoveRange(listLuongCtGiamTruSauThue);
                    context.LuongCtHoanLaiSauThue.RemoveRange(listLuongCtHoanLaiSauThue);
                    context.LuongCtCtyDong.RemoveRange(listLuongCtCtyDong);
                    context.LuongCtOther.RemoveRange(listLuongCtOther);
                    context.LuongTongHop.RemoveRange(listLuongTongHop);
                    context.SaveChanges();

                    trans.Commit();

                    return new DeleteKyLuongResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DatVeMoiKyLuongResult DatVeMoiKyLuong(DatVeMoiKyLuongParameter parameter)
        {
            try
            {
                var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                if (kyLuong == null)
                {
                    return new DatVeMoiKyLuongResult()
                    {
                        MessageCode = "Kỳ lương không tồn tại trên hệ thống",
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                if (kyLuong.TrangThai != 4)
                {
                    return new DatVeMoiKyLuongResult()
                    {
                        MessageCode = "Chỉ được đặt về mới đề xuất ở trạng thái Từ chối",
                        StatusCode = HttpStatusCode.Conflict
                    };
                }

                kyLuong.TrangThai = 1;
                context.KyLuong.Update(kyLuong);

                //Thêm ghi chú
                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectId = Guid.Empty;
                note.Description = "Đã đặt đề xuất về mới";
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = "Đã thêm ghi chú";
                note.ObjectNumber = kyLuong.KyLuongId;
                note.ObjectType = NoteObjectType.KYLUONG;

                context.Note.Add(note);

                context.SaveChanges();

                return new DatVeMoiKyLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Đặt về mới thành công"
                };
            }
            catch (Exception e)
            {
                return new DatVeMoiKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetKyLuongByIdResult GetKyLuongById(GetKyLuongByIdParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetKyLuongByIdResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Nguời dùng không tồn tại trên hệ thống"
                    };
                }

                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                if (emp == null)
                {
                    return new GetKyLuongByIdResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Nguời dùng không tồn tại trên hệ thống"
                    };
                }

                var kyLuong = context.KyLuong.Where(x => x.KyLuongId == parameter.KyLuongId)
                    .Select(y => new KyLuongModel(y)).FirstOrDefault();
                if (kyLuong == null)
                {
                    return new GetKyLuongByIdResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                    };
                }

                var userCreated = context.User.FirstOrDefault(x => x.UserId == kyLuong.CreatedById);
                var empCreated = new Employee();
                if (userCreated != null)
                {
                    empCreated = context.Employee.FirstOrDefault(x => x.EmployeeId == userCreated.EmployeeId);
                    kyLuong.CreatedName = empCreated?.EmployeeCode + " - " + empCreated?.EmployeeName;
                }

                var listEmp =
                    context.Employee.Select(y => new Employee
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeCode = y.EmployeeCode,
                        EmployeeName = y.EmployeeName,
                        SubCode1Value = y.SubCode1Value
                    }).ToList();

                var listPosition = context.Position.Select(y => new Position
                {
                    PositionId = y.PositionId,
                    PositionCode = y.PositionCode,
                    PositionName = y.PositionName
                }).ToList();

                var listGetSubCode1 = GeneralList.GetSubCode1();

                #region Tổng hợp chấm công

                var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new TongHopChamCongModel(y)).ToList();

                listTongHopChamCong.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                });

                listTongHopChamCong = listTongHopChamCong.OrderBy(z => z.EmployeeCode).ToList();

                #endregion

                #region Bảo hiểm

                var listLuongCtBaoHiem = context.LuongCtBaoHiem
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtBaoHiemModel(y)).ToList();

                listLuongCtBaoHiem.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Bảng lương tổng hợp

                var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongTongHopModel(y)).ToList();

                listLuongTongHop.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                    item.LuongCoBanDongBh = listLuongCtBaoHiem.FirstOrDefault(x => x.EmployeeId == item.EmployeeId).Bhxh;
                });

                #endregion

                #region Thu nhập chỉ đưa vào tính thuế

                var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtThuNhapTinhThueModel(y)).ToList();

                listLuongCtThuNhapTinhThue.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Giảm trừ trước thuế

                var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtGiamTruTruocThueModel(y)).ToList();

                listLuongCtGiamTruTruocThue.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Giảm trừ sau thuế

                var listLuongCtGiamTruSauThue = context.LuongCtGiamTruSauThue
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtGiamTruSauThueModel(y)).ToList();

                listLuongCtGiamTruSauThue.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Hoàn lại sau thuế

                var listLuongCtHoanLaiSauThue = context.LuongCtHoanLaiSauThue
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtHoanLaiSauThueModel(y)).ToList();

                listLuongCtHoanLaiSauThue.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Công ty phải đóng

                var listLuongCtCtyDong = context.LuongCtCtyDong
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtCtyDongModel(y)).ToList();

                listLuongCtCtyDong.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Other

                var listLuongCtOther = context.LuongCtOther
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtOtherModel(y)).ToList();

                listLuongCtOther.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

              

                #region Trợ cấp OT

                var listLuongCtTroCapOt = context.LuongCtTroCapOt
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtTroCapOtModel(y)).ToList();

                var listLuongCtLoaiTroCapOt =
                    context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                var typeLoaiOt = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT");
                var listLoaiOt = context.Category.Where(x => x.CategoryTypeId == typeLoaiOt.CategoryTypeId).ToList();

                #region Data

                var listDataTroCapOt = new List<List<DataRowModel>>();

                int stt = 0;
                listLuongCtTroCapOt.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;

                    var listLuongCtLoaiTroCapOt_ByEmp = listLuongCtLoaiTroCapOt
                        .Where(x => x.LuongCtTroCapOtId == item.LuongCtTroCapOtId).ToList();

                    stt++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "40!important";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "employeeCode";
                    dataRow2.ColumnValue = item.EmployeeCode;
                    dataRow2.Width = "150px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "subCode1";
                    dataRow3.ColumnValue = item.SubCode1;
                    dataRow3.Width = "90px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "employeeName";
                    dataRow4.ColumnValue = item.EmployeeName;
                    dataRow4.Width = "200px";
                    dataRow4.TextAlign = "left";
                    listDataRow.Add(dataRow4);

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "positionName";
                    dataRow5.ColumnValue = item.PositionName;
                    dataRow5.Width = "130px";
                    dataRow5.TextAlign = "left";
                    listDataRow.Add(dataRow5);

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "mucLuongHienTai";
                    dataRow6.ColumnValue = item.MucLuongHienTai.ToString();
                    dataRow6.Width = "200px";
                    dataRow6.TextAlign = "right";
                    dataRow6.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow6);

                    var tongLuongOt = TongLuongOt(listLuongCtLoaiTroCapOt_ByEmp);

                    var dataRow7 = new DataRowModel();
                    dataRow7.ColumnKey = "tongLuongOt";
                    dataRow7.ColumnValue = tongLuongOt.ToString();
                    dataRow7.Width = "200px";
                    dataRow7.TextAlign = "right";
                    dataRow7.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow7);

                    var luongOtTinhThue = LuongOtTinhThue(item.ToEntity(), listLuongCtLoaiTroCapOt_ByEmp);

                    var dataRow8 = new DataRowModel();
                    dataRow8.ColumnKey = "luongOtTinhThue";
                    dataRow8.ColumnValue = luongOtTinhThue.ToString();
                    dataRow8.Width = "200px";
                    dataRow8.TextAlign = "right";
                    dataRow8.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow8);

                    int index = 0;
                    listLoaiOt.ForEach(loaiOt =>
                    {
                        var existsOt =
                            listLuongCtLoaiTroCapOt_ByEmp.FirstOrDefault(x => x.LoaiOtId == loaiOt.CategoryId);

                        //Nếu nhân viên có dữ liệu của loại OT này
                        if (existsOt != null)
                        {
                            index++;
                            var dataRowLoaiOt = new DataRowModel();
                            dataRowLoaiOt.ColumnKey = "index_" + index;
                            dataRowLoaiOt.ColumnValue = existsOt.MucTroCap.ToString();
                            dataRowLoaiOt.Width = "200px";
                            dataRowLoaiOt.TextAlign = "right";
                            dataRowLoaiOt.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiOt);

                            index++;
                            var dataRowMucTroCap = new DataRowModel();
                            dataRowMucTroCap.ColumnKey = "index_" + index;
                            dataRowMucTroCap.ColumnValue = existsOt.SoGioOt.ToString();
                            dataRowMucTroCap.Width = "200px";
                            dataRowMucTroCap.TextAlign = "right";
                            dataRowMucTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowMucTroCap);
                        }
                        //Nếu nhân viên không có dữ liệu của loại OT này
                        else
                        {
                            index++;
                            var dataRowLoaiOt = new DataRowModel();
                            dataRowLoaiOt.ColumnKey = "index_" + index;
                            dataRowLoaiOt.ColumnValue = "0";
                            dataRowLoaiOt.Width = "200px";
                            dataRowLoaiOt.TextAlign = "right";
                            dataRowLoaiOt.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiOt);

                            index++;
                            var dataRowMucTroCap = new DataRowModel();
                            dataRowMucTroCap.ColumnKey = "index_" + index;
                            dataRowMucTroCap.ColumnValue = "0";
                            dataRowMucTroCap.Width = "200px";
                            dataRowMucTroCap.TextAlign = "right";
                            dataRowMucTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowMucTroCap);
                        }
                    });

                    listDataTroCapOt.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeaderTroCapOt = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "90px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeCode",
                        ColumnValue = "Mã nhân viên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "subCode1",
                        ColumnValue = "Mã phòng ban",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "100px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeName",
                        ColumnValue = "Họ tên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "positionName",
                        ColumnValue = "Chức vụ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "mucLuongHienTai",
                        ColumnValue = "Mức lương hiện tại (1 giờ)",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "tongLuongOt",
                        ColumnValue = "Tổng lương OT",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "luongOtTinhThue",
                        ColumnValue = "Tổng lương OT tính thuế",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                };

                int _index = 0;
                listLoaiOt.ForEach(loaiOt =>
                {
                    _index++;
                    var _dataHeader1 = new DataHeaderModel()
                    {
                        ColumnKey = "index_loai_" + _index,
                        ColumnValue = "Mức OT " + loaiOt.CategoryName,
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    };
                    listHeader1.Add(_dataHeader1);

                    _index++;
                    var _dataHeader2 = new DataHeaderModel()
                    {
                        ColumnKey = "index_gio_" + _index,
                        ColumnValue = "Số giờ " + loaiOt.CategoryName,
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    };
                    listHeader1.Add(_dataHeader2);
                });

                listDataHeaderTroCapOt.Add(listHeader1);

                #endregion

                #endregion

                #region Trợ cấp cố định

                var listLuongCtTroCapCoDinh = context.LuongCtTroCapCoDinh
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtTroCapCoDinhModel(y)).ToList();

                var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                var loaiTroCapCoDinhType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
                var listLoaiTroCapCoDinh = context.Category
                    .Where(x => x.CategoryTypeId == loaiTroCapCoDinhType.CategoryTypeId).ToList();

                var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
                var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();

                #region Data

                var listDataTroCapCoDinh = new List<List<DataRowModel>>();

                int stt_tccd = 0;
                listLuongCtTroCapCoDinh.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;

                    var listLuongCtLoaiTroCapCoDinh_ByEmp = listLuongCtLoaiTroCapCoDinh
                        .Where(x => x.LuongCtTroCapCoDinhId == item.LuongCtTroCapCoDinhId).ToList();

                    stt_tccd++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt_tccd.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "40px!important";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "employeeCode";
                    dataRow2.ColumnValue = item.EmployeeCode;
                    dataRow2.Width = "150px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "subCode1";
                    dataRow3.ColumnValue = item.SubCode1;
                    dataRow3.Width = "90px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "employeeName";
                    dataRow4.ColumnValue = item.EmployeeName;
                    dataRow4.Width = "200px";
                    dataRow4.TextAlign = "left";
                    listDataRow.Add(dataRow4);

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "positionName";
                    dataRow5.ColumnValue = item.PositionName;
                    dataRow5.Width = "130px";
                    dataRow5.TextAlign = "left";
                    listDataRow.Add(dataRow5);

                    var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == item.LoaiHopDongId);

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "loaiHopDong";
                    dataRow6.ColumnValue = loaiHopDong?.CategoryName;
                    dataRow6.Width = "300px";
                    dataRow6.TextAlign = "left";
                    listDataRow.Add(dataRow6);

                    int index = 0;
                    listLoaiTroCapCoDinh.ForEach(loaiTroCap =>
                    {
                        var exists =
                            listLuongCtLoaiTroCapCoDinh_ByEmp.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCap.CategoryId);

                        //Nếu nhân viên có dữ liệu của loại trợ cấp này
                        if (exists != null)
                        {
                            index++;
                            var dataRowLoaiTroCap = new DataRowModel();
                            dataRowLoaiTroCap.ColumnKey = "index_" + index;
                            dataRowLoaiTroCap.ColumnValue = exists.MucTroCap.ToString();
                            dataRowLoaiTroCap.Width = "200px";
                            dataRowLoaiTroCap.TextAlign = "right";
                            dataRowLoaiTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiTroCap);

                            var dataRowLoaiTroCapId = new DataRowModel();
                            dataRowLoaiTroCapId.ColumnKey = "loaiTroCapCoDinhId_index_" + index;
                            dataRowLoaiTroCapId.ColumnValue = exists.LuongCtLoaiTroCapCoDinhId.ToString();
                            dataRowLoaiTroCapId.Width = "200px";
                            dataRowLoaiTroCapId.TextAlign = "right";
                            dataRowLoaiTroCapId.ValueType = ValueTypeEnum.NUMBER;
                            dataRowLoaiTroCapId.IsShow = false;
                            listDataRow.Add(dataRowLoaiTroCapId);
                        }
                        //Nếu nhân viên không có dữ liệu của loại trợ cấp này
                        else
                        {
                            index++;
                            var dataRowLoaiTroCap = new DataRowModel();
                            dataRowLoaiTroCap.ColumnKey = "index_" + index;
                            dataRowLoaiTroCap.ColumnValue = "0";
                            dataRowLoaiTroCap.Width = "200px";
                            dataRowLoaiTroCap.TextAlign = "right";
                            dataRowLoaiTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiTroCap);

                            var dataRowLoaiTroCapId = new DataRowModel();
                            dataRowLoaiTroCapId.ColumnKey = "loaiTroCapCoDinhId_index_" + index;
                            dataRowLoaiTroCapId.ColumnValue = null;
                            dataRowLoaiTroCapId.Width = "200px";
                            dataRowLoaiTroCapId.TextAlign = "right";
                            dataRowLoaiTroCapId.ValueType = ValueTypeEnum.NUMBER;
                            dataRowLoaiTroCapId.IsShow = false;
                            listDataRow.Add(dataRowLoaiTroCapId);
                        }
                    });

                    listDataTroCapCoDinh.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeaderTroCapCoDinh = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1_tccd = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "90px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeCode",
                        ColumnValue = "Mã nhân viên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "subCode1",
                        ColumnValue = "Mã phòng ban",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "100px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeName",
                        ColumnValue = "Họ tên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "positionName",
                        ColumnValue = "Chức vụ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "loaiHopDong",
                        ColumnValue = "Loại HĐ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                };

                int _index_tccd = 0;
                listLoaiTroCapCoDinh.ForEach(loaiTroCap =>
                {
                    _index_tccd++;
                    var _dataHeader1 = new DataHeaderModel()
                    {
                        ColumnKey = "index_loai_" + _index_tccd,
                        ColumnValue = loaiTroCap.CategoryName,
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    };
                    listHeader1_tccd.Add(_dataHeader1);
                });

                listDataHeaderTroCapCoDinh.Add(listHeader1_tccd);

                #endregion

                #endregion

                #region Trợ cấp khác

                var loaiTroCapKhacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCK");
                var listLoaiTroCapKhac = context.Category
                    .Where(x => x.CategoryTypeId == loaiTroCapKhacType.CategoryTypeId)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var listLuongCtTroCapKhac = context.LuongCtTroCapKhac
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtTroCapKhacModel(y)).ToList();
                var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .ToList();
                var listSelectedLoaiTroCapKhacId =
                    listLuongCtLoaiTroCapKhac.Select(y => y.LoaiTroCapId).Distinct().ToList();
                var listSelectedLoaiTroCapKhac = listLoaiTroCapKhac
                    .Where(x => listSelectedLoaiTroCapKhacId.Contains(x.CategoryId.Value)).ToList();

                #region Data

                var listDataTroCapKhac = new List<List<DataRowModel>>();

                int stt_tck = 0;
                listLuongCtTroCapKhac.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;

                    var listLuongCtLoaiTroCapKhac_ByEmp = listLuongCtLoaiTroCapKhac
                        .Where(x => x.LuongCtTroCapKhacId == item.LuongCtTroCapKhacId).ToList();

                    stt_tck++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt_tck.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "40px";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "employeeCode";
                    dataRow2.ColumnValue = item.EmployeeCode;
                    dataRow2.Width = "100px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "subCode1";
                    dataRow3.ColumnValue = item.SubCode1;
                    dataRow3.Width = "100px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "employeeName";
                    dataRow4.ColumnValue = item.EmployeeName;
                    dataRow4.Width = "200px";
                    dataRow4.TextAlign = "left";
                    listDataRow.Add(dataRow4);

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "positionName";
                    dataRow5.ColumnValue = item.PositionName;
                    dataRow5.Width = "130px";
                    dataRow5.TextAlign = "left";
                    listDataRow.Add(dataRow5);

                    var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == item.LoaiHopDongId);

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "loaiHopDong";
                    dataRow6.ColumnValue = loaiHopDong?.CategoryName;
                    dataRow6.Width = "300px";
                    dataRow6.TextAlign = "left";
                    listDataRow.Add(dataRow6);

                    int index = 0;
                    listSelectedLoaiTroCapKhac.ForEach(loaiTroCap =>
                    {
                        var exists =
                            listLuongCtLoaiTroCapKhac_ByEmp.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCap.CategoryId);

                        //Nếu nhân viên có dữ liệu của loại trợ cấp này
                        if (exists != null)
                        {
                            index++;
                            var dataRowLoaiTroCap = new DataRowModel();
                            dataRowLoaiTroCap.ColumnKey = "index_" + index;
                            dataRowLoaiTroCap.ColumnValue = exists.MucTroCap.ToString();
                            dataRowLoaiTroCap.Width = "200px";
                            dataRowLoaiTroCap.TextAlign = "right";
                            dataRowLoaiTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiTroCap);

                            var dataRowLoaiTroCapId = new DataRowModel();
                            dataRowLoaiTroCapId.ColumnKey = "loaiTroCapKhacId_index_" + index;
                            dataRowLoaiTroCapId.ColumnValue = exists.LuongCtLoaiTroCapKhacId.ToString();
                            dataRowLoaiTroCapId.Width = "200px";
                            dataRowLoaiTroCapId.TextAlign = "right";
                            dataRowLoaiTroCapId.ValueType = ValueTypeEnum.NUMBER;
                            dataRowLoaiTroCapId.IsShow = false;
                            listDataRow.Add(dataRowLoaiTroCapId);

                            var dataRowIsEdit = new DataRowModel();
                            dataRowIsEdit.ColumnKey = "isEdit_index_" + index;
                            dataRowIsEdit.ColumnValue = exists.IsEdit.ToString();
                            dataRowIsEdit.Width = "200px";
                            dataRowIsEdit.TextAlign = "right";
                            dataRowIsEdit.ValueType = ValueTypeEnum.BOOL;
                            dataRowIsEdit.IsShow = false;
                            listDataRow.Add(dataRowIsEdit);
                        }
                    });

                    listDataTroCapKhac.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeaderTroCapKhac = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1_tck = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "40px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeCode",
                        ColumnValue = "Mã nhân viên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "100px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "subCode1",
                        ColumnValue = "Mã phòng ban",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "100px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeName",
                        ColumnValue = "Họ tên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "positionName",
                        ColumnValue = "Chức vụ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "130px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "loaiHopDong",
                        ColumnValue = "Loại HĐ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "300px",
                    },
                };

                int _index_tck = 0;
                listSelectedLoaiTroCapKhac.ForEach(loaiTroCap =>
                {
                    _index_tck++;
                    var _dataHeader1 = new DataHeaderModel()
                    {
                        ColumnKey = "index_loai_" + _index_tck,
                        ColumnValue = loaiTroCap.CategoryName,
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    };
                    listHeader1_tck.Add(_dataHeader1);
                });

                listDataHeaderTroCapKhac.Add(listHeader1_tck);

                #endregion

                #endregion

                #region Điều kiện hiển thị các button

                bool isShowGuiPheDuyet = false;
                bool isShowPheDuyet = false;
                bool isShowTuChoi = false;
                bool isShowDatVeMoi = false;
                bool isShowXoa = false;
                bool isShowSua = false;

                //Trạng thái Mới và User đăng nhập là người tạo đề xuất
                if (kyLuong.TrangThai == 1 && kyLuong.CreatedById == user.UserId)
                {
                    isShowGuiPheDuyet = true;
                    isShowXoa = true;
                    isShowSua = true;
                }

                //Trạng thái Chờ phê duyệt
                if (kyLuong.TrangThai == 2)
                {
                    var buocHienTai = context.CacBuocApDung.Where(x => x.ObjectNumber == kyLuong.KyLuongId &&
                                                                       x.DoiTuongApDung == 14 &&
                                                                       x.TrangThai == 0)
                        .OrderByDescending(z => z.Stt)
                        .FirstOrDefault();

                    //Nếu là phê duyệt trưởng bộ phận
                    if (buocHienTai?.LoaiPheDuyet == 1)
                    {
                        //Lấy list phòng ban của người tạo đề xuất
                        var listPhongBanId_NguoiPhuTrach = context.ThanhVienPhongBan
                            .Where(x => x.EmployeeId == empCreated.EmployeeId)
                            .Select(y => y.OrganizationId).ToList();

                        //Lấy số phòng ban mà User đăng nhập là trưởng bộ phận trong số phòng ban của người tạo đề xuất
                        var countPheDuyet = context.ThanhVienPhongBan.Count(x => x.EmployeeId == user.EmployeeId &&
                                                                                 x.IsManager == 1 &&
                                                                                 listPhongBanId_NguoiPhuTrach.Contains(
                                                                                     x.OrganizationId));

                        //Nếu User đăng nhập là trưởng bộ phận của 1 trong số các phòng ban của người tạo đề xuất
                        if (countPheDuyet > 0)
                        {
                            isShowPheDuyet = true;
                            isShowTuChoi = true;
                        }
                    }
                    //Nếu là phòng ban phê duyệt
                    else if (buocHienTai?.LoaiPheDuyet == 2)
                    {
                        //Lấy list Phòng ban đã phê duyệt ở bước hiện tại
                        var listPhongBanIdDaPheDuyet = context.PhongBanApDung
                            .Where(x => x.CacBuocApDungId == buocHienTai.Id &&
                                        x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId)
                            .Select(y => y.OrganizationId).ToList();

                        //Lấy list Phòng ban chưa phê duyệt ở bước hiện tại
                        var listPhongBanId = context.PhongBanTrongCacBuocQuyTrinh
                            .Where(x => x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId &&
                                        !listPhongBanIdDaPheDuyet.Contains(x.OrganizationId))
                            .Select(y => y.OrganizationId).ToList();

                        //Lấy số phòng ban mà User đăng nhập là trưởng bộ phận trong số các phòng ban chưa phê duyệt ở bước hiện tại
                        var countPheDuyet = context.ThanhVienPhongBan.Count(x => x.EmployeeId == user.EmployeeId &&
                                                                                 x.IsManager == 1 &&
                                                                                 listPhongBanId.Contains(
                                                                                     x.OrganizationId));

                        //Nếu User đăng nhập là trưởng bộ phận của 1 trong số các phòng ban chưa phê duyệt ở bước hiện tại
                        if (countPheDuyet > 0)
                        {
                            isShowPheDuyet = true;
                            isShowTuChoi = true;
                        }
                    }
                    //Nếu là Phê duyệt trưởng bộ phận cấp trên
                    else if (buocHienTai?.LoaiPheDuyet == 3)
                    {
                        //Lấy phòng ban cấp trên của người đề xuất
                        var _emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                        var exists = context.PhongBanPheDuyetDoiTuong.FirstOrDefault(x =>
                            x.ObjectNumber == kyLuong.KyLuongId && x.DoiTuongApDung == 14 &&
                            x.IsPheDuyetCapTren == true && x.OrganizationId == _emp.OrganizationId);

                        //Nếu phòng ban cấp trên của người đang đăng nhập giống trong cấu hình thì
                        if (exists != null)
                        {
                            isShowPheDuyet = true;
                            isShowTuChoi = true;
                        }
                    }
                }

                //Trạng thái Từ chối và User đăng nhập là người tạo đề xuất
                if (kyLuong.TrangThai == 4 && kyLuong.CreatedById == user.UserId)
                {
                    isShowDatVeMoi = true;
                }

                #endregion

                return new GetKyLuongByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    KyLuong = kyLuong,
                    ListTongHopChamCong = listTongHopChamCong,
                    ListLuongTongHop = listLuongTongHop,
                    ListLuongCtThuNhapTinhThue = listLuongCtThuNhapTinhThue,
                    ListLuongCtBaoHiem = listLuongCtBaoHiem,
                    ListLuongCtGiamTruTruocThue = listLuongCtGiamTruTruocThue,
                    ListLuongCtGiamTruSauThue = listLuongCtGiamTruSauThue,
                    ListLuongCtHoanLaiSauThue = listLuongCtHoanLaiSauThue,
                    ListLuongCtCtyDong = listLuongCtCtyDong,
                    ListLuongCtOther = listLuongCtOther,
                    ListDataTroCapOt = listDataTroCapOt,
                    ListDataHeaderTroCapOt = listDataHeaderTroCapOt,
                    ListDataTroCapCoDinh = listDataTroCapCoDinh,
                    ListDataHeaderTroCapCoDinh = listDataHeaderTroCapCoDinh,
                    ListLoaiTroCapKhac = listLoaiTroCapKhac,
                    ListSelectedLoaiTroCapKhac = listSelectedLoaiTroCapKhac,
                    ListDataTroCapKhac = listDataTroCapKhac,
                    ListDataHeaderTroCapKhac = listDataHeaderTroCapKhac,
                    IsShowGuiPheDuyet = isShowGuiPheDuyet,
                    IsShowPheDuyet = isShowPheDuyet,
                    IsShowDatVeMoi = isShowDatVeMoi,
                    IsShowTuChoi = isShowTuChoi,
                    IsShowXoa = isShowXoa,
                    IsShowSua = isShowSua
                };
            }
            catch (Exception e)
            {
                return new GetKyLuongByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveThuNhapTinhThueResult SaveThuNhapTinhThue(SaveThuNhapTinhThueParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtThuNhapTinhThue>();

                parameter.ListLuongCtThuNhapTinhThue.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtThuNhapTinhThue.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveThuNhapTinhThueResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveThuNhapTinhThueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveBaoHiemResult SaveBaoHiem(SaveBaoHiemParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtBaoHiem>();

                parameter.ListLuongCtBaoHiem.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtBaoHiem.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveBaoHiemResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveBaoHiemResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveGiamTruTruocThueResult SaveGiamTruTruocThue(SaveGiamTruTruocThueParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtGiamTruTruocThue>();

                parameter.ListLuongCtGiamTruTruocThue.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtGiamTruTruocThue.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveGiamTruTruocThueResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveGiamTruTruocThueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveGiamTruSauThueResult SaveGiamTruSauThue(SaveGiamTruSauThueParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtGiamTruSauThue>();

                parameter.ListLuongCtGiamTruSauThue.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtGiamTruSauThue.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveGiamTruSauThueResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveGiamTruSauThueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveHoanLaiSauThueResult SaveHoanLaiSauThue(SaveHoanLaiSauThueParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtHoanLaiSauThue>();

                parameter.ListLuongCtHoanLaiSauThue.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtHoanLaiSauThue.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveHoanLaiSauThueResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveHoanLaiSauThueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveCtyDongResult SaveCtyDong(SaveCtyDongParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtCtyDong>();

                parameter.ListLuongCtCtyDong.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtCtyDong.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveCtyDongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveCtyDongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveOtherResult SaveOther(SaveOtherParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtOther>();

                parameter.ListLuongCtOther.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtOther.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveOtherResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveOtherResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListDieuKienTroCapCoDinhResult GetListDieuKienTroCapCoDinh(GetListDieuKienTroCapCoDinhParameter parameter)
        {
            try
            {
                var luongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                    x.LuongCtLoaiTroCapCoDinhId == parameter.LuongCtLoaiTroCapCoDinhId);
                if (luongCtLoaiTroCapCoDinh == null)
                {
                    return new GetListDieuKienTroCapCoDinhResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Data loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                var loaiTroCapCoDinhType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
                var listLoaiTroCapCoDinh = context.Category
                    .Where(x => x.CategoryTypeId == loaiTroCapCoDinhType.CategoryTypeId).ToList();

                var loaiTroCapCoDinh =
                    listLoaiTroCapCoDinh.FirstOrDefault(x => x.CategoryId == luongCtLoaiTroCapCoDinh.LoaiTroCapId);
                if (loaiTroCapCoDinh == null)
                {
                    return new GetListDieuKienTroCapCoDinhResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                var listLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh.Where(x =>
                    x.LuongCtLoaiTroCapCoDinhId == luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId)
                    .Select(y => new LuongCtDieuKienTroCapCoDinhModel(y)).ToList();

                var dieuKienHuongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC");
                var listDieuKienHuong = context.Category
                    .Where(x => x.CategoryTypeId == dieuKienHuongType.CategoryTypeId).ToList();

                listLuongCtDieuKienTroCapCoDinh.ForEach(item =>
                {
                    var dieuKienHuong = listDieuKienHuong.FirstOrDefault(x => x.CategoryId == item.DieuKienHuongId);
                    item.DieuKienHuong = dieuKienHuong?.CategoryName;
                });

                return new GetListDieuKienTroCapCoDinhResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    LoaiTroCap = loaiTroCapCoDinh?.CategoryName,
                    ListLuongCtDieuKienTroCapCoDinh = listLuongCtDieuKienTroCapCoDinh
                };
            }
            catch (Exception e)
            {
                return new GetListDieuKienTroCapCoDinhResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateDieuKienTroCapCoDinhResult UpdateDieuKienTroCapCoDinh(UpdateDieuKienTroCapCoDinhParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x =>
                        x.KyLuongId == parameter.LuongCtDieuKienTroCapCoDinh.KyLuongId);

                    if (kyLuong.TrangThai != 1)
                    {
                        return new UpdateDieuKienTroCapCoDinhResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Lưu thất bại, kỳ lương phải ở trạng thái mới"
                        };
                    }

                    //Lấy các điều kiện hưởng trong các loại trợ cấp khác nếu cùng điều kiện hưởng
                    var listUpdate = context.LuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtTroCapCoDinhId == parameter.LuongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.DieuKienHuongId == parameter.LuongCtDieuKienTroCapCoDinh.DieuKienHuongId)
                        .ToList();

                    listUpdate.ForEach(item => { item.DuDieuKien = parameter.LuongCtDieuKienTroCapCoDinh.DuDieuKien; });
                    
                    context.LuongCtDieuKienTroCapCoDinh.UpdateRange(listUpdate);
                    context.SaveChanges();

                    #region Tính lại trợ cấp

                    var listTroCapCoDinh = context.TroCap.Where(x => x.TypeId != 4).ToList();
                    var luongCtTroCapCoDinh = context.LuongCtTroCapCoDinh
                        .FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == parameter.LuongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId);

                    //Lấy list loại trợ cấp có thay đổi
                    var listLuongCtLoaiTroCapCoDinhId = listUpdate.Select(y => y.LuongCtLoaiTroCapCoDinhId).ToList();
                    var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh
                        .Where(x => listLuongCtLoaiTroCapCoDinhId.Contains(x.LuongCtLoaiTroCapCoDinhId))
                        .ToList();

                    var listChamCongByEmp = context.ChamCong
                        .Where(x => x.NgayChamCong.Date >= kyLuong.TuNgay.Date &&
                                    x.NgayChamCong.Date <= kyLuong.DenNgay.Date &&
                                    x.EmployeeId == luongCtTroCapCoDinh.EmployeeId).ToList();
                    var listMucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi.ToList();

                    var listDmvs = context.ThongKeDiMuonVeSom
                        .Where(x => x.EmployeeId == luongCtTroCapCoDinh.EmployeeId &&
                                    x.NgayDmvs.Date >= kyLuong.TuNgay.Date &&
                                    x.NgayDmvs.Date <= kyLuong.DenNgay.Date).ToList();

                    listLuongCtLoaiTroCapCoDinh.ForEach(item =>
                    {
                        var troCap =
                            listTroCapCoDinh.FirstOrDefault(x => x.LoaiTroCapId == item.LoaiTroCapId);

                        //
                        if (troCap != null && troCap.TypeId == 1)
                        {
                            var ngayLamViecThucTe = context.TongHopChamCong
                                                        .FirstOrDefault(x => x.KyLuongId == item.KyLuongId &&
                                                                             x.EmployeeId == luongCtTroCapCoDinh.EmployeeId)
                                                        ?.NgayLamViecThucTe ?? 0;

                            var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                x.KyLuongId == item.KyLuongId &&
                                x.LuongCtLoaiTroCapCoDinhId == item.LuongCtLoaiTroCapCoDinhId &&
                                x.DuDieuKien == false);

                            //Nếu tất cả điều kiện hưởng đều = true
                            if (countFalse == 0)
                            {
                                item.MucTroCap =
                                    Math.Round(troCap.MucTroCap * ngayLamViecThucTe / kyLuong.SoNgayLamViec, 0);
                            }
                            //Nếu có điều kiện hưởng = false
                            else
                            {
                                item.MucTroCap = 0;
                            }
                        }
                        //Trợ cấp theo ngày nghỉ
                        else if (troCap != null && troCap.TypeId == 2)
                        {
                            var listMucHuong = listMucHuongTheoNgayNghi
                                .Where(x => x.TroCapId == troCap.TroCapId)
                                .OrderByDescending(z => z.MucHuongPhanTram).ToList();

                            var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == item.KyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == item.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                            //Nếu tất cả điều kiện hưởng đều = true
                            if (countFalse == 0)
                            {
                                item.MucTroCap = TinhMucHuongTroCapTheoNgayNghi(troCap, listChamCongByEmp, listMucHuong);
                            }
                            //Nếu có điều kiện hưởng = false
                            else
                            {
                                item.MucTroCap = 0;
                            }
                        }
                        //Trợ cấp theo số lần đi muộn về sớm
                        else if (troCap != null && troCap.TypeId == 3)
                        {
                            var listMucHuong = context.MucHuongDmvs.Where(x => x.TroCapId == troCap.TroCapId)
                                .OrderByDescending(z => z.MucTru).ToList();

                            var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == item.KyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == item.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                            //Nếu tất cả điều kiện hưởng đều = true
                            if (countFalse == 0)
                            {
                                int soLanDmvs = GetSoLanDmvs(luongCtTroCapCoDinh.EmployeeId, listDmvs);

                                item.MucTroCap = TinhMucHuongTroCapTheoDmvs(troCap, soLanDmvs, listMucHuong);
                            }
                            //Nếu có điều kiện hưởng = false
                            else
                            {
                                item.MucTroCap = 0;
                            }
                        }
                    });

                    context.LuongCtLoaiTroCapCoDinh.UpdateRange(listLuongCtLoaiTroCapCoDinh);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();

                    return new UpdateDieuKienTroCapCoDinhResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new UpdateDieuKienTroCapCoDinhResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveLuongCtTroCapKhacResult SaveLuongCtTroCapKhac(SaveLuongCtTroCapKhacParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new SaveLuongCtTroCapKhacResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại"
                        };
                    }

                    var listLoaiTroCapKhac = context.Category
                        .Where(x => parameter.ListLoaiTroCapKhacId.Contains(x.CategoryId)).ToList();

                    var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= kyLuong.TuNgay.Date &&
                                                                   x.NgayChamCong.Date <= kyLuong.DenNgay.Date).ToList();
                    var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
                    var listEmp = context.Employee.Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user })
                        .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                                    x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();

                    listEmp.ForEach(item => { SaveLuongCt_TroCapKhac(kyLuong.KyLuongId, item.Emp, listLoaiTroCapKhac); });

                    trans.Commit();

                    return new SaveLuongCtTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new SaveLuongCtTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListDieuKienTroCapKhacResult GetListDieuKienTroCapKhac(GetListDieuKienTroCapKhacParameter parameter)
        {
            try
            {
                var luongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                    x.LuongCtLoaiTroCapKhacId == parameter.LuongCtLoaiTroCapKhacId);
                if (luongCtLoaiTroCapKhac == null)
                {
                    return new GetListDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Data loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                var loaiTroCapKhacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCK");
                var listLoaiTroCapKhac = context.Category
                    .Where(x => x.CategoryTypeId == loaiTroCapKhacType.CategoryTypeId).ToList();

                var loaiTroCapKhac =
                    listLoaiTroCapKhac.FirstOrDefault(x => x.CategoryId == luongCtLoaiTroCapKhac.LoaiTroCapId);
                if (listLoaiTroCapKhac == null)
                {
                    return new GetListDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                bool isEdit = loaiTroCapKhac.IsEdit;
                var existCauHinhLoaiTroCap =
                    context.TroCap.FirstOrDefault(x => x.LoaiTroCapId == luongCtLoaiTroCapKhac.LoaiTroCapId);
                bool isCoCauHinhLoaiTroCap = existCauHinhLoaiTroCap != null;

                var luongCtTroCapKhac = context.LuongCtTroCapKhac.FirstOrDefault(x =>
                    x.LuongCtTroCapKhacId == luongCtLoaiTroCapKhac.LuongCtTroCapKhacId);
                if (luongCtTroCapKhac == null)
                {
                    return new GetListDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Data trợ cấp khác không tồn tại trên hệ thống"
                    };
                }

                var listLuongCtEmpDktcKhac = context.LuongCtEmpDktcKhac.Where(x =>
                    x.EmployeeId == luongCtTroCapKhac.EmployeeId &&
                    x.KyLuongId == luongCtTroCapKhac.KyLuongId).ToList();

                var listLuongCtDieuKienTroCapKhac = context.LuongCtDieuKienTroCapKhac.Where(x =>
                    x.LuongCtLoaiTroCapKhacId == luongCtLoaiTroCapKhac.LuongCtLoaiTroCapKhacId)
                    .Select(y => new LuongCtDieuKienTroCapKhacModel(y)).ToList();

                if (isCoCauHinhLoaiTroCap && listLuongCtDieuKienTroCapKhac.Count == 0)
                {
                    return new GetListDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Loại trợ cấp này không có điều kiện hưởng hoặc nhân viên không đủ điều kiện hưởng trợ cấp"
                    };
                }

                var dieuKienHuongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC");
                var listDieuKienHuong = context.Category
                    .Where(x => x.CategoryTypeId == dieuKienHuongType.CategoryTypeId).ToList();

                listLuongCtDieuKienTroCapKhac.ForEach(item =>
                {
                    var dieuKienHuong = listDieuKienHuong.FirstOrDefault(x => x.CategoryId == item.DieuKienHuongId);
                    item.DieuKienHuong = dieuKienHuong?.CategoryName;

                    var duDieuKienMapping =
                        listLuongCtEmpDktcKhac.FirstOrDefault(x => x.DieuKienHuongId == item.DieuKienHuongId);
                    item.DuDieuKien = duDieuKienMapping.DuDieuKien;
                });

                return new GetListDieuKienTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    LoaiTroCap = loaiTroCapKhac?.CategoryName,
                    MucTroCap = luongCtLoaiTroCapKhac.MucTroCap,
                    ListLuongCtDieuKienTroCapKhac = listLuongCtDieuKienTroCapKhac,
                    IsCoCauHinhLoaiTroCap = isCoCauHinhLoaiTroCap
                };
            }
            catch (Exception e)
            {
                return new GetListDieuKienTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateDieuKienTroCapKhacResult UpdateDieuKienTroCapKhac(UpdateDieuKienTroCapKhacParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x =>
                        x.KyLuongId == parameter.LuongCtDieuKienTroCapKhac.KyLuongId);

                    if (kyLuong.TrangThai != 1)
                    {
                        return new UpdateDieuKienTroCapKhacResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Lưu thất bại, kỳ lương phải ở trạng thái mới"
                        };
                    }

                    SaveLuongCtEmpDktcKhac(parameter.EmployeeId, kyLuong.KyLuongId,
                        parameter.LuongCtDieuKienTroCapKhac.DieuKienHuongId,
                        parameter.LuongCtDieuKienTroCapKhac.DuDieuKien);

                    #region Tính lại trợ cấp

                    var listTroCapKhac = context.TroCap.Where(x => x.TypeId == 4).ToList();
                    var luongCtTroCapKhac = context.LuongCtTroCapKhac
                        .FirstOrDefault(x =>
                            x.LuongCtTroCapKhacId == parameter.LuongCtDieuKienTroCapKhac.LuongCtTroCapKhacId);

                    //Lấy list loại trợ cấp có thay đổi
                    var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac
                        .Where(x => x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId)
                        .ToList();
                    var listDieuKienHuong = context.LuongCtDieuKienTroCapKhac
                        .Where(x => x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId).ToList();

                    listLuongCtLoaiTroCapKhac.ForEach(item =>
                    {
                        var troCap =
                            listTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == item.LoaiTroCapId);

                        if (troCap != null)
                        {
                            var listDieuKienHuongId = listDieuKienHuong.Where(x =>
                                    x.LuongCtLoaiTroCapKhacId == item.LuongCtLoaiTroCapKhacId)
                                .Select(y => y.DieuKienHuongId).ToList();

                            var countFalse = context.LuongCtEmpDktcKhac.Count(x =>
                                x.EmployeeId == parameter.EmployeeId &&
                                x.KyLuongId == kyLuong.KyLuongId &&
                                listDieuKienHuongId.Contains(x.DieuKienHuongId) &&
                                x.DuDieuKien == false);

                            //Nếu tất cả điều kiện hưởng đều = true
                            if (countFalse == 0)
                            {
                                item.MucTroCap = troCap.MucTroCap;
                            }
                            //Nếu có điều kiện hưởng = false
                            else
                            {
                                item.MucTroCap = 0;
                            }
                        }
                    });

                    context.LuongCtLoaiTroCapKhac.UpdateRange(listLuongCtLoaiTroCapKhac);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();

                    return new UpdateDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new UpdateDieuKienTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateMucTroCapKhacResult UpdateMucTroCapKhac(UpdateMucTroCapKhacParameter parameter)
        {
            try
            {
                var loaiTroCapKhac = context.LuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                    x.LuongCtLoaiTroCapKhacId == parameter.LuongCtLoaiTroCapKhacId);
                if (loaiTroCapKhac == null)
                {
                    return new UpdateMucTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                loaiTroCapKhac.MucTroCap = parameter.MucTroCap;
                context.LuongCtLoaiTroCapKhac.Update(loaiTroCapKhac);
                context.SaveChanges();

                return new UpdateMucTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new UpdateMucTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ExportExcelKyLuongMealAllowanceResult ExportExcelKyLuongMealAllowance(ExportExcelKyLuongMealAllowanceParameter parameter)
        {
            try
            {
                if (parameter.KyLuongId == 0 || parameter.KyLuongId == null)
                {
                    return new ExportExcelKyLuongMealAllowanceResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Kỳ lương không tồn tại trên hệ thống",
                    };
                }

                var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                var kyLuongTruoc = context.KyLuong.FirstOrDefault(x => x.DenNgay.Date == kyLuong.TuNgay.AddDays(-1).Date);
                var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                var listEmpId = listLuongTongHop.Select(y => y.EmployeeId).ToList();
                var listEmp = context.Employee
                    .Join(listEmpId,
                        emp => emp.EmployeeId,
                        empId => empId,
                        (emp, empId) => new { Emp = emp, EmpId = empId })
                    .ToList();
                var listContact = context.Contact.Join(context.Employee,
                        cont => cont.ObjectId,
                        emp => emp.EmployeeId,
                        (cont, emp) => new {Cont = cont, Emp = emp}) 
                    .Where(x => x.Cont.ObjectType == ContactObjectType.EMP)
                    .ToList();
                var listHopDongNhanSu = context.HopDongNhanSu.ToList();
                var troCapCoDinhType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
                var listLoaiTroCapCoDinh = context.Category.Where(x => x.CategoryTypeId == troCapCoDinhType.CategoryTypeId)
                    .ToList();
                var loaiTcAnTruaId = listLoaiTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCAT").CategoryId;
                var loaiTcChuyenCanNgayCongId =
                    listLoaiTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCCCNC").CategoryId;
                var loaiTcDmvsId = listLoaiTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCCCDMVS").CategoryId;
                var troCapAnTrua = context.TroCap.FirstOrDefault(x => x.TypeId == 1 &&
                                                                 x.LoaiTroCapId == loaiTcAnTruaId);
                var troCapChuyenCanNgayCong =
                    context.TroCap.FirstOrDefault(x => x.LoaiTroCapId == loaiTcChuyenCanNgayCongId);
                var troCapDmvs = context.TroCap.FirstOrDefault(x => x.LoaiTroCapId == loaiTcDmvsId);
                var listSubCode1 = GeneralList.GetSubCode1();
                var listLuongCtTroCapCoDinh =
                    context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh
                    .Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                var listLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh
                    .Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == parameter.KyLuongId)
                    .Select(y => new TongHopChamCongModel(y)).ToList();
                var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS").CategoryTypeId;
                var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType).ToList();
                var dieuKienHuongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC")
                    .CategoryTypeId;
                var listDieuKienHuong = context.Category.Where(x => x.CategoryTypeId == dieuKienHuongType).ToList();
                var dieuKienHuong_DuocTcAnTrua = listDieuKienHuong.FirstOrDefault(x => x.CategoryCode == "DKH003");
                var dieuKienHuong_DuocTcChuyenCan = listDieuKienHuong.FirstOrDefault(x => x.CategoryCode == "DKH004");

                string[] listTile2 =
                {
                    "BẢNG TÍNH TIỀN ĂN TRƯA TỪ " + kyLuong.TuNgay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +
                    " ĐẾN " + kyLuong.DenNgay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    "BẢNG CÔNG THÁNG " + kyLuong.DenNgay.Month.ToString("D2") + " NĂM " + kyLuong.DenNgay.Year,
                };

                string[] listTile3 = {
                    "BẢNG CÔNG THÁNG " + kyLuong.DenNgay.Month.ToString("D2") + " NĂM " + kyLuong.DenNgay.Year
                };

                var ListTile = new List<dynamic>();
                ListTile.Add(listTile2);
                ListTile.Add(listTile3);

                string[] listHeader1 = {
                    "Số TT/ No.",
                    "Mã nhân viên/ Emp. Code",
                    "Họ và tên tiếng Việt/ Full name in English",
                    "Họ và tên tiếng Anh/ Full name in Vietnam",
                    "LƯƠNG CƠ BẢN HĐ THỬ VIỆC/ Basic Salary in probation period",
                    "LƯƠNG CƠ BẢN KÝ HỢP ĐỒNG/  Basic Salary in Labor contract",
                    "Balance"
                };

                string[] listHeader21 = {
                    "No.",
                    "Mã nhân viên",
                    "Mã phòng",
                    "Họ tên",
                    "Được trả tiền ăn (Yes/ No)",
                    "Số tiền ăn TB/ ngày",
                    "DỰA THEO SỐ NGÀY LÀM VIỆC THỰC TẾ",
                    "Số tiền ăn được trả tháng này",
                    "Số tiền ăn trả thiếu tháng trước",
                    "Số tiền ăn trả thừa tháng trước",
                    "Số tiền ăn được trả tháng này (+), phải trả thêm tháng này (-)",
                    "Ghi chú"
                };

                dynamic[] listHeader22 = {
                    "",
                    "",
                    "",
                    "",
                    "",
                    troCapAnTrua.MucTroCap,
                    "",
                    kyLuong.SoNgayLamViec,
                    kyLuongTruoc?.SoNgayLamViec ?? 0,
                    kyLuongTruoc?.SoNgayLamViec ?? 0,
                    "",
                    "",
                };

                string[] listHeader31 = {
                    "No.",
                    "Mã nhân viên",
                    "Mã phòng",
                    "Họ tên",
                    "Type of Contract",
                    "Được trả Chuyên Cần (Yes/ No)",
                    "Trợ cấp chuyên cần ngày công",
                    "",
                    "",
                    "",
                    "",
                    "Trợ cấp chuyên cần đi muộn về sớm",
                    "",
                    "",
                    "",
                    "Ghi chú",
                };

                string[] listHeader32 = {
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "Tổng ngày nghỉ đột xuất trong tháng",
                    "Tổng ngày nghỉ trong tháng",
                    "Tổng ngày làm việc thực tế trong tháng",
                    "Trợ cấp chuyên cần ngày công theo ngày làm việc thực tế",
                    "Trợ cấp chuyên cần ngày công được hưởng",
                    "Tổng số lần đi muộn về sớm",
                    "Tổng ngày làm việc trong tháng",
                    "Trợ cấp chuyên cần đi muộn về sớm",
                    "Trợ cấp chuyên cần ngày công được hưởng",
                    "",
                };

                dynamic[] listHeader33 = {
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    troCapChuyenCanNgayCong?.MucTroCap ?? 0,
                    "",
                    "",
                    "",
                    troCapDmvs?.MucTroCap ?? 0,
                    "",
                };

                var ListHeader = new List<dynamic>();
                var listHeader3 = new List<dynamic>();
                var listHeader2 = new List<dynamic>();

                listHeader2.Add(listHeader21);
                listHeader2.Add(listHeader22);

                listHeader3.Add(listHeader31);
                listHeader3.Add(listHeader32);
                listHeader3.Add(listHeader33);

                ListHeader.Add(listHeader1);
                ListHeader.Add(listHeader2);
                ListHeader.Add(listHeader3);

                var ListData = new List<templateDueDateContract>();
                var ListData1 = new List<templateMealAllowance>();
                var ListData2 = new List<templateTroCapChuyenCan>();

                if (parameter.number == 1)
                {
                    listEmp.ForEach(emp =>
                    {
                        var hopDongCuNhat = listHopDongNhanSu.Where(x => x.EmployeeId == emp.EmpId)
                            .OrderBy(z => z.NgayKyHopDong).FirstOrDefault();
                        decimal luongThuViec = hopDongCuNhat == null ? 0 : hopDongCuNhat.MucLuong;
                        var luongTongHop = listLuongTongHop.FirstOrDefault(x => x.EmployeeId == emp.EmpId);
                        var luongHienTai = luongTongHop.MucLuongHienTai;

                        var obj = new templateDueDateContract();
                        obj.maNhanVien = emp.Emp.EmployeeCode;
                        obj.hoTen = emp.Emp.EmployeeName;
                        obj.hoTenTiengAnh = emp.Emp.HoTenTiengAnh;
                        obj.luongThuViec = luongThuViec;
                        obj.luongHopDong = luongHienTai;

                        ListData.Add(obj);
                    });
                }
                else if (parameter.number == 2)
                {
                    listEmp.ForEach(emp =>
                    {
                        var troCapCoDinh = listLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == emp.EmpId);
                        var dieuKienHuongTroCapAnTrua = listLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == troCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.DieuKienHuongId == dieuKienHuong_DuocTcAnTrua.CategoryId);
                        var tongHopChamCong = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == emp.EmpId);
                        var loaiHopDong =
                            listLoaiHopDong.FirstOrDefault(x => x.CategoryId == troCapCoDinh.LoaiHopDongId);
                        var dieuKienHuongTroCapChuyenCan = listLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == troCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.DieuKienHuongId == dieuKienHuong_DuocTcChuyenCan.CategoryId);

                        var obj = new templateMealAllowance();
                        obj.code = emp.Emp.EmployeeCode;
                        obj.maPhong = listSubCode1.FirstOrDefault(x => x.Value == emp.Emp.SubCode1Value)?.Name;
                        obj.name = emp.Emp.EmployeeName;
                        obj.duocTraTienAn = dieuKienHuongTroCapAnTrua == null
                            ? "Yes"
                            : (dieuKienHuongTroCapAnTrua.DuDieuKien == true ? "Yes" : "No");
                        obj.soTienAnTB = 0;
                        obj.soNgayLamViec = tongHopChamCong.NgayLamViecThucTe;
                        obj.soTienAnDuocTra = 0;
                        obj.soTienAnThieu = 0;
                        obj.soTienAnThua = 0;
                        obj.sumTienAnDuocTra = 0;
                        obj.ghiChu = "";

                        ListData1.Add(obj);

                        var _obj = new templateTroCapChuyenCan();
                        _obj.maNV = emp.Emp.EmployeeCode;
                        _obj.maPhong = listSubCode1.FirstOrDefault(x => x.Value == emp.Emp.SubCode1Value)?.Name;
                        _obj.name = emp.Emp.EmployeeName;
                        _obj.typeOfContact = loaiHopDong.CategoryCode.Substring(0, 4);
                        _obj.chuyenCan = dieuKienHuongTroCapChuyenCan == null
                            ? "Yes"
                            : (dieuKienHuongTroCapChuyenCan.DuDieuKien == true ? "Yes" : "No");
                        _obj.ngayNghiDotXuat = tongHopChamCong.NghiKhongPhep;
                        _obj.ngayNghi = tongHopChamCong.TongNgayNghiTinhTroCapChuyenCan;
                        _obj.ngayLamViec = tongHopChamCong.NgayLamViecThucTe;
                        _obj.troCapTheoNgayLam = 0;
                        _obj.troCapChuyenCan = 0;
                        _obj.soLanDMVS = tongHopChamCong.SoLanTruChuyenCan;
                        _obj.soNgayLamViec = kyLuong.SoNgayLamViec;
                        _obj.troCapDMVS = 0;
                        _obj.troCapChuyenCanNgayCong = 0;
                        _obj.ghiChu = "";

                        ListData2.Add(_obj);
                    });
                }

                return new ExportExcelKyLuongMealAllowanceResult()
                {
                    listTitle = ListTile,
                    listHeader = ListHeader,
                    data = ListData,
                    data1 = ListData1,
                    data2 = ListData2,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                };
            }
            catch (Exception e)
            {
                return new ExportExcelKyLuongMealAllowanceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ImportTroCapKhacResult ImportTroCapKhac(ImportTroCapKhacParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    if (parameter.ListData.Count == 0)
                    {
                        return new ImportTroCapKhacResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "File import không có dữ liệu"
                        };
                    }

                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new ImportTroCapKhacResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    var listEmp = context.Employee.ToList();
                    var listCauHinhTroCapKhac = context.TroCap.Where(x => x.TypeId == 4).ToList();
                    var listLuongCtTroCapKhac =
                        context.LuongCtTroCapKhac.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLoaiTroCapKhacId = context.LuongCtLoaiTroCapKhac.Where(x => x.KyLuongId == parameter.KyLuongId)
                        .Select(y => y.LoaiTroCapId).Distinct().ToList();
                    var listLoaiTroCapKhac = context.Category
                        .Where(x => listLoaiTroCapKhacId.Contains(x.CategoryId)).ToList();

                    var messEmpHeThong = new List<string>();
                    var messEmpBangLuong = new List<string>();
                    var messLoaiTroCap = new List<string>();

                    var listUpdate = new List<LuongCtLoaiTroCapKhac>();

                    parameter.ListData.ForEach(item =>
                    {
                        var emp = listEmp.FirstOrDefault(x =>
                            x.EmployeeCode.ToLower().Trim() == item.EmployeeCode?.ToLower()?.Trim());

                        if (emp == null)
                        {
                            messEmpHeThong.Add(item.EmployeeCode);
                        }
                        else
                        {
                            var empBangLuong = listLuongCtTroCapKhac.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);

                            //Nếu không có trong bảng lương
                            if (empBangLuong == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            //Nếu có trong bảng lương
                            else
                            {
                                item.ListDataLoaiTroCap.ForEach(_item =>
                                {
                                    var loaiTroCap = listLoaiTroCapKhac.FirstOrDefault(x =>
                                        x.CategoryName.Trim() == _item.TenLoaiTroCap?.Trim());

                                    //Nếu tên loại trợ cấp không đúng
                                    if (loaiTroCap == null)
                                    {
                                        messLoaiTroCap.Add(_item.TenLoaiTroCap);
                                    }
                                    //Nếu tên loại trợ cấp đúng trong bảng mapping (bảng 2)
                                    else
                                    {
                                        var existsCauHinh =
                                            listCauHinhTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCap.CategoryId);

                                        //Nếu loại trợ cấp này không có trong cấu hình
                                        if (existsCauHinh == null)
                                        {
                                            //Update bảng 2
                                            var luongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                                                x.LuongCtTroCapKhacId == empBangLuong.LuongCtTroCapKhacId &&
                                                x.KyLuongId == parameter.KyLuongId &&
                                                x.LoaiTroCapId == loaiTroCap.CategoryId);

                                            luongCtLoaiTroCapKhac.MucTroCap = _item.MucTroCap;

                                            listUpdate.Add(luongCtLoaiTroCapKhac);
                                        }
                                    }
                                });
                            }
                        }
                    });

                    bool isError = true;
                    //Nếu không có lỗi
                    if (messEmpHeThong.Count == 0 && messEmpBangLuong.Count == 0 && messLoaiTroCap.Count == 0)
                    {
                        context.LuongCtLoaiTroCapKhac.UpdateRange(listUpdate);
                        context.SaveChanges();

                        trans.Commit();

                        isError = false;
                    }

                    return new ImportTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Import thành công",
                        IsError = isError,
                        MessEmpHeThong = messEmpHeThong.Join(),
                        MessEmpBangLuong = messEmpBangLuong.Join(),
                        MessLoaiTroCap = messLoaiTroCap.Join()
                };
                }
            }
            catch (Exception e)
            {
                return new ImportTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ImportLuongChiTietResult ImportLuongChiTiet(ImportLuongChiTietParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var messEmpHeThong = new List<string>();
                    var messEmpBangLuong = new List<string>();
                    bool isError = true;

                    if (parameter.ListData.Count == 0)
                    {
                        return new ImportLuongChiTietResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "File import không có dữ liệu"
                        };
                    }

                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.ListData.First().KyLuongId);
                    if (kyLuong == null)
                    {
                        return new ImportLuongChiTietResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    var listEmp = context.Employee.ToList();
                    var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtBaoHiem =
                        context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtGiamTruSauThue = context.LuongCtGiamTruSauThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtHoanLaiSauThue = context.LuongCtHoanLaiSauThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtCtyDong =
                        context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtOther = context.LuongCtOther.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                    parameter.ListData.ForEach(item =>
                    {
                        var emp = listEmp.FirstOrDefault(x =>
                            x.EmployeeCode.ToLower().Trim() == item.EmployeeCode?.ToLower()?.Trim());

                        if (emp == null)
                        {
                            messEmpHeThong.Add(item.EmployeeCode);
                        }
                        else
                        {
                            var emp1 = listLuongCtThuNhapTinhThue.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp1 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp1.NetToGross = item.ThuNhapTinhThueNetToGross;
                                emp1.Month13 = item.ThuNhapTinhThueMonth13;
                                emp1.Gift = item.ThuNhapTinhThueGift;
                                emp1.Other = item.ThuNhapTinhThueOther;

                                context.LuongCtThuNhapTinhThue.Update(emp1);
                            }

                            var emp2 = listLuongCtBaoHiem.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp2 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp2.Other = item.BaoHiemOther;

                                context.LuongCtBaoHiem.Update(emp2);
                            }

                            var emp3 = listLuongCtGiamTruTruocThue.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp3 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp3.GiamTruKhac = item.GiamTruTruocThueGiamTruKhac;

                                context.LuongCtGiamTruTruocThue.Update(emp3);
                            }

                            var emp4 = listLuongCtGiamTruSauThue.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp4 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp4.QuyetToanThueTncn = item.GiamTruSauThueQuyetToanThueTncn;
                                emp4.Other = item.GiamTruSauThueOther;

                                context.LuongCtGiamTruSauThue.Update(emp4);
                            }

                            var emp5 = listLuongCtHoanLaiSauThue.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp5 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp5.ThueTncn = item.HoanLaiSauThueThueTncn;
                                emp5.Other = item.HoanLaiSauThueOther;

                                context.LuongCtHoanLaiSauThue.Update(emp5);
                            }

                            var emp6 = listLuongCtCtyDong.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp6 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp6.Other = item.CtyDongOther;
                                emp6.FundOct = item.CtyDongFundOct;

                                context.LuongCtCtyDong.Update(emp6);
                            }

                            var emp7 = listLuongCtOther.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp7 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp7.KhoanBuTruThangTruoc = item.OtherKhoanBuTruThangTruoc;
                                emp7.TroCapKhacKhongTinhThue = item.OtherTroCapKhacKhongTinhThue;
                                emp7.KhauTruHoanLaiTruocThue = item.OtherKhauTruHoanLaiTruocThue;
                                emp7.LuongTamUng = item.OtherLuongTamUng;

                                context.LuongCtOther.Update(emp7);
                            }
                        }
                    });

                    messEmpBangLuong = messEmpBangLuong.Select(y => y).Distinct().ToList();
                    //Nếu không có lỗi
                    if (messEmpHeThong.Count == 0 && messEmpBangLuong.Count == 0)
                    {
                        context.SaveChanges();
                        trans.Commit();

                        isError = false;
                    }

                    return new ImportLuongChiTietResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Import thành công",
                        IsError = isError,
                        MessEmpHeThong = messEmpHeThong.Join(),
                        MessEmpBangLuong = messEmpBangLuong.Join(),
                    };
                }
            }
            catch (Exception e)
            {
                return new ImportLuongChiTietResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public HoanThanhKyLuongResult HoanThanhKyLuong(HoanThanhKyLuongParameter parameter)
        {
            var listPhieuLuong = new List<PhieuLuongModel>();

            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                    if (user == null)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Người dùng không tồn tại trên hệ thống"
                        };
                    }

                    var empUser = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                    if (empUser == null)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Người dùng không tồn tại trên hệ thống"
                        };
                    }

                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    //Nếu kỳ lương có trạng thái khác Đã phê duyệt
                    if (kyLuong.TrangThai != 3)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương có trạng thái khác Đã phê duyệt"
                        };
                    }

                    //Chuyển trạng thái => Hoàn thành
                    kyLuong.TrangThai = 5;
                    context.KyLuong.Update(kyLuong);

                    var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listEmpId = listLuongTongHop.Select(y => y.EmployeeId).ToList();
                    var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).ToList();
                    var listContact = context.Contact.Where(x => x.ObjectType == ContactObjectType.EMP &&
                                                                 listEmpId.Contains(x.ObjectId)).ToList();
                    var listGetSubCode1 = GeneralList.GetSubCode1();
                    var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == parameter.KyLuongId)
                        .Select(y => new TongHopChamCongModel(y)).ToList();
                    var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue
                        .Where(x => x.KyLuongId == parameter.KyLuongId).ToList();

                    var troCapCoDinhType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
                    var listTroCapCoDinh = context.Category.Where(x => x.CategoryTypeId == troCapCoDinhType.CategoryTypeId)
                        .ToList();
                    var troCapDiLai = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCDL");
                    var troCapDienThoai = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCDT");
                    var troCapAnTrua = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCAT");
                    var troCapCcnc = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCCCNC");
                    var troCapDmvs = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCCCDMVS");

                    var listTroCap = context.TroCap.ToList();
                    var troCapCcncCauHinh = listTroCap.FirstOrDefault(x => x.LoaiTroCapId == troCapCcnc.CategoryId);
                    var troCapDmvsCauHinh = listTroCap.FirstOrDefault(x => x.LoaiTroCapId == troCapDmvs.CategoryId);

                    var listLuongCtTroCapCoDinh = context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh
                        .Where(x => x.KyLuongId == parameter.KyLuongId).ToList();

                    var dieuKienHuongTroCapType =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC");
                    var listDieuKienHuongTroCap =
                        context.Category.Where(x => x.CategoryTypeId == dieuKienHuongTroCapType.CategoryTypeId).ToList();
                    var dieuKienHuongDatKpi = listDieuKienHuongTroCap.FirstOrDefault(x => x.CategoryCode == "DKH002");
                    var dieuKienHuongDuocHuongTccc =
                        listDieuKienHuongTroCap.FirstOrDefault(x => x.CategoryCode == "DKH004");

                    var troCapKhacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCK");
                    var listTroCapKhac = context.Category.Where(x => x.CategoryTypeId == troCapKhacType.CategoryTypeId)
                        .ToList();
                    var troCapThuongKpi = listTroCapKhac.FirstOrDefault(x => x.CategoryCode == "TKPITT");
                    var troCapTrachNhiem = listTroCapKhac.FirstOrDefault(x => x.CategoryCode == "TCTN1");

                    var listLuongCtTroCapKhac = context.LuongCtTroCapKhac.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();

                    var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLuongCtCtyDong = context.LuongCtCtyDong.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();

                    var cauHinhGiamTruBanThan = context.CauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 1);
                    var cauHinhGiamTruNguoiPhuThuoc = context.CauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 2);

                    var cauHinhKinhPhiCongDoan = context.KinhPhiCongDoan.FirstOrDefault();

                    var cauHinhBaoHiem = context.CauHinhBaoHiem.FirstOrDefault();

                    if (listLuongTongHop.Count == 0)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương này không có nhân viên"
                        };
                    }
                    
                    listLuongTongHop.ForEach(item =>
                    {
                        var emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                        var contact = listContact.FirstOrDefault(x => x.ObjectId == item.EmployeeId);
                        var tongHopChamCong = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                        var luongCtGiamTruTruocThue =
                            listLuongCtGiamTruTruocThue.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                        var luongCtTroCapCoDinh =
                            listLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        var luongCtTroCapDiLai = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapDiLai.CategoryId);
                        var listLuongCtDieuKienTroCapDiLai = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapDiLai.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapDienThoai = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapDienThoai.CategoryId);
                        var listLuongCtDieuKienTroCapDienThoai = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapDienThoai.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapAnTrua = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapAnTrua.CategoryId);
                        var listLuongCtDieuKienTroCapAnTrua = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapAnTrua.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapCcnc = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapCcnc.CategoryId);
                        var listLuongCtDieuKienTroCapCcnc = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapCcnc.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapDmvs = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapDmvs.CategoryId);
                        var listLuongCtDieuKienTroCapDmvs = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapDmvs.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapKhac =
                            listLuongCtTroCapKhac.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        var luongCtTroCapKpi = listLuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                            x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId &&
                            x.LoaiTroCapId == troCapThuongKpi.CategoryId);

                        var luongCtTroCapTrachNhiem = listLuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                            x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId &&
                            x.LoaiTroCapId == troCapTrachNhiem.CategoryId);

                        var luongCtThuNhapTinhThue =
                            listLuongCtThuNhapTinhThue.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        var luongCtCtyDong = listLuongCtCtyDong.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        var phieuLuong = new PhieuLuongModel();
                        phieuLuong.PhieuLuongCode = GenCodePhieuLuong();
                        phieuLuong.KyLuongId = parameter.KyLuongId;
                        phieuLuong.TenKyLuong = kyLuong.TenKyLuong;
                        phieuLuong.EmployeeId = item.EmployeeId;
                        phieuLuong.EmployeeCode = emp.EmployeeCode;
                        phieuLuong.EmployeeName = emp.EmployeeName;
                        phieuLuong.SubCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == emp.SubCode1Value)?.Name;
                        phieuLuong.OrganizationId = item.OrganizationId;
                        phieuLuong.PositionId = item.PositionId;
                        phieuLuong.CreatedByEmpId = user.EmployeeId;

                        phieuLuong.WorkEmail = contact?.WorkEmail;
                        phieuLuong.SoNgayLamViec = kyLuong.SoNgayLamViec;
                        phieuLuong.NgayBatDauKyLuong = kyLuong.TuNgay.Day.ToString("D2");
                        phieuLuong.ThangBatDauKyLuong = kyLuong.TuNgay.Month.ToString("D2");
                        phieuLuong.NamBatDauKyLuong = kyLuong.TuNgay.Year.ToString();
                        phieuLuong.NgayKetThucKyLuong = kyLuong.DenNgay.Day.ToString("D2");
                        phieuLuong.ThangKetThucKyLuong = kyLuong.DenNgay.Month.ToString("D2");
                        phieuLuong.NamKetThucKyLuong = kyLuong.DenNgay.Year.ToString();

                        var thangTruoc = kyLuong.TuNgay.AddDays(-30);
                        phieuLuong.ThangTruoc = thangTruoc.Month.ToString("D2");
                        phieuLuong.ThangTruocTiengAnh = ConvertThangSangTiengAnh(thangTruoc.Month, false);
                        phieuLuong.NamTheoThangTruoc = thangTruoc.Year.ToString();

                        phieuLuong.CauHinhTroCapCc = troCapCcncCauHinh?.MucTroCap ?? 0;
                        phieuLuong.CauHinhTroCapDmvs = troCapDmvsCauHinh?.MucTroCap ?? 0;

                        phieuLuong.CauHinhGiamTruCaNhan = cauHinhGiamTruBanThan?.MucGiamTru ?? 0;
                        phieuLuong.CauHinhGiamTruNguoiPhuThuoc = cauHinhGiamTruNguoiPhuThuoc?.MucGiamTru ?? 0;
                        phieuLuong.PhanTramKinhPhiCongDoanCty = cauHinhKinhPhiCongDoan?.MucDongCongTy ?? 0;
                        phieuLuong.PhanTramBaoHiemNld = (cauHinhBaoHiem?.TiLePhanBoMucDongBhxhcuaNld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhytcuaNld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhtncuaNld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhtnnncuaNld) ?? 0;

                        phieuLuong.PhanTramBaoHiemCty = (cauHinhBaoHiem?.TiLePhanBoMucDongBhxhcuaNsdld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhytcuaNsdld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhtncuaNsdld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhtnnncuaNsdld) ?? 0;

                        phieuLuong.LuongCoBan = item.MucLuongCu;
                        phieuLuong.LuongCoBanSau = item.MucLuongHienTai;
                        phieuLuong.MucDieuChinh = phieuLuong.LuongCoBanSau - phieuLuong.LuongCoBan;
                        phieuLuong.NgayLamViecThucTe = tongHopChamCong.NgayLamViecThucTe;
                        phieuLuong.NgayNghiPhep = tongHopChamCong.NghiPhep;
                        phieuLuong.NgayNghiLe = tongHopChamCong.TongSoNgayTinhLuong - tongHopChamCong.NghiPhep;
                        phieuLuong.NgayNghiKhongLuong = tongHopChamCong.NghiKhongLuong;
                        phieuLuong.NgayDmvs = tongHopChamCong.TongNgayDmvs;
                        phieuLuong.NgayKhongHuongChuyenCan = tongHopChamCong.TongNgayNghiTinhTroCapChuyenCan;

                        var listDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapDiLai);
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapDienThoai);
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapAnTrua);
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapCcnc);
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapDmvs);

                        phieuLuong.DuocHuongTroCapKpi = CheckDuocHuongTroCap(dieuKienHuongDatKpi.CategoryId, listDieuKienTroCapCoDinh);
                        phieuLuong.DuocHuongTroCapChuyenCan = CheckDuocHuongTroCap(dieuKienHuongDuocHuongTccc.CategoryId, listDieuKienTroCapCoDinh);

                        phieuLuong.SoLuongDkGiamTruGiaCanh = luongCtGiamTruTruocThue.SoNguoiPhuThuoc;
                        phieuLuong.GiamTruGiaCanh = luongCtGiamTruTruocThue.GiamTruCaNhan + luongCtGiamTruTruocThue.GiamTruNguoiPhuThuoc;
                        phieuLuong.LuongTheoNgayHocViec = tongHopChamCong.TongSoNgayTinhLuong;
                        phieuLuong.TroCapDiLai = luongCtTroCapDiLai?.MucTroCap ?? 0;
                        phieuLuong.TroCapDienThoai = luongCtTroCapDienThoai?.MucTroCap ?? 0;
                        phieuLuong.TroCapAnTrua = luongCtTroCapAnTrua?.MucTroCap ?? 0;
                        phieuLuong.TroCapNhaO = luongCtTroCapCcnc?.MucTroCap ?? 0;
                        phieuLuong.TroCapChuyenCan = luongCtTroCapDmvs?.MucTroCap ?? 0;
                        phieuLuong.ThuongKpi = luongCtTroCapKpi?.MucTroCap ?? 0;
                        phieuLuong.ThuongCuoiNam = 0;
                        phieuLuong.TroCapTrachNhiem = luongCtTroCapTrachNhiem?.MucTroCap ?? 0;
                        phieuLuong.TroCapHocViec = 0;
                        phieuLuong.OtTinhThue = item.LuongOtTinhThue;
                        phieuLuong.OtKhongTinhThue = item.LuongOtKhongTinhThue;
                        phieuLuong.LuongThang13 = luongCtThuNhapTinhThue?.Month13 ?? 0;
                        phieuLuong.QuaBocTham = luongCtThuNhapTinhThue?.Gift ?? 0;
                        phieuLuong.TongThueTncn = item.ThueTncnNld;
                        phieuLuong.BaoHiem = item.TongTienBhNldPhaiDong;
                        phieuLuong.ThucNhan = item.ThuNhapThucNhan;
                        phieuLuong.CtyTraBh = item.TongTienBhCtyPhaiDong;
                        phieuLuong.KinhPhiCongDoan = luongCtCtyDong.KinhPhiCongDoan;
                        phieuLuong.TongChiPhiNhanVien = item.TongChiPhiCtyPhaiTra;

                        //Tự ý nghỉ không hưởng lương: tongHopChamCong.NghiKhongPhep;
                        //Tổng số ngày không tính lương: tongHopChamCong.TongSoNgayKhongTinhLuong
                        //Ngày làm việc thực tế: phieuLuong.NgayLamViecThucTe
                        //Trợ cấp chuyên cần ngày công theo ngày làm việc thực tế: troCapCcncCauHinh.MucTroCap / kyLuong.SoNgayLamViec * tongHopChamCong.NgayLamViecThucTe
                        //Trợ cấp chuyên cần ngày công được hưởng: phieuLuong.TroCapNhaO
                        //Tổng số lần đi muộn về sớm: tongHopChamCong.SoLanTruChuyenCan
                        //Tổng ngày làm việc trong tháng: phieuLuong.SoNgayLamViec
                        //Trợ cấp chuyên cần đi muộn về sớm: troCapDmvsCauHinh.MucTroCap
                        //Trợ cấp chuyên cần ngày công được hưởng: phieuLuong.TroCapChuyenCan

                        phieuLuong.NghiKhongPhep = tongHopChamCong.NghiKhongPhep;
                        phieuLuong.TongSoNgayKhongTinhLuong = tongHopChamCong.TongSoNgayKhongTinhLuong;
                        phieuLuong.TroCapCcncTheoNgayLvtt =
                            troCapCcncCauHinh.MucTroCap / kyLuong.SoNgayLamViec * tongHopChamCong.NgayLamViecThucTe;
                        phieuLuong.SoLanTruChuyenCan = tongHopChamCong.SoLanTruChuyenCan;
                        phieuLuong.TroCapChuyenCanDmvs = troCapDmvsCauHinh.MucTroCap;

                        listPhieuLuong.Add(phieuLuong);

                        context.PhieuLuong.Add(phieuLuong.ToEntity());
                        context.SaveChanges();
                    });

                    #region Thêm ghi chú vào dòng thời gian

                    Note note = new Note();
                    note.NoteId = Guid.NewGuid();
                    note.ObjectId = Guid.Empty;
                    note.Description = "Đã chuyển đề xuất về hoàn thành";
                    note.Type = "ADD";
                    note.Active = true;
                    note.CreatedById = parameter.UserId;
                    note.CreatedDate = DateTime.Now;
                    note.NoteTitle = "Đã thêm ghi chú";
                    note.ObjectNumber = kyLuong.KyLuongId;
                    note.ObjectType = NoteObjectType.KYLUONG;

                    context.Note.Add(note);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                return new HoanThanhKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

            /* Gửi email phiếu lương đến nhân viên */
            listPhieuLuong.ForEach(item =>
            {
                NotificationHelper.AccessNotification(context, TypeModel.PhieuLuong, "SEND_EMP", item, item,
                    true, null, null, new List<string>());
            });

            return new HoanThanhKyLuongResult()
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = "Lưu thành công"
            };
        }

        public GetListPhieuLuongResult GetListPhieuLuong(GetListPhieuLuongParameter parameter)
        {
            try
            {
                #region Check permision: manager

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new GetListPhieuLuongResult
                    {
                        Status = false,
                        Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }

                var employeeLogin = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId && x.Active == true);
                if (employeeLogin == null)
                {
                    return new GetListPhieuLuongResult
                    {
                        Status = false,
                        Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                #endregion

                var isAccess = context.Organization.FirstOrDefault(x => x.OrganizationId == employeeLogin.OrganizationId)?.IsAccess;

                var listKyLuong = context.KyLuong.Select(y => new KyLuongModel(y)).ToList();
                var listEmployee = context.Employee.Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode,
                    EmployeeName = y.EmployeeName,
                    EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                    SubCode1Value = y.SubCode1Value
                }).ToList();
                var listPosition = context.Position.ToList();
                var listGetSubCode1 = GeneralList.GetSubCode1();

                var listPhieuLuong = context.PhieuLuong.Where(x =>
                        (parameter.ListKyLuongId.Count == 0 || parameter.ListKyLuongId.Contains(x.KyLuongId)) &&
                        (parameter.ListEmployeeId.Count == 0 || parameter.ListEmployeeId.Contains(x.EmployeeId)))
                    .Select(y => new PhieuLuongModel(y)).ToList();

                listPhieuLuong.ForEach(item =>
                {
                    var kyLuong = listKyLuong.FirstOrDefault(x => x.KyLuongId == item.KyLuongId);
                    item.TenKyLuong = kyLuong?.TenKyLuong;

                    var emp = listEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    item.EmployeeCode = emp?.EmployeeCode;
                    item.EmployeeName = emp?.EmployeeName;

                    var position = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                    item.PositionName = position?.PositionName;

                    item.SubCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == emp.SubCode1Value)?.Name;
                });

                //Không có quyền xem dự liệu phòng ban khác => xem mỗi phiếu lương của người đăng nhập
                if(isAccess == true)
                {
                    listPhieuLuong = listPhieuLuong.OrderByDescending(z => z.KyLuongId).ThenBy(z => z.EmployeeCode)
                                    .ToList();
                }
                //Nếu có quyền xem data phòng ban khác => xem all
                else
                {
                    listPhieuLuong = listPhieuLuong.Where(x => x.EmployeeId == employeeLogin.EmployeeId).OrderByDescending(z => z.KyLuongId).ThenBy(z => z.EmployeeCode)
                      .ToList();
                }

                return new GetListPhieuLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListKyLuong = listKyLuong,
                    ListEmployee = listEmployee,
                    ListPhieuLuong = listPhieuLuong
                };
            }
            catch (Exception e)
            {
                return new GetListPhieuLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetPhieuLuongByIdResult GetPhieuLuongById(GetPhieuLuongByIdParameter parameter)
        {
            try
            {
                var phieuLuong = context.PhieuLuong.Where(x => x.PhieuLuongId == parameter.PhieuLuongId)
                    .Select(y => new PhieuLuongModel(y)).FirstOrDefault();

                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == phieuLuong.EmployeeId);
                var empContact = context.Contact.FirstOrDefault(x =>
                    x.ObjectId == emp.EmployeeId && x.ObjectType == ContactObjectType.EMP);

                phieuLuong.EmployeeName = emp.EmployeeName;
                phieuLuong.EmployeeCode = emp.EmployeeCode;
                phieuLuong.WorkEmail = empContact.WorkEmail;

                return new GetPhieuLuongByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    PhieuLuong = phieuLuong
                };
            }
            catch (Exception e)
            {
                return new GetPhieuLuongByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ExportBaoCaoKyLuongResult ExportBaoCaoKyLuong(ExportBaoCaoKyLuongParameter parameter)
        {
            try
            {
                var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                if (kyLuong == null)
                {
                    return new ExportBaoCaoKyLuongResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                    };
                }

                var kyLuongTruoc = context.KyLuong.Where(x => x.TuNgay.Date < kyLuong.TuNgay.Date)
                    .OrderByDescending(z => z.TuNgay).FirstOrDefault();

                var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongTongHopModel(y)).ToList();
                var listLuongCtCyDong = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new TongHopChamCongModel(y)).ToList();
                var listLuongCtOther = context.LuongCtOther.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                var listLuongCtCtyDong = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                var listLuongCtBaoHiem = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                #region Báo cáo 1

                #region Data Header

                var thangKetThucKyLuongTruoc =
                    kyLuongTruoc == null ? "" : ConvertThangSangTiengAnh(kyLuongTruoc.DenNgay.Month, false);
                decimal tongSoNhanVienKyLuongTruoc = kyLuongTruoc == null ? 0 :
                    context.LuongTongHop.Count(x => x.KyLuongId == kyLuongTruoc.KyLuongId);
                var thangKetThucKyLuong = ConvertThangSangTiengAnh(kyLuong.DenNgay.Month, false);
                decimal tongSoNhanVienKyLuong = context.KyLuong.Count(x => x.KyLuongId == kyLuong.KyLuongId);

                var listHeaderBaoBao = new List<string>()
                {
                    "No.",
                    "Payment items",
                    thangKetThucKyLuongTruoc + ". CD actual (" + tongSoNhanVienKyLuongTruoc.ToString() + ")",
                    thangKetThucKyLuong + " Forecast",
                    thangKetThucKyLuong + ". CD actual (" + tongSoNhanVienKyLuong.ToString() + ")",
                    "Vs actual in " + thangKetThucKyLuongTruoc,
                    "Vs forecast",
                    "Note"
                };

                #endregion

                #region Data Báo cáo

                var listDataBaoCao = new List<ExportBaoCaoKyLuongModel>();

                var row1Bc1 = new ExportBaoCaoKyLuongModel();
                row1Bc1.Payment = "Salary";
                row1Bc1.CdActual1 = kyLuongTruoc == null
                    ? 0
                    : context.LuongTongHop.Where(x => x.KyLuongId == kyLuongTruoc.KyLuongId)
                        .Sum(s => s.ThuNhapThucNhan);
                row1Bc1.Forecast = 0;
                row1Bc1.CdActual2 = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Sum(s => s.ThuNhapThucNhan);
                row1Bc1.VsActual = row1Bc1.CdActual2 - row1Bc1.CdActual1;
                row1Bc1.VsForecast = row1Bc1.Forecast - row1Bc1.CdActual2;
                listDataBaoCao.Add(row1Bc1);

                var row2Bc1 = new ExportBaoCaoKyLuongModel();
                row2Bc1.Payment = "Trade Union";
                row2Bc1.CdActual1 = kyLuongTruoc == null
                    ? 0
                    : context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuongTruoc.KyLuongId)
                        .Sum(s => s.KinhPhiCongDoan + s.FundOct);
                row2Bc1.Forecast = 0;
                row2Bc1.CdActual2 = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Sum(s => s.KinhPhiCongDoan + s.FundOct);
                row2Bc1.VsActual = row2Bc1.CdActual2 - row2Bc1.CdActual1;
                row2Bc1.VsForecast = row2Bc1.Forecast - row2Bc1.CdActual2;
                listDataBaoCao.Add(row2Bc1);

                var row3Bc1 = new ExportBaoCaoKyLuongModel();
                row3Bc1.Payment = "Obligation insurance (SI, HI, UI,AI)";
                row3Bc1.CdActual1 = kyLuongTruoc == null
                    ? 0
                    : context.LuongTongHop.Where(x => x.KyLuongId == kyLuongTruoc.KyLuongId)
                        .Sum(s => s.TongTienBhCtyPhaiDong + s.TongTienBhNldPhaiDong);
                row3Bc1.Forecast = 0;
                row3Bc1.CdActual2 = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Sum(s => s.TongTienBhCtyPhaiDong + s.TongTienBhNldPhaiDong);
                row3Bc1.VsActual = row3Bc1.CdActual2 - row3Bc1.CdActual1;
                row3Bc1.VsForecast = row3Bc1.Forecast - row3Bc1.CdActual2;
                listDataBaoCao.Add(row3Bc1);

                var row4Bc1 = new ExportBaoCaoKyLuongModel();
                row4Bc1.Payment = "PIT";
                row4Bc1.CdActual1 = kyLuongTruoc == null
                    ? 0
                    : context.LuongTongHop.Where(x => x.KyLuongId == kyLuongTruoc.KyLuongId)
                        .Sum(s => s.ThueTncnNld);
                row4Bc1.Forecast = 0;
                row4Bc1.CdActual2 = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Sum(s => s.ThueTncnNld);
                row4Bc1.VsActual = row4Bc1.CdActual2 - row4Bc1.CdActual1;
                row4Bc1.VsForecast = row4Bc1.Forecast - row4Bc1.CdActual2;
                listDataBaoCao.Add(row4Bc1);

                var row5Bc1 = new ExportBaoCaoKyLuongModel();
                row5Bc1.Payment = "";
                row5Bc1.CdActual1 = listDataBaoCao.Sum(s => s.CdActual1);
                row5Bc1.Forecast = 0;
                row5Bc1.CdActual2 = listDataBaoCao.Sum(s => s.CdActual2);
                row5Bc1.VsActual = listDataBaoCao.Sum(s => s.VsActual);
                row5Bc1.VsForecast = listDataBaoCao.Sum(s => s.VsForecast);
                listDataBaoCao.Add(row5Bc1);

                #endregion

                #endregion

                #region Báo cáo 2

                var thangNamBatDauKyLuongTiengAnh = ConvertThangSangTiengAnh(kyLuong.TuNgay.Month, true) + " " + kyLuong.TuNgay.Year;

                #region Bảng 1

                var listDataBaoCao2Bang1 = new List<ExportBaoCaoKyLuong2Bang1Model>();

                var listSubCode1 = GeneralList.GetSubCode1().OrderByDescending(z => z.Value).ToList();

                listSubCode1.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.SubCode1Value == item.Value).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang1Model();
                    newRow.Dept = item.Name.ToUpper();
                    newRow.NoOfStaff = listLuongTongHop.Count(x => x.SubCode1Value == item.Value);
                    newRow.BasicGrossSalary = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongThucTe);
                    newRow.Bonus = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue + s.TongTroCapCoDinh + s.TongTroCapKhac);
                    newRow.TradeUnion = listLuongCtCyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.PitTax = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Obligation = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongTienBhCtyPhaiDong + s.TongTienBhNldPhaiDong);
                    newRow.NetPayable = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.TotalCompBefore = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.TotalCompAfter = null;

                    listDataBaoCao2Bang1.Add(newRow);
                });

                #endregion

                #region Bảng 2

                var listDataBaoCao2Bang2 = new List<ExportBaoCaoKyLuong2Bang2Model>();

                var capBacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "CAPBAC");
                var listCapBac = context.Category.Where(x => x.CategoryTypeId == capBacType.CategoryTypeId &&
                                                             (x.CategoryCode == "Junior" ||
                                                              x.CategoryCode == "Middle" ||
                                                              x.CategoryCode == "Senior")).OrderBy(z => z.CategoryCode).ToList();

                listCapBac.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang2Model();
                    newRow.Dept = item.CategoryName;
                    newRow.NoOfStaff = listLuongTongHop.Count(x => x.CapBacId == item.CategoryId);
                    newRow.GrossSalary = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.LuongThucTe);
                    newRow.Bonus = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue + s.TongTroCapCoDinh + s.TongTroCapKhac);
                    newRow.TradeUnion = listLuongCtCyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.PitTax = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Obligation = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.TongTienBhCtyPhaiDong + s.TongTienBhNldPhaiDong);
                    newRow.NetPayroll = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.TotalComp = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);

                    listDataBaoCao2Bang2.Add(newRow);
                });

                #endregion

                #region Bảng 3

                var listDataBaoCao2Bang3 = new List<ExportBaoCaoKyLuong2Bang3Model>();

                listCapBac.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang3Model();
                    newRow.Grade = item.CategoryName;
                    newRow.NumberOfEmployees = listLuongTongHop.Count(x => x.CapBacId == item.CategoryId);
                    newRow.GrossBase = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.LuongThucTe);
                    newRow.AverageGrossBase = 0; //Tính trên excel
                    newRow.TargetAverage = item.CategoryCode == "Junior" ? 8000000 : (item.CategoryCode == "Middle" ? 12500000 : 20000000);
                    newRow.Actual = 0; //Tính trên excel

                    listDataBaoCao2Bang3.Add(newRow);
                });

                #endregion

                #region Bảng 4

                var listDataBaoCao2Bang4 = new List<ExportBaoCaoKyLuong2Bang3Model>();

                listCapBac.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang3Model();
                    newRow.Grade = item.CategoryName;
                    newRow.NumberOfEmployees = listLuongTongHop.Count(x => x.CapBacId == item.CategoryId);
                    newRow.TargetAverage = item.CategoryCode == "Junior" ? 10500000 : (item.CategoryCode == "Middle" ? 15000000 : 22500000);

                    listDataBaoCao2Bang4.Add(newRow);
                });

                #endregion

                #region Bảng 5

                var listDataBaoCao2Bang5 = new List<ExportBaoCaoKyLuong2Bang3Model>();

                listCapBac.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang3Model();
                    newRow.Grade = item.CategoryName;
                    newRow.NumberOfEmployees = listLuongTongHop.Count(x => x.CapBacId == item.CategoryId);
                    newRow.TargetAverage = item.CategoryCode == "Junior" ? 10500000 : (item.CategoryCode == "Middle" ? 15000000 : 22500000);

                    listDataBaoCao2Bang5.Add(newRow);
                });

                #endregion

                #endregion

                #region Báo cáo 3

                #region Bảng 1

                var listDataBaoCao3Bang1 = new List<ExportBaoCaoKyLuong3Bang1Model>();

                int index = 0;
                listSubCode1.ForEach(item =>
                {
                    index++;
                    var listEmpId = listLuongTongHop.Where(x => x.SubCode1Value == item.Value).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong3Bang1Model();
                    newRow.Col1 = index;
                    newRow.Col2 = item.Name.ToUpper();
                    newRow.Col3 = item.Name.ToUpper();
                    newRow.Col4 = listLuongTongHop.Count(x => x.SubCode1Value == item.Value);
                    newRow.Col5 = null;
                    newRow.Col6 = null;
                    newRow.Col7 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NgayLamViecThucTe);
                    newRow.Col8 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.MucLuongCu);
                    newRow.Col9 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiPhep);
                    newRow.Col10 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiLe);
                    newRow.Col11 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CongTac + s.DaoTaoHoiThao + s.NghiCheDo + s.NghiHuongLuongKhac + s.NghiBu);
                    newRow.Col12 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongLuong + s.NghiHuongBhxh);
                    newRow.Col13 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongPhep);
                    newRow.Col14 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongNgayDmvs);
                    newRow.Col15 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.SoLanTruChuyenCan);
                    newRow.Col16 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongSoNgayTinhLuong);
                    newRow.Col17 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhoanBuTruThangTruoc);
                    newRow.Col18 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongThucTe);
                    newRow.Col19 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue);
                    newRow.Col20 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongTroCapCoDinh);
                    newRow.Col21 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongTroCapKhac);
                    newRow.Col22 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TroCapKhacKhongTinhThue);
                    newRow.Col23 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhauTruHoanLaiTruocThue);
                    newRow.Col24 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.ThueTncnNld);
                    newRow.Col25 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Col26 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col27 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col28 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col29 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col30 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col31 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col32 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Other);
                    newRow.Col33 = null;
                    newRow.Col34 = null;
                    newRow.Col35 = null;
                    newRow.Col36 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.Col37 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.CacKhoanGiamTruSauThue);
                    newRow.Col38 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.CacKhoanHoanLaiSauThue);
                    newRow.Col39 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.Col40 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongTamUng);
                    newRow.Col41 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongConLai);
                    newRow.Col42 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col43 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col44 = null;
                    newRow.Col45 = null;

                    listDataBaoCao3Bang1.Add(newRow);
                });

                #endregion

                #region Bảng 2

                var listDataBaoCao3Bang2 = new List<ExportBaoCaoKyLuong3Bang1Model>();

                var listSubCode2 = GeneralList.GetSubCode2().OrderByDescending(z => z.Value).ToList();

                for (int i = 0; i < 9; i++)
                {
                    var newRow = new ExportBaoCaoKyLuong3Bang1Model();
                    newRow.Col1 = i + 1;

                    if (i >= 0 && i <= 4)
                    {
                        newRow.Col2 = "G&A";

                        if (i == 0)
                        {
                            newRow.Col3 = "CM";
                        }
                        else if (i == 1)
                        {
                            newRow.Col3 = "HRD";
                        }
                        else if (i == 2)
                        {
                            newRow.Col3 = "G&A-HR";
                        }
                        else if (i == 3)
                        {
                            newRow.Col3 = "G&A-ACC";
                        }
                        else if (i == 4)
                        {
                            newRow.Col3 = "G&A-AD";
                        }
                    }
                    else if (i >= 5 && i <= 6)
                    {
                        newRow.Col2 = "OPS";

                        if (i == 5)
                        {
                            newRow.Col3 = "OPS-PM";
                        }
                        else
                        {
                            newRow.Col3 = "OPS-IT";
                        }
                    }
                    else
                    {
                        newRow.Col2 = "COS";

                        if (i == 7)
                        {
                            newRow.Col3 = "COS-3D";
                        }
                        else
                        {
                            newRow.Col3 = "COS-QA";
                        }
                    }

                    var subCode2 = listSubCode2.FirstOrDefault(x => x.Name == newRow.Col3);
                    var listEmpId = listLuongTongHop.Where(x => x.SubCode2Value == subCode2.Value).Select(y => y.EmployeeId)
                        .ToList();

                    newRow.Col4 = listLuongTongHop.Count(x => x.SubCode2Value == subCode2.Value);
                    newRow.Col5 = null;
                    newRow.Col6 = null;
                    newRow.Col7 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NgayLamViecThucTe);
                    newRow.Col8 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.MucLuongCu);
                    newRow.Col9 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiPhep);
                    newRow.Col10 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiLe);
                    newRow.Col11 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CongTac + s.DaoTaoHoiThao + s.NghiCheDo + s.NghiHuongLuongKhac + s.NghiBu);
                    newRow.Col12 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongLuong + s.NghiHuongBhxh);
                    newRow.Col13 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongPhep);
                    newRow.Col14 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongNgayDmvs);
                    newRow.Col15 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.SoLanTruChuyenCan);
                    newRow.Col16 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongSoNgayTinhLuong);
                    newRow.Col17 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhoanBuTruThangTruoc);
                    newRow.Col18 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.LuongThucTe);
                    newRow.Col19 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue);
                    newRow.Col20 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongTroCapCoDinh);
                    newRow.Col21 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongTroCapKhac);
                    newRow.Col22 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TroCapKhacKhongTinhThue);
                    newRow.Col23 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhauTruHoanLaiTruocThue);
                    newRow.Col24 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.ThueTncnNld);
                    newRow.Col25 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Col26 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col27 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col28 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col29 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col30 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col31 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col32 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Other);
                    newRow.Col33 = null;
                    newRow.Col34 = null;
                    newRow.Col35 = null;
                    newRow.Col36 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.Col37 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.CacKhoanGiamTruSauThue);
                    newRow.Col38 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.CacKhoanHoanLaiSauThue);
                    newRow.Col39 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.Col40 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.LuongTamUng);
                    newRow.Col41 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.LuongConLai);
                    newRow.Col42 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col43 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col44 = null;
                    newRow.Col45 = null;

                    listDataBaoCao3Bang2.Add(newRow);
                }

                #endregion

                #region Bảng 3

                var listDataBaoCao3Bang3 = new List<ExportBaoCaoKyLuong3Bang1Model>();

                for (int i = 0; i < 3; i++)
                {
                    var newRow = new ExportBaoCaoKyLuong3Bang1Model();
                    newRow.Col1 = i + 1;
                    newRow.Col2 = "COS-3D";

                    if (i == 0)
                    {
                        newRow.Col3 = "Middle";
                    }
                    else if (i == 1)
                    {
                        newRow.Col3 = "Junior";
                    }
                    else if (i == 2)
                    {
                        newRow.Col3 = "Senior";
                    }

                    var item = listCapBac.FirstOrDefault(x => x.CategoryCode == newRow.Col3);
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    newRow.Col4 = listLuongTongHop.Count(x => listEmpId.Contains(x.EmployeeId));
                    newRow.Col5 = null;
                    newRow.Col6 = null;
                    newRow.Col7 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NgayLamViecThucTe);
                    newRow.Col8 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.MucLuongCu);
                    newRow.Col9 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiPhep);
                    newRow.Col10 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiLe);
                    newRow.Col11 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CongTac + s.DaoTaoHoiThao + s.NghiCheDo + s.NghiHuongLuongKhac + s.NghiBu);
                    newRow.Col12 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongLuong + s.NghiHuongBhxh);
                    newRow.Col13 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongPhep);
                    newRow.Col14 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongNgayDmvs);
                    newRow.Col15 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.SoLanTruChuyenCan);
                    newRow.Col16 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongSoNgayTinhLuong);
                    newRow.Col17 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhoanBuTruThangTruoc);
                    newRow.Col18 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.LuongThucTe);
                    newRow.Col19 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue);
                    newRow.Col20 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongTroCapCoDinh);
                    newRow.Col21 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongTroCapKhac);
                    newRow.Col22 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TroCapKhacKhongTinhThue);
                    newRow.Col23 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhauTruHoanLaiTruocThue);
                    newRow.Col24 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.ThueTncnNld);
                    newRow.Col25 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Col26 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col27 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col28 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col29 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col30 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col31 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col32 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Other);
                    newRow.Col33 = null;
                    newRow.Col34 = null;
                    newRow.Col35 = null;
                    newRow.Col36 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.Col37 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CacKhoanGiamTruSauThue);
                    newRow.Col38 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CacKhoanHoanLaiSauThue);
                    newRow.Col39 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.Col40 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.LuongTamUng);
                    newRow.Col41 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.LuongConLai);
                    newRow.Col42 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col43 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col44 = null;
                    newRow.Col45 = null;

                    listDataBaoCao3Bang3.Add(newRow);
                }

                #endregion

                #endregion

                return new ExportBaoCaoKyLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListHeaderBaoBao = listHeaderBaoBao,
                    ListDataBaoCao = listDataBaoCao,
                    ThangNamBatDauKyLuongTiengAnh = thangNamBatDauKyLuongTiengAnh,
                    ListDataBaoCao2Bang1 = listDataBaoCao2Bang1,
                    ListDataBaoCao2Bang2 = listDataBaoCao2Bang2,
                    ListDataBaoCao2Bang3 = listDataBaoCao2Bang3,
                    ListDataBaoCao2Bang4 = listDataBaoCao2Bang4,
                    ListDataBaoCao2Bang5 = listDataBaoCao2Bang5,
                    ListDataBaoCao3Bang1 = listDataBaoCao3Bang1,
                    ListDataBaoCao3Bang2 = listDataBaoCao3Bang2,
                    ListDataBaoCao3Bang3 = listDataBaoCao3Bang3
                };
            }
            catch (Exception e)
            {
                return new ExportBaoCaoKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CapNhatBangLuongResult CapNhatBangLuong(CapNhatBangLuongParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new CapNhatBangLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    int kyLuongId = kyLuong.KyLuongId;

                    SaveTongHopChamCong(kyLuongId, kyLuong.TuNgay.Date, kyLuong.DenNgay.Date);

                    SaveLuongCt_TroCap(kyLuongId, kyLuong.SoNgayLamViec, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_TroCapOt(kyLuongId, kyLuong.SoNgayLamViec, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_ThuNhapTinhThue(kyLuongId, kyLuong.TuNgay.Date, kyLuong.DenNgay.Date);

                    SaveLuongCt_BaoHiem(kyLuongId, kyLuong.TuNgay.Date, kyLuong.DenNgay.Date);

                    SaveLuongCt_GiamTruTruocThue(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_GiamTruSauThue(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_HoanLaiSauThue(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_CtyDong(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_Other(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongTongHop(kyLuongId, kyLuong.SoNgayLamViec, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    context.SaveChanges();

                    trans.Commit();

                    return new CapNhatBangLuongResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Cập nhật thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new CapNhatBangLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateKinhPhiResult CreateOrUpdateKinhPhi(CreateOrUpdateKinhPhiParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    if (parameter.KinhPhiCongDoan.MucDongNhanVien > 100 ||
                        parameter.KinhPhiCongDoan.MucDongCongTy > 100)
                    {
                        return new CreateOrUpdateKinhPhiResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Số phần trăm không được vượt quá 100%"
                        };
                    }

                    //Create
                    if (parameter.KinhPhiCongDoan.KinhPhiCongDoanId == null || parameter.KinhPhiCongDoan.KinhPhiCongDoanId == 0)
                    {
                        var kinhPhiCongDoan = parameter.KinhPhiCongDoan.ToEntity();
                        context.KinhPhiCongDoan.Add(kinhPhiCongDoan);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var kinhPhiCongDoan = context.KinhPhiCongDoan.
                                FirstOrDefault(x => x.KinhPhiCongDoanId == parameter.KinhPhiCongDoan.KinhPhiCongDoanId);

                        if (kinhPhiCongDoan == null)
                        {
                            return new CreateOrUpdateKinhPhiResult
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        kinhPhiCongDoan.MucDongNhanVien = parameter.KinhPhiCongDoan.MucDongNhanVien;
                        kinhPhiCongDoan.MucDongCongTy = parameter.KinhPhiCongDoan.MucDongCongTy;
                        context.KinhPhiCongDoan.Update(kinhPhiCongDoan);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateKinhPhiResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateKinhPhiResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhCaLamViecResult DeleteCauHinhCaLamViec(DeleteCauHinhCaLamViecParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var objDelete = context.CaLamViec.FirstOrDefault(x =>
                        x.CaLamViecId == parameter.CaLamViecId);
                    if (objDelete == null)
                    {
                        return new DeleteCauHinhCaLamViecResult()
                        {
                            StatusCode = HttpStatusCode.FailedDependency,
                            MessageCode = "Dữ liệu không có trong hệ thống, xóa thất bại.",
                        };
                    }

                    context.CaLamViec.Remove(objDelete);
                    context.SaveChanges();

                    var listCaLamViecChiTiet = context.CaLamViecChiTiet
                        .Where(x => x.CaLamViecId == objDelete.CaLamViecId).ToList();
                    context.CaLamViecChiTiet.RemoveRange(listCaLamViecChiTiet);
                    context.SaveChanges();

                    trans.Commit();

                    return new DeleteCauHinhCaLamViecResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công",
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteCauHinhCaLamViecResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhGiamTruResult DeleteCauHinhGiamTru(DeleteCauHinhGiamTruParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var objDelete = context.CauHinhGiamTru.FirstOrDefault(x =>
                        x.CauHinhGiamTruId == parameter.CauHinhGiamTruId);
                    if (objDelete == null)
                    {
                        return new DeleteCauHinhGiamTruResult()
                        {
                            StatusCode = HttpStatusCode.FailedDependency,
                            MessageCode = "Dữ liệu không có trong hệ thống, xóa thất bại.",
                        };
                    }
                    context.CauHinhGiamTru.Remove(objDelete);
                    context.SaveChanges();
                    trans.Commit();

                    return new DeleteCauHinhGiamTruResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công",
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteCauHinhGiamTruResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhNghiLeResult DeleteCauHinhNghiLe(DeleteCauHinhNghiLeParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var objDelete = context.CauHinhNghiLe.FirstOrDefault(x =>
                        x.NghiLeId == parameter.NghiLeId);

                    if (objDelete == null)
                    {
                        return new DeleteCauHinhNghiLeResult()
                        {
                            StatusCode = HttpStatusCode.FailedDependency,
                            MessageCode = "Dữ liệu không có trong hệ thống, xóa thất bại.",
                        };
                    }

                    var listOld = context.CauHinhNghiLeChiTiet.Where(x => x.NghiLeId == parameter.NghiLeId).ToList();
                    context.CauHinhNghiLeChiTiet.RemoveRange(listOld);
                    context.CauHinhNghiLe.Remove(objDelete);
                    context.SaveChanges();
                    trans.Commit();

                    return new DeleteCauHinhNghiLeResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công",
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteCauHinhNghiLeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhOtResult DeleteCauHinhOt(DeleteCauHinhOtParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var objDelete = context.CauHinhOt.FirstOrDefault(x => x.CauHinhOtId == parameter.CauHinhOtId);
                    if (objDelete == null)
                    {
                        return new DeleteCauHinhOtResult()
                        {
                            StatusCode = HttpStatusCode.FailedDependency,
                            MessageCode = "Dữ liệu không có trong hệ thống, xóa thất bại.",
                        };
                    }

                    context.CauHinhOt.Remove(objDelete);
                    context.SaveChanges();
                    trans.Commit();

                    return new DeleteCauHinhOtResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công",
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteCauHinhOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterCauHinhLuongResult GetMasterCauHinhLuong(GetMasterCauHinhLuongParameter parameter)
        {
            try
            {
                #region Ca làm việc

                var listLoaiCaLamViec = GeneralList.GetTrangThais("LoaiCaLamViec");
                var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");
                var listCaLamViec = context.CaLamViec.Select(y => new CaLamViecModel(y)).ToList();
                var listCaLamViecChiTiet = context.CaLamViecChiTiet.ToList();

                listCaLamViec.ForEach(item =>
                {
                    var loaiCa = listLoaiCaLamViec.FirstOrDefault(x => x.Value == item.LoaiCaLamViecId);
                    item.TenLoaiCaLamViec = loaiCa?.Name;

                    item.ListCaLamViecChiTiet = listCaLamViecChiTiet
                        .Where(x => x.CaLamViecId == item.CaLamViecId)
                        .Select(y => new CaLamViecChiTietModel(y))
                        .OrderBy(z => z.NgayTrongTuan)
                        .ToList();

                    for (int i = 0; i < item.ListCaLamViecChiTiet.Count; i++)
                    {
                        var _item = item.ListCaLamViecChiTiet[i];

                        if (i == 0)
                        {
                            item.NgayLamViecTrongTuanText = _item.NgayTrongTuanText;
                        }
                        else
                        {
                            item.NgayLamViecTrongTuanText += ", " + _item.NgayTrongTuanText;
                        }
                    }
                });

                #endregion

                #region Nghỉ lễ

                var listLoaiNghiLe = GeneralList.GetTrangThais("LoaiNghiLe");
                var listCauHinhNghiLe = context.CauHinhNghiLe.Select(y => new CauHinhNghiLeModel(y))
                    .OrderByDescending(z => z.SoNam).ToList();
                var listCauHinhNghiLeChiTiet = context.CauHinhNghiLeChiTiet.ToList();

                listCauHinhNghiLe.ForEach(item =>
                {
                    var listNgay = listCauHinhNghiLeChiTiet.Where(x => x.NghiLeId == item.NghiLeId)
                        .Select(y => new CauHinhNghiLeChiTietModel(y)).ToList();
                    item.ListCauHinhNghiLeChiTiet = listNgay;

                    #region Ngày nghỉ lễ

                    var listNgayNghiLe = listNgay.Where(x => x.LoaiNghiLe == 1).OrderBy(z => z.Ngay).ToList();

                    for (int i = 0; i < listNgayNghiLe.Count; i++)
                    {
                        var _item = listNgayNghiLe[i];

                        if (i == 0)
                        {
                            item.NgayNghiLe = _item.Ngay.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            item.NgayNghiLe += ", " + _item.Ngay.ToString("dd/MM/yyyy");
                        }
                    }

                    #endregion

                    #region Ngày nghỉ bù

                    var listNgayNghiBu = listNgay.Where(x => x.LoaiNghiLe == 2).OrderBy(z => z.Ngay).ToList();

                    for (int i = 0; i < listNgayNghiBu.Count; i++)
                    {
                        var _item = listNgayNghiBu[i];

                        if (i == 0)
                        {
                            item.NgayNghiBu = _item.Ngay.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            item.NgayNghiBu += ", " + _item.Ngay.ToString("dd/MM/yyyy");
                        }
                    }

                    #endregion

                    #region Ngày làm bù

                    var listNgayLamBu = listNgay.Where(x => x.LoaiNghiLe == 3).OrderBy(z => z.Ngay).ToList();

                    for (int i = 0; i < listNgayLamBu.Count; i++)
                    {
                        var _item = listNgayLamBu[i];

                        if (i == 0)
                        {
                            item.NgayLamBu = _item.Ngay.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            item.NgayLamBu += ", " + _item.Ngay.ToString("dd/MM/yyyy");
                        }
                    }

                    #endregion
                });

                #endregion

                #region Cấu hình OT

                var loaiOtType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT");
                var listLoaiOt = new List<CategoryEntityModel>();
                if (loaiOtType != null)
                {
                    listLoaiOt = context.Category
                        .Where(x => x.CategoryTypeId == loaiOtType.CategoryTypeId)
                        .Select(y => new CategoryEntityModel(y))
                        .ToList();
                }

                var listCauHinhOt = context.CauHinhOt.Select(y => new CauHinhOtModel(y))
                    .OrderByDescending(z => z.CreatedDate).ToList();

                var stt_ot = 0;
                listCauHinhOt.ForEach(item =>
                {
                    stt_ot++;
                    item.Stt = stt_ot;

                    var loaiOt = listLoaiOt.FirstOrDefault(x => x.CategoryId == item.LoaiOtId);
                    item.TenLoaiOt = loaiOt?.CategoryName;
                });

                #endregion

                #region Cấu hình giảm trừ

                var listLoaiGiamTru = GeneralList.GetTrangThais("LoaiGiamTru");
                var listCauHinhGiamTru = context.CauHinhGiamTru.Select(y => new CauHinhGiamTruModel(y))
                    .OrderByDescending(z => z.CreatedDate).ToList();
                var stt_giamtru = 0;
                listCauHinhGiamTru.ForEach(item =>
                {
                    stt_giamtru++;
                    item.Stt = stt_giamtru;
                });

                #endregion

                #region Cấu hình kinh phí công đoàn

                var kinhPhiCongDoan = context.KinhPhiCongDoan
                        .Select(y => new KinhPhiCongDoanModel(y)).FirstOrDefault();

                #endregion

                #region Công thức tính lương

                var listTokenTinhLuong = GeneralList.GetTrangThais("TokenTinhLuong");
                var congThucTinhLuong = context.CongThucTinhLuong
                    .Select(y => new CongThucTinhLuongModel(y)).FirstOrDefault();

                #endregion

                #region Cấu hình chấm công OT

                var cauHinhChamCongOt = context.CauHinhChamCongOt
                    .Select(y => new CauHinhChamCongOtModel(y)).FirstOrDefault();

                #endregion

                #region Cấu hình OT cả ngày

                var cauHinhOtCaNgay = context.CauHinhOtCaNgay
                    .Select(y => new CauHinhOtCaNgayModel(y)).FirstOrDefault();

                #endregion

                #region Cấu hình thuế TNCN

                var listCauHinhThueTncn = context.CauHinhThueTncn
                    .Select(y => new CauHinhThueTncnModel(y))
                    .OrderBy(z => z.SoTienTu).ToList();

                #endregion

                return new GetMasterCauHinhLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListCaLamViec = listCaLamViec,
                    ListLoaiCaLamViec = listLoaiCaLamViec,
                    ListNgayLamViecTrongTuan = listNgayLamViecTrongTuan,
                    ListLoaiNghiLe = listLoaiNghiLe,
                    ListCauHinhNghiLe = listCauHinhNghiLe,
                    ListLoaiOt = listLoaiOt,
                    ListCauHinhOt = listCauHinhOt,
                    ListLoaiGiamTru = listLoaiGiamTru,
                    ListCauHinhGiamTru = listCauHinhGiamTru,
                    KinhPhiCongDoan = kinhPhiCongDoan,
                    ListTokenTinhLuong = listTokenTinhLuong,
                    CongThucTinhLuong = congThucTinhLuong,
                    CauHinhChamCongOt = cauHinhChamCongOt,
                    CauHinhOtCaNgay = cauHinhOtCaNgay,
                    ListCauHinhThueTncn = listCauHinhThueTncn
                };
            }
            catch (Exception e)
            {
                return new GetMasterCauHinhLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataExportOTResult GetDataExportOT(GetDataExportOTParameter parameter)
        {
            try
            {
                var listData = new List<List<DataRowModel>>();
                var listDataHeader = new List<List<DataHeaderModel>>();
                switch (parameter.BaoCaoNumber)
                {
                    case 3:
                        LayBaoCao3(out listData, out listDataHeader, parameter.KyLuongId);
                        break;
                    case 4:
                        LayBaoCao4(out listData, out listDataHeader, parameter.KyLuongId);
                        break;
                    case 5:
                        
                        break;
                    case 521:
                        LayBaoCao521(out listData, out listDataHeader, parameter.KyLuongId);
                        break;
                    case 9:
                        LayBaoCaoOT9(out listData, out listDataHeader, parameter.KyLuongId);
                        break;
                    case 91:
                        LayBaoCaoOT91(out listData, out listDataHeader, parameter.KyLuongId);
                        break;
                    case 10:
                        LayBaoCao10(out listData, out listDataHeader, parameter.KyLuongId);
                        break;
                }

                return new GetDataExportOTResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListData = listData,
                    ListDataHeader = listDataHeader,
                };
            }
            catch (Exception e)
            {
                return new GetDataExportOTResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SendMailDuLieuChamCongBatThuongResult SendMailDuLieuChamCongBatThuong(
            SendMailDuLieuChamCongBatThuongParameter parameter)
        {
            var listChamCongBatThuong = parameter.ListChamCongBatThuong.OrderBy(z => z.EmployeeId)
                .ThenBy(z => z.CaChamCong).ToList();
            var listEmpId = listChamCongBatThuong.Select(y => y.EmployeeId).Distinct().ToList();

            var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).ToList();
            var listContact = context.Contact
                .Where(x => x.ObjectType == ContactObjectType.EMP && listEmpId.Contains(x.ObjectId)).ToList();

            MailModel mailModel = new MailModel();

            #region Lấy thông tin cấu hình

            var emailNhanSu = context.EmailNhanSu.FirstOrDefault();

            mailModel.UsingDefaultReceiverEmail = false;

            mailModel.SmtpEmailAccount = emailNhanSu?.Email;

            mailModel.SmtpPassword = emailNhanSu?.Password;

            mailModel.SmtpServer =
                context.SystemParameter.FirstOrDefault(w => w.SystemKey == "PrimaryDomain")?.SystemValueString;

            mailModel.SmtpPort =
                int.Parse(context.SystemParameter.FirstOrDefault(w => w.SystemKey == "PrimaryPort")?.SystemValueString);

            mailModel.SmtpSsl =
                context.SystemParameter.FirstOrDefault(x => x.SystemKey == "Ssl").SystemValue.Value;

            var screenId = context.Screen.FirstOrDefault(x => x.ScreenCode == "CHAM_CONG")?.ScreenId;
            var NotifiActionId = context.NotifiAction.FirstOrDefault(x => x.NotifiActionCode == "SEND_EMP" && x.ScreenId == screenId)
                ?.NotifiActionId;

            var notifiSetting =
                context.NotifiSetting.FirstOrDefault(x => x.ScreenId == screenId &&
                                                          x.NotifiActionId == NotifiActionId && x.Active);

            #endregion

            if (notifiSetting != null)
            {
                listChamCongBatThuong.ForEach(item =>
                {
                    var emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var contact = listContact.FirstOrDefault(x =>
                        x.ObjectType == ContactObjectType.EMP && x.ObjectId == item.EmployeeId);

                    item.EmployeeCode = emp?.EmployeeCode;
                    item.EmployeeName = emp?.EmployeeName;
                    item.Email = contact?.WorkEmail;
                });

                listEmp.ForEach(item =>
                {
                    var listDuLieu = listChamCongBatThuong.Where(x => x.EmployeeId == item.EmployeeId)
                        .OrderBy(z => z.CaChamCong).ToList();

                    if (listDuLieu.Count > 0)
                    {
                        var emp = listDuLieu.First();

                        var message = notifiSetting.EmailContent;

                        if (message.Contains("[EmployeeCode]"))
                        {
                            if (!string.IsNullOrWhiteSpace(item.EmployeeCode))
                            {
                                message = message.Replace("[EmployeeCode]", item.EmployeeCode);
                            }
                        }

                        if (message.Contains("[EmployeeName]"))
                        {
                            if (!string.IsNullOrWhiteSpace(item.EmployeeName))
                            {
                                message = message.Replace("[EmployeeName]", item.EmployeeName);
                            }
                        }

                        if (message.Contains("[Danh_Sach_Thong_Ke]"))
                        {
                            string danhSachThongKe = "";

                            listDuLieu.ForEach(data =>
                            {
                                string temp = "<p>- Ngày [NgayChamCong]: [CaChamCong] giờ vào <strong>\"[Gio]\"</strong></p>";

                                string NgayChamCong =
                                    data.NgayChamCong.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                string CaChamCong = data.CaChamCong == 1
                                    ? "Ca sáng"
                                    : (data.CaChamCong == 2 ? "Ca chiều" : (data.CaChamCong == 3 ? "Ca tối" : ""));

                                var Gio = data.CaChamCong == 1
                                    ? data.VaoSang.Value.ToString(@"hh\:mm")
                                    : (data.CaChamCong == 2
                                        ? data.VaoChieu.Value.ToString(@"hh\:mm")
                                        : (data.CaChamCong == 3 ? data.VaoToi.Value.ToString(@"hh\:mm") : ""));

                                if (!string.IsNullOrEmpty(NgayChamCong))
                                {
                                    temp = temp.Replace("[NgayChamCong]", NgayChamCong);
                                }

                                if (!string.IsNullOrEmpty(CaChamCong))
                                {
                                    temp = temp.Replace("[CaChamCong]", CaChamCong);
                                }

                                if (!string.IsNullOrEmpty(Gio))
                                {
                                    temp = temp.Replace("[Gio]", Gio);
                                }

                                danhSachThongKe += temp;
                            });

                            message = message.Replace("[Danh_Sach_Thong_Ke]", danhSachThongKe);
                        }

                        SendEmail(mailModel, new List<string>() { emp.Email }, notifiSetting.EmailTitle, message);
                    }
                });
            }

            return new SendMailDuLieuChamCongBatThuongResult()
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = "OK"
            };
        }

        public GetDataBaoCaoAllowancesResult GetDataBaoCaoAllowances(GetDataBaoCaoAllowancesParameter parameter)
        {
            try
            {
                if (parameter.KyLuongId == 0 || parameter.KyLuongId == null)
                {
                    return new GetDataBaoCaoAllowancesResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Kỳ lương không tồn tại trên hệ thống",
                    };
                }

                var listHeader2 = new List<string>();
                var listData = new List<List<DataRowModel>>();

                var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                var listEmpId = listLuongTongHop.Select(y => y.EmployeeId).ToList();
                var listEmp = context.Employee
                    .Join(listEmpId,
                        emp => emp.EmployeeId,
                        empId => empId,
                        (emp, empId) => new { Emp = emp, EmpId = empId })
                    .ToList();
                var listContact = context.Contact.Join(context.Employee,
                        cont => cont.ObjectId,
                        emp => emp.EmployeeId,
                        (cont, emp) => new { Cont = cont, Emp = emp })
                    .Where(x => x.Cont.ObjectType == ContactObjectType.EMP)
                    .ToList();
                var listHopDongNhanSu = context.HopDongNhanSu.ToList();
                var listSubCode1 = GeneralList.GetSubCode1();
                var listPosition = context.Position.ToList();
                var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS").CategoryTypeId;
                var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType).ToList();
                var listLuongCtTroCapCoDinh =
                    context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                var listLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh
                    .Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                var dieuKienHuongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC")
                    .CategoryTypeId;
                var listDieuKienHuong = context.Category.Where(x => x.CategoryTypeId == dieuKienHuongType).ToList();
                var dieuKienHuong_ThuongTtnqld = listDieuKienHuong.FirstOrDefault(x => x.CategoryCode == "DKH001");
                var dieuKienHuong_DatKpi = listDieuKienHuong.FirstOrDefault(x => x.CategoryCode == "DKH002");
                var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == parameter.KyLuongId)
                    .Select(y => new TongHopChamCongModel(y)).ToList();
                var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh
                    .Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                var listPhongBan = context.Organization.ToList();

                var listHeader1 = new List<string>
                {
                    "No.",
                    "HR Filing Code",
                    "Vietnamese name",
                    "Team",
                    "DEPT",
                    "TITLE",
                    "ELIGIBLE FOR LEADER/MENTOR BONUS OR NOT?",
                    "Contract Type",
                    "Thưởng tuân thủ nội quy lao động Theo HĐ",
                    "LEADER/ TEAM KPI OF … " + kyLuong.DenNgay.AddYears(-1).Year,
                    "",
                    "",
                    "",
                    "",
                    "",
                    "LEADER/ TEAM KPI OF " + ConvertThangSangTiengAnh(kyLuong.DenNgay.Month, false).ToUpper() + " " +
                    kyLuong.DenNgay.AddYears(-1).Year,
                    "",
                    "",
                    "",
                    "",
                    "",
                    "Actual working days",
                    "Actual working day with old salary/ Probation salary",
                    "AL/",
                    "Holiday/",
                    "Other paid leave with full paid",
                    "Unpaid Leave with reason",
                    "Leave without approval_Unpaid",
                    "Leave early and come late - Days w/o pay",
                    "Days w/o attenance award",
                    "Total payable day for this month, including Holiday (Mon-Fri)",
                };

                #region Header

                #region Header row1

                /* tr1 */

                //Lấy list trợ cấp cố định đã được cấu hình
                var listTroCapCoDinh = context.TroCap
                    .Join(context.Category,
                        troCap => troCap.LoaiTroCapId,
                        category => category.CategoryId,
                        (troCap, category) => new { TroCap = troCap, Category = category })
                    .Where(x => x.TroCap.TypeId <= 3).OrderBy(z => z.TroCap.TypeId).ToList();

                for (int i = 0; i < listTroCapCoDinh.Count; i++)
                {
                    var item = listTroCapCoDinh[i];
                    listHeader1.Add("");
                }

                listHeader1.Add("");
                listHeader1.Add("MONTHLY KPI BONUS (R5, R4 or Top 10% over 100%)");
                listHeader1.Add("LEADER BONUS");
                listHeader1.Add("GHI CHÚ");

                #endregion

                #region Header row2

                /* tr2 */

                for (int j = 0; j < 31; j++)
                {
                    if (j == 9)
                    {
                        listHeader2.Add("Personal Ranking");
                    }
                    else if (j == 10)
                    {
                        listHeader2.Add("Rank");
                    }
                    else if (j == 11)
                    {
                        listHeader2.Add("Thưởng trợ cấp nếu đạt KPI, bottom 10% no allowance");
                    }
                    else if (j == 12)
                    {
                        listHeader2.Add("Team Ranking");
                    }
                    else if (j == 13)
                    {
                        listHeader2.Add("Rank");
                    }
                    else if (j == 14)
                    {
                        listHeader2.Add("Leader Bonus?");
                    }
                    else if (j == 15)
                    {
                        listHeader2.Add("Final Ranking");
                    }
                    else if (j == 16)
                    {
                        listHeader2.Add("Rank");
                    }
                    else if (j == 17)
                    {
                        listHeader2.Add("Thưởng trợ cấp nếu đạt KPI, bottom 10% no allowance");
                    }
                    else if (j == 18)
                    {
                        listHeader2.Add("Team Ranking");
                    }
                    else if (j == 19)
                    {
                        listHeader2.Add("Rank");
                    }
                    else if (j == 20)
                    {
                        listHeader2.Add("Leader or Mentor Bonus?");
                    }
                    else
                    {
                        listHeader2.Add("");
                    }
                }

                for (int k = 0; k < listTroCapCoDinh.Count; k++)
                {
                    var item = listTroCapCoDinh[k];
                    listHeader2.Add(item.Category.CategoryName);
                }

                listHeader2.Add("TOTAL GROSS ALLOWANCE (Cell phone, Housing allowance, lunch, attendance reward)");
                listHeader2.Add("");
                listHeader2.Add("");
                listHeader2.Add("");

                #endregion

                #endregion

                #region Data

                int stt = 0;

                listEmp.ForEach(emp =>
                {
                    stt++;
                    var troCapCoDinh = listLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == emp.EmpId);
                    var loaiHopDong =
                        listLoaiHopDong.FirstOrDefault(x => x.CategoryId == troCapCoDinh.LoaiHopDongId);
                    var dieuKienHuong_thuongTtnqld = listLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                        x.LuongCtTroCapCoDinhId == troCapCoDinh.LuongCtTroCapCoDinhId &&
                        x.DieuKienHuongId == dieuKienHuong_ThuongTtnqld.CategoryId);
                    var dieuKienHuong_thuongDatKpi = listLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                        x.LuongCtTroCapCoDinhId == troCapCoDinh.LuongCtTroCapCoDinhId &&
                        x.DieuKienHuongId == dieuKienHuong_DatKpi.CategoryId);
                    var tongHopChamCong = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == emp.EmpId);
                    var luongTongHop = listLuongTongHop.FirstOrDefault(x => x.EmployeeId == emp.EmpId);
                    var phongBan = listPhongBan.FirstOrDefault(x => x.OrganizationId == emp.Emp.OrganizationId);

                    int indexColumn = 1;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "index_" + indexColumn;
                    dataEmpIdRow.ColumnValue = stt.ToString();
                    listDataRow.Add(dataEmpIdRow);
                    indexColumn++;

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "index_" + indexColumn;
                    dataRow.ColumnValue = emp.Emp.EmployeeCode;
                    listDataRow.Add(dataRow);
                    indexColumn++;

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "index_" + indexColumn;
                    dataRow2.ColumnValue = emp.Emp.EmployeeName;
                    listDataRow.Add(dataRow2);
                    indexColumn++;

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "index_" + indexColumn;
                    dataRow3.ColumnValue = phongBan.OrganizationName;
                    listDataRow.Add(dataRow3);
                    indexColumn++;

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "index_" + indexColumn;
                    dataRow4.ColumnValue = listSubCode1.FirstOrDefault(x => x.Value == emp.Emp.SubCode1Value)?.Name;
                    listDataRow.Add(dataRow4);
                    indexColumn++;

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "index_" + indexColumn;
                    dataRow5.ColumnValue = listPosition.FirstOrDefault(x => x.PositionId == emp.Emp.PositionId)?.PositionName;
                    listDataRow.Add(dataRow5);
                    indexColumn++;

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "index_" + indexColumn;
                    dataRow6.ColumnValue = "YES";
                    listDataRow.Add(dataRow6);
                    indexColumn++;

                    var dataRow7 = new DataRowModel();
                    dataRow7.ColumnKey = "index_" + indexColumn;
                    dataRow7.ColumnValue = loaiHopDong.CategoryCode.Substring(0, 4); ;
                    listDataRow.Add(dataRow7);
                    indexColumn++;

                    var dataRow8 = new DataRowModel();
                    dataRow8.ColumnKey = "index_" + indexColumn;
                    dataRow8.ColumnValue = dieuKienHuong_thuongTtnqld == null
                        ? "YES"
                        : (dieuKienHuong_thuongTtnqld.DuDieuKien == true ? "YES" : "NO");
                    listDataRow.Add(dataRow8);
                    indexColumn++;

                    //1
                    var dataRow9 = new DataRowModel();
                    dataRow9.ColumnKey = "index_" + indexColumn;
                    dataRow9.ColumnValue = "";
                    listDataRow.Add(dataRow9);
                    indexColumn++;

                    //2
                    var dataRow10 = new DataRowModel();
                    dataRow10.ColumnKey = "index_" + indexColumn;
                    dataRow10.ColumnValue = "";
                    listDataRow.Add(dataRow10);
                    indexColumn++;

                    //3
                    var dataRow11 = new DataRowModel();
                    dataRow11.ColumnKey = "index_" + indexColumn;
                    dataRow11.ColumnValue = "";
                    listDataRow.Add(dataRow11);
                    indexColumn++;

                    //4
                    var dataRow12 = new DataRowModel();
                    dataRow12.ColumnKey = "index_" + indexColumn;
                    dataRow12.ColumnValue = "";
                    listDataRow.Add(dataRow12);
                    indexColumn++;

                    //5
                    var dataRow13 = new DataRowModel();
                    dataRow13.ColumnKey = "index_" + indexColumn;
                    dataRow13.ColumnValue = "";
                    listDataRow.Add(dataRow13);
                    indexColumn++;

                    //6
                    var dataRow14 = new DataRowModel();
                    dataRow14.ColumnKey = "index_" + indexColumn;
                    dataRow14.ColumnValue = "";
                    listDataRow.Add(dataRow14);
                    indexColumn++;

                    //7
                    var dataRow15 = new DataRowModel();
                    dataRow15.ColumnKey = "index_" + indexColumn;
                    dataRow15.ColumnValue = "";
                    listDataRow.Add(dataRow15);
                    indexColumn++;

                    //8
                    var dataRow16 = new DataRowModel();
                    dataRow16.ColumnKey = "index_" + indexColumn;
                    dataRow16.ColumnValue = "";
                    listDataRow.Add(dataRow16);
                    indexColumn++;

                    //9
                    var dataRow17 = new DataRowModel();
                    dataRow17.ColumnKey = "index_" + indexColumn;
                    dataRow17.ColumnValue = dieuKienHuong_thuongDatKpi == null
                        ? "Có"
                        : (dieuKienHuong_thuongDatKpi.DuDieuKien == true ? "Có" : "Không"); ;
                    listDataRow.Add(dataRow17);
                    indexColumn++;

                    //10
                    var dataRow18 = new DataRowModel();
                    dataRow18.ColumnKey = "index_" + indexColumn;
                    dataRow18.ColumnValue = "";
                    listDataRow.Add(dataRow18);
                    indexColumn++;

                    //11
                    var dataRow19 = new DataRowModel();
                    dataRow19.ColumnKey = "index_" + indexColumn;
                    dataRow19.ColumnValue = "";
                    listDataRow.Add(dataRow19);
                    indexColumn++;

                    //12
                    var dataRow20 = new DataRowModel();
                    dataRow20.ColumnKey = "index_" + indexColumn;
                    dataRow20.ColumnValue = "";
                    listDataRow.Add(dataRow20);
                    indexColumn++;

                    var dataRow21 = new DataRowModel();
                    dataRow21.ColumnKey = "index_" + indexColumn;
                    dataRow21.ColumnValue = tongHopChamCong.NgayLamViecThucTe.ToString();
                    dataRow21.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow21);
                    indexColumn++;

                    var dataRow22 = new DataRowModel();
                    dataRow22.ColumnKey = "index_" + indexColumn;
                    dataRow22.ColumnValue = luongTongHop.SoNgayMucLuongCu.ToString();
                    dataRow22.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow22);
                    indexColumn++;

                    var dataRow23 = new DataRowModel();
                    dataRow23.ColumnKey = "index_" + indexColumn;
                    dataRow23.ColumnValue = tongHopChamCong.NghiPhep.ToString();
                    dataRow23.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow23);
                    indexColumn++;

                    var dataRow24 = new DataRowModel();
                    dataRow24.ColumnKey = "index_" + indexColumn;
                    dataRow24.ColumnValue = tongHopChamCong.NghiLe.ToString();
                    dataRow24.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow24);
                    indexColumn++;

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = (tongHopChamCong.CongTac + tongHopChamCong.DaoTaoHoiThao +
                                       tongHopChamCong.NghiCheDo + tongHopChamCong.NghiHuongLuongKhac +
                                       tongHopChamCong.NghiBu).ToString(),
                        ValueType = ValueTypeEnum.NUMBER,
                    });
                    indexColumn++;

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = (tongHopChamCong.NghiHuongBhxh + tongHopChamCong.NghiKhongLuong).ToString(),
                        ValueType = ValueTypeEnum.NUMBER,
                    });
                    indexColumn++;

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = (tongHopChamCong.NghiKhongPhep).ToString(),
                        ValueType = ValueTypeEnum.NUMBER,
                    });
                    indexColumn++;

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = (tongHopChamCong.TongNgayDmvs).ToString(),
                        ValueType = ValueTypeEnum.NUMBER,
                    });
                    indexColumn++;

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = (tongHopChamCong.SoLanTruChuyenCan).ToString(),
                        ValueType = ValueTypeEnum.NUMBER,
                    });
                    indexColumn++;

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = "0",
                        ValueType = ValueTypeEnum.NUMBER,
                    });
                    indexColumn++;

                    for (int p = 0; p < listTroCapCoDinh.Count; p++)
                    {
                        var item = listTroCapCoDinh[p];
                        var luongCtLoaiTroCapCoDinh = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == troCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == item.Category.CategoryId);

                        listDataRow.Add(new DataRowModel
                        {
                            ColumnKey = "index_" + indexColumn,
                            ColumnValue = (luongCtLoaiTroCapCoDinh?.MucTroCap ?? 0).ToString(),
                            ValueType = ValueTypeEnum.NUMBER,
                        });
                        indexColumn++;
                    }

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = "0",
                        ValueType = ValueTypeEnum.NUMBER,
                    });
                    indexColumn++;

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = "0",
                        ValueType = ValueTypeEnum.NUMBER,
                    });
                    indexColumn++;

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = "0",
                        ValueType = ValueTypeEnum.NUMBER,
                    });
                    indexColumn++;

                    listDataRow.Add(new DataRowModel
                    {
                        ColumnKey = "index_" + indexColumn,
                        ColumnValue = "",
                    });
                    indexColumn++;

                    listData.Add(listDataRow);
                });

                #endregion

                return new GetDataBaoCaoAllowancesResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListHeader1 = listHeader1,
                    ListHeader2 = listHeader2,
                    ListData = listData,
                    TongSoCotDong = listTroCapCoDinh.Count + 1
                };
            }
            catch (Exception e)
            {
                return new GetDataBaoCaoAllowancesResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    MessageCode = e.Message
                };
            }
        }

        public DownloadTemplateImportResult DownloadTemplateImport(DownloadTemplateImportParameter parameter)
        {
            try
            {
                string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = "";

                //Trợ cấp khác
                if (parameter.Type == 1)
                {
                    fileName = "Template_Import_TroCapKhac" + ".xlsx";
                }
                //Lương chi tiết
                else if (parameter.Type == 2)
                {
                    fileName = "Template_Import_LuongChiTiet" + ".xlsx";
                }

                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateImportResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    TemplateExcel = data,
                    FileName = fileName,
                };
            }
            catch (Exception e)
            {
                return new DownloadTemplateImportResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    MessageCode = e.Message
                };
            }
        }

        private BaseResult SendEmail(MailModel mailModel, List<string> toAddress, string subject, string message)
        {
            var feedback = new BaseResult();

            //Config for using default receiver email

            var fromAddress = mailModel.SmtpEmailAccount;
            var toEmailAddress = new List<string>(); //Config for using default receiver email
            
            if (toAddress != null)
            {
                toEmailAddress.AddRange(toAddress);
            }

            // Validate sender and receiver email addresses););
            if (!ValidateEmailAddress(fromAddress))
            {
                feedback.Status = false;
                feedback.Message = "Emai không đúng định dạng";
                return feedback;
            }

            if (!ValidateEmailAddress(toEmailAddress))
            {
                feedback.Status = false;
                feedback.Message = "Emai không đúng định dạng";
                return feedback;
            }

            using (var mailMessage = new MailMessage())
            {
                var smtpClient = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(mailModel.SmtpEmailAccount, mailModel.SmtpPassword),
                    Host = mailModel.SmtpServer,
                    Port = mailModel.SmtpPort,
                    EnableSsl = mailModel.SmtpSsl
                };

                // Build Email
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(fromAddress, "TNM");
                foreach (var addReceiver in toEmailAddress) mailMessage.To.Add(addReceiver);

                AlternateView alterView = ContentToAlternateView(message);
                mailMessage.AlternateViews.Add(alterView);
                mailMessage.Subject = subject;

                try
                {
                    smtpClient.Send(mailMessage);
                    feedback.Status = true;
                    feedback.StatusCode = HttpStatusCode.OK;
                }
                catch (SmtpException ex)
                {
                    feedback.Status = false;
                    feedback.Message = ex.ToString();
                    feedback.StatusCode = HttpStatusCode.Forbidden;
                }

                return feedback;
            }
        }

        private static bool ValidateEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress)) return false;

            if (!Regex.IsMatch(emailAddress, "^[-A-Za-z0-9_@.]+$")) return false;

            // Search for the @ char
            var i = emailAddress.IndexOf("@", StringComparison.Ordinal);

            // There must be at least 3 chars after the @
            if (i <= 0 || i >= emailAddress.Length - 3) return false;

            // Ensure there is only one @
            if (emailAddress.IndexOf("@", i + 1, StringComparison.Ordinal) > 0) return false;

            // Check the domain portion contains at least one dot
            var j = emailAddress.LastIndexOf(".", StringComparison.Ordinal);

            // It can't be before or immediately after the @ character
            if (j < 0 || j <= i + 1) return false;

            // EmailAddress is validated
            return true;
        }

        private static bool ValidateEmailAddress(IEnumerable<string> emailAddress)
        {
            foreach (var email in emailAddress)
            {
                // ReShaper suggest declare variable in here
                var retValue = ValidateEmailAddress(email);
                if (!retValue) return false;
            }

            return true;
        }

        private static AlternateView ContentToAlternateView(string content)
        {
            var imgCount = 0;
            List<LinkedResource> resourceCollection = new List<LinkedResource>();
            foreach (Match m in Regex.Matches(content, "<img(?<value>.*?)>"))
            {
                imgCount++;
                var imgContent = m.Groups["value"].Value;
                string type = Regex.Match(imgContent, ":(?<type>.*?);base64,").Groups["type"].Value;
                string base64 = Regex.Match(imgContent, "base64,(?<base64>.*?)\"").Groups["base64"].Value;
                if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(base64))
                {
                    //ignore replacement when match normal <img> tag
                    continue;
                }
                var replacement = " src=\"cid:" + imgCount + "\"";
                content = content.Replace(imgContent, replacement);
                var tempResource = new LinkedResource(Base64ToImageStream(base64), new ContentType(type))
                {
                    ContentId = imgCount.ToString()
                };
                resourceCollection.Add(tempResource);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);
            foreach (var item in resourceCollection)
            {
                alternateView.LinkedResources.Add(item);
            }

            return alternateView;
        }

        private static Stream Base64ToImageStream(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            return ms;
        }

        private int ConvertNgayTrongTuan(DayOfWeek ngay)
        {
            switch (ngay)
            {
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                case DayOfWeek.Sunday:
                    return 0;
                default: return -1;
            }
        }

        private void GetGioChamCong(Guid employeeId, DateTime ngayChamCong, List<TimeSpan> listThoiGianChamCong, List<CaLamViecModel> listCaLamViec, 
            int ngayTrongTuan, out bool isNoData, out ChamCong data, out List<ThongKeDiMuonVeSom> listThongKeDiMuonVeSom)
        {
            data = new ChamCong
            {
                VaoSang = null,
                RaSang = null,
                VaoChieu = null,
                RaChieu = null
            };

            listThongKeDiMuonVeSom = new List<ThongKeDiMuonVeSom>();

            isNoData = false;
            listThoiGianChamCong = listThoiGianChamCong.OrderBy(z => z).ToList();

            var listCaLamViecChiTiet = new List<CaLamViecChiTietModel>();
            listCaLamViec.ForEach(item =>
            {
                listCaLamViecChiTiet.AddRange(item.ListCaLamViecChiTiet);
            });

            //Lấy ra cấu hình ca làm việc chi tiết theo ngày chấm công
            var _listCaLamViecChiTiet = listCaLamViecChiTiet.Where(x => x.NgayTrongTuan == ngayTrongTuan).ToList();

            //Nếu ngày chấm công không có trong cấu hình ca làm việc thì bỏ qua không thêm data
            if (_listCaLamViecChiTiet.Count == 0)
            {
                isNoData = true;
                return;
            }

            var _listCaLamViecId = _listCaLamViecChiTiet.Select(y => y.CaLamViecId).ToList();

            /* Nếu ngày chấm công có cấu hình ca làm việc thì */
            var caSang =
                listCaLamViec.FirstOrDefault(x => _listCaLamViecId.Contains(x.CaLamViecId) && x.LoaiCaLamViecId == 1);
            var caChieu =
                listCaLamViec.FirstOrDefault(x => _listCaLamViecId.Contains(x.CaLamViecId) && x.LoaiCaLamViecId == 2);

            //Nếu có cấu hình ca sáng
            if (caSang != null)
            {
                var _listGio = listThoiGianChamCong.Where(x => x <= caSang.ThoiGianKetThucCa)
                    .OrderBy(z => z).ToList();

                if (_listGio.Count == 1)
                {
                    data.VaoSang = _listGio.First();

                    //Nếu đi muộn
                    if (data.VaoSang > caSang.GioVao)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(caSang.GioVao, data.VaoSang.Value));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 1,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }
                }

                if (_listGio.Count > 1)
                {
                    data.VaoSang = _listGio.First();
                    data.RaSang = _listGio.Last();

                    #region Tính đi muộn về sớm

                    //Nếu đi muộn
                    if (data.VaoSang > caSang.GioVao)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(caSang.GioVao, data.VaoSang.Value));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 1,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }

                    //Nếu về sớm
                    if (data.RaSang < caSang.GioRa)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(data.RaSang.Value, caSang.GioRa));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 2,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }

                    #endregion
                }
            }

            //Nếu có cấu hình ca chiều
            if (caChieu != null)
            {
                var _listGio = listThoiGianChamCong.Where(x => (caSang == null || x > caSang.ThoiGianKetThucCa) && 
                                                               x <= caChieu.ThoiGianKetThucCa)
                    .OrderBy(z => z).ToList();

                if (_listGio.Count == 1)
                {
                    data.VaoChieu = _listGio.First();

                    //Nếu đi muộn
                    if (data.VaoChieu > caChieu.GioVao)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(caChieu.GioVao, data.VaoChieu.Value));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 3,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }
                }

                if (_listGio.Count > 1)
                {
                    data.VaoChieu = _listGio.First();
                    data.RaChieu = _listGio.Last();

                    #region Tính đi muộn về sớm

                    //Nếu đi muộn
                    if (data.VaoChieu > caChieu.GioVao)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(caChieu.GioVao, data.VaoChieu.Value));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 3,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }

                    //Nếu về sớm
                    if (data.RaChieu < caChieu.GioRa)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(data.RaChieu.Value, caChieu.GioRa));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 4,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }

                    #endregion
                }
            }
        }

        private double TinhSoPhut(TimeSpan start, TimeSpan end)
        {
            double result = (end - start).TotalMinutes;

            return result;
        }

        private string HienThiKyHieu(ChamCong chamCong, int type)
        {
            string result = "--";
            var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong");

            //Vào sáng
            if (type == 1)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.VaoSang != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuVaoSang == null)
                    {
                        result = chamCong.VaoSang.Value.ToString(@"hh\:mm");
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuVaoSang != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)?.ValueText;
                    }
                }
            }
            //Ra sáng
            else if (type == 2)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.RaSang != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuRaSang == null)
                    {
                        result = chamCong.RaSang.Value.ToString(@"hh\:mm");
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuRaSang)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuRaSang != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuRaSang)?.ValueText;
                    }
                }
            }
            //Vào chiều
            else if (type == 3)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.VaoChieu != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuVaoChieu == null)
                    {
                        result = chamCong.VaoChieu.Value.ToString(@"hh\:mm");
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuVaoChieu != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)?.ValueText;
                    }
                }
            }
            //Ra chiều
            else if (type == 4)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.RaChieu != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuRaChieu == null)
                    {
                        result = chamCong.RaChieu.Value.ToString(@"hh\:mm");
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuRaChieu)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuRaChieu != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuRaChieu)?.ValueText;
                    }
                }
            }

            return result;
        }

        private string HienThiMau(ChamCong chamCong, int type, List<ThongKeDiMuonVeSom> listDmvs)
        {
            string result = "#000";
            string red = "#f44336";
            var _listDmvs = listDmvs.Where(x => x.EmployeeId == chamCong.EmployeeId && x.NgayDmvs == chamCong.NgayChamCong)
                .ToList();

            //Vào sáng
            if (type == 1)
            {
                if (chamCong.VaoSang != null)
                {
                    var dmvs = _listDmvs.FirstOrDefault(x => x.LoaiDmvs == 1);

                    //Nếu ko có ký hiệu và là đi muộn về sớm
                    if (chamCong.KyHieuVaoSang == null && dmvs != null)
                    {
                        result = red;
                    }
                }
            }
            //Ra sáng
            else if (type == 2)
            {
                if (chamCong.RaSang != null)
                {
                    var dmvs = _listDmvs.FirstOrDefault(x => x.LoaiDmvs == 2);

                    //Nếu ko có ký hiệu và là đi muộn về sớm
                    if (chamCong.KyHieuRaSang == null && dmvs != null)
                    {
                        result = red;
                    }
                }
            }
            //Vào chiều
            else if (type == 3)
            {
                if (chamCong.VaoChieu != null)
                {
                    var dmvs = _listDmvs.FirstOrDefault(x => x.LoaiDmvs == 3);

                    //Nếu ko có ký hiệu và là đi muộn về sớm
                    if (chamCong.KyHieuVaoChieu == null && dmvs != null)
                    {
                        result = red;
                    }
                }
            }
            //Ra chiều
            else if (type == 4)
            {
                if (chamCong.RaChieu != null)
                {
                    var dmvs = _listDmvs.FirstOrDefault(x => x.LoaiDmvs == 4);

                    //Nếu ko có ký hiệu và là đi muộn về sớm
                    if (chamCong.KyHieuRaChieu == null && dmvs != null)
                    {
                        result = red;
                    }
                }
            }

            return result;
        }

        private int GetSoLanDmvs(Guid employeeId, List<ThongKeDiMuonVeSom> listDmvs)
        {
            int result = 0;

            result = listDmvs.Count(x => x.EmployeeId == employeeId);

            return result;
        }

        private double GetTongSoNgayNghi(Guid employeeId, List<ChamCong> listChamCong)
        {
            double result = 0;

            var _listChamCong = listChamCong.Where(x => x.EmployeeId == employeeId &&
                                                        (x.KyHieuVaoSang != null || x.KyHieuRaSang != null ||
                                                         x.KyHieuVaoChieu != null || x.KyHieuRaChieu != null)).ToList();

            //Bỏ qua nếu đc đánh dấu là Làm tại nhà (mã 14)
            _listChamCong.ForEach(item =>
            {
                if (item.KyHieuVaoSang != null && item.KyHieuVaoSang != 14) result += 0.25;
                if (item.KyHieuRaSang != null && item.KyHieuRaSang != 14) result += 0.25;
                if (item.KyHieuVaoChieu != null && item.KyHieuVaoChieu != 14) result += 0.25;
                if (item.KyHieuRaChieu != null && item.KyHieuRaChieu != 14) result += 0.25;
            });

            return result;
        }

        private CaLamViecModel CheckExistsCauHinhChamCong(List<CaLamViecModel> listCaLamViec, int ngayTrongTuan, int loaiCaLamViecId)
        {
            //Mặc định là có cấu hình
            var caLamViec = new CaLamViecModel();

            var listCaLamViecChiTiet = new List<CaLamViecChiTietModel>();
            listCaLamViec.ForEach(item =>
            {
                listCaLamViecChiTiet.AddRange(item.ListCaLamViecChiTiet);
            });

            //Lấy ra cấu hình ca làm việc chi tiết theo ngày chấm công
            var _listCaLamViecChiTiet = listCaLamViecChiTiet.Where(x => x.NgayTrongTuan == ngayTrongTuan).ToList();
            var _listCaLamViecId = _listCaLamViecChiTiet.Select(y => y.CaLamViecId).ToList();

            /* Nếu ngày chấm công có cấu hình ca làm việc thì */
            caLamViec = listCaLamViec.FirstOrDefault(x =>
                _listCaLamViecId.Contains(x.CaLamViecId) && x.LoaiCaLamViecId == loaiCaLamViecId);

            return caLamViec;
        }

        private string CheckGioVaoGioRa(TimeSpan? gioVao, TimeSpan? gioRa)
        {
            string result = "";

            bool valid_1 = !(gioVao != null && gioRa != null && gioVao > gioRa);

            bool valid_2 = !(gioVao == null && gioRa != null);


            if (!valid_1)
            {
                result = "Giờ vào phải trước Giờ ra";
            }
            else if (!valid_2)
            {
                result = "Có giờ ra mà không có giờ vào";
            }

            return result;
        }

        private void GetThongKeDmvsTheoThoiGian(CaLamViecModel caLamViec, int typeField, TimeSpan thoiGian, out ThongKeDiMuonVeSom thongKeDmvs)
        {
            thongKeDmvs = new ThongKeDiMuonVeSom();
            thongKeDmvs.LoaiDmvs = typeField;

            //Vào sáng
            if (typeField == 1 || typeField == 3)
            {
                if (thoiGian > caLamViec.GioVao)
                {
                    int _thoiGian = (int)Math.Floor(TinhSoPhut(caLamViec.GioVao, thoiGian));
                    if (_thoiGian > 0)
                    {
                        thongKeDmvs.ThoiGian = _thoiGian;
                    }
                }
            }
            //Ra sáng
            else if (typeField == 2 || typeField == 4)
            {
                if (thoiGian < caLamViec.GioRa)
                {
                    int _thoiGian = (int)Math.Floor(TinhSoPhut(thoiGian, caLamViec.GioRa));
                    if (_thoiGian > 0)
                    {
                        thongKeDmvs.ThoiGian = _thoiGian;
                    }
                }
            }
        }

        private bool CheckSyntaxCongThuc(string congThuc)
        {
            bool isValid;
            var listTokenTinhLuong = GeneralList.GetTrangThais("TokenTinhLuong");

            listTokenTinhLuong.ForEach(token => { congThuc = congThuc.Replace(token.ValueText, "1"); });

            try
            {
                var result = new DataTable().Compute(congThuc, null).ToString();

                decimal _result;
                isValid = decimal.TryParse(result, out _result);
            }
            //Nếu công thức sai syntax
            catch (Exception e)
            {
                isValid = false;
            }

            return isValid;
        }

        private string ConvertGioChamCongOt(TimeSpan? gio)
        {
            string result = "--";

            if (gio != null)
            {
                result = gio.Value.ToString(@"hh\:mm");
            }

            return result;
        }

        private List<TongHopChamCongOt> TinhTongHopChamCongOt(List<ChamCongOt> listChamCongOt)
        {
            var result = new List<TongHopChamCongOt>();

            #region Ca sáng

            var listCaSang = listChamCongOt
                .Where(x => x.GioVaoSang != null && x.GioRaSang != null && x.GioVaoSang < x.GioRaSang)
                .GroupBy(g => new {g.EmployeeId, g.LoaiOtId, g.NgayChamCong})
                .Select(y => new TongHopChamCongOt
                {
                    EmployeeId = y.Key.EmployeeId,
                    NgayChamCong = y.Key.NgayChamCong,
                    LoaiOtId = y.Key.LoaiOtId,
                    SoPhut = Math.Round(
                        (decimal) y.Sum(s => s.GioRaSang.Value.TotalMinutes - s.GioVaoSang.Value.TotalMinutes), 0),
                    SoGio = Math.Round(
                        (decimal) y.Sum(s => s.GioRaSang.Value.TotalMinutes - s.GioVaoSang.Value.TotalMinutes) / 60, 2)
                }).ToList();

            result.AddRange(listCaSang);

            #endregion

            #region Ca chiều

            var listCaChieu = listChamCongOt
                .Where(x => x.GioVaoChieu != null && x.GioRaChieu != null && x.GioVaoChieu < x.GioRaChieu)
                .GroupBy(g => new {g.EmployeeId, g.LoaiOtId, g.NgayChamCong})
                .Select(y => new TongHopChamCongOt
                {
                    EmployeeId = y.Key.EmployeeId,
                    NgayChamCong = y.Key.NgayChamCong,
                    LoaiOtId = y.Key.LoaiOtId,
                    SoPhut = Math.Round(
                        (decimal) y.Sum(s => s.GioRaChieu.Value.TotalMinutes - s.GioVaoChieu.Value.TotalMinutes), 0),
                    SoGio = Math.Round(
                        (decimal) y.Sum(s => s.GioRaChieu.Value.TotalMinutes - s.GioVaoChieu.Value.TotalMinutes) / 60, 2)
                }).ToList();

            result.AddRange(listCaChieu);

            #endregion

            #region Ca tối

            var listCaToi = listChamCongOt
                .Where(x => x.GioVaoToi != null && x.GioRaToi != null && x.GioVaoToi < x.GioRaToi)
                .GroupBy(g => new {g.EmployeeId, g.LoaiOtId, g.NgayChamCong})
                .Select(y => new TongHopChamCongOt
                {
                    EmployeeId = y.Key.EmployeeId,
                    NgayChamCong = y.Key.NgayChamCong,
                    LoaiOtId = y.Key.LoaiOtId,
                    SoPhut = Math.Round(
                        (decimal) y.Sum(s => s.GioRaToi.Value.TotalMinutes - s.GioVaoToi.Value.TotalMinutes), 0),
                    SoGio = Math.Round(
                        (decimal) y.Sum(s => s.GioRaToi.Value.TotalMinutes - s.GioVaoToi.Value.TotalMinutes) / 60, 2)
                }).ToList();

            result.AddRange(listCaToi);

            #endregion

            result = result.GroupBy(g => new { g.EmployeeId, g.LoaiOtId, g.NgayChamCong })
                .Select(y => new TongHopChamCongOt
                {
                    EmployeeId = y.Key.EmployeeId,
                    NgayChamCong = y.Key.NgayChamCong,
                    LoaiOtId = y.Key.LoaiOtId,
                    SoPhut = y.Sum(s => s.SoPhut),
                    SoGio = y.Sum(s => s.SoGio)
                }).ToList();

            return result;
        }

        private void SaveTongHopChamCong(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();

            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeCode).ToList();

            var listDmvs = context.ThongKeDiMuonVeSom
                .Where(x => listEmpId.Contains(x.EmployeeId) &&
                            x.NgayDmvs.Date >= ngayBatDau.Date &&
                            x.NgayDmvs.Date <= ngayKetThuc.Date).ToList();

            var listTongHopChamCong = new List<TongHopChamCong>();

            listEmp.ForEach(item =>
            {
                var newItem = new TongHopChamCong();
                newItem.KyLuongId = kyLuongId;
                newItem.EmployeeId = item.EmployeeId;
                newItem.OrganizationId = item.OrganizationId ?? Guid.Empty;
                newItem.PositionId = item.PositionId ?? Guid.Empty;

                var listChamCongByEmp = listChamCong.Where(x => x.EmployeeId == item.EmployeeId).ToList();

                #region Tính số ngày

                double ngayLvThucTe = 0;
                double ngayCongTac = 0;
                double ngayHoiThao = 0;
                double ngayNghiPhep = 0;
                double ngayNghiLe = 0;
                double ngayNghiCheDo = 0;
                double ngayNghiHuongLuongKhac = 0;
                double ngayNghiBu = 0;
                double ngayNghiBhxh = 0;
                double ngayNghiTuYKoLuong = 0;
                double ngayNghiKoLuong_KoDuLieu = 0;

                listChamCongByEmp.ForEach(chamCong =>
                {
                    #region Ngày làm việc thực tế

                    if (chamCong.KyHieuVaoSang == null && chamCong.KyHieuRaSang == null &&
                        chamCong.VaoSang != null && chamCong.RaSang != null)
                    {
                        ngayLvThucTe += 0.5;
                    }

                    if (chamCong.KyHieuVaoChieu == null && chamCong.KyHieuRaChieu == null &&
                        chamCong.VaoChieu != null && chamCong.RaChieu != null)
                    {
                        ngayLvThucTe += 0.5;
                    }

                    if (chamCong.KyHieuVaoSang == 14) ngayLvThucTe += 0.25;

                    if (chamCong.KyHieuRaSang == 14) ngayLvThucTe += 0.25;

                    if (chamCong.KyHieuVaoChieu == 14) ngayLvThucTe += 0.25;

                    if (chamCong.KyHieuRaChieu == 14) ngayLvThucTe += 0.25;

                    #endregion

                    #region Ngày công tác

                    if (chamCong.KyHieuVaoSang == 6) ngayCongTac += 0.25;
                    if (chamCong.KyHieuRaSang == 6) ngayCongTac += 0.25;
                    if (chamCong.KyHieuVaoChieu == 6) ngayCongTac += 0.25;
                    if (chamCong.KyHieuRaChieu == 6) ngayCongTac += 0.25;

                    #endregion

                    #region Ngày hội thảo

                    if (chamCong.KyHieuVaoSang == 7) ngayHoiThao += 0.25;
                    if (chamCong.KyHieuRaSang == 7) ngayHoiThao += 0.25;
                    if (chamCong.KyHieuVaoChieu == 7) ngayHoiThao += 0.25;
                    if (chamCong.KyHieuRaChieu == 7) ngayHoiThao += 0.25;

                    #endregion

                    #region Ngày nghỉ phép

                    if (chamCong.KyHieuVaoSang == 1) ngayNghiPhep += 0.25;
                    if (chamCong.KyHieuRaSang == 1) ngayNghiPhep += 0.25;
                    if (chamCong.KyHieuVaoChieu == 1) ngayNghiPhep += 0.25;
                    if (chamCong.KyHieuRaChieu == 1) ngayNghiPhep += 0.25;

                    #endregion

                    #region Ngày nghỉ lễ

                    if (chamCong.KyHieuVaoSang == 2) ngayNghiLe += 0.25;
                    if (chamCong.KyHieuRaSang == 2) ngayNghiLe += 0.25;
                    if (chamCong.KyHieuVaoChieu == 2) ngayNghiLe += 0.25;
                    if (chamCong.KyHieuRaChieu == 2) ngayNghiLe += 0.25;

                    #endregion

                    #region Ngày nghỉ chế độ

                    if (chamCong.KyHieuVaoSang == 5) ngayNghiCheDo += 0.25;
                    if (chamCong.KyHieuRaSang == 5) ngayNghiCheDo += 0.25;
                    if (chamCong.KyHieuVaoChieu == 5) ngayNghiCheDo += 0.25;
                    if (chamCong.KyHieuRaChieu == 5) ngayNghiCheDo += 0.25;

                    #endregion

                    #region Ngày nghỉ hưởng lương khác

                    if (chamCong.KyHieuVaoSang == 4) ngayNghiHuongLuongKhac += 0.25;
                    if (chamCong.KyHieuRaSang == 4) ngayNghiHuongLuongKhac += 0.25;
                    if (chamCong.KyHieuVaoChieu == 4) ngayNghiHuongLuongKhac += 0.25;
                    if (chamCong.KyHieuRaChieu == 4) ngayNghiHuongLuongKhac += 0.25;

                    #endregion

                    #region Ngày nghỉ bù

                    if (chamCong.KyHieuVaoSang == 3) ngayNghiBu += 0.25;
                    if (chamCong.KyHieuRaSang == 3) ngayNghiBu += 0.25;
                    if (chamCong.KyHieuVaoChieu == 3) ngayNghiBu += 0.25;
                    if (chamCong.KyHieuRaChieu == 3) ngayNghiBu += 0.25;

                    #endregion

                    #region Ngày nghỉ BHXH

                    if (chamCong.KyHieuVaoSang == 11) ngayNghiBhxh += 0.25;
                    if (chamCong.KyHieuRaSang == 11) ngayNghiBhxh += 0.25;
                    if (chamCong.KyHieuVaoChieu == 11) ngayNghiBhxh += 0.25;
                    if (chamCong.KyHieuRaChieu == 11) ngayNghiBhxh += 0.25;

                    #endregion

                    #region Ngày nghỉ tự ý nghỉ không lương

                    if (chamCong.KyHieuVaoSang == 9) ngayNghiTuYKoLuong += 0.25;
                    if (chamCong.KyHieuRaSang == 9) ngayNghiTuYKoLuong += 0.25;
                    if (chamCong.KyHieuVaoChieu == 9) ngayNghiTuYKoLuong += 0.25;
                    if (chamCong.KyHieuRaChieu == 9) ngayNghiTuYKoLuong += 0.25;

                    #endregion

                    #region Ngày nghỉ không lương + Không có dữ liệu bảng công

                    if (chamCong.KyHieuVaoSang == 8 || chamCong.KyHieuVaoSang == 10) ngayNghiKoLuong_KoDuLieu += 0.25;
                    if (chamCong.KyHieuRaSang == 8 || chamCong.KyHieuRaSang == 10) ngayNghiKoLuong_KoDuLieu += 0.25;
                    if (chamCong.KyHieuVaoChieu == 8 || chamCong.KyHieuVaoChieu == 10) ngayNghiKoLuong_KoDuLieu += 0.25;
                    if (chamCong.KyHieuRaChieu == 8 || chamCong.KyHieuRaChieu == 10) ngayNghiKoLuong_KoDuLieu += 0.25;

                    #endregion
                    
                });

                #region Tổng ngày đi muộn/về sớm không được trả lương

                int tongSoPhutDmvs = CommonHelper.GetSoPhutDmvs(item.EmployeeId, listDmvs);
                double tongSoNgayDmvs = Math.Round((double)tongSoPhutDmvs / 480, 2);

                #endregion

                #region Số lần trừ chuyên cần

                int tongSoLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);

                #endregion

                #region Tổng số ngày không tính lương

                double tongNgayKhongTinhLuong = ngayNghiKoLuong_KoDuLieu + ngayNghiTuYKoLuong +
                                                ngayNghiBhxh + tongSoNgayDmvs;

                #endregion

                #region Tổng số ngày tính lương

                double tongNgayTinhLuong = ngayLvThucTe + ngayCongTac + ngayHoiThao + ngayNghiPhep +
                                           ngayNghiLe + ngayNghiBu + ngayNghiCheDo + ngayNghiHuongLuongKhac -
                                           tongSoNgayDmvs;

                #endregion

                #region Tổng ngày nghỉ tính trợ cấp chuyên cần

                double tongNgayNghiTinhTroCapChuyenCan = ngayNghiPhep + ngayNghiKoLuong_KoDuLieu + ngayNghiTuYKoLuong;

                #endregion

                #endregion

                newItem.NgayLamViecThucTe = (decimal) ngayLvThucTe;
                newItem.CongTac = (decimal) ngayCongTac;
                newItem.DaoTaoHoiThao = (decimal) ngayHoiThao;
                newItem.NghiPhep = (decimal) ngayNghiPhep;
                newItem.NghiLe = (decimal) ngayNghiLe;
                newItem.NghiCheDo = (decimal) ngayNghiCheDo;
                newItem.NghiHuongLuongKhac = (decimal) ngayNghiHuongLuongKhac;
                newItem.NghiBu = (decimal) ngayNghiBu;
                newItem.NghiHuongBhxh = (decimal) ngayNghiBhxh;
                newItem.NghiKhongPhep = (decimal) ngayNghiTuYKoLuong;
                newItem.NghiKhongLuong = (decimal) ngayNghiKoLuong_KoDuLieu;
                newItem.TongNgayDmvs = (decimal) tongSoNgayDmvs;
                newItem.SoLanTruChuyenCan = (decimal) tongSoLanDmvs;

                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                //Create
                if (exists == null)
                {
                    listTongHopChamCong.Add(newItem);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.PositionId ?? Guid.Empty;
                    exists.NgayLamViecThucTe = (decimal)ngayLvThucTe;
                    exists.CongTac = (decimal)ngayCongTac;
                    exists.DaoTaoHoiThao = (decimal)ngayHoiThao;
                    exists.NghiPhep = (decimal)ngayNghiPhep;
                    exists.NghiLe = (decimal)ngayNghiLe;
                    exists.NghiCheDo = (decimal)ngayNghiCheDo;
                    exists.NghiHuongLuongKhac = (decimal)ngayNghiHuongLuongKhac;
                    exists.NghiBu = (decimal)ngayNghiBu;
                    exists.NghiHuongBhxh = (decimal)ngayNghiBhxh;
                    exists.NghiKhongPhep = (decimal)ngayNghiTuYKoLuong;
                    exists.NghiKhongLuong = (decimal)ngayNghiKoLuong_KoDuLieu;
                    exists.TongNgayDmvs = (decimal)tongSoNgayDmvs;
                    exists.SoLanTruChuyenCan = (decimal)tongSoLanDmvs;

                    context.TongHopChamCong.Update(exists);
                }
            });

            context.TongHopChamCong.AddRange(listTongHopChamCong);
            context.SaveChanges();
        }

        private void SaveLuongCt_TroCap(int kyLuongId, decimal soNgayLamViec, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOldLuongCtTroCapCoDinh = context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listOldLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listOldLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();

            context.SaveChanges();

            #endregion

            var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listEmpId = listTongHopChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeCode).ToList();

            var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
            var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();
            var listHopDongNhanSu = context.HopDongNhanSu.Where(x => listEmpId.Contains(x.EmployeeId)).ToList();
            var listTroCap = context.TroCap.Where(x => x.TypeId != 4).ToList();
            var listTroCapChucVu = context.TroCapChucVuMapping.ToList();
            var listTroCapLoaiHopDong = context.TroCapLoaiHopDongMapping.ToList();
            var listTroCapDieuKienHuong = context.TroCapDieuKienHuongMapping.ToList();
            var listMucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi.ToList();
            var listMucHuongDmvs = context.MucHuongDmvs.ToList();
            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listDmvs = context.ThongKeDiMuonVeSom.Where(x => listEmpId.Contains(x.EmployeeId) &&
                                                                 x.NgayDmvs.Date >= ngayBatDau.Date &&
                                                                 x.NgayDmvs.Date <= ngayKetThuc.Date).ToList(); 

            for (int e = 0; e < listEmp.Count; e++)
            {
                var item = listEmp[e];

                var ngayLamViecThucTe = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.EmployeeId)
                                       ?.NgayLamViecThucTe ?? 0;

                if (ngayLamViecThucTe == 0)
                {
                    //continue;
                }

                var hopDongMoiNhat = listHopDongNhanSu.Where(x => x.EmployeeId == item.EmployeeId)
                    .OrderByDescending(z => z.NgayBatDauLamViec).FirstOrDefault();
                var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == hopDongMoiNhat?.LoaiHopDongId);
                var listChamCongByEmp = listChamCong.Where(x => x.EmployeeId == item.EmployeeId).ToList();

                var existsTroCapCoDinh =
                    listOldLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                //Nếu chưa có data bảng 1
                if (existsTroCapCoDinh == null)
                {
                    var luongCtTroCapCoDinh = new LuongCtTroCapCoDinh();
                    luongCtTroCapCoDinh.KyLuongId = kyLuongId;
                    luongCtTroCapCoDinh.EmployeeId = item.EmployeeId;
                    luongCtTroCapCoDinh.OrganizationId = item.OrganizationId ?? Guid.Empty;
                    luongCtTroCapCoDinh.PositionId = item.PositionId ?? Guid.Empty;
                    luongCtTroCapCoDinh.LoaiHopDongId = loaiHopDong?.CategoryId;

                    context.LuongCtTroCapCoDinh.Add(luongCtTroCapCoDinh);
                    context.SaveChanges();

                    #region Trợ cấp cố định

                    var listTroCapCoDinh = listTroCap.Where(x => x.TypeId == 1).ToList();

                    for (int i = 0; i < listTroCapCoDinh.Count; i++)
                    {
                        var troCap = listTroCapCoDinh[i];
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong =
                            listTroCapDieuKienHuong.Where(x => x.TroCapId == troCap.TroCapId).ToList();

                        var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                        luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                        luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                        luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCap.LoaiTroCapId;
                        luongCtLoaiTroCapCoDinh.MucTroCap = Math.Round(troCap.MucTroCap * ngayLamViecThucTe / soNgayLamViec, 0);

                        //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                        if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                            listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = Math.Round(troCap.MucTroCap * ngayLamViecThucTe / soNgayLamViec, 0);

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                context.SaveChanges();
                            });
                        }
                        //Nếu không đủ điều kiện
                        else
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();
                        }
                    }

                    #endregion

                    #region Trợ cấp theo ngày nghỉ

                    var troCapTheoNgayNghi = listTroCap.FirstOrDefault(x => x.TypeId == 2);

                    if (troCapTheoNgayNghi != null)
                    {
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong
                            .Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong = listTroCapDieuKienHuong
                            .Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listMucHuong = listMucHuongTheoNgayNghi.Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId)
                            .OrderByDescending(z => z.MucHuongPhanTram).ToList();

                        var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                        luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                        luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                        luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCapTheoNgayNghi.LoaiTroCapId;

                        if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                            listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = TinhMucHuongTroCapTheoNgayNghi(troCapTheoNgayNghi,
                                listChamCongByEmp, listMucHuong);

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                context.SaveChanges();
                            });
                        }
                        else
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();
                        }
                    }

                    #endregion

                    #region Trợ cấp theo số lần đi muộn về sớm

                    var troCapTheoDmvs = listTroCap.FirstOrDefault(x => x.TypeId == 3);

                    if (troCapTheoDmvs != null)
                    {
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong
                            .Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong = listTroCapDieuKienHuong
                            .Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listMucHuong = listMucHuongDmvs.Where(x => x.TroCapId == troCapTheoDmvs.TroCapId)
                            .OrderByDescending(z => z.MucTru).ToList();

                        var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                        luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                        luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                        luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCapTheoDmvs.LoaiTroCapId;

                        if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                            listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                        {
                            int soLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);

                            luongCtLoaiTroCapCoDinh.MucTroCap = TinhMucHuongTroCapTheoDmvs(troCapTheoDmvs,
                                soLanDmvs, listMucHuong);

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                context.SaveChanges();
                            });
                        }
                        else
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();
                        }
                    }

                    #endregion
                }
                //Nếu đã có data bảng 1
                else
                {
                    existsTroCapCoDinh.OrganizationId = item.OrganizationId ?? Guid.Empty;
                    existsTroCapCoDinh.PositionId = item.PositionId ?? Guid.Empty;
                    existsTroCapCoDinh.LoaiHopDongId = loaiHopDong?.CategoryId;

                    context.LuongCtTroCapCoDinh.Update(existsTroCapCoDinh);
                    context.SaveChanges();

                    var _listOldLuongCtLoaiTroCapCoDinh = listOldLuongCtLoaiTroCapCoDinh.Where(x =>
                        x.LuongCtTroCapCoDinhId == existsTroCapCoDinh.LuongCtTroCapCoDinhId).ToList();

                    #region Trợ cấp cố định

                    var listTroCapCoDinh = listTroCap.Where(x => x.TypeId == 1).ToList();

                    for (int i = 0; i < listTroCapCoDinh.Count; i++)
                    {
                        var troCap = listTroCapCoDinh[i];
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong =
                            listTroCapDieuKienHuong.Where(x => x.TroCapId == troCap.TroCapId).ToList();

                        var existsLoaiTroCap = listOldLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == existsTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCap.LoaiTroCapId);

                        //Nếu chưa có data bảng 2
                        if (existsLoaiTroCap == null)
                        {
                            var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                            luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                            luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                            luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCap.LoaiTroCapId;

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();

                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = Math.Round(troCap.MucTroCap * ngayLamViecThucTe / soNgayLamViec, 0);

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                    luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId =
                                        luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                    luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                    context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                    context.SaveChanges();
                                });
                            }
                            //Nếu không đủ điều kiện
                            else
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();
                            }
                        }
                        //Nếu đã có data bảng 2
                        else
                        {
                            #region Kiểm tra nếu cấu hình điều kiện hưởng có, mà trong data không có => thêm data

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var existsDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                                    x.LuongCtLoaiTroCapCoDinhId == existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId &&
                                    x.DieuKienHuongId == dieuKienHuong.DieuKienHuongId);

                                //Nếu chưa có data bảng 3
                                if (existsDieuKienHuong == null)
                                {
                                    var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                    luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId =
                                        existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                    luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                    context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                    context.SaveChanges();
                                }
                            });

                            #endregion

                            #region Kiểm tra nếu cấu hình điều kiện hưởng không có, mà trong data có => xóa data

                            var _listOldLuongCtDieuKienTroCapCoDinh = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                    x.LuongCtLoaiTroCapCoDinhId == existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId)
                                .ToList();

                            var listRemoveDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                            _listOldLuongCtDieuKienTroCapCoDinh.ForEach(dataDieuKienHuong =>
                            {
                                var existsDieuKienHuong = listTcDieuKienHuong.FirstOrDefault(x =>
                                    x.DieuKienHuongId == dataDieuKienHuong.DieuKienHuongId);

                                if (existsDieuKienHuong == null)
                                {
                                    listRemoveDieuKienTroCapCoDinh.Add(dataDieuKienHuong);
                                }
                            });

                            context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listRemoveDieuKienTroCapCoDinh);
                            context.SaveChanges();

                            #endregion

                            #region Tính lại mức trợ cấp

                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                                //Nếu tất cả điều kiện hưởng đều = true
                                if (countFalse == 0)
                                {
                                    existsLoaiTroCap.MucTroCap = Math.Round(troCap.MucTroCap * ngayLamViecThucTe / soNgayLamViec, 0);
                                }
                                //Nếu có điều kiện hưởng = false
                                else
                                {
                                    existsLoaiTroCap.MucTroCap = 0;
                                }
                            }
                            //Nếu không đủ điều kiện
                            else
                            {
                                existsLoaiTroCap.MucTroCap = 0;

                                //Xóa điều kiện hưởng
                                var newListDieuKienHuong = context.LuongCtDieuKienTroCapCoDinh.Where(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId).ToList();

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(newListDieuKienHuong);
                            }

                            context.LuongCtLoaiTroCapCoDinh.Update(existsLoaiTroCap);
                            context.SaveChanges();

                            #endregion
                        }
                    }

                    #endregion

                    #region Trợ cấp theo ngày nghỉ

                    var troCapTheoNgayNghi = listTroCap.FirstOrDefault(x => x.TypeId == 2);

                    if (troCapTheoNgayNghi != null)
                    {
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong
                            .Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong = listTroCapDieuKienHuong
                            .Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listMucHuong = listMucHuongTheoNgayNghi.Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId)
                            .OrderByDescending(z => z.MucHuongPhanTram).ToList();

                        var existsDataTroCapTheoNgayNghi = listOldLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == existsTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapTheoNgayNghi.LoaiTroCapId);

                        //Nếu không có data trong bảng 2
                        if (existsDataTroCapTheoNgayNghi == null)
                        {
                            var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                            luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                            luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                            luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCapTheoNgayNghi.LoaiTroCapId;

                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = TinhMucHuongTroCapTheoNgayNghi(troCapTheoNgayNghi,
                                    listChamCongByEmp, listMucHuong);

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                    luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                    luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                    context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                    context.SaveChanges();
                                });
                            }
                            else
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();
                            }
                        }
                        //Nếu có data trong bảng 2
                        else
                        {
                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                #region Kiểm tra nếu cấu hình điều kiện hưởng có, mà trong data không có => thêm data

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var existsDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                                        x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId &&
                                        x.DieuKienHuongId == dieuKienHuong.DieuKienHuongId);

                                    //Nếu chưa có data bảng 3
                                    if (existsDieuKienHuong == null)
                                    {
                                        var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                        luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                        luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                        luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId =
                                            existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId;
                                        luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                        luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                        context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                        context.SaveChanges();
                                    }
                                });

                                #endregion

                                #region Kiểm tra nếu cấu hình điều kiện hưởng không có, mà trong data có => xóa data

                                var _listOldLuongCtDieuKienTroCapCoDinh = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                        x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId)
                                    .ToList();

                                var listRemoveDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                                _listOldLuongCtDieuKienTroCapCoDinh.ForEach(dataDieuKienHuong =>
                                {
                                    var existsDieuKienHuong = listTcDieuKienHuong.FirstOrDefault(x =>
                                        x.DieuKienHuongId == dataDieuKienHuong.DieuKienHuongId);

                                    if (existsDieuKienHuong == null)
                                    {
                                        listRemoveDieuKienTroCapCoDinh.Add(dataDieuKienHuong);
                                    }
                                });

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listRemoveDieuKienTroCapCoDinh);
                                context.SaveChanges();

                                #endregion

                                #region Tính lại mức trợ cấp

                                var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                                //Nếu tất cả điều kiện hưởng đều = true
                                if (countFalse == 0)
                                {
                                    existsDataTroCapTheoNgayNghi.MucTroCap = TinhMucHuongTroCapTheoNgayNghi(
                                        troCapTheoNgayNghi, listChamCongByEmp, listMucHuong);
                                }
                                //Nếu có điều kiện hưởng = false
                                else
                                {
                                    existsDataTroCapTheoNgayNghi.MucTroCap = 0;
                                }

                                context.LuongCtLoaiTroCapCoDinh.Update(existsDataTroCapTheoNgayNghi);
                                context.SaveChanges();

                                #endregion
                            }
                            //Nếu không đủ điều kiện
                            else
                            {
                                existsDataTroCapTheoNgayNghi.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Update(existsDataTroCapTheoNgayNghi);
                                context.SaveChanges();

                                //Xóa data điều kiện hưởng
                                var newListDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                    x.LuongCtLoaiTroCapCoDinhId ==
                                    existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId).ToList();

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(newListDieuKienHuong);
                                context.SaveChanges();
                            }
                        }
                    }

                    #endregion

                    #region Trợ cấp theo số lần đi muộn về sớm

                    var troCapTheoDmvs = listTroCap.FirstOrDefault(x => x.TypeId == 3);

                    if (troCapTheoDmvs != null)
                    {
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong
                            .Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong = listTroCapDieuKienHuong
                            .Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listMucHuong = listMucHuongDmvs.Where(x => x.TroCapId == troCapTheoDmvs.TroCapId)
                            .OrderByDescending(z => z.MucTru).ToList();

                        var existsDataTroCapTheoDmvs = listOldLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == existsTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapTheoDmvs.LoaiTroCapId);

                        //Nếu không có data trong bảng 2
                        if (existsDataTroCapTheoDmvs == null)
                        {
                            var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                            luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                            luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = existsDataTroCapTheoDmvs.LuongCtTroCapCoDinhId;
                            luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCapTheoDmvs.LoaiTroCapId;

                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                int soLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);

                                luongCtLoaiTroCapCoDinh.MucTroCap = TinhMucHuongTroCapTheoDmvs(troCapTheoDmvs,
                                    soLanDmvs, listMucHuong);

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                    luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsDataTroCapTheoDmvs.LuongCtTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                    luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                    context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                    context.SaveChanges();
                                });
                            }
                            else
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();
                            }
                        }
                        //Nếu có data trong bảng 2
                        else
                        {
                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                #region Kiểm tra nếu cấu hình điều kiện hưởng có, mà trong data không có => thêm data

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var existsDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                                        x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId &&
                                        x.DieuKienHuongId == dieuKienHuong.DieuKienHuongId);

                                    //Nếu chưa có data bảng 3
                                    if (existsDieuKienHuong == null)
                                    {
                                        var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                        luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                        luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                        luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId =
                                            existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId;
                                        luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                        luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                        context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                        context.SaveChanges();
                                    }
                                });

                                #endregion

                                #region Kiểm tra nếu cấu hình điều kiện hưởng không có, mà trong data có => xóa data

                                var _listOldLuongCtDieuKienTroCapCoDinh = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                        x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId)
                                    .ToList();

                                var listRemoveDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                                _listOldLuongCtDieuKienTroCapCoDinh.ForEach(dataDieuKienHuong =>
                                {
                                    var existsDieuKienHuong = listTcDieuKienHuong.FirstOrDefault(x =>
                                        x.DieuKienHuongId == dataDieuKienHuong.DieuKienHuongId);

                                    if (existsDieuKienHuong == null)
                                    {
                                        listRemoveDieuKienTroCapCoDinh.Add(dataDieuKienHuong);
                                    }
                                });

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listRemoveDieuKienTroCapCoDinh);
                                context.SaveChanges();

                                #endregion

                                #region Tính lại mức trợ cấp

                                var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                                //Nếu tất cả điều kiện hưởng đều = true
                                if (countFalse == 0)
                                {
                                    int soLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);

                                    existsDataTroCapTheoDmvs.MucTroCap = TinhMucHuongTroCapTheoDmvs(troCapTheoDmvs,
                                        soLanDmvs, listMucHuong);
                                }
                                //Nếu có điều kiện hưởng = false
                                else
                                {
                                    existsDataTroCapTheoDmvs.MucTroCap = 0;
                                }

                                context.LuongCtLoaiTroCapCoDinh.Update(existsDataTroCapTheoDmvs);
                                context.SaveChanges();

                                #endregion
                            }
                            //Nếu không đủ điều kiện
                            else
                            {
                                existsDataTroCapTheoDmvs.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Update(existsDataTroCapTheoDmvs);
                                context.SaveChanges();

                                //Xóa data điều kiện hưởng
                                var newListDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                    x.LuongCtLoaiTroCapCoDinhId ==
                                    existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId).ToList();

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(newListDieuKienHuong);
                                context.SaveChanges();
                            }
                        }
                    }

                    #endregion

                    #region Kiểm tra nếu cấu hình loại trợ cấp không có, mà trong data có => xóa data

                    var listRemoveLuongCtLoaiTroCapCoDinh = new List<LuongCtLoaiTroCapCoDinh>();
                    var listRemoveLuongCtDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                    _listOldLuongCtLoaiTroCapCoDinh.ForEach(dataLoaiTroCap =>
                    {
                        var existsLoaiTroCap =
                            listTroCap.FirstOrDefault(x => x.LoaiTroCapId == dataLoaiTroCap.LoaiTroCapId);

                        if (existsLoaiTroCap == null)
                        {
                            listRemoveLuongCtLoaiTroCapCoDinh.Add(dataLoaiTroCap);

                            var _listRemoveLuongCtDieuKienTroCapCoDinh = listOldLuongCtDieuKienTroCapCoDinh
                                .Where(x => x.LuongCtLoaiTroCapCoDinhId == dataLoaiTroCap.LuongCtLoaiTroCapCoDinhId)
                                .ToList();

                            listRemoveLuongCtDieuKienTroCapCoDinh.AddRange(_listRemoveLuongCtDieuKienTroCapCoDinh);
                        }
                    });

                    context.LuongCtLoaiTroCapCoDinh.RemoveRange(listRemoveLuongCtLoaiTroCapCoDinh);
                    context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listRemoveLuongCtDieuKienTroCapCoDinh);
                    context.SaveChanges();

                    #endregion
                }
            }
        }

        private void SaveLuongCt_TroCapKhac(int kyLuongId, Employee item, List<Category> listLoaiTroCapKhac)
        {
            var listOldLuongCtTroCapKhac = context.LuongCtTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listOldLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listOldLuongCtDieuKienTroCapKhac = context.LuongCtDieuKienTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listTroCapChucVu = context.TroCapChucVuMapping.ToList();
            var listTroCapLoaiHopDong = context.TroCapLoaiHopDongMapping.ToList();
            var listTroCapDieuKienHuong = context.TroCapDieuKienHuongMapping.ToList();
            var listHopDongNhanSu = context.HopDongNhanSu.Where(x => x.EmployeeId == item.EmployeeId).ToList();
            var hopDongMoiNhat = listHopDongNhanSu.Where(x => x.EmployeeId == item.EmployeeId)
                .OrderByDescending(z => z.NgayBatDauLamViec).FirstOrDefault();
            var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
            var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();
            var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == hopDongMoiNhat?.LoaiHopDongId);

            #region Trợ cấp khác

            var existsTroCapKhac = listOldLuongCtTroCapKhac.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
            var listTroCapKhac = context.TroCap.Where(x => x.TypeId == 4).ToList();

            //Create
            if (existsTroCapKhac == null)
            {
                var luongCtTroCapKhac = new LuongCtTroCapKhac();
                luongCtTroCapKhac.KyLuongId = kyLuongId;
                luongCtTroCapKhac.EmployeeId = item.EmployeeId;
                luongCtTroCapKhac.OrganizationId = item.OrganizationId ?? Guid.Empty;
                luongCtTroCapKhac.PositionId = item.PositionId ?? Guid.Empty;
                luongCtTroCapKhac.LoaiHopDongId = loaiHopDong?.CategoryId;

                context.LuongCtTroCapKhac.Add(luongCtTroCapKhac);
                context.SaveChanges();

                for (int i = 0; i < listLoaiTroCapKhac.Count; i++)
                {
                    var loaiTroCapKhac = listLoaiTroCapKhac[i];

                    var troCap = listTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCapKhac.CategoryId);

                    SaveLuongCt_LoaiTroCapKhac_DieuKienTroCapKhac(item.EmployeeId, troCap, listTroCapChucVu, listTroCapLoaiHopDong,
                        listTroCapDieuKienHuong, kyLuongId, luongCtTroCapKhac, item.PositionId,
                        loaiHopDong?.CategoryId, loaiTroCapKhac.CategoryId);
                }
            }
            //Update
            else
            {
                existsTroCapKhac.OrganizationId = item.OrganizationId ?? Guid.Empty;
                existsTroCapKhac.PositionId = item.PositionId ?? Guid.Empty;
                existsTroCapKhac.LoaiHopDongId = loaiHopDong?.CategoryId;

                context.LuongCtTroCapKhac.Update(existsTroCapKhac);
                context.SaveChanges();

                listOldLuongCtLoaiTroCapKhac = listOldLuongCtLoaiTroCapKhac
                    .Where(x => x.LuongCtTroCapKhacId == existsTroCapKhac.LuongCtTroCapKhacId).ToList();

                for (int i = 0; i < listLoaiTroCapKhac.Count; i++)
                {
                    var loaiTroCapKhac = listLoaiTroCapKhac[i];

                    var existsData = listOldLuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                        x.LoaiTroCapId == loaiTroCapKhac.CategoryId);

                    //Nếu chưa có data loại trợ cấp
                    if (existsData == null)
                    {
                        var existsCauHinh = listTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCapKhac.CategoryId);

                        SaveLuongCt_LoaiTroCapKhac_DieuKienTroCapKhac(item.EmployeeId, existsCauHinh, listTroCapChucVu, listTroCapLoaiHopDong,
                            listTroCapDieuKienHuong, kyLuongId, existsTroCapKhac, item.PositionId,
                            loaiHopDong?.CategoryId, loaiTroCapKhac.CategoryId);
                    }
                    //Nếu có data loại trợ cấp
                    else
                    {
                        var existsLoaiTroCapCauHinh = listTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCapKhac.CategoryId);

                        //Nếu có loại trợ cấp trong cấu hình
                        if (existsLoaiTroCapCauHinh != null)
                        {
                            #region Kiểm tra Điều kiện hưởng trong cấu hình và trong bảng có thay đổi không?

                            var listTcDieuKienHuong =
                                listTroCapDieuKienHuong.Where(x => x.TroCapId == existsLoaiTroCapCauHinh.TroCapId).ToList();

                            var listDataDieuKienHuong = listOldLuongCtDieuKienTroCapKhac.Where(x =>
                                x.LuongCtLoaiTroCapKhacId == existsData.LuongCtLoaiTroCapKhacId).ToList();

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var existsDataDieuKienHuong =
                                    listDataDieuKienHuong.FirstOrDefault(x => x.DieuKienHuongId == dieuKienHuong.DieuKienHuongId);

                                //Nếu điều kiện hưởng có trong cấu hình nhưng chưa có data trong bảng
                                if (existsDataDieuKienHuong == null)
                                {
                                    var luongCtDieuKienTroCapKhac = new LuongCtDieuKienTroCapKhac();
                                    luongCtDieuKienTroCapKhac.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapKhac.LuongCtTroCapKhacId = existsData.LuongCtTroCapKhacId;
                                    luongCtDieuKienTroCapKhac.LuongCtLoaiTroCapKhacId = existsData.LuongCtLoaiTroCapKhacId;
                                    luongCtDieuKienTroCapKhac.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;

                                    context.LuongCtDieuKienTroCapKhac.Add(luongCtDieuKienTroCapKhac);
                                    context.SaveChanges();

                                    SaveLuongCtEmpDktcKhac(item.EmployeeId, kyLuongId, dieuKienHuong.DieuKienHuongId);
                                }
                            });

                            var listRemove = new List<LuongCtDieuKienTroCapKhac>();
                            listDataDieuKienHuong.ForEach(dataDieuKienHuong =>
                            {
                                var existsDieuKienHuong = listTcDieuKienHuong.FirstOrDefault(x =>
                                    x.DieuKienHuongId == dataDieuKienHuong.DieuKienHuongId);

                                //Nếu có data nhưng không có trong cấu hình
                                if (existsDieuKienHuong == null)
                                {
                                    //Thêm vào list để xóa đi
                                    listRemove.Add(dataDieuKienHuong);
                                }
                            });

                            context.LuongCtDieuKienTroCapKhac.RemoveRange(listRemove);
                            context.SaveChanges();

                            #endregion

                            var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == existsLoaiTroCapCauHinh.TroCapId).ToList();
                            var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                            var listTcLoaiHopDong = listTroCapLoaiHopDong.Where(x => x.TroCapId == existsLoaiTroCapCauHinh.TroCapId).ToList();
                            var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();

                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                var listDieuKienHuongId = context.LuongCtDieuKienTroCapKhac.Where(x =>
                                        x.LuongCtLoaiTroCapKhacId == existsData.LuongCtLoaiTroCapKhacId)
                                    .Select(y => y.DieuKienHuongId).ToList();

                                var countFalse = context.LuongCtEmpDktcKhac.Count(x =>
                                    x.EmployeeId == item.EmployeeId &&
                                    x.KyLuongId == kyLuongId &&
                                    listDieuKienHuongId.Contains(x.DieuKienHuongId) &&
                                    x.DuDieuKien == false);

                                //Nếu tất cả điều kiện hưởng đều = true
                                if (countFalse == 0)
                                {
                                    existsData.MucTroCap = existsLoaiTroCapCauHinh.MucTroCap;
                                }
                                //Nếu có điều kiện hưởng = false
                                else
                                {
                                    existsData.MucTroCap = 0;
                                }
                            }
                            //Nếu không đủ điều kiện Chức vụ hoặc Loại hợp đồng trong cấu hình
                            else
                            {
                                existsData.MucTroCap = 0;

                                //Xóa điều kiện hưởng
                                var newListDieuKienHuong = context.LuongCtDieuKienTroCapKhac.Where(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapKhacId == existsData.LuongCtLoaiTroCapKhacId).ToList();

                                context.LuongCtDieuKienTroCapKhac.RemoveRange(newListDieuKienHuong);
                            }

                            context.LuongCtLoaiTroCapKhac.Update(existsData);
                            context.SaveChanges();
                        }
                        //Nếu không có loại trợ cấp trong cấu hình
                        else
                        {
                            //Xóa điều kiện hưởng đã lưu nếu có
                            var listDataDieuKienHuong = listOldLuongCtDieuKienTroCapKhac.Where(x =>
                                x.LuongCtLoaiTroCapKhacId == existsData.LuongCtLoaiTroCapKhacId).ToList();
                            context.LuongCtDieuKienTroCapKhac.RemoveRange(listDataDieuKienHuong);
                            context.SaveChanges();

                            existsData.MucTroCap = 0;
                            context.LuongCtLoaiTroCapKhac.Update(existsData);
                            context.SaveChanges();
                        }
                    }
                }

                var listRemoveLuongCtLoaiTroCapKhac = new List<LuongCtLoaiTroCapKhac>();
                var listRemoveLuongCtDieuKienTroCapKhac = new List<LuongCtDieuKienTroCapKhac>();
                listOldLuongCtLoaiTroCapKhac.ForEach(loaiTroCapData =>
                {
                    var existsLoaiTroCap =
                        listLoaiTroCapKhac.FirstOrDefault(x => x.CategoryId == loaiTroCapData.LoaiTroCapId);

                    //Nếu có trong data nhưng không có trong cấu hình được chọn
                    if (existsLoaiTroCap == null)
                    {
                        listRemoveLuongCtLoaiTroCapKhac.Add(loaiTroCapData);

                        var LuongCtDieuKienTroCapKhac = listOldLuongCtDieuKienTroCapKhac.Where(x =>
                            x.LuongCtLoaiTroCapKhacId == loaiTroCapData.LuongCtLoaiTroCapKhacId).ToList();
                        listRemoveLuongCtDieuKienTroCapKhac.AddRange(LuongCtDieuKienTroCapKhac);
                    }
                });

                context.LuongCtLoaiTroCapKhac.RemoveRange(listRemoveLuongCtLoaiTroCapKhac);
                context.LuongCtDieuKienTroCapKhac.RemoveRange(listRemoveLuongCtDieuKienTroCapKhac);
                context.SaveChanges();
            }

            #endregion
        }

        private void SaveLuongCt_LoaiTroCapKhac_DieuKienTroCapKhac(Guid employeeId, TroCap troCap, List<TroCapChucVuMapping> listTroCapChucVu, 
            List<TroCapLoaiHopDongMapping> listTroCapLoaiHopDong, List<TroCapDieuKienHuongMapping> listTroCapDieuKienHuong,
            int kyLuongId, LuongCtTroCapKhac luongCtTroCapKhac, Guid? positionId, Guid? loaiHopDongId, Guid loaiTroCapId)
        {
            //Nếu loại trợ cấp có cấu hình
            if (troCap != null)
            {
                var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                var listTcLoaiHopDong = listTroCapLoaiHopDong.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                var listTcDieuKienHuong =
                    listTroCapDieuKienHuong.Where(x => x.TroCapId == troCap.TroCapId).ToList();

                var luongCtLoaiTroCapKhac = new LuongCtLoaiTroCapKhac();
                luongCtLoaiTroCapKhac.KyLuongId = kyLuongId;
                luongCtLoaiTroCapKhac.LuongCtTroCapKhacId = luongCtTroCapKhac.LuongCtTroCapKhacId;
                luongCtLoaiTroCapKhac.LoaiTroCapId = troCap.LoaiTroCapId;
                luongCtLoaiTroCapKhac.IsEdit = false;

                if (listChucVuId.Contains(positionId ?? Guid.Empty) &&
                    listLoaiHopDongId.Contains(loaiHopDongId ?? Guid.Empty))
                {
                    luongCtLoaiTroCapKhac.MucTroCap = troCap.MucTroCap;

                    context.LuongCtLoaiTroCapKhac.Add(luongCtLoaiTroCapKhac);
                    context.SaveChanges();

                    listTcDieuKienHuong.ForEach(dieuKienHuong =>
                    {
                        var luongCtDieuKienTroCapKhac = new LuongCtDieuKienTroCapKhac();
                        luongCtDieuKienTroCapKhac.KyLuongId = kyLuongId;
                        luongCtDieuKienTroCapKhac.LuongCtTroCapKhacId = luongCtTroCapKhac.LuongCtTroCapKhacId;
                        luongCtDieuKienTroCapKhac.LuongCtLoaiTroCapKhacId = luongCtLoaiTroCapKhac.LuongCtLoaiTroCapKhacId;
                        luongCtDieuKienTroCapKhac.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;

                        context.LuongCtDieuKienTroCapKhac.Add(luongCtDieuKienTroCapKhac);
                        context.SaveChanges();

                        SaveLuongCtEmpDktcKhac(employeeId, kyLuongId, dieuKienHuong.DieuKienHuongId);
                    });
                }
                //Nếu không thỏa mãn điều kiện Chức vụ hoặc Hợp đồng
                else
                {
                    luongCtLoaiTroCapKhac.MucTroCap = 0;

                    context.LuongCtLoaiTroCapKhac.Add(luongCtLoaiTroCapKhac);
                    context.SaveChanges();
                }
            }
            //Nếu loại trợ cấp không có cấu hình
            else
            {
                var luongCtLoaiTroCapKhac = new LuongCtLoaiTroCapKhac();
                luongCtLoaiTroCapKhac.KyLuongId = kyLuongId;
                luongCtLoaiTroCapKhac.LuongCtTroCapKhacId = luongCtTroCapKhac.LuongCtTroCapKhacId;
                luongCtLoaiTroCapKhac.LoaiTroCapId = loaiTroCapId;
                luongCtLoaiTroCapKhac.MucTroCap = 0;
                luongCtLoaiTroCapKhac.IsEdit = true;

                context.LuongCtLoaiTroCapKhac.Add(luongCtLoaiTroCapKhac);
                context.SaveChanges();
            }
        }

        private void SaveLuongCtEmpDktcKhac(Guid employeeId, int kyLuongId, Guid dieuKienHuongId, bool? duDieuKien = null)
        {
            var exists = context.LuongCtEmpDktcKhac.FirstOrDefault(x => x.EmployeeId == employeeId &&
                                                                        x.KyLuongId == kyLuongId &&
                                                                        x.DieuKienHuongId == dieuKienHuongId);

            //Update
            if (exists != null && duDieuKien != null)
            {
                exists.DuDieuKien = duDieuKien.Value;

                context.LuongCtEmpDktcKhac.Update(exists);
                context.SaveChanges();
            }
            //Create
            else if (exists == null)
            {
                var luongCtEmpDktcKhac = new LuongCtEmpDktcKhac();
                luongCtEmpDktcKhac.EmployeeId = employeeId;
                luongCtEmpDktcKhac.KyLuongId = kyLuongId;
                luongCtEmpDktcKhac.DieuKienHuongId = dieuKienHuongId;
                luongCtEmpDktcKhac.DuDieuKien = true;

                context.LuongCtEmpDktcKhac.Add(luongCtEmpDktcKhac);
                context.SaveChanges();
            }
        }

        private decimal TinhSoTienTroCapDmvsDuocHuong(decimal mucTroCap, MucHuongDmvs mucHuong, int soLanDmvs)
        {
            decimal result = 0;

            //Nếu trừ theo số lần
            if (mucHuong.HinhThucTru == 1)
            {
                result = mucHuong.MucTru * soLanDmvs;
            }
            //Nếu trừ theo phần trăm mức trợ cấp
            else if (mucHuong.HinhThucTru == 2)
            {
                result = mucHuong.MucTru * mucTroCap / 100;
            }

            return mucTroCap - Math.Round(result, 0);
        }

        private void SaveLuongCt_TroCapOt(int kyLuongId, decimal soNgayLamViec, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Xóa dữ liệu cũ

            var listOldLuongCtTroCapOt = context.LuongCtTroCapOt.Where(x => x.KyLuongId == kyLuongId).ToList();
            context.LuongCtTroCapOt.RemoveRange(listOldLuongCtTroCapOt);
            var listOldLuongCtLoaiTroCapOt = context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == kyLuongId).ToList();
            context.LuongCtLoaiTroCapOt.RemoveRange(listOldLuongCtLoaiTroCapOt);

            context.SaveChanges();

            #endregion

            var listTongHopChamCongOt = context.TongHopChamCongOt.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                                             x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listTongHopChamCongOt.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeCode).ToList();
            var listCauHinhOt = context.CauHinhOt.ToList();

            for (int e = 0; e < listEmp.Count; e++)
            {
                var item = listEmp[e];

                decimal mucLuongHienTai = CommonHelper.GetMucLuongHienTaiByEmployeeId(context, item.EmployeeId);

                var luongCtTroCapOt = new LuongCtTroCapOt();
                luongCtTroCapOt.KyLuongId = kyLuongId;
                luongCtTroCapOt.EmployeeId = item.EmployeeId;
                luongCtTroCapOt.OrganizationId = item.OrganizationId ?? Guid.Empty;
                luongCtTroCapOt.PositionId = item.PositionId ?? Guid.Empty;
                luongCtTroCapOt.MucLuongHienTai = Math.Round(mucLuongHienTai / soNgayLamViec / 8, 0);

                context.LuongCtTroCapOt.Add(luongCtTroCapOt);
                context.SaveChanges();

                var listTongHopChamCongOtByEmp =
                    listTongHopChamCongOt.Where(x => x.EmployeeId == item.EmployeeId).ToList();

                var listLoaiOtId = listTongHopChamCongOtByEmp.Select(y => y.LoaiOtId).Distinct().ToList();

                listLoaiOtId.ForEach(loaiOtId =>
                {
                    var cauHinhTheoLoaiOt = listCauHinhOt.FirstOrDefault(x => x.LoaiOtId == loaiOtId);
                    decimal phanTram = cauHinhTheoLoaiOt?.TyLeHuong ?? 0;
                    decimal soGioOt = listTongHopChamCongOtByEmp.Where(x => x.LoaiOtId == loaiOtId).Sum(s => s.SoGio);

                    var luongCtLoaiTroCapOt = new LuongCtLoaiTroCapOt();
                    luongCtLoaiTroCapOt.LuongCtTroCapOtId = luongCtTroCapOt.LuongCtTroCapOtId;
                    luongCtLoaiTroCapOt.KyLuongId = kyLuongId;
                    luongCtLoaiTroCapOt.LoaiOtId = loaiOtId;
                    luongCtLoaiTroCapOt.MucTroCap = Math.Round(luongCtTroCapOt.MucLuongHienTai * phanTram / 100, 0);
                    luongCtLoaiTroCapOt.SoGioOt = soGioOt;

                    context.LuongCtLoaiTroCapOt.Add(luongCtLoaiTroCapOt);
                    context.SaveChanges();
                });
            }
        }

        private void SaveLuongCt_ThuNhapTinhThue(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtThuNhapTinhThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new {Emp = emp, User = user})
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();

            listEmp.ForEach(item =>
            {
                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                //Create
                if (exists == null)
                {
                    var luongCtThuNhapTinhThue = new LuongCtThuNhapTinhThue();
                    luongCtThuNhapTinhThue.KyLuongId = kyLuongId;
                    luongCtThuNhapTinhThue.EmployeeId = item.Emp.EmployeeId;
                    luongCtThuNhapTinhThue.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    luongCtThuNhapTinhThue.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    luongCtThuNhapTinhThue.NetToGross = 0;
                    luongCtThuNhapTinhThue.Month13 = 0;
                    luongCtThuNhapTinhThue.Gift = 0;
                    luongCtThuNhapTinhThue.Other = 0;

                    context.LuongCtThuNhapTinhThue.Add(luongCtThuNhapTinhThue);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;

                    context.LuongCtThuNhapTinhThue.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_BaoHiem(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
            var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();
            var listHopDongNhanSu = context.HopDongNhanSu.Where(x => listEmpId.Contains(x.EmployeeId)).ToList();
            var cauHinhBaoHiem = context.CauHinhBaoHiem.FirstOrDefault();

            listEmp.ForEach(item =>
            {
                var ngayLamViecThucTe = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId)
                                            ?.NgayLamViecThucTe ?? 0;
                decimal mucLuongHienTai = CommonHelper.GetMucLuongHienTaiByEmployeeId(context, item.Emp.EmployeeId);
                var hopDongMoiNhat = listHopDongNhanSu.Where(x => x.EmployeeId == item.Emp.EmployeeId)
                    .OrderByDescending(z => z.NgayBatDauLamViec).FirstOrDefault();
                var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == hopDongMoiNhat?.LoaiHopDongId);

                var luongCtBaoHiem = new LuongCtBaoHiem();
                luongCtBaoHiem.KyLuongId = kyLuongId;
                luongCtBaoHiem.EmployeeId = item.Emp.EmployeeId;
                luongCtBaoHiem.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongCtBaoHiem.PositionId = item.Emp.PositionId ?? Guid.Empty;

                #region Lương cơ bản đóng BHXH

                decimal baseBhxh = 0;
                if (loaiHopDong != null)
                {
                    if (ngayLamViecThucTe <= 14 || !loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        baseBhxh = 0;
                    }
                    else if (ngayLamViecThucTe > 14 && loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        if (cauHinhBaoHiem != null && mucLuongHienTai >= cauHinhBaoHiem.MucDongToiDa)
                        {
                            baseBhxh = cauHinhBaoHiem.MucDongToiDa;
                        }
                        else
                        {
                            baseBhxh = mucLuongHienTai;
                        }
                    }
                }

                luongCtBaoHiem.BaseBhxh = baseBhxh;

                #endregion

                #region Lương cơ bản đóng BHTN

                decimal baseBhtn = 0;
                if (loaiHopDong != null)
                {
                    if (ngayLamViecThucTe <= 14 || !loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        baseBhtn = 0;
                    }
                    else if (ngayLamViecThucTe > 14 && loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        if (cauHinhBaoHiem != null && mucLuongHienTai >= cauHinhBaoHiem.MucDongToiDaBHTN)
                        {
                            baseBhtn = cauHinhBaoHiem.MucDongToiDaBHTN;
                        }
                        else
                        {
                            baseBhtn = mucLuongHienTai;
                        }
                    }
                }

                luongCtBaoHiem.BaseBhtn = baseBhtn;

                #endregion

                #region Lương đóng BHXH

                decimal bhxh = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhxh = luongCtBaoHiem.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNld / 100;
                }

                luongCtBaoHiem.Bhxh = bhxh;

                #endregion

                #region Lương đóng BHYT

                decimal bhyt = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhyt = luongCtBaoHiem.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNld / 100;
                }

                luongCtBaoHiem.Bhyt = bhyt;

                #endregion

                #region Lương đóng BHTN

                decimal bhtn = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhtn = luongCtBaoHiem.BaseBhtn * cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNld / 100;
                }

                luongCtBaoHiem.Bhtn = bhtn;

                #endregion

                #region Lương đóng BHTNNN

                decimal bhtnnn = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhtnnn = luongCtBaoHiem.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNld / 100;
                }

                luongCtBaoHiem.Bhtnnn = bhtnnn;

                #endregion

                luongCtBaoHiem.Other = 0;

                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                //Create
                if (exists == null)
                {
                    context.LuongCtBaoHiem.Add(luongCtBaoHiem);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    exists.BaseBhxh = baseBhxh;
                    exists.BaseBhtn = baseBhtn;
                    exists.Bhxh = bhxh;
                    exists.Bhyt = bhyt;
                    exists.Bhtn = bhtn;
                    exists.Bhtnnn = bhtnnn;

                    context.LuongCtBaoHiem.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_GiamTruTruocThue(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtGiamTruTruocThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var cauHinhGiamTru = context.CauHinhGiamTru.ToList();
            var cauHinhGiamTruBanThan = cauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 1);
            var cauHinhGiamTruNguoiPhuThuoc = cauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 2);
            var listContact = context.Contact.Where(x => x.ObjectType == ContactObjectType.EMP_CON).ToList();
            var listLuongCtBaoHiem = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listAllHdns = context.HopDongNhanSu.Where(x => listEmpId.Contains(x.EmployeeId))
                .OrderByDescending(z => z.NgayKyHopDong).ToList();

            var loaiHDCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS").CategoryTypeId;
            var listLoaiHd = context.Category.Where(x => x.CategoryTypeId == loaiHDCateTypeId).ToList();

            listEmp.ForEach(item =>
            {
                decimal mucGiamTruBanThan = cauHinhGiamTruBanThan?.MucGiamTru ?? 0;
                decimal mucGiamTruNguoiPhuThuoc = cauHinhGiamTruNguoiPhuThuoc?.MucGiamTru ?? 0;

                var luongCtGiamTruTruocThue = new LuongCtGiamTruTruocThue();
                luongCtGiamTruTruocThue.KyLuongId = kyLuongId;
                luongCtGiamTruTruocThue.EmployeeId = item.Emp.EmployeeId;
                luongCtGiamTruTruocThue.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongCtGiamTruTruocThue.PositionId = item.Emp.PositionId ?? Guid.Empty;
                luongCtGiamTruTruocThue.GiamTruCaNhan = mucGiamTruBanThan;
                luongCtGiamTruTruocThue.GiamTruKhac = 0;

                var listNguoiPhuThuoc = listContact.Where(x =>
                    x.ObjectId == item.Emp.EmployeeId && x.QuanHeId != null &&
                    x.PhuThuoc == true &&
                    x.PhuThuocTuNgay != null).ToList();

                decimal soNguoiPhuThuoc = 0;
                listNguoiPhuThuoc.ForEach(nguoiPhuThuoc =>
                {
                    if (nguoiPhuThuoc.PhuThuocDenNgay == null)
                    {
                        if (nguoiPhuThuoc.PhuThuocTuNgay.Value.Year < DateTime.Now.Year ||
                            (nguoiPhuThuoc.PhuThuocTuNgay.Value.Year == DateTime.Now.Year &&
                             nguoiPhuThuoc.PhuThuocTuNgay.Value.Month <= DateTime.Now.Month)) soNguoiPhuThuoc++;
                    }
                    else
                    {
                        var tuNgay = new DateTime(nguoiPhuThuoc.PhuThuocTuNgay.Value.Year,
                            nguoiPhuThuoc.PhuThuocTuNgay.Value.Month, 1);
                        var denNgay = new DateTime(nguoiPhuThuoc.PhuThuocDenNgay.Value.Year,
                            nguoiPhuThuoc.PhuThuocDenNgay.Value.Month, 1);
                        var ngayHienTai = new DateTime(DateTime.Now.Year,
                            DateTime.Now.Month, 1);

                        if (tuNgay.Ticks <= ngayHienTai.Ticks && ngayHienTai.Ticks <= denNgay.Ticks) soNguoiPhuThuoc++;
                    }
                });

                luongCtGiamTruTruocThue.SoNguoiPhuThuoc = (int) soNguoiPhuThuoc;
                decimal giamTruNguoiPhuThuoc = soNguoiPhuThuoc * mucGiamTruNguoiPhuThuoc;
                luongCtGiamTruTruocThue.GiamTruNguoiPhuThuoc = giamTruNguoiPhuThuoc;

                decimal tienDongBaoHiem = listLuongCtBaoHiem.Where(x => x.EmployeeId == item.Emp.EmployeeId)
                    .Sum(s => s.BaseBhxh + s.Bhxh + s.Bhyt + s.Bhtn + s.Bhtnnn + s.Other);
                luongCtGiamTruTruocThue.TienDongBaoHiem = tienDongBaoHiem;

                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                //Lấy hợp đồng mới nhất của emp 
                var hopDongEmp = listAllHdns.Where(x => x.EmployeeId == item.Emp.EmployeeId).OrderByDescending(z => z.NgayKyHopDong).FirstOrDefault();
                if (hopDongEmp != null)
                {
                    var hdongMoiNhatId = hopDongEmp.LoaiHopDongId;
                    var loaiHopDong = listLoaiHd.FirstOrDefault(x => x.CategoryId == hdongMoiNhatId)?.CategoryCode;
                    if (loaiHopDong.Contains("HĐTV"))
                    {
                        if (exists == null)
                        {
                            luongCtGiamTruTruocThue.GiamTruCaNhan = 0;
                            luongCtGiamTruTruocThue.GiamTruNguoiPhuThuoc = 0;
                        }
                        //Update
                        else
                        {
                            mucGiamTruBanThan = 0;
                            giamTruNguoiPhuThuoc = 0;
                        }
                    }
                }

                //Create
                if (exists == null)
                {
                    context.LuongCtGiamTruTruocThue.Add(luongCtGiamTruTruocThue);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    exists.GiamTruCaNhan = mucGiamTruBanThan;
                    exists.GiamTruNguoiPhuThuoc = giamTruNguoiPhuThuoc;
                    exists.SoNguoiPhuThuoc = (int) soNguoiPhuThuoc;
                    exists.TienDongBaoHiem = tienDongBaoHiem;

                    context.LuongCtGiamTruTruocThue.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_GiamTruSauThue(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtGiamTruSauThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var listLuongCtBaoHiem = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuongId).ToList();
            var cauHinhPhiCongDoan = context.KinhPhiCongDoan.FirstOrDefault();
            var mucDongCuaNhanVien = cauHinhPhiCongDoan?.MucDongNhanVien ?? 0;


            listEmp.ForEach(item =>
            {
                decimal baseBhxh = listLuongCtBaoHiem.
                                   FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId)?.BaseBhxh ?? 0;

                var luongCtGiamTruSauThue = new LuongCtGiamTruSauThue();
                luongCtGiamTruSauThue.KyLuongId = kyLuongId;
                luongCtGiamTruSauThue.EmployeeId = item.Emp.EmployeeId;
                luongCtGiamTruSauThue.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongCtGiamTruSauThue.PositionId = item.Emp.PositionId ?? Guid.Empty;

                decimal kinhPhiCongDoan = Math.Round(baseBhxh * mucDongCuaNhanVien / 100, 0);
                luongCtGiamTruSauThue.KinhPhiCongDoan = kinhPhiCongDoan;

                luongCtGiamTruSauThue.QuyetToanThueTncn = 0;
                luongCtGiamTruSauThue.Other = 0;

                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                //Create
                if (exists == null)
                {
                    context.LuongCtGiamTruSauThue.Add(luongCtGiamTruSauThue);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    exists.KinhPhiCongDoan = kinhPhiCongDoan;

                    context.LuongCtGiamTruSauThue.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_HoanLaiSauThue(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtHoanLaiSauThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();

            listEmp.ForEach(item =>
            {
                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                //Create
                if (exists == null)
                {
                    var luongCtHoanLaiSauThue = new LuongCtHoanLaiSauThue();
                    luongCtHoanLaiSauThue.KyLuongId = kyLuongId;
                    luongCtHoanLaiSauThue.EmployeeId = item.Emp.EmployeeId;
                    luongCtHoanLaiSauThue.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    luongCtHoanLaiSauThue.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    luongCtHoanLaiSauThue.ThueTncn = 0;
                    luongCtHoanLaiSauThue.Other = 0;

                    context.LuongCtHoanLaiSauThue.Add(luongCtHoanLaiSauThue);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;

                    context.LuongCtHoanLaiSauThue.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_CtyDong(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
            var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();
            var listHopDongNhanSu = context.HopDongNhanSu.Where(x => listEmpId.Contains(x.EmployeeId)).ToList();
            var cauHinhBaoHiem = context.CauHinhBaoHiem.FirstOrDefault();
            var cauHinhPhiCongDoan = context.KinhPhiCongDoan.FirstOrDefault();
            var mucDongCuaCty = cauHinhPhiCongDoan?.MucDongCongTy ?? 0;

            listEmp.ForEach(item =>
            {
                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                var ngayLamViecThucTe = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId)
                                            ?.NgayLamViecThucTe ?? 0;
                decimal mucLuongHienTai = CommonHelper.GetMucLuongHienTaiByEmployeeId(context, item.Emp.EmployeeId);
                var hopDongMoiNhat = listHopDongNhanSu.Where(x => x.EmployeeId == item.Emp.EmployeeId)
                    .OrderByDescending(z => z.NgayBatDauLamViec).FirstOrDefault();
                var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == hopDongMoiNhat?.LoaiHopDongId);

                var luongCtCtyDong = new LuongCtCtyDong();
                luongCtCtyDong.KyLuongId = kyLuongId;
                luongCtCtyDong.EmployeeId = item.Emp.EmployeeId;
                luongCtCtyDong.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongCtCtyDong.PositionId = item.Emp.OrganizationId ?? Guid.Empty;

                #region Lương cơ bản đóng BHXH

                decimal baseBhxh = 0;
                if (loaiHopDong != null)
                {
                    if (ngayLamViecThucTe <= 14 || !loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        baseBhxh = 0;
                    }
                    else if (ngayLamViecThucTe > 14 && loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        if (cauHinhBaoHiem != null && mucLuongHienTai >= cauHinhBaoHiem.MucDongToiDa)
                        {
                            baseBhxh = cauHinhBaoHiem.MucDongToiDa;
                        }
                        else
                        {
                            baseBhxh = mucLuongHienTai;
                        }
                    }
                }

                luongCtCtyDong.BaseBhxh = baseBhxh;

                #endregion

                #region Lương cơ bản đóng BHTN

                decimal baseBhtn = 0;
                if (loaiHopDong != null)
                {
                    if (ngayLamViecThucTe <= 14 || !loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        baseBhtn = 0;
                    }
                    else if (ngayLamViecThucTe > 14 && loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        if (cauHinhBaoHiem != null && mucLuongHienTai >= cauHinhBaoHiem.MucDongToiDaBHTN)
                        {
                            baseBhtn = cauHinhBaoHiem.MucDongToiDaBHTN;
                        }
                        else
                        {
                            baseBhtn = mucLuongHienTai;
                        }
                    }
                }

                luongCtCtyDong.BaseBhtn = baseBhtn;

                #endregion

                #region Lương đóng BHXH

                decimal bhxh = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhxh = luongCtCtyDong.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNsdld / 100;
                }

                luongCtCtyDong.Bhxh = bhxh;

                #endregion

                #region Lương đóng BHYT

                decimal bhyt = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhyt = luongCtCtyDong.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNsdld / 100;
                }

                luongCtCtyDong.Bhyt = bhyt;

                #endregion

                #region Lương đóng BHTN

                decimal bhtn = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhtn = luongCtCtyDong.BaseBhtn * cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNsdld / 100;
                }

                luongCtCtyDong.Bhtn = bhtn;

                #endregion

                #region Lương đóng BHTNNN

                decimal bhtnnn = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhtnnn = luongCtCtyDong.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNsdld / 100;
                }

                luongCtCtyDong.Bhtnnn = bhtnnn;

                #endregion

                #region Kinh phí công đoàn

                decimal kinhPhiCongDoan = luongCtCtyDong.BaseBhxh * mucDongCuaCty / 100;
                luongCtCtyDong.KinhPhiCongDoan = kinhPhiCongDoan;

                #endregion

                luongCtCtyDong.Other = 0;
                luongCtCtyDong.FundOct = 0;

                //Create
                if (exists == null)
                {
                    context.LuongCtCtyDong.Add(luongCtCtyDong);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    exists.BaseBhxh = baseBhxh;
                    exists.BaseBhtn = baseBhtn;
                    exists.Bhxh = bhxh;
                    exists.Bhyt = bhyt;
                    exists.Bhtn = bhtn;
                    exists.Bhtnnn = bhtnnn;
                    exists.KinhPhiCongDoan = kinhPhiCongDoan;

                    context.LuongCtCtyDong.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_Other(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtOther.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();

            listEmp.ForEach(item =>
            {
                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                if (exists == null)
                {
                    var luongCtOther = new LuongCtOther();
                    luongCtOther.KyLuongId = kyLuongId;
                    luongCtOther.EmployeeId = item.Emp.EmployeeId;
                    luongCtOther.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    luongCtOther.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    luongCtOther.KhoanBuTruThangTruoc = 0;
                    luongCtOther.TroCapKhacKhongTinhThue = 0;
                    luongCtOther.KhauTruHoanLaiTruocThue = 0;
                    luongCtOther.LuongTamUng = 0;

                    context.LuongCtOther.Add(luongCtOther);
                }
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;

                    context.LuongCtOther.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongTongHop(int kyLuongId, decimal soNgayLamViec, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Xóa dữ liệu cũ

            var listOld = context.LuongTongHop.Where(x => x.KyLuongId == kyLuongId).ToList();

            context.LuongTongHop.RemoveRange(listOld);
            context.SaveChanges();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtOther = context.LuongCtOther.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtTroCapOt = context.LuongCtTroCapOt.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtLoaiTroCapOt = context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtTroCapCoDinh = context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtTroCapKhac = context.LuongCtTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listCauHinhThueTncn = context.CauHinhThueTncn.ToList();
            var cauHinhBaoHiem = context.CauHinhBaoHiem.FirstOrDefault();
            var listLuongCtBaoHiem = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtCtyDong = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtGiamTruSauThue = context.LuongCtGiamTruSauThue.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtHoanLaiSauThue = context.LuongCtHoanLaiSauThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            var listAllHdns = context.HopDongNhanSu.Where(x => listEmpId.Contains(x.EmployeeId))
                .OrderByDescending(z => z.NgayKyHopDong).ToList();

            var loaiHDCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS").CategoryTypeId;
            var listLoaiHd = context.Category.Where(x => x.CategoryTypeId == loaiHDCateTypeId).ToList();

            var codeHD = "";
            listEmp.ForEach(item =>
            {
                //Lấy hợp đồng mới nhất của emp 
                var hopDongEmp = listAllHdns.Where(x => x.EmployeeId == item.Emp.EmployeeId)
                    .OrderByDescending(z => z.NgayKyHopDong).FirstOrDefault();
                if (hopDongEmp != null)
                {
                    var hdongMoiNhatId = hopDongEmp.LoaiHopDongId;
                    var loaiHopDong = listLoaiHd.FirstOrDefault(x => x.CategoryId == hdongMoiNhatId)?.CategoryCode;
                    if (loaiHopDong.Contains("HĐTV"))
                    {
                        codeHD = "HĐTV";
                    }
                    if (loaiHopDong.Contains("HÐLÐ"))
                    {
                        codeHD = "HÐLÐ";
                    }
                }

                var tongHopChamCong = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                var luongTongHop = new LuongTongHop();
                luongTongHop.KyLuongId = kyLuongId;
                luongTongHop.EmployeeId = item.Emp.EmployeeId;
                luongTongHop.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongTongHop.PositionId = item.Emp.PositionId ?? Guid.Empty;
                luongTongHop.SubCode1Value = item.Emp.SubCode1Value;
                luongTongHop.SubCode2Value = item.Emp.SubCode2Value;
                luongTongHop.CapBacId = item.Emp.CapBacId;

                #region Tổng ngày tính lương

                if (tongHopChamCong == null) luongTongHop.TongNgayTinhLuong = 0;
                else
                {
                    luongTongHop.TongNgayTinhLuong =
                        tongHopChamCong.NgayLamViecThucTe + tongHopChamCong.CongTac + tongHopChamCong.DaoTaoHoiThao +
                        tongHopChamCong.NghiPhep + tongHopChamCong.NghiLe + tongHopChamCong.NghiBu +
                        tongHopChamCong.NghiCheDo + tongHopChamCong.NghiHuongLuongKhac - 
                        tongHopChamCong.TongNgayDmvs;
                }

                #endregion

                #region Mức lương cũ

                luongTongHop.MucLuongCu =
                    CommonHelper.GetMucLuongCuByEmployeeId(context, item.Emp.EmployeeId, ngayBatDau);

                #endregion

                #region Số ngày tính theo mức lương cũ

                luongTongHop.SoNgayMucLuongCu =
                    CommonHelper.GetSoNgayTinhTheoMucLuongCu(context, item.Emp.EmployeeId, ngayBatDau);

                #endregion

                #region Mức lương hiện tại

                luongTongHop.MucLuongHienTai =
                    CommonHelper.GetMucLuongHienTaiByEmployeeId(context, item.Emp.EmployeeId);

                #endregion

                #region Lương thực tế

                luongTongHop.LuongThucTe =
                    luongTongHop.MucLuongHienTai / soNgayLamViec * (luongTongHop.TongNgayTinhLuong - luongTongHop.SoNgayMucLuongCu) +
                    luongTongHop.MucLuongCu * luongTongHop.SoNgayMucLuongCu;

                #endregion

                #region Khoản bù trừ tháng trước

                var luongCtOther = listLuongCtOther.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.KhoanBuTruThangTruoc = luongCtOther?.KhoanBuTruThangTruoc ?? 0;

                #endregion

                #region Lương OT tính thuế

                var luongCtTroCapOt = listLuongCtTroCapOt.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                if (luongCtTroCapOt == null) luongTongHop.LuongOtTinhThue = 0;
                else
                {
                    var _listLuongCtLoaiTroCapOt = listLuongCtLoaiTroCapOt
                        .Where(x => x.LuongCtTroCapOtId == luongCtTroCapOt.LuongCtTroCapOtId).ToList();

                    luongTongHop.LuongOtTinhThue = LuongOtTinhThue(luongCtTroCapOt, _listLuongCtLoaiTroCapOt);
                }

                #endregion

                #region Lương OT không tính thuế

                if (luongCtTroCapOt == null) luongTongHop.LuongOtKhongTinhThue = 0;
                else 
                {
                    var _listLuongCtLoaiTroCapOt = listLuongCtLoaiTroCapOt
                        .Where(x => x.LuongCtTroCapOtId == luongCtTroCapOt.LuongCtTroCapOtId).ToList();

                    var tongLuongOt = TongLuongOt(_listLuongCtLoaiTroCapOt);
                    luongTongHop.LuongOtKhongTinhThue = tongLuongOt - luongTongHop.LuongOtTinhThue;
                }

                #endregion

                #region Tổng trợ cấp cố định

                var luongCtTroCapCoDinh =
                    listLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                if (luongCtTroCapCoDinh == null) luongTongHop.TongTroCapCoDinh = 0;
                else
                {
                    var _listLuongCtLoaiTroCapCoDinh = listLuongCtLoaiTroCapCoDinh
                        .Where(x => x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId).ToList();
                    luongTongHop.TongTroCapCoDinh = TongTroCapCoDinh(_listLuongCtLoaiTroCapCoDinh);
                }

                #endregion

                #region Tổng trợ cấp khác

                var luongCtTroCapKhac =
                    listLuongCtTroCapKhac.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                if (luongCtTroCapKhac == null) luongTongHop.TongTroCapKhac = 0;
                else
                {
                    var _listLuongCtLoaiTroCapKhac = listLuongCtLoaiTroCapKhac
                        .Where(x => x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId).ToList();
                    luongTongHop.TongTroCapKhac = TongTroCapKhac(_listLuongCtLoaiTroCapKhac);
                }

                #endregion

                #region Trợ cấp khác không tính thuế

                luongTongHop.TroCapKhacKhongTinhThue = luongCtOther?.TroCapKhacKhongTinhThue ?? 0;

                #endregion

                #region Khấu trừ/Hoàn lại trước thuế

                luongTongHop.KhauTruHoanLaiTruocThue = luongCtOther?.KhauTruHoanLaiTruocThue ?? 0;

                #endregion

                #region Tổng thu nhập bao gồm cả các khoản tính thuế và không tính thuế

                luongTongHop.TongThuNhapBaoGomThueVaKhongThue = luongTongHop.LuongThucTe + luongTongHop.KhoanBuTruThangTruoc + luongTongHop.LuongOtTinhThue +
                                                                luongTongHop.LuongOtKhongTinhThue + luongTongHop.TongTroCapCoDinh + luongTongHop.TongTroCapKhac +
                                                                luongTongHop.TroCapKhacKhongTinhThue - luongTongHop.KhauTruHoanLaiTruocThue;

                #endregion

                #region Tổng thu nhập sau khi đã loại bỏ các khoản không chịu thuế

                luongTongHop.TongThuNhapSauKhiBoCacKhoanKhongTinhThue =
                    (luongTongHop.TongThuNhapBaoGomThueVaKhongThue - luongTongHop.TroCapKhacKhongTinhThue -
                     luongTongHop.LuongOtKhongTinhThue) < 0
                        ? 0
                        : (luongTongHop.TongThuNhapBaoGomThueVaKhongThue - luongTongHop.TroCapKhacKhongTinhThue -
                           luongTongHop.LuongOtKhongTinhThue);

                #endregion

                #region Giảm trừ trước thuế

                var luongCtGiamTruTruocThue =
                    listLuongCtGiamTruTruocThue.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.GiamTruTruocThue = TongGiamTruTruocThue(luongCtGiamTruTruocThue);

                #endregion

                #region Thu nhập chỉ đưa vào tính thuế

                var luongCtThuNhapTinhThue =
                    listLuongCtThuNhapTinhThue.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.ThuNhapChiDuaVaoTinhThue = TongThuNhapChiDuaVaoTinhThue(luongCtThuNhapTinhThue);

                #endregion

                #region Tổng thu nhập chịu thuế sau giảm trừ

                luongTongHop.TongThuNhapChiuThueSauGiamTru = luongTongHop.TongThuNhapSauKhiBoCacKhoanKhongTinhThue - luongTongHop.GiamTruTruocThue +
                                                             luongTongHop.ThuNhapChiDuaVaoTinhThue;

                if (codeHD == "HĐTV" && luongTongHop.TongThuNhapChiuThueSauGiamTru < 2000000)
                {
                    luongTongHop.TongThuNhapChiuThueSauGiamTru = 0;
                }
                if (codeHD == "HÐLÐ" && luongTongHop.TongThuNhapChiuThueSauGiamTru < 0)
                {
                    luongTongHop.TongThuNhapChiuThueSauGiamTru = 0;
                }

                #endregion

                #region Tổng thuế TNCN phải trả bởi công ty và NLĐ

                if (listCauHinhThueTncn.Count == 0) luongTongHop.TongThueTncnCtyVaNld = 0;
                else
                {
                    var cauHinh = listCauHinhThueTncn.FirstOrDefault(x =>
                        x.SoTienTu < luongTongHop.TongThuNhapChiuThueSauGiamTru &&
                        x.SoTienDen >= luongTongHop.TongThuNhapChiuThueSauGiamTru);

                    if (cauHinh == null) luongTongHop.TongThueTncnCtyVaNld = 0;
                    else
                    {
                        if (codeHD == "HĐTV")
                        {
                           luongTongHop.TongThueTncnCtyVaNld =
                           Math.Round(luongTongHop.TongThuNhapChiuThueSauGiamTru * 10 / 100, 0);
                        }
                        else
                        {
                            luongTongHop.TongThueTncnCtyVaNld =
                            Math.Round(luongTongHop.TongThuNhapChiuThueSauGiamTru * cauHinh.PhanTramThue / 100 -
                                       cauHinh.SoBiTruTheoCongThuc, 0);
                        }
                    }
                }

                #endregion

                #region Thuế TNCN NLĐ phải trả

                if (listCauHinhThueTncn.Count == 0) luongTongHop.ThueTncnNld = 0;
                else
                {
                    var cauHinh = listCauHinhThueTncn.FirstOrDefault(x =>
                        x.SoTienTu < luongTongHop.TongThuNhapChiuThueSauGiamTru &&
                        x.SoTienDen >= luongTongHop.TongThuNhapChiuThueSauGiamTru);

                    if (cauHinh == null) luongTongHop.ThueTncnNld = 0;
                    else
                    {
                        decimal gross = luongCtThuNhapTinhThue?.NetToGross ?? 0;

                        if (codeHD == "HĐTV")
                        {
                            luongTongHop.ThueTncnNld =
                                Math.Round((luongTongHop.TongThuNhapChiuThueSauGiamTru - gross) * 10 / 100, 0);
                        }
                        else
                        {
                            luongTongHop.ThueTncnNld =
                                Math.Round((luongTongHop.TongThuNhapChiuThueSauGiamTru - gross) * cauHinh.PhanTramThue / 100 -
                                           cauHinh.SoBiTruTheoCongThuc, 0);
                        }
                    }
                }

                #endregion

                #region Lương cơ bản đóng BHXH, BHYT, BHNN, KPCĐ

                if (tongHopChamCong == null) luongTongHop.LuongCoBanDongBh = 0;
                else
                {
                    if (tongHopChamCong.NghiKhongPhep + tongHopChamCong.NghiHuongBhxh +
                        tongHopChamCong.TongNgayDmvs > 14)
                    {
                        luongTongHop.LuongCoBanDongBh = 0;
                    }
                    else
                    {
                        if (cauHinhBaoHiem == null)
                        {
                            luongTongHop.LuongCoBanDongBh = 0;
                        }
                        else if (luongTongHop.MucLuongHienTai >= cauHinhBaoHiem.MucDongToiDa)
                        {
                            luongTongHop.LuongCoBanDongBh = cauHinhBaoHiem.MucDongToiDa;
                        }
                        else if (luongTongHop.MucLuongHienTai < cauHinhBaoHiem.MucDongToiDa)
                        {
                            luongTongHop.LuongCoBanDongBh = luongTongHop.MucLuongHienTai;
                        }
                    }
                }

                #endregion

                #region Tổng tiền BH NLĐ phải đóng

                var luongCtBaoHiem = listLuongCtBaoHiem.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.TongTienBhNldPhaiDong = TongTienBaoHiem(luongCtBaoHiem);

                #endregion

                #region Tổng tiền BH Công ty phải đóng

                var luongCtCtyDong = listLuongCtCtyDong.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                if (luongCtCtyDong == null) luongTongHop.TongTienBhCtyPhaiDong = 0;
                else
                {
                    luongTongHop.TongTienBhCtyPhaiDong = luongCtCtyDong.Bhxh + luongCtCtyDong.Bhyt + luongCtCtyDong.Bhtn + luongCtCtyDong.Bhtnnn +
                                                         luongCtCtyDong.Other;
                }

                #endregion

                #region Thu nhập thực nhận trước khi bù trừ thuế nếu có

                luongTongHop.ThuNhapThucNhanTruocKhiBuTruThue =
                    luongTongHop.TongThuNhapBaoGomThueVaKhongThue - luongTongHop.ThueTncnNld -
                    luongTongHop.TongTienBhNldPhaiDong;

                #endregion

                #region Các khoản giảm trừ sau thuế

                var luongCtGiamTruSauThue =
                    listLuongCtGiamTruSauThue.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.CacKhoanGiamTruSauThue = TongGiamTruSauThue(luongCtGiamTruSauThue);

                #endregion

                #region Các khoản hoàn lại sau thuế

                var luongCtHoanLaiSauThue =
                    listLuongCtHoanLaiSauThue.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.CacKhoanHoanLaiSauThue = TongCacKhoanHoanLaiSauThue(luongCtHoanLaiSauThue);

                #endregion

                #region Thu nhập thực nhận

                luongTongHop.ThuNhapThucNhan = luongTongHop.ThuNhapThucNhanTruocKhiBuTruThue -
                                               luongTongHop.CacKhoanGiamTruSauThue + luongTongHop.CacKhoanHoanLaiSauThue;

                #endregion

                #region Đã trả lần 1 hoặc Tạm ứng

                luongTongHop.LuongTamUng = luongCtOther?.LuongTamUng ?? 0;

                #endregion

                #region Thực nhận trong tháng hoặc lương còn lại

                luongTongHop.LuongConLai = luongTongHop.ThuNhapThucNhan - luongCtOther?.LuongTamUng ?? 0;

                #endregion

                #region Các khoản công ty phải trả khác

                luongTongHop.CacKhoanCtyPhaiTraKhac = TongCacKhoanCtyPhaiDong(luongCtCtyDong);

                #endregion

                #region Tổng chi phí công ty phải trả

                luongTongHop.TongChiPhiCtyPhaiTra =
                    luongTongHop.TongThueTncnCtyVaNld + luongTongHop.TongTienBhCtyPhaiDong +
                    luongTongHop.ThuNhapThucNhanTruocKhiBuTruThue +
                    luongTongHop.CacKhoanGiamTruSauThue - luongTongHop.CacKhoanHoanLaiSauThue +
                    luongTongHop.CacKhoanCtyPhaiTraKhac;

                #endregion

                context.LuongTongHop.Add(luongTongHop);
                context.SaveChanges();
            });
        }

        private decimal LuongOtTinhThue(LuongCtTroCapOt luongCtTroCapOt,
            List<LuongCtLoaiTroCapOt> listLuongCtLoaiTroCapOt)
        {
            decimal result = 0;

            if (luongCtTroCapOt != null)
            {
                result = luongCtTroCapOt.MucLuongHienTai * listLuongCtLoaiTroCapOt.Sum(s => s.SoGioOt);
            }

            return result;
        }

        private decimal TongLuongOt(List<LuongCtLoaiTroCapOt> listLuongCtLoaiTroCapOt)
        {
            decimal result = listLuongCtLoaiTroCapOt.Sum(s => s.MucTroCap * s.SoGioOt);

            return result;
        }

        private decimal TongTroCapCoDinh(List<LuongCtLoaiTroCapCoDinh> listLuongCtLoaiTroCapCoDinh)
        {
            decimal result = listLuongCtLoaiTroCapCoDinh.Sum(s => s.MucTroCap);

            return result;
        }

        private decimal TongTroCapKhac(List<LuongCtLoaiTroCapKhac> listLuongCtLoaiTroCapKhac)
        {
            decimal result = listLuongCtLoaiTroCapKhac.Sum(s => s.MucTroCap);

            return result;
        }

        private decimal TongGiamTruTruocThue(LuongCtGiamTruTruocThue luongCtGiamTruTruocThue)
        {
            decimal result = 0;

            if (luongCtGiamTruTruocThue != null)
            {
                result = luongCtGiamTruTruocThue.GiamTruCaNhan + luongCtGiamTruTruocThue.GiamTruNguoiPhuThuoc +
                         luongCtGiamTruTruocThue.TienDongBaoHiem + luongCtGiamTruTruocThue.GiamTruKhac;
            }

            return result;
        }

        private decimal TongThuNhapChiDuaVaoTinhThue(LuongCtThuNhapTinhThue luongCtThuNhapTinhThue)
        {
            decimal result = 0;

            if (luongCtThuNhapTinhThue != null)
            {
                result = luongCtThuNhapTinhThue.NetToGross + luongCtThuNhapTinhThue.Month13 +
                         luongCtThuNhapTinhThue.Gift + luongCtThuNhapTinhThue.Other;
            }

            return result;
        }

        private decimal TongTienBaoHiem(LuongCtBaoHiem luongCtBaoHiem)
        {
            decimal result = 0;

            if (luongCtBaoHiem != null)
            {
                result = luongCtBaoHiem.Bhxh +
                         luongCtBaoHiem.Bhyt + luongCtBaoHiem.Bhtn + luongCtBaoHiem.Bhtnnn + luongCtBaoHiem.Other;
            }

            return result;
        }

        private decimal TongCacKhoanCtyPhaiDong(LuongCtCtyDong luongCtCtyDong)
        {
            decimal result = 0;

            if (luongCtCtyDong != null)
            {
                result = luongCtCtyDong.Bhxh + luongCtCtyDong.Bhyt + luongCtCtyDong.Bhtn + luongCtCtyDong.Bhtnnn + 
                         luongCtCtyDong.KinhPhiCongDoan + luongCtCtyDong.Other + luongCtCtyDong.FundOct;
            }

            return result;
        }

        private decimal TongGiamTruSauThue(LuongCtGiamTruSauThue luongCtGiamTruSauThue)
        {
            decimal result = 0;

            if (luongCtGiamTruSauThue != null)
            {
                result = luongCtGiamTruSauThue.KinhPhiCongDoan + luongCtGiamTruSauThue.QuyetToanThueTncn +
                         luongCtGiamTruSauThue.Other;
            }

            return result;
        }

        private decimal TongCacKhoanHoanLaiSauThue(LuongCtHoanLaiSauThue luongCtHoanLaiSauThue)
        {
            decimal result = 0;

            if (luongCtHoanLaiSauThue != null)
            {
                result = luongCtHoanLaiSauThue.ThueTncn + luongCtHoanLaiSauThue.Other;
            }

            return result;
        }

        private decimal TinhMucHuongTroCapTheoNgayNghi(TroCap troCapTheoNgayNghi,
            List<ChamCong> listChamCongByEmp, List<MucHuongTheoNgayNghi> listMucHuong)
        {
            decimal result = 0;

            double ngayNghiBaoTruoc = 0;
            double ngayNghiDotXuat = 0;
            double tongNgayNghi = 0;

            listChamCongByEmp.ForEach(chamCong =>
            {
                #region Ngày nghỉ có báo trước

                if (chamCong.KyHieuVaoSang == 1 ||
                    chamCong.KyHieuVaoSang == 5 ||
                    chamCong.KyHieuVaoSang == 8) ngayNghiBaoTruoc += 0.25;

                if (chamCong.KyHieuRaSang == 1 ||
                    chamCong.KyHieuRaSang == 5 ||
                    chamCong.KyHieuRaSang == 8) ngayNghiBaoTruoc += 0.25;

                if (chamCong.KyHieuVaoChieu == 1 ||
                    chamCong.KyHieuVaoChieu == 5 ||
                    chamCong.KyHieuVaoChieu == 8) ngayNghiBaoTruoc += 0.25;

                if (chamCong.KyHieuRaChieu == 1 ||
                    chamCong.KyHieuRaChieu == 5 ||
                    chamCong.KyHieuRaChieu == 8) ngayNghiBaoTruoc += 0.25;

                #endregion

                #region Ngày nghỉ đột xuất

                if (chamCong.KyHieuVaoSang == 9) ngayNghiDotXuat += 0.25;
                if (chamCong.KyHieuRaSang == 9) ngayNghiDotXuat += 0.25;
                if (chamCong.KyHieuVaoChieu == 9) ngayNghiDotXuat += 0.25;
                if (chamCong.KyHieuRaChieu == 9) ngayNghiDotXuat += 0.25;

                #endregion
            });

            tongNgayNghi = ngayNghiBaoTruoc + ngayNghiDotXuat;
            var listMucHuongPhanTram = new List<decimal>();

            for (int i = 0; i < listMucHuong.Count; i++)
            {
                var mucHuong = listMucHuong[i];

                //Nếu số ngày bằng một số cố định
                if (mucHuong.SoNgayTu == mucHuong.SoNgayDen)
                {
                    //Nếu là Nghỉ báo trước + Nghỉ đột xuất
                    if (mucHuong.LoaiNgayNghi == null && mucHuong.SoNgayTu == (decimal)tongNgayNghi)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ báo trước
                    else if (mucHuong.LoaiNgayNghi == 1 && mucHuong.SoNgayTu == (decimal)ngayNghiBaoTruoc)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ đột xuất
                    else if (mucHuong.LoaiNgayNghi == 2 && mucHuong.SoNgayTu == (decimal)ngayNghiDotXuat)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                }
                //Nếu số ngày là trong khoảng Từ -> Đến
                else if (mucHuong.SoNgayDen != null)
                {
                    //Nếu là Nghỉ báo trước + Nghỉ đột xuất
                    if (mucHuong.LoaiNgayNghi == null &&
                        mucHuong.SoNgayTu <= (decimal)tongNgayNghi &&
                        (decimal)tongNgayNghi <= mucHuong.SoNgayDen)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ báo trước
                    else if (mucHuong.LoaiNgayNghi == 1 &&
                             mucHuong.SoNgayTu <= (decimal)ngayNghiBaoTruoc &&
                             (decimal)ngayNghiBaoTruoc <= mucHuong.SoNgayDen)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ đột xuất
                    else if (mucHuong.LoaiNgayNghi == 2 &&
                             mucHuong.SoNgayTu <= (decimal)ngayNghiDotXuat &&
                             (decimal)ngayNghiDotXuat <= mucHuong.SoNgayDen)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                }
                //Nếu số ngày là lớn hơn 1 số cố định
                else if (mucHuong.SoNgayDen == null)
                {
                    //Nếu là Nghỉ báo trước + Nghỉ đột xuất
                    if (mucHuong.LoaiNgayNghi == null &&
                        mucHuong.SoNgayTu < (decimal)tongNgayNghi)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ báo trước
                    else if (mucHuong.LoaiNgayNghi == 1 &&
                             mucHuong.SoNgayTu < (decimal)ngayNghiBaoTruoc)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ đột xuất
                    else if (mucHuong.LoaiNgayNghi == 2 &&
                             mucHuong.SoNgayTu < (decimal)ngayNghiDotXuat)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                }
            }

            decimal mucHuongPhanTram = 0;

            //Nếu không thỏa mãn cấu hình nào thì lấy giá trị = mức trợ cấp
            if (listMucHuongPhanTram.Count == 0)
            {
                mucHuongPhanTram = 100;
            }
            //Nếu có giá trị thỏa mãn cấu hình thì
            else
            {
                mucHuongPhanTram = listMucHuongPhanTram.Min();
            }

            result = Math.Round(troCapTheoNgayNghi.MucTroCap * mucHuongPhanTram / 100, 0);

            return result;
        }

        private decimal TinhMucHuongTroCapTheoDmvs(TroCap troCapTheoDmvs, int soLanDmvs, List<MucHuongDmvs> listMucHuong)
        {
            decimal mucTroCap = 0;
            var listTienTc = new List<decimal>();

            for (int i = 0; i < listMucHuong.Count; i++)
            {
                var mucHuong = listMucHuong[i];

                //Nếu số ngày bằng một số cố định
                if (mucHuong.SoLanTu == mucHuong.SoLanDen && mucHuong.SoLanTu == soLanDmvs)
                {
                    var tienTc = TinhSoTienTroCapDmvsDuocHuong(troCapTheoDmvs.MucTroCap, mucHuong, soLanDmvs);
                    listTienTc.Add(tienTc);
                }
                //Nếu số ngày là trong khoảng Từ -> Đến
                else if (mucHuong.SoLanDen != null && mucHuong.SoLanTu <= soLanDmvs && soLanDmvs <= mucHuong.SoLanDen)
                {
                    var tienTc = TinhSoTienTroCapDmvsDuocHuong(troCapTheoDmvs.MucTroCap, mucHuong, soLanDmvs);
                    listTienTc.Add(tienTc);
                }
                //Nếu số ngày là lớn hơn 1 số cố định
                else if (mucHuong.SoLanDen == null && mucHuong.SoLanTu < soLanDmvs)
                {
                    var tienTc = TinhSoTienTroCapDmvsDuocHuong(troCapTheoDmvs.MucTroCap, mucHuong, soLanDmvs);
                    listTienTc.Add(tienTc);
                }
            }

            //Nếu không thỏa mãn cấu hình nào thì lấy giá trị = mức trợ cấp
            if (listTienTc.Count == 0)
            {
                mucTroCap = troCapTheoDmvs.MucTroCap;
            }
            //Nếu có giá trị thỏa mãn cấu hình thì
            else
            {
                mucTroCap = listTienTc.Min();
            }

            return mucTroCap;
        }

        private string GenCodePhieuLuong()
        {
            var code = "";
            var prefix = "PL";

            var listCode = context.PhieuLuong.Where(x => x.PhieuLuongCode.Contains(prefix)).Select(y => new
            {
                PhieuLuongCode = Int32.Parse(y.PhieuLuongCode.Substring(2))
            }).OrderByDescending(z => z.PhieuLuongCode).ToList();

            if (listCode.Count == 0)
            {
                code = prefix + 1.ToString("D3");
            }

            if (listCode.Count < 999)
            {
                code = prefix + (listCode.Count + 1).ToString("D3");
            }
            else
            {
                code = prefix + (listCode.Count + 1).ToString();
            }

            return code;
        }

        private string ConvertThangSangTiengAnh(int thang, bool isFull)
        {
            var result = "";

            switch (thang)
            {
                case 1:
                    result = isFull ? "January" : "Jan";
                    break;
                case 2:
                    result = isFull ? "February" : "Feb";
                    break;
                case 3:
                    result = isFull ? "March" : "Mar";
                    break;
                case 4:
                    result = isFull ? "April" : "Apr";
                    break;
                case 5:
                    result = isFull ? "May" : "May";
                    break;
                case 6:
                    result = isFull ? "June" : "Jun";
                    break;
                case 7:
                    result = isFull ? "July" : "Jul";
                    break;
                case 8:
                    result = isFull ? "August" : "Aug";
                    break;
                case 9:
                    result = isFull ? "September" : "Sep";
                    break;
                case 10:
                    result = isFull ? "October" : "Oct";
                    break;
                case 11:
                    result = isFull ? "November" : "Nov";
                    break;
                case 12:
                    result = isFull ? "December" : "Dec";
                    break;
            }

            return result;
        }

        private string CheckDuocHuongTroCap(Guid dieuKienHuongId, List<LuongCtDieuKienTroCapCoDinh> listDkTroCap)
        {
            string result = "";

            var cauHinh = listDkTroCap.FirstOrDefault(x => x.DieuKienHuongId == dieuKienHuongId);
            if (cauHinh != null)
            {
                result = cauHinh.DuDieuKien ? "Có" : "Không";
                return result;
            }

            return result;
        }

        private List<DateTime> GetListNgayKhongDuocPhepSuaChamCong(List<KyLuong> listKyLuong)
        {
            List<DateTime> result = new List<DateTime>();

            listKyLuong.ForEach(kyLuong =>
            {
                var temp = kyLuong.TuNgay.Date;
                while (temp <= kyLuong.DenNgay.Date)
                {
                    result.Add(temp);

                    temp = temp.AddDays(1);
                }
            });

            result = result.OrderByDescending(z => z).ToList();

            return result;
        }

        private void LayBaoCaoOT9(out List<List<DataRowModel>>  listData, out List<List<DataHeaderModel>> listDataHeader, int KyLuongId)
        {
            #region Common Data
            var listdataRowTong = new List<DataRowModel>();
            listData = new List<List<DataRowModel>>();
            var listDataRoot = new List<List<DataRowModel>>();
            listDataHeader = new List<List<DataHeaderModel>>();
            var kyLuongInfor = context.KyLuong.FirstOrDefault(x => x.KyLuongId == KyLuongId);

            var listDate = new List<DateTime>();
            var startDate = kyLuongInfor.TuNgay.Date;
            var endDate = kyLuongInfor.DenNgay.Date;
            while (startDate <= endDate)
            {
                listDate.Add(startDate);
                startDate = startDate.AddDays(1).Date;
            }

            var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == KyLuongId)
              .Select(y => new LuongTongHopModel(y)).ToList();


            var listLuongCtTroCapOt = context.LuongCtTroCapOt
                    .Where(x => x.KyLuongId == KyLuongId)
                    .Select(y => new LuongCtTroCapOtModel(y)).ToList();

            var listLuongCtLoaiTroCapOt =
                context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == KyLuongId).ToList();

            var typeLoaiOt = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT");
            var listLoaiOt = context.Category.Where(x => x.CategoryTypeId == typeLoaiOt.CategoryTypeId).ToList();

            var listEmpId = listLuongCtTroCapOt.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeName).ToList();
            var listSubCode1 = GeneralList.GetSubCode1().ToList();
            var listSubCode2 = GeneralList.GetSubCode2().ToList();
            var cauHinhOt = context.CauHinhOt.ToList();
            #endregion

            #region Data

            var listDataTroCapOt = new List<List<DataRowModel>>();

            int stt = 0;
            listLuongCtTroCapOt.ForEach(item =>
            {
                stt++;
                var emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                var listLuongCtLoaiTroCapOt_ByEmp = listLuongCtLoaiTroCapOt
                       .Where(x => x.LuongCtTroCapOtId == item.LuongCtTroCapOtId).ToList();

                var listDataRow = new List<DataRowModel>();

                var dataEmpIdRow = new DataRowModel();
                dataEmpIdRow.ColumnKey = "no";
                dataEmpIdRow.ColumnValue = stt.ToString();
                listDataRow.Add(dataEmpIdRow);

                var dataRow = new DataRowModel();
                dataRow.ColumnKey = "empCode";
                dataRow.ColumnValue = emp.EmployeeCode;
                listDataRow.Add(dataRow);

                var dataRow2 = new DataRowModel();
                dataRow2.ColumnKey = "deptCode";
                dataRow2.ColumnValue = "";
                if (emp.SubCode1Value != null)
                {
                    dataRow2.ColumnValue = listSubCode1.FirstOrDefault(x => x.Value == emp.SubCode1Value).Name;
                }
                listDataRow.Add(dataRow2);

                var dataRow3 = new DataRowModel();
                dataRow3.ColumnKey = "subDeptCode";
                dataRow3.ColumnValue = "";
                if (emp.SubCode2Value != null)
                {
                    dataRow3.ColumnValue = listSubCode2.FirstOrDefault(x => x.Value == emp.SubCode2Value).Name;
                }
                listDataRow.Add(dataRow3);

                var dataRowEmpName = new DataRowModel();
                dataRowEmpName.ColumnKey = "fullName";
                dataRowEmpName.ColumnValue = emp.EmployeeName;
                listDataRow.Add(dataRowEmpName);

                var luongTongHopEmp = listLuongTongHop.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                var luongHienTai = "";
                if(luongTongHopEmp != null)
                {
                    luongHienTai = luongTongHopEmp.MucLuongHienTai.ToString();
                }
                var dataRow4 = new DataRowModel();
                dataRow4.ColumnKey = "basicTHISMONTH";
                dataRow4.ColumnValue = luongHienTai;
                listDataRow.Add(dataRow4);

                var luongHienTai1Gio = "";
                var luongCtTroCapOtEmp = listLuongCtTroCapOt.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                if(luongCtTroCapOtEmp != null)
                {
                    luongHienTai1Gio = luongCtTroCapOtEmp.MucLuongHienTai.ToString();
                }
                var dataRow5 = new DataRowModel();
                dataRow5.ColumnKey = "oT/Hr";
                dataRow5.ColumnValue = luongHienTai1Gio;
                listDataRow.Add(dataRow5);

                #region Phần cột động

                int index = 0;
                Decimal totalOT = 0;
                Decimal totalTaxableInCome = 0;
                Decimal totalHour = 0;
                listLoaiOt.ForEach(loaiOt =>
                {
                    var existsOt =
                        listLuongCtLoaiTroCapOt_ByEmp.FirstOrDefault(x => x.LoaiOtId == loaiOt.CategoryId);

                    var tiLeHuongCate = cauHinhOt.FirstOrDefault(x => x.LoaiOtId == loaiOt.CategoryId);
                    var tiLeHuong = tiLeHuongCate != null ? tiLeHuongCate.TyLeHuong : 0;

                    //Nếu nhân viên có dữ liệu của loại OT này
                    if (existsOt != null)
                    {
                        index++;
                        var loaiOT_Hour = new DataRowModel();
                        var data = "";
                        loaiOT_Hour.ColumnKey = "index_" + index;
                        GetDataBaoCaoOtChoTungLoaiOT(existsOt, "_Hour", luongCtTroCapOtEmp, tiLeHuong, out data, out totalOT, out totalTaxableInCome, out totalHour, totalOT, totalTaxableInCome, totalHour);
                        loaiOT_Hour.ColumnValue = data;
                        listDataRow.Add(loaiOT_Hour);

                        index++;
                        var loaiOT_A = new DataRowModel();
                        loaiOT_A.ColumnKey = "index_" + index;
                        GetDataBaoCaoOtChoTungLoaiOT(existsOt, "_@", luongCtTroCapOtEmp, tiLeHuong, out data, out totalOT, out totalTaxableInCome, out totalHour, totalOT, totalTaxableInCome, totalHour);
                        loaiOT_A.ColumnValue = data;
                        listDataRow.Add(loaiOT_A);

                        index++;
                        var loaiOT_OT = new DataRowModel();
                        loaiOT_OT.ColumnKey = "index_" + index;
                        GetDataBaoCaoOtChoTungLoaiOT(existsOt, "_OT", luongCtTroCapOtEmp, tiLeHuong, out data, out totalOT, out totalTaxableInCome, out totalHour, totalOT, totalTaxableInCome, totalHour);
                        loaiOT_OT.ColumnValue = data;
                        listDataRow.Add(loaiOT_OT);

                        index++;
                        var loaiOT_TaxableInCome = new DataRowModel();
                        loaiOT_TaxableInCome.ColumnKey = "index_" + index;
                        GetDataBaoCaoOtChoTungLoaiOT(existsOt, "_Taxable", luongCtTroCapOtEmp, tiLeHuong, out data, out totalOT, out totalTaxableInCome, out totalHour, totalOT, totalTaxableInCome, totalHour);
                        loaiOT_TaxableInCome.ColumnValue = data;
                        listDataRow.Add(loaiOT_TaxableInCome);
                    }
                    //Nếu nhân viên không có dữ liệu của loại OT này
                    else
                    {
                        index++;
                        var loaiOT_Hour = new DataRowModel();
                        loaiOT_Hour.ColumnKey = "index_" + index;
                        loaiOT_Hour.ColumnValue = "0";
                        listDataRow.Add(loaiOT_Hour);

                        index++;
                        var loaiOT_A = new DataRowModel();
                        loaiOT_A.ColumnKey = "index_" + index;
                        loaiOT_A.ColumnValue = "0";
                        listDataRow.Add(loaiOT_A);

                        index++;
                        var loaiOT_OT = new DataRowModel();
                        loaiOT_OT.ColumnKey = "index_" + index;
                        loaiOT_OT.ColumnValue = "0";
                        listDataRow.Add(loaiOT_OT);
                        index++;
                        var loaiOT_TaxableInCome = new DataRowModel();
                        loaiOT_TaxableInCome.ColumnKey = "index_" + index;
                        loaiOT_TaxableInCome.ColumnValue = "0";
                        listDataRow.Add(loaiOT_TaxableInCome);
                    }
                });
                
                var dataRow10 = new DataRowModel();
                dataRow10.ColumnKey = "total_OT_VND_TaxableInCome";
                dataRow10.ColumnValue = totalTaxableInCome.ToString(); 
                listDataRow.Add(dataRow10);

                var dataRow11 = new DataRowModel();
                dataRow11.ColumnKey = "total_OT_VND_TaxExempt";
                dataRow11.ColumnValue = (totalOT - totalTaxableInCome).ToString();
                listDataRow.Add(dataRow11);

                var dataRow12 = new DataRowModel();
                dataRow12.ColumnKey = "total_OT_Payable";
                dataRow12.ColumnValue = totalOT.ToString();
                listDataRow.Add(dataRow12);

                var dataRow13 = new DataRowModel();
                dataRow13.ColumnKey = "Total OT HOURS used before this request";
                dataRow13.ColumnValue = "";
                listDataRow.Add(dataRow13);

                var dataRow14 = new DataRowModel();
                dataRow14.ColumnKey = "Total OT Hour of this request";
                dataRow14.ColumnValue = totalHour.ToString();
                listDataRow.Add(dataRow14);

                var dataRow15 = new DataRowModel();
                dataRow15.ColumnKey = "TOTAL OT UP TODATE (INCLUDING THIS REQUEST)";
                dataRow15.ColumnValue = totalHour.ToString();
                listDataRow.Add(dataRow15);

                var dataRow16 = new DataRowModel();
                dataRow16.ColumnKey = "NOTE";
                dataRow16.ColumnValue = "";
                listDataRow.Add(dataRow16);

                listDataRoot.Add(listDataRow);
            });
            listData = listDataRoot;

            #endregion

            #endregion

            #region Header

            #region Header row1
            /* tr1 */
            var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "no",
                        ColumnValue = "No.",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "empCode",
                        ColumnValue = "Emp. Code",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "deptCode",
                        ColumnValue = "Dept Code",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "subDeptCode",
                        ColumnValue = "Sub Dept Code",
                    },
                        new DataHeaderModel()
                    {
                        ColumnKey = "fullName",
                        ColumnValue = "Full name",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "basicTHISMONTH",
                        ColumnValue = "Basic THIS MONTH",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "oT/Hr",
                        ColumnValue = "OT / Hr",
                    },
                };

            var sttIndex = 0;
            listLoaiOt.ForEach(loaiOt =>
            {
                sttIndex++;
                var loaiOT_Hour = new DataHeaderModel();
                loaiOT_Hour.ColumnKey = "index_" + sttIndex;
                loaiOT_Hour.ColumnValue = loaiOt.CategoryName;
                listHeader1.Add(loaiOT_Hour);

                sttIndex++;
                var loaiOT_A = new DataHeaderModel();
                loaiOT_A.ColumnKey = "index_" + sttIndex;
                loaiOT_A.ColumnValue = loaiOt.CategoryName;
                listHeader1.Add(loaiOT_A);

                sttIndex++;
                var loaiOT_OT = new DataHeaderModel();
                loaiOT_OT.ColumnKey = "index_" + sttIndex;
                loaiOT_OT.ColumnValue = loaiOt.CategoryName;
                listHeader1.Add(loaiOT_OT);

                sttIndex++;
                var loaiOT_TaxableInCome = new DataHeaderModel();
                loaiOT_TaxableInCome.ColumnKey = "index_" + sttIndex;
                loaiOT_TaxableInCome.ColumnValue = loaiOt.CategoryName;
                listHeader1.Add(loaiOT_TaxableInCome);
            });

            var dataHeader1Row10 = new DataHeaderModel();
            dataHeader1Row10.ColumnKey = "total_OT_VND_TaxableInCome";
            dataHeader1Row10.ColumnValue = "Total_OT-VND";
            listHeader1.Add(dataHeader1Row10);

            var dataHeader1Row11 = new DataHeaderModel();
            dataHeader1Row11.ColumnKey = "total_OT_VND_TaxExempt";
            dataHeader1Row11.ColumnValue = "Total_OT-VND";
            listHeader1.Add(dataHeader1Row11);

            var dataHeader1Row12 = new DataHeaderModel();
            dataHeader1Row12.ColumnKey = "total_OT_Payable";
            dataHeader1Row12.ColumnValue = "Total_OT-VND";
            listHeader1.Add(dataHeader1Row12);

            var dataHeader1Row13 = new DataHeaderModel();
            dataHeader1Row13.ColumnKey = "Total OT HOURS used before this request";
            dataHeader1Row13.ColumnValue = "Total OT HOURS used before this request";
            listHeader1.Add(dataHeader1Row13);

            var dataHeader1Row14 = new DataHeaderModel();
            dataHeader1Row14.ColumnKey = "Total OT Hour of this request";
            dataHeader1Row14.ColumnValue = "Total OT Hour of this request";
            listHeader1.Add(dataHeader1Row14);

            var dataHeader1Row15 = new DataHeaderModel();
            dataHeader1Row15.ColumnKey = "TOTAL OT UP TODATE (INCLUDING THIS REQUEST)";
            dataHeader1Row15.ColumnValue = "TOTAL OT UP TODATE (INCLUDING THIS REQUEST)";
            listHeader1.Add(dataHeader1Row15);

            var dataHeader1Row16 = new DataHeaderModel();
            dataHeader1Row16.ColumnKey = "NOTE";
            dataHeader1Row16.ColumnValue = "NOTE";
            listHeader1.Add(dataHeader1Row16);

            listDataHeader.Add(listHeader1);
            #endregion

            #region Header row2
            /* tr2 */
            var listHeader2 = new List<DataHeaderModel>() {
                    new DataHeaderModel()
                    {
                        ColumnKey = "",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "",
                        ColumnValue = "",
                    },
                };

            listLoaiOt.ForEach(loaiOt =>
            {
                var dataHeader2Row6 = new DataHeaderModel();
                dataHeader2Row6.ColumnValue = "Hour";
                listHeader2.Add(dataHeader2Row6);

                var dataHeader2Row7 = new DataHeaderModel();
                dataHeader2Row7.ColumnValue = "@";
                listHeader2.Add(dataHeader2Row7);

                var dataHeader2Row8 = new DataHeaderModel();
                dataHeader2Row8.ColumnValue = "OT";
                listHeader2.Add(dataHeader2Row8);

                var dataHeader2Row9 = new DataHeaderModel();
                dataHeader2Row9.ColumnValue = "Taxable income";
                listHeader2.Add(dataHeader2Row9);
            });

            var dataHeader2Row10 = new DataHeaderModel();
            dataHeader2Row10.ColumnValue = "Taxable income";
            listHeader2.Add(dataHeader2Row10);

            var dataHeader2Row11 = new DataHeaderModel();
            dataHeader2Row11.ColumnValue = "Tax exempt O.T";
            listHeader2.Add(dataHeader2Row11);

            var dataHeader2Row12 = new DataHeaderModel();
            dataHeader2Row12.ColumnValue = "Total O.T payable";
            listHeader2.Add(dataHeader2Row12);


            var dataHeader2Row13 = new DataHeaderModel();
            dataHeader2Row13.ColumnValue = "";
            listHeader2.Add(dataHeader2Row13);

            var dataHeader2Row14 = new DataHeaderModel();
            dataHeader2Row14.ColumnValue = "";
            listHeader2.Add(dataHeader2Row14);

            var dataHeader2Row15 = new DataHeaderModel();
            dataHeader2Row15.ColumnValue = "";
            listHeader2.Add(dataHeader2Row15);

            var dataHeader2Row16 = new DataHeaderModel();
            dataHeader2Row16.ColumnValue = "";
            listHeader2.Add(dataHeader2Row16);
          
            listDataHeader.Add(listHeader2);
            #endregion


            #endregion
        }

        private void LayBaoCao521(out List<List<DataRowModel>> listData, out List<List<DataHeaderModel>> listDataHeader, int KyLuongId)
        {
            #region Common Data
            var listdataRowTong = new List<DataRowModel>();
            listData = new List<List<DataRowModel>>();
            var listDataRoot = new List<List<DataRowModel>>();
            listDataHeader = new List<List<DataHeaderModel>>();
            var kyLuongInfor = context.KyLuong.FirstOrDefault(x => x.KyLuongId == KyLuongId);
            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= kyLuongInfor.TuNgay.Date &&
                                                           x.NgayChamCong.Date <= kyLuongInfor.DenNgay.Date)
                .OrderBy(z => z.NgayChamCong).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listDate = new List<DateTime>();
            var startDate = kyLuongInfor.TuNgay.Date;
            var endDate = kyLuongInfor.DenNgay.Date;
            while (startDate <= endDate)
            {
                listDate.Add(startDate);
                startDate = startDate.AddDays(1).Date;
            }
            var listDmvs = context.ThongKeDiMuonVeSom
                .Where(x => listEmpId.Contains(x.EmployeeId) &&
                            x.NgayDmvs.Date >= kyLuongInfor.TuNgay.Date &&
                            x.NgayDmvs.Date <= kyLuongInfor.DenNgay.Date).ToList();
            var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");
            var listAllTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == KyLuongId).ToList();
            var lisempId = listAllTongHopChamCong.Select(x => x.EmployeeId).ToList();
            var listEmp = context.Employee.Where(x => lisempId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeName).ToList();
            var listConTactEmp = context.Contact.Where(x => x.ObjectType == "EMP" && lisempId.Contains(x.ObjectId)).ToList();
            var listAllProvince = context.Province.Where(x => x.IsShowAsset == true).ToList();
            var listOrg = context.Organization.ToList();
            #endregion

            #region Các dòng data của từng nhân viên
            int stt = 0;
            listEmp.ForEach(item =>
            {

                stt++;
                var listDataRow = new List<DataRowModel>();

                var dataRow = new DataRowModel();
                dataRow.ColumnKey = "stt";
                dataRow.ColumnValue = stt.ToString();
                listDataRow.Add(dataRow);

                var dataRow2 = new DataRowModel();
                dataRow2.ColumnKey = "code";
                dataRow2.ColumnValue = item.CodeMayChamCong;
                listDataRow.Add(dataRow2);

                var dataRow3 = new DataRowModel();
                dataRow3.ColumnKey = "name";
                dataRow3.ColumnValue = item.EmployeeName;
                listDataRow.Add(dataRow3);

                var dataRow4x = new DataRowModel();
                dataRow4x.ColumnKey = "codeMayChamCong";
                dataRow4x.ColumnValue = item.CodeMayChamCong;
                listDataRow.Add(dataRow4x);

                int index = 0;

                var tongNghiDotXuat = 0;
                listDate.ForEach(data =>
                {
                    //Lấy dữ liệu chấm công của nhân viên theo ngày
                    var chamCong = listChamCong.FirstOrDefault(x =>
                        x.NgayChamCong == data && x.EmployeeId == item.EmployeeId);


                    //Nếu nhân viên có dữ liệu chấm công theo ngày này
                    if (chamCong != null)
                    {
                        if ((chamCong.KyHieuVaoSang != null && chamCong.KyHieuVaoSang == 9) ||
                            (chamCong.KyHieuRaSang != null && chamCong.KyHieuRaSang == 9) ||
                            (chamCong.KyHieuVaoChieu != null && chamCong.KyHieuVaoChieu == 9) ||
                            (chamCong.KyHieuRaChieu != null && chamCong.KyHieuRaChieu == 9))
                        {
                            tongNghiDotXuat += 1;
                        }

                        #region Vào sáng
                        index++;
                        var _vaoSang = new DataRowModel();
                        _vaoSang.ColumnKey = "index_" + index;
                        _vaoSang.ColumnValue = HienThiKyHieuTimeSheet(chamCong, 1);
                        listDataRow.Add(_vaoSang);
                        #endregion


                        #region Vào chiều
                        index++;
                        var _vaoChieu = new DataRowModel();
                        _vaoChieu.ColumnKey = "index_" + index;
                        _vaoChieu.ColumnValue = HienThiKyHieuTimeSheet(chamCong, 3);
                        listDataRow.Add(_vaoChieu);
                        #endregion

                    }
                    //Nếu nhân viên không có dữ liệu chấm công
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            index++;
                            var _dataRow = new DataRowModel();
                            _dataRow.ColumnKey = "index_" + index;
                            _dataRow.ColumnValue = "--";
                            listDataRow.Add(_dataRow);
                        }
                    }
                });
                listDataRow.Add(new DataRowModel()
                {
                    ColumnKey = "ngayLamViecThucTe",
                    ColumnValue = tongNghiDotXuat.ToString(),
                });
                
                var team = "";
                if (item.OrganizationId != null)
                {
                    team = listOrg.FirstOrDefault(x => x.OrganizationId == item.OrganizationId)?.OrganizationName;
                }
                var dataRow5 = new DataRowModel();
                dataRow5.ColumnKey = "congTac";
                dataRow5.ColumnValue = team;
                listDataRow.Add(dataRow5);

                var locaition = "";
                var provinceId = listConTactEmp.FirstOrDefault(x => x.ObjectId == item.EmployeeId)?.ProvinceId;
                if (provinceId != null)
                {
                    locaition = listAllProvince.FirstOrDefault(x => x.ProvinceId == provinceId)?.ProvinceName;
                }
                var dataRow6 = new DataRowModel();
                dataRow6.ColumnKey = "nghiDaoTaoHoiThao";
                dataRow6.ColumnValue = locaition;
                listDataRow.Add(dataRow6);

                listDataRoot.Add(listDataRow);

            });
            #endregion

            #region Thêm row tổng kết - TN

            var listDataRowTK = new List<DataRowModel>();
            var listTN = new List<int>() { 14 }; //Tại nhà
            TongKetNghiDotXuat(out listDataRowTK, listDate, listChamCong, listTN, "Tổng làm việc tại nhà");
            listDataRoot.Add(listDataRowTK);

            #endregion

            #region Tổng nghỉ báo trước

            var listDataRowBT = new List<DataRowModel>();
            var listNghiBaoTrc = new List<int>() { 1, 5, 8 }; //Nghỉ báo trc
            TongKetNghiDotXuat(out listDataRowBT, listDate, listChamCong, listNghiBaoTrc, "Tổng nghỉ báo trước");
            listDataRoot.Add(listDataRowBT);

            #endregion

            #region Tổng nghỉ đột xuất

            var listDataRowTYN = new List<DataRowModel>();
            var listNghiBaoDotXuat = new List<int>() { 9 }; //Tự ý nghỉ
            TongKetNghiDotXuat(out listDataRowTYN, listDate, listChamCong, listNghiBaoDotXuat, "Tổng nghỉ đột xuất");
            listDataRoot.Add(listDataRowTYN);

            #endregion


            listData = listDataRoot;

            #region Header
            /* tr1 */
            var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "40px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "code",
                        ColumnValue = "Code",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "120px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "name",
                        ColumnValue = "Họ tên",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "codeMayChamCong",
                        ColumnValue = "Mã máy chấm công",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                };

            int _index = 0;
            listDate.ForEach(item =>
            {
                string ngayTrongTuan = listNgayLamViecTrongTuan
                    .FirstOrDefault(x => x.Value == ConvertNgayTrongTuan(item.DayOfWeek))?.Name;

                _index++;
                var _dataHeader = new DataHeaderModel();
                _dataHeader.ColumnKey = "index_ngay_" + _index;
                _dataHeader.ColumnValue = item.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + ngayTrongTuan;
                _dataHeader.Rowspan = 0;
                _dataHeader.Colspan = 4;
                _dataHeader.Width = "240px";

                listHeader1.Add(_dataHeader);
            });
            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "ngayLamViecThucTe",
                ColumnValue = "Tổng số ngày nghỉ đột xuất",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "congTac",
                ColumnValue = "Team",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiDaoTaoHoiThao",
                ColumnValue = "Location",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listDataHeader.Add(listHeader1);

            /* tr2 */
            var listHeader2 = new List<DataHeaderModel>();
            int _indexCa = 0;
            listDate.ForEach(date =>
            {
                for (int i = 1; i <= 2; i++)
                {
                    _indexCa++;
                    var _dataHeader = new DataHeaderModel();
                    _dataHeader.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader.ColumnValue = i % 2 != 0 ? "Sáng" : "Chiều";
                    _dataHeader.Rowspan = 0;
                    _dataHeader.Colspan = 2;
                    _dataHeader.Width = "120px";

                    listHeader2.Add(_dataHeader);
                }
            });

            listDataHeader.Add(listHeader2);

            #endregion
        }

        private void LayBaoCao3(out List<List<DataRowModel>> listData, out List<List<DataHeaderModel>> listDataHeader, int KyLuongId)
        {
            #region common data
            var listdataRowTong = new List<DataRowModel>();
            listData = new List<List<DataRowModel>>();
            var listDataRoot = new List<List<DataRowModel>>();
            listDataHeader = new List<List<DataHeaderModel>>();
            var kyLuongInfor = context.KyLuong.FirstOrDefault(x => x.KyLuongId == KyLuongId);
            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= kyLuongInfor.TuNgay.Date &&
                                                           x.NgayChamCong.Date <= kyLuongInfor.DenNgay.Date)
                .OrderBy(z => z.NgayChamCong).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listDate = new List<DateTime>();
            var startDate = kyLuongInfor.TuNgay.Date;
            var endDate = kyLuongInfor.DenNgay.Date;
            while (startDate <= endDate)
            {
                listDate.Add(startDate);
                startDate = startDate.AddDays(1).Date;
            }
            var listDmvs = context.ThongKeDiMuonVeSom
                .Where(x => listEmpId.Contains(x.EmployeeId) &&
                            x.NgayDmvs.Date >= kyLuongInfor.TuNgay.Date &&
                            x.NgayDmvs.Date <= kyLuongInfor.DenNgay.Date).ToList();
            var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");
            var listAllTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == KyLuongId).ToList();
            var lisempId = listAllTongHopChamCong.Select(x => x.EmployeeId).ToList();
            var listEmp = context.Employee.Where(x => lisempId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeName).ToList();
            var listAllThongTinHopDong = context.HopDongNhanSu.Where(x => lisempId.Contains(x.EmployeeId)).ToList();

            decimal tongNgayLamViecThucTe = 0;
            decimal congTac = 0;
            decimal nghiDaoTao = 0;
            decimal nghiPhep = 0;
            decimal nghiLe = 0;
            decimal nghiCheDo = 0;
            decimal nghiHuongNguyenLuongKhac = 0;
            decimal nghiBu = 0;
            decimal nghiHuongBHXH = 0;
            decimal tuYNghiKhongHuongLuong = 0;
            decimal nghiKhongHuongLuong = 0;
            decimal tongSoNgayDiMuonVeSom = 0;
            decimal soLanTruChuyenCanDmvs = 0;
            decimal tongNgayKhongTinhLuong = 0;
            decimal tongNgayCongTinhLuong = 0;
            decimal soNgayDiLamCaNgayCoTroCapAnChua = 0;
            decimal tongNgayNghiTinhTroCapChuyenCan = 0;
            decimal tongNgayNghiTinhLuongDaoTao = 0;
            #endregion

            #region Data theo từng nhân viên
            int stt = 0;
            listEmp.ForEach(item =>
            {

                stt++;
                var listDataRow = new List<DataRowModel>();

                var dataRow = new DataRowModel();
                dataRow.ColumnKey = "stt";
                dataRow.ColumnValue = stt.ToString();
                listDataRow.Add(dataRow);

                var dataRow2 = new DataRowModel();
                dataRow2.ColumnKey = "code";
                dataRow2.ColumnValue = item.CodeMayChamCong;
                listDataRow.Add(dataRow2);

                var dataRow3 = new DataRowModel();
                dataRow3.ColumnKey = "name";
                dataRow3.ColumnValue = item.EmployeeName;
                listDataRow.Add(dataRow3);

                var hopDongCuNhat = listAllThongTinHopDong.Where(x => x.EmployeeId == item.EmployeeId).OrderBy(x => x.NgayKyHopDong).ToList();
                var ngayBd = "";
                if (hopDongCuNhat.Count > 0)
                {
                    if (hopDongCuNhat.First().NgayKetThucHopDong != null)
                    {
                        ngayBd = hopDongCuNhat.First().NgayBatDauLamViec.ToString("dd/MM/yyyy");
                    }
                }
                var dataRowNgayBatDau = new DataRowModel();
                dataRowNgayBatDau.ColumnKey = "dataRowNgayBatDau";
                dataRowNgayBatDau.ColumnValue = ngayBd;
                listDataRow.Add(dataRowNgayBatDau);

                var hopDongMoiNhat = listAllThongTinHopDong.Where(x => x.EmployeeId == item.EmployeeId).OrderByDescending(x => x.NgayKyHopDong).ToList();
                var ngayKt = "";
                if (hopDongMoiNhat.Count > 0)
                {
                    if (hopDongMoiNhat.First().NgayKetThucHopDong != null)
                    {
                        ngayKt = hopDongMoiNhat.First().NgayKetThucHopDong.Value.ToString("dd/MM/yyyy");
                    }
                }
                var dataRowNgayKt = new DataRowModel();
                dataRowNgayKt.ColumnKey = "dataRowNgayKt";
                dataRowNgayKt.ColumnValue = ngayKt;
                listDataRow.Add(dataRowNgayKt);


                int index = 0;
                listDate.ForEach(data =>
                {
                    //Lấy dữ liệu chấm công của nhân viên theo ngày
                    var chamCong = listChamCong.FirstOrDefault(x =>
                        x.NgayChamCong == data && x.EmployeeId == item.EmployeeId);

                    //Nếu nhân viên có dữ liệu chấm công theo ngày này
                    if (chamCong != null)
                    {
                        #region Vào sáng

                        index++;
                        var _vaoSang = new DataRowModel();
                        _vaoSang.ColumnKey = "index_" + index;
                        _vaoSang.ColumnValue = HienThiKyHieuTimeSheet(chamCong, 1);
                        listDataRow.Add(_vaoSang);
                        #endregion

                        #region Vào chiều

                        index++;
                        var _vaoChieu = new DataRowModel();
                        _vaoChieu.ColumnKey = "index_" + index;
                        _vaoChieu.ColumnValue = HienThiKyHieuTimeSheet(chamCong, 3);
                        listDataRow.Add(_vaoChieu);

                        #endregion
                    
                    }
                    //Nếu nhân viên không có dữ liệu chấm công
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            index++;
                            var _dataRow = new DataRowModel();
                            _dataRow.ColumnKey = "index_" + index;
                            _dataRow.ColumnValue = "--";
                            listDataRow.Add(_dataRow);
                        }
                    }
                });

                var tongHopChamCongEmp = listAllTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                if (tongHopChamCongEmp != null)
                {
                    var TongSoNgayKhongTinhLuong = tongHopChamCongEmp.NghiKhongLuong + tongHopChamCongEmp.NghiKhongPhep + tongHopChamCongEmp.NghiHuongBhxh +
                                      tongHopChamCongEmp.TongNgayDmvs;

                    var TongSoNgayTinhLuong = tongHopChamCongEmp.NgayLamViecThucTe + tongHopChamCongEmp.CongTac + tongHopChamCongEmp.DaoTaoHoiThao + tongHopChamCongEmp.NghiPhep +
                                          tongHopChamCongEmp.NghiLe + tongHopChamCongEmp.NghiBu + tongHopChamCongEmp.NghiCheDo +
                                          tongHopChamCongEmp.NghiHuongLuongKhac - tongHopChamCongEmp.TongNgayDmvs;

                    var TongNgayNghiTinhTroCapChuyenCan = tongHopChamCongEmp.NghiPhep + tongHopChamCongEmp.NghiKhongLuong + tongHopChamCongEmp.NghiKhongPhep;

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "ngayLamViecThucTe";
                    dataRow4.ColumnValue = tongHopChamCongEmp.NgayLamViecThucTe.ToString();
                    listDataRow.Add(dataRow4);
                    tongNgayLamViecThucTe += tongHopChamCongEmp.NgayLamViecThucTe;

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "congTac";
                    dataRow5.ColumnValue = tongHopChamCongEmp.CongTac.ToString();
                    listDataRow.Add(dataRow5);
                    congTac += tongHopChamCongEmp.CongTac;

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "nghiDaoTaoHoiThao";
                    dataRow6.ColumnValue = tongHopChamCongEmp.DaoTaoHoiThao.ToString();
                    listDataRow.Add(dataRow6);
                    nghiDaoTao += tongHopChamCongEmp.DaoTaoHoiThao;

                    var dataRow7 = new DataRowModel();
                    dataRow7.ColumnKey = "nghiPhep";
                    dataRow7.ColumnValue = tongHopChamCongEmp.NghiPhep.ToString();
                    listDataRow.Add(dataRow7);
                    nghiPhep += tongHopChamCongEmp.NghiPhep;

                    var dataRow70 = new DataRowModel();
                    dataRow70.ColumnKey = "nghiLe";
                    dataRow70.ColumnValue = tongHopChamCongEmp.NghiLe.ToString();
                    listDataRow.Add(dataRow70);
                    nghiLe += tongHopChamCongEmp.NghiLe;

                    var dataRow8 = new DataRowModel();
                    dataRow8.ColumnKey = "nghiCheDo";
                    dataRow8.ColumnValue = tongHopChamCongEmp.NghiCheDo.ToString();
                    listDataRow.Add(dataRow8);
                    nghiCheDo += tongHopChamCongEmp.NghiCheDo;

                    var dataRow9 = new DataRowModel();
                    dataRow9.ColumnKey = "nghiHuongNguyenLuongKhac";
                    dataRow9.ColumnValue = tongHopChamCongEmp.NghiHuongLuongKhac.ToString();
                    listDataRow.Add(dataRow9);
                    nghiHuongNguyenLuongKhac += tongHopChamCongEmp.NghiHuongLuongKhac;

                    var dataRow10 = new DataRowModel();
                    dataRow10.ColumnKey = "nghiBu";
                    dataRow10.ColumnValue = tongHopChamCongEmp.NghiBu.ToString();
                    listDataRow.Add(dataRow10);
                    nghiBu += tongHopChamCongEmp.NghiBu;

                    var dataRow11 = new DataRowModel();
                    dataRow11.ColumnKey = "nghiHuongBHXH";
                    dataRow11.ColumnValue = tongHopChamCongEmp.NghiHuongBhxh.ToString();
                    listDataRow.Add(dataRow11);
                    nghiHuongBHXH += tongHopChamCongEmp.NghiHuongBhxh;

                    var dataRow12 = new DataRowModel();
                    dataRow12.ColumnKey = "tuYNghiKhongHuongLuong";
                    dataRow12.ColumnValue = tongHopChamCongEmp.NghiKhongPhep.ToString();
                    listDataRow.Add(dataRow12);
                    tuYNghiKhongHuongLuong += tongHopChamCongEmp.NghiKhongPhep;

                    var dataRow13 = new DataRowModel();
                    dataRow13.ColumnKey = "nghiKhongHuongLuong";
                    dataRow13.ColumnValue = tongHopChamCongEmp.NghiKhongLuong.ToString();
                    listDataRow.Add(dataRow13);
                    nghiKhongHuongLuong += tongHopChamCongEmp.NghiKhongLuong;

                    var dataRow14 = new DataRowModel();
                    dataRow14.ColumnKey = "tongDiMuongVeSomDuocHuongLuong";
                    dataRow14.ColumnValue = tongHopChamCongEmp.TongNgayDmvs.ToString();
                    listDataRow.Add(dataRow14);
                    tongSoNgayDiMuonVeSom += tongHopChamCongEmp.TongNgayDmvs;

                    var dataRow15 = new DataRowModel();
                    dataRow15.ColumnKey = "soLanTruChuyenCanDiMuonVeSom";
                    dataRow15.ColumnValue = tongHopChamCongEmp.SoLanTruChuyenCan.ToString();
                    listDataRow.Add(dataRow15);
                    soLanTruChuyenCanDmvs += tongHopChamCongEmp.SoLanTruChuyenCan;

                    var dataRow16 = new DataRowModel();
                    dataRow16.ColumnKey = "tongSoNgayTinhKhongLuong";
                    dataRow16.ColumnValue = TongSoNgayKhongTinhLuong.ToString();
                    listDataRow.Add(dataRow16);
                    tongNgayKhongTinhLuong += TongSoNgayKhongTinhLuong;

                    var dataRow17 = new DataRowModel();
                    dataRow17.ColumnKey = "tongNgayCongTinhLuong";
                    dataRow17.ColumnValue = TongSoNgayTinhLuong.ToString();
                    listDataRow.Add(dataRow17);
                    tongNgayCongTinhLuong += TongSoNgayTinhLuong;

                    var dataRow18 = new DataRowModel();
                    dataRow18.ColumnKey = "soNgayDiLamDuocTraTroCapAnTrua";
                    dataRow18.ColumnValue = tongHopChamCongEmp.NgayLamViecThucTe.ToString(); // xem lại
                    listDataRow.Add(dataRow18);
                    soNgayDiLamCaNgayCoTroCapAnChua += tongHopChamCongEmp.NgayLamViecThucTe;

                    var dataRow19 = new DataRowModel();
                    dataRow19.ColumnKey = "ghiChu";
                    dataRow19.ColumnValue = "";
                    listDataRow.Add(dataRow19);

                    var dataRow20 = new DataRowModel();
                    dataRow20.ColumnKey = "tongNgayNghiTinhTroCapChuyenCan";
                    dataRow20.ColumnValue = TongNgayNghiTinhTroCapChuyenCan.ToString();
                    listDataRow.Add(dataRow20);
                    tongNgayNghiTinhTroCapChuyenCan += TongNgayNghiTinhTroCapChuyenCan;

                    var dataRow21 = new DataRowModel();
                    dataRow21.ColumnKey = "tongNgayTinhLuongDaoTao";
                    dataRow21.ColumnValue = "";
                    listDataRow.Add(dataRow21);
                    tongNgayNghiTinhLuongDaoTao += 0;

                }

                listDataRoot.Add(listDataRow);
            });
            #endregion

            #region Thêm row tổng kết

            var listDataRowTK = new List<DataRowModel>();

            var dataRowTK = new DataRowModel();
            dataRowTK.ColumnKey = "stt";
            dataRowTK.ColumnValue = "Tổng";
            listDataRowTK.Add(dataRowTK);

            var dataRow2TK = new DataRowModel();
            dataRow2TK.ColumnKey = "code";
            dataRow2TK.ColumnValue = "";
            listDataRowTK.Add(dataRow2TK);

            var dataRow3TK = new DataRowModel();
            dataRow3TK.ColumnKey = "name";
            dataRow3TK.ColumnValue = "";
            listDataRowTK.Add(dataRow3TK);

            var dataRowNgayBatDauTK = new DataRowModel();
            dataRowNgayBatDauTK.ColumnKey = "dataRowNgayBatDau";
            dataRowNgayBatDauTK.ColumnValue = "";
            listDataRowTK.Add(dataRowNgayBatDauTK);

            var dataRowNgayKtTK = new DataRowModel();
            dataRowNgayKtTK.ColumnKey = "dataRowNgayKt";
            dataRowNgayKtTK.ColumnValue = "";
            listDataRowTK.Add(dataRowNgayKtTK);

            int indexTK = 0;
            listDate.ForEach(data =>
            {
                #region Vào sáng
                var listChamCongCaSangCount = listChamCong.Where(x => x.NgayChamCong.Date == data.Date &&
                                                    ((x.VaoSang == null && x.RaSang == null && x.KyHieuVaoSang == 14) || //làm việc tại nhà 
                                                    (x.VaoSang != null || x.RaSang != null)) //Đã chấm coong => có đi làm
                                                    ).Count();
                indexTK++;
                var _vaoSang = new DataRowModel();
                _vaoSang.ColumnKey = "index_" + indexTK;
                _vaoSang.ColumnValue = listChamCongCaSangCount.ToString();
                listDataRowTK.Add(_vaoSang);
                #endregion


                #region Vào chiều
                var listChamCongCaChieuCount = listChamCong.Where(x => x.NgayChamCong.Date == data.Date &&
                                                  ((x.VaoChieu == null && x.RaChieu == null && x.KyHieuVaoSang == 14) || //làm việc tại nhà 
                                                  (x.VaoChieu != null || x.RaChieu != null)) //Đã chấm coong => có đi làm
                                                  ).Count();
                indexTK++;
                var _vaoChieu = new DataRowModel();
                _vaoChieu.ColumnKey = "index_" + indexTK;
                _vaoChieu.ColumnValue = listChamCongCaChieuCount.ToString();
                listDataRowTK.Add(_vaoChieu);

                #endregion
              
            });

            #region Phần tổng kết
            var dataRow4TK = new DataRowModel();
            dataRow4TK.ColumnKey = "ngayLamViecThucTe";
            dataRow4TK.ColumnValue = tongNgayLamViecThucTe.ToString();
            listDataRowTK.Add(dataRow4TK);

            var dataRow5TK = new DataRowModel();
            dataRow5TK.ColumnKey = "congTac";
            dataRow5TK.ColumnValue = congTac.ToString();
            listDataRowTK.Add(dataRow5TK);

            var dataRow6TK = new DataRowModel();
            dataRow6TK.ColumnKey = "nghiDaoTaoHoiThao";
            dataRow6TK.ColumnValue = "";
            listDataRowTK.Add(dataRow6TK);

            var dataRow7TK = new DataRowModel();
            dataRow7TK.ColumnKey = "nghiPhep";
            dataRow7TK.ColumnValue = nghiPhep.ToString();
            listDataRowTK.Add(dataRow7TK);

            var dataRow70TK = new DataRowModel();
            dataRow70TK.ColumnKey = "nghiLe";
            dataRow70TK.ColumnValue = nghiLe.ToString();
            listDataRowTK.Add(dataRow70TK);

            var dataRow8TK = new DataRowModel();
            dataRow8TK.ColumnKey = "nghiCheDo";
            dataRow8TK.ColumnValue = nghiCheDo.ToString();
            listDataRowTK.Add(dataRow8TK);

            var dataRow9TK = new DataRowModel();
            dataRow9TK.ColumnKey = "nghiHuongNguyenLuongKhac";
            dataRow9TK.ColumnValue = nghiHuongNguyenLuongKhac.ToString();
            listDataRowTK.Add(dataRow9TK);

            var dataRow10TK = new DataRowModel();
            dataRow10TK.ColumnKey = "nghiBu";
            dataRow10TK.ColumnValue = nghiBu.ToString();
            listDataRowTK.Add(dataRow10TK);

            var dataRow11TK = new DataRowModel();
            dataRow11TK.ColumnKey = "nghiHuongBHXH";
            dataRow11TK.ColumnValue = nghiHuongBHXH.ToString();
            listDataRowTK.Add(dataRow11TK);

            var dataRow12TK = new DataRowModel();
            dataRow12TK.ColumnKey = "tuYNghiKhongHuongLuong";
            dataRow12TK.ColumnValue = tuYNghiKhongHuongLuong.ToString();
            listDataRowTK.Add(dataRow12TK);

            var dataRow13TK = new DataRowModel();
            dataRow13TK.ColumnKey = "nghiKhongHuongLuong";
            dataRow13TK.ColumnValue = nghiKhongHuongLuong.ToString();
            listDataRowTK.Add(dataRow13TK);

            var dataRow14TK = new DataRowModel();
            dataRow14TK.ColumnKey = "tongDiMuongVeSomDuocHuongLuong";
            dataRow14TK.ColumnValue = tongSoNgayDiMuonVeSom.ToString();
            listDataRowTK.Add(dataRow14TK);

            var dataRow15TK = new DataRowModel();
            dataRow15TK.ColumnKey = "soLanTruChuyenCanDiMuonVeSom";
            dataRow15TK.ColumnValue = soLanTruChuyenCanDmvs.ToString();
            listDataRowTK.Add(dataRow15TK);

            var dataRow16TK = new DataRowModel();
            dataRow16TK.ColumnKey = "tongSoNgayTinhKhongLuong";
            dataRow16TK.ColumnValue = tongNgayKhongTinhLuong.ToString();
            listDataRowTK.Add(dataRow16TK);

            var dataRow17TK = new DataRowModel();
            dataRow17TK.ColumnKey = "tongNgayCongTinhLuong";
            dataRow17TK.ColumnValue = tongNgayCongTinhLuong.ToString();
            listDataRowTK.Add(dataRow17TK);

            var dataRow18TK = new DataRowModel();
            dataRow18TK.ColumnKey = "soNgayDiLamDuocTraTroCapAnTrua";
            dataRow18TK.ColumnValue = soNgayDiLamCaNgayCoTroCapAnChua.ToString();
            listDataRowTK.Add(dataRow18TK);

            var dataRow19TK = new DataRowModel();
            dataRow19TK.ColumnKey = "ghiChu";
            dataRow19TK.ColumnValue = "";
            listDataRowTK.Add(dataRow19TK);

            var dataRow20TK = new DataRowModel();
            dataRow20TK.ColumnKey = "tongNgayNghiTinhTroCapChuyenCan";
            dataRow20TK.ColumnValue = tongNgayNghiTinhTroCapChuyenCan.ToString();
            listDataRowTK.Add(dataRow20TK);

            var dataRow21TK = new DataRowModel();
            dataRow21TK.ColumnKey = "tongNgayTinhLuongDaoTao";
            dataRow21TK.ColumnValue = tongNgayNghiTinhLuongDaoTao.ToString();
            listDataRowTK.Add(dataRow21TK);

            listDataRoot.Add(listDataRowTK);


            #endregion

            #endregion

            listData = listDataRoot;


            #region Header
            /* tr1 */
            var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "40px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "code",
                        ColumnValue = "Code",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "120px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "name",
                        ColumnValue = "Họ tên",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "dataRowNgayBatDau",
                        ColumnValue = "Ngày bắt đầu làm việc",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                      new DataHeaderModel()
                    {
                        ColumnKey = "dataRowNgayKt",
                        ColumnValue = "Ngày kết thúc thử việc để tính phép/ Ngày làm việc cuối cùng",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                };

            int _index = 0;
            listDate.ForEach(item =>
            {
                string ngayTrongTuan = listNgayLamViecTrongTuan
                    .FirstOrDefault(x => x.Value == ConvertNgayTrongTuan(item.DayOfWeek))?.Name;

                _index++;
                var _dataHeader = new DataHeaderModel();
                _dataHeader.ColumnKey = "index_ngay_" + _index;
                _dataHeader.ColumnValue = item.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + ngayTrongTuan;
                _dataHeader.Rowspan = 0;
                _dataHeader.Colspan = 4;
                _dataHeader.Width = "240px";

                listHeader1.Add(_dataHeader);
            });
            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "ngayLamViecThucTe",
                ColumnValue = "Ngày lv thực tế",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "congTac",
                ColumnValue = "Công tác",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiDaoTaoHoiThao",
                ColumnValue = "Nghỉ đào tạo/ hội thảo",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiPhep",
                ColumnValue = "Nghỉ phép",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiLe",
                ColumnValue = "Nghỉ lễ",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiCheDo",
                ColumnValue = "Nghỉ chế độ",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiHuongNguyenLuongKhac",
                ColumnValue = "Nghỉ hưởng nguyên lương khác",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiBu",
                ColumnValue = "Nghỉ bù",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiHuongBHXH",
                ColumnValue = "Nghỉ hưởng BHXH",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "tuYNghiKhongHuongLuong",
                ColumnValue = "Tự ý nghỉ không hưởng lương ",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiKhongHuongLuong",
                ColumnValue = "Nghỉ không hưởng lương ",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });


            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "tongDiMuongVeSomDuocHuongLuong",
                ColumnValue = "TỔNG SỐ NGÀY ĐI MUỘN/ VỀ SỚM KHÔNG ĐƯỢC TRẢ LƯƠNG",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "soLanTruChuyenCanDiMuonVeSom",
                ColumnValue = "SỐ LẦN TRỪ CHUYÊN CẦN ĐI MUỘN VỀ SỚM",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "tongSoNgayTinhKhongLuong",
                ColumnValue = "TỔNG NGÀY KHÔNG TÍNH LƯƠNG",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "tongNgayCongTinhLuong",
                ColumnValue = "Tổng ngày công tính lương",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "soNgayDiLamDuocTraTroCapAnTrua",
                ColumnValue = "SỐ NGÀY ĐI LÀM CẢ NGÀY, ĐƯỢC TRẢ TRỢ CẤP ĂN TRƯA",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "ghiChu",
                ColumnValue = "Ghi chú",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "tongNgayNghiTinhTroCapChuyenCan",
                ColumnValue = "Tổng ngày nghỉ tính trợ cấp chuyên cần",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "tongNgayTinhLuongDaoTao",
                ColumnValue = "Tổng ngày tính lương đào tạo",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listDataHeader.Add(listHeader1);

            /* tr2 */
            var listHeader2 = new List<DataHeaderModel>();
            int _indexCa = 0;
            listDate.ForEach(date =>
            {
                for (int i = 1; i <= 2; i++)
                {
                    _indexCa++;
                    var _dataHeader = new DataHeaderModel();
                    _dataHeader.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader.ColumnValue = i % 2 != 0 ? "Sáng" : "Chiều";
                    _dataHeader.Rowspan = 0;
                    _dataHeader.Colspan = 2;
                    _dataHeader.Width = "120px";

                    listHeader2.Add(_dataHeader);
                }
            });

            listDataHeader.Add(listHeader2);

            #endregion
        }

        private void LayBaoCaoOT91(out List<List<DataRowModel>> listData, out List<List<DataHeaderModel>> listDataHeader, int KyLuongId)
        {
            #region Common Data
            var listdataRowTong = new List<DataRowModel>();
            listData = new List<List<DataRowModel>>();
            var listDataRoot = new List<List<DataRowModel>>();
            listDataHeader = new List<List<DataHeaderModel>>();

            var kyLuongInfor = context.KyLuong.FirstOrDefault(x => x.KyLuongId == KyLuongId);
            var listChamCongOt = context.ChamCongOt.Where(x => x.NgayChamCong.Date >= kyLuongInfor.TuNgay.Date &&
                                                           x.NgayChamCong.Date <= kyLuongInfor.DenNgay.Date)
                .OrderBy(z => z.NgayChamCong).ToList();
            var listEmpId = listChamCongOt.Select(y => y.EmployeeId).Distinct().ToList();
            var listDate = new List<DateTime>();
            var startDate = kyLuongInfor.TuNgay.Date;
            var endDate = kyLuongInfor.DenNgay.Date;
            while (startDate <= endDate)
            {
                listDate.Add(startDate);
                startDate = startDate.AddDays(1).Date;
            }
            var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");


            var listAllTongHopChamCongOt = context.TongHopChamCongOt.Where(x => x.NgayChamCong.Date >= kyLuongInfor.TuNgay.Date &&
                                                                        x.NgayChamCong.Date <= kyLuongInfor.DenNgay.Date).ToList();
            var lisempId = listAllTongHopChamCongOt.Select(x => x.EmployeeId).ToList();
            var listEmp = context.Employee.Where(x => lisempId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeName).ToList();
            var listConTactEmp = context.Contact.Where(x => x.ObjectType == "EMP" && lisempId.Contains(x.ObjectId)).ToList();
            var listAllProvince = context.Province.Where(x => x.IsShowAsset == true).ToList();
            var listOrg = context.Organization.ToList();

            //Loại OT
            var listCauHInhOt = context.CauHinhOt.ToList();
            var typeLoaiOt = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT");
            var listLoaiOt = context.Category.Where(x => x.CategoryTypeId == typeLoaiOt.CategoryTypeId).ToList();
            #endregion

            #region Các dòng data của từng nhân viên
            int stt = 0;
            listEmp.ForEach(item =>
            {

                stt++;
                var listDataRow = new List<DataRowModel>();

                var dataRow = new DataRowModel();
                dataRow.ColumnKey = "stt";
                dataRow.ColumnValue = stt.ToString();
                listDataRow.Add(dataRow);

                var dataRow2 = new DataRowModel();
                dataRow2.ColumnKey = "code";
                dataRow2.ColumnValue = item.EmployeeCode;
                listDataRow.Add(dataRow2);

                var dataRow3 = new DataRowModel();
                dataRow3.ColumnKey = "name";
                dataRow3.ColumnValue = item.EmployeeName;
                listDataRow.Add(dataRow3);

                var dataRow4x = new DataRowModel();
                dataRow4x.ColumnKey = "codeMayChamCong";
                dataRow4x.ColumnValue = item.CodeMayChamCong;
                listDataRow.Add(dataRow4x);

                int index = 0;

                listDate.ForEach(data =>
                {
                    //Lấy dữ liệu chấm công của nhân viên theo ngày
                    var chamCongOt = listChamCongOt.FirstOrDefault(x =>
                        x.NgayChamCong.Date == data.Date && x.EmployeeId == item.EmployeeId);
                 
                    //Nếu nhân viên có dữ liệu chấm công OT theo ngày này
                    if (chamCongOt != null)
                    {
                        index++;
                        var _vaoChieu = new DataRowModel();
                        _vaoChieu.ColumnKey = "index_" + index;
                        var loaiOt = listLoaiOt.FirstOrDefault(x => x.CategoryId == chamCongOt.LoaiOtId);
                        _vaoChieu.ColumnValue = loaiOt != null ? GetLoaiOTCode(loaiOt.CategoryCode) : "";
                        listDataRow.Add(_vaoChieu);
                    }
                    //Nếu nhân viên không có dữ liệu chấm công
                    else
                    {
                        index++;
                        var _vaoChieu = new DataRowModel();
                        _vaoChieu.ColumnKey = "index_" + index;
                        _vaoChieu.ColumnValue = "-";
                        listDataRow.Add(_vaoChieu);
                    }
                });

                listLoaiOt.ForEach(loaiOT =>
                {
                    var dataRow5x = new DataRowModel();
                    dataRow5x.ColumnKey = loaiOT.CategoryCode;
                    dataRow5x.ColumnValue = listChamCongOt.Where(x => x.EmployeeId == item.EmployeeId && x.LoaiOtId == loaiOT.CategoryId).Count().ToString();
                    listDataRow.Add(dataRow5x);
                });

                //Thêm cột số liệu cụ thể cho từng loại OT
                //listLoaiOt.ForEach(loaiOT =>
                //{
                //    var dataRow5x = new DataRowModel();
                //    dataRow5x.ColumnKey = loaiOT.CategoryCode;
                //    dataRow5x.ColumnValue = listChamCongOt.Where(x => x.EmployeeId == item.EmployeeId && x.LoaiOtId == loaiOT.CategoryId).Count().ToString();
                //    listDataRow.Add(dataRow5x);

                //    var dataRow6x = new DataRowModel();
                //    dataRow6x.ColumnKey = loaiOT.CategoryCode;
                //    dataRow6x.ColumnValue = listChamCongOt.Where(x => x.EmployeeId == item.EmployeeId && x.LoaiOtId == loaiOT.CategoryId).Count().ToString();
                //    listDataRow.Add(dataRow6x);

                //    var dataRow7x = new DataRowModel();
                //    dataRow7x.ColumnKey = loaiOT.CategoryCode;
                //    dataRow7x.ColumnValue = listChamCongOt.Where(x => x.EmployeeId == item.EmployeeId && x.LoaiOtId == loaiOT.CategoryId).Count().ToString();
                //    listDataRow.Add(dataRow7x);

                //});


                var team = "";
                if(item.OrganizationId != null)
                {
                    team = listOrg.FirstOrDefault(x => x.OrganizationId == item.OrganizationId)?.OrganizationName;
                }
                var dataRow5 = new DataRowModel();
                dataRow5.ColumnKey = "congTac";
                dataRow5.ColumnValue = team;
                listDataRow.Add(dataRow5);

                var locaition = "";
                var provinceId = listConTactEmp.FirstOrDefault(x => x.ObjectId == item.EmployeeId)?.ProvinceId;
                if (provinceId != null)
                {
                    locaition = listAllProvince.FirstOrDefault(x => x.ProvinceId == provinceId)?.ProvinceName;
                }
                var dataRow6 = new DataRowModel();
                dataRow6.ColumnKey = "nghiDaoTaoHoiThao";
                dataRow6.ColumnValue = locaition;
                listDataRow.Add(dataRow6);

                listDataRoot.Add(listDataRow);

            });
            #endregion

            #region Thêm row tổng kết cho từng loại OT
            listLoaiOt.ForEach(item =>
            {
                var listDataRowTK = new List<DataRowModel>();
                TongKetBaoCaoTongHopOT(out listDataRowTK, listDate, listChamCongOt, item.CategoryId, item.CategoryName, item.CategoryCode, listLoaiOt);
                listDataRoot.Add(listDataRowTK);
            });
            #endregion




            listData = listDataRoot;

            #region Header
            /* tr1 */
            var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "40px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "code",
                        ColumnValue = "Code",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "120px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "name",
                        ColumnValue = "Họ tên",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "codeMayChamCong",
                        ColumnValue = "Mã máy chấm công",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                };

            int _index = 0;
            listDate.ForEach(item =>
            {
                string ngayTrongTuan = listNgayLamViecTrongTuan
                    .FirstOrDefault(x => x.Value == ConvertNgayTrongTuan(item.DayOfWeek))?.Name;

                _index++;
                var _dataHeader = new DataHeaderModel();
                _dataHeader.ColumnKey = "index_ngay_" + _index;
                _dataHeader.ColumnValue = item.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + ngayTrongTuan;
                _dataHeader.Rowspan = 0;
                _dataHeader.Colspan = 4;
                _dataHeader.Width = "240px";

                listHeader1.Add(_dataHeader);
            });

            listLoaiOt.ForEach(loaiOT =>
            {
                var otInfor = listCauHInhOt.FirstOrDefault(x => x.LoaiOtId == loaiOT.CategoryId);
                var dataRow5x = new DataHeaderModel();
                dataRow5x.ColumnKey = loaiOT.CategoryCode;
                dataRow5x.ColumnValue = loaiOT.CategoryName;
                if (otInfor != null)
                {
                    dataRow5x.ColumnValue = loaiOT.CategoryName + " ( " + Decimal.ToInt32(otInfor.TyLeHuong).ToString() + "% )";
                }
                listHeader1.Add(dataRow5x);
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "congTac",
                ColumnValue = "Team",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiDaoTaoHoiThao",
                ColumnValue = "Location",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listDataHeader.Add(listHeader1);
            #endregion
        }

        private void LayBaoCao4(out List<List<DataRowModel>> listData, out List<List<DataHeaderModel>> listDataHeader, int KyLuongId)
        {
            #region common data
            var listdataRowTong = new List<DataRowModel>();
            listData = new List<List<DataRowModel>>();
            var listDataRoot = new List<List<DataRowModel>>();
            listDataHeader = new List<List<DataHeaderModel>>();
            var kyLuongInfor = context.KyLuong.FirstOrDefault(x => x.KyLuongId == KyLuongId);
            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= kyLuongInfor.TuNgay.Date &&
                                                           x.NgayChamCong.Date <= kyLuongInfor.DenNgay.Date)
                .OrderBy(z => z.NgayChamCong).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listDate = new List<DateTime>();
            var startDate = kyLuongInfor.TuNgay.Date;
            var endDate = kyLuongInfor.DenNgay.Date;
            while (startDate <= endDate)
            {
                listDate.Add(startDate);
                startDate = startDate.AddDays(1).Date;
            }
            var listDmvs = context.ThongKeDiMuonVeSom
                .Where(x => listEmpId.Contains(x.EmployeeId) &&
                            x.NgayDmvs.Date >= kyLuongInfor.TuNgay.Date &&
                            x.NgayDmvs.Date <= kyLuongInfor.DenNgay.Date).ToList();

            var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");
            var listAllTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == KyLuongId).ToList();
            var lisempId = listAllTongHopChamCong.Select(x => x.EmployeeId).ToList();
            var listEmp = context.Employee.Where(x => lisempId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeName).ToList();
            var listAllThongTinHopDong = context.HopDongNhanSu.Where(x => lisempId.Contains(x.EmployeeId)).ToList();


            var dieuKienHuongCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC")?.CategoryTypeId;
            var dieuKienHuongId = context.Category.FirstOrDefault(x => x.CategoryTypeId == dieuKienHuongCateTypeId)?.CategoryId;
            var listAllLuongCtTroCapCoDinh = context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == KyLuongId).ToList();

            var listAllLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh.Where(x => x.KyLuongId == KyLuongId).ToList();
            decimal tongNgayLamViecThucTe = 0;
            decimal congTac = 0;
            decimal nghiDaoTao = 0;
            double nghiPhep = 0;
            decimal nghiLe = 0;
            decimal nghiCheDo = 0;
            #endregion

            #region Data theo từng nhân viên
            int stt = 0;
            listEmp.ForEach(item =>
            {

                stt++;
                var listDataRow = new List<DataRowModel>();

                var dataRow = new DataRowModel();
                dataRow.ColumnKey = "stt";
                dataRow.ColumnValue = stt.ToString();
                listDataRow.Add(dataRow);

                var dataRow2 = new DataRowModel();
                dataRow2.ColumnKey = "codeEmp";
                dataRow2.ColumnValue = item.EmployeeCode;
                listDataRow.Add(dataRow2);

                var dataRow3 = new DataRowModel();
                dataRow3.ColumnKey = "name";
                dataRow3.ColumnValue = item.EmployeeName;
                listDataRow.Add(dataRow3);

                var dataRow3CodeMcc = new DataRowModel();
                dataRow3CodeMcc.ColumnKey = "codeMcc";
                dataRow3CodeMcc.ColumnValue = item.CodeMayChamCong;
                listDataRow.Add(dataRow3CodeMcc);

                var dataRow3TenMcc = new DataRowModel();
                dataRow3TenMcc.ColumnKey = "tenMcc";
                dataRow3TenMcc.ColumnValue = item.TenMayChamCong;
                listDataRow.Add(dataRow3TenMcc);


                var hopDongCuNhat = listAllThongTinHopDong.Where(x => x.EmployeeId == item.EmployeeId).OrderBy(x => x.NgayKyHopDong).ToList();
                var ngayBd = "";
                if (hopDongCuNhat.Count > 0)
                {
                    if (hopDongCuNhat.First().NgayKetThucHopDong != null)
                    {
                        ngayBd = hopDongCuNhat.First().NgayBatDauLamViec.ToString("dd/MM/yyyy");
                    }
                }
                var dataRowNgayBatDau = new DataRowModel();
                dataRowNgayBatDau.ColumnKey = "dataRowNgayBatDau";
                dataRowNgayBatDau.ColumnValue = ngayBd;
                listDataRow.Add(dataRowNgayBatDau);

                var hopDongMoiNhat = listAllThongTinHopDong.Where(x => x.EmployeeId == item.EmployeeId).OrderByDescending(x => x.NgayKyHopDong).ToList();
                var ngayKt = "";
                if (hopDongMoiNhat.Count > 0)
                {
                    if (hopDongMoiNhat.First().NgayKetThucHopDong != null)
                    {
                        ngayKt = hopDongMoiNhat.First().NgayKetThucHopDong.Value.ToString("dd/MM/yyyy");
                    }
                }
                var dataRowNgayKt = new DataRowModel();
                dataRowNgayKt.ColumnKey = "dataRowNgayKt";
                dataRowNgayKt.ColumnValue = ngayKt;
                listDataRow.Add(dataRowNgayKt);

                int index = 0;
                listDate.ForEach(data =>
                {
                    //Lấy dữ liệu chấm công của nhân viên theo ngày
                    var chamCong = listChamCong.FirstOrDefault(x =>
                        x.NgayChamCong == data && x.EmployeeId == item.EmployeeId);

                    //Nếu nhân viên có dữ liệu chấm công theo ngày này
                    if (chamCong != null)
                    {
                        #region Vào sáng

                        index++;
                        var _vaoSang = new DataRowModel();
                        _vaoSang.ColumnKey = "index_" + index;
                        _vaoSang.ColumnValue = HienThiKyHieuHoacSoPhutDiMuon(chamCong, 1, listDmvs, data, item.EmployeeId);
                        listDataRow.Add(_vaoSang);
                        #endregion
                      

                        #region Vào chiều

                        index++;
                        var _vaoChieu = new DataRowModel();
                        _vaoChieu.ColumnKey = "index_" + index;
                        _vaoChieu.ColumnValue = HienThiKyHieuHoacSoPhutDiMuon(chamCong, 3, listDmvs, data, item.EmployeeId);
                        listDataRow.Add(_vaoChieu);

                        #endregion

                    }
                    //Nếu nhân viên không có dữ liệu chấm công
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            index++;
                            var _dataRow = new DataRowModel();
                            _dataRow.ColumnKey = "index_" + index;
                            _dataRow.ColumnValue = "--";
                            listDataRow.Add(_dataRow);
                        }
                    }
                });

                var tongHopChamCongEmp = listAllTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                if (tongHopChamCongEmp != null)
                {
                    var TongSoNgayKhongTinhLuong = tongHopChamCongEmp.NghiKhongLuong + tongHopChamCongEmp.NghiKhongPhep + tongHopChamCongEmp.NghiHuongBhxh +
                                      tongHopChamCongEmp.TongNgayDmvs;

                    var TongSoNgayTinhLuong = tongHopChamCongEmp.NgayLamViecThucTe + tongHopChamCongEmp.CongTac + tongHopChamCongEmp.DaoTaoHoiThao + tongHopChamCongEmp.NghiPhep +
                                          tongHopChamCongEmp.NghiLe + tongHopChamCongEmp.NghiBu + tongHopChamCongEmp.NghiCheDo +
                                          tongHopChamCongEmp.NghiHuongLuongKhac - tongHopChamCongEmp.TongNgayDmvs;

                    var TongNgayNghiTinhTroCapChuyenCan = tongHopChamCongEmp.NghiPhep + tongHopChamCongEmp.NghiKhongLuong + tongHopChamCongEmp.NghiKhongPhep;

                    Decimal soGioDmvs = 0;
                    Decimal soNgayDmvs = 0;
                    Decimal soPhutDmvs = listDmvs.Where(x => x.EmployeeId == item.EmployeeId).Sum(x => x.ThoiGian);
                    if(soPhutDmvs != null)
                    {
                        if(soPhutDmvs != 0)
                        {
                            soGioDmvs = soPhutDmvs / 60;
                            soNgayDmvs = soGioDmvs*125/1000;
                        }
                    }
                    //Số phút dmvs
                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "ngayLamViecThucTe";
                    dataRow4.ColumnValue = soPhutDmvs.ToString();
                    listDataRow.Add(dataRow4);
                    tongNgayLamViecThucTe += soPhutDmvs;

                    //Số giờ dmvs
                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "congTac";
                    dataRow5.ColumnValue = soGioDmvs.ToString("F", CultureInfo.InvariantCulture);
                    listDataRow.Add(dataRow5);
                    congTac += soGioDmvs;

                    //Số ngày dmvs
                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "nghiDaoTaoHoiThao";
                    dataRow6.ColumnValue = soNgayDmvs.ToString("F", CultureInfo.InvariantCulture);
                    listDataRow.Add(dataRow6);
                    nghiDaoTao += soNgayDmvs;

                    //Tổng số ngày nghỉ
                    var dataRow7 = new DataRowModel();
                    dataRow7.ColumnKey = "nghiPhep";
                    dataRow7.ColumnValue = GetTongSoNgayNghi(item.EmployeeId, listChamCong).ToString();
                    listDataRow.Add(dataRow7);
                    nghiPhep += GetTongSoNgayNghi(item.EmployeeId, listChamCong);

                    //Tổng số lần dmvs
                    var dataRow70 = new DataRowModel();
                    dataRow70.ColumnKey = "nghiLe";
                    dataRow70.ColumnValue = GetSoLanDmvs(item.EmployeeId, listDmvs).ToString();
                    listDataRow.Add(dataRow70);
                    nghiLe += GetSoLanDmvs(item.EmployeeId, listDmvs);

                    //Tổng số ngày trừ chuyên cần
                    var dataRow8 = new DataRowModel();
                    dataRow8.ColumnKey = "nghiCheDo";
                    dataRow8.ColumnValue = tongHopChamCongEmp.SoLanTruChuyenCan.ToString();
                    listDataRow.Add(dataRow8);
                    nghiCheDo += tongHopChamCongEmp.SoLanTruChuyenCan;

                    //Ghi chú
                    var dataRow9 = new DataRowModel();
                    dataRow9.ColumnKey = "nghiHuongNguyenLuongKhac";
                    dataRow9.ColumnValue = "";
                    listDataRow.Add(dataRow9);

                    var chuyenCan = "NO";
                    var luongTroCapCoDinh = listAllLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var LuongctDkTroCapCoDinh = listAllLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x => x.DieuKienHuongId == dieuKienHuongId &&
                                                                                   x.LuongCtTroCapCoDinhId == luongTroCapCoDinh.LuongCtTroCapCoDinhId);
                    if(LuongctDkTroCapCoDinh == null)
                    {
                        chuyenCan = "YES";
                    }
                    else
                    {
                        if(LuongctDkTroCapCoDinh.DuDieuKien == true)
                        {
                            chuyenCan = "YES";
                        }
                    }
                    //Chuyên 
                    var dataRow10 = new DataRowModel();
                    dataRow10.ColumnKey = "nghiBu";
                    dataRow10.ColumnValue = chuyenCan;
                    listDataRow.Add(dataRow10);
                }

                listDataRoot.Add(listDataRow);
            });
            #endregion

            #region Thêm row tổng kết

            var listDataRowTK = new List<DataRowModel>();

            var dataRowTK = new DataRowModel();
            dataRowTK.ColumnKey = "stt";
            dataRowTK.ColumnValue = "Tổng";
            listDataRowTK.Add(dataRowTK);

            var dataRow2TK = new DataRowModel();
            dataRow2TK.ColumnKey = "code";
            dataRow2TK.ColumnValue = "";
            listDataRowTK.Add(dataRow2TK);

            var dataRow3TK = new DataRowModel();
            dataRow3TK.ColumnKey = "name";
            dataRow3TK.ColumnValue = "";
            listDataRowTK.Add(dataRow3TK);

            var dataRow3TKCodeMcc = new DataRowModel();
            dataRow3TKCodeMcc.ColumnKey = "codeMcc";
            dataRow3TKCodeMcc.ColumnValue = "";
            listDataRowTK.Add(dataRow3TKCodeMcc);

            var dataRow3TKTenMcc = new DataRowModel();
            dataRow3TKTenMcc.ColumnKey = "tenMcc";
            dataRow3TKTenMcc.ColumnValue = "";
            listDataRowTK.Add(dataRow3TKTenMcc);

            var dataRowNgayBatDauTK = new DataRowModel();
            dataRowNgayBatDauTK.ColumnKey = "dataRowNgayBatDau";
            dataRowNgayBatDauTK.ColumnValue = "";
            listDataRowTK.Add(dataRowNgayBatDauTK);

            var dataRowNgayKtTK = new DataRowModel();
            dataRowNgayKtTK.ColumnKey = "dataRowNgayKt";
            dataRowNgayKtTK.ColumnValue = "";
            listDataRowTK.Add(dataRowNgayKtTK);

            int indexTK = 0;
            listDate.ForEach(data =>
            {
                #region Vào sáng
                indexTK++;
                var _vaoSang = new DataRowModel();
                _vaoSang.ColumnKey = "index_" + indexTK;
                _vaoSang.ColumnValue = GetTongSoGioDiMuon(1, listDmvs, data);
                listDataRowTK.Add(_vaoSang);
                #endregion


                #region Vào chiều

                indexTK++;
                var _vaoChieu = new DataRowModel();
                _vaoChieu.ColumnKey = "index_" + indexTK;
                _vaoChieu.ColumnValue = GetTongSoGioDiMuon(3, listDmvs, data);
                listDataRowTK.Add(_vaoChieu);

                #endregion
           
            });

            var dataRow4TK = new DataRowModel();
            dataRow4TK.ColumnKey = "ngayLamViecThucTe";
            dataRow4TK.ColumnValue = tongNgayLamViecThucTe.ToString();
            listDataRowTK.Add(dataRow4TK);

            var dataRow5TK = new DataRowModel();
            dataRow5TK.ColumnKey = "congTac";
            dataRow5TK.ColumnValue = congTac.ToString("F", CultureInfo.InvariantCulture);
            listDataRowTK.Add(dataRow5TK);

            var dataRow6TK = new DataRowModel();
            dataRow6TK.ColumnKey = "nghiDaoTaoHoiThao";
            dataRow6TK.ColumnValue = nghiDaoTao.ToString("F", CultureInfo.InvariantCulture);
            listDataRowTK.Add(dataRow6TK);

            var dataRow7TK = new DataRowModel();
            dataRow7TK.ColumnKey = "nghiPhep";
            dataRow7TK.ColumnValue = nghiPhep.ToString();
            listDataRowTK.Add(dataRow7TK);

            var dataRow70TK = new DataRowModel();
            dataRow70TK.ColumnKey = "nghiLe";
            dataRow70TK.ColumnValue = nghiLe.ToString();
            dataRow70TK.TextAlign = "right";
            listDataRowTK.Add(dataRow70TK);

            var dataRow8TK = new DataRowModel();
            dataRow8TK.ColumnKey = "nghiCheDo";
            dataRow8TK.ColumnValue = nghiCheDo.ToString();
            listDataRowTK.Add(dataRow8TK);

            var dataRow9TK = new DataRowModel();
            dataRow9TK.ColumnKey = "nghiHuongNguyenLuongKhac";
            dataRow9TK.ColumnValue = "";
            listDataRowTK.Add(dataRow9TK);

            var dataRow10TK = new DataRowModel();
            dataRow10TK.ColumnKey = "nghiBu";
            dataRow10TK.ColumnValue = "";
            listDataRowTK.Add(dataRow10TK);
      

            listDataRoot.Add(listDataRowTK);
            #endregion


            listData = listDataRoot;


            #region Header
            /* tr1 */
            var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "40px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "code",
                        ColumnValue = "Code",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "120px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "name",
                        ColumnValue = "Họ tên",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },

                    new DataHeaderModel()
                    {
                        ColumnKey = "codeMcc",
                        ColumnValue = "Code máy chấm công",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "tenMcc",
                        ColumnValue = "Họ tên máy chấm công",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "dataRowNgayBatDau",
                        ColumnValue = "Ngày bắt đầu làm việc",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                      new DataHeaderModel()
                    {
                        ColumnKey = "dataRowNgayKt",
                        ColumnValue = "Ngày kết thúc thử việc để tính phép/ Ngày làm việc cuối cùng",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                };

            int _index = 0;
            listDate.ForEach(item =>
            {
                string ngayTrongTuan = listNgayLamViecTrongTuan
                    .FirstOrDefault(x => x.Value == ConvertNgayTrongTuan(item.DayOfWeek))?.Name;

                _index++;
                var _dataHeader = new DataHeaderModel();
                _dataHeader.ColumnKey = "index_ngay_" + _index;
                _dataHeader.ColumnValue = item.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + ngayTrongTuan;
                _dataHeader.Rowspan = 0;
                _dataHeader.Colspan = 4;
                _dataHeader.Width = "240px";

                listHeader1.Add(_dataHeader);
            });
            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "ngayLamViecThucTe",
                ColumnValue = "Số phút đi muộn trong tháng",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "congTac",
                ColumnValue = "Tổng số giờ đi muộn trong tháng",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiDaoTaoHoiThao",
                ColumnValue = "TỔNG SỐ NGÀY ĐI MUỘN/ VỀ SỚM KHÔNG ĐƯỢC TRẢ LƯƠNG",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiPhep",
                ColumnValue = "Tổng số ngày nghỉ (Có phép, ko phép, có lương và k lương)",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiLe",
                ColumnValue = "Số lần đi muộn",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiCheDo",
                ColumnValue = "Tổng số ngày trừ chuyên cần",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiHuongNguyenLuongKhac",
                ColumnValue = "Ghi chú",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listHeader1.Add(new DataHeaderModel()
            {
                ColumnKey = "nghiBu",
                ColumnValue = "CHUYÊN CẦN",
                Rowspan = 3,
                Colspan = 0,
                Width = "85px",
            });

            listDataHeader.Add(listHeader1);

            /* tr2 */
            var listHeader2 = new List<DataHeaderModel>();
            int _indexCa = 0;
            listDate.ForEach(date =>
            {
                for (int i = 1; i <= 2; i++)
                {
                    _indexCa++;
                    var _dataHeader = new DataHeaderModel();
                    _dataHeader.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader.ColumnValue = i % 2 != 0 ? "Sáng" : "Chiều";
                    _dataHeader.Rowspan = 0;
                    _dataHeader.Colspan = 2;
                    _dataHeader.Width = "120px";

                    listHeader2.Add(_dataHeader);
                }
            });
            listDataHeader.Add(listHeader2);

            #endregion
        }

        private string HienThiKyHieuHoacSoPhutDiMuon(ChamCong chamCong, int type, List<ThongKeDiMuonVeSom> listDmvs ,DateTime data, Guid empId)
        {
            //LoaiDmvs:  --1: Đi muộn sáng, 2: Về sớm sáng, 3: Vào muộn chiều, 4: Về sớm chiều
            string result = "--";
            var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong");

            //Vào sáng
            if (type == 1)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.VaoSang != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuVaoSang == null)
                    {
                        var soPhutDiMuonVeSom = listDmvs.FirstOrDefault(x => (x.LoaiDmvs == 1 || x.LoaiDmvs == 2) && x.EmployeeId == empId && x.NgayDmvs.Date == data.Date)?.ThoiGian;
                        if(soPhutDiMuonVeSom != null)
                        {
                            result = soPhutDiMuonVeSom.ToString();
                        }
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuVaoSang != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)?.ValueText;
                    }
                }
            }
            //Vào chiều
            else if (type == 3)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.VaoChieu != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuVaoChieu == null)
                    {
                        var soPhutDiMuonVeSom = listDmvs.FirstOrDefault(x => (x.LoaiDmvs == 3 || x.LoaiDmvs == 4) && x.EmployeeId == empId && x.NgayDmvs.Date == data.Date)?.ThoiGian;
                        if (soPhutDiMuonVeSom != null)
                        {
                            result = soPhutDiMuonVeSom.ToString();
                        }
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuVaoChieu != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)?.ValueText;
                    }
                }
            }
            return result;
        }

        private string GetTongSoGioDiMuon(int type, List<ThongKeDiMuonVeSom> listDmvs, DateTime data)
        {
            //LoaiDmvs:  --1: Đi muộn sáng, 2: Về sớm sáng, 3: Vào muộn chiều, 4: Về sớm chiều
            string result = "--";
            //Vào sáng
            if (type == 1)
            {
                var soPhutDiMuonVeSom = listDmvs.FirstOrDefault(x => (x.LoaiDmvs == 1 || x.LoaiDmvs == 2) && x.NgayDmvs.Date == data.Date)?.ThoiGian;
                if (soPhutDiMuonVeSom != null)
                {
                    result = soPhutDiMuonVeSom.ToString();
                }
            }
            //Vào chiều
            else if (type == 3)
            {
                var soPhutDiMuonVeSom = listDmvs.FirstOrDefault(x => (x.LoaiDmvs == 3 || x.LoaiDmvs == 4) && x.NgayDmvs.Date == data.Date)?.ThoiGian;
                if (soPhutDiMuonVeSom != null)
                {
                    result = soPhutDiMuonVeSom.ToString();
                }
            }
            return result;
        }

        private void TongKetNghiDotXuat(out List<DataRowModel> listDataRowTK, List<DateTime> listDate, List<ChamCong> listChamCong, List<int> type, string text)
        {
            listDataRowTK = new List<DataRowModel>();

            var dataRowTK = new DataRowModel();
            dataRowTK.ColumnKey = "stt";
            dataRowTK.ColumnValue = text;
            listDataRowTK.Add(dataRowTK);

            var dataRow2TK = new DataRowModel();
            dataRow2TK.ColumnKey = "code";
            dataRow2TK.ColumnValue = "";
            listDataRowTK.Add(dataRow2TK);

            var dataRow3TK = new DataRowModel();
            dataRow3TK.ColumnKey = "name";
            dataRow3TK.ColumnValue = "";
            listDataRowTK.Add(dataRow3TK);

            var dataRowNgayBatDauTK = new DataRowModel();
            dataRowNgayBatDauTK.ColumnKey = "codeMayChamCong";
            dataRowNgayBatDauTK.ColumnValue = "";
            listDataRowTK.Add(dataRowNgayBatDauTK);

            int indexTK = 0;
            var listDataRowTkDate = new List<DataRowModel>();
            listDate.ForEach(data =>
            {
                var chamCongLoop = listChamCong.Where(x => x.NgayChamCong.Date == data.Date).ToList();
                #region Vào sáng
                indexTK++;
                var _vaoSang = new DataRowModel();
                _vaoSang.ColumnKey = "index_" + indexTK;
                _vaoSang.ColumnValue = chamCongLoop.Where(x => x.KyHieuVaoSang != null && type.Contains(x.KyHieuVaoSang.Value)).Count().ToString();
                listDataRowTkDate.Add(_vaoSang);
                #endregion

                #region Vào chiều
                indexTK++;
                var _vaoChieu = new DataRowModel();
                _vaoChieu.ColumnKey = "index_" + indexTK;
                _vaoChieu.ColumnValue = chamCongLoop.Where(x => x.KyHieuVaoChieu != null && type.Contains(x.KyHieuVaoChieu.Value)).Count().ToString();
                listDataRowTkDate.Add(_vaoChieu);

                #endregion
            });
            listDataRowTK.AddRange(listDataRowTkDate);

            var dataRow4TK = new DataRowModel();
            dataRow4TK.ColumnKey = "ngayLamViecThucTe";
            dataRow4TK.ColumnValue = listChamCong.Where(x => (x.KyHieuVaoChieu != null && type.Contains(x.KyHieuVaoChieu.Value) || 
                                                            x.KyHieuVaoSang != null && type.Contains(x.KyHieuVaoSang.Value))).Count().ToString();
            listDataRowTK.Add(dataRow4TK);

            var dataRow5TK = new DataRowModel();
            dataRow5TK.ColumnKey = "congTac";
            dataRow5TK.ColumnValue = "";
            listDataRowTK.Add(dataRow5TK);

            var dataRow6TK = new DataRowModel();
            dataRow6TK.ColumnKey = "nghiDaoTaoHoiThao";
            dataRow6TK.ColumnValue = "";
            listDataRowTK.Add(dataRow6TK);
        }

        private string HienThiKyHieuTimeSheet(ChamCong chamCong, int type)
        {
            string result = "--";
            var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong");

            //Vào sáng
            if (type == 1)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.VaoSang != null || chamCong.RaSang != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuVaoSang == null)
                    {
                        result = "X";
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuVaoSang != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)?.ValueText;
                    }
                }
            }
            //Vào chiều
            else if (type == 3)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.VaoChieu != null || chamCong.RaChieu != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuVaoChieu == null)
                    {
                        result = "X";
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuVaoChieu != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)?.ValueText;
                    }
                }
            }
           
            return result;
        }

        private void LayBaoCao10(out List<List<DataRowModel>> listData, out List<List<DataHeaderModel>> listDataHeader, int KyLuongId)
        {
            #region common data
            int index = 0;
            var listdataRowTong = new List<DataRowModel>();
            listData = new List<List<DataRowModel>>();
            var listDataRoot = new List<List<DataRowModel>>();
            listDataHeader = new List<List<DataHeaderModel>>();
            var listAllCateType = context.CategoryType.ToList();
            var listAllCate = context.Category.ToList();

            var kyLuongInfor = context.KyLuong.FirstOrDefault(x => x.KyLuongId == KyLuongId);
            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= kyLuongInfor.TuNgay.Date &&
                                                           x.NgayChamCong.Date <= kyLuongInfor.DenNgay.Date)
                .OrderBy(z => z.NgayChamCong).ToList();
            var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == KyLuongId).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listDate = new List<DateTime>();
            var startDate = kyLuongInfor.TuNgay.Date;
            var endDate = kyLuongInfor.DenNgay.Date;
            while (startDate <= endDate)
            {
                listDate.Add(startDate);
                startDate = startDate.AddDays(1).Date;
            }
            var listDmvs = context.ThongKeDiMuonVeSom
                .Where(x => listEmpId.Contains(x.EmployeeId) &&
                            x.NgayDmvs.Date >= kyLuongInfor.TuNgay.Date &&
                            x.NgayDmvs.Date <= kyLuongInfor.DenNgay.Date).ToList();
            var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");
            var listAllTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == KyLuongId).Select(x =>new TongHopChamCongModel(x)).ToList();
            var lisempId = listAllTongHopChamCong.Select(x => x.EmployeeId).ToList();


            //Lương trợ cấp khác
            var listLuongTroCapKhac = context.LuongCtOther.Where(x => x.KyLuongId == KyLuongId).ToList();


            var listLuongCtTroCapKhac = context.LuongCtTroCapKhac
                 .Where(x => x.KyLuongId == KyLuongId).Select(y => new LuongCtTroCapKhacModel(y)).ToList();

            var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac
                .Where(x => x.KyLuongId == KyLuongId).ToList();

            var listSelectedLoaiTroCapKhacId =
                listLuongCtLoaiTroCapKhac.Select(y => y.LoaiTroCapId).Distinct().ToList();

            var troCapKhacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCK");
            var listLoaiTroCapKhac = context.Category
                .Where(x => x.CategoryTypeId == troCapKhacType.CategoryTypeId).ToList();

            var listSelectedLoaiTroCapKhac = listLoaiTroCapKhac
                .Where(x => listSelectedLoaiTroCapKhacId.Contains(x.CategoryId)).ToList();

            //Trợ cấp cố định
            var listTroCap = context.TroCap.ToList();
            var listLuongCtTroCapCoDinh =
                       context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == KyLuongId).ToList();

            var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh
                   .Where(x => x.KyLuongId == KyLuongId).ToList();

            var loaiTroCapCoDinhType = listAllCateType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
            var listLoaiTroCapCoDinh = listAllCate.Where(x => x.CategoryTypeId == loaiTroCapCoDinhType.CategoryTypeId).ToList();

            //Điều kiện hưởng trọ cấp cố định 
            var listAllLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh.Where(x => x.KyLuongId == KyLuongId).ToList();

            //Trợ cấp OT
            var listLuongCtLoaiTroCapOt =
                context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == KyLuongId).ToList();

            var listLuongCtTroCapOt = context.LuongCtTroCapOt
                  .Where(x => x.KyLuongId == KyLuongId)
                  .Select(y => new LuongCtTroCapOtModel(y)).ToList();

            var typeLoaiOt = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT");
            var listLoaiOt = context.Category.Where(x => x.CategoryTypeId == typeLoaiOt.CategoryTypeId).ToList();

            

            //Hợp đồng nhần sự
            var listEmp = context.Employee.Where(x => lisempId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeName).ToList();
            var listConTact = context.Contact.Where(x => x.ObjectType == "EMP" && lisempId.Contains(x.ObjectId)).ToList();
            var listAllThongTinHopDong = context.HopDongNhanSu.Where(x => lisempId.Contains(x.EmployeeId)).ToList();
            var listSubCode1 = GeneralList.GetSubCode1().ToList();
            var listSubCode2 = GeneralList.GetSubCode2().ToList();
            var cauHinhOt = context.CauHinhOt.ToList();

            //Junior, senior
            var kyNangTayNgheCateTypeId = listAllCateType.FirstOrDefault(x => x.CategoryTypeCode == "CAPBAC")?.CategoryTypeId;
            var listKyNang = listAllCate.Where(x => x.CategoryTypeId == kyNangTayNgheCateTypeId).ToList();

            //HĐ 1 năm, 2 năm
            var hdnsCateTypeId = listAllCateType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS")?.CategoryTypeId;
            var listHDNS = listAllCate.Where(x => x.CategoryTypeId == hdnsCateTypeId).ToList();

            var listAllPosition = context.Position.ToList();

            var listAllHDLD = context.HopDongNhanSu.Where(x => lisempId.Contains(x.EmployeeId)).ToList();

            //Kinh phí công đoàn
            var kinhPhiCongDong = context.KinhPhiCongDoan.FirstOrDefault();
            //Cấu hình bảo hiểm
            var cauHinhBaoHiem = context.CauHinhBaoHiem.FirstOrDefault();
            //Cấu hình giảm trừ
            var cauHinhGiamTru = context.CauHinhGiamTru.ToList();
            #endregion

            #region Thêm cột giá trị của hệ thống
            int stt = 0;

            var listDataRow = new List<DataRowModel>();
            stt++;
            //Stt
            var dataEmpIdRow = new DataRowModel();
            dataEmpIdRow.ColumnKey = "cot1";
            dataEmpIdRow.ColumnValue = "";
            listDataRow.Add(dataEmpIdRow);
            //Mã nhân viên
            var dataRow = new DataRowModel();
            dataRow.ColumnKey = "cot2";
            dataRow.ColumnValue = "";
            listDataRow.Add(dataRow);
            //Mã phòng ban: COS 
            var dataRow2 = new DataRowModel();
            dataRow2.ColumnKey = "cot3";
            dataRow2.ColumnValue = "";
            listDataRow.Add(dataRow2);
            //Điểm test
            var dataRow3 = new DataRowModel();
            dataRow3.ColumnKey = "cot4";
            dataRow3.ColumnValue = "";
            listDataRow.Add(dataRow3);
            //Trình độ tay nghề
            var dataRow4 = new DataRowModel();
            dataRow4.ColumnKey = "cot5";
            dataRow4.ColumnValue = "";
            listDataRow.Add(dataRow4);
            //Mức bảo hiểm loftcare được hưởng
            var dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot6";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Tên tiếng việt
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot7";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Subcode 2
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot8";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Chức danh
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot9";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Max thưởng
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot10";
            dataRowAll.ColumnValue = "Day";
            listDataRow.Add(dataRowAll);
            //Dự tính thưởng
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot11";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Ngày bắt đầu thử việc
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot12";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Ngày kết thúc thử việc
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot13";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Ngày bắt đầu HDLD
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot14";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Ngày kết thúc HDLD
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot15";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Tổng thời gian làm việc tại cty đến hiện tại
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot16";
            dataRowAll.ColumnValue = DateTime.Now.ToString("dd/MM/yyyy");
            listDataRow.Add(dataRowAll);
            //Tax code
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot17";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Số sổ BHXH
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot18";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Tình trạng hợp đồng
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot19";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Thưởng tuân thủ nội quy lao động theo HĐ
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot20";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Ngày làm việc thực tế
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot21";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Ngày tính lương thử việc
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot22";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Nghỉ phép
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot23";
            dataRowAll.ColumnValue = "Day";
            listDataRow.Add(dataRowAll);
            //NGhỉ lễ. tết
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot24";
            dataRowAll.ColumnValue = "Day";
            listDataRow.Add(dataRowAll);
            //Nghỉ hưởng nguyên lương khác
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot25";
            dataRowAll.ColumnValue = "Day";
            listDataRow.Add(dataRowAll);
            //Nghỉ không lương có xin phép
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot26";
            dataRowAll.ColumnValue = "Day";
            listDataRow.Add(dataRowAll);
            //Nghỉ không phép
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot27";
            dataRowAll.ColumnValue = "Day";
            listDataRow.Add(dataRowAll);
            //Tổng thời gian đi muộn về sớm không được trả lương
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot28";
            dataRowAll.ColumnValue = "Day";
            listDataRow.Add(dataRowAll);
            //Số lần trừ trợ cấp đi muộn về sớm
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot29";
            dataRowAll.ColumnValue = "WD:";
            listDataRow.Add(dataRowAll);
            //Tổng ngày được trả lương bao gồm lễ tết
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot30";
            dataRowAll.ColumnValue = (kyLuongInfor.SoNgayLamViec).ToString();
            listDataRow.Add(dataRowAll);
            //Lương theo HĐ cũ
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot31";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Tỉ lệ tăng
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot32";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);
            //Lương theo HĐ mới
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot33";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);

            //Mức tăng/Mức giảm
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot33x";
            dataRowAll.ColumnValue = "0";
            listDataRow.Add(dataRowAll);

            //Các khoản bù trứ tháng trước 
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot34";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);
            //Lương thực tế dựa trên ngày làm việc
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot35";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            //OT tính thuế
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot36";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);
            //OT không tính thuế
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot37";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);
            //Tổng OT
            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot38";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            #region Phần cột động trợ cấp cố định
            index = 0;
            listLoaiTroCapCoDinh.ForEach(loaiTroCap =>
            {
                index++;
                var dataRowLoaiTroCap = new DataRowModel();
                dataRowLoaiTroCap.ColumnKey = "index_ctcd" + index;
                var mucTraCap = listTroCap.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCap.CategoryId);
                dataRowLoaiTroCap.ColumnValue = mucTraCap != null ? mucTraCap.MucTroCap.ToString() : "0";
                listDataRow.Add(dataRowLoaiTroCap);
            });

            //Thêm cột tính tổng ở cuối
            if (index == listLoaiTroCapCoDinh.Count())
            {
                var dataRowLoaiTroCap = new DataRowModel();
                dataRowLoaiTroCap.ColumnKey = "indexCTCDSum_" + index;
                dataRowLoaiTroCap.ColumnValue = listTroCap.Sum(x => x.MucTroCap).ToString();
                listDataRow.Add(dataRowLoaiTroCap);
            }

            #endregion

            #region Phần cột động các khoản trợ cấp tính thuế khác tùy từng tháng


            index = 0;
            listSelectedLoaiTroCapKhac.ForEach(loaiTroCap =>
            {
                index++;
                var dataRowLoaiTroCap = new DataRowModel();
                dataRowLoaiTroCap.ColumnKey = "index_ctk" + index;
                dataRowLoaiTroCap.ColumnValue = "VND";
                listDataRow.Add(dataRowLoaiTroCap);
            });

            //for (var i = 0; i < 6; i++)
            //{
            //    index++;
            //    var dataRowLoaiTroCap = new DataRowModel();
            //    dataRowLoaiTroCap.ColumnKey = "index_ctk" + index;
            //    dataRowLoaiTroCap.ColumnValue = "VND";
            //    listDataRow.Add(dataRowLoaiTroCap);
            //}

            //Thêm cột tính tổng ở cuối
            if (index == listLuongCtTroCapKhac.Count())
            {
                index++;
                var dataRowLoaiTroCap = new DataRowModel();
                dataRowLoaiTroCap.ColumnKey = "indexCTKSum_" + index;
                dataRowLoaiTroCap.ColumnValue = "VND";
                listDataRow.Add(dataRowLoaiTroCap);
            }
            #endregion

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot52";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot53";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot54";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot55";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot56";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot57";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot58";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot59";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot60";
            var giamTruNguoiPhuThuoc = cauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 2);
            dataRowAll.ColumnValue = giamTruNguoiPhuThuoc != null ? giamTruNguoiPhuThuoc.MucGiamTru.ToString() : "";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot61";
            var giamTruBanThan = cauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 1);
            dataRowAll.ColumnValue = giamTruBanThan != null ? giamTruBanThan.MucGiamTru.ToString() : "";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot62";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot63";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot64";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot65";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot66";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot67";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot68";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot69";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot70";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot71";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot72";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot73";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot74";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot75";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot76";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot77";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot78";
            dataRowAll.ColumnValue = cauHinhBaoHiem.MucLuongCoSo.ToString() != null ? cauHinhBaoHiem.MucLuongCoSo.ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot79";
            dataRowAll.ColumnValue = "14900000";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot80";
            dataRowAll.ColumnValue = cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNsdld.ToString() != null ? cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNsdld.ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot81";
            dataRowAll.ColumnValue = cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNld.ToString() != null ? cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNld.ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot82";
            dataRowAll.ColumnValue = (cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNld).ToString() != null ? (cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNld).ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot83";
            dataRowAll.ColumnValue = cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNsdld.ToString() != null ? cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNsdld.ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot84";
            dataRowAll.ColumnValue = cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNld.ToString() != null ? cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNld.ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot85";
            dataRowAll.ColumnValue = (cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNld).ToString() != null ? (cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNld).ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot86";
            dataRowAll.ColumnValue = cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNsdld.ToString() != null ? cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNsdld.ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot87";
            dataRowAll.ColumnValue = cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNld.ToString() != null ? cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNld.ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot88";
            dataRowAll.ColumnValue = (cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNld).ToString() != null ? (cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNld).ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot89";
            dataRowAll.ColumnValue = (cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNld +
                    cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNld).ToString() != null ? (cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNld +
                    cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNld).ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot90";
            dataRowAll.ColumnValue = cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNsdld.ToString() != null ? cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNsdld.ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot91";
            dataRowAll.ColumnValue = cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNld.ToString() != null ? cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNld.ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot92";
            dataRowAll.ColumnValue = (cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNld).ToString() != null ? (cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNsdld + cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNld).ToString() : "0";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot93";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot94";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);


            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot95";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot96";
            dataRowAll.ColumnValue = "21.5%";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot97";
            dataRowAll.ColumnValue = "10.5%";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot98";
            dataRowAll.ColumnValue = "32.0%";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot99";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot100";
            dataRowAll.ColumnValue = kinhPhiCongDong.MucDongCongTy.ToString();
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot101";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot102";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot103";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot104";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot105";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot106";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot107";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot108";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot109";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot110";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot111";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot112";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);

            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot113";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);


            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot114";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);


            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot115";
            dataRowAll.ColumnValue = "VND";
            listDataRow.Add(dataRowAll);


            dataRowAll = new DataRowModel();
            dataRowAll.ColumnKey = "cot116";
            dataRowAll.ColumnValue = "";
            listDataRow.Add(dataRowAll);

            listDataRoot.Add(listDataRow);
            #endregion

            #region Map data
            stt = 0;
            listEmp.ForEach(item =>
            {
                listDataRow = new List<DataRowModel>();
                stt++;
                //Stt
                dataEmpIdRow = new DataRowModel();
                dataEmpIdRow.ColumnKey = "cot1";
                dataEmpIdRow.ColumnValue = stt.ToString();
                listDataRow.Add(dataEmpIdRow);
                //Mã nhân viên
                dataRow = new DataRowModel();
                dataRow.ColumnKey = "cot2";
                dataRow.ColumnValue = item.EmployeeCode;
                listDataRow.Add(dataRow);
                //Mã phòng ban: COS 
                dataRow2 = new DataRowModel();
                dataRow2.ColumnKey = "cot3";
                dataRow2.ColumnValue = item.SubCode1Value != null ? listSubCode1.FirstOrDefault(x => x.Value == item.SubCode1Value).Name : "";
                listDataRow.Add(dataRow2);
                //Điểm test
                dataRow3 = new DataRowModel();
                dataRow3.ColumnKey = "cot4";
                dataRow3.ColumnValue = item.DiemTest != null ? item.DiemTest : "0";
                listDataRow.Add(dataRow3);
                //Trình độ tay nghề
                dataRow4 = new DataRowModel();
                dataRow4.ColumnKey = "cot5";
                dataRow4.ColumnValue = item.CapBacId != null ? listKyNang.FirstOrDefault(x => x.CategoryId == item.CapBacId).CategoryName : "";
                listDataRow.Add(dataRow4);
                //Mức bảo hiểm loftcare được hưởng
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot6";
                var soNamKinhNghiem = GetSoNamKinhNghiemByEmployeeId(listAllHDLD.Where(x => x.EmployeeId == item.EmployeeId).ToList());
                //Lấy cấu hình bh
                var namHienTai = DateTime.Now.Year;
                var cauHinhBaoHiemLoftCare = context.CauHinhBaoHiemLoftCare.FirstOrDefault(x => x.NamCauHinh == namHienTai.ToString());//lấy cấu hình bảo hiểm LoftCare của năm hiện tại
                if (cauHinhBaoHiemLoftCare != null)
                {
                    var nhomBaoHiemLoftCare = context.NhomBaoHiemLoftCare.Where(x => x.CauHinhBaoHiemLoftCareId == cauHinhBaoHiemLoftCare.CauHinhBaoHiemLoftCareId).ToList();
                    var nhomBaoHiemLoftCareId = nhomBaoHiemLoftCare.Select(x => x.NhomBaoHiemLoftCareId).ToList();

                    var listChucVuBaoHiemLoftCare = context.ChucVuBaoHiemLoftCare
                        .Where(x => x.PositionId == item.PositionId && nhomBaoHiemLoftCareId.Contains((int)x.NhomBaoHiemLoftCareId))
                        .OrderByDescending(z => z.SoNamKinhNghiem).ToList();

                    var listChucVuLoftCare =
                        listChucVuBaoHiemLoftCare
                            .Where(x => x.SoNamKinhNghiem <= soNamKinhNghiem)
                            .ToList();
                    if (listChucVuLoftCare.Count > 0)
                    {
                        var listNhomBaoHiemLoftCareId = listChucVuLoftCare.Select(y => y.NhomBaoHiemLoftCareId).ToList();
                        dataRowAll.ColumnValue = String.Join(", ", nhomBaoHiemLoftCare.Where(x => listNhomBaoHiemLoftCareId.Contains(x.NhomBaoHiemLoftCareId)).Select(x => x.TenNhom).ToArray()); ;
                    }
                }
                listDataRow.Add(dataRowAll);
                //Tên tiếng việt
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot7";
                dataRowAll.ColumnValue = item.EmployeeName;
                listDataRow.Add(dataRowAll);
                //Subcode 2
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot8";
                dataRowAll.ColumnValue = item.SubCode2Value != null ? listSubCode2.FirstOrDefault(x => x.Value == item.SubCode2Value).Name : "";
                listDataRow.Add(dataRowAll);
                //Chức danh
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot9";
                dataRowAll.ColumnValue = item.PositionId != null ? listAllPosition.FirstOrDefault(x => x.PositionId == item.PositionId).PositionName : "";
                listDataRow.Add(dataRowAll);
                //Max thưởng
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot10";
                dataRowAll.ColumnValue = "0";
                listDataRow.Add(dataRowAll);
                //Dự tính thưởng
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot11";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);
                //Ngày bắt đầu thử việc
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot12";
                dataRowAll.ColumnValue = item.TrainingStartDate != null ? item.TrainingStartDate.Value.ToString("dd/MM/yyyy") : "";
                listDataRow.Add(dataRowAll);
                //Ngày kết thúc thử việc
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot13";
                dataRowAll.ColumnValue = item.ProbationEndDate != null ? item.ProbationEndDate.Value.ToString("dd / MM / yyyy") : "";
                listDataRow.Add(dataRowAll);
                //Ngày bắt đầu HDLD
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot14";
                var hopDongCuNhat = listAllHDLD.Where(x => x.EmployeeId == item.EmployeeId).OrderBy(x => x.NgayKyHopDong).ToList();
                dataRowAll.ColumnValue = hopDongCuNhat.Count > 0 ? hopDongCuNhat.First().NgayBatDauLamViec.ToString("dd / MM / yyyy") : "";
                listDataRow.Add(dataRowAll);
                //Ngày kết thúc HDLD
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot15";
                var hopDongMoiNhat = listAllHDLD.Where(x => x.EmployeeId == item.EmployeeId).OrderByDescending(x => x.NgayKyHopDong).ToList();
                dataRowAll.ColumnValue = (hopDongMoiNhat.Count > 0 && hopDongMoiNhat.First().NgayBatDauLamViec != null) ? hopDongMoiNhat.First().NgayBatDauLamViec.ToString("dd / MM / yyyy") : "";
                listDataRow.Add(dataRowAll);
                //Tổng thời gian làm việc tại cty đến hiện tại
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot16";
                dataRowAll.ColumnValue = GetSoNamKinhNghiemByEmployeeId(listAllHDLD.Where(x => x.EmployeeId == item.EmployeeId).ToList()).ToString();
                listDataRow.Add(dataRowAll);
                //Tax code
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot17";
                dataRowAll.ColumnValue = item.MaSoThueCaNhan;
                listDataRow.Add(dataRowAll);
                //Số sổ BHXH
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot18";
                var contactEmp = listConTact.FirstOrDefault(x => x.ObjectId == item.EmployeeId);
                dataRowAll.ColumnValue = contactEmp.SocialInsuranceNumber;
                listDataRow.Add(dataRowAll);
                //Tình trạng hợp đồng
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot19";
                dataRowAll.ColumnValue = hopDongMoiNhat.Count > 0 ? listHDNS.FirstOrDefault(x => x.CategoryId == hopDongMoiNhat.First().LoaiHopDongId).CategoryCode.Replace("Đ","D") : "";
                listDataRow.Add(dataRowAll);
                //Thưởng tuân thủ nội quy lao động theo HĐ
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot20";
                dataRowAll.ColumnValue = "Hỏi lại A Giang";

                var luongTroCTCD_emp = listLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                var listLuongCtLoaiTroCapCoDinh_ByEmp = listLuongCtLoaiTroCapCoDinh
                    .Where(x => x.LuongCtTroCapCoDinhId == luongTroCTCD_emp.LuongCtTroCapCoDinhId).ToList();

                var listLuongCtLoaiTroCapCoDinhId_ByEmp = listLuongCtLoaiTroCapCoDinh_ByEmp.Select(x => x.LuongCtLoaiTroCapCoDinhId).ToList();

                var thuongTuanThuTheoHdLD = listAllCate.FirstOrDefault(x => x.CategoryCode == "DKH001")?.CategoryId;

                var listLuongCtDieuKienTroCapCoDinh = listAllLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                            listLuongCtLoaiTroCapCoDinhId_ByEmp.Contains(x.LuongCtLoaiTroCapCoDinhId) && 
                            x.DieuKienHuongId == thuongTuanThuTheoHdLD);
                if(listLuongCtDieuKienTroCapCoDinh != null)
                {
                    dataRowAll.ColumnValue = listLuongCtDieuKienTroCapCoDinh.DuDieuKien == true ? "YES" : "NO";
                }


                listDataRow.Add(dataRowAll);
                //Ngày làm việc thực tế
                var tongHopChamCongEmp = listAllTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot21";
                dataRowAll.ColumnValue = tongHopChamCongEmp != null ? tongHopChamCongEmp.NgayLamViecThucTe.ToString() : "";
                listDataRow.Add(dataRowAll);
                //Ngày tính lương thử việc
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot22";
                var luongTongHopEmp = listLuongTongHop.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                dataRowAll.ColumnValue = luongTongHopEmp != null ? luongTongHopEmp.SoNgayMucLuongCu.ToString() : "";
                listDataRow.Add(dataRowAll);
                //Nghỉ phép
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot23";
                dataRowAll.ColumnValue = tongHopChamCongEmp != null ? tongHopChamCongEmp.NghiPhep.ToString() : "";
                listDataRow.Add(dataRowAll);
                //NGhỉ lễ. tết
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot24";
                dataRowAll.ColumnValue = tongHopChamCongEmp != null ? tongHopChamCongEmp.NghiLe.ToString() : "";
                listDataRow.Add(dataRowAll);
                //Nghỉ hưởng nguyên lương khác
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot25";
                dataRowAll.ColumnValue = tongHopChamCongEmp != null ? tongHopChamCongEmp.NghiHuongLuongKhac.ToString() : "";
                listDataRow.Add(dataRowAll);
                //Nghỉ không lương có xin phép
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot26";
                dataRowAll.ColumnValue = tongHopChamCongEmp != null ? tongHopChamCongEmp.NghiKhongLuong.ToString() : "";
                listDataRow.Add(dataRowAll);
                //Nghỉ không phép
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot27";
                dataRowAll.ColumnValue = tongHopChamCongEmp != null ? tongHopChamCongEmp.NghiKhongPhep.ToString() : "";
                listDataRow.Add(dataRowAll);
                //Tổng thời gian đi muộn về sớm không được trả lương
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot28";
                dataRowAll.ColumnValue = tongHopChamCongEmp != null ? tongHopChamCongEmp.TongNgayDmvs.ToString() : "";
                listDataRow.Add(dataRowAll);
                //Số lần trừ trợ cấp đi muộn về sớm
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot29";
                dataRowAll.ColumnValue = tongHopChamCongEmp != null ? tongHopChamCongEmp.SoLanTruChuyenCan.ToString() : "";
                listDataRow.Add(dataRowAll);
                //Tổng ngày được trả lương bao gồm lễ tết
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot30";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);
                //Lương theo HĐ cũ
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot31";
                dataRowAll.ColumnValue = luongTongHopEmp != null ? luongTongHopEmp.MucLuongCu.ToString() : "0";
                listDataRow.Add(dataRowAll);
                //Tỉ lệ tăng
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot32";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);
                //Lương theo HĐ mới
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot33";
                dataRowAll.ColumnValue = luongTongHopEmp != null ? luongTongHopEmp.MucLuongHienTai.ToString() : "0";
                listDataRow.Add(dataRowAll);

                //Mức tăng/Mức giảm
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot33x";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                //Các khoản bù trứ tháng trước 
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot34";
                var luongCtKhac = listLuongTroCapKhac.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                dataRowAll.ColumnValue = luongCtKhac != null ? luongCtKhac.KhoanBuTruThangTruoc.ToString() : "0";
                listDataRow.Add(dataRowAll);
                //Lương thực tế dựa trên ngày làm việc
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot35";
                dataRowAll.ColumnValue = luongTongHopEmp != null ? luongTongHopEmp.LuongThucTe.ToString() : "0";
                listDataRow.Add(dataRowAll);

                string data = "";
                Decimal totalOT = 0;
                Decimal totalTaxableInCome = 0;
                Decimal totalHour = 0;

                var luongHienTai1Gio = "";
                var luongCtTroCapOtEmp = listLuongCtTroCapOt.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                if (luongCtTroCapOtEmp != null)
                {
                    luongHienTai1Gio = luongCtTroCapOtEmp.MucLuongHienTai.ToString();
                    var listLuongCtLoaiTroCapOt_ByEmp = listLuongCtLoaiTroCapOt
                 .Where(x => x.LuongCtTroCapOtId == luongCtTroCapOtEmp.LuongCtTroCapOtId).ToList();

                    listLoaiOt.ForEach(loaiOt =>
                    {
                        var existsOt =
                            listLuongCtLoaiTroCapOt_ByEmp.FirstOrDefault(x => x.LoaiOtId == loaiOt.CategoryId);
                        var tiLeHuongCate = cauHinhOt.FirstOrDefault(x => x.LoaiOtId == loaiOt.CategoryId);
                        var tiLeHuong = tiLeHuongCate != null ? tiLeHuongCate.TyLeHuong : 0;
                        //Nếu nhân viên có dữ liệu của loại OT này
                        if (existsOt != null)
                        {
                            GetDataBaoCaoOtChoTungLoaiOT(existsOt, "_Hour", luongCtTroCapOtEmp, tiLeHuong, out data, out totalOT, out totalTaxableInCome, out totalHour, totalOT, totalTaxableInCome, totalHour);
                            GetDataBaoCaoOtChoTungLoaiOT(existsOt, "_@", luongCtTroCapOtEmp, tiLeHuong, out data, out totalOT, out totalTaxableInCome, out totalHour, totalOT, totalTaxableInCome, totalHour);
                            GetDataBaoCaoOtChoTungLoaiOT(existsOt, "_OT", luongCtTroCapOtEmp, tiLeHuong, out data, out totalOT, out totalTaxableInCome, out totalHour, totalOT, totalTaxableInCome, totalHour);
                            GetDataBaoCaoOtChoTungLoaiOT(existsOt, "_Taxable", luongCtTroCapOtEmp, tiLeHuong, out data, out totalOT, out totalTaxableInCome, out totalHour, totalOT, totalTaxableInCome, totalHour);
                        }
                    });
                }

                //OT tính thuế
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot36";
                dataRowAll.ColumnValue = totalTaxableInCome.ToString() != null ? totalTaxableInCome.ToString() : "0";
                listDataRow.Add(dataRowAll);
                //OT không tính thuế
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot37";
                dataRowAll.ColumnValue = (totalOT - totalTaxableInCome).ToString() != null ? (totalOT - totalTaxableInCome).ToString() : "0";
                listDataRow.Add(dataRowAll);
                //Tổng OT
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot38";
                dataRowAll.ColumnValue = totalOT.ToString() != null ? totalOT.ToString() : "0";
                listDataRow.Add(dataRowAll);

                #region Phần cột động trợ cấp cố định
              
               

                index = 0;
                listLoaiTroCapCoDinh.ForEach(loaiTroCap =>
                {
                    var exists =
                        listLuongCtLoaiTroCapCoDinh_ByEmp.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCap.CategoryId);

                    //Nếu nhân viên có dữ liệu của loại trợ cấp này
                    if (exists != null)
                    {
                        index++;
                        var dataRowLoaiTroCap = new DataRowModel();
                        dataRowLoaiTroCap.ColumnKey = "index_ctcd" + index;
                        dataRowLoaiTroCap.ColumnValue = exists.MucTroCap.ToString();
                        //dataRowLoaiTroCap.ColumnValue = "60,000";
                        listDataRow.Add(dataRowLoaiTroCap);
                  
                    }
                    //Nếu nhân viên không có dữ liệu của loại trợ cấp này
                    else
                    {
                        index++;
                        var dataRowLoaiTroCap = new DataRowModel();
                        dataRowLoaiTroCap.ColumnKey = "index_ctcd" + index;
                        dataRowLoaiTroCap.ColumnValue = "0";
                        listDataRow.Add(dataRowLoaiTroCap);
                       
                    }
                });

                //Thêm cột tính tổng ở cuối
                if (index == listLoaiTroCapCoDinh.Count())
                {
                    index++;
                    var dataRowLoaiTroCap = new DataRowModel();
                    dataRowLoaiTroCap.ColumnKey = "indexCTCDSum_" + index;
                    dataRowLoaiTroCap.ColumnValue = "Sum CTCD";
                    listDataRow.Add(dataRowLoaiTroCap);
                }
              
                #endregion

                #region Phần cột động các khoản trợ cấp tính thuế khác tùy từng tháng

              
                var luongCTK_Emp = listLuongCtTroCapKhac.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                var listLuongCtLoaiTroCapKhac_ByEmp = listLuongCtLoaiTroCapKhac
                    .Where(x => x.LuongCtTroCapKhacId == luongCTK_Emp.LuongCtTroCapKhacId).ToList();

                index = 0;
                listSelectedLoaiTroCapKhac.ForEach(loaiTroCap =>
                {
                    var exists =
                        listLuongCtLoaiTroCapKhac_ByEmp.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCap.CategoryId);

                    //Nếu nhân viên có dữ liệu của loại trợ cấp này
                    if (exists != null)
                    {
                        index++;
                        var dataRowLoaiTroCap = new DataRowModel();
                        dataRowLoaiTroCap.ColumnKey = "index_ctk" + index;
                        dataRowLoaiTroCap.ColumnValue = exists.MucTroCap.ToString();
                        listDataRow.Add(dataRowLoaiTroCap);
                    }
                    else
                    {
                        index++;
                        var dataRowLoaiTroCap = new DataRowModel();
                        dataRowLoaiTroCap.ColumnKey = "index_ctk" + index;
                        dataRowLoaiTroCap.ColumnValue = "";
                        listDataRow.Add(dataRowLoaiTroCap);
                    }
                });

                //for (var i = 0; i < 6; i++)
                //{
                //    index++;
                //    var dataRowLoaiTroCap = new DataRowModel();
                //    dataRowLoaiTroCap.ColumnKey = "index_ctk" + index;
                //    dataRowLoaiTroCap.ColumnValue = "100";
                //    listDataRow.Add(dataRowLoaiTroCap);
                //}

                //Thêm cột tính tổng ở cuối
                if (index == listLuongCtTroCapKhac.Count())
                {
                    index++;
                    var dataRowLoaiTroCap = new DataRowModel();
                    dataRowLoaiTroCap.ColumnKey = "indexCTKSum_" + index;
                    dataRowLoaiTroCap.ColumnValue = "Sum CTK";
                    listDataRow.Add(dataRowLoaiTroCap);
                }


                #endregion

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot52";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot53";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot54";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot55";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot56";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                var soPhuThuoc = context.Contact.Where(x => x.ObjectId == item.EmployeeId).ToList();
                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot57";
                dataRowAll.ColumnValue = soPhuThuoc != null ? (soPhuThuoc.Count() - 1).ToString() : "0";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot58";
                dataRowAll.ColumnValue =  "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot59";
                dataRowAll.ColumnValue =  "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot60";
                dataRowAll.ColumnValue =  "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot61";
                dataRowAll.ColumnValue =  "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot62";
                dataRowAll.ColumnValue =  "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot63";
                dataRowAll.ColumnValue =  "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot64";
                dataRowAll.ColumnValue =  "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot65";
                dataRowAll.ColumnValue =  "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot66";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot67";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot68";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot69";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot70";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot71";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot72";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot73";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot74";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot75";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot76";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot77";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot78";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot79";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot80";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot81";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot82";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot83";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot84";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot85";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot86";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot87";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot88";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot89";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot90";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot91";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot92";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot93";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot94";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);


                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot95";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot96";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot97";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot98";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot99";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot100";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot101";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot102";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot103";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot104";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot105";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot106";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot107";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot108";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot109";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot110";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot111";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot112";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);

                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot113";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);


                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot114";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);


                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot115";
                dataRowAll.ColumnValue = "";
                listDataRow.Add(dataRowAll);


                dataRowAll = new DataRowModel();
                dataRowAll.ColumnKey = "cot116";
                dataRowAll.ColumnValue = item.DiaDiemLamviec;
                listDataRow.Add(dataRowAll);

                listDataRoot.Add(listDataRow);
            });

            listData = listDataRoot;
            #endregion

            #region Thêm cột tổng giá trị
            stt = 0;

            var listDataRowTotal = new List<DataRowModel>();
            stt++;
            //Stt
            var dataEmpIdRowTotal = new DataRowModel();
            dataEmpIdRowTotal.ColumnKey = "cot1";
            dataEmpIdRowTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataEmpIdRowTotal);
            //Mã nhân viên
            var dataRowTotal = new DataRowModel();
            dataRowTotal.ColumnKey = "cot2";
            dataRowTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowTotal);
            //Mã phòng ban: COS 
            var dataRow2Total = new DataRowModel();
            dataRow2Total.ColumnKey = "cot3";
            dataRow2Total.ColumnValue = "x";
            listDataRowTotal.Add(dataRow2Total);
            //Điểm test
            var dataRow3Total = new DataRowModel();
            dataRow3Total.ColumnKey = "cot4";
            dataRow3Total.ColumnValue = "x";
            listDataRowTotal.Add(dataRow3Total);
            //Trình độ tay nghề
            var dataRow4Total = new DataRowModel();
            dataRow4Total.ColumnKey = "cot5";
            dataRow4Total.ColumnValue = "x";
            listDataRowTotal.Add(dataRow4Total);
            //Mức bảo hiểm loftcare được hưởng
            var dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot6";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Tên tiếng việt
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot7";
            dataRowAllTotal.ColumnValue = "Total";
            listDataRowTotal.Add(dataRowAllTotal);
            //Subcode 2
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot8";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Chức danh
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot9";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Max thưởng
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot10";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Dự tính thưởng
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot11";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //Ngày bắt đầu thử việc
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot12";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Ngày kết thúc thử việc
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot13";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Ngày bắt đầu HDLD
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot14";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Ngày kết thúc HDLD
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot15";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Tổng thời gian làm việc tại cty đến hiện tại
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot16";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Tax code
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot17";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Số sổ BHXH
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot18";
            dataRowAllTotal.ColumnValue = "x";
            listDataRowTotal.Add(dataRowAllTotal);
            //Tình trạng hợp đồng
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot19";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //Thưởng tuân thủ nội quy lao động theo HĐ
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot20";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //Ngày làm việc thực tế
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot21";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //Ngày tính lương thử việc
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot22";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //Nghỉ phép
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot23";
            dataRowAllTotal.ColumnValue = "Day";
            listDataRowTotal.Add(dataRowAllTotal);
            //NGhỉ lễ. tết
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot24";
            dataRowAllTotal.ColumnValue = "Day";
            listDataRowTotal.Add(dataRowAllTotal);
            //Nghỉ hưởng nguyên lương khác
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot25";
            dataRowAllTotal.ColumnValue = "Day";
            listDataRowTotal.Add(dataRowAllTotal);
            //Nghỉ không lương có xin phép
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot26";
            dataRowAllTotal.ColumnValue = "Day";
            listDataRowTotal.Add(dataRowAllTotal);
            //Nghỉ không phép
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot27";
            dataRowAllTotal.ColumnValue = "Day";
            listDataRowTotal.Add(dataRowAllTotal);
            //Tổng thời gian đi muộn về sớm không được trả lương
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot28";
            dataRowAllTotal.ColumnValue = "Day";
            listDataRowTotal.Add(dataRowAllTotal);
            //Số lần trừ trợ cấp đi muộn về sớm
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot29";
            dataRowAllTotal.ColumnValue = "WD:";
            listDataRowTotal.Add(dataRowAllTotal);
            //Tổng ngày được trả lương bao gồm lễ tết
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot30";
            dataRowAllTotal.ColumnValue = "20";
            listDataRowTotal.Add(dataRowAllTotal);
            //Lương theo HĐ cũ
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot31";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //Tỉ lệ tăng
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot32";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //Lương theo HĐ mới
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot33";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            //Mức tăng/Mức giảm
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot33x";
            dataRowAllTotal.ColumnValue = "0";
            listDataRowTotal.Add(dataRowAllTotal);

            //Các khoản bù trứ tháng trước 
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot34";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //Lương thực tế dựa trên ngày làm việc
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot35";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            //OT tính thuế
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot36";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //OT không tính thuế
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot37";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);
            //Tổng OT
            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot38";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            #region Phần cột động trợ cấp cố định
            index = 0;
            listLoaiTroCapCoDinh.ForEach(loaiTroCap =>
            {
                index++;
                var dataRowLoaiTroCapTotal = new DataRowModel();
                dataRowLoaiTroCapTotal.ColumnKey = "index_ctcd" + index;
                dataRowLoaiTroCapTotal.ColumnValue =  "";
                listDataRowTotal.Add(dataRowLoaiTroCapTotal);
            });

            //Thêm cột tính tổng ở cuối
            if (index == listLoaiTroCapCoDinh.Count())
            {
                var dataRowLoaiTroCapTotal = new DataRowModel();
                dataRowLoaiTroCapTotal.ColumnKey = "indexCTCDSum_" + index;
                dataRowLoaiTroCapTotal.ColumnValue = "";
                listDataRowTotal.Add(dataRowLoaiTroCapTotal);
            }

            #endregion

            #region Phần cột động các khoản trợ cấp tính thuế khác tùy từng tháng


            index = 0;
            listSelectedLoaiTroCapKhac.ForEach(loaiTroCap =>
            {
                index++;
                var dataRowLoaiTroCapTotal = new DataRowModel();
                dataRowLoaiTroCapTotal.ColumnKey = "index_ctk" + index;
                dataRowLoaiTroCapTotal.ColumnValue = "";
                listDataRowTotal.Add(dataRowLoaiTroCapTotal);
            });

            //for (var i = 0; i < 6; i++)
            //{
            //    index++;
            //    var dataRowLoaiTroCap = new DataRowModel();
            //    dataRowLoaiTroCap.ColumnKey = "index_ctk" + index;
            //    dataRowLoaiTroCap.ColumnValue = "VND";
            //    listDataRow.Add(dataRowLoaiTroCap);
            //}

            //Thêm cột tính tổng ở cuối
            if (index == listLuongCtTroCapKhac.Count())
            {
                index++;
                var dataRowLoaiTroCapTotal = new DataRowModel();
                dataRowLoaiTroCapTotal.ColumnKey = "indexCTKSum_" + index;
                dataRowLoaiTroCapTotal.ColumnValue = "";
                listDataRowTotal.Add(dataRowLoaiTroCapTotal);
            }
            #endregion

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot52";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot53";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot54";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot55";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot56";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot57";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot58";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot59";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot60";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot61";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot62";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot63";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot64";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot65";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot66";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot67";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot68";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot69";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot70";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot71";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot72";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot73";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot74";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot75";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot76";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot77";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot78";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot79";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot80";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot81";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot82";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot83";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot84";
            dataRowAllTotal.ColumnValue ="";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot85";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot86";
            dataRowAllTotal.ColumnValue =  "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot87";
            dataRowAllTotal.ColumnValue ="";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot88";
            dataRowAllTotal.ColumnValue ="";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot89";
            dataRowAllTotal.ColumnValue =  "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot90";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot91";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot92";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot93";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot94";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);


            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot95";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot96";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot97";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot98";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot99";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot100";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot101";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot102";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot103";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot104";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot105";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot106";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot107";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot108";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot109";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot110";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot111";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot112";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot113";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);


            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot114";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);


            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot115";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);


            dataRowAllTotal = new DataRowModel();
            dataRowAllTotal.ColumnKey = "cot116";
            dataRowAllTotal.ColumnValue = "";
            listDataRowTotal.Add(dataRowAllTotal);

            listDataRoot.Add(listDataRowTotal);
            #endregion

            #region Header

            #region Header row1
            /* tr1 */
            var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot1",
                        ColumnValue = "No.",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot2",
                        ColumnValue = "HR Filing Code Mã nhân viên",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot3",
                        ColumnValue = "Dept Code Mã phòng ban",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot4",
                        ColumnValue = "TESTING SCORE Điểm test",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot5",
                        ColumnValue = "3D Grade Trình độ tay nghề 3D",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot6",
                        ColumnValue = "Group Health Care Mức BH loftcare được hưởng",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot7",
                        ColumnValue = "Vietnamese name",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot8",
                        ColumnValue = "NEW Dept Code for Bonus Mã phòng ban để tính chi phí",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot9",
                        ColumnValue = "Title Chức danh",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot10",
                        ColumnValue = "Max Target Bonus  Max thưởng",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot11",
                        ColumnValue = "Monthly Bonus/ Incentive Accrual Local Currency Dự tính thưởng",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot12",
                        ColumnValue = "Start working date Ngày bắt đầu thử việc",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot13",
                        ColumnValue = "Ending probation date/ Current date Ngày kết thúc thử việc ",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot14",
                        ColumnValue = "Start labor contract date with Ngày bắt đầu HĐLĐ",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot15",
                        ColumnValue = "Ending contract date Ngày kết thúc hợp đồng LĐ",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot16",
                        ColumnValue = "Total working time up to date Tổng thời gian làm việc tại công ty tính đến thời điểm hiện tại",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot17",
                        ColumnValue = "Tax Code Mã số thuế",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot18",
                        ColumnValue = "SI Book No. Số sổ BHXH",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot19",
                        ColumnValue = "Contract Type Tình trạng hợp đồng",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot20",
                        ColumnValue = "Thưởng tuân thủ nội quy lao động Theo HĐ",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot21",
                        ColumnValue = "Actual working days/ Ngày làm việc thực tế",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot22",
                        ColumnValue = "Actual working day with old salary/ Ngày tính lương thử việc",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot23",
                        ColumnValue = "AL/ Nghỉ phép",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot24",
                        ColumnValue = "Holiday/ Nghỉ lễ, nghỉ tết nguyên lương",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot25",
                        ColumnValue = "Other paid leave with full paid/ Nghỉ hưởng nguyên lương khác (Hiếu, hỷ)",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot26",
                        ColumnValue = "Unpaid Leave with reason/ Nghi không lương có xin phép",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot27",
                        ColumnValue = "Leave without approval_Unpaid/ Nghỉ không phép",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot28",
                        ColumnValue = "Leave early and come late - Days w/o pay/ Tổng thời gian đi muộn về sớm không được trả lương",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot29",
                        ColumnValue = "Days w/o attenance award/ Số lần trừ trợ cấp chuyên cần",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot30",
                        ColumnValue = "Total payable day for this month, including Holiday (Mon-Fri)/ Tổng ngày được trả lương bao gồm lễ tết",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot31",
                        ColumnValue = "GROSS SALARY LAST MONTH/ VOCATIONAL TRAINING/ PROBATION Lương theo hợp đồng cũ, HĐ đào tạo hoặc thử việc",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot32",
                        ColumnValue = "MI Tỷ lệ tăng ",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot33",
                        ColumnValue = "NEW GROSS MI TO SIGN LABOR CONTRACT THIS MONTH Lương sau điều chỉnh so với hợp đồng cũ, HĐ thử việc (nếu có)",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot34",
                        ColumnValue = "Increase/ Decrease Mức tăng hoặc giảm ( Cột 32 - cột 30)",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot35",
                        ColumnValue = "PAYBACK - GROSS BASIC IF ANY (Các khoản bù trừ tháng trước nếu có)",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot36",
                        ColumnValue = "NEW GROSS BASED ON ACTUAL WORKING DAY Lương thực tế dựa trên ngày làm việc thực tế",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot37",
                        ColumnValue = "GROSS OT/ Tổng OT",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot38",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot39",
                        ColumnValue = "",
                    },

                };
            
            index = 0;
            listLoaiTroCapCoDinh.ForEach(loaiTroCap =>
            {
                index++;
                var dataRowLoaiTroCap = new DataHeaderModel();
                dataRowLoaiTroCap.ColumnKey = "indexCTCDHeader1_" + index;
                dataRowLoaiTroCap.ColumnValue = "OTHER ALLOWANCE (Only from R2 up, R1 bottom 10% will not be eligible except lunch and employees working less than 1 year)";
                listHeader1.Add(dataRowLoaiTroCap);

            });
            //Thêm cột tính tổng ở cuối
            if (index == listLoaiTroCapCoDinh.Count())
            {
                index++;
                var dataRowLoaiTroCapHeader1 = new DataHeaderModel();
                dataRowLoaiTroCapHeader1.ColumnKey = "indexCTCDSumHeader1_" + index;
                dataRowLoaiTroCapHeader1.ColumnValue = "";
                listHeader1.Add(dataRowLoaiTroCapHeader1);
            }

            //Trợ cấp khác
            index = 0;
            //for (var i = 0; i < 6; i++)
            //{
            //    index++;
            //    var dataRowLoaiTroCap = new DataHeaderModel();
            //    dataRowLoaiTroCap.ColumnKey = "index_ctkHeader1" + index;
            //    dataRowLoaiTroCap.ColumnValue = "OTHER BONUS";
            //    listHeader1.Add(dataRowLoaiTroCap);
            //}

            listSelectedLoaiTroCapKhac.ForEach(loaiTroCap =>
            {
                index++;
                var dataRowLoaiTroCap = new DataHeaderModel();
                dataRowLoaiTroCap.ColumnKey = "index_ctkHeader1" + index;
                dataRowLoaiTroCap.ColumnValue = "OTHER BONUS";
                listHeader1.Add(dataRowLoaiTroCap);
            });

            //Thêm cột tính tổng ở cuối
            if (index == listLuongCtTroCapKhac.Count())
            {
                index++;
                var dataRowLoaiTroCapSum = new DataHeaderModel();
                dataRowLoaiTroCapSum.ColumnKey = "indexCTKSumHeader1_" + index;
                dataRowLoaiTroCapSum.ColumnValue = "OTHER BONUS Các khoản trợ cấp tính thuế khác tùy từng tháng";
                listHeader1.Add(dataRowLoaiTroCapSum);
            }
           

            //cột số 52
            var dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot52";
            dataHeader1RowAll.ColumnValue = "TOTAL GROSS OTHER INCOME + OT Tổng trợ cấp và OT";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot53";
            dataHeader1RowAll.ColumnValue = "ADDED NET  ALLOWANCE - TAX EXAMPTION FOR EMPLOYEE/ TAXABLE BY COMPANY Trợ cấp phụ cấp khác có tính thuế";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot54";
            dataHeader1RowAll.ColumnValue = "DEDUCTION FROM EMPLOYEE/ Refund company before Tax (Over paid AL) Các khoản giảm trừ trước thuế nếu có";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot55";
            dataHeader1RowAll.ColumnValue = "GRAND TOTAL GROSS (BF deduct OT Examption & Severance allowance yet) Tổng thu nhập bao gồm cả các khoản tính thuế và không tính thuế ";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot56";
            dataHeader1RowAll.ColumnValue = "GRAND TOTAL GROSS (After deduct OT Examption, Severance allowance) Tổng thu nhập sau khi đã loại bỏ các khoản không chịu thuế";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot57";
            dataHeader1RowAll.ColumnValue = "OTHER DEDUCTION BEFORE TAX (TAX EXAMPTION) Các khoản giảm trừ trước thuế";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot58";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot59";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot60";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot61";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot62";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot63";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot64";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot65";
            dataHeader1RowAll.ColumnValue = "OTHER ADDED TO CALCULATED TAX ONLY, none of those benefits be paid through payroll Thu nhập đã trả tháng trước nhưng chưa tính thuế TNCN, chỉ add vào để tính thuế, không bao gồm khoản trả tháng này";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot66";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot67";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot67";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot69";
            dataHeader1RowAll.ColumnValue = "TOTAL GROSS SALARY AFTER DEDUCT TAX EXAMPTION Tổng thu nhập chịu thuế sau giảm trừ";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot70";
            dataHeader1RowAll.ColumnValue = "TOTAL PIT PAYMENT PAID BY COMPANY AND EMPLOYEE Tổng thuế TNCN phải trả bởi công ty và NLĐ";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot71";
            dataHeader1RowAll.ColumnValue = "Assessable income__Borne by employee (excl net amount-Company paid PIT) Tổng số thuế NLĐ phải trả không bao gồm các khoản được trả bởi cty";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot72";
            dataHeader1RowAll.ColumnValue = "TOTAL PIT PAYMENT_Borne by employee Tổng thuế TNCN NLĐ phải trả";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot73";
            dataHeader1RowAll.ColumnValue = "TOTAL PIT PAYMENT_Borne by employer Tổng thuế TNCN công ty phải trả cho NLĐ";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot74";
            dataHeader1RowAll.ColumnValue = "PIT off-set for company Thuế TNCN bù trừ trong quý nếu có (Cột này phát sinh thời điểm trước tháng 1.2021  do công ty áp dụng khai thuế TNCN theo quý. Sau tháng 01.2021 bắt đầu khai thuế theo tháng thì cột này không phát sinh nữa)";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot75";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot76";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot77";
            dataHeader1RowAll.ColumnValue = "PIT born by company &employee after off-set Thuế TNCN còn phải nộp sau khi bù trừ nếu có";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot78";
            dataHeader1RowAll.ColumnValue = "Basic salary for UI fr 1st Jan 20: (4,420,000*20) = 88.400.000 Lương cơ bản đóng BHTN Max 20 tháng lương cơ bản Mức này thay đổi từ 01.07.2022";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot79";
            dataHeader1RowAll.ColumnValue = "Basic salary for SI, AI & HI, Trade Union from 1.7.2019 (1,490,000*20=29.8mil) Lương cơ bản đóng BHXH, BHYT, BHNN, KPCĐ Không quá 20 tháng lương cơ sở -1tr490";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot80";
            dataHeader1RowAll.ColumnValue = "Unemployment Ins_2% Bảo hiểm thất nghiệp";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot81";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot82";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot83";
            dataHeader1RowAll.ColumnValue = "Social Ins_25% BHXH";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot84";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot85";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot86";
            dataHeader1RowAll.ColumnValue = "Accident Ins_0.5% BHTNNN";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot87";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot88";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot89";
            dataHeader1RowAll.ColumnValue = "Grand total SI, AI = 25.5%";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot90";
            dataHeader1RowAll.ColumnValue = "H.I_4.5% BHYT";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot91";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot92";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot93";
            dataHeader1RowAll.ColumnValue = "Additional SI, AI, HI, UI payback …. Paid in this month if any Các khoản vể BH bắt buộc bù trừ tháng trước phải trả vào tháng này nếu có";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot94";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot95";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot96";
            dataHeader1RowAll.ColumnValue = "Total company and employee_SI, AI, UI and HI _32% Tổng tiền BH bắt buộc phải trả của cty và NLĐ(Tổng 32 %)";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot97";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot98";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot99";
            dataHeader1RowAll.ColumnValue = "Net Income BEFORE ADD/ DEDUCT AFTER TAX Thu nhập thực nhận trước khi bù trừ thuế nếu có";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot100";
            dataHeader1RowAll.ColumnValue = "Other company payable Các khoản công ty phải đóng khác";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot101";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot102";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot103";
            dataHeader1RowAll.ColumnValue = "Other deduction after Tax Các khoản giảm trừ sau thuế";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot104";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot105";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot106";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot107";
            dataHeader1RowAll.ColumnValue = "Other Additional after Tax Các khoản hoàn lại sau thuế";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot108";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot109";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot110";
            dataHeader1RowAll.ColumnValue = "";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot111";
            dataHeader1RowAll.ColumnValue = "TOTAL NET INCOME PAYABLE Tổng thực nhận phải trả";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot112";
            dataHeader1RowAll.ColumnValue = "Paid - 1st time if any Đã trả lần 1 nếu có(hoặc tạm ứng)";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot113";
            dataHeader1RowAll.ColumnValue = "NET RECEIVE THIS MONTH Thực nhận trong tháng";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot114";
            dataHeader1RowAll.ColumnValue = "TOTAL GROSS PAYABLE - BEFORE OFFSET PIT DEDUCTION FROM PIT LIQUIDATION Tổng chi phí công ty phải trả trước khi bù trừ thuế TNCN";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot115";
            dataHeader1RowAll.ColumnValue = "TOTAL GROSS PAYABLE-AFTER OFFSET  FROM PIT LIQUIDATION YEAR 2020 Tổng chi phí công ty phải trả sau khi bù trừ thuế TNCN";
            listHeader1.Add(dataHeader1RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot116";
            dataHeader1RowAll.ColumnValue = "Location Khu vực";
            listHeader1.Add(dataHeader1RowAll);
            listDataHeader.Add(listHeader1);
            #endregion

            #region Header row2
            /* tr1 */
            var listHeader2 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot1",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot2",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot3",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot4",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot5",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot6",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot7",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot8",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot9",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot10",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot11",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot12",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot13",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot14",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot15",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot16",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot17",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot18",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot19",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot20",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot21",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot22",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot23",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot24",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot25",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot26",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot27",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot28",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot29",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot30",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot31",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot32",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot33",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot34",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot35",
                        ColumnValue = "",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot36",
                        ColumnValue = "",
                    },
                     new DataHeaderModel()
                    {
                        ColumnKey = "cot37",
                        ColumnValue = "Taxable income O.T (OT tính thuế )",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot38",
                        ColumnValue = "Tax exempt O.T OT không tính thuế",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "cot39",
                        ColumnValue = "TOTAL OT VND Tổng OT",
                    },

                };

            //Trợ cấp cố định
            index = 0;
            listLoaiTroCapCoDinh.ForEach(loaiTroCap =>
            {
                index++;
                var dataRowLoaiTroCap = new DataHeaderModel();
                dataRowLoaiTroCap.ColumnKey = "indexCTCDHeader2_" + index;
                dataRowLoaiTroCap.ColumnValue = loaiTroCap.CategoryName;
                listHeader2.Add(dataRowLoaiTroCap);

            });
            //Thêm cột tính tổng ở cuối
            if (index == listLoaiTroCapCoDinh.Count())
            {
                index++;
                var dataRowLoaiTroCapHeader1 = new DataHeaderModel();
                dataRowLoaiTroCapHeader1.ColumnKey = "indexCTCDSumHeader2_" + index;
                dataRowLoaiTroCapHeader1.ColumnValue = "TOTAL GROSS ALLOWANCE (Cell phone, Housing allowance, lunch, attendance reward)";
                listHeader2.Add(dataRowLoaiTroCapHeader1);
            }
     
            //Trợ cấp khác
            index = 0;
            //for (var i = 0; i < 6; i++)
            //{
            //    index++;
            //    var dataRowLoaiTroCap = new DataHeaderModel();
            //    dataRowLoaiTroCap.ColumnKey = "index_ctkHeader2" + index;
            //    dataRowLoaiTroCap.ColumnValue = "100";
            //    listHeader2.Add(dataRowLoaiTroCap);
            //}
            listSelectedLoaiTroCapKhac.ForEach(loaiTroCap =>
            {
                index++;
                var dataRowLoaiTroCap = new DataHeaderModel();
                dataRowLoaiTroCap.ColumnKey = "index_ctkHeader2" + index;
                dataRowLoaiTroCap.ColumnValue = loaiTroCap.CategoryName;
                listHeader2.Add(dataRowLoaiTroCap);

            });
            //Thêm cột tính tổng ở cuối
            if (index == listLuongCtTroCapKhac.Count())
            {
                index++;
                var dataRowLoaiTroCapSum = new DataHeaderModel();
                dataRowLoaiTroCapSum.ColumnKey = "indexCTKSumHeader2_" + index;
                dataRowLoaiTroCapSum.ColumnValue = "TOTAL BONUS";
                listHeader2.Add(dataRowLoaiTroCapSum);
            }

            //cột số 52
            var dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot52";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot53";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot54";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot55";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot56";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot57";
            dataHeader2RowAll.ColumnValue = "Dependant number Số người phụ thuộc";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot58";
            dataHeader2RowAll.ColumnValue = "Tháng đăng ký giảm trừ gia cảnh";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot59";
            dataHeader2RowAll.ColumnValue = "Tổng số tháng giảm trừ trong năm";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot60";
            dataHeader2RowAll.ColumnValue = "Dependant Amount (4.4 million/ pax from Jul 2020) Số tiền giảm trừ/ 1 người phụ thuộc";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot61";
            dataHeader2RowAll.ColumnValue = "Personal Deduction (11 million/ pax from Jul 2020) Mức được giảm trừ cá nhân -11tr / 1 người";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot62";
            dataHeader2RowAll.ColumnValue = "Personal and Dependant Deduction Tổng tiền giảm trừ gia cảnh (Giảm trừ cá nhân +giảm trừ gia cảnh nếu có người phụ thuộc)";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot63";
            dataHeader2RowAll.ColumnValue = "Obligation 10.5%_Employee's side 10.5 % BHXH do người LĐ đóng";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot64";
            dataHeader2RowAll.ColumnValue = "Other deduction Các khoản giảm trừ khác nếu có";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot65";
            dataHeader2RowAll.ColumnValue = "13th monthy salary Lương tháng 13 - thường sẽ trả trước 15.01 hàng năm";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot66";
            dataHeader2RowAll.ColumnValue = "Tet gifts, Cash gift receiving during year-end meeting , Cash Tet bonus Quà tết hoặc các khoản thưởng tiền mặt, quà bốc thăm trúng thưởng chịu thuế TNCN";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot67";
            dataHeader2RowAll.ColumnValue = "Other Net to pay PIT/ Gross salary and bonus Các khoản khác có tính thuế TNCN";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot67";
            dataHeader2RowAll.ColumnValue = "'Convert from Net to Gross to pay Tax Các khoản chuyển đổi từ net sang Gross nếu có";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot69";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot70";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader1RowAll = new DataHeaderModel();
            dataHeader1RowAll.ColumnKey = "cot71";
            dataHeader1RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader1RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot72";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot73";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot74";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);


            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot75";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);


            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot76";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot77";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot78";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot79";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot80";
            dataHeader2RowAll.ColumnValue = "Company";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot81";
            dataHeader2RowAll.ColumnValue = "Employee";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot82";
            dataHeader2RowAll.ColumnValue = "TOTAL";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot83";
            dataHeader2RowAll.ColumnValue = "Company";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot84";
            dataHeader2RowAll.ColumnValue = "Employee";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot85";
            dataHeader2RowAll.ColumnValue = "TOTAL";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot86";
            dataHeader2RowAll.ColumnValue = "Company";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot87";
            dataHeader2RowAll.ColumnValue = "Employee";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot88";
            dataHeader2RowAll.ColumnValue = "TOTAL";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot89";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot90";
            dataHeader2RowAll.ColumnValue = "Company";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot91";
            dataHeader2RowAll.ColumnValue = "Employee";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot92";
            dataHeader2RowAll.ColumnValue = "TOTAL";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot93";
            dataHeader2RowAll.ColumnValue = "Company";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot94";
            dataHeader2RowAll.ColumnValue = "Employee";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot95";
            dataHeader2RowAll.ColumnValue = "TOTAL";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot96";
            dataHeader2RowAll.ColumnValue = "Company";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot97";
            dataHeader2RowAll.ColumnValue = "Employee";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot98";
            dataHeader2RowAll.ColumnValue = "TOTAL";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot99";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot100";
            dataHeader2RowAll.ColumnValue = "T.U_Company KPCĐ - công ty đóng hết 2 % trên lương cơ bản cho nhân viên";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot101";
            dataHeader2RowAll.ColumnValue = "Fund_Oct (2%)";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot102";
            dataHeader2RowAll.ColumnValue = "TOTAL";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot103";
            dataHeader2RowAll.ColumnValue = "T.U_Employee (1%) KPCĐ nếu có";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot104";
            dataHeader2RowAll.ColumnValue = "PIT liquidation Quyết toán thuế TNCN";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot105";
            dataHeader2RowAll.ColumnValue = "Other deduction after Tax Các khoản giảm trừ sau thuế khác";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot106";
            dataHeader2RowAll.ColumnValue = "TOTAL";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot107";
            dataHeader2RowAll.ColumnValue = "PIT Refund PIT last month Thuế TNCN hoàn lại nếu có";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot108";
            dataHeader2RowAll.ColumnValue = "Other add  after Tax Các khoản cộng thêm sau thuế";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot109";
            dataHeader2RowAll.ColumnValue = "Other add  after Tax Các khoản cộng thêm sau thuế";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot110";
            dataHeader2RowAll.ColumnValue = "TOTAL";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot111";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot112";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot113";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot114";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot115";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);

            dataHeader2RowAll = new DataHeaderModel();
            dataHeader2RowAll.ColumnKey = "cot116";
            dataHeader2RowAll.ColumnValue = "";
            listHeader2.Add(dataHeader2RowAll);
            listDataHeader.Add(listHeader2);
            #endregion

            #endregion
        }

        private static int GetSoNamKinhNghiemByEmployeeId(List<HopDongNhanSu> listAllHDNSEmp)
        {
            int soNamKinhNghiem = 0;
            var nowDate = DateTime.Now;

            //Chỉ lấy những hợp đồng có Ngày ký hợp đồng nhỏ hơn hoặc bằng thời điểm hiện tại
            var listHopDong = listAllHDNSEmp
                .Where(x => x.NgayBatDauLamViec.Date <= nowDate.Date).ToList();

            double tongSoNgay = 0;

            listHopDong.ForEach(item =>
            {
                if (item.NgayKetThucHopDong != null)
                {
                    tongSoNgay += (item.NgayKetThucHopDong.Value.Date - item.NgayBatDauLamViec.Date).TotalDays;
                }
                else
                {
                    tongSoNgay += (nowDate.Date - item.NgayBatDauLamViec.Date).TotalDays;
                }
            });

            soNamKinhNghiem = (int)(Math.Floor(tongSoNgay / 365));

            return soNamKinhNghiem;
        }

        private void GetDataBaoCaoOtChoTungLoaiOT(LuongCtLoaiTroCapOt existsOt, string type, LuongCtTroCapOtModel MucLuongHienTai, Decimal tiLeHuong,
            out string data, out Decimal totalOT, out Decimal totalTaxableInCome, out Decimal totalHour, in Decimal totalOTCurrent, in Decimal totalTaxableInComeCurrent, in Decimal totalHourCurrent)
        {
            data = "--";
            totalOT = totalOTCurrent;
            totalTaxableInCome = totalTaxableInComeCurrent;
            totalHour = totalHourCurrent;
            if (type == "_Hour")
            {
                totalHour += existsOt.SoGioOt;
                data = existsOt.SoGioOt.ToString();
            }
            else if (type == "_@")
            {
                data = (tiLeHuong * MucLuongHienTai.MucLuongHienTai).ToString();
            }
            else if (type == "_OT")
            {
                var ot = (tiLeHuong * MucLuongHienTai.MucLuongHienTai * existsOt.SoGioOt/100);
                totalOT += ot;
                data = ot.ToString();
            }
            else if (type == "_Taxable")
            {
                var tax = (MucLuongHienTai.MucLuongHienTai * existsOt.SoGioOt);
                totalTaxableInCome += tax;
                data = tax.ToString();
            }
        }

        private void TongKetBaoCaoTongHopOT(out List<DataRowModel> listDataRowTK, List<DateTime> listDate, List<ChamCongOt> listChamCongOt, Guid loaiOtId, string text, string otCode, List<Category> listLoaiOt)
        {
            listDataRowTK = new List<DataRowModel>();

            var dataRowTK = new DataRowModel();
            dataRowTK.ColumnKey = "stt";
            dataRowTK.ColumnValue = GetLoaiOTCode(otCode) + ": " + text;
            listDataRowTK.Add(dataRowTK);

            var dataRow2TK = new DataRowModel();
            dataRow2TK.ColumnKey = "code";
            dataRow2TK.ColumnValue = "";
            listDataRowTK.Add(dataRow2TK);

            var dataRow3TK = new DataRowModel();
            dataRow3TK.ColumnKey = "name";
            dataRow3TK.ColumnValue = "";
            listDataRowTK.Add(dataRow3TK);

            var dataRowNgayBatDauTK = new DataRowModel();
            dataRowNgayBatDauTK.ColumnKey = "codeMayChamCong";
            dataRowNgayBatDauTK.ColumnValue = "";
            listDataRowTK.Add(dataRowNgayBatDauTK);

            int indexTK = 0;
            var listDataRowTkDate = new List<DataRowModel>();
            listDate.ForEach(data =>
            {
                var chamCongLoop = listChamCongOt.Where(x => x.NgayChamCong.Date == data.Date).ToList();
                indexTK++;
                var _vaoSang = new DataRowModel();
                _vaoSang.ColumnKey = "index_" + indexTK;
                _vaoSang.ColumnValue = chamCongLoop.Where(x => x.LoaiOtId == loaiOtId).Count().ToString();
                listDataRowTkDate.Add(_vaoSang);
            });

            listLoaiOt.ForEach(loaiOT =>
            {
                if(loaiOT.CategoryId == loaiOtId)
                {
                    var dataRow5x = new DataRowModel();
                    dataRow5x.ColumnKey = loaiOT.CategoryCode;
                    dataRow5x.ColumnValue = listChamCongOt.Where(x => x.LoaiOtId == loaiOtId).Count().ToString();
                    listDataRowTkDate.Add(dataRow5x);
                }
                else
                {
                    var dataRow5x = new DataRowModel();
                    dataRow5x.ColumnKey = loaiOT.CategoryCode;
                    dataRow5x.ColumnValue = "0";
                    listDataRowTkDate.Add(dataRow5x);
                }
            });

            listDataRowTK.AddRange(listDataRowTkDate);

            var dataRow4TK = new DataRowModel();
            dataRow4TK.ColumnKey = "ngayLamViecThucTe";
            dataRow4TK.ColumnValue = "";
            listDataRowTK.Add(dataRow4TK);

            var dataRow5TK = new DataRowModel();
            dataRow5TK.ColumnKey = "congTac";
            dataRow5TK.ColumnValue = "";
            listDataRowTK.Add(dataRow5TK);
        }

        private string GetLoaiOTCode(string loaiOtCode)
        {
            string result = "--";
            if(loaiOtCode == "OTNGAYLE")
            {
                result = "C1";
            }
            else if (loaiOtCode == "CTUAN")
            {
                result = "C2";
            }
            else if (loaiOtCode == "TTUAN")
            {
                result = "C3";
            }
            else if (loaiOtCode == "OTDEM_HLD")
            {
                result = "C4";
            }
            else if (loaiOtCode == "OTDEM_WK")
            {
                result = "C5";
            }
            else if (loaiOtCode == "OTDEM_ND")
            {
                result = "C6";
            }
            else if (loaiOtCode == "OTDEM_NMW")
            {
                result = "C7";
            }
            return result;
        }

    }
}



