using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SachOnline.Models;
using PagedList;
using PagedList.Mvc;
using System.Web.UI;

namespace SachOnline.Controllers
{
    public class SachOnlineController : Controller
    {
        private string connection;
        private dbSachOnlineDataContext data;

        public SachOnlineController()
        {
            // Khởi tạo chuỗi kết nối
            connection = "Data Source=(local);Initial Catalog=SachOnline;Integrated Security=True";
            data = new dbSachOnlineDataContext(connection);
        }

        private List<SACH> LaySachMoi(int count)
        {
            return data.SACHes.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        // GET: SachOnline
        public ActionResult Index()
        {
            //Lay 6 quyen sach moi
            var listSachMoi = LaySachMoi(6);
            return View(listSachMoi);
        }

        public ActionResult SachBanNhieuPartial()
        {
            // Truy vấn cơ sở dữ liệu để lấy danh sách sách bán nhiều nhất.
            var listSachBanNhieu = data.SACHes.OrderByDescending(a => a.SoLuongBan).Take(6).ToList();

            // Trả về PartialView chứa danh sách sách bán nhiều nhất.
            return PartialView("SachBanNhieuPartial", listSachBanNhieu);
        }

        public ActionResult ChuDePartial()
        {
            var listChuDe = from cd in data.CHUDEs select cd;
            return PartialView(listChuDe);
        }
        public ActionResult NhaXuatBanPartial()
        {
            var listNhaXuatBan = from cd in data.NHAXUATBANs select cd;
            return PartialView(listNhaXuatBan);
        }
        // GET: FooterPartial
        public ActionResult FooterPartial()
        {
            return PartialView();
        }
        // GET: NavPartial
        public ActionResult NavPartial()
        {
            return PartialView();
        }
        // GET: SliderPartial
        public ActionResult SliderPartial()
        {
            return PartialView();
        }

        // Lab3
        private string LayTenChuDe(int id)
		{
			var chuDe = data.CHUDEs.SingleOrDefault(cd => cd.MaCD == id);

			if (chuDe != null)
			{
				return chuDe.TenChuDe;
			}

			return "Không tìm thấy chủ đề";
		}
		public ActionResult SachTheoChuDe(int id, int? page)
		{
			int pageSize = 1;
			int pageNumber = page ?? 1;

			var sach = data.SACHes.Where(s => s.MaCD == id)
								  .OrderByDescending(s => s.NgayCapNhat)
								  .ToPagedList(pageNumber, pageSize);

			ViewBag.Id = id;
			ViewBag.ten = LayTenChuDe(id);

			return View(sach);
		}
		private string LayTenNhaXuatBan(int id)
		{
			var nhaxuatban = data.NHAXUATBANs.SingleOrDefault(cd => cd.MaNXB == id);

			if (nhaxuatban != null)
			{
				return nhaxuatban.TenNXB;
			}

			return "Không tìm thấy nhà xuất bản";
		}
		public ActionResult SachTheoNhaXuatBan(int id, int? page)
		{
			int pageSize = 3;
			int pageNumber = page ?? 1;

			var sach = data.SACHes.Where(s => s.MaNXB == id)
								  .OrderByDescending(s => s.NgayCapNhat)
								  .ToPagedList(pageNumber, pageSize);

			ViewBag.Id = id;
			ViewBag.ten = LayTenNhaXuatBan(id);
			return View(sach);
		}
		public ActionResult ChiTietSach(int id)
        {
            var sach = from s in data.SACHes
                       where s.MaSach == id
                       select s;
            return View(sach.Single());
        }
        public ActionResult LoginLogout()
        {
            return PartialView ("LoginLogoutPartial");
        }
    }
}