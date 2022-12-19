using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Metadata;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Plugin
{
    public class EmailUpdate : IPlugin
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
                {
                    // Plug-in business logic goes here.
                    string email = string.Empty;
                    if (account.Attributes.Contains("emailaddress1"))

                    {
                        email = account.Attributes["emailaddress1"].ToString();
                        Guid accountId = account.Id;
                        //Guid accountId = ((EntityReference)account.Attributes["accountid"]).Id; 
                       // Guid accountId = (Guid)account.GetAttributeValue<EntityReference>("accountid").Id;
                        QueryExpression query = new QueryExpression("contact");
                        query.ColumnSet = new ColumnSet(new string[] { "parentcustomerid" });
                        //query.Criteria.AddCondition(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, accountId));
                        query.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal, accountId);
                        EntityCollection AllAccounts = service.RetrieveMultiple(query);
                        tracingService.Trace(email);
                        tracingService.Trace(accountId.ToString());

                        foreach (var act in AllAccounts.Entities)

                        {

                            if (act.Attributes.Contains("emailaddress1"))

                            {

                                act.Attributes["emailaddress1"] = email;


                            }

                            else

                            {

                                act.Attributes.Add("emailaddress1", email);

                                service.Update(act);

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
