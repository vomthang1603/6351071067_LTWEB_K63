using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBANSACH.Models
{
    public class DatHangViewModel
    {
        public KHACHHANG KHACHHANG { get; set; }  // Thông tin người dùng
        public List<GioHang> GioHang { get; set; }  // Danh sách giỏ hàng
        public int TongSoLuong { get; set; }  // Tổng số lượng sản phẩm
        public decimal TongTien { get; set; }  // Tổng tiền giỏ hàng
        public DateTime NgayDat { get; set; }  // Ngày đặt hàng
        public DateTime NgayGiao { get; set; }  // Ngày giao hàng
    }
}