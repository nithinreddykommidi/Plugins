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
    public class CountOfEmployees : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
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
                    string CountOf = string.Empty;
                    int number;



                    if (account.Attributes.Contains("nw_countofemp"))
                    {
                        CountOf = account.Attributes["nw_countofemp"].ToString();
                        number = Convert.ToInt32(CountOf);
                        Guid accountId = account.Id;
                        


                        QueryExpression query = new QueryExpression("contact");
                        query.ColumnSet = new ColumnSet(new string[] { "parentcustomerid", "firstname", "lastname" });
                        //query.Criteria.AddCondition(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, accountId));
                        query.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal, accountId);
                        EntityCollection collection = service.RetrieveMultiple(query);



                        int CurrentCts = collection.Entities.Count;

                        if (CurrentCts == 0)
                        {
                            string actname = account.Attributes["name"].ToString();
                            for (int i = 0; i < number; i++)
                            {
                                Entity newContact = new Entity("contact");
                                // if(account.Attributes.Contains("name"))
                                // {
                                newContact["firstname"] = actname;
                                //  }

                                newContact["lastname"] = i.ToString();
                                newContact["parentcustomerid"] = new EntityReference("account", accountId);
                                service.Create(newContact);

                            }
                        }
                        //if ((CurrentCts == 0 && CurrentCts < number) || (CurrentCts < number && CurrentCts != 0))
                        if (CurrentCts < number && CurrentCts != 0)
                        {
                            Entity preimage = (Entity)context.PreEntityImages["preimage"];
                            string name = preimage.Attributes["name"].ToString();
                            for (int i = CurrentCts; i <= number; i++)
                            {
                                Entity newContact = new Entity("contact");
                                // if(account.Attributes.Contains("name"))
                                // {
                                newContact["firstname"] = name;
                                //  }

                                newContact["lastname"] = i.ToString();
                                newContact["parentcustomerid"] = new EntityReference("account", accountId);
                                service.Create(newContact);

                            }
                        }


                        else if (CurrentCts > number)

                        {
                            tracingService.Trace(CurrentCts.ToString());
                            foreach (Entity contact in collection.Entities)
                            {
                                Guid Contact_ID = contact.Id;
                                string LName = contact["lastname"].ToString();
                                int lName = Convert.ToInt32(LName);
                                if (lName > number)
                                {
                                    service.Delete("contact", Contact_ID);
                                }



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