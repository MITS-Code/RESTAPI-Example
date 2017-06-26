using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;

namespace RESTAPIExample
{
    class Program
    {
        static void Main(string[] args)
        {
            #region User/Password Obfuscation
            ConfigReader cr = new ConfigReader(); //See class for more information.

            string Username = cr.username;
            string Password = cr.password;
            #endregion

            Console.Out.WriteLine("This is still very BETA");
            Console.Out.WriteLine("Username: " + Username);
            Console.Out.WriteLine("Password: " + Password);
            Console.ReadKey();
           
        }
    }
}
