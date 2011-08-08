#region License
/* Copyright (C) 2011 by Scott W. Anderson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ActionMailer.Net.Mvc {
    /// <summary>
    /// This class contains some handy extension methods that make it easier
    /// to consume inline attachments in your message body.
    /// </summary>
    public static class HtmlHelperExtensions {
        /// <summary>
        /// Creates an image tag linked against the specified inline image attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <returns>An HTML image tag linked against the specified inline image attachment.</returns>
        public static IHtmlString InlineImage(this HtmlHelper helper, string contentId) {
            return helper.InlineImage(contentId, null, null, null);
        }

        /// <summary>
        /// Creates an image tag linked against the specified inline image attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <param name="htmlAttributes">An anonymous object with extra html attributes you wish to add to this tag.</param>
        /// <returns>An HTML image tag linked against the specified inline image attachment.</returns>
        public static IHtmlString InlineImage(this HtmlHelper helper, string contentId, object htmlAttributes) {
            return helper.InlineImage(contentId, null, null, htmlAttributes);
        }

        /// <summary>
        /// Creates an image tag linked against the specified inline image attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <param name="alt">The tooltip text to display when hovering over the image</param>
        /// <returns>An HTML image tag linked against the specified inline image attachment.</returns>
        public static IHtmlString InlineImage(this HtmlHelper helper, string contentId, string alt) {
            return helper.InlineImage(contentId, alt, null, null);
        }

        /// <summary>
        /// Creates an image tag linked against the specified inline image attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <param name="alt">The tooltip text to display when hovering over the image</param>
        /// <param name="htmlAttributes">An anonymous object with extra html attributes you wish to add to this tag.</param>
        /// <returns>An HTML image tag linked against the specified inline image attachment.</returns>
        public static IHtmlString InlineImage(this HtmlHelper helper, string contentId, string alt, object htmlAttributes) {
            return helper.InlineImage(contentId, alt, null, htmlAttributes);
        }

        /// <summary>
        /// Creates an image tag linked against the specified inline image attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <param name="alt">The tooltip text to display when hovering over the image</param>
        /// <param name="id">Any HTML ID that you want the image tag to have.</param>
        /// <returns>An HTML image tag linked against the specified inline image attachment.</returns>
        public static IHtmlString InlineImage(this HtmlHelper helper, string contentId, string alt, string id) {
            return helper.InlineImage(contentId, alt, id, null);
        }

        /// <summary>
        /// Creates an image tag linked against the specified inline image attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <param name="alt">The tooltip text to display when hovering over the image</param>
        /// <param name="id">Any HTML ID that you want the image tag to have.</param>
        /// <param name="htmlAttributes">An anonymous object with extra html attributes you wish to add to this tag.</param>
        /// <returns>An HTML image tag linked against the specified inline image attachment.</returns>
        public static IHtmlString InlineImage(this HtmlHelper helper, string contentId, string alt, string id, object htmlAttributes) {
            if (contentId == null)
                throw new ArgumentNullException("contentId");

            var builder = new TagBuilder("img");
            var fileUrl = String.Format("cid:{0}", contentId);

            builder.MergeAttribute("src", fileUrl);

            if (!string.IsNullOrWhiteSpace(alt))
                builder.MergeAttribute("alt", alt);

            if (!string.IsNullOrWhiteSpace(id))
                builder.GenerateId(id);

            if (htmlAttributes != null)
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// Creates a link tag for an inline CSS attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <returns>An HTML link tag that reprensents the inline CSS attachment specified.</returns>
        public static IHtmlString InlineCSS(this HtmlHelper helper, string contentId) {
            return helper.InlineCSS(contentId, null, null);
        }

        /// <summary>
        /// Creates a link tag for an inline CSS attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <param name="htmlAttributes">An anonymous object with extra html attributes you wish to add to this tag.</param>
        /// <returns>An HTML link tag that reprensents the inline CSS attachment specified.</returns>
        public static IHtmlString InlineCSS(this HtmlHelper helper, string contentId, object htmlAttributes) {
            return helper.InlineCSS(contentId, null, htmlAttributes);
        }

        /// <summary>
        /// Creates a link tag for an inline CSS attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <param name="media">The media type for the link tag.</param>
        /// <returns>An HTML link tag that reprensents the inline CSS attachment specified.</returns>
        public static IHtmlString InlineCSS(this HtmlHelper helper, string contentId, string media) {
            return helper.InlineCSS(contentId, media, null);
        }

        /// <summary>
        /// Creates a link tag for an inline CSS attachment.
        /// </summary>
        /// <param name="helper">Your html helper instance.</param>
        /// <param name="contentId">The content id (in ActionMailer this is the attachment name).</param>
        /// <param name="media">The media type for the link tag.</param>
        /// <param name="htmlAttributes">An anonymous object with extra html attributes you wish to add to this tag.</param>
        /// <returns>An HTML link tag that reprensents the inline CSS attachment specified.</returns>
        public static IHtmlString InlineCSS(this HtmlHelper helper, string contentId, string media, object htmlAttributes) {
            if (contentId == null)
                throw new ArgumentNullException("contentId");

            var builder = new TagBuilder("link");
            var fileUrl = String.Format("cid:{0}", contentId);

            builder.MergeAttribute("href", fileUrl);
            builder.MergeAttribute("rel", "stylesheet");
            builder.MergeAttribute("type", "text/css");

            if (!string.IsNullOrWhiteSpace(media))
                builder.MergeAttribute("media", media);

            if (htmlAttributes != null)
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}
