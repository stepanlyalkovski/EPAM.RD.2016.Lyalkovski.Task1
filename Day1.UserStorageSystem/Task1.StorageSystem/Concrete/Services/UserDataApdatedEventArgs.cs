using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Entities;

namespace Task1.StorageSystem.Concrete.Services
{
    public class UserDataApdatedEventArgs : EventArgs
    {
        public User User { get; set; }
    }
}
