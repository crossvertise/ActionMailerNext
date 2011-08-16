using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ActionMailer.Net.Standalone;

namespace StandaloneSample.Mailers {
    public class SampleMailer : RazorMailerBase {
        private readonly string _viewPath;
        public override string ViewPath {
            get { return _viewPath; }
        }

        public SampleMailer() {
            var workingDirectory = Assembly.GetExecutingAssembly().Location;
            _viewPath = Path.Combine(workingDirectory, "..", "..", "Templates");
        }

        public RazorEmailResult EmailWithNoModel() {
            
        }

        
    }
}
