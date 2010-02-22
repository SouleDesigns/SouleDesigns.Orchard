﻿using Orchard.UI.Navigation;

namespace Orchard.Blogs {
    public class AdminMenu : INavigationProvider {
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add("Blogs", "2",
                        menu => menu
                                    .Add("Manage Blogs", "1.0", item => item.Action("List", "BlogAdmin", new { area = "Orchard.Blogs" }).Permission(Permissions.MetaListBlogs))
                                    .Add("Add New Blog", "1.1", item => item.Action("Create", "BlogAdmin", new { area = "Orchard.Blogs" }).Permission(Permissions.ManageBlogs)));
        }
    }
}