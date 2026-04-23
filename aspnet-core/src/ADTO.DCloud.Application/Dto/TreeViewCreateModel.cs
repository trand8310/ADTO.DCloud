using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Dto
{
    public class TreeViewCreateModel<T>
    {
        public TreeViewCreateModel()
        {

            Children = new List<TreeViewCreateModel<T>>();
        }
        public T Id { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
        public bool IsLeaf { get; set; }
        public T? ParentId { get; set; }
        public Guid? CreateUserId { get; set; }
        public List<TreeViewCreateModel<T>> Children { get; set; }
    }
}
