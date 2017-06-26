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
using System.Web;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Extensions;

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
            Console.Out.WriteLine("3. Get");
            Console.Out.WriteLine("4. Post");
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
            Console.Out.WriteLine("orgId: " + orgId);
            Console.Out.WriteLine("Authorization: " + Authorization);
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
                        case '1'://Completed
                            //Should only need to happen once.
                            Console.Clear();
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

                        case '2'://Completed
                            Console.Clear();
                            Console.Out.WriteLine("Getting orgId's");

                            var orgRequest = new RestRequest("/api/v1/organizations", Method.GET);
                            orgRequest.RequestFormat = DataFormat.Xml;
                            orgRequest.AddHeader("Authorization", Authorization);
                            var asyncHandle = client.ExecuteAsync<organizationList>(orgRequest, orgresponse =>
                            {
                                //Console.Out.WriteLine(orgresponse.Content + "\n***Finished***\n"); //Raw Output
                                foreach (organization o in orgresponse.Data.data)
                                {
                                    Console.Out.WriteLine("ID: " + o.id);
                                    Console.Out.WriteLine("isDefault: " + o.isDefault);
                                    Console.Out.WriteLine("logoURL: " + o.logoURL);
                                    Console.Out.WriteLine("organizationName: " + o.organizationName);
                                    Console.Out.WriteLine("portalURL: " + o.portalURL);
                                } 
                            });
                            Console.ReadKey();
                            break;

                        case '3':
                            Console.Clear();
                            Console.Out.WriteLine("Getting");

                            var getterRequest = new RestRequest("/api/v1/tickets", Method.GET);
                            getterRequest.AddHeader("orgId", orgId);
                            getterRequest.AddHeader("Authorization", Authorization);
                            var asyncHandle2 = client.ExecuteAsync<TicketList>(getterRequest, getterResponse =>
                            {
                                //Console.Out.WriteLine(getterResponse.Content + "\n***Finished***\n"); //Raw Output
                                foreach (Ticket o in getterResponse.Data.data)
                                {
                                    Console.Out.WriteLine("ID: " + o.id);
                                    Console.Out.WriteLine("Assignee ID: " + o.assigneeId);
                                    Console.Out.WriteLine("Assignee Email: " + o.email);
                                    Console.Out.WriteLine("Subject: " + o.subject);
                                    if (o.contact != null)
                                    {
                                        Console.Out.WriteLine("Contact Email: " + o.contact.email);
                                        Console.Out.WriteLine("Contact FirstName: " + o.contact.firstName);
                                        Console.Out.WriteLine("Contact LastName: " + o.contact.lastName);
                                        Console.Out.WriteLine("Contact Phone: " + o.contact.phone);
                                    }
                                    Console.Out.WriteLine("Due Date: " + o.dueDate);
                                    Console.Out.WriteLine("Status: " + o.status);
                                }
                            });
                            Console.ReadKey();
                            break;

                        case '4': //Not Even close to completion
                            Console.Clear();
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
