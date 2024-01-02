using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apg.Shared.Core.Utilities
{
    /// <summary>
    /// Entity collection, Entity null checks, Field Null checks, Get respective Column values from Tables,Images.
    /// </summary>
    public static class EntityUtilities
    {
        /// <summary>
        /// Check if Entity Contains specified Attribute.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool AttributeIsNotNullOrEmpty(AttributeCollection attributes, string fieldName)
        {
            if (attributes.Contains(fieldName) && attributes[fieldName] != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if Entity or Imgae contains attribute.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="imageAttributes"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool AttributeIsNotNullOrEmpty(AttributeCollection attributes, AttributeCollection imageAttributes, string fieldName)
        {
            if (attributes.Contains(fieldName) && attributes[fieldName] != null)
            {
                return true;
            }
            else if (imageAttributes.Contains(fieldName) && imageAttributes[fieldName] != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if given Entity Collection contains any Entity.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool EntityCollectionIsNotNullOrEmpty(EntityCollection collection)
        {
            if (collection != null && collection.Entities != null && collection.Entities.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get Entity Ref from Entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Guid GetAttributeRefId(Entity entity, string fieldName)
        {
            var refId = entity.Attributes.Contains(fieldName) &&
                        entity.Attributes[fieldName] != null
                ? ((EntityReference)entity.Attributes[fieldName]).Id
                : Guid.Empty;

            return refId;
        }

        /// <summary>
        /// Get Entity Id from Entity or Image.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="image">Image</param>
        /// <param name="fieldName">FieldName</param>
        /// <returns>EntityRegGuid</returns>
        public static Guid GetAttributeRefId(Entity entity, Entity image, string fieldName)
        {
            var refId = entity.Attributes.Contains(fieldName) &&
                        entity.Attributes[fieldName] != null
                ? ((EntityReference)entity.Attributes[fieldName]).Id
                : image.Attributes.Contains(fieldName) &&
                        image.Attributes[fieldName] != null
                ? ((EntityReference)image.Attributes[fieldName]).Id : Guid.Empty;

            return refId;
        }

        /// <summary>
        /// Get Entity Reference.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="preImage"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static EntityReference GetAttributeRef(Entity entity, Entity preImage, string fieldName)
        {
            var entityRef = entity.Attributes.Contains(fieldName) &&
                        entity.Attributes[fieldName] != null
                ? ((EntityReference)entity.Attributes[fieldName])
                : preImage.Attributes.Contains(fieldName) &&
                        preImage.Attributes[fieldName] != null
                ? ((EntityReference)preImage.Attributes[fieldName]) : null;

            return entityRef;
        }

        /// <summary>
        /// Get Boolean Value for given field.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="preImage"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool GetAttributeBoolValue(Entity entity, Entity preImage, string fieldName)
        {
            var result = entity.Attributes.Contains(fieldName) &&
                        entity.Attributes[fieldName] != null
                ? ((bool)entity.Attributes[fieldName])
                : preImage.Attributes.Contains(fieldName) &&
                        preImage.Attributes[fieldName] != null
                ? ((bool)preImage.Attributes[fieldName]) : false;

            return result;
        }

        /// <summary>
        ///  Get Optionset value for given field.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="preImage"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int GetAttributeOptionSet(Entity entity, Entity preImage, string fieldName)
        {
            var result = entity.Attributes.Contains(fieldName) &&
                        entity.Attributes[fieldName] != null
                ? (((OptionSetValue)entity.Attributes[fieldName])).Value
                : preImage.Attributes.Contains(fieldName) &&
                        preImage.Attributes[fieldName] != null
                ? (((OptionSetValue)preImage.Attributes[fieldName])).Value : -1;

            return result;
        }

        /// <summary>
        /// Get Attribute Optionset Formatted Value.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="preImage"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetAttributeOptionSetFormattedValue(Entity entity, Entity preImage, string fieldName)
        {
            var result = entity.Attributes.Contains(fieldName) &&
                        entity.Attributes[fieldName] != null
                ? entity.FormattedValues[fieldName]
                : preImage.Attributes.Contains(fieldName) &&
                        preImage.Attributes[fieldName] != null
                ? preImage.FormattedValues[fieldName] : string.Empty;

            return result;
        }

        /// <summary>
        /// Get String value for the given field.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="preImage"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetAttributeString(Entity entity, Entity preImage, string fieldName)
        {
            var stringValue = entity.Attributes.Contains(fieldName) &&
                          entity.Attributes[fieldName] != null
                  ? entity.Attributes[fieldName].ToString()
                  : preImage.Attributes.Contains(fieldName) &&
                          preImage.Attributes[fieldName] != null
                  ? preImage.Attributes[fieldName].ToString() : string.Empty;

            return stringValue;
        }

        /// <summary>
        /// Get Int value for the given field.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="preImage"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int GetAttributeInt(Entity entity, Entity preImage, string fieldName)
        {
            var intValue = entity.Attributes.Contains(fieldName) &&
                          entity.Attributes[fieldName] != null
                  ? int.Parse(entity.Attributes[fieldName].ToString())
                  : preImage.Attributes.Contains(fieldName) &&
                          preImage.Attributes[fieldName] != null
                  ? int.Parse(preImage.Attributes[fieldName].ToString()) : -1;

            return intValue;
        }

        /// <summary>
        /// Get Decimal Value from given field.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="preImage"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static decimal GetAttributeDecimal(Entity entity, Entity preImage, string fieldName)
        {
            var intValue = entity.Attributes.Contains(fieldName) &&
                          entity.Attributes[fieldName] != null
                  ? decimal.Parse(entity.Attributes[fieldName].ToString())
                  : preImage.Attributes.Contains(fieldName) &&
                          preImage.Attributes[fieldName] != null
                  ? decimal.Parse(preImage.Attributes[fieldName].ToString()) : 0;

            return intValue;
        }

        /// <summary>
        /// Get Money value from given field.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="image"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Money GetAttributeMoney(Entity entity, Entity image, string fieldName)
        {
            var result = entity.Attributes.Contains(fieldName) &&
                         entity.Attributes[fieldName] != null
                 ? entity.GetAttributeValue<Money>(fieldName)
                 : image.Attributes.Contains(fieldName) &&
                         image.Attributes[fieldName] != null
                 ? image.GetAttributeValue<Money>(fieldName) : null;

            return result;
        }

        /// <summary>
        /// Get DateTime from given field value.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="preImage"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static DateTime GetAttributeDateTime(Entity entity, Entity preImage, string fieldName)
        {
            var dateTimeValue = entity.Attributes.Contains(fieldName) &&
                          entity.Attributes[fieldName] != null
                  ? DateTime.Parse(entity.Attributes[fieldName].ToString())
                  : preImage.Attributes.Contains(fieldName) &&
                          preImage.Attributes[fieldName] != null
                  ? DateTime.Parse(preImage.Attributes[fieldName].ToString()) : DateTime.MinValue;

            return dateTimeValue;
        }

        /// <summary>
        /// Map Source To Destination Attribute. Very usefull in case of cloning.
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <param name="destinationEntity"></param>
        /// <param name="fieldName"></param>
        public static void SetAttributeValue(Entity sourceEntity, Entity destinationEntity, string fieldName)
        {
            if (AttributeIsNotNullOrEmpty(sourceEntity.Attributes, fieldName))
            {
                destinationEntity.Attributes[fieldName] = sourceEntity.Attributes[fieldName];
            }
        }

    }
}
