﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solemart.WeixinAPI.Entities;
using Solemart.WeixinAPI.Base.Entities;

namespace Solemart.WeixinAPI.AdvancedAPIs
{
    /// <summary>
    /// 增加分组返回信息
    /// </summary>
    public class AddGroupResult : WxJsonResult
    {
        public int group_id { get; set; }//分组ID
    }

    /// <summary>
    /// 获取所有分组返回信息
    /// </summary>
    public class GetAllGroup : WxJsonResult
    {
        public List<GroupsDetail> groups_detail { get; set; }//分组集合
    }

    public class GroupsDetail : WxJsonResult
    {
        public int group_id { get; set; }//分组ID
        public string group_name { get; set; }//分组名称
    }

    public class GetByIdGroup : WxJsonResult
    {
        public Group_Detail group_detail { get; set; }//分组信息
    }

    public class Group_Detail
    {
        public int group_id { get; set; }//分组ID
        public string group_name { get; set; }//分组名称
        public string[] product_list { get; set; }//商品ID集合
    }
}


