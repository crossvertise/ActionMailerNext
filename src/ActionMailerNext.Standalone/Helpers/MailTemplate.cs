﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActionMailerNext.Standalone.Helpers
{
    public class MailTemplate
    {
        public MailTemplate()
        {
        }

        public MailTemplate(string key, string value, string label, bool isPartial)
        {
            Key = key;
            Label = label;
            Value = value;
            IsPartial = isPartial;
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Label { get; set; }

        public bool IsPartial { get; set; }
    }
}
