using LearnArchitecture.Core.Helper.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static LearnArchitecture.Core.Helper.Enums.Enums;

namespace LearnArchitecture.Core.Entities
{
    public class CommonField
    {
        public bool isActive {get; set;}
        public bool isDelete { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? updatedOn { get; set; }
        public int? updatedBy { get; set; }
    }


    public static class GenericCommonField
    {
        public static T UpdateCommonField<T>(T data, EnumOperationType operationType, AuthClaim authClaim)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                switch (operationType)
                {
                    case EnumOperationType.Add:
                        SetAddFields(prop, data, authClaim);
                        break;

                    case EnumOperationType.Update:
                        SetUpdateFields(prop, data, authClaim);
                        break;

                    case EnumOperationType.Delete:
                        SetDeleteFields(prop, data, authClaim);
                        break;
                }

            }

            return data;
        }

        private static void SetAddFields<T>(PropertyInfo prop, T data, AuthClaim authClaim)
        {
            if (prop.Name == nameof(CommonField.isActive) && prop.PropertyType == typeof(bool))
                prop.SetValue(data, true);

            else if (prop.Name == nameof(CommonField.isDelete) && prop.PropertyType == typeof(bool))
                prop.SetValue(data, false);

            else if (prop.Name == nameof(CommonField.createdOn) && prop.PropertyType == typeof(DateTime))
                prop.SetValue(data, DateTime.Now);

            else if (prop.Name == nameof(CommonField.createdBy) && prop.PropertyType == typeof(int))
                prop.SetValue(data, authClaim.userId);
        }

        private static void SetUpdateFields<T>(PropertyInfo prop, T data, AuthClaim authClaim)
        {
            if (prop.Name == nameof(CommonField.isDelete) && prop.PropertyType == typeof(bool))
                prop.SetValue(data, false);

            else if (prop.Name == nameof(CommonField.updatedOn) && prop.PropertyType == typeof(DateTime?))
                prop.SetValue(data, DateTime.Now);

            else if (prop.Name == nameof(CommonField.updatedBy) && prop.PropertyType == typeof(int?))
                prop.SetValue(data, authClaim.userId);
        }

        private static void SetDeleteFields<T>(PropertyInfo prop, T data, AuthClaim authClaim)
        {
            if (prop.Name == nameof(CommonField.isActive) && prop.PropertyType == typeof(bool))
                prop.SetValue(data, false);

            else if (prop.Name == nameof(CommonField.isDelete) && prop.PropertyType == typeof(bool))
                prop.SetValue(data, true);

            else if (prop.Name == nameof(CommonField.updatedOn) && prop.PropertyType == typeof(DateTime?))
                prop.SetValue(data, DateTime.Now);

            else if (prop.Name == nameof(CommonField.updatedBy) && prop.PropertyType == typeof(int?))
                prop.SetValue(data, authClaim.userId);
        }
    }
}
