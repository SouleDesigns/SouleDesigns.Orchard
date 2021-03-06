﻿using System.Collections.Generic;
using Contrib.Widgets.Models;
using Orchard;
using Orchard.Widgets.Models;

namespace Contrib.Widgets.Services {
    public interface IWidgetManager : IDependency {
        IEnumerable<WidgetExPart> GetWidgets(int hostId);
        LayerPart GetContentLayer();
    }
}