namespace FaceMan.SemanticHub.Helper.EnumHelper
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumNameAttribute : Attribute
    {
        public static readonly EnumNameAttribute Default = new EnumNameAttribute();

        public virtual string NameValue => EnumNameValue;

        protected string EnumNameValue { get; set; }

        public EnumNameAttribute()
            : this(string.Empty)
        {
        }

        public EnumNameAttribute(string enumName)
        {
            EnumNameValue = enumName;
        }

        public override bool Equals(object obj)
        {
            EnumNameAttribute enumNameAttribute = obj as EnumNameAttribute;
            if (enumNameAttribute != null)
            {
                return enumNameAttribute.EnumNameValue == NameValue;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return NameValue?.GetHashCode() ?? 0;
        }

        public override bool IsDefaultAttribute()
        {
            return Equals(Default);
        }
    }
}
