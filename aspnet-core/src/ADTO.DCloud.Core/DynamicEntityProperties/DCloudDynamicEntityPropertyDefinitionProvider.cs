using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Customers;
using ADTO.DCloud.CustomInputTypes;
using ADTOSharp.DynamicEntityProperties;
using ADTOSharp.UI.Inputs;
using System;


namespace ADTO.DCloud.DynamicEntityProperties
{
    /// <summary>
    /// 动态属性是一种允许在运行时添加和管理实体对象的新属性的功能，
    /// 可以定义实体对象的动态属性并轻松对这些对象执行操作。例如，它可以用于城市、县、性别、状态代码等。
    /// </summary>
    public class DCloudDynamicEntityPropertyDefinitionProvider : DynamicEntityPropertyDefinitionProvider
    {
        public override void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context)
        {
            context.Manager.AddAllowedInputType<SingleLineStringInputType>();
            context.Manager.AddAllowedInputType<ComboboxInputType>();
            context.Manager.AddAllowedInputType<CheckboxInputType>();
            context.Manager.AddAllowedInputType<MultiSelectComboboxInputType>();

            //Add entities here 
            context.Manager.AddEntity<User, Guid>();
            context.Manager.AddEntity<Customer, Guid>();
        }
    }
}

