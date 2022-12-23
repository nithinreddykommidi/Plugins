using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;

namespace Plugin
{
    internal class AddressCheck : IPlugin
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
                Entity contact = (Entity)context.InputParameters["Target"];

                try
                {
                    string address1 = string.Empty;
                    string address2 = string.Empty;
                    string address3 = string.Empty;
                    string NewAddress = string.Empty;
                    // Plug-in business logic goes here.
                     
                    if (contact.Attributes.Contains("nw_newaddresstype") && contact.Attributes["nw_newaddresstype"] != null)
                    {
                        NewAddress = contact.FormattedValues["nw_newaddresstype"];
                        //NewAddress = (contact.GetAttributeValue<OptionSetValue>("nw_newaddresstype").Value).ToString();
                        tracingService.Trace(NewAddress);
                        Guid contactID = contact.Id;
                        //tracingService.Trace(contactID.ToString());

                        if (contact.Attributes.Contains("address1_addresstypecode") && contact.Attributes["address1_addresstypecode"] != null)
                        {
                            //contact.GetAttributeValue<OptionSetValue>("").Value;


                            address1 = contact.FormattedValues["address1_addresstypecode"];
                            //address1 = (contact.GetAttributeValue<OptionSetValue>("address1_addresstypecode").Value).ToString();
                        }
                        if (contact.Attributes.Contains("address2_addresstypecode") && contact.Attributes["address2_addresstypecode"] != null)
                        {
                            address2 = contact.FormattedValues["address2_addresstypecode"];
                        }
                        if (contact.Attributes.Contains("address3_addresstypecode") && contact.Attributes["address3_addresstypecode"] != null)
                        {
                            address3 = contact.FormattedValues["address3_addresstypecode"];
                        }
                    }

                    if(NewAddress == address1 || NewAddress == address2 || NewAddress == address3) 
                    {
                        throw new InvalidPluginExecutionException(NewAddress + "type already exists");
                    }
                    else
                    {
                        contact.Attributes["nw_newaddresstype"] = NewAddress;
                        service.Update(contact);
                    }


                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in MyPlug-in.", ex);
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
