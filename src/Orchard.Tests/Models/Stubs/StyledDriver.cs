﻿using Orchard.Models.Driver;

namespace Orchard.Tests.Models.Stubs {
    public class StyledDriver : ModelDriverBase {
        protected override void New(NewModelContext context) {
            if (context.ModelType == "alpha") {
                WeldModelPart<Styled>(context);
            }
        }
    }
}