using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse
{
    class Client
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public Client(string name, string phone, string email)
        {
            this.Name = name;
            this.Phone = phone;
            this.Email = email;
        }
    }
}
