using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using Solemart.DataProvider;
using Solemart.DataProvider.Entity;
using Solemart.SystemUtil;

namespace Solemart.BusinessLib {
    /// <summary>类别管理类
    /// </summary>
    public class CategoryManager {

        private static CategoryManager instance = new CategoryManager();

        /// <summary>
        /// 作为缓存的业务对象，不去数据库进行及时查找
        /// </summary>
        private IList<CategoryItem> categoryList = null;

        /// <summary>
        /// 获取类别管理对象
        /// </summary>
        public static CategoryManager Instance {
            get { return instance; }
        }

        /// <summary>
        /// Add new category
        /// </summary>
        /// <param name="newCategory">The category want to add</param>
        /// <returns>Return true if success, or return false</returns>
        public Result<string> AddNewCategory(CategoryItem newCategory) {
            using (SolemartDBContext context = new SolemartDBContext())
            {
                if (context.CategoryItems.First(c => c.CategoryName == newCategory.CategoryName) != null)
                    return Result<string>.DuplicatedField;

                context.CategoryItems.Add(newCategory);
                if (context.SaveChanges() <= 0)
                    return Result<string>.NormalErrorResult;

                if (newCategory.ParentCategoryID != 0)
                {
                    CategoryItem parentCategory = GetCategoryInList(categoryList, newCategory.ParentCategoryID);
                    if (parentCategory.SubCategories == null)
                        parentCategory.SubCategories = new List<CategoryItem>();

                    parentCategory.SubCategories.Add(newCategory);
                }
                else
                {
                    categoryList.Add(newCategory);
                }

                return Result<string>.SuccessResult;
            }
        }

        /// <summary>
        /// Get the category in the list.
        /// </summary>
        /// <param name="categoryList">The category list to search</param>
        /// <param name="categoryID">The category id to search</param>
        /// <returns>Return the category if get one, or return null</returns>
        private CategoryItem GetCategoryInList(IList<CategoryItem> categoryList, int categoryID) {
            CategoryItem findedCategory = null;
            foreach (CategoryItem category in categoryList) {
                if (category.CategoryID == categoryID)
                    return category;

                if (category.SubCategories != null) {
                    findedCategory = GetCategoryInList(category.SubCategories, categoryID);
                    if (findedCategory != null)
                        return findedCategory;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the category item by the category id.
        /// </summary>
        /// <param name="categoryID">要获取的类别的ID</param>
        /// <returns>返回获取的类别，如果没有，返回null</returns>
        public CategoryItem GetCategoryById(int categoryID) {
            return GetCategoryInList(categoryList, categoryID);
        }

        /// <summary>
        /// Load the category list.
        /// </summary>
        private void LoadCategoryList() {
            if (categoryList == null)
                categoryList = new List<CategoryItem>();

            //Clear the list first.
            categoryList.Clear();

            //Save the list get from the DataProvider
            using(SolemartDBContext context=new SolemartDBContext())
            {
                List<CategoryItem> nostackCategoryList = context.CategoryItems.ToList();

                foreach (CategoryItem category in nostackCategoryList)
                {
                    string cate_name = category.CategoryName;

                    if (category.ParentCategoryID == 0)
                    {
                        categoryList.Add(category);    //只添加顶级的类别到列表中
                    }
                    else
                    {
                        CategoryItem parentCategory = GetCategoryInList(nostackCategoryList, category.ParentCategoryID);
                        if (parentCategory.SubCategories == null)
                        {
                            parentCategory.SubCategories = new List<CategoryItem>();
                        }
                        parentCategory.SubCategories.Add(category);
                    }
                }
            }
        }

        /// <summary>
        /// Get the category list(hierarchical structure).
        /// </summary>
        public IList<CategoryItem> Categories {
            get {
                if (categoryList != null)
                    return categoryList;

                LoadCategoryList();

                return categoryList;
            }
        }

        /// <summary>
        /// Get the first category of the child category.
        /// </summary>
        /// <returns></returns>
        public CategoryItem GetFirstChildCate() {
            CategoryItem category = Categories[0];
            while (category.SubCategories != null && category.SubCategories.Count > 0) {
                category = category.SubCategories[0];
            }

            return category;
        }

        /// <summary>
        /// Change the category parent to another category.
        /// </summary>
        /// <param name="categoryID">要移动的类别ID</param>
        /// <param name="parentCategoryID">作为新父类别的ID</param>
        /// <returns>执行成功返回true，否则返回false</returns>
        public bool ChangeCateToOtherSuperCate(int categoryID, int parentCategoryID)
        {
            if (categoryID < 1 || parentCategoryID < 1 || categoryID == parentCategoryID)
                return false;

            CategoryItem cate = GetCategoryInList(categoryList, categoryID);
            CategoryItem super_cate = GetCategoryInList(categoryList, parentCategoryID);
            CategoryItem src_super_cate = null;

            //原来的类别ID
            int src_super_cate_id = 0;
            if (cate.ParentCategoryID != 0) {
                src_super_cate = GetCategoryInList(categoryList, src_super_cate_id);
            }

            if (cate == null || super_cate == null || cate == super_cate)
                return false;


            if (super_cate.SubCategories == null) {
                super_cate.SubCategories = new List<CategoryItem>();
            }
            super_cate.SubCategories.Add(cate);

            //如果非顶级类别，就从原来的删除。否则就从顶级列表中移除
            if (src_super_cate != null)
                src_super_cate.SubCategories.Remove(cate);
            else
                categoryList.Remove(cate);

            return true;
        }

        /// <summary>把项super_cate的子项插入到cate_list中
        /// </summary>
        /// <param name="cate_list">要插入的列表</param>
        /// <param name="super_cate">要插入的子项的父项</param>
        private void InsertSubCateToList(List<CategoryItem> cate_list, CategoryItem super_cate)
        {
            if (super_cate.SubCategories == null)
                return;

            foreach (CategoryItem cate in super_cate.SubCategories)
            {
                cate_list.Add(cate);
                InsertSubCateToList(cate_list, cate);
            }
        }

        /// <summary>获取所有项，是平面一级结构
        /// </summary>
        public List<CategoryItem> AllCategories
        {
            get {
                List<CategoryItem> all_cates = new List<CategoryItem>();
                foreach (CategoryItem cate in Categories)
                {
                    all_cates.Add(cate);
                    InsertSubCateToList(all_cates, cate);
                }

                return all_cates;
            }
        }
    }
}
