using QLBANSACH.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace QLBANSACH.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly QLBANSACHEntities1 db = new QLBANSACHEntities1();
        // GET: KhachHang
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp email
                var checkEmail = db.KHACHHANGs.FirstOrDefault(u => u.Email == model.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra trùng lặp tài khoản
                var checkTaiKhoan = db.KHACHHANGs.FirstOrDefault(u => u.Taikhoan == model.TaiKhoan);
                if (checkTaiKhoan != null)
                {
                    ModelState.AddModelError("TaiKhoan", "Tài khoản đã tồn tại.");
                    return View(model);
                }

                var customer = new KHACHHANG
                {
                    Taikhoan = model.TaiKhoan,
                    Email = model.Email,
                    Matkhau = model.MatKhau,
                    HoTen = model.HoTen,
                    DiachiKH = model.DiaChiKH,
                    DienthoaiKH = model.DienThoaiKH,
                    Ngaysinh = model.Ngaysinh
                };

                db.KHACHHANGs.Add(customer);
                db.SaveChanges();

                return RedirectToAction("DangNhap", "NguoiDung");
            }
            return View(model);
        }


        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var khachhang = db.KHACHHANGs.FirstOrDefault(kh => kh.Taikhoan == model.TaiKhoan);
                if (khachhang != null)
                {
                    if (khachhang.Matkhau == model.MatKhau)
                    {
                        Session["TaiKhoan"] = khachhang;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu không đúng.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên tài khoản không tồn tại.");
                }
            }
            return View(model);
        }


        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}