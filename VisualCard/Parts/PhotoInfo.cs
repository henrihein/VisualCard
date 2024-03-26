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
using System.Text;
using VisualCard.Parsers;

namespace VisualCard.Parts
{
    /// <summary>
    /// Contact photo info
    /// </summary>
    [DebuggerDisplay("Photo, {Encoding}, {PhotoType}, {ValueType}")]
    public class PhotoInfo : IEquatable<PhotoInfo>
    {
        /// <summary>
        /// Alternative ID. Zero if unspecified.
        /// </summary>
        public int AltId { get; }
        /// <summary>
        /// Arguments that follow the AltId
        /// </summary>
        public string[] AltArguments { get; }
        /// <summary>
        /// Value type
        /// </summary>
        public string ValueType { get; }
        /// <summary>
        /// Photo encoding type
        /// </summary>
        public string Encoding { get; }
        /// <summary>
        /// Photo type (JPEG, ...)
        /// </summary>
        public string PhotoType { get; }
        /// <summary>
        /// Encoded photo
        /// </summary>
        public string PhotoEncoded { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            base.Equals(obj);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="other">The target <see cref="PhotoInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(PhotoInfo other) =>
            Equals(this, other);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="source">The source <see cref="PhotoInfo"/> instance to check to see if they equal</param>
        /// <param name="target">The target <see cref="PhotoInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(PhotoInfo source, PhotoInfo target)
        {
            // We can't perform this operation on null.
            if (source is null)
                return false;

            // Check all the properties
            return
                source.AltArguments.SequenceEqual(target.AltArguments) &&
                source.AltId == target.AltId &&
                source.ValueType == target.ValueType &&
                source.Encoding == target.Encoding &&
                source.PhotoType == target.PhotoType &&
                source.PhotoEncoded == target.PhotoEncoded
            ;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -1042689907;
            hashCode = hashCode * -1521134295 + AltId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(AltArguments);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ValueType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Encoding);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PhotoType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PhotoEncoded);
            return hashCode;
        }

        internal string ToStringVcardTwo()
        {
            if (ValueType == "uri" || ValueType == "url")
            {
                return
                    $"{VcardConstants._photoSpecifier};" +
                    $"{VcardConstants._valueArgumentSpecifier}{ValueType}{VcardConstants._argumentDelimiter}" +
                    $"{PhotoEncoded}";
            }
            else
            {
                string photoArgsLine =
                    $"{VcardConstants._photoSpecifier};" +
                    $"{VcardConstants._valueArgumentSpecifier}{ValueType}{VcardConstants._fieldDelimiter}" +
                    $"{VcardConstants._encodingArgumentSpecifier}{Encoding}{VcardConstants._fieldDelimiter}" +
                    $"{VcardConstants._typeArgumentSpecifier}{PhotoType}{VcardConstants._argumentDelimiter}";
                return photoArgsLine + BaseVcardParser.MakeStringBlock(PhotoEncoded, photoArgsLine.Length);
            }
        }

        internal string ToStringVcardThree()
        {
            if (ValueType == "uri" || ValueType == "url")
            {
                return
                    $"{VcardConstants._photoSpecifier};" +
                    $"{VcardConstants._valueArgumentSpecifier}{ValueType}{VcardConstants._argumentDelimiter}" +
                    $"{PhotoEncoded}";
            }
            else
            {
                string photoArgsLine =
                    $"{VcardConstants._photoSpecifier};" +
                    $"{VcardConstants._valueArgumentSpecifier}{ValueType}{VcardConstants._fieldDelimiter}" +
                    $"{VcardConstants._encodingArgumentSpecifier}{Encoding}{VcardConstants._fieldDelimiter}" +
                    $"{VcardConstants._typeArgumentSpecifier}{PhotoType}{VcardConstants._argumentDelimiter}";
                return photoArgsLine + BaseVcardParser.MakeStringBlock(PhotoEncoded, photoArgsLine.Length);
            }
        }

