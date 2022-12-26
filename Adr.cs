using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

namespace Plugin
{
    public class Adr:IPlugin
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
                Entity customeraddress = (Entity)context.InputParameters["Target"];

                try
                {
                    // Plug-in business logic goes here.  
                    // Creating address
                    int AdrType;
                    int others = 0;
                    Guid contactId = ((EntityReference)customeraddress.Attributes["parentid"]).Id;
                    int currentType = ((OptionSetValue)customeraddress["addresstypecode"]).Value;
                    
                    
                    QueryExpression query = new QueryExpression("customeraddress");
                    query.ColumnSet = new ColumnSet(new string[] { "parentid", "addresstypecode" , "customeraddressid" });
                    query.Criteria.AddCondition("parentid", ConditionOperator.Equal, contactId);
                    EntityCollection collection = service.RetrieveMultiple(query);

                    /* string XmlQuery = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
  "<entity name='customeraddress'>" +
    "<attribute name='name' />" +
    "<attribute name='line1' />" +
    "<attribute name='city' />" +
    "<attribute name='postalcode' />" +
    "<attribute name='telephone1' />" +
    "<attribute name='customeraddressid' />" +
    "<order attribute='name' descending='false' />" +
    "<filter type='and'>" +
      "<condition attribute='addresstypecode' operator='eq' value='4' />" +
    "</filter>" +
    "<link-entity name='contact' from='contactid' to='parentid' link-type='inner' alias='aa' />" +
    "<filter type='and'>" +
    "<condition attribute='parent' operator='eq' value='" + contactId + "' />" +
    "</filter>" +
"</link-entity>" +
  "</entity>" +
"</fetch>";
                    EntityCollection XmlResults = service.RetrieveMultiple(new FetchExpression(XmlQuery));
                    if (XmlResults.Entities.Count>2)
                    {
                        throw new InvalidPluginExecutionException("contact cannot have more than 2 other addresses");
                    }
                    */


                    //int[] AdrTypes = new int[10];
                    //int i = 0;
                    foreach (Entity address in collection.Entities)
                    {
                        tracingService.Trace(others.ToString());
                        AdrType = address.GetAttributeValue<OptionSetValue>("addresstypecode").Value;
                        if (address.Attributes.Contains("addresstypecode") && currentType != 4)
                        {
                            
                            if (currentType == AdrType)
                            {
                                throw new InvalidPluginExecutionException("contact already had adress type" + currentType.ToString());
                            }

                        }

                        else if (AdrType == 4)
                        {
                            others++;
                            if (others >= 2 && currentType == 4)
                            {
                                throw new InvalidPluginExecutionException("contact cannot have more than 2 other addresses");
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
