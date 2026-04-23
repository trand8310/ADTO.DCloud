using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTOSharp.Localization;
public interface IHasNameWithLocalizableDisplayName
{
    [NotNull]
    public string Name { get; }

    public ILocalizableString? DisplayName { get; }
}
