﻿using Orchard.Models.Driver;

namespace Orchard.Tests.Models.Stubs {
    public class AlphaDriver : ModelDriverBase {
        protected override void New(NewModelContext context) {
            if (context.ModelType == "alpha") {
                WeldModelPart<Alpha>(context);
            }
        }
    }
}