using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Solemart.DataProvider.Entity
{
    /// <summary>
    /// 套餐项
    /// </summary>    
#if TEST 
    [Table("TestPackageItems")]
#endif
    public class PackageItem
    {
        [Key]
        public int PackageID { get; set; }

        public decimal Price { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Remark { get; set; }

        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Package item detail.
    /// </summary>
#if TEST 
    [Table("TestPackageItemDetails")]
#endif
    public class PackageItemDetail
    {
        [Key, Column(Order = 0)]
        public int PackageID { get; set; }

        [ForeignKey("PackageID")]
        public PackageItem PackageItem { get; set; }

        [Key, Column(Order = 1)]
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]

        public ProductItem ProductItem { get; set; }
        public decimal Amount { get; set; }
    }
}
