using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account.Manage {
    public static class ManageNavPages {
        public static string Index => "Index";

        public static string Email => "Email";

        public static string LinkSummoner => "LinkSummoner";

        public static string ChangePassword => "ChangePassword";

        public static string ManageProfileCards => "ManageProfileCards";

        public static string IndexNavClass(ViewContext viewContext) =>
            PageNavClass(viewContext, Index);

        public static string EmailNavClass(ViewContext viewContext) =>
            PageNavClass(viewContext, Email);

        public static string ChangePasswordNavClass(ViewContext viewContext) =>
            PageNavClass(viewContext, ChangePassword);

        public static string SummonerLinkNavClass(ViewContext viewContext) =>
            PageNavClass(viewContext, LinkSummoner);

        public static string ManageProfileCardsNavClass(ViewContext viewContext) =>
            PageNavClass(viewContext, ManageProfileCards);

        private static string PageNavClass(ViewContext viewContext, string page) {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}