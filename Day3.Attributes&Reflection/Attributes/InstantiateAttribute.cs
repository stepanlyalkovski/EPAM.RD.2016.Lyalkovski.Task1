using System;

namespace Attributes
{
    // Should be applied to classes only.
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class InstantiateUserAttribute : Attribute
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public InstantiateUserAttribute()
        {
            
        }
        public InstantiateUserAttribute(int id , string firsName, string lastName)
        {
            FirstName = firsName;
            LastName = lastName;
            Id = id;
        }
        public InstantiateUserAttribute(string firsName, string lastName)
        {
            FirstName = firsName;
            LastName = lastName;
        }

        //public InstantiateUserAttribute(int num1, string str1, string str2, int num2)
        //{
           
        //}
    }
}
