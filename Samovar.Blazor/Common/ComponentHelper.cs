//using Microsoft.AspNetCore.Components;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Samovar.Blazor
//{
//    internal static class ComponentHelper
//    {
//		private static ResolveDelegate CreateResolver(Type t)
//		{
//			PropertyInfo[] array = FindAllInjectableProperties(t);
//			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "service");
//			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(IComponentServiceProvider), "sp");
//			ParameterExpression parameterExpression3 = Expression.Variable(t, "result");
//			List<Expression> list = new List<Expression>();
//			list.Add(parameterExpression3);
//			list.Add(Expression.Assign(parameterExpression3, Expression.Convert(parameterExpression, t)));
//			PropertyInfo[] array2 = array;
//			foreach (PropertyInfo propertyInfo in array2)
//			{
//				MemberExpression left = Expression.Property(parameterExpression3, propertyInfo);
//				MethodCallExpression expression = Expression.Call(parameterExpression2, "GetService", new Type[1] { propertyInfo.PropertyType });
//				list.Add(Expression.Assign(left, Expression.Convert(expression, propertyInfo.PropertyType)));
//			}
//			List<ParameterExpression> parameters = new List<ParameterExpression> { parameterExpression, parameterExpression2 };
//			BlockExpression body = Expression.Block(new ParameterExpression[1] { parameterExpression3 }, list);
//			Expression<ResolveDelegate> expression2 = Expression.Lambda<ResolveDelegate>(body, parameters);
//			return expression2.Compile();
//		}

//		private static PropertyInfo[] FindAllInjectableProperties(Type t)
//		{
//			return FindIn(t);
//			static PropertyInfo[] FindIn(Type t)
//			{
//				if (!(t == null))
//				{
//					return GetCachedItem(t, CachedPropertyInfoLookups, FindInExtended, t.BaseType);
//				}
//				return Array.Empty<PropertyInfo>();
//			}
//			static PropertyInfo[] FindInExtended(Type t, Type baseType)
//			{
//				return FindIn(baseType).Concat(FindProperties(t)).ToArray();
//			}
//			static IEnumerable<PropertyInfo> FindProperties(Type t)
//			{
//				PropertyInfo[] propertiesToSet = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//				foreach (PropertyInfo propertyInfo in propertiesToSet)
//				{
//					if (propertyInfo.CanWrite && CanBeInjected(propertyInfo))
//					{
//						yield return propertyInfo;
//					}
//				}
//			}
//		}
//		private static bool CanBeInjected(PropertyInfo x)
//		{
//			if (!(x.DeclaringType == x.ReflectedType) || x.GetCustomAttribute<SmInjectAttribute>(inherit: true) == null)
//			{
//				if (_globalServices.Contains<Type>(x.PropertyType))
//				{
//					return x.GetCustomAttribute<InjectAttribute>(inherit: true) != null;
//				}
//				return false;
//			}
//			return true;
//		}
//	}
//}
