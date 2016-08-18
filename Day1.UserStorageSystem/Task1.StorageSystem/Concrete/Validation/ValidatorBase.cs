namespace Task1.StorageSystem.Concrete.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public abstract class ValidatorBase<T> : MarshalByRefObject
    {
        protected abstract IEnumerable<Rule> Rules { get; }

        public IEnumerable<string> Validate(T t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "User must not be a null");
            }

            return Rules?.Where(r => !r.Test(t)).Select(r => r.Message);
        }

        public class Rule
        {
            public Func<T, bool> Test { get; set; }

            public string Message { get; set; }
        }
    }
}
