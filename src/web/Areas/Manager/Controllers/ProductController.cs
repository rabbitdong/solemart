using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media.Imaging;
using Solemart.EntityLib;
using Solemart.BusinessLib;
using System.IO;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "su,operator")]
    public class ProductController : Controller
    {
        /// <summary>产品管理的实例
        /// </summary>
        private ProductManager pm = ProductManager.Instance;
        //
        // GET: /Manager/ProductManager/

        public ActionResult Index()
        {
            int pi = 1; //表示页索引
            if (Request["p"] != null && int.TryParse(Request["p"], out pi)) ;

            List<Product> PagedProdList = ProductManager.Instance.GetPagedProduct(pi - 1);

            ViewData["PageCount"] = pm.ProductPagedCount;
            ViewData["CurrentPageIndex"] = pi;

            return View(PagedProdList);
        }

        /// <summary>用户请求修改产品信息的处理
        /// </summary>
        /// <param name="id">要修改的产品的ID</param>
        /// <returns>修改产品的视图</returns>
        public ActionResult Modify(int id) {
            Product CurrentProduct = ProductManager.Instance.GetProductByID(id);

            return View(CurrentProduct);
        }

        /// <summary>用户提交修改产品信息的处理
        /// </summary>
        /// <param name="id">要修改的产品的ID</param>
        /// <returns>返回修改的结果View</returns>
        public ActionResult CommitModify(int id) {
            Product CurrentProduct = ProductManager.Instance.GetProductByID(id);
            string pname = Request["pname"];
            int cateid = 0;
            string unit = Request["unit"];
            string spec = Request["spec"];
            string desc = Request["desc"];

            if (pname == null || pname.Trim() == "" || unit == null || unit.Trim() == ""
                || Request["cateid"] == null || !int.TryParse(Request["cateid"], out cateid)) {
                return Content("error");
            }

            if (CurrentProduct.Name != pname)
                CurrentProduct.Name = pname;
            if (CurrentProduct.OwnedCategory.CateID != cateid)
                CurrentProduct.OwnedCategory = CategoryManager.Instance.GetCateById(cateid);
            if (CurrentProduct.Unit != unit)
                CurrentProduct.Unit = unit;

            CurrentProduct.Desc = desc;
            CurrentProduct.Spec = spec;
            if (pm.ModifyProductInfo(CurrentProduct)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        /// <summary>删除产品的请求处理
        /// </summary>
        /// <param name="id">要处理的产品的ID</param>
        /// <returns>删除图片的结果</returns>
        public ActionResult DeleteProductImage(int id) {
            Product CurrentProduct = ProductManager.Instance.GetProductByID(id);
            if (CurrentProduct == null) {
                return Content("error");
            }

            int iid = 0;
            if (!int.TryParse(Request["iid"], out iid)) {
                return Content("error");
            }

            ProductImage img = CurrentProduct.Images.First(a => a.ImgID == iid);
            if (img != null && pm.DeleteProductImage(CurrentProduct, iid)) {
                System.IO.File.Delete(Server.MapPath("~/images/product/normal/" + img.Url));
                System.IO.File.Delete(Server.MapPath("~/images/product/thumb/" + img.Url));
                return Content("ok");
            }
            else
                return Content("error");
        }

        /// <summary>添加一个产品的图片处理
        /// </summary>
        /// <param name="id">要添加的产品的ID</param>
        /// <returns>添加的产品的结果View</returns>
        public ActionResult AddNewProductImage(int id) {
            Product CurrentProduct = ProductManager.Instance.GetProductByID(id);
            if (CurrentProduct == null) {
                return Content("error");
            }

            HttpPostedFileBase file = Request.Files[0];
            if (!file.ContentType.Contains("image") || file.ContentLength > 65535) {
                return Content("error");
            }

            string mimetype = file.ContentType;
            string file_name = pm.GenerateProductImageFileName(CurrentProduct.ProductID,
                pm.FromMimeTypeGetExtendName(mimetype));

            BitmapDecoder decoder = BitmapDecoder.Create(file.InputStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            if (decoder.Frames[0].PixelWidth != 350 || decoder.Frames[0].PixelHeight != 350) {
                Response.Write("error - must be 350*350 dimension");
                Response.End();
            }

            /* 生成小图标 */
            TransformedBitmap thumb_img = new TransformedBitmap(decoder.Frames[0], new System.Windows.Media.ScaleTransform(0.2, 0.2));
            BitmapEncoder encoder = BitmapEncoder.Create(decoder.CodecInfo.ContainerFormat);
            FileStream thumb_file = new FileStream(HttpContext.Server.MapPath("~/images/product/thumb/" + file_name), FileMode.Create);
            encoder.Frames.Add(BitmapFrame.Create(thumb_img));
            encoder.Save(thumb_file);

            file.SaveAs(HttpContext.Server.MapPath("~/Images/product/normal/" + file_name));

            if (pm.AddNewImageToProduct(CurrentProduct.ProductID, file.ContentType, file_name)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        /// <summary>获取商品的最后入库价格
        /// </summary>
        /// <param name="id">要获取的商品ID</param>
        /// <returns>返回最后价格结果</returns>
        public ActionResult GetLastStockPrice(int id) {
            decimal price = 0.0m;
            price = pm.GetLastStockPrice(id);
            price = price + price / 2;

            return Content(price.ToString("C2"));
        }

        /// <summary>商品下架的请求的处理
        /// </summary>
        /// <param name="id">要下架的商品的ID</param>
        /// <returns>商品下架的结果</returns>
        public ActionResult GetBackSaling(int id) {
            if (pm.GetBackSaling(id)) {
                return Content("ok");
            }

            return Content("error");
        }

        /// <summary>商品上架的请求的处理
        /// </summary>
        /// <param name="id">要上架的商品的ID</param>
        /// <returns>商品上架的结果</returns>
        public ActionResult PutToSaling(int id) {
            decimal price = 0.0m;
            int discount = 100;

            if (!decimal.TryParse(Request["price"], out price)
                || !int.TryParse(Request["discount"], out discount)) {
                return Content("error");
            }


            bool is_spec_flag = Request["isspec"] == "true";
            if (pm.PutToSaling(id, price, discount, is_spec_flag)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        /// <summary>商品入库的处理
        /// </summary>
        /// <param name="id">要入库的商品ID</param>
        /// <returns>入库的结果</returns>
        public ActionResult InStock(int? id) {
            if (id == null) {
                return View();
            }
            else {
                Product ChangedProduct = pm.GetProductByID(id.Value);
                if(ChangedProduct == null)
                    return Content("error");
                return View(ChangedProduct);
            }
        }

        /// <summary>入库一个新产品
        /// </summary>
        /// <returns>返回入库的结果</returns>
        public ActionResult InstockNewProduct() {
            string prod_name = Request["prod_name"];
            decimal price = 0.0m;
            int cate_id = -1;
            int brand_id = -1;
            int vendor_id = -1;
            int amount = 0;
            if (!decimal.TryParse(Request["price"], out price) ||
                !int.TryParse(Request["cate_id"], out cate_id) ||
                !int.TryParse(Request["vendor"], out vendor_id) ||
                !int.TryParse(Request["brand"], out brand_id) ||
                !int.TryParse(Request["amount"], out amount)) {
                return Content("error");
            }

            string unit = Request["unit"];
            string prod_spec = Request["prod_spec"];

            if (ProductManager.Instance.InStockNewProduct(prod_name, cate_id, vendor_id, prod_spec, brand_id, price, amount, unit)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }

        /// <summary>入库一个存在的商品
        /// </summary>
        /// <param name="id">要入库的商品ID</param>
        /// <remarks>返回入库的操作结果</remarks>
        public ActionResult InstockProduct(int id) {
            Product ChangedProduct = pm.GetProductByID(id);
            if (ChangedProduct == null) {
                return Content("error");
            }

            decimal price = 0.0m;
            int amount = 0;
            if (!decimal.TryParse(Request["price"], out price) ||
                !int.TryParse(Request["amount"], out amount)) {
                return Content("error");
            }

            if (ProductManager.Instance.InStockProduct(id, price, amount)) {
                return Content("ok");
            }
            else {
                return Content("error");
            }
        }
    }
}
