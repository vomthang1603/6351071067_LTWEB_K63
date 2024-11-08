using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBANSACH.Controllers
{
    public class GioHangController : Controller
    {
        private readonly QLBANSACHEntities1 db = new QLBANSACHEntities1();

        // GET: GioHang
        public ActionResult Index()
        {
            var giohang = LayGioHang();
            decimal TongTienGioHang = giohang.Sum(i => i.ThanhTien);
            if (giohang != null)
            {
                ViewBag.TongTien = giohang.Sum(g => g.ThanhTien);
            }
            else
            {
                ViewBag.TongTien = 0;
            }

            return View(giohang);
        }

        // Lấy giỏ hàng từ session
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        // Thêm sách vào giỏ hàng
        public ActionResult ThemGioHang(int MaSach)
        {
            var sach = db.SACHes.FirstOrDefault(u => u.Masach == MaSach);
            if (sach == null)
            {
                return HttpNotFound();
            }

            var giohang = LayGioHang();
            var item = giohang.FirstOrDefault(i => i.Masach == MaSach);
            if (item != null)
            {
                item.Soluong++; // Tăng số lượng nếu sách đã có trong giỏ
            }
            else
            {
                giohang.Add(new GioHang
                {
                    Masach = sach.Masach,
                    Tensach = sach.Tensach,
                    Anhbia = sach.Anhbia,
                    Dongia = sach.Giaban ?? 0, // Nếu Giaban null thì thay bằng 0
                    Soluong = 1
                });
            }

            // Cập nhật giỏ hàng vào session
            Session["GioHang"] = giohang;

            return RedirectToAction("Index", "GioHang"); // Sau khi thêm sách, chuyển hướng về trang giỏ hàng
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public ActionResult XoaGioHang(int MaSach)
        {
            var giohang = LayGioHang();
            var item = giohang.FirstOrDefault(i => i.Masach == MaSach);
            if (item != null)
            {
                giohang.Remove(item); // Xóa sản phẩm khỏi giỏ hàng
                Session["GioHang"] = giohang;
            }

            return RedirectToAction("Index", "GioHang");
        }

        public ActionResult CapNhatGioHang(int MaSach, int soLuong)
        {
            var giohang = LayGioHang();
            var item = giohang.FirstOrDefault(i => i.Masach == MaSach);
            if (item != null)
            {
                item.Soluong = soLuong;
                if (item.Soluong <= 0)
                {
                    giohang.Remove(item);
                }
            }

            Session["GioHang"] = giohang;
            return RedirectToAction("Index", "GioHang");
        }


        public ActionResult GioHangPartial()
        {
            var giohang = LayGioHang();
            var soluongsp = giohang.Sum(i => i.Soluong);
            return PartialView("GioHangPartial", soluongsp);
        }

        // GET: Trang để xem thông tin đơn hàng và giỏ hàng
        public ActionResult DatHang()
        {
            var gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang == null || gioHang.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "GioHang");
            }

            var user = Session["TaiKhoan"] as KHACHHANG;
            if (user == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            // Tạo DatHangViewModel để truyền dữ liệu vào view
            var model = new DatHangViewModel
            {
                KHACHHANG = user,
                GioHang = gioHang,
                NgayDat = DateTime.Now,
                NgayGiao = DateTime.Now.AddDays(3),
                TongSoLuong = gioHang.Sum(item => item.Soluong),
                TongTien = gioHang.Sum(item => item.ThanhTien)
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult DatHang(DatHangViewModel model)
        {
            // Kiểm tra giỏ hàng
            var gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang == null || gioHang.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "GioHang");
            }

            // Kiểm tra tài khoản người dùng
            var user = Session["TaiKhoan"] as KHACHHANG;
            if (user == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra và gán giá trị hợp lệ cho Ngaydat và Ngaygiao
            DateTime ngayDat = model.NgayDat;
            DateTime ngayGiao = model.NgayGiao;

            // Nếu NgayDat là MinValue, gán giá trị là ngày hiện tại
            if (ngayDat == DateTime.MinValue)
            {
                ngayDat = DateTime.Now;
            }
            // Nếu NgayGiao là MinValue, gán giá trị là ngày hiện tại cộng thêm 3 ngày (hoặc thời gian mong muốn)
            if (ngayGiao == DateTime.MinValue)
            {
                ngayGiao = DateTime.Now.AddDays(3);
            }

            // Tạo đơn hàng mới
            var donDatHang = new DONDATHANG
            {
                MaKH = user.MaKH,
                Ngaydat = ngayDat,
                Ngaygiao = ngayGiao,
                Dathanhtoan = false,  // Chưa thanh toán
                Tinhtranggiaohang = false  // Chưa giao
            };

            db.DONDATHANGs.Add(donDatHang);

            try
            {
                db.SaveChanges();  // Lưu đơn hàng
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                // Log lỗi ra console hoặc file log để kiểm tra chi tiết
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Lỗi lưu đơn hàng. Vui lòng thử lại.";
                return RedirectToAction("Index", "GioHang");
            }

            // Thêm các chi tiết đơn hàng
            foreach (var item in gioHang)
            {
                var chiTiet = new CHITIETDONTHANG
                {
                    MaDonHang = donDatHang.MaDonHang,
                    Masach = item.Masach,
                    Soluong = item.Soluong,
                    Dongia = item.Dongia
                };

                db.CHITIETDONTHANGs.Add(chiTiet);
            }

            db.SaveChanges();  // Lưu chi tiết đơn hàng

            // Xóa giỏ hàng sau khi đặt
            Session["GioHang"] = null;

            TempData["Message"] = "Đặt hàng thành công.";
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}
