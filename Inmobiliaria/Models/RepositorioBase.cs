using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioBase
    {
        
        protected readonly string connectionString;

        public RepositorioBase()
        {
            connectionString = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = BDInmobiliaria; Integrated Security = True;";
        }
      
    }
}
