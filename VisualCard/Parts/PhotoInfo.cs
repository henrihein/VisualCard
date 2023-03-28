﻿/*
 * MIT License
 *
 * Copyright (c) 2021-2022 Aptivi
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using VisualCard.Parsers;
using VisualCard.Parsers.Four;
using VisualCard.Parsers.Three;
using VisualCard.Parsers.Two;

namespace VisualCard.Parts
{
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
                    $"{VcardConstants._photoSpecifierWithType}" +
                    $"VALUE={ValueType}{VcardConstants._argumentDelimiter}" +
                    $"{PhotoEncoded}";
            }
            else
            {
                string photoArgsLine =
                    $"{VcardConstants._photoSpecifierWithType}" +
                    $"VALUE={ValueType}{VcardConstants._fieldDelimiter}" +
                    $"ENCODING={Encoding}{VcardConstants._fieldDelimiter}" +
                    $"TYPE={PhotoType}{VcardConstants._argumentDelimiter}";
                return photoArgsLine + BaseVcardParser.MakeStringBlock(PhotoEncoded, photoArgsLine.Length);
            }
        }

        internal string ToStringVcardThree()
        {
            if (ValueType == "uri" || ValueType == "url")
            {
                return
                    $"{VcardConstants._photoSpecifierWithType}" +
                    $"VALUE={ValueType}{VcardConstants._argumentDelimiter}" +
                    $"{PhotoEncoded}";
            }
            else
            {
                string photoArgsLine =
                    $"{VcardConstants._photoSpecifierWithType}" +
                    $"VALUE={ValueType}{VcardConstants._fieldDelimiter}" +
                    $"ENCODING={Encoding}{VcardConstants._fieldDelimiter}" +
                    $"TYPE={PhotoType}{VcardConstants._argumentDelimiter}";
                return photoArgsLine + BaseVcardParser.MakeStringBlock(PhotoEncoded, photoArgsLine.Length);
            }
        }

        internal string ToStringVcardFour()
        {
            bool installAltId = AltId >= 0 && AltArguments.Length > 0;
            if (ValueType == "uri" || ValueType == "url")
            {
                return
                    $"{VcardConstants._photoSpecifierWithType}" +
                    $"{(installAltId ? "ALTID=" + AltId + VcardConstants._fieldDelimiter : "")}" +
                    $"{(installAltId ? string.Join(VcardConstants._fieldDelimiter.ToString(), AltArguments) + VcardConstants._fieldDelimiter : "")}" +
                    $"VALUE={ValueType}{VcardConstants._argumentDelimiter}" +
                    $"{PhotoEncoded}";
            }
            else
            {
                string photoArgsLine =
                    $"{VcardConstants._photoSpecifierWithType}" +
                    $"{(installAltId ? "ALTID=" + AltId + VcardConstants._fieldDelimiter : "")}" +
                    $"{(installAltId ? string.Join(VcardConstants._fieldDelimiter.ToString(), AltArguments) + VcardConstants._fieldDelimiter : "")}" +
                    $"VALUE={ValueType}{VcardConstants._fieldDelimiter}" +
                    $"ENCODING={Encoding}{VcardConstants._fieldDelimiter}" +
                    $"TYPE={PhotoType}{VcardConstants._argumentDelimiter}";
                return photoArgsLine + BaseVcardParser.MakeStringBlock(PhotoEncoded, photoArgsLine.Length);
            }
        }

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
