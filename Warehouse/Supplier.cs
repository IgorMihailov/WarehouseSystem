using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse
{
    public class Supplier
    {
        public string Name { get; set;}
        public string ProdType { get; set;}
        public string Email { get; set; }

        public Supplier (string name, string prodType, string email)
        {
            this.Name = name;
            this.ProdType = prodType;
            this.Email = email;
        }

    }
}
