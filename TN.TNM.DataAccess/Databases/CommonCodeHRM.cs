using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Databases
{
    public static class CommonCodeHRM
    {

        public static class QuanHeVoiNhanVien
        {
            public const string Vo = "Vo";                      //Vợ
            public const string Chong = "Chong";                //Chồng
            public const string Bo = "Bo";                      //Bố
            public const string Me = "Me";                      //Mẹ
            public const string AnhEmTrai = "AnhEmTrai";        //Anh/Em Trai
            public const string ChiEmGai = "ChiEmGai";          //Chị/Em Gái
            public const string Ong = "Ong";                    //Ông
            public const string Ba = "Ba";                      //Bà
            public const string Con = "Con";                    //Con
        }

        public static class LoaiGiayTo
        {
            public const string SYLL = "SYLL";                      //Sơ yếu lý lịch công chứng
            public const string CMTND = "CMTND";                    //CMTND công chứng
            public const string GKSK = "GKSK";                      //Giấy khám sức khỏe
            public const string GKS = "GKS";                        //Giấy khai sinh công chứng
            public const string SoBHXH = "SoBHXH";                  //Sổ BHXH
            public const string SoHoKhau = "SoHoKhau";              //Sổ hộ khẩu công chứng
            public const string Anh = "Anh";                        //Ảnh
            public const string BangCap = "BangCap";                //Bằng Cấp
            public const string QDNV = "QDNV";                      //Quyết định nghỉ việc
            public const string HDLDCu = "HDLDCu";                  //HĐLĐ các công ty cũ
            public const string SaoKeTK = "SaoKeTK";                //Sao kê tài khoản
            public const string NguoiNhanXN = "NguoiNhanXN";        //Người nhận xác nhận
        }

//--Thêm bảng Cấu hình bảo hiểm LoftCare: CauHinhBaoHiemLoftCare -----------------------------------
//--Enum loại mức đóng 3 loại:
//--- 1: Mức đóng cố định
//--- 2: Mức đóng theo lương
//--- 3: Mức phí giảm

//-------Thêm bảng cấu hình trợ cấp: TroCap -----------------------------------
//--1: Trợ cấp đi lại
//--2: Trợ cấp điện thoại
//--3: Trợ cấp ăn trưa
//--4: Trợ cấp chuyên cần ngày công
//--5: Trợ cấp chuyên cần đi muộn về sớm
    }

}
