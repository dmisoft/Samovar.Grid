//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Samovar.Blazor
//{
//    public class ServiceRegister
//    {
//		protected static ReadOnlyMemory<ServiceDescriptor> ValueFactory<T>() where T : ComponentServiceCollection, new()
//		{
//			T val = new T();
//			return val.BuildComponentRootScope();
//		}

//		public ReadOnlyMemory<ServiceDescriptor> BuildComponentRootScope()
//		{
//			_serviceCollection = new ServiceCollection();
//			MethodInfo[] methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
//			MethodInfo[] array = methods;
//			foreach (MethodInfo methodInfo in array)
//			{
//				ParameterInfo[] parameters = methodInfo.GetParameters();
//				if (!parameters.Any())
//				{
//					if (methodInfo.ContainsGenericParameters)
//					{
//						_unwrapGenerics = true;
//						methodInfo.MakeGenericMethod(typeof(bool)).Invoke(this, _emptyArgs);
//						_unwrapGenerics = false;
//					}
//					else
//					{
//						methodInfo.Invoke(this, _emptyArgs);
//					}
//				}
//				else
//				{
//					ProcessRegisterMethodWithParameters(parameters, methodInfo);
//				}
//			}
//			return new ReadOnlyMemory<ServiceDescriptor>(_serviceCollection.ToArray());
//		}
//	}
//}
