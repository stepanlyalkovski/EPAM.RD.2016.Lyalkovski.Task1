using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Attributes;

namespace Reflection.User
{
    public class UserValidation
    {
        public static bool ValidateUser(Attributes.User user)
        {
            var type = user.GetType();
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
            var fields = type.GetFields(flags);

            bool isValidFileds = ValidateFileds(fields, user);
            if (!isValidFileds)
                return false;

            var properties = type.GetProperties();
            bool isValidProperties = ValidateProperties(properties, user);
            if (!isValidProperties)
                return false;

            return true;           
        }

        private static bool ValidateFileds<T>(IEnumerable<FieldInfo> fields, T obj)
        {
            foreach (var fieldInfo in fields)
            {
                var fieldAttributes = fieldInfo.GetCustomAttributes();
                foreach (var fieldAttribute in fieldAttributes)
                {
                    var intValidatorAttribute = fieldAttribute as IntValidatorAttribute;
                    if (intValidatorAttribute != null)
                    {
                        bool isValidNumber = ValidateNumber((int)fieldInfo.GetValue(obj), intValidatorAttribute);

                        if (!isValidNumber)
                        {
                            return false;
                        }
                    }

                    var stringValidatorAttribute = fieldAttribute as StringValidatorAttribute;
                    if (stringValidatorAttribute != null)
                    {
                        bool isValidString = ValidateString((string)fieldInfo.GetValue(obj), stringValidatorAttribute);

                        if (!isValidString)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool ValidateProperties<T>(IEnumerable<PropertyInfo> properties, T obj)
        {
            foreach (var propertyInfo in properties)
            {
                var stringValidator = (StringValidatorAttribute)propertyInfo.GetCustomAttribute(typeof(StringValidatorAttribute));
                if (stringValidator != null)
                {
                    bool isValidString = ValidateString((string)propertyInfo.GetValue(obj), stringValidator);

                    if (!isValidString)
                    {
                        return false;
                    }
                }

                var intValidatorAttribute = (IntValidatorAttribute)propertyInfo.GetCustomAttribute(
                                                                                    typeof(IntValidatorAttribute));
                if (intValidatorAttribute != null)
                {
                    bool isValidNumber = ValidateNumber((int)propertyInfo.GetValue(obj), intValidatorAttribute);

                    if (!isValidNumber)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool ValidateNumber(int number, IntValidatorAttribute validatorAttribute)
        {
            if (validatorAttribute == null) throw new ArgumentNullException(nameof(validatorAttribute));

            return number >= validatorAttribute.LowerBound && number <= validatorAttribute.UpperBound;
        }

        private static bool ValidateString(string str, StringValidatorAttribute validatorAttribute)
        {
            if (validatorAttribute == null) throw new ArgumentNullException(nameof(validatorAttribute));

            return str.Length <= validatorAttribute.CharNumber;
        }
    }
}
