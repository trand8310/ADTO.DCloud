using System.Collections.Generic;

namespace ADTO.DCloud.Dto;

/// <summary>
/// 树状视图,泛型结构
/// </summary>
/// <typeparam name="T"></typeparam>
public class TreeViewModel<T>
{
    public TreeViewModel()
    {

        Children = new List<TreeViewModel<T>>();
    }
    public T Id { get; set; }
    public string Label { get; set; }
    public bool Disabled { get; set; }
    public bool IsLeaf { get; set; }
    public T? ParentId { get; set; }
    public List<TreeViewModel<T>> Children { get; set; }
}

///// <summary>
///// 树状视图
///// </summary>
//public class TreeViewModel
//{
//    public TreeViewModel()
//    {

//        Children = new List<TreeViewModel>();
//    }
//    public int Id { get; set; }
//    public string Label { get; set; }
//    public bool Disabled { get; set; }
//    public bool IsLeaf { get; set; }
//    public int? ParentId { get; set; }
//    public List<TreeViewModel> Children { get; set; }
//}
