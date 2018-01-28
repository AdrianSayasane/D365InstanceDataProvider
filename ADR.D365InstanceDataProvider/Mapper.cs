using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.D365InstanceDataProvider
{
    public class Mapper
    {
        public static QueryExpression MapQuery(MetadataHelper metadataHelper, QueryExpression query)
        {
            QueryExpression externalQuery = new QueryExpression() {
                ColumnSet = MapColumnSet(metadataHelper, query.ColumnSet),
                Criteria = MapCriteria(metadataHelper, query.Criteria),
                Distinct = query.Distinct,
                EntityName = metadataHelper.GetExternalEntityName(),
                NoLock = query.NoLock,
                PageInfo = query.PageInfo,
                TopCount = query.TopCount,
            };

            foreach(var order in query.Orders)
            {
                externalQuery.AddOrder(metadataHelper.GetExternalAttributeName(order.AttributeName), order.OrderType);
            }

            //TODO: What to do with query.LinkEntities?

            return query;
        }

        public static ColumnSet MapColumnSet(MetadataHelper metadataHelper, ColumnSet columnSet)
        {
            ColumnSet externalColumnSet = new ColumnSet()
            {
                AllColumns = columnSet.AllColumns,
            };

            foreach(var col in columnSet.Columns)
            {
                externalColumnSet.AddColumn(metadataHelper.GetExternalAttributeName(col));
            }

            return externalColumnSet;
        }

        public static FilterExpression MapCriteria(MetadataHelper metadataHelper, FilterExpression criteria)
        {
            FilterExpression externalCriteria = new FilterExpression() {
                FilterHint = criteria.FilterHint,
                FilterOperator = criteria.FilterOperator,
                IsQuickFindFilter = criteria.IsQuickFindFilter
            };

            foreach(var condition in criteria.Conditions)
            {
                var externalCondition = new ConditionExpression()
                {
                    AttributeName = metadataHelper.GetExternalAttributeName(condition.AttributeName),
                    EntityName = metadataHelper.GetExternalEntityName(),
                    Operator = condition.Operator
                };
                
                foreach(var val in condition.Values)
                {
                    externalCondition.Values.Add(val);
                }

                externalCriteria.AddCondition(externalCondition);
            }

            //TODO: What to do with criteria.Filters? Map recursively?

            return externalCriteria;
        }

        public static EntityCollection MapExternalResults(MetadataHelper metadataHelper, EntityCollection externalResults)
        {
            EntityCollection results = new EntityCollection()
            {
                EntityName = externalResults.EntityName,
                MinActiveRowVersion = externalResults.MinActiveRowVersion,
                MoreRecords = externalResults.MoreRecords,
                PagingCookie = externalResults.PagingCookie,
                TotalRecordCount = externalResults.TotalRecordCount,
                TotalRecordCountLimitExceeded = externalResults.TotalRecordCountLimitExceeded
            };
           
            foreach (var externalResult in externalResults.Entities)
            {
                results.Entities.Add(MapExternalResult(metadataHelper, externalResult));
            }

            return results;
        }

        public static Entity MapExternalResult(MetadataHelper metadataHelper, Entity externalResult)
        {
            Entity entity = new Entity()
            {
                Id = externalResult.Id,
                LogicalName = externalResult.LogicalName,
                RowVersion = externalResult.RowVersion
            };
            
            foreach (var externalAttribute in externalResult.Attributes)
            {
                var attributeName = metadataHelper.GetAttributeName(externalAttribute.Key);

                if (attributeName != null)
                {
                    entity.Attributes.Add(attributeName, externalAttribute.Value);
                }
            }

            return entity;
        }
    }
}
