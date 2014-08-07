using System;
using System.Collections.Generic;

namespace ActionMailerNext.Standalone
{
    /// <summary>
    ///     Exception thrown when a template could not be resolved
    /// </summary>
    public class TemplateResolvingException : Exception
    {
        /// <summary>
        ///     A list of paths that were checked when searching for a template.
        /// </summary>
        public List<string> SearchPaths { get; set; }
    }
}