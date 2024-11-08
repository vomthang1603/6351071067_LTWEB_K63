using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBANSACH.Models
{
    public class GioHang
    {
        public int Masach { get; set; }
        public string Tensach { get; set; }
        public string Anhbia { get; set; }
        public decimal Dongia { get; set; }
        public int Soluong { get; set; }
        public decimal ThanhTien
        {
            get { return Dongia * Soluong; }
        }
    }
}