        internal string ToStringVcardFour()
        {
            bool installAltId = AltId >= 0 && AltArguments.Length > 0;
            if (ValueType == "uri" || ValueType == "url")
            {
                return
                    $"{VcardConstants._photoSpecifier};" +
                    $"{(installAltId ? VcardConstants._altIdArgumentSpecifier + AltId + VcardConstants._fieldDelimiter : "")}" +
                    $"{(installAltId ? string.Join(VcardConstants._fieldDelimiter.ToString(), AltArguments) + VcardConstants._fieldDelimiter : "")}" +
                    $"{VcardConstants._valueArgumentSpecifier}{ValueType}{VcardConstants._argumentDelimiter}" +
                    $"{PhotoEncoded}";
            }
            else
            {
                string photoArgsLine =
                    $"{VcardConstants._photoSpecifier};" +
                    $"{(installAltId ? VcardConstants._altIdArgumentSpecifier + AltId + VcardConstants._fieldDelimiter : "")}" +
                    $"{(installAltId ? string.Join(VcardConstants._fieldDelimiter.ToString(), AltArguments) + VcardConstants._fieldDelimiter : "")}" +
                    $"{VcardConstants._valueArgumentSpecifier}{ValueType}{VcardConstants._fieldDelimiter}" +
                    $"{VcardConstants._encodingArgumentSpecifier}{Encoding}{VcardConstants._fieldDelimiter}" +
                    $"{VcardConstants._typeArgumentSpecifier}{PhotoType}{VcardConstants._argumentDelimiter}";
                return photoArgsLine + BaseVcardParser.MakeStringBlock(PhotoEncoded, photoArgsLine.Length);
            }
        }

        internal string ToStringVcardFive() =>
            ToStringVcardFour();

        internal static PhotoInfo FromStringVcardTwoWithType(string value, StreamReader cardContentReader)
        {
            // Get the value
            string photoValue = value.Substring(VcardConstants._photoSpecifier.Length + 1);
            string[] splitPhoto = photoValue.Split(VcardConstants._argumentDelimiter);
            if (splitPhoto.Length < 2)
                throw new InvalidDataException("Photo field must specify exactly two values (Type and arguments, and photo information)");

            // Check to see if the value is prepended by the VALUE= argument
            string valueType = VcardParserTools.GetValuesString(splitPhoto, "", VcardConstants._valueArgumentSpecifier).ToLower();
            bool isUrl = valueType == "url" || valueType == "uri";

            // Check to see if the value is prepended by the ENCODING= argument
            string photoEncoding = VcardParserTools.GetValuesString(splitPhoto, "BASE64", VcardConstants._encodingArgumentSpecifier);

            // Check to see if the value is prepended with the TYPE= argument
            string photoType = VcardParserTools.GetTypesString(splitPhoto, "JPEG", false);

            // Now, get the encoded photo
            StringBuilder encodedPhoto = new();
            if (splitPhoto.Length == 2)
                encodedPhoto.Append(splitPhoto[1]);

            // Make sure to get all the blocks until we reach an empty line
            if (!isUrl)
            {
                string lineToBeAppended = cardContentReader.ReadLine();
                while (!string.IsNullOrWhiteSpace(lineToBeAppended) && lineToBeAppended.StartsWith(" "))
                {
                    encodedPhoto.Append(lineToBeAppended.Trim());
                    lineToBeAppended = cardContentReader.ReadLine();
                }
            }

            // Populate the fields
            PhotoInfo _photo = new(0, [], valueType, photoEncoding, photoType, encodedPhoto.ToString());
            return _photo;
        }

