using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace JPNSample.API.Core
{
    public static class EnumExtensions
    {
        public static T GetAttributeOfType<T>(this object obj) where T : Attribute
        {
            var type = obj.GetType();
            var memInfo = type.GetMember(obj.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static string GetEnumMemberStringValue(this System.Enum enumVal) => enumVal.GetAttributeOfType<EnumMemberAttribute>()?.Value;
        public static string GetJsonPropertyStringName(this object obj) => obj.GetAttributeOfType<JsonPropertyAttribute>()?.GetJsonPropertyStringName();

        public static T? GetEnumMemberEnumValue<T>(this string stringVal) where T : struct, System.Enum
        {
            var tType = typeof(T);
            var enumValues = Enum.GetValues(tType).Cast<T>();

            var enumTuple = enumValues.Select(enumVal => new
            {
                enumVal = enumVal,
                memberInfo = tType.GetMember(enumVal.ToString()).FirstOrDefault()
            })
            .Where(enumVal => {
                var customAttr = enumVal.memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false);
                var enumMemberAttr = ((EnumMemberAttribute)customAttr.FirstOrDefault())?.Value == stringVal;
                return enumMemberAttr;
            })
            .SingleOrDefault();

            if (enumTuple == null)
                return null;

            return enumTuple.enumVal;
        }
    }
}
