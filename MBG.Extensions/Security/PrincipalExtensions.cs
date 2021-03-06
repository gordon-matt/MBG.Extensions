﻿using System.Reflection;
using System.Security.Principal;
using MBG.Extensions.Reflection;

namespace MBG.Extensions.Security
{
    public static class PrincipalExtensions
    {
        public static string[] GetRoles(this IPrincipal principal)
        {
            MethodInfo method = principal.GetType().GetMethod("GetRoles");

            if (method != null)
            {
                // RolePrincipal (Web Forms) already has a GetRoles() method
                return (string[])method.Invoke(principal, null);
            }

            // GenericPrincipal and WindowsPrincipal have this private field
            return (string[])principal.GetPrivateFieldValue("m_roles");
        }
    }
}