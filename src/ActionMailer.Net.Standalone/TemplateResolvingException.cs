using System;
using System.Collections.Generic;

namespace ActionMailer.Net.Standalone {
    public class TemplateResolvingException : Exception {
        public List<string> SearchPaths { get; set; }
    }
}
