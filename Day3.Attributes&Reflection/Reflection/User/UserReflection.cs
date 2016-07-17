using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Attributes;

namespace Reflection.User
{
    public class UserReflection
    {
        public static IEnumerable<Attributes.User> CreateUserInstances()
        {
            var type = typeof (Attributes.User);
            var users = new List<Attributes.User>();           
            int defaultValue = 0;
            var userAttributes = Attribute.GetCustomAttributes(type, typeof (InstantiateUserAttribute));
            foreach (var userAttribute in userAttributes)
            {
                var attribute = userAttribute as InstantiateUserAttribute;

                if (attribute != null)
                {
                    Attributes.User userInstance;
                    if (attribute.Id == null)
                    {
                        var ctor = type.GetConstructor(new[] {typeof (int)});

                        var ctorAttr = (MatchParameterWithPropertyAttribute)
                            ctor.GetCustomAttribute(typeof (MatchParameterWithPropertyAttribute));

                        #region Property Search

                        foreach (PropertyInfo p in type.GetProperties())
                        {
                            //search property
                            if (p.Name == ctorAttr.Property)
                            {
                                defaultValue = (int) GetPropertyDefaultValue(p);
                            }
                        }

                        #endregion

                        userInstance = (Attributes.User) Activator.CreateInstance(type, defaultValue);
                        
                        
                    }
                    else
                    {
                        userInstance = (Attributes.User) Activator.CreateInstance(type,attribute.Id);
                    }
                    InitializeUserProperties(userInstance, attribute);
                    users.Add(userInstance);
                } 
                             
            }
            return users;
        }

        public static AdvancedUser CreateAdvancedUser()
        {
            var assembly = typeof (AdvancedUser).Assembly;
            var userType = typeof (AdvancedUser);
            int id = 0;
            int externalId = 0;

            var userAttribute = (InstantiateAdvancedUserAttribute)assembly.GetCustomAttribute(typeof (InstantiateAdvancedUserAttribute));
            var ctor = userType.GetConstructor(new[] {typeof (int), typeof (int)});
            var ctorMatchAttributes = ctor.GetCustomAttributes().Select(a => a as MatchParameterWithPropertyAttribute).ToList();
            //pretend that we know argument's names and positions
            
            if (userAttribute.Id == null) //some temp copy paste
            {
                foreach (var ctorMatchAttribute in ctorMatchAttributes)
                {
                    var propertyInfo = userType.GetProperty(ctorMatchAttribute.Property);
                    var deafultValue = (int)GetPropertyDefaultValue(propertyInfo);
                    if (propertyInfo.Name == "Id")
                        id = deafultValue;
                }
            }
            else
            {
                id = userAttribute.Id.Value;
            }

            externalId = userAttribute.ExternalId;
            var userInstance = (AdvancedUser)Activator.CreateInstance(userType, id, externalId);
            InitializeUserProperties(userInstance, userAttribute);
            Debug.WriteLine(userAttribute.FirstName + " " + userAttribute.LastName + " " + userAttribute.Id + " " + userAttribute.ExternalId);
            
            return userInstance;
        }

        private static object GetPropertyDefaultValue(PropertyInfo propertyInfo)
        {
            foreach (Attribute attr in propertyInfo.GetCustomAttributes(true))
            {
                var valueAttribute = attr as DefaultValueAttribute;
                if (valueAttribute == null) continue;

                DefaultValueAttribute dv = valueAttribute;
                return dv.Value;
            }
            //temp
            return null;
        }

        private static void InitializeUserProperties(Attributes.User userInstance, InstantiateUserAttribute instanceAttribute)
        {
            var userType = typeof (Attributes.User);
            var properties = userType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name == "LastName")
                {
                    propertyInfo.SetValue(userInstance, instanceAttribute.LastName);
                }
                if (propertyInfo.Name == "FirstName")
                {
                    propertyInfo.SetValue(userInstance, instanceAttribute.FirstName);
                }

            }
        }
    }
}