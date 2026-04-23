using System.Globalization;
using ADTOSharp.Configuration;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Localization;
using ADTOSharp.Localization.Sources;
using ADTOSharp.ObjectMapping;
using Castle.Core.Logging;

namespace ADTOSharp
{
    /// <summary>
    /// 这个类可以用作service的基类。
    /// 它有一些有用的对象属性注入和一些基本的方法
    /// </summary>
    public abstract class ADTOSharpServiceBase
    {
        /// <summary>
        /// 设置管理.
        /// </summary>
        public ISettingManager SettingManager { get; set; }

        /// <summary>
        /// 工作单元管理 <see cref="IUnitOfWorkManager"/>.
        /// </summary>
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get
            {
                if (_unitOfWorkManager == null)
                {
                    throw new ADTOSharpException("Must set UnitOfWorkManager before use it.");
                }

                return _unitOfWorkManager;
            }
            set { _unitOfWorkManager = value; }
        }
        private IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// 当前的工作单元
        /// </summary>
        protected IActiveUnitOfWork CurrentUnitOfWork { get { return UnitOfWorkManager.Current; } }

        /// <summary>
        /// 本地化管理,用于多语言
        /// </summary>
        public ILocalizationManager LocalizationManager { get; set; }

        /// <summary>
        /// 获取/设置此应用程序服务中使用的本地化资源的名称
        /// 它必须设置才能使用 <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// 获取本地化资源。为XML格式的文件
        /// 如果<see cref="LocalizationSourceName"/> 设置后是则有效.
        /// </summary>
        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (LocalizationSourceName == null)
                {
                    throw new ADTOSharpException("Must set LocalizationSourceName before, in order to get LocalizationSource");
                }

                if (_localizationSource == null || _localizationSource.Name != LocalizationSourceName)
                {
                    _localizationSource = LocalizationManager.GetSource(LocalizationSourceName);
                }

                return _localizationSource;
            }
        }
        private ILocalizationSource _localizationSource;

        /// <summary>
        /// 日志记录
        /// </summary>
        public ILogger Logger { protected get; set; }

        /// <summary>
        /// 对象映射,使用AutoMapper 
        /// </summary>
        public IObjectMapper ObjectMapper { get; set; }

        /// <summary>
        /// 构造函数.
        /// </summary>
        protected ADTOSharpServiceBase()
        {
            Logger = NullLogger.Instance;
            ObjectMapper = NullObjectMapper.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        /// <summary>
        /// 获取给定键名和当前语言的本地化字符串。
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationSource.GetString(name);
        }

        /// <summary>
        /// 获取给定键名的本地化字符串和具有格式化字符串的当前语言。
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, params object[] args)
        {
            return LocalizationSource.GetString(name, args);
        }

        /// <summary>
        /// 获取给定键名和指定区域性信息的本地化字符串。
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationSource.GetString(name, culture);
        }

        /// <summary>
        /// 获取给定键名的本地化字符串和具有格式化字符串的当前语言。
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture, params object[] args)
        {
            return LocalizationSource.GetString(name, culture, args);
        }
    }
}
