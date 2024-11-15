using QLBANSACH.Models;
using PagedList;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Data.Entity;
using QLBANSACH.Models;
using System.Data;


namespace QLBANSACH.Controllers
{
    public class AdminController : Controller
    {
        private QLBANSACHEntities1 db = new QLBANSACHEntities1();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        // POST: Admin/Login
        [HttpPost]
        public ActionResult Login(string UserAdmin, string PassAdmin)
        {
            if (string.IsNullOrEmpty(UserAdmin))
            {
                ModelState.AddModelError("UserAdmin", "Username is required.");
            }

            if (string.IsNullOrEmpty(PassAdmin))
            {
                ModelState.AddModelError("PassAdmin", "Password is required.");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var admin = db.Admins.FirstOrDefault(a => a.UserAdmin == UserAdmin && a.PassAdmin == PassAdmin);

            if (admin != null)
            {
                if (admin.UserAdmin == "admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (admin.UserAdmin == "user")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid user role.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View();
        }


        public ActionResult Sach(int? page)
        {
            int pageSize = 3; // Số sách trên mỗi trang
            int pageNumber = (page ?? 1); // Lấy trang hiện tại

            var books = db.SACHes
                          .Include(s => s.CHUDE) // Nạp chủ đề
                          .Include(s => s.NHAXUATBAN) // Nạp nhà xuất bản
                          .OrderBy(s => s.Masach) // Sắp xếp theo Mã sách
                          .ToPagedList(pageNumber, pageSize); // Phân trang

            return View(books);
        }


        // GET: Admin/ThemmoiSach
        [HttpGet]
        public ActionResult ThemmoiSach()
        {
            // Fetch categories (MaCD) from the database and prepare the SelectList
            var chude = db.CHUDEs.Select(c => new { c.MaCD, c.TenChuDe }).ToList();
            ViewData["MaCD"] = new SelectList(chude, "MaCD", "TenChuDe");  // Correct key used for SelectList

            // Fetch publishers (MaNXB) from the database and prepare the SelectList
            var nxb = db.NHAXUATBANs.Select(n => new { n.MaNXB, n.TenNXB }).ToList();
            ViewData["MaNXB"] = new SelectList(nxb, "MaNXB", "TenNXB");  // Correct key used for SelectList

            return View();
        }


        // POST: Admin/ThemmoiSach
        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SACH sach, HttpPostedFileBase fileUpLoad)
        {
            if (ModelState.IsValid)
            {
                sach.Ngaycapnhat = DateTime.Now;
                if (fileUpLoad != null && fileUpLoad.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(fileUpLoad.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Hinhsanpham"), fileName);
                    fileUpLoad.SaveAs(path);
                    sach.Anhbia = fileName; // Lưu tên ảnh
                }
                db.SACHes.Add(sach);
                db.SaveChanges();
                return RedirectToAction("Sach");
            }

            ViewData["MaCD"] = new SelectList(db.CHUDEs, "MaCD", "TenCD", sach.MaCD);
            ViewData["MaNXB"] = new SelectList(db.NHAXUATBANs, "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }


        // GET: Admin/Detail/5
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sach = db.SACHes
                         .Include(s => s.CHUDE)
                         .Include(s => s.NHAXUATBAN)
                         .SingleOrDefault(s => s.Masach == id);

            if (sach == null)
            {
                return HttpNotFound();
            }

            return View(sach);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SACH sach = db.SACHes.Include(s => s.CHUDE).FirstOrDefault(s => s.Masach == id);
            if (sach == null)
            {
                return HttpNotFound();
            }

            var chude = db.CHUDEs.Select(c => new { c.MaCD, c.TenChuDe }).ToList();
            ViewData["MaCD"] = new SelectList(chude, "MaCD", "TenChuDe", sach.MaCD);
            ViewData["MaNXB"] = new SelectList(db.NHAXUATBANs, "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }


        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SACH sach, HttpPostedFileBase fileUpLoad)
        {
            if (ModelState.IsValid)
            {
                if (fileUpLoad != null && fileUpLoad.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(fileUpLoad.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    fileUpLoad.SaveAs(path);
                    sach.Anhbia = fileName; // Lưu tên ảnh mới
                }

                db.Entry(sach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Sach");
            }

            ViewData["MaCD"] = new SelectList(db.CHUDEs, "MaCD", "TenCD", sach.MaCD);
            ViewData["MaNXB"] = new SelectList(db.NHAXUATBANs, "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SACH sach = db.SACHes
                          .Include(s => s.CHUDE)
                          .Include(s => s.NHAXUATBAN)
                          .SingleOrDefault(n => n.Masach == id);

            if (sach == null)
            {
                return HttpNotFound();
            }

            return View(sach);
        }



        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SACH sach = db.SACHes.Find(id);
            if (sach != null)
            {
                string imagePath = Path.Combine(Server.MapPath("~/Content/Images/Hinhsanpham"), sach.Anhbia);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                db.SACHes.Remove(sach);
                db.SaveChanges();
            }

            return RedirectToAction("Sach");  // Sau khi xóa, điều hướng về trang danh sách sách
        }
        public ActionResult ThongKeSach()
        {
            var statistics = db.SACHes
                        .GroupBy(s => s.MaCD) // Nhóm sách theo chủ đề
                        .Select(g => new
                        {
                            TenChuDe = g.FirstOrDefault().CHUDE.TenChuDe, // Lấy tên chủ đề
                            SoLuong = g.Count() // Đếm số lượng sách trong mỗi nhóm
                        })
                        .ToList();

            // Trả dữ liệu về view
            return View(statistics);
        }
    }

}