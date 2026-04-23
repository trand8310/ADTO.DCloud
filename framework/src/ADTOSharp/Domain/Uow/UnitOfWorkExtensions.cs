using System;
using System.Collections.Generic;
using System.Linq;
using ADTOSharp;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using JetBrains.Annotations;

namespace Volo.Abp.Uow;

public static class UnitOfWorkExtensions
{
    public static bool IsReservedFor([NotNull] this IActiveUnitOfWork unitOfWork, string reservationName)
    {
        Check.NotNull(unitOfWork, nameof(unitOfWork));

        //return unitOfWork.IsReserved && unitOfWork.ReservationName == reservationName;
        return false;
    }

    public static void AddItem<TValue>([NotNull] this IActiveUnitOfWork unitOfWork, string key, TValue value)
        where TValue : class
    {
        Check.NotNull(unitOfWork, nameof(unitOfWork));

        unitOfWork.Items[key] = value;
    }

    public static TValue GetItemOrDefault<TValue>([NotNull] this IActiveUnitOfWork unitOfWork, string key)
        where TValue : class
    {
        Check.NotNull(unitOfWork, nameof(unitOfWork));

        return unitOfWork.Items.FirstOrDefault(x => x.Key == key).Value.As<TValue>();
    }

    public static TValue GetOrAddItem<TValue>([NotNull] this IActiveUnitOfWork unitOfWork, string key, Func<string, TValue> factory)
        where TValue : class
    {
        Check.NotNull(unitOfWork, nameof(unitOfWork));

        return unitOfWork.Items.GetOrAdd(key, factory).As<TValue>();
    }

    public static void RemoveItem([NotNull] this IActiveUnitOfWork unitOfWork, string key)
    {
        Check.NotNull(unitOfWork, nameof(unitOfWork));

        unitOfWork.Items.RemoveAll(x => x.Key == key);
    }
}
