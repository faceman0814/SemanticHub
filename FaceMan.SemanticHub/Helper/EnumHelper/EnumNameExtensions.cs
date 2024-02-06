using System.Reflection;

namespace FaceMan.SemanticHub.Helper.EnumHelper
{
    public static class EnumNameExtensions
    {
        public static string ToNameValue(this Enum value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            MemberInfo memberInfo = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                return memberInfo.ToNameValue();
            }

            return value.ToString();
        }

        public static string ToNameValue(this MemberInfo member, bool inherit = false)
        {
            EnumNameAttribute attribute = member.GetAttribute<EnumNameAttribute>(inherit);
            if (attribute == null)
            {
                return member.Name;
            }

            return attribute.NameValue;
        }
    }
}
