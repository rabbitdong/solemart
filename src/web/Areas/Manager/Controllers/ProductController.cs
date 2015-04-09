using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Windows.Media.Imaging;
using Solemart.DataProvider.Entity;
using Solemart.BusinessLib;
using Solemart.Web.Areas.Manager.Models;
using Solemart.WebUtil;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "Super,Operator")]
    public class ProductController : Controller
    {
        /// <summary>产品管理的实例
        /// </summary>
        //
        // GET: /Manager/ProductManager/

        public ActionResult Index(int? p)
        {
            int pi = p ?? 0; //表示页索引
            int totalPageCount = 0;

            ProductManagerViewModel model = new ProductManagerViewModel();
            model.ProductList = ProductManager.GetPagedAllProducts(pi, 10, out totalPageCount);
            model.PageIndex = pi;
            model.TotalPageCount = totalPageCount;

            return View(model);
        }

        /// <summary>用户请求修改产品信息的处理
        /// </summary>
        /// <param name="id">要修改的产品的ID</param>
        /// <returns>修改产品的视图</returns>
        public ActionResult Modify(int id)
        {
            ProductItem CurrentProduct = ProductManager.GetProductByID(id);

            return View(CurrentProduct);
        }

        /// <summary>用户提交修改产品信息的处理
        /// </summary>
        /// <param name="id">要修改的产品的ID</param>
        /// <returns>返回修改的结果View</returns>
        public ActionResult CommitModify(ProductItem product)
        {
            if (ProductManager.ModifyProductInfo(product))
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }

        /// <summary>删除产品的请求处理
        /// </summary>
        /// <param name="id">要处理的产品的ID</param>
        /// <returns>删除图片的结果</returns>
        public ActionResult DeleteProductImage(int id, int imageID)
        {
            ProductImageItem img = ProductManager.GetProductImage(id, imageID);
            if (img != null && ProductManager.DeleteProductImage(id, imageID))
            {
                System.IO.File.Delete(Server.MapPath("~/images/product/normal/" + img.ImageUrl));
                System.IO.File.Delete(Server.MapPath("~/images/product/thumb/" + img.ImageUrl));
                return Content("ok");
            }
            else
                return Content("error");
        }

        /// <summary>添加一个产品的图片处理
        /// </summary>
        /// <param name="id">要添加的产品的ID</param>
        /// <returns>添加的产品的结果View</returns>
        public ActionResult AddNewProductImage(int id)
        {
            ProductItem CurrentProduct = ProductManager.GetProductByID(id);
            if (CurrentProduct == null)
            {
                return Content("error");
            }

            HttpPostedFileBase file = Request.Files[0];
            if (!file.ContentType.Contains("image") || file.ContentLength > 65535)
            {
                return Content("error");
            }

            string mimetype = file.ContentType;
            string file_name = ProductManager.GenerateProductImageFileName(CurrentProduct.ProductID,
                ProductManager.FromMimeTypeGetExtendName(mimetype));

            BitmapDecoder decoder = BitmapDecoder.Create(file.InputStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            if (decoder.Frames[0].PixelWidth != 350 || decoder.Frames[0].PixelHeight != 350)
            {
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
            ProductImageItem item = new ProductImageItem();
            item.ProductID = CurrentProduct.ProductID;
            item.ImageUrl = file_name;
            item.MimeType = file.ContentType;
            item.Description = "";

            if (ProductManager.AddNewImageToProduct(item))
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }

        /// <summary>获取商品的最后入库价格
        /// </summary>
        /// <param name="id">要获取的商品ID</param>
        /// <returns>返回最后价格结果</returns>
        public ActionResult GetLastStockPrice(int id)
        {
            decimal price = 0.0m;
            price = ProductManager.GetLastStockPrice(id);
            price = price + price / 2;

            return Content(price.ToString("C2"));
        }

        /// <summary>商品下架的请求的处理
        /// </summary>
        /// <param name="id">要下架的商品的ID</param>
        /// <returns>商品下架的结果</returns>
        public ActionResult GetBackSaling(int id)
        {
            if (ProductManager.TakeOffSaling(id))
            {
                return Content("ok");
            }

            return Content("error");
        }

        /// <summary>商品上架的请求的处理
        /// </summary>
        /// <param name="id">要上架的商品的ID</param>
        /// <returns>商品上架的结果</returns>
        public ActionResult PutToSaling(int id)
        {
            decimal price = 0.0m;
            int discount = 100;

            if (!decimal.TryParse(Request["price"], out price)
                || !int.TryParse(Request["discount"], out discount))
            {
                return Content("error");
            }


            bool is_spec_flag = Request["isspec"] == "true";
            SaledProductItem item = new SaledProductItem();
            item.ProductID = id;
            item.Price = price;
            item.Discount = discount;
            item.SpecialFlag = is_spec_flag;
            if (ProductManager.PutToSaling(item))
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }

        /// <summary>商品入库的处理
        /// </summary>
        /// <param name="id">要入库的商品ID</param>
        /// <returns>入库的结果</returns>
        public ActionResult InStock(int? id)
        {
            if (id == null)
            {
                return View();
            }
            else
            {
                ProductItem ChangedProduct = ProductManager.GetProductByID(id.Value);
                if (ChangedProduct == null)
                    return Content("error");
                return View(ChangedProduct);
            }
        }

        /// <summary>
        /// In stock a new product
        /// </summary>
        /// <returns>返回入库的结果</returns>
        public ActionResult InstockNewProduct(ProductInStockViewModel model)
        {
            ProductItem product = new ProductItem { ProductName=model.ProductName, CategoryID = model.CategoryID, BrandID = model.BrandID, 
                 Description = model.Description, Specification = model.Specification, VendorID=model.VendorID, Unit=model.Unit};
            if (ProductManager.InStockProduct(product, model.StockPrice, model.StockAmount, model.Remark))
            {
                return Content(WebResult<string>.SuccessResult.ResponseString);
            }

            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }

        /// <summary>入库一个存在的商品
        /// </summary>
        /// <param name="id">要入库的商品ID</param>
        /// <remarks>返回入库的操作结果</remarks>
        public ActionResult InstockProduct(int id, string remark)
        {
            ProductItem product = ProductManager.GetProductByID(id);
            if (product == null)
            {
                return Content("error");
            }

            decimal price = 0.0m;
            int amount = 0;
            if (!decimal.TryParse(Request["price"], out price) ||
                !int.TryParse(Request["amount"], out amount))
            {
                return Content("error");
            }

            if (ProductManager.InStockProduct(product, price, amount, remark))
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }
    }
}
