using System;

namespace Attributes
{
    // Should be applied to properties and fields.
    public class IntValidatorAttribute : Attribute
    {
        public int LowerBound { get; set; }
        public int UpperBound { get; set; }

        public IntValidatorAttribute(int lowerBound, int upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }
    }
}
