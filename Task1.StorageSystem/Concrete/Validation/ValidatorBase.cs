using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.StorageSystem.Concrete.Validation
{
    public abstract class ValidatorBase<T>
    {
        public class Rule
        {
            public Func<T,bool> Test { get; set; }
            public string Message { get; set; }
          
        }
        protected abstract IEnumerable<Rule> Rules { get; }

        private IEnumerable<string> GetMessages(T t)
        {
            return Rules?.Where(r => !r.Test(t)).Select(r => r.Message);
        }

        public void Validate(T t)
        {
            var errorMessages = GetMessages(t);

            if (errorMessages == null)
                return;

            var messages = errorMessages as IList<string> ?? errorMessages.ToList();

            if (messages.Any())
            {
                throw new ArgumentException("Entity is not valid:\n" + string.Join("\n", messages));
            }
        }

    }
}
