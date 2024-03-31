﻿//
// VisualCard  Copyright (C) 2021-2024  Aptivi
//
// This file is part of VisualCard
//
// VisualCard is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// VisualCard is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VisualCard.Parsers;

namespace VisualCard.Parts.Implementations
{
    /// <summary>
    /// Contact IMPP info
    /// </summary>
    [DebuggerDisplay("IMPP info = {ContactIMPP}")]
    public class ImppInfo : BaseCardPartInfo, IEquatable<ImppInfo>
    {
        /// <summary>
        /// The contact's IMPP information, such as SIP and XMPP
        /// </summary>
        public string ContactIMPP { get; }

        internal static BaseCardPartInfo FromStringVcardStatic(string value, string[] finalArgs, int altId, string[] elementTypes, string valueType, Version cardVersion) =>
            new ImppInfo().FromStringVcardInternal(value, finalArgs, altId, elementTypes, valueType, cardVersion);

        internal override string ToStringVcardInternal(Version cardVersion)
        {
            bool altIdSupported = cardVersion.Major >= 4;
            if (altIdSupported)
            {
                bool installAltId = AltId >= 0 && Arguments.Length > 0;
                bool installType = ElementTypes.Length > 0 && ElementTypes[0].ToUpper() != "HOME";
                return
                    $"{VcardConstants._imppSpecifier}{(installType || installAltId ? VcardConstants._fieldDelimiter : VcardConstants._argumentDelimiter)}" +
                    $"{(installAltId ? VcardConstants._altIdArgumentSpecifier + AltId + (installType ? VcardConstants._fieldDelimiter : VcardConstants._argumentDelimiter) : "")}" +
                    $"{(installType ? $"{VcardConstants._typeArgumentSpecifier}{string.Join(",", ElementTypes)}{VcardConstants._argumentDelimiter}" : "")}" +
                    $"{ContactIMPP}";
            }
            else
            {
                bool installType = ElementTypes.Length > 0 && ElementTypes[0].ToUpper() != "HOME";
                return
                    $"{VcardConstants._imppSpecifier}{(installType ? VcardConstants._fieldDelimiter : VcardConstants._argumentDelimiter)}" +
                    $"{(installType ? $"{VcardConstants._typeArgumentSpecifier}{string.Join(",", ElementTypes)}{VcardConstants._argumentDelimiter}" : "")}" +
                    $"{ContactIMPP}";
            }
        }

        internal override BaseCardPartInfo FromStringVcardInternal(string value, string[] finalArgs, int altId, string[] elementTypes, string valueType, Version cardVersion)
        {
            bool altIdSupported = cardVersion.Major >= 4;

            // Populate the fields
            string _impp = Regex.Unescape(value);
            ImppInfo _imppInstance = new(altIdSupported ? altId : 0, finalArgs, elementTypes, valueType, _impp);
            return _imppInstance;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            Equals((ImppInfo)obj);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="other">The target <see cref="ImppInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(ImppInfo other) =>
            Equals(this, other);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="source">The source <see cref="ImppInfo"/> instance to check to see if they equal</param>
        /// <param name="target">The target <see cref="ImppInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(ImppInfo source, ImppInfo target)
        {
            // We can't perform this operation on null.
            if (source is null || target is null)
                return false;

            // Check all the properties
            return
                source.ContactIMPP == target.ContactIMPP
            ;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 175591591;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContactIMPP);
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(ImppInfo left, ImppInfo right) =>
            left.Equals(right);

        /// <inheritdoc/>
        public static bool operator !=(ImppInfo left, ImppInfo right) =>
            !(left == right);

        internal override bool EqualsInternal(BaseCardPartInfo source, BaseCardPartInfo target) =>
            ((ImppInfo)source) == ((ImppInfo)target);

        internal ImppInfo() { }

        internal ImppInfo(int altId, string[] arguments, string[] elementTypes, string valueType, string contactImpp) :
            base(arguments, altId, elementTypes, valueType)
        {
            ContactIMPP = contactImpp;
        }
    }
}
