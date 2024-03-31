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
using System.Text.RegularExpressions;
using VisualCard.Parsers;

namespace VisualCard.Parts.Implementations
{
    /// <summary>
    /// Contact organization info
    /// </summary>
    [DebuggerDisplay("OrgName = {Name}, {Unit}, {Role}")]
    public class OrganizationInfo : BaseCardPartInfo, IEquatable<OrganizationInfo>
    {
        /// <summary>
        /// The contact's organization name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The contact's organization unit
        /// </summary>
        public string Unit { get; }
        /// <summary>
        /// The contact's organization unit's role
        /// </summary>
        public string Role { get; }

        internal static BaseCardPartInfo FromStringVcardStatic(string value, string[] finalArgs, int altId, string[] elementTypes, string valueType, Version cardVersion) =>
            new OrganizationInfo().FromStringVcardInternal(value, finalArgs, altId, elementTypes, valueType, cardVersion);

        internal override string ToStringVcardInternal(Version cardVersion)
        {
            bool altIdSupported = cardVersion.Major >= 4;
            if (altIdSupported)
            {
                bool installAltId = AltId >= 0 && Arguments.Length > 0;
                bool installType = (installAltId || ElementTypes.Length > 0) && ElementTypes[0].ToUpper() != "WORK";
                return
                    $"{VcardConstants._orgSpecifier}{(installType || installAltId ? VcardConstants._fieldDelimiter : VcardConstants._argumentDelimiter)}" +
                    $"{(installAltId ? VcardConstants._altIdArgumentSpecifier + AltId + (installType ? VcardConstants._fieldDelimiter : VcardConstants._argumentDelimiter) : "")}" +
                    $"{(installType ? $"{VcardConstants._typeArgumentSpecifier}{string.Join(",", ElementTypes)}{VcardConstants._argumentDelimiter}" : "")}" +
                    $"{Name}{VcardConstants._fieldDelimiter}" +
                    $"{Unit}{VcardConstants._fieldDelimiter}" +
                    $"{Role}";
            }
            else
            {
                bool installType = ElementTypes.Length > 0 && ElementTypes[0].ToUpper() != "WORK";
                return
                    $"{VcardConstants._orgSpecifier}{(installType ? VcardConstants._fieldDelimiter : VcardConstants._argumentDelimiter)}" +
                    $"{(installType ? $"{VcardConstants._typeArgumentSpecifier}{string.Join(",", ElementTypes)}{VcardConstants._argumentDelimiter}" : "")}" +
                    $"{Name}{VcardConstants._fieldDelimiter}" +
                    $"{Unit}{VcardConstants._fieldDelimiter}" +
                    $"{Role}";
            }
        }

        internal override BaseCardPartInfo FromStringVcardInternal(string value, string[] finalArgs, int altId, string[] elementTypes, string valueType, Version cardVersion)
        {
            bool altIdSupported = cardVersion.Major >= 4;
            string[] splitOrg = value.Split(VcardConstants._fieldDelimiter);

            // Populate the fields
            string _orgName = Regex.Unescape(splitOrg[0]);
            string _orgUnit = Regex.Unescape(splitOrg.Length >= 2 ? splitOrg[1] : "");
            string _orgUnitRole = Regex.Unescape(splitOrg.Length >= 3 ? splitOrg[2] : "");
            OrganizationInfo _org = new(altIdSupported ? altId : 0, finalArgs, elementTypes, valueType, _orgName, _orgUnit, _orgUnitRole);
            return _org;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            Equals((OrganizationInfo)obj);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="other">The target <see cref="OrganizationInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(OrganizationInfo other) =>
            Equals(this, other);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="source">The source <see cref="OrganizationInfo"/> instance to check to see if they equal</param>
        /// <param name="target">The target <see cref="OrganizationInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(OrganizationInfo source, OrganizationInfo target)
        {
            // We can't perform this operation on null.
            if (source is null || target is null)
                return false;

            // Check all the properties
            return
                source.Name == target.Name &&
                source.Unit == target.Unit &&
                source.Role == target.Role
            ;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1382759124;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Unit);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Role);
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(OrganizationInfo left, OrganizationInfo right) =>
            left.Equals(right);

        /// <inheritdoc/>
        public static bool operator !=(OrganizationInfo left, OrganizationInfo right) =>
            !(left == right);

        internal override bool EqualsInternal(BaseCardPartInfo source, BaseCardPartInfo target) =>
            ((OrganizationInfo)source) == ((OrganizationInfo)target);

        internal OrganizationInfo() { }

        internal OrganizationInfo(int altId, string[] arguments, string[] elementTypes, string valueType, string name, string unit, string role) :
            base(arguments, altId, elementTypes, valueType)
        {
            Name = name;
            Unit = unit;
            Role = role;
        }
    }
}
