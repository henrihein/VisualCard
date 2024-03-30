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
using VisualCard.Parsers;

namespace VisualCard.Parts.Implementations
{
    /// <summary>
    /// Contact wedding anniversary info
    /// </summary>
    [DebuggerDisplay("Wedding anniversary = {Anniversary}")]
    public class AnniversaryInfo : BaseCardPartInfo, IEquatable<AnniversaryInfo>
    {
        /// <summary>
        /// The contact's wedding anniversary date (that is, the day that this contact is married)
        /// </summary>
        public DateTime? Anniversary { get; }

        internal static BaseCardPartInfo FromStringVcardStatic(string value, int altId, Version cardVersion) =>
            new AnniversaryInfo().FromStringVcardInternal(value, altId, cardVersion);

        internal static BaseCardPartInfo FromStringVcardWithTypeStatic(string value, string[] finalArgs, int altId, Version cardVersion) =>
            new AnniversaryInfo().FromStringVcardWithTypeInternal(value, finalArgs, altId, cardVersion);

        internal override string ToStringVcardInternal(Version cardVersion) =>
            $"{VcardConstants._anniversarySpecifier}:{Anniversary:yyyyMMdd}";

        internal override BaseCardPartInfo FromStringVcardInternal(string value, int altId, Version cardVersion)
        {
            // Get the value
            string anniversaryValue = value.Substring(VcardConstants._anniversarySpecifier.Length + 1);

            // Populate the fields
            return InstallInfo(anniversaryValue);
        }

        internal override BaseCardPartInfo FromStringVcardWithTypeInternal(string value, string[] finalArgs, int altId, Version cardVersion) =>
            FromStringVcardInternal(value, altId, cardVersion);

        private AnniversaryInfo InstallInfo(string value)
        {
            // Populate field
            DateTime anniversary;
            if (int.TryParse(value, out _) && value.Length == 8)
            {
                int anniversaryNum = int.Parse(value);
                var anniversaryDigits = VcardParserTools.GetDigits(anniversaryNum).ToList();
                int anniversaryYear = anniversaryDigits[0] * 1000 + anniversaryDigits[1] * 100 + anniversaryDigits[2] * 10 + anniversaryDigits[3];
                int anniversaryMonth = anniversaryDigits[4] * 10 + anniversaryDigits[5];
                int anniversaryDay = anniversaryDigits[6] * 10 + anniversaryDigits[7];
                anniversary = new DateTime(anniversaryYear, anniversaryMonth, anniversaryDay);
            }
            else
                anniversary = DateTime.Parse(value);

            // Add the fetched information
            AnniversaryInfo _time = new(0, [], anniversary);
            return _time;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            base.Equals(obj);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="other">The target <see cref="AnniversaryInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(AnniversaryInfo other) =>
            Equals(this, other);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="source">The source <see cref="AnniversaryInfo"/> instance to check to see if they equal</param>
        /// <param name="target">The target <see cref="AnniversaryInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(AnniversaryInfo source, AnniversaryInfo target)
        {
            // We can't perform this operation on null.
            if (source is null)
                return false;

            // Check all the properties
            return
                source.AltArguments.SequenceEqual(target.AltArguments) &&
                source.AltId == target.AltId &&
                source.Anniversary == target.Anniversary
            ;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -2035919920;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(AltArguments);
            hashCode = hashCode * -1521134295 + AltId.GetHashCode();
            hashCode = hashCode * -1521134295 + Anniversary.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(AnniversaryInfo left, AnniversaryInfo right) =>
            EqualityComparer<AnniversaryInfo>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(AnniversaryInfo left, AnniversaryInfo right) =>
            !(left == right);

        internal AnniversaryInfo() { }

        internal AnniversaryInfo(int altId, string[] altArguments, DateTime? anniversary)
        {
            AltId = altId;
            AltArguments = altArguments;
            Anniversary = anniversary;
        }
    }
}
