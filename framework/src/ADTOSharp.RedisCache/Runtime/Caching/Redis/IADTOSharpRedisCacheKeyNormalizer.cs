using System;
using System.Collections.Generic;
using System.Text;

namespace ADTOSharp.Runtime.Caching.Redis
{
    public interface IADTOSharpRedisCacheKeyNormalizer
    {
        string NormalizeKey(ADTOSharpRedisCacheKeyNormalizeArgs args);
    }
}
