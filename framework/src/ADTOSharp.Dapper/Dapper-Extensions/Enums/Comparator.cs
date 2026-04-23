using System.ComponentModel;

namespace ADTOSharp.Dapper_Extensions.Enums
{
    public enum Comparator
    {
        [Description("=")]
        Equal,
        [Description("!=")]
        NotEqual,
        [Description("<")]
        LessThan,
        [Description(">")]
        GreaterThan,
        [Description("<=")]
        LessThanOrEqual,
        [Description(">=")]
        GreaterThanOrEqual,
    }
}
