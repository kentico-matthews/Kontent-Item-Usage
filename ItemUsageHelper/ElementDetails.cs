using System;
using System.Collections.Generic;
using System.Text;

namespace ItemUsage
{
    public class ElementDetails
    {
        /// <summary>
        /// Codename of the content element
        /// </summary>
        public string Codename { get; set; }

        /// <summary>
        /// Display name (System.Name) of the content element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Codename of the content type that the element belongs to
        /// </summary>
        public string ContentTypeCodename { get; set; }

        /// <summary>
        /// Display name (System.Name) of the content type the the element belongs to
        /// </summary>
        public string ContentTypeName { get; set; }
    }
}
