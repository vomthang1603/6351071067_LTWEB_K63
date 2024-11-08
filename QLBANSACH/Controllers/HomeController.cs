using System;
using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using QLBANSACH.Models;
using static LinqToDB.Common.Configuration;


namespace QLBANSACH.Controllers
{
    public class HomeController : Controller
    {
        private QLBANSACHEntities1 db = new QLBANSACHEntities1();
        public ActionResult Index()
        {
            var Sach = db.SACHes.ToList();
            return View(Sach);

        }
        
        public PartialViewResult Category()
        {
            var category = from cd in db.CHUDEs select cd;
            return PartialView(category);
        }

        public PartialViewResult NhaXuatBan()
        {
            var nhaxuatban = from cd in db.NHAXUATBANs select cd;
            return PartialView(nhaxuatban);
        }

        public ActionResult SPtheochude(int id)
        {
            var sach = db.SACHes.Where(s => s.MaCD == id).ToList();
            return View(sach);
        }
        

        public ActionResult SPtheonhaxuatban(int id)
        {
            var sach = db.SACHes.Where(s => s.MaNXB == id).ToList();
            return View(sach);
        }

        public ActionResult Details(int id)
        {
            var sach = db.SACHes.FirstOrDefault(s => s.Masach == id);
            return View(sach);
        }


        public ActionResult Search()
        {
            return View();
        }
    }
}