        internal static PhotoInfo FromStringVcardThreeWithType(string value, StreamReader cardContentReader)
        {
            // Get the value
            string photoValue = value.Substring(VcardConstants._photoSpecifier.Length + 1);
            string[] splitPhoto = photoValue.Split(VcardConstants._argumentDelimiter);
            if (splitPhoto.Length < 2)
                throw new InvalidDataException("Photo field must specify exactly two values (Type and arguments, and photo information)");

            // Check to see if the value is prepended by the VALUE= argument
            string valueType = VcardParserTools.GetValuesString(splitPhoto, "", VcardConstants._valueArgumentSpecifier).ToLower();
            bool isUrl = valueType == "url" || valueType == "uri";

            // Check to see if the value is prepended by the ENCODING= argument
            string photoEncoding = VcardParserTools.GetValuesString(splitPhoto, "BASE64", VcardConstants._encodingArgumentSpecifier);

            // Check to see if the value is prepended with the TYPE= argument
            string photoType = VcardParserTools.GetTypesString(splitPhoto, "JPEG", true);

            // Now, get the encoded photo
            StringBuilder encodedPhoto = new();
            if (splitPhoto.Length == 2)
                encodedPhoto.Append(splitPhoto[1]);

            // Make sure to get all the blocks until we reach an empty line
            if (!isUrl)
            {
                string lineToBeAppended = cardContentReader.ReadLine();
                while (!string.IsNullOrWhiteSpace(lineToBeAppended) && lineToBeAppended.StartsWith(" "))
                {
                    encodedPhoto.Append(lineToBeAppended.Trim());
                    lineToBeAppended = cardContentReader.ReadLine();
                }
            }

            // Populate the fields
            PhotoInfo _photo = new(0, [], valueType, photoEncoding, photoType, encodedPhoto.ToString());
            return _photo;
        }

        internal static PhotoInfo FromStringVcardFourWithType(string value, List<string> finalArgs, int altId, StreamReader cardContentReader)
        {
            // Get the value
            string photoValue = value.Substring(VcardConstants._photoSpecifier.Length + 1);
            string[] splitPhoto = photoValue.Split(VcardConstants._argumentDelimiter);
            if (splitPhoto.Length < 2)
                throw new InvalidDataException("Photo field must specify exactly two values (Type and arguments, and photo information)");

            // Check to see if the value is prepended by the VALUE= argument
            string valueType = VcardParserTools.GetValuesString(splitPhoto, "", VcardConstants._valueArgumentSpecifier).ToLower();
            bool isUrl = valueType == "url" || valueType == "uri";

            // Check to see if the value is prepended by the ENCODING= argument
            string photoEncoding = VcardParserTools.GetValuesString(splitPhoto, "BASE64", VcardConstants._encodingArgumentSpecifier);

            // Check to see if the value is prepended with the TYPE= argument
            string photoType = VcardParserTools.GetTypesString(splitPhoto, "JPEG", true);

            // Now, get the encoded photo
            StringBuilder encodedPhoto = new();
            if (splitPhoto.Length == 2)
                encodedPhoto.Append(splitPhoto[1]);

            // Make sure to get all the blocks until we reach an empty line
            if (!isUrl)
            {
                string lineToBeAppended = cardContentReader.ReadLine();
                while (!string.IsNullOrWhiteSpace(lineToBeAppended) && lineToBeAppended.StartsWith(" "))
                {
                    encodedPhoto.Append(lineToBeAppended.Trim());
                    lineToBeAppended = cardContentReader.ReadLine();
                }
            }

            // Populate the fields
            PhotoInfo _photo = new(altId, [.. finalArgs], valueType, photoEncoding, photoType, encodedPhoto.ToString());
            return _photo;
        }

        internal static PhotoInfo FromStringVcardFiveWithType(string value, List<string> finalArgs, int altId, StreamReader cardContentReader) =>
            FromStringVcardFourWithType(value, finalArgs, altId, cardContentReader);

        internal PhotoInfo() { }

        internal PhotoInfo(int altId, string[] altArguments, string valueType, string encoding, string photoType, string photoEncoded)
        {
            AltId = altId;
            AltArguments = altArguments;
            ValueType = valueType;
            Encoding = encoding;
            PhotoType = photoType;
            PhotoEncoded = photoEncoded;
        }
    }
}
