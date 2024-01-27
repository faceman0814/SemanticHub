using FaceMan.SemanticHub.EnumHelper;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FaceMan.SemanticHub
{
    //
    // 摘要:
    //     类型System.Type辅助扩展方法类
    public static class TypeExtensions
    {
        //
        // 摘要:
        //     字符串类型
        public static readonly Type STRING_TYPE = typeof(string);

        //
        // 摘要:
        //     判断类型是否为Nullable类型
        //
        // 参数:
        //   type:
        //     要处理的类型
        //
        // 返回结果:
        //     是返回True，不是返回False
        public static bool IsNullableType(this Type type)
        {
            if (type != null && type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(Nullable<>);
            }

            return false;
        }

        //
        // 摘要:
        //     由类型的Nullable类型返回实际类型
        //
        // 参数:
        //   type:
        //     要处理的类型对象
        public static Type GetNonNummableType(this Type type)
        {
            if (type.IsNullableType())
            {
                return type.GetGenericArguments()[0];
            }

            return type;
        }

        //
        // 摘要:
        //     通过类型转换器获取Nullable类型的基础类型
        //
        // 参数:
        //   type:
        //     要处理的类型对象
        public static Type GetUnNullableType(this Type type)
        {
            if (type.IsNullableType())
            {
                return new NullableConverter(type).UnderlyingType;
            }

            return type;
        }

        //
        // 摘要:
        //     获取成员元数据的Description特性描述信息
        //
        // 参数:
        //   member:
        //     成员元数据对象
        //
        //   inherit:
        //     是否搜索成员的继承链以查找描述特性
        //
        // 返回结果:
        //     返回Description特性描述信息，如不存在则返回成员的名称
        public static string ToDescription(this MemberInfo member, bool inherit = false)
        {
            DescriptionAttribute attribute = member.GetAttribute<DescriptionAttribute>(inherit);
            if (attribute != null)
            {
                return attribute.Description;
            }

            return member.Name;
        }

        //
        // 摘要:
        //     检查指定指定类型成员中是否存在指定的Attribute特性
        //
        // 参数:
        //   memberInfo:
        //     要检查的类型成员
        //
        //   inherit:
        //     是否从继承中查找
        //
        // 类型参数:
        //   T:
        //     要检查的Attribute特性类型
        //
        // 返回结果:
        //     是否存在
        public static bool AttributeExists<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Any((object m) => m as T != null);
        }

        //
        // 摘要:
        //     从类型成员获取指定Attribute特性
        //
        // 参数:
        //   memberInfo:
        //     类型类型成员
        //
        //   inherit:
        //     是否从继承中查找
        //
        // 类型参数:
        //   T:
        //     Attribute特性类型
        //
        // 返回结果:
        //     存在返回第一个，不存在返回null
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).FirstOrDefault() as T;
        }

        //
        // 摘要:
        //     从类型成员获取指定Attribute特性
        //
        // 参数:
        //   memberInfo:
        //     类型类型成员
        //
        //   inherit:
        //     是否从继承中查找
        //
        // 类型参数:
        //   T:
        //     Attribute特性类型
        //
        // 返回结果:
        //     返回所有指定Attribute特性的数组
        public static T[] GetAttributes<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToArray();
        }

        //
        // 摘要:
        //     判断当前泛型类型是否可由指定类型的实例填充
        //
        // 参数:
        //   genericType:
        //     泛型类型
        //
        //   type:
        //     指定类型
        public static bool IsGenericAssignableFrom(this Type genericType, Type type)
        {
            genericType.CheckNotNull("genericType");
            type.CheckNotNull("type");
            if (!genericType.IsGenericType)
            {
                throw new ArgumentException("该功能只支持泛型类型的调用，非泛型类型可使用 IsAssignableFrom 方法。");
            }

            List<Type> list = new List<Type> { type };
            if (genericType.IsInterface)
            {
                list.AddRange(type.GetInterfaces());
            }

            foreach (Type item in list)
            {
                Type type2 = item;
                while (type2 != null)
                {
                    if (type2.IsGenericType)
                    {
                        type2 = type2.GetGenericTypeDefinition();
                    }

                    if (type2.IsSubclassOf(genericType) || type2 == genericType)
                    {
                        return true;
                    }

                    type2 = type2.BaseType;
                }
            }

            return false;
        }

        //
        // 摘要:
        //     判断类型是否实现此接口
        //
        // 参数:
        //   type:
        //     要判断的类型
        //
        // 类型参数:
        //   TInterface:
        //     接口类型
        public static bool HasInterface<TInterface>(this Type type)
        {
            return type.HasInterface(typeof(TInterface));
        }

        //
        // 摘要:
        //     判断类型是否实现此接口
        //
        // 参数:
        //   type:
        //     类型
        //
        //   interfaceType:
        //     接口类型
        public static bool HasInterface(this Type type, Type interfaceType)
        {
            Type @interface = type.GetInterface(interfaceType.FullName);
            if (@interface == null)
            {
                return false;
            }

            return @interface.FullName == interfaceType.FullName;
        }

        //
        // 摘要:
        //     判断是否为基本类型
        //
        // 参数:
        //   type:
        //
        //   skipEnum:
        //     跳过枚举
        public static bool IsStructs(this Type type, bool skipEnum = false)
        {
            if (skipEnum)
            {
                if (type.IsValueType || type == STRING_TYPE)
                {
                    return !type.IsEnum;
                }

                return false;
            }

            if (!type.IsValueType)
            {
                return type == STRING_TYPE;
            }

            return true;
        }

        //
        // 摘要:
        //     是否为泛型迭代器
        //
        // 参数:
        //   type:
        //
        //   genericType:
        //     迭代器中的泛型类型
        public static bool IsEnumerable(this Type type, out Type genericType)
        {
            genericType = null;
            if (type.IsGenericType)
            {
                genericType = type.GenericTypeArguments[0];
                return typeof(IEnumerable<>).MakeGenericType(type.GenericTypeArguments).IsAssignableFrom(type);
            }

            return false;
        }

        //
        // 摘要:
        //     是否为泛型集合
        //
        // 参数:
        //   type:
        public static bool IsEnumerable(this Type type)
        {
            Type genericType;
            return type.IsEnumerable(out genericType);
        }

        //
        // 摘要:
        //     判断类型是否为集合类型
        //
        // 参数:
        //   type:
        //     要处理的类型
        //
        // 返回结果:
        //     是返回True，不是返回False
        public static bool IsEnumerable2(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }

            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        //
        // 摘要:
        //     获取类型的名称，如果是泛型，则取第一个泛型参数的类型名称
        //
        // 参数:
        //   type:
        public static string GetTypeFullName(this Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericArguments()[0].FullName;
            }

            if (type.IsNullable(out var nullableType))
            {
                return nullableType.FullName;
            }

            return type.FullName;
        }

        public static bool IsNullable(this Type type, out Type nullableType)
        {
            nullableType = Nullable.GetUnderlyingType(type);
            return nullableType != null;
        }
    }
}
