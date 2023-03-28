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
    public class LogoInfo : IEquatable<LogoInfo>
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
        /// Logo encoding type
        /// </summary>
        public string Encoding { get; }
        /// <summary>
        /// Logo type (JPEG, ...)
        /// </summary>
        public string LogoType { get; }
        /// <summary>
        /// Encoded logo
        /// </summary>
        public string LogoEncoded { get; }

        public override bool Equals(object obj) =>
            base.Equals(obj);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="other">The target <see cref="LogoInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(LogoInfo other) =>
            Equals(this, other);

        /// <summary>
        /// Checks to see if both the parts are equal
        /// </summary>
        /// <param name="source">The source <see cref="LogoInfo"/> instance to check to see if they equal</param>
        /// <param name="target">The target <see cref="LogoInfo"/> instance to check to see if they equal</param>
        /// <returns>True if all the part elements are equal. Otherwise, false.</returns>
        public bool Equals(LogoInfo source, LogoInfo target)
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
                source.LogoType == target.LogoType &&
                source.LogoEncoded == target.LogoEncoded
            ;
        }

        public override int GetHashCode()
        {
            int hashCode = -1881924127;
            hashCode = hashCode * -1521134295 + AltId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(AltArguments);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ValueType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Encoding);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LogoType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LogoEncoded);
            return hashCode;
        }

        internal string ToStringVcardTwo()
        {
            if (ValueType == "uri" || ValueType == "url")
            {
                return
                    $"{VcardConstants._logoSpecifierWithType}" +
                    $"VALUE={ValueType}{VcardConstants._argumentDelimiter}" +
                    $"{LogoEncoded}";
            }
            else
            {
                string logoArgsLine =
                    $"{VcardConstants._logoSpecifierWithType}" +
                    $"VALUE={ValueType}{VcardConstants._fieldDelimiter}" +
                    $"ENCODING={Encoding}{VcardConstants._fieldDelimiter}" +
                    $"TYPE={LogoType}{VcardConstants._argumentDelimiter}";
                return logoArgsLine + BaseVcardParser.MakeStringBlock(LogoEncoded, logoArgsLine.Length);
            }
        }

        internal string ToStringVcardThree()
        {
            if (ValueType == "uri" || ValueType == "url")
            {
                return
                    $"{VcardConstants._logoSpecifierWithType}" +
                    $"VALUE={ValueType}{VcardConstants._argumentDelimiter}" +
                    $"{LogoEncoded}";
            }
            else
            {
                string logoArgsLine =
                    $"{VcardConstants._logoSpecifierWithType}" +
                    $"VALUE={ValueType}{VcardConstants._fieldDelimiter}" +
                    $"ENCODING={Encoding}{VcardConstants._fieldDelimiter}" +
                    $"TYPE={LogoType}{VcardConstants._argumentDelimiter}";
                return logoArgsLine + BaseVcardParser.MakeStringBlock(LogoEncoded, logoArgsLine.Length);
            }
        }

        internal string ToStringVcardFour()
        {
            bool installAltId = AltId > 0 && AltArguments.Length > 0;
            if (ValueType == "uri" || ValueType == "url")
            {
                return
                    $"{VcardConstants._logoSpecifierWithType}" +
                    $"{(installAltId ? "ALTID=" + AltId + VcardConstants._fieldDelimiter : "")}" +
                    $"{(installAltId ? string.Join(VcardConstants._fieldDelimiter.ToString(), AltArguments) + VcardConstants._fieldDelimiter : "")}" +
                    $"VALUE={ValueType}{VcardConstants._argumentDelimiter}" +
                    $"{LogoEncoded}";
            }
            else
            {
                string logoArgsLine =
                    $"{VcardConstants._logoSpecifierWithType}" +
                    $"{(installAltId ? "ALTID=" + AltId + VcardConstants._fieldDelimiter : "")}" +
                    $"{(installAltId ? string.Join(VcardConstants._fieldDelimiter.ToString(), AltArguments) + VcardConstants._fieldDelimiter : "")}" +
                    $"VALUE={ValueType}{VcardConstants._fieldDelimiter}" +
                    $"ENCODING={Encoding}{VcardConstants._fieldDelimiter}" +
                    $"TYPE={LogoType}{VcardConstants._argumentDelimiter}";
                return logoArgsLine + BaseVcardParser.MakeStringBlock(LogoEncoded, logoArgsLine.Length);
            }
        }

        internal LogoInfo() { }

        internal LogoInfo(int altId, string[] altArguments, string valueType, string encoding, string logoType, string logoEncoded)
        {
            AltId = altId;
            AltArguments = altArguments;
            ValueType = valueType;
            Encoding = encoding;
            LogoType = logoType;
            LogoEncoded = logoEncoded;
        }
    }
}
