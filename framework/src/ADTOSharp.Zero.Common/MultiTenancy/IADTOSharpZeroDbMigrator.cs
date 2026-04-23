namespace ADTOSharp.MultiTenancy
{
    public interface IADTOSharpZeroDbMigrator
    {
        void CreateOrMigrateForHost();

        void CreateOrMigrateForTenant(ADTOSharpTenantBase tenant);
    }
}
