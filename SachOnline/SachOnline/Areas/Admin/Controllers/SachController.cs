using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using Antlr.Runtime.Tree;

namespace SachOnline.Areas.Admin.Controllers
{
    public class SachController : Controller
    {
        dbSachOnlineDataContext db = new dbSachOnlineDataContext("Data Source=(local);Initial Catalog=SachOnline;Integrated Security=True");
        // GET: Admin/Sach
        public ActionResult Index(int ? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.SACHes.ToList().OrderBy(n => n.MaSach).ToPagedList(iPageNum, iPageSize));
        }
        // GET: Create
        [HttpGet]
        public ActionResult Create()
        {
            // Lay danh sach tu cac table ChuDe, NXB, Hien thi ten, khi chon se lay ma
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(SACH sach, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            // Dua du lieu vao Dropdown
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaCD", "TenChuDe");
            if (fFileUpload == null) {
                // Noi dung thong bao yeu chon anh bia
                ViewBag.ThongBao = "Hãy chọn ảnh bìa";
                // Luu thong tin de de khi load lai trang do yeu cau chon anh bia se hien thi cac thong tin nay len trang
                ViewBag.TenSach = f["sTenSach"];
                ViewBag.MoTa = f["sMoTa"];
                ViewBag.SoLuong = int.Parse(f["iSoLuong"]);
                ViewBag.GiaBan = decimal.Parse(f["mGiaBan"]);
                ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", int.Parse(f["MaCD"]));
                ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", int.Parse(f["MaNXB"]));
                return View();
            } else
            {
                if (ModelState.IsValid)
                {
                    //Lấy tên file (Khai báo thư viện: System.10)
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    //Lấy đường dẫn lưu file
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                    //Kiểm tra anh bìa đã tôn tại chưa để lưu lên thư mục
                    if (!System.IO.File.Exists(path)) {
                        fFileUpload.SaveAs(path);
                    }
                    //Lưu Sach vào CSDL
                    sach.TenSach = f["sTenSach"];
                    sach.MoTa = f["sMoTa"];
                    sach.AnhBia = sFileName;
                    sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                    sach.SoLuongBan = int.Parse(f["iSoLuong"]);
                    sach.GiaBan = decimal.Parse(f["mGiaBan"]);
                    sach.MaCD = int.Parse(f["MaCD"]);
                    sach.MaNXB = int.Parse(f["MaNXB"]);
                    db.SACHes.InsertOnSubmit(sach); 
                    db.SubmitChanges();
                    //Ve Lại trang Quản lý sách
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        // Get: Details
        public ActionResult Details(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        // Get: Delete
        public ActionResult Delete(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteComfirm(int id, FormCollection f)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);

            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var ctdh = db.CHITIETDATHANGs.Where(ct => ct.MaSach == id);
            if (ctdh.Count() > 0)
            {
                //Nội dung sẽ hiển thị khi sách cần xóa đã có trong table ChiTietDonHang
                ViewBag.ThongBao = "Sách này đang có trong bảng Chi tiết đặt hàng <br>" + " Nếu muốn xóa thì phải xóa hết mã sách này trong bảng Chi tiết đặt hàng";
                return View(sach);
            }
            //Xóa hết thông tin của cuốn sách trong table VietSach trước khi xóa sách này
            var vietsach = db.VIETSACHes.Where(vs => vs.MaSach == id).ToList();
            if (vietsach != null)
            {
                db.VIETSACHes.DeleteAllOnSubmit(vietsach);
                db.SubmitChanges();
            }
            //Xóa sách
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        // Edit
        [HttpGet]

        public ActionResult Edit(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == int.Parse(f["iMaSach"]));
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            if (ModelState.IsValid)
            {
                if (fFileUpload != null)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    sach.AnhBia = sFileName;
                }
                // Luu sach vao CSDL
                sach.TenSach = f["sTenSach"];
                sach.MoTa = f["sMoTa"];
                sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                sach.SoLuongBan = int.Parse(f["iSoLuong"]);
                sach.GiaBan = decimal.Parse(f["mGiaBan"]);
                sach.MaCD = int.Parse(f["MaNXB"]);
                db.SubmitChanges();
                // Ve lai quan ly sach
                return RedirectToAction("Index");
            }
            return View(sach);
        }
    }
}