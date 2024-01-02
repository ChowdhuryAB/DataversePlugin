using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Apg.Shared.Core
{
    public partial class PluginPortfolio : IDisposable
    {

        public ServiceProxy Service { get; }

        public TracingService TracingService { get; }

        public IPluginExecutionContext PluginContext { get { return context as IPluginExecutionContext; } }

        public IWorkflowContext WorkflowContext { get { return context as IWorkflowContext; } }

        public Entity TargetEntity
        {
            get
            {
                if (context != null &&
                    context.InputParameters.Contains("Target") &&
                    context.InputParameters["Target"] is Entity)
                {
                    return (Entity)context.InputParameters["Target"];
                }
                return null;
            }
            set
            {
                context.InputParameters["Target"] = value;
            }
        }

        public EntityReference TargetEntityReference
        {
            get
            {
                if (context != null &&
                    context.InputParameters.Contains("Target") &&
                    context.InputParameters["Target"] is EntityReference)
                {
                    return (EntityReference)context.InputParameters["Target"];
                }
                return null;
            }
            set
            {
                context.InputParameters["Target"] = value;
            }
        }

        public Entity PreImage
        {
            get
            {
                if (context != null &&
                    context.PreEntityImages != null &&
                    context.PreEntityImages.Count > 0)
                {
                    return context.PreEntityImages.Values.FirstOrDefault();
                }
                return null;
            }
        }

        public Entity PostImage
        {
            get
            {
                if (context != null &&
                    context.PostEntityImages != null &&
                    context.PostEntityImages.Count > 0)
                {
                    return context.PostEntityImages.Values.FirstOrDefault();
                }
                return null;
            }
        }

        /// <summary>
        /// A combination of Target Entity, PreImage and PostImage Entity.
        /// </summary>
        public Entity CompleteEntity
        {
            get
            {
                if (completeEntity == null)
                {
                    completeEntity = TargetEntity.Clone().Merge(PostImage).Merge(PreImage);
                }
                return completeEntity;
            }
        }

        /// <summary>
        /// Constructor to be used from a Microsoft Dataverse plugins.
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider passed to the IPlugin.Execute method</param>
        public PluginPortfolio(IServiceProvider serviceProvider)
        {
            TracingService = new TracingService((ITracingService)serviceProvider.GetService(typeof(ITracingService)));
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);
            if (context.UserId != Guid.Empty && context.UserId != context.InitiatingUserId)
            {
                service = serviceFactory.CreateOrganizationService(context.UserId);
            }
            Service = new ServiceProxy(service, this);
        }

        /// <summary>
        /// Constructor to be used from a Microsoft Dataverse custom workflow activity.
        /// </summary>
        /// <param name="executionContext">CodeActivityContext passed to the CodeActivity.Execute method</param>
        public PluginPortfolio(CodeActivityContext executionContext)
        {
            TracingService = new TracingService(executionContext.GetExtension<ITracingService>());
            context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);
            if (context.UserId != Guid.Empty && context.UserId != context.InitiatingUserId)
            {
                service = serviceFactory.CreateOrganizationService(context.UserId);
            }
            Service = new ServiceProxy(service, this);
            CodeActivityContext = executionContext;
            Init();
        }

        /// <summary>
        /// Constructor to use when PluginPortfolio is used in custom applications
        /// </summary>
        /// <param name="service">IOrganizationService connected to Microsoft Dataverse</param>
        /// <param name="context"></param>
        /// <param name="trace"></param>
        public PluginPortfolio(IOrganizationService service, IPluginExecutionContext context, ITracingService trace)
        {
            TracingService = new TracingService(trace);
            Service = new ServiceProxy(service, this);
            this.context = context;
            Init();
        }

        /// <summary>
        /// Trace method automatically adding timestamp to each traced item
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Trace(string format, params object[] args)
        {
            TracingService.Trace(format, args);
        }

        /// <summary>
        /// Traces the text straight to the tracing service, without timestamps, indentation etc.
        /// </summary>
        /// <param name="text"></param>
        public void TraceRaw(string text)
        {
            TracingService.TraceRaw(text);
        }

        /// <summary>
        /// Call this function to start a block in the log.
        /// Log lines will be indented, until next call to TraceBlockEnd.
        /// Block label with be the name of the calling method.
        /// </summary>
        public void TraceBlockStart()
        {
            var label = new StackTrace().GetFrame(1).GetMethod().Name;
            TraceBlockStart(label);
        }

        /// <summary>
        /// Call this function to start a block in the log.
        /// Log lines will be indented, until next call to TraceBlockEnd.
        /// </summary>
        /// <param name="label">Label to set for the block</param>
        public void TraceBlockStart(string label)
        {
            TracingService.BlockBegin(label);
        }

        /// <summary>
        /// Call this to en a block in the log.
        /// </summary>
        public void TraceBlockEnd()
        {
            TracingService.BlockEnd();
        }

        /// <summary>
        /// Get label for specified optionset attribute and value
        /// </summary>
        /// <param name="entity">Entity where the attribute is used</param>
        /// <param name="attribute">Attribute name</param>
        /// <param name="value">Value of the optionset for which to return label</param>
        /// <returns></returns>
        public string GetOptionsetLabel(string entity, string attribute, int value)
        {
            trace($"Getting metadata for {entity}.{attribute}");
            var req = new RetrieveAttributeRequest
            {
                EntityLogicalName = entity,
                LogicalName = attribute
            };
            var resp = (RetrieveAttributeResponse)Service.Execute(req);
            var plmeta = (PicklistAttributeMetadata)resp.AttributeMetadata;
            if (plmeta == null)
            {
                throw new InvalidPluginExecutionException($"{entity}.{attribute} does not appear to be an optionset");
            }
            var result = plmeta.OptionSet.Options.FirstOrDefault(o => o.Value == value)?.Label?.UserLocalizedLabel?.Label;
            trace($"Returning label for value {value}: {result}");
            return result;
        }
        //Get Business Unit
        public EntityCollection FetchBusinessUnit(string BU_Name, IOrganizationService service)
        {
            var queryBU = new QueryExpression("businessunit");
            queryBU.NoLock = true;
            var query_name = BU_Name;
            queryBU.ColumnSet.AddColumns("name", "businessunitid");
            queryBU.Criteria.AddCondition("name", ConditionOperator.Equal, query_name);
            return service.RetrieveMultiple(queryBU);
        }

        public void Dispose()
        {
            if (TracingService != null)
            {
                TracingService.Dispose();
            }
        }

        #region private/internal members

        private readonly IExecutionContext context;

        private Entity completeEntity;

        private void Init()
        {
            LogTheContext(context);
            var entity = TargetEntity;
            if (entity != null)
            {
                var attrs = entity.ExtractAttributes(PreImage);
                trace($"Incoming {entity.LogicalName}\n{attrs}\n");
            }
        }

        private void LogTheContext(IExecutionContext context)
        {
            if (context == null)
                return;
            var step = context.OwningExtension != null ? !string.IsNullOrEmpty(context.OwningExtension.Name) ? context.OwningExtension.Name : context.OwningExtension.Id.ToString() : "null";
            var stage = context is IPluginExecutionContext ? ((IPluginExecutionContext)context).Stage : 0;
            trace($@"Context details:
                      Step:  {step}
                      Msg:   {context.MessageName}
                      Stage: {stage}
                      Mode:  {context.Mode}
                      Depth: {context.Depth}
                      Corr-Id:  {context.CorrelationId}
                      Type:  {context.PrimaryEntityName}
                      Id:    {context.PrimaryEntityId}
                      User:  {context.UserId}
                      IUser: {context.InitiatingUserId}
                    ");
            if (TracingService.Verbose)
            {
                var parentcontext = context is IPluginExecutionContext ? ((IPluginExecutionContext)context).ParentContext : null;
                if (parentcontext != null)
                {
                    LogTheContext(parentcontext);
                }
            }
        }
        internal string PrimaryAttribute(string entityName)
        {
            var metabase = (RetrieveEntityResponse)Service.Execute(new RetrieveEntityRequest()
            {
                LogicalName = entityName,
                EntityFilters = EntityFilters.Entity
            });
            trace($"Metadata retrieved for {entityName}");
            if (metabase != null)
            {
                EntityMetadata meta = metabase.EntityMetadata;
                var result = meta.PrimaryNameAttribute;
                trace($"Primary attribute is: {result}");
                return result;
            }
            else
            {
                throw new InvalidPluginExecutionException($"Unable to retrieve metadata/primaryattribute for entity: {entityName} ");
            }
        }

        internal void trace(string format, params object[] args)
        {
            Trace("[APG] " + format, args);
        }


        #endregion
    }
}
