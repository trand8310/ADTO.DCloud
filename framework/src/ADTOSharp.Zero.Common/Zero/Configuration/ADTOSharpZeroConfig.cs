namespace ADTOSharp.Zero.Configuration
{
    internal class ADTOSharpZeroConfig : IADTOSharpZeroConfig
    {
        public IRoleManagementConfig RoleManagement
        {
            get { return _roleManagementConfig; }
        }
        private readonly IRoleManagementConfig _roleManagementConfig;

        public IUserManagementConfig UserManagement
        {
            get { return _userManagementConfig; }
        }
        private readonly IUserManagementConfig _userManagementConfig;

        public ILanguageManagementConfig LanguageManagement
        {
            get { return _languageManagement; }
        }
        private readonly ILanguageManagementConfig _languageManagement;

        public IADTOSharpZeroEntityTypes EntityTypes
        {
            get { return _entityTypes; }
        }
        private readonly IADTOSharpZeroEntityTypes _entityTypes;


        public ADTOSharpZeroConfig(
            IRoleManagementConfig roleManagementConfig,
            IUserManagementConfig userManagementConfig,
            ILanguageManagementConfig languageManagement,
            IADTOSharpZeroEntityTypes entityTypes)
        {
            _entityTypes = entityTypes;
            _roleManagementConfig = roleManagementConfig;
            _userManagementConfig = userManagementConfig;
            _languageManagement = languageManagement;
        }
    }
}