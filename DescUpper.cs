using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Plugin
{
    internal class DescUpper: IPlugin
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
                Entity entity = (Entity)context.InputParameters["Target"];

                try
                {
                    // Plug-in business logic goes here.
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        // Obtain the target entity from the input parameters.
                        Entity contactEntity = (Entity)context.InputParameters["Target"];

                        // Verify that the target entity represents an contact.
                        if (entity.LogicalName == "contact")
                        {
                            if (contactEntity.Attributes.Contains("description"))
                            {
                                // Get the value from description field, you can only get this value from context, if user has put some value in it.
                                String Upper_Desc = (contactEntity["description"].ToString()).ToUpper();

                                // Set the description in upper case  value in description field
                                contactEntity["description"] = Upper_Desc;

                                //You no need to write service.Update(contact entity); if registering the plugin on Pre-Operation Stage

                            }

                        }
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
