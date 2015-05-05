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
using System.Net;

namespace Solemart.Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "Super,Operator")]
    public class ProductController : Controller
    {
        public ActionResult Index(int? p)
        {
            int pi = p ?? 0; //表示页索引
            int totalCount = 0;

            ProductManagerViewModel model = new ProductManagerViewModel();
            model.ProductList = ProductManager.GetPagedAllProducts(pi, 10, out totalCount);
            model.PageIndex = pi;
            model.TotalPageCount = (totalCount + 9) / 10;

            return View(model);
        }

        /// <summary>
        /// The modify view
        /// </summary>
        /// <param name="id">The product id for modifing</param>
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
            //decode the text before save.
            product.Description = WebUtility.HtmlDecode(product.Description);
            if (ProductManager.ModifyProductInfo(product))
                return Content(WebResult<string>.SuccessResult.ResponseString);

            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }

        /// <summary>
        /// Delete the product image
        /// </summary>
        /// <param name="id">product ID</param>
        /// <param name="imageID">the image id to delete</param>
        /// <returns>删除图片的结果</returns>
        public ActionResult DeleteProductImage(int id, int imageID)
        {
            ProductImageItem img = ProductManager.GetProductImage(id, imageID);
            if (img != null && ProductManager.DeleteProductImage(id, imageID))
            {
                System.IO.File.Delete(Server.MapPath("~/images/product/normal/" + img.ImageUrl));
                System.IO.File.Delete(Server.MapPath("~/images/product/thumb/" + img.ImageUrl));
                return Content(WebResult<string>.SuccessResult.ResponseString);
            }

            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }

        /// <summary>
        /// Add a image for the product
        /// </summary>
        /// <param name="id">the product id</param>
        /// <returns>action result</returns>
        public ActionResult AddNewProductImage(int id)
        {
            ProductItem CurrentProduct = ProductManager.GetProductByID(id);
            if (CurrentProduct == null)
            {
                return Content(WebResult<string>.NormalErrorResult.ResponseString);
            }

            HttpPostedFileBase file = Request.Files[0];
            if (!file.ContentType.Contains("image") || file.ContentLength > 262144)
            {
                return Content(WebResult<string>.FileTooLongResult.ResponseString);
            }

            string mimetype = file.ContentType;
            string file_name = ProductManager.GenerateProductImageFileName(CurrentProduct.ProductID,
                ProductManager.FromMimeTypeGetExtendName(mimetype));

            BitmapDecoder decoder = BitmapDecoder.Create(file.InputStream, BitmapCreateOptions.None, BitmapCacheOption.Default);

            /* 生成小图标 */
            TransformedBitmap thumb_img = new TransformedBitmap(decoder.Frames[0], new System.Windows.Media.ScaleTransform(0.2, 0.2));
            BitmapEncoder encoder = BitmapEncoder.Create(decoder.CodecInfo.ContainerFormat);            
            using (FileStream thumb_file = new FileStream(HttpContext.Server.MapPath("~/images/product/thumb/" + file_name), FileMode.Create))
            {
                encoder.Frames.Add(BitmapFrame.Create(thumb_img));
                encoder.Save(thumb_file);
            }

            file.SaveAs(HttpContext.Server.MapPath("~/Images/product/normal/" + file_name));
            ProductImageItem item = new ProductImageItem();
            item.ProductID = CurrentProduct.ProductID;
            item.ImageUrl = file_name;
            item.MimeType = file.ContentType;
            item.Description = "";
            //the first image is default to logo.
            if (file_name.Contains("_0."))
                item.ForLogo = true;

            if (ProductManager.AddNewImageToProduct(item))
            {
                return Content(WebResult<string>.SuccessResult.ResponseString);
            }
            else
            {
                return Content(WebResult<string>.NormalErrorResult.ResponseString);
            }
        }

        /// <summary>
        /// Get the product lasted in stock price
        /// </summary>
        /// <param name="id">The product id</param>
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
                return Content(WebResult<string>.SuccessResult.ResponseString);

            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }

        /// <summary>
        /// Put the product for saling
        /// </summary>
        /// <param name="id">The product id</param>
        /// <returns>商品上架的结果</returns>
        public ActionResult PutToSaling(SaledProductItem saledProduct)
        {
            if (ProductManager.PutToSaling(saledProduct))
                return Content(WebResult<string>.SuccessResult.ResponseString);

            return Content(WebResult<string>.NormalErrorResult.ResponseString);
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
                    return Content(WebResult<string>.NormalErrorResult.ResponseString);

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
                 Description = model.Description, Specification = model.Specification, ProducingArea=model.ProducingArea, VendorID=model.VendorID, Unit=model.Unit};
            if (ProductManager.InStockProduct(product, model.StockPrice, model.StockAmount, model.Remark))
            {
                return Content(WebResult<string>.SuccessResult.ResponseString);
            }

            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }

        /// <summary>
        /// In stock a existed product
        /// </summary>
        /// <param name="id">product ID want to in stock</param>
        /// <remarks>返回入库的操作结果</remarks>
        public ActionResult InstockProduct(ProductInStockViewModel model)
        {
            ProductItem product = ProductManager.GetProductByID(model.ProductID);
            if (product == null)
                return Content(WebResult<string>.NormalErrorResult.ResponseString);

            if (ProductManager.InStockProduct(product, model.StockPrice, model.StockAmount, model.Remark))
                return Content(WebResult<string>.SuccessResult.ResponseString);

            return Content(WebResult<string>.NormalErrorResult.ResponseString);
        }
    }
}
