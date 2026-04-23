using ADTO.DCloud.Authorization.Delegation;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Chat;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.DashboardCustomization.Definitions;
using ADTO.DCloud.Debugging;
using ADTO.DCloud.DynamicEntityProperties;
using ADTO.DCloud.Features;
using ADTO.DCloud.Friendships;
using ADTO.DCloud.Friendships.Cache;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Localization;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Net.Emailing;
using ADTO.DCloud.Notifications;
using ADTO.DCloud.Timing;
using ADTO.DCloud.WebHooks;
using ADTO.OpenIddict;
using ADTOSharp;
using ADTOSharp.AutoMapper;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Localization;
using ADTOSharp.MailKit;
using ADTOSharp.Modules;
using ADTOSharp.Net.Mail;
using ADTOSharp.Net.Mail.Smtp;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Timing;
using ADTOSharp.Zero;
using ADTOSharp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using MailKit.Security;
using System;


namespace ADTO.DCloud
{

    [DependsOn(
        typeof(ADTOSharpZeroCoreModule),
        typeof(ADTOSharpAutoMapperModule),
        typeof(ADTOSharpMailKitModule),
        typeof(ADTOOpenIddictModule)
        )]
    public class DCloudCoreModule : ADTOSharpModule
    {
        public override void PreInitialize()
        {
            AppContext.SetSwitch("Microsoft.EntityFrameworkCore.Issue9825", true);
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;
            //Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

   


            Configuration.ReplaceService(typeof(IGuidGenerator), () =>
            {
                IocManager.IocContainer.Register(
                    Component
                        .For<IGuidGenerator, SequentialGuidGenerator>()
                        .Instance(SequentialGuidGenerator.Instance)
                );
            });


            //if (!IocManager.IsRegistered<IGuidGenerator>())
            //{
            //    IocManager.IocContainer.Register(
            //        Component
            //            .For<IGuidGenerator, SequentialGuidGenerator>()
            //            .Instance(SequentialGuidGenerator.Instance)
            //    );
            //}



            DCloudLocalizationConfigurer.Configure(Configuration.Localization);

            //应用功能管理
            Configuration.Features.Providers.Add<AppFeatureProvider>();

            //应用设置管理
            Configuration.Settings.Providers.Add<AppSettingProvider>();

            //消息管理
            Configuration.Notifications.Providers.Add<AppNotificationProvider>();

            //事件注入管理
            Configuration.Webhooks.Providers.Add<AppWebhookDefinitionProvider>();
            Configuration.Webhooks.TimeoutDuration = TimeSpan.FromMinutes(1);
            Configuration.Webhooks.IsAutomaticSubscriptionDeactivationEnabled = false;

            //应用程序多租户状态
            Configuration.MultiTenancy.IsEnabled = DCloudConsts.MultiTenancyEnabled;

            //Enable LDAP authentication
            //Configuration.Modules.ZeroLdap().Enable(typeof(AppLdapAuthenticationSource));

            //Twilio - Enable this line to activate Twilio SMS integration
            //Configuration.ReplaceService<ISmsSender,TwilioSmsSender>();

            //Adding DynamicEntityParameters definition providers
            Configuration.DynamicEntityProperties.Providers.Add<DCloudDynamicEntityPropertyDefinitionProvider>();

            // 邮件配置
            Configuration.Modules.ADTOSharpMailKit().SecureSocketOption = SecureSocketOptions.Auto;
            Configuration.ReplaceService<IMailKitSmtpBuilder, DCloudMailKitSmtpBuilder>(DependencyLifeStyle.Transient);

            //文件处理
            Configuration.ReplaceService<IDCloudFileProvider, DCloudFileProvider>(DependencyLifeStyle.Transient);

            //角色配置
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            if (DebugHelper.IsDebug)
            {
                //在调试模式下,禁用邮件发送功能
                //Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);
            }

            Configuration.ReplaceService(typeof(IEmailSenderConfiguration), () =>
            {
                Configuration.IocManager.IocContainer.Register(
                    Component.For<IEmailSenderConfiguration, ISmtpEmailSenderConfiguration>()
                             .ImplementedBy<DCloudSmtpEmailSenderConfiguration>()
                             .LifestyleTransient()
                );
            });

            Configuration.Caching.Configure(FriendCacheItem.CacheName, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(30);
            });

            //应用面板配置.
            IocManager.Register<DashboardConfiguration>();
            Configuration.Notifications.Notifiers.Add<SmsRealTimeNotifier>();
            Configuration.Notifications.Notifiers.Add<EmailRealTimeNotifier>();

            Configuration.Localization.Languages.Add(new LanguageInfo("zh-Hans", "简体中文", "famfamfam-flags cn"));
            // Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = AppConsts.DefaultPassPhrase;
            // SimpleStringCipher.DefaultPassPhrase = AppConsts.DefaultPassPhrase;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DCloudCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IChatCommunicator, NullChatCommunicator>();
            IocManager.Register<IUserDelegationConfiguration, UserDelegationConfiguration>();
            IocManager.Resolve<ChatUserStateWatcher>().Initialize();
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}

