using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Dto
{
    public static class TreeDataMakeDto
    {
            /// <summary>
            /// 树形数据转化
            /// </summary>
            /// <param name="list">数据</param>
            /// <returns></returns>
            public static List<TreeModelDto> ToTree(this List<TreeModelDto> list, string parentId = "")
            {
                Dictionary<string, List<TreeModelDto>> childrenMap = new Dictionary<string, List<TreeModelDto>>();
                Dictionary<string, TreeModelDto> parentMap = new Dictionary<string, TreeModelDto>();
                List<TreeModelDto> res = new List<TreeModelDto>();

                //首先按照
                foreach (var node in list)
                {
                    node.hasChildren = false;
                    node.complete = true;
                    // 注册子节点映射表
                    if (!childrenMap.ContainsKey(node.parentId))
                    {
                        childrenMap.Add(node.parentId, new List<TreeModelDto>());
                    }
                    else if (parentMap.ContainsKey(node.parentId))
                    {
                        parentMap[node.parentId].hasChildren = true;
                    }
                    childrenMap[node.parentId].Add(node);
                    // 注册父节点映射节点表
                    parentMap.Add(node.id, node);

                    // 查找自己的子节点
                    if (!childrenMap.ContainsKey(node.id))
                    {
                        childrenMap.Add(node.id, new List<TreeModelDto>());
                    }
                    else
                    {
                        node.hasChildren = true;
                    }
                    node.ChildNodes = childrenMap[node.id];
                }

                if (string.IsNullOrEmpty(parentId))
                {
                    // 获取祖先数据列表
                    foreach (var item in childrenMap)
                    {
                        if (!parentMap.ContainsKey(item.Key))
                        {
                            res.AddRange(item.Value);
                        }
                    }
                }
                else
                {
                    if (childrenMap.ContainsKey(parentId))
                    {
                        return childrenMap[parentId];
                    }
                    else
                    {
                        return new List<TreeModelDto>();
                    }
                }
                return res;
            }
        /// <summary>
        /// 树形数据转化
        /// </summary>
        /// <param name="list">数据</param>
        /// <returns></returns>
        public static List<TreeModelExDto<T>> ToTree<T>(this List<TreeModelExDto<T>> list) where T : class
        {
            Dictionary<string, List<TreeModelExDto<T>>> childrenMap = new Dictionary<string, List<TreeModelExDto<T>>>();
            Dictionary<string, TreeModelExDto<T>> parentMap = new Dictionary<string, TreeModelExDto<T>>();
            List<TreeModelExDto<T>> res = new List<TreeModelExDto<T>>();

            //首先按照
            foreach (var node in list)
            {
                // 注册子节点映射表
                if (!childrenMap.ContainsKey(node.parentId))
                {
                    childrenMap.Add(node.parentId, new List<TreeModelExDto<T>>());
                }
                childrenMap[node.parentId].Add(node);
                // 注册父节点映射节点表
                parentMap.Add(node.id, node);

                // 查找自己的子节点
                if (!childrenMap.ContainsKey(node.id))
                {
                    childrenMap.Add(node.id, new List<TreeModelExDto<T>>());
                }
                node.ChildNodes = childrenMap[node.id];
            }
            // 获取祖先数据列表
            foreach (var item in childrenMap)
            {
                if (!parentMap.ContainsKey(item.Key))
                {
                    res.AddRange(item.Value);
                }
            }
            return res;
        }
    }
}
