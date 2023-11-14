﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace  QRCodeAttendance.Views.Shared
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static class ManageSidebar
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static string Home => "Home";
        public static string Attendance => "Attendance";
        public static string Student => "Student";
        public static string About => "About";
        public static string Dashboard => "Dashboard";
        public static string CameraSurveillance => "CameraSurveillance";

        public static string HomeNavClass(ViewContext viewContext) => SidebarNavClass(viewContext, Home);
        public static string AttendanceNavClass(ViewContext viewContext) => SidebarNavClass(viewContext, Attendance);
        public static string StudentNavClass(ViewContext viewContext) => SidebarNavClass(viewContext, Student);
        public static string AboutNavClass(ViewContext viewContext) => SidebarNavClass(viewContext, About);
        public static string DashboardNavClass(ViewContext viewContext) => SidebarNavClass(viewContext, Dashboard);
        public static string CameraNavClass(ViewContext viewContext) => SidebarNavClass(viewContext, CameraSurveillance);

        public static string SidebarNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
