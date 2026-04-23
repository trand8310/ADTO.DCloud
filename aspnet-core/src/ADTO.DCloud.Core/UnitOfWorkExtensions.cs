

using ADTOSharp.Domain.Uow;

namespace ADTO.DCloud;

public static class UnitOfWorkExtensions
{
    public static void SetReadOnly(this IActiveUnitOfWork uow, bool isReadOnly)
    {
        if (uow is IUnitOfWork unitOfWorkImpl)
        {
            unitOfWorkImpl.Items["IsReadOnly"] = isReadOnly;
        }
    }

    public static bool IsReadOnly(this IActiveUnitOfWork uow)
    {
        if (uow is IUnitOfWork unitOfWorkImpl &&
            unitOfWorkImpl.Items.TryGetValue("IsReadOnly", out var val) &&
            val is bool b)
        {
            return b;
        }

        return false;
    }


    //private const string ReadOnlyKey = "IsReadOnly";

    //public static void SetReadOnly(this IUnitOfWork uow, bool value)
    //{
    //    uow.Items[ReadOnlyKey] = value;
    //}

    //public static bool IsReadOnly(this IUnitOfWork uow)
    //{
    //    if (uow.Items.TryGetValue(ReadOnlyKey, out var val) && val is bool flag)
    //    {
    //        return flag;
    //    }
    //    return false;
    //}
}