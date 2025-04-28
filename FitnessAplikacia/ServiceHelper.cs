using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAplikacia
{
    public static class ServiceHelper
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static T GetService<T>() => ServiceProvider.GetRequiredService<T>();
    }

}
