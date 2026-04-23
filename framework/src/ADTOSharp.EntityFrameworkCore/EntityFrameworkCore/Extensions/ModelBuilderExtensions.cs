using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ADTOSharp.EntityFrameworkCore.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ConfigureSoftDeleteDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, ADTOSharpEfCoreCurrentDbContext efCoreCurrentDbContext)
    {
        modelBuilder.HasDbFunction(methodInfo)
            .HasTranslation(args =>
            {
                // (bool isDeleted, bool boolParam)
                var isDeleted = args[0];
                var boolParam = args[1];

                if (efCoreCurrentDbContext.Context?.IsSoftDeleteFilterEnabled == true)
                {
                    // IsDeleted == false
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        isDeleted,
                        new SqlConstantExpression(false, boolParam.TypeMapping),
                        boolParam.Type,
                        boolParam.TypeMapping);
                }

                // empty where sql
                return new SqlConstantExpression(true, boolParam.Type, boolParam.TypeMapping);
            });

        return modelBuilder;
    }

    public static ModelBuilder ConfigureMayHaveTenantDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, ADTOSharpEfCoreCurrentDbContext efCoreCurrentDbContext)
    {
        modelBuilder.HasDbFunction(methodInfo)
            .HasTranslation(args =>
            {
                // (int? tenantId, int? currentTenantId, bool boolParam)
                var tenantId = args[0];
                var currentTenantId = args[1];
                var boolParam = args[2];

                if (efCoreCurrentDbContext.Context?.IsMayHaveTenantFilterEnabled == true)
                {
                    // TenantId == CurrentTenantId
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        tenantId,
                        currentTenantId,
                        boolParam.Type,
                        boolParam.TypeMapping);
                }

                // empty where sql
                return new SqlConstantExpression(true, boolParam.Type, boolParam.TypeMapping);
            });

        return modelBuilder;
    }

    public static ModelBuilder ConfigureMustHaveTenantDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, ADTOSharpEfCoreCurrentDbContext efCoreCurrentDbContext)
    {
        modelBuilder.HasDbFunction(methodInfo)
            .HasTranslation(args =>
            {
                // (int tenantId, int? currentTenantId, bool boolParam)
                var tenantId = args[0];
                var currentTenantId = args[1];
                var boolParam = args[2];

                if (efCoreCurrentDbContext.Context?.IsMustHaveTenantFilterEnabled == true)
                {
                    // TenantId == CurrentTenantId
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        tenantId,
                        currentTenantId,
                        boolParam.Type,
                        boolParam.TypeMapping);
                }

                // empty where sql
                return new SqlConstantExpression(true, boolParam.Type, boolParam.TypeMapping);
            });

        return modelBuilder;
    }
}
