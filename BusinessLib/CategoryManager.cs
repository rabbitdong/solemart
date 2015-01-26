using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xxx.EntityLib;
using Xxx.DataAccessLib;

namespace Xxx.BusinessLib {
    /// <summary>类别管理类
    /// </summary>
    public class CategoryManager {
        private ProductDA prod_da = ProductDA.Instance;

        private static CategoryManager instance = new CategoryManager();

        /// <summary>作为缓存的业务对象，不去数据库进行及时查找
        /// </summary>
        private List<Category> categories = null;

        /// <summary>获取类别管理对象
        /// </summary>
        public static CategoryManager Instance {
            get { return instance; }
        }

        /// <summary>添加一个新类别
        /// </summary>
        /// <param name="cate_name">新类别的名称</param>
        /// <param name="cate_desc">新类别的说明</param>
        /// <param name="sup_cate_id">新类别所属的父类别</param>
        /// <returns>如果添加成功，返回true，否则返回false</returns>
        public bool AddNewCategory(string cate_name, string cate_desc, int sup_cate_id) {
            if (prod_da.GetCategoryList().Find(cate => cate.CateName == cate_name) != null)
                return false;

            int new_cate_id = prod_da.AddNewCategory(cate_name, cate_desc, sup_cate_id);

            if (new_cate_id > 0) {
                Category new_cate = new Category();
                new_cate.CateID = new_cate_id;
                new_cate.CateName = cate_name;
                new_cate.Desc = cate_desc;
                new_cate.SubCategories = null;

                if (sup_cate_id != 0) {
                    Category sup_cate = GetCateInList(categories, sup_cate_id);
                    if (sup_cate.SubCategories == null)
                        sup_cate.SubCategories = new List<Category>();

                    sup_cate.SubCategories.Add(new_cate);
                }
                else
                    categories.Add(new_cate);
                return true;
            }

            return false;
        }

        /// <summary>列表中是否存在cat_id的类别
        /// </summary>
        /// <param name="cates">要查找的列表(平面列表)</param>
        /// <param name="cat_id">要查询的id</param>
        /// <returns>如果找到，返回该类别对象</returns>
        private Category GetCateInList(List<Category> cates, int cate_id) {
            Category finded_cate = null;
            foreach (Category cate in cates) {
                if (cate.CateID == cate_id)
                    return cate;

                if (cate.SubCategories != null) {
                    finded_cate = GetCateInList(cate.SubCategories, cate_id);
                    if (finded_cate != null)
                        return finded_cate;
                }
            }

            return null;
        }

        /// <summary>根据cate_id获取其类别对象
        /// </summary>
        /// <param name="cate_id">要获取的类别的ID</param>
        /// <returns>返回获取的类别，如果没有，返回null</returns>
        public Category GetCateById(int cate_id) {
            return GetCateInList(Categories, cate_id);
        }

        /// <summary>刷新类别列表
        /// </summary>
        private void FreshCategoryList() {
            if (categories == null)
                categories = new List<Category>();

            //刷新的时候先清空下
            categories.Clear();

            //保存从DataAccessLib类的平面的列表
            List<Category> nostack_cate_list = prod_da.GetCategoryList();
            IEnumerable<KeyValuePair<int, int>> cate_prod_count = GetCateCountList();

            foreach (Category cate in nostack_cate_list) {
                int cid = cate.CateID;
                string cate_name = cate.CateName;
                int sid = cate.SupCategory == null ? 0 : cate.SupCategory.CateID;
                cate.ProductCount = cate_prod_count.FirstOrDefault(v => v.Key == cid).Value;

                if (sid == 0) {
                    categories.Add(cate);    //只添加顶级的类别到列表中
                }
                else {
                    Category sup_cate = GetCateInList(nostack_cate_list, sid);
                    if (sup_cate.SubCategories == null) {
                        sup_cate.SubCategories = new List<Category>();
                    }
                    sup_cate.SubCategories.Add(cate);
                    sup_cate.ProductCount += cate.ProductCount;
                }
            }
        }

        /// <summary>获取非层次性的类别列表
        /// </summary>
        public List<Category> Categories {
            get {
                if (categories != null)
                    return categories;

                FreshCategoryList();

                return categories;
            }
        }

        /// <summary>获取第一个显示的子类别（包含产品的类别）
        /// </summary>
        /// <returns></returns>
        public Category GetFirstChildCate() {
            Category cate = Categories[0];
            while (cate.SubCategories != null && cate.SubCategories.Count > 0) {
                cate = cate.SubCategories[0];
            }

            return cate;
        }

        /// <summary>把cate_id的类别作为super_cate_id的子类别
        /// </summary>
        /// <param name="cate_id">要移动的类别ID</param>
        /// <param name="super_cate_id">作为新父类别的ID</param>
        /// <returns>执行成功返回true，否则返回false</returns>
        public bool ChangeCateToOtherSuperCate(int cate_id, int super_cate_id) {
            if (cate_id < 1 || super_cate_id < 1 || cate_id == super_cate_id)
                return false;

            Category cate = GetCateInList(categories, cate_id);
            Category super_cate = GetCateInList(categories, super_cate_id);
            Category src_super_cate = null;

            //原来的类别ID
            int src_super_cate_id = 0;
            if (cate.SupCategory != null) {
                src_super_cate_id = cate.SupCategory.CateID;
                src_super_cate = GetCateInList(categories, src_super_cate_id);
            }

            if (cate == null || super_cate == null || cate == super_cate)
                return false;

            if (!prod_da.ChangeCateToOtherSuperCate(cate_id, super_cate_id))
                return false;

            if (super_cate.SubCategories == null) {
                super_cate.SubCategories = new List<Category>();
            }
            super_cate.SubCategories.Add(cate);

            //如果非顶级类别，就从原来的删除。否则就从顶级列表中移除
            if (src_super_cate != null)
                src_super_cate.SubCategories.Remove(cate);
            else
                categories.Remove(cate);

            return true;
        }

        /// <summary>把项super_cate的子项插入到cate_list中
        /// </summary>
        /// <param name="cate_list">要插入的列表</param>
        /// <param name="super_cate">要插入的子项的父项</param>
        private void InsertSubCateToList(List<Category> cate_list, Category super_cate) {
            if (super_cate.SubCategories == null)
                return;

            foreach (Category cate in super_cate.SubCategories) {
                cate_list.Add(cate);
                InsertSubCateToList(cate_list, cate);
            }
        }

        /// <summary>获取所有项，是平面一级结构
        /// </summary>
        public List<Category> AllCategories {
            get {
                List<Category> all_cates = new List<Category>();
                foreach (Category cate in Categories) {
                    all_cates.Add(cate);
                    InsertSubCateToList(all_cates, cate);
                }

                return all_cates;
            }
        }

        /// <summary>获取每类别下产品的数量
        /// </summary>
        internal IEnumerable<KeyValuePair<int, int>> GetCateCountList() {
            return prod_da.GetProductCountEachCategory();
        }
    }
}
