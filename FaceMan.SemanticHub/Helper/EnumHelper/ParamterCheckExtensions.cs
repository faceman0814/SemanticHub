namespace FaceMan.SemanticHub.Helper.EnumHelper
{
    //
    // 摘要:
    //     用于参数检查的扩展方法
    public static class ParamterCheckExtensions
    {
        //
        // 摘要:
        //     验证指定值的断言assertion是否为真，如果不为真，抛出指定消息message的指定类型 TException异常
        //
        // 参数:
        //   assertion:
        //     要验证的断言。
        //
        //   message:
        //     异常消息。
        //
        // 类型参数:
        //   TException:
        //     异常类型
        private static void Require<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion)
            {
                return;
            }

            throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        //
        // 摘要:
        //     验证指定值的断言表达式是否为真，不为值抛出System.Exception异常
        //
        // 参数:
        //   value:
        //
        //   assertionFunc:
        //     要验证的断言表达式
        //
        //   message:
        //     异常消息
        public static void Required<T>(this T value, Func<T, bool> assertionFunc, string message)
        {
            Require<Exception>(assertionFunc(value), message);
        }

        //
        // 摘要:
        //     验证指定值的断言表达式是否为真，不为真抛出 异常
        //
        // 参数:
        //   value:
        //     要判断的值
        //
        //   assertionFunc:
        //     要验证的断言表达式
        //
        //   message:
        //     异常消息
        //
        // 类型参数:
        //   T:
        //     要判断的值的类型
        public static void Required<T, TException>(this T value, Func<T, bool> assertionFunc, string message) where TException : Exception
        {
            Require<TException>(assertionFunc(value), message);
        }

        //
        // 摘要:
        //     检查参数不能为空引用，否则抛出System.ArgumentNullException异常。
        //
        // 参数:
        //   value:
        //
        //   paramName:
        //     参数名称
        //
        // 异常:
        //   T:System.ArgumentNullException:
        public static void CheckNotNull<T>(this T value, string paramName) where T : class
        {
            Require<ArgumentNullException>(value != null, "参数“" + paramName + "”不能为空引用。");
        }

        //
        // 摘要:
        //     检查字符串不能为空引用或空字符串，否则抛出System.ArgumentNullException异常或System.ArgumentException异常。
        //
        // 参数:
        //   value:
        //
        //   paramName:
        //     参数名称。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //
        //   T:System.ArgumentException:
        public static void CheckNotNullOrEmpty(this string value, string paramName)
        {
            value.CheckNotNull(paramName);
            Require<ArgumentException>(value.Length > 0, "参数“" + paramName + "”不能为空引用或空字符串。");
        }

        //
        // 摘要:
        //     检查Guid值不能为Guid.Empty，否则抛出System.ArgumentException异常。
        //
        // 参数:
        //   value:
        //
        //   paramName:
        //     参数名称。
        //
        // 异常:
        //   T:System.ArgumentException:
        public static void CheckNotEmpty(this Guid value, string paramName)
        {
            Require<ArgumentException>(value != Guid.Empty, "参数“" + paramName + "”的值不能为Guid.Empty");
        }

        //
        // 摘要:
        //     检查集合不能为空引用或空集合，否则抛出System.ArgumentNullException异常或System.ArgumentException异常。
        //
        // 参数:
        //   collection:
        //
        //   paramName:
        //     参数名称。
        //
        // 类型参数:
        //   T:
        //     集合项的类型。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //
        //   T:System.ArgumentException:
        public static void CheckNotNullOrEmpty<T>(this IEnumerable<T> collection, string paramName)
        {
            collection.CheckNotNull(paramName);
            Require<ArgumentException>(collection.Any(), "参数“" + paramName + "”不能为空引用或空集合。");
        }

        //
        // 摘要:
        //     检查参数必须小于[或可等于，参数canEqual]指定值，否则抛出System.ArgumentOutOfRangeException异常。
        //
        // 参数:
        //   value:
        //
        //   paramName:
        //     参数名称。
        //
        //   target:
        //     要比较的值。
        //
        //   canEqual:
        //     是否可等于。
        //
        // 类型参数:
        //   T:
        //     参数类型。
        //
        // 异常:
        //   T:System.ArgumentOutOfRangeException:
        public static void CheckLessThan<T>(this T value, string paramName, T target, bool canEqual = false) where T : IComparable<T>
        {
            bool assertion = canEqual ? value.CompareTo(target) <= 0 : value.CompareTo(target) < 0;
            string format = canEqual ? "参数“{0}”的值必须小于或等于“{1}”。" : "参数“{0}”的值必须小于“{1}”。";
            Require<ArgumentOutOfRangeException>(assertion, string.Format(format, paramName, target));
        }

        //
        // 摘要:
        //     检查参数必须大于[或可等于，参数canEqual]指定值，否则抛出System.ArgumentOutOfRangeException异常。
        //
        // 参数:
        //   value:
        //
        //   paramName:
        //     参数名称。
        //
        //   target:
        //     要比较的值。
        //
        //   canEqual:
        //     是否可等于。
        //
        // 类型参数:
        //   T:
        //     参数类型。
        //
        // 异常:
        //   T:System.ArgumentOutOfRangeException:
        public static void CheckGreaterThan<T>(this T value, string paramName, T target, bool canEqual = false) where T : IComparable<T>
        {
            bool assertion = canEqual ? value.CompareTo(target) >= 0 : value.CompareTo(target) > 0;
            string format = canEqual ? "参数“{0}”的值必须大于或等于“{1}”。" : "参数“{0}”的值必须大于“{1}”。";
            Require<ArgumentOutOfRangeException>(assertion, string.Format(format, paramName, target));
        }

        //
        // 摘要:
        //     检查参数必须在指定范围之间，否则抛出System.ArgumentOutOfRangeException异常。
        //
        // 参数:
        //   value:
        //
        //   paramName:
        //     参数名称。
        //
        //   start:
        //     比较范围的起始值。
        //
        //   end:
        //     比较范围的结束值。
        //
        //   startEqual:
        //     是否可等于起始值
        //
        //   endEqual:
        //     是否可等于结束值
        //
        // 类型参数:
        //   T:
        //     参数类型。
        //
        // 异常:
        //   T:System.ArgumentOutOfRangeException:
        public static void CheckBetween<T>(this T value, string paramName, T start, T end, bool startEqual = false, bool endEqual = false) where T : IComparable<T>
        {
            bool assertion = startEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            string message = startEqual ? $"参数“{paramName}”的值必须在“{start}”与“{end}”之间，且不能等于“{start}”。" : $"参数“{paramName}”的值必须在“{start}”与“{end}”之间。";
            Require<ArgumentOutOfRangeException>(assertion, message);
            assertion = endEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0;
            message = endEqual ? $"参数“{paramName}”的值必须在“{start}”与“{end}”之间，且不能等于“{end}”。" : $"参数“{paramName}”的值必须在“{start}”与“{end}”之间。";
            Require<ArgumentOutOfRangeException>(assertion, message);
        }

        //
        // 摘要:
        //     检查指定路径的文件夹必须存在，否则抛出System.IO.DirectoryNotFoundException异常。
        //
        // 参数:
        //   directory:
        //
        //   paramName:
        //     参数名称。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //
        //   T:System.IO.DirectoryNotFoundException:
        public static void CheckDirectoryExists(this string directory, string paramName = null)
        {
            directory.CheckNotNull(paramName);
            Require<DirectoryNotFoundException>(Directory.Exists(directory), "指定的目录路径“" + directory + "”不存在。");
        }

        //
        // 摘要:
        //     检查指定路径的文件必须存在，否则抛出System.IO.FileNotFoundException异常。
        //
        // 参数:
        //   filename:
        //
        //   paramName:
        //     参数名称。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     当文件路径为null时
        //
        //   T:System.IO.FileNotFoundException:
        //     当文件路径不存在时
        public static void CheckFileExists(this string filename, string paramName = null)
        {
            filename.CheckNotNull(paramName);
            Require<FileNotFoundException>(File.Exists(filename), "指定的文件路径“" + filename + "”不存在。");
        }
    }
}
