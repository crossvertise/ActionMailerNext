﻿namespace ActionMailerNext.Standalone.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using ActionMailerNext.Standalone.Interfaces;
    using ActionMailerNext.Standalone.Models;

    public class HandlebarsFilesTemplateResolver : ITemplateResolver
    {
        private readonly string _viewPath;

        /// <summary>
        /// Creates a template resolver using the specified path.  If no path is given, this defaults to "Views".
        /// </summary>
        /// <param name="viewPath">The path containing your views</param>
        public HandlebarsFilesTemplateResolver(string viewPath)
        {
            _viewPath = viewPath;
        }

        public string Resolve(string name, string externalViewPath = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            string csViewName = name;

            if (!csViewName.EndsWith(".hbs"))
            {
                csViewName += ".hbs";
            }

            var appRoot = AppDomain.CurrentDomain.BaseDirectory;

            var viewPath = string.IsNullOrEmpty(externalViewPath) ? _viewPath : externalViewPath;

            var csViewPath = (csViewName).StartsWith("~")
                ? Path.GetFullPath(Path.Combine(appRoot, csViewName.Substring(2)))
                : Path.GetFullPath(Path.Combine(appRoot, viewPath, csViewName));

            // Works with forward and backward slashes in the path
            if (File.Exists(csViewPath))
            {
                return File.ReadAllText(csViewPath, Encoding.UTF8);
            }
            throw new TemplateResolvingException { SearchPaths = new List<string> { csViewPath } };
        }

        public List<MailTemplate> GetAllPartialTemplates() => GetAllTemplates().Where(t => t.IsPartial).ToList();

        private IEnumerable<MailTemplate> GetAllTemplates() => SearchTemplates("*");

        private List<MailTemplate> SearchTemplates(string searchPattern)
        {
            searchPattern = $"*{searchPattern}*.hbs";
            var templatesDir = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName, _viewPath);
            var filePaths = Directory.EnumerateFiles(templatesDir, searchPattern, SearchOption.AllDirectories);
            var templates = from path in filePaths
                            let name = Path.GetFileNameWithoutExtension(path)
                            let nameParts = name.Split('-')
                            select new MailTemplate(
                                key: path.Replace(templatesDir + "\\", "").Replace(".hbs", ""),
                                value: File.ReadAllText(path, Encoding.UTF8),
                                label: nameParts.Length > 1 ? nameParts[1] : null,
                                isPartial: nameParts[0].StartsWith("_")
                            );
            return templates.ToList();
        }
    }
}
