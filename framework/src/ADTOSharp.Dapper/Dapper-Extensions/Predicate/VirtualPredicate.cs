using System;
using System.Collections.Generic;
using ADTOSharp.Dapper_Extensions.Extensions;
using ADTOSharp.Dapper_Extensions.Mapper;
using ADTOSharp.Dapper_Extensions.Sql;

namespace ADTOSharp.Dapper_Extensions.Predicate
{
    public interface IVirtualPredicate : IPredicate
    {
        Operator Operator { get; set; }
        bool Not { get; set; }
        string Comparable { get; set; }
        object Value { get; set; }
    }

    public class VirtualPredicate : IVirtualPredicate
    {
        public Operator Operator { get; set; }
        public bool Not { get; set; }
        public string Comparable { get; set; }
        public object Value { get; set; }

        public VirtualPredicate() : base()
        {
        }

        public VirtualPredicate(string comparable, Operator op, object value, bool not = false) : base()
        {
            Comparable = comparable;
            Operator = op;
            Value = value;
            Not = not;
        }

        public virtual string GetSql(ISqlGenerator sqlGenerator, IDictionary<string, object> parameters, bool isDml = false)
        {
            var param = new Parameter
            {
                ColumnName = Comparable,
                Value = Value is Func<object> ? (Value as Func<object>).Invoke() : Value,
                Name = Comparable
            };

            var valueParameter = parameters.SetParameterName(param, sqlGenerator.Configuration.Dialect.ParameterPrefix);
            return $"({Comparable} {Operator.GetString(Not)} {valueParameter})";
        }
    }
}
