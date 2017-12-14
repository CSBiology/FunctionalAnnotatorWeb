#region license
// The MIT License (MIT)

// ReflectionHelper.cs

// Copyright (c) 2016 Alexander Lüdemann
// alexander.luedemann@outlook.com
// luedeman@rhrk.uni-kl.de

// Computational Systems Biology, Technical University of Kaiserslautern, Germany
 

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FaTool.Web.Services.OData
{
    internal static class ReflectionHelper
    {

        public static string GetPropertyName<T, TProperty>(
            Expression<Func<T, TProperty>> property)
        {            
            return GetPropertyInfo(property).Name;
        }

        public static PropertyInfo GetPropertyInfo<T, TProperty>(
            Expression<Func<T, TProperty>> property)
        {
            MemberExpression member = property.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression body '{0}' is not a member expression.",
                    property.Body.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    property.ToString()));
            return propInfo;
        }

        public static IEnumerable<MethodInfo> GetDeclaredMethods<TTarget, TAttribute>() 
            where TAttribute : Attribute
        {
            return typeof(TTarget)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(x => HasAttribute<TAttribute>(x));
        }

        public static IEnumerable<PropertyInfo> GetDeclaredProperties<TAttribute>(Type targetType)
            where TAttribute : Attribute
        {
            return targetType
                .GetProperties(BindingFlags.Instance | 
                    BindingFlags.Public | 
                    BindingFlags.DeclaredOnly | 
                    BindingFlags.GetProperty | 
                    BindingFlags.SetProperty)
                .Where(x => HasAttribute<TAttribute>(x));
        }

        public static bool HasAttribute<TAttribute>(MemberInfo mi, bool inherit = false) 
            where TAttribute : Attribute
        {
            return mi.GetCustomAttributes(typeof(TAttribute), inherit).Length > 0;
        }

        public static bool IsGenericQueryableType(Type clrType)
        {
            return typeof(IQueryable).IsAssignableFrom(clrType) && 
                clrType.IsGenericType && 
                clrType.GenericTypeArguments.Length == 1;
        }

        public static bool IsGenericEnumerableType(Type clrType)
        {
            return typeof(IEnumerable).IsAssignableFrom(clrType) &&
                clrType.IsGenericType &&
                clrType.GenericTypeArguments.Length == 1;
        }

        public static object Invoke(object target, string name, Type[] typeArgs,  params object[] pars) 
        {            
            Type[] paramTypes;

            if (pars == null)
            {
                paramTypes = new Type[0];
            }
            else
            {
                paramTypes = pars.Select(x => x.GetType()).ToArray();
            }

            MethodInfo mi = target.GetType().GetMethod(
                name,
                BindingFlags.Instance | BindingFlags.Public,
                null,
                paramTypes,
                null);

            if(typeArgs != null && typeArgs.Length > 0)
                mi = mi.MakeGenericMethod(typeArgs);

            return mi.Invoke(target, pars);
        }

        public static object GetProperty(object target, string propertyName)
        {
            PropertyInfo pi = target.GetType().GetProperty(
                propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return pi.GetValue(target);
        }

        public static void SetProperty(object target, string propertyName, object value)
        {
            PropertyInfo pi = target.GetType().GetProperty(
                propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            pi.SetValue(target, value);
        }
    }
}