using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using System.Windows.Media.Imaging;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "su,operator")]
    public class BrandController : Controller
    {
        //
        // GET: /Manager/Brand/

        public ActionResult Index()
        {
            List<BrandItem> Brands = BrandManager.GetAllUsedBrands();
            return View(Brands);
        }

        /// <summary>管理员请求创建新品牌的操作处理
        /// </summary>
        /// <returns>创建新品牌的结果</returns>
        public ActionResult Create(BrandItem brand)
        {
            BrandManager.AddNewBrand(brand);
            if (brand != null)
            {
                return Json(brand);
            }
            else
            {
                return Content("error");
            }
        }

        /// <summary>更新品牌的LOGO的处理
        /// </summary>
        /// <returns>更新品牌的LOGO的结果View</returns>
        public ActionResult UpdateLogo(int id)
        {
            HttpPostedFileBase file = Request.Files[0];
            if (!file.ContentType.Contains("image") || file.ContentLength > 65535)
            {
                return Content("error");
            }

            //如果获取该ID的品牌信息失败，返回错误
            BrandItem brand = BrandManager.GetBrandByID(id);
            if (brand == null)
            {
                return Content("error");
            }

            //上传的LOGO图片必须是100*50的大小
            string mimetype = file.ContentType;
            BitmapDecoder decoder = BitmapDecoder.Create(file.InputStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            if (decoder.Frames[0].PixelWidth != 100 || decoder.Frames[0].PixelHeight != 50)
            {
                return Content("error - must be 100*50 dimension");
            }

            string logo_filename = BrandManager.GetLogoFileName(brand, mimetype);
            file.SaveAs(HttpContext.Server.MapPath("~/Images/logo/" + logo_filename));

            bm.UpdateBrandImage(brand, logo_filename);
            return Content("ok-" + logo_filename);
        }
    }
}
