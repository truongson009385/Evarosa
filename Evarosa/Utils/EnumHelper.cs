using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Evarosa.Utils
{
    public static class EnumHelper
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            try
            {
                string displayName;
                displayName = enumValue.GetType()
                    .GetMember(enumValue.ToString())
                    .FirstOrDefault()
                    .GetCustomAttribute<DisplayAttribute>()?
                    .GetName();

                if (String.IsNullOrEmpty(displayName))
                {
                    displayName = enumValue.ToString();
                }

                return displayName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static int GetValue(this Enum enumValue)
        {
            try
            {
                return Convert.ToInt32(enumValue);
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                return -1; // Or some default error value
            }
        }
    }
}
