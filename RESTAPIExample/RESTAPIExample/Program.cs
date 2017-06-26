using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using RestSharp;
using RestSharp.Authenticators;

namespace RESTAPIExample
{
    class Program
    {
        /* This application: 
         * Seeks to use the ZoHo Desk API as an example of how to work with REST API's in general
         * Will always be slightly unstable as I will not be implementing Threadsafe async tasks.
         * Will not update with any changes made to ZoHo's API. Although, changing what is needed should not be very difficult.
         */
        public static void displayMenu()
        {
            Console.Clear();
            Console.Out.WriteLine("1. Authorization");
            Console.Out.WriteLine("2. OrgId");
            Console.Out.WriteLine("2. Get");
            Console.Out.WriteLine("3. Post");
            Console.Out.WriteLine("X. Exit");
        } 
        static void Main(string[] args)
        {
            #region User/Password Obfuscation
            ConfigReader cr = new ConfigReader(); //See class for more information.

            string Username = cr.username;
            string Password = cr.password;
            string orgId = cr.orgId;
            string Authorization = cr.Authorization;
            #endregion

            Console.Out.WriteLine("This is still very BETA");
            Console.Out.WriteLine("Username: " + Username);
            Console.Out.WriteLine("Password: " + Password);
            Console.ReadKey();

            //Using RESTSharp NuGet Package
            var client = new RestClient("https://desk.zoho.com");
           
            #region Main Loop

            while (true)
            {
                displayMenu();
                var input = Console.Read();
                try
                {
                    char ch = Convert.ToChar(input);
                    switch (ch)
                    {
                        case '1': //Should only need to happen once.
                            Console.Out.WriteLine("Authenticating");
                            //client.Authenticator = new HttpBasicAuthenticator(Username, Password);Not needed for this API due to manual authentication and authToken retrieval
                            var getAuthToken = new RestClient("https://accounts.zoho.com");//Client is the main API hosting site (essentially)
                            var getToken = new RestRequest("/apiauthtoken/nb/create", Method.POST);//Request is the drilled down string of what we want from the client
                            #region Parameter Addition
                            getToken.AddParameter("SCOPE", "ZohoSupport/supportapi,ZohoSearch/SearchAPI");
                            getToken.AddParameter("EMAIL_ID", Username);
                            getToken.AddParameter("PASSWORD", Password);
                            //The above is equivalent to: "?SCOPE=ZohoSupport/supportapi,ZohoSearch/SearchAPI&EMAIL_ID=[Username]&PASSWORD=[Password];
                            #endregion //Parameters are what we're trying to achieve with our application
                            var response = getAuthToken.Execute(getToken);//Execution

                            Console.Out.Write(response.Content + "\nFinished");
                            Console.ReadKey();
                            break;

                        case '2':
                            Console.Out.WriteLine("Getting orgId's");

                            var orgRequest = new RestRequest("/api/v1/organizations", Method.GET);
                            orgRequest.AddHeader("Authorization", Authorization);
                            var orgResponse= client.Execute(orgRequest);

                            Console.Out.Write(orgResponse.Content + "\nFinished");
                            Console.ReadKey();
                            break;

                        case '3':
                            Console.Out.WriteLine("Getting");

                            var getterRequest = new RestRequest("/api/v1/tickets", Method.GET);
                            getterRequest.AddHeader("orgId", orgId);
                            getterRequest.AddHeader("Authorization", Authorization);
                            var getterResponse = client.Execute(getterRequest);

                            Console.Out.Write(getterResponse.Content + "\nFinished");
                            Console.ReadKey();
                            break;

                        case '4':
                            Console.Out.WriteLine("Posting");

                            var posterRequest = new RestRequest("tickets", Method.POST);
                            posterRequest.AddHeader("header", "value");
                            posterRequest.AddParameter("name", "value", ParameterType.RequestBody);

                            Console.ReadKey();
                            break;

                        case 'X': //Fallthrough logic
                        case 'x':
                            Console.Out.WriteLine("Exiting");
                            Environment.Exit(0);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.Clear();
                    Console.Error.WriteLine(e);
                    Console.ReadKey();
                }
            }
            #endregion
        }
    }
}
