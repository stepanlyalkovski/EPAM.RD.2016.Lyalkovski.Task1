using System;

namespace Attributes
{
    // Should be applied to properties and fields.
    public class StringValidatorAttribute : Attribute
    {
        public int CharNumber { get; set; }
        public StringValidatorAttribute(int charNumber)
        {
            CharNumber = charNumber;
        }
    }
}
