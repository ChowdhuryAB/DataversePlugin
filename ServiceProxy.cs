using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Apg.Shared.Core
{
    public class ServiceProxy : IOrganizationService
    {
        private readonly IOrganizationService _service;
        private readonly PluginPortfolio _pluginPortfolio;
        public ServiceProxy(IOrganizationService service, PluginPortfolio pluginPortfolio)
        {
            _service = service;
            _pluginPortfolio = pluginPortfolio;
        }
        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _pluginPortfolio.trace($"Associate({entityName}, {entityId}, {relationship.SchemaName}, {relatedEntities.Count})");
            if (_pluginPortfolio.TracingService.Verbose)
            {
                _pluginPortfolio.trace("Associated record(s):{0}", relatedEntities.Select(r => $"\n  {r.LogicalName} {r.Id} {r.Name}"));
            }
            var watch = Stopwatch.StartNew();
            _service.Associate(entityName, entityId, relationship, relatedEntities);
            watch.Stop();
            _pluginPortfolio.trace($"Associated in: {watch.ElapsedMilliseconds} ms");
        }

        public Guid Create(Entity entity)
        {
            _pluginPortfolio.trace($"Create({entity.LogicalName}) {entity.Id} ({entity.Attributes.Count} attributes)");
            if (_pluginPortfolio.TracingService.Verbose)
            {
                _pluginPortfolio.trace("\n{0}", entity.ExtractAttributes(null));
            }
            var watch = Stopwatch.StartNew();
            var result = _service.Create(entity);
            watch.Stop();
            _pluginPortfolio.trace($"Created in: {watch.ElapsedMilliseconds} ms");
            return result;
        }

        public void Delete(string entityName, Guid id)
        {
            _pluginPortfolio.trace($"Delete({entityName}, {id})");
            var watch = Stopwatch.StartNew();
            _service.Delete(entityName, id);
            watch.Stop();
            _pluginPortfolio.trace($"Deleted in: {watch.ElapsedMilliseconds} ms");
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _pluginPortfolio.trace($"Disassociate({entityName}, {entityId}, {relationship.SchemaName}, {relatedEntities.Count})");
            if (_pluginPortfolio.TracingService.Verbose)
            {
                _pluginPortfolio.trace("Disassociated record(s):{0}", relatedEntities.Select(r => $"\n  {r.LogicalName} {r.Id} {r.Name}"));
            }
            var watch = Stopwatch.StartNew();
            _service.Disassociate(entityName, entityId, relationship, relatedEntities);
            watch.Stop();
            _pluginPortfolio.trace($"Disassociated in: {watch.ElapsedMilliseconds} ms");
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            _pluginPortfolio.trace($"Execute({request.RequestName})");
            if (_pluginPortfolio.TracingService.Verbose && request is ExecuteFetchRequest)
            {
                _pluginPortfolio.trace("FetchXML: {0}", ((ExecuteFetchRequest)request).FetchXml);
            }
            var watch = Stopwatch.StartNew();
            var result = _service.Execute(request);
            watch.Stop();
            _pluginPortfolio.trace($"Executed in: {watch.ElapsedMilliseconds} ms");
            return result;
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            _pluginPortfolio.trace($"Retrieve({entityName}, {id}, {columnSet.Columns.Count})");
            if (_pluginPortfolio.TracingService.Verbose)
            {
                _pluginPortfolio.trace("Columns:{0}", columnSet.Columns.Select(c => "\n  " + c));
            }
            var watch = Stopwatch.StartNew();
            var result = _service.Retrieve(entityName, id, columnSet);
            watch.Stop();
            _pluginPortfolio.trace($"Retrieved in: {watch.ElapsedMilliseconds} ms");
            if (_pluginPortfolio.TracingService.Verbose)
            {
                _pluginPortfolio.trace("Retrieved\n{0}", result.ExtractAttributes(null));
            }
            return result;
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            _pluginPortfolio.trace("RetrieveMultiple({0})", query is QueryExpression ? ((QueryExpression)query).EntityName : query is QueryByAttribute ? ((QueryByAttribute)query).EntityName : query is FetchExpression ? "fetchxml" : "unkstartn");
            if (_pluginPortfolio.TracingService.Verbose)
            {
                var fetch = ((QueryExpressionToFetchXmlResponse)_pluginPortfolio.Service.Execute(new QueryExpressionToFetchXmlRequest() { Query = query })).FetchXml;
                _pluginPortfolio.trace("Query: {0}", fetch);
            }
            var watch = Stopwatch.StartNew();
            var result = _service.RetrieveMultiple(query);
            watch.Stop();
            _pluginPortfolio.trace($"Retrieved {result.Entities.Count} records in: {watch.ElapsedMilliseconds} ms");
            return result;
        }

        public void Update(Entity entity)
        {
            _pluginPortfolio.trace($"Update({entity.LogicalName}) {entity.Id} ({entity.Attributes.Count} attributes)");
            if (_pluginPortfolio.TracingService.Verbose)
            {
                _pluginPortfolio.trace("\n{0}", entity.ExtractAttributes(null));
            }
            var watch = Stopwatch.StartNew();
            _service.Update(entity);
            watch.Stop();
            _pluginPortfolio.trace($"Updated in: {watch.ElapsedMilliseconds} ms");
        }
    }
}
