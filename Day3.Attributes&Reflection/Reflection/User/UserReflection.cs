using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Attributes;

namespace Reflection.User
{
    public class UserReflection
    {
        public static IEnumerable<Attributes.User> CreateInstances()
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
                                foreach (Attribute attr in p.GetCustomAttributes(true))
                                {
                                    var valueAttribute = attr as DefaultValueAttribute;
                                    if (valueAttribute != null)
                                    {
                                        DefaultValueAttribute dv = valueAttribute;
                                        defaultValue = (int) dv.Value;
                                    }
                                }
                            }
                        }

                        #endregion

                        userInstance = (Attributes.User) Activator.CreateInstance(type, defaultValue);
                        
                        
                    }
                    else
                    {
                        userInstance = (Attributes.User) Activator.CreateInstance(type,attribute.Id);
                    }

                    var properties = type.GetProperties();
                    foreach (var propertyInfo in properties)
                    {
                        if (propertyInfo.Name == "LastName")
                        {
                            propertyInfo.SetValue(userInstance, attribute.LastName);
                        }
                        if (propertyInfo.Name == "FirstName")
                        {
                            propertyInfo.SetValue(userInstance, attribute.FirstName);
                        }
                        
                    }
                    users.Add(userInstance);
                    //Debug.WriteLine(userInstance.FirstName + " " + userInstance.LastName + " " + userInstance.Id);

                } 
                             
            }
            return users;
        }

    }
}