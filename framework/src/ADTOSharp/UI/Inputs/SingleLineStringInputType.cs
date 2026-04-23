using System;
using ADTOSharp.Runtime.Validation;

namespace ADTOSharp.UI.Inputs
{
    [Serializable]
    [InputType("SINGLE_LINE_STRING")]
    public class SingleLineStringInputType : InputTypeBase
    {
        public SingleLineStringInputType()
        {

        }

        public SingleLineStringInputType(IValueValidator validator)
            : base(validator)
        {
        }
    }
}