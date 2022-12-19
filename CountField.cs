﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Metadata;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Plugin
{
    public class CountField: IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Extract the tracing service for use in debugging sandboxed plug-ins.  
            // If you are not registering the plug-in in the sandbox, then you do  
            // not have to add any tracing service related code.  
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference which you will need for  
            // web service calls.  
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);



            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity account = (Entity)context.InputParameters["Target"];

                try
                {   string CountOf = string.Empty;
                    int number;

                    if (account.Attributes.Contains("nw_countofemp"))
                    {
                        CountOf = account.Attributes["nw_countofemp"].ToString();
                        number = Convert.ToInt32(CountOf);
                        Guid accountId = account.Id;
                        account.Attributes.Add("description", CountOf);


                        for (int i = 0; i < number; i++)
                        {
                            Entity contact = new Entity();
                            contact.LogicalName = "contact";
                            contact["firstname"] = "contact";

                            contact["lastname"] = i.ToString();
                            contact["description"] = accountId.ToString();

                            //contact.Attributes.Add("parentcustomerid",account.ToEntityReference());
                            service.Create(contact);

                        }

                    }
                    // Plug-in business logic goes here.  

                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in MyPlug-in.",ex);
                }
                catch (Exception ex)
                {
                    tracingService.Trace("MyPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
