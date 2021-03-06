﻿using System;
using RestSharp;

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
            Console.Out.WriteLine("3. Get Tickets");
            Console.Out.WriteLine("4. Create Account");
            Console.Out.WriteLine("5. Create Contact");
            Console.Out.WriteLine("6. Post Tickets");
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

            #region Informational Output
            Console.Out.WriteLine(" This is still very BETA");
            Console.Out.WriteLine(" Username: " + Username);
            Console.Out.WriteLine(" Password: " + Password);
            Console.Out.WriteLine(" orgId: " + orgId);
            Console.Out.WriteLine(" Authorization: " + Authorization);
            Console.Out.WriteLine("\n   Press any key to continue");
            Console.ReadKey();
            #endregion

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
                        #region getAuthToken (Case 1)
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
                            Console.Out.WriteLine("\n   Press any key to continue");
                            Console.ReadKey();
                            break;
                        #endregion

                        #region getOrgId (Case 2)
                        case '2'://Completed
                            Console.Clear();
                            Console.Out.WriteLine("Getting orgId's\n");

                            var orgRequest = new RestRequest("/api/v1/organizations", Method.GET);
                            orgRequest.RequestFormat = DataFormat.Xml;
                            orgRequest.AddHeader("Authorization", Authorization);
                            var asyncHandle = client.ExecuteAsync<organizationList>(orgRequest, orgresponse =>
                            {
                                //Console.Out.WriteLine(orgresponse.Content + "\n***Finished***\n"); //Raw Output
                                foreach (organization o in orgresponse.Data.data)
                                {
                                    Console.Out.WriteLine(" ID: " + o.id);
                                    Console.Out.WriteLine(" isDefault: " + o.isDefault);
                                    Console.Out.WriteLine(" logoURL: " + o.logoURL);
                                    Console.Out.WriteLine(" organizationName: " + o.organizationName);
                                    Console.Out.WriteLine(" portalURL: " + o.portalURL);
                                    Console.Out.WriteLine(" ---------------\n");
                                }
                                Console.Out.WriteLine("\n   Press any key to continue");
                            });
                            Console.ReadKey();
                            break;
                        #endregion

                        #region getTickets (Case 3)
                        case '3':
                            Console.Clear();
                            Console.Out.WriteLine("Getting Tickets\n");

                            var getterRequest = new RestRequest("/api/v1/tickets", Method.GET);
                            getterRequest.AddHeader("orgId", orgId);
                            getterRequest.AddHeader("Authorization", Authorization);
                            var asyncHandle2 = client.ExecuteAsync<TicketList>(getterRequest, getterResponse =>
                            {
                                //Console.Out.WriteLine(getterResponse.Content + "\n***Finished***\n"); //Raw Output
                                foreach (Ticket o in getterResponse.Data.data)
                                {
                                    Console.Out.WriteLine(" ID: " + o.id);
                                    Console.Out.WriteLine(" Assignee ID: " + o.assigneeId);
                                    Console.Out.WriteLine(" Assignee Email: " + o.email);
                                    Console.Out.WriteLine(" Subject: " + o.subject);
                                    Console.Out.WriteLine(" Due Date: " + o.dueDate);
                                    Console.Out.WriteLine(" Status: " + o.status);
                                    Console.Out.WriteLine(" ---------------\n");
                                }
                                Console.Out.WriteLine("\n   Press any key to continue");
                            });
                            Console.ReadKey();
                            break;
                        #endregion

                        #region create account (Case 4)
                        case '4':
                            Console.Clear();
                            Console.Out.WriteLine("Creating Account...");
                            var accountRequest = new RestRequest("/api/v1/accounts", Method.POST);
                            accountRequest.AddHeader("orgId", orgId);
                            accountRequest.AddHeader("Authorization", Authorization);
                            Account account = new Account();
                            account.accountName = "Account Name";
                            accountRequest.AddJsonBody(account);
                            var accountResponse = client.Execute<Account>(accountRequest);
                            account = accountResponse.Data;
                            
                            Console.Out.WriteLine("ID: " + account.id);
                            Console.Out.WriteLine("AccountName: " + account.accountName);
                            Console.Out.WriteLine("---------------");
                            //Console.Out.WriteLine(accountResponse.Content);
                            Console.Out.WriteLine("REST Response: " + accountResponse.ResponseStatus);
                            Console.Out.WriteLine("HTTP Response: " + accountResponse.StatusCode);
                            Console.Out.WriteLine("\nPress any key to continue");
                            Console.ReadKey();
                            break;
                        #endregion

                        #region create contact (Case 5)
                        case '5':
                            Console.Clear();
                            Console.Out.WriteLine("Creating Contact...");

                            var contactRequest = new RestRequest("/api/v1/contacts", Method.POST);
                            contactRequest.AddHeader("orgId", orgId);
                            contactRequest.AddHeader("Authorization", Authorization);

                            Contact contact = new Contact();
                            contact.firstName = "Name";
                            contact.lastName = "Surname";
                            contact.accountId = "197572000000100097";
                            contactRequest.AddJsonBody(contact);

                            var contactResponse = client.Execute<Contact>(contactRequest);
                            Contact obj = contactResponse.Data;
                            Console.Out.WriteLine("ID: " + obj.id);
                            Console.Out.WriteLine("First Name: " + obj.firstName);
                            Console.Out.WriteLine("Last Name: " + obj.lastName);
                            Console.Out.WriteLine("Account ID: " + obj.accountId);
                            Console.Out.WriteLine("---------------");
                            //Console.Out.WriteLine(contactResponse.Content);
                            Console.Out.WriteLine("REST Response: " + contactResponse.ResponseStatus);
                            Console.Out.WriteLine("HTTP Response: " + contactResponse.StatusCode);
                            Console.Out.WriteLine("\nPress any key to continue");
                            Console.ReadKey();

                            break;
                        #endregion

                        #region postTicket (Case 6)
                        case '6': //Completed
                            Console.Clear();
                            Console.Out.WriteLine("Posting ticket...");

                            var postRequest = new RestRequest("/api/v1/tickets", Method.POST);
                            postRequest.RequestFormat = DataFormat.Json;

                            postRequest.AddHeader("orgId", orgId);
                            postRequest.AddHeader("Authorization", Authorization);
                            
                            Ticket toSend = new Ticket();

                            toSend.productId = "";
                            toSend.contactId = "197572000000106045"; // Need to create an account, to create a contact, to create a ticket associated with that contact.
                            toSend.subject = "This is a ticket :" + DateTime.Now;
                            toSend.dueDate = "2017-07-20T16:16:16.000Z";
                            toSend.departmentId = "197572000000006907";
                            toSend.channel = "Webapp";
                            toSend.description = "This is a ticket created through console app";
                            toSend.priority = "High";
                            toSend.classification = "Classification";
                            toSend.assigneeId = "197572000000073005";
                            toSend.phone = "072 760 7234";
                            toSend.category = "Category";
                            toSend.email = "email@student.monash.edu";
                            toSend.status = "Open";

                            postRequest.AddJsonBody(toSend);
                            var postResponse = client.Execute<Ticket>(postRequest);
                            Ticket t = postResponse.Data;

                            Console.Out.WriteLine("ID: " + t.id);
                            Console.Out.WriteLine("Contact ID: " + t.contactId);
                            Console.Out.WriteLine("Ticket Number: " + t.ticketNumber);
                            Console.Out.WriteLine("Subject: " + t.subject);
                            Console.Out.WriteLine("Description: " + t.description);
                            Console.Out.WriteLine("Email: " + t.email);
                            Console.Out.WriteLine("Priority: " + t.priority);
                            Console.Out.WriteLine("Channel: " + t.channel);
                            Console.Out.WriteLine("Status: " + t.status);

                            //Console.Out.WriteLine(postResponse.Content);
                            Console.Out.WriteLine("REST Response: " + postResponse.ResponseStatus);
                            Console.Out.WriteLine("HTTP Response: " + postResponse.StatusCode);

                            Console.Out.WriteLine("\n   Press any key to continue");
                            Console.ReadKey();
                            break;
                        #endregion

                        #region exit (case x/X)
                        case 'X': //Fallthrough logic
                        case 'x':
                            Console.Out.WriteLine("Exiting");
                            Environment.Exit(0);
                            break;
                            #endregion
                    }
                }
                catch (Exception e)
                {
                    Console.Clear();
                    Console.Error.WriteLine(e);
                    Console.Out.WriteLine("\n   Press any key to continue");
                    Console.ReadKey();
                }
            }
            #endregion
        }
    }
}