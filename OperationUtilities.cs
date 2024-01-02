using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apg.Shared.Core.Utilities
{
    public class OperationUtilities
    {
        /// <summary>
        /// Get Fetch records count.
        /// </summary>
        /// <param name="entityCollection"></param>
        /// <param name="aliasAttributeName"></param>
        /// <returns></returns>
        public static int GetEntityFetchRecordCount(EntityCollection entityCollection, string aliasAttributeName)
        {
            if (entityCollection?.Entities != null && entityCollection.Entities.Count != 0)
            {
                var entity = entityCollection.Entities[0];
                AliasedValue value = (AliasedValue)entity[aliasAttributeName];
                return (int)value.Value;
            }

            return 0;
        }

        /// <summary>
        /// Delete Record Change History.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="entityId"></param>
        public static void DeleteRecordChangeHistory(IOrganizationService service, string entityLogicalName, Guid entityId)
        {
            if (service == null)
            {
                return;
            }

            var deteteChangeHistoryRecordRequest = new DeleteRecordChangeHistoryRequest();
            EntityReference entityRef = new EntityReference(entityLogicalName, entityId);
            deteteChangeHistoryRecordRequest.Target = entityRef;
            service.Execute(deteteChangeHistoryRecordRequest);
        }

        /// <summary>
        /// State Transition.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityId"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="stateCodFieldeName"></param>
        /// <param name="statusCodeFieldName"></param>
        /// <param name="stateCode"></param>
        /// <param name="statusCode"></param>
        public static void SetStateRequest(IOrganizationService service, Guid entityId, string entityLogicalName, string stateCodFieldeName, string statusCodeFieldName, int stateCode, int statusCode)
        {
            if (service == null)
            {
                return;
            }

            var entity = new Entity(entityLogicalName, entityId);
            entity.Attributes.Add(stateCodFieldeName, new OptionSetValue(stateCode));
            entity.Attributes.Add(statusCodeFieldName, new OptionSetValue(statusCode));
            service.Update(entity);
        }

        /// <summary>
        /// Get Associated NToN Relations.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="tracingService"></param>
        /// <param name="entityId"></param>
        /// <param name="relationshipname"></param>
        /// <param name="filterColumn"></param>
        /// <param name="getColumn"></param>
        /// <returns></returns>
        public static int GetNToNRelations(IOrganizationService service, ITracingService tracingService, Guid entityId, string relationshipname, string filterColumn, string getColumn)
        {
            // Fetching N:N associated to Records.
            QueryExpression query = new QueryExpression(relationshipname);
            query.ColumnSet.AddColumns(getColumn, filterColumn);
            query.Criteria = new FilterExpression();
            query.Criteria.AddCondition(filterColumn, ConditionOperator.Equal, entityId);

            EntityCollection relatedDataProcessingActivities = service.RetrieveMultiple(query);
            if (EntityUtilities.EntityCollectionIsNotNullOrEmpty(relatedDataProcessingActivities))
            {
                return relatedDataProcessingActivities.Entities.Count;
            }

            return 0;
        }

        /// <summary>
        /// Retrieves main forms for the specified entity.
        /// </summary>
        /// <param name="logicalName">Entity logical name.</param>
        /// <param name="service">Crm organization service.</param>
        /// <returns>Document containing all forms definition.</returns>
        public static IEnumerable<Entity> GetEntityFormList(string logicalName, IOrganizationService service)
        {
            var qe = new QueryExpression("systemform")
            {
                ColumnSet = new ColumnSet("formxml"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("objecttypecode", ConditionOperator.Equal, logicalName),
                        new ConditionExpression("type", ConditionOperator.In, new[] { 2, 7 }),
                    },
                },
            };

            try
            {
                return service.RetrieveMultiple(qe).Entities;
            }
            catch
            {
                qe.Criteria.Conditions.RemoveAt(qe.Criteria.Conditions.Count - 1);
                return service.RetrieveMultiple(qe).Entities;
            }
        }

        /// <summary>
        /// Add Business Days to current date.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static DateTime AddBusinessDays(DateTime date, int days)
        {
            if (days < 0)
            {
                throw new ArgumentException("days cannot be negative", "days");
            }

            if (days == 0) return date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
                days -= 1;
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
                days -= 1;
            }

            date = date.AddDays(days / 5 * 7);
            int extraDays = days % 5;

            if ((int)date.DayOfWeek + extraDays > 5)
            {
                extraDays += 2;
            }

            return date.AddDays(extraDays);

        }

        /// <summary>
        /// Get no of Business Days between two dates.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int GetBusinessDays(DateTime start, DateTime end)
        {
            if (start.DayOfWeek == DayOfWeek.Saturday)
            {
                start = start.AddDays(2);
            }
            else if (start.DayOfWeek == DayOfWeek.Sunday)
            {
                start = start.AddDays(1);
            }

            if (end.DayOfWeek == DayOfWeek.Saturday)
            {
                end = end.AddDays(-1);
            }
            else if (end.DayOfWeek == DayOfWeek.Sunday)
            {
                end = end.AddDays(-2);
            }

            int diff = (int)end.Subtract(start).TotalDays;

            int result = diff / 7 * 5 + diff % 7;

            if (end.DayOfWeek < start.DayOfWeek)
            {
                return result - 2;
            }
            else
            {
                return result;
            }
        }
    }
}
