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
using System.IO;
using VisualCard.Parts.Enums;
using VisualCard.Parts.Implementations;
using VisualCard.Parts;
using System.Text.RegularExpressions;

namespace VisualCard.Parsers
{
    internal class VcardParserTools
    {
        internal static bool EnumArrayTypeSupported(PartsArrayEnum partsArrayEnum, Version cardVersion, string kind) =>
            partsArrayEnum switch
            {
                PartsArrayEnum.Names => true,
                PartsArrayEnum.Telephones => true,
                PartsArrayEnum.Addresses => true,
                PartsArrayEnum.Mails => true,
                PartsArrayEnum.Organizations => true,
                PartsArrayEnum.Titles => true,
                PartsArrayEnum.Photos => true,
                PartsArrayEnum.Roles => true,
                PartsArrayEnum.Logos => true,
                PartsArrayEnum.TimeZone => true,
                PartsArrayEnum.Geo => true,
                PartsArrayEnum.Sounds => true,
                PartsArrayEnum.Categories => true,
                PartsArrayEnum.Key => true,
                PartsArrayEnum.Revision => true,
                PartsArrayEnum.Birthdate => true,
                PartsArrayEnum.FullName => true,
                PartsArrayEnum.Url => true,
                PartsArrayEnum.Notes => true,
                PartsArrayEnum.Source => true,
                PartsArrayEnum.Impps => cardVersion.Major >= 3,
                PartsArrayEnum.Nicknames => cardVersion.Major >= 3,
                PartsArrayEnum.Labels => cardVersion.Major != 4,
                PartsArrayEnum.Agents => cardVersion.Major != 4,
                PartsArrayEnum.Langs => cardVersion.Major >= 4,
                PartsArrayEnum.Xml => cardVersion.Major == 4,
                PartsArrayEnum.Anniversary => cardVersion.Major >= 4,
                PartsArrayEnum.Gender => cardVersion.Major >= 4,
                PartsArrayEnum.FreeBusyUrl => cardVersion.Major >= 4,
                PartsArrayEnum.CalendarUrl => cardVersion.Major >= 4,
                PartsArrayEnum.CalendarSchedulingRequestUrl => cardVersion.Major >= 4,
                PartsArrayEnum.ContactUri => cardVersion.Major >= 4,
                PartsArrayEnum.Member => kind == "group" && cardVersion.Major == 4,

                // Extensions are allowed
                _ => true,
            };

        internal static string GetPrefixFromStringsEnum(StringsEnum stringsEnum) =>
            stringsEnum switch
            {
                StringsEnum.AccessClassification => VcardConstants._classSpecifier,
                StringsEnum.Kind => VcardConstants._kindSpecifier,
                StringsEnum.Mailer => VcardConstants._mailerSpecifier,
                StringsEnum.ProductId => VcardConstants._productIdSpecifier,
                StringsEnum.SortString => VcardConstants._sortStringSpecifier,
                StringsEnum.Uid => VcardConstants._uidSpecifier,
                _ =>
                    throw new NotImplementedException($"String enumeration {stringsEnum} is not implemented.")
            };

        internal static string GetPrefixFromPartsArrayEnum(PartsArrayEnum partsArrayEnum) =>
            partsArrayEnum switch
            {
                PartsArrayEnum.Names => VcardConstants._nameSpecifier,
                PartsArrayEnum.Telephones => VcardConstants._telephoneSpecifier,
                PartsArrayEnum.Addresses => VcardConstants._addressSpecifier,
                PartsArrayEnum.Labels => VcardConstants._labelSpecifier,
                PartsArrayEnum.Agents => VcardConstants._agentSpecifier,
                PartsArrayEnum.Mails => VcardConstants._emailSpecifier,
                PartsArrayEnum.Organizations => VcardConstants._orgSpecifier,
                PartsArrayEnum.Titles => VcardConstants._titleSpecifier,
                PartsArrayEnum.Photos => VcardConstants._photoSpecifier,
                PartsArrayEnum.Nicknames => VcardConstants._nicknameSpecifier,
                PartsArrayEnum.Roles => VcardConstants._roleSpecifier,
                PartsArrayEnum.Logos => VcardConstants._logoSpecifier,
                PartsArrayEnum.TimeZone => VcardConstants._timeZoneSpecifier,
                PartsArrayEnum.Geo => VcardConstants._geoSpecifier,
                PartsArrayEnum.Sounds => VcardConstants._soundSpecifier,
                PartsArrayEnum.Impps => VcardConstants._imppSpecifier,
                PartsArrayEnum.Categories => VcardConstants._categoriesSpecifier,
                PartsArrayEnum.Langs => VcardConstants._langSpecifier,
                PartsArrayEnum.Xml => VcardConstants._xmlSpecifier,
                PartsArrayEnum.Key => VcardConstants._keySpecifier,
                PartsArrayEnum.Birthdate => VcardConstants._birthSpecifier,
                PartsArrayEnum.Revision => VcardConstants._revSpecifier,
                PartsArrayEnum.Anniversary => VcardConstants._anniversarySpecifier,
                PartsArrayEnum.Gender => VcardConstants._genderSpecifier,
                PartsArrayEnum.CalendarSchedulingRequestUrl => VcardConstants._caladrUriSpecifier,
                PartsArrayEnum.CalendarUrl => VcardConstants._calUriSpecifier,
                PartsArrayEnum.FreeBusyUrl => VcardConstants._fbUrlSpecifier,
                PartsArrayEnum.FullName => VcardConstants._fullNameSpecifier,
                PartsArrayEnum.Notes => VcardConstants._noteSpecifier,
                PartsArrayEnum.Source => VcardConstants._sourceSpecifier,
                PartsArrayEnum.Url => VcardConstants._urlSpecifier,
                PartsArrayEnum.ContactUri => VcardConstants._contactUriSpecifier,
                PartsArrayEnum.Member => VcardConstants._memberSpecifier,

                // Extensions are allowed
                PartsArrayEnum.NonstandardNames => VcardConstants._xSpecifier,
                _ => ""
            };

        internal static (PartsArrayEnum, PartCardinality) GetPartsArrayEnumFromType(Type? partsArrayType, Version cardVersion)
        {
            if (partsArrayType is null)
                throw new NotImplementedException("Type is not provided.");

            // Now, iterate through every type
            if (partsArrayType == typeof(NameInfo))
                return (PartsArrayEnum.Names, cardVersion.Major == 4 ? PartCardinality.MayBeOne : PartCardinality.ShouldBeOne);
            else if (partsArrayType == typeof(TelephoneInfo))
                return (PartsArrayEnum.Telephones, PartCardinality.Any);
            else if (partsArrayType == typeof(AddressInfo))
                return (PartsArrayEnum.Addresses, PartCardinality.Any);
            else if (partsArrayType == typeof(LabelAddressInfo))
                return (PartsArrayEnum.Labels, PartCardinality.Any);
            else if (partsArrayType == typeof(AgentInfo))
                return (PartsArrayEnum.Agents, PartCardinality.Any);
            else if (partsArrayType == typeof(EmailInfo))
                return (PartsArrayEnum.Mails, PartCardinality.Any);
            else if (partsArrayType == typeof(OrganizationInfo))
                return (PartsArrayEnum.Organizations, PartCardinality.Any);
            else if (partsArrayType == typeof(TitleInfo))
                return (PartsArrayEnum.Titles, PartCardinality.Any);
            else if (partsArrayType == typeof(PhotoInfo))
                return (PartsArrayEnum.Photos, PartCardinality.Any);
            else if (partsArrayType == typeof(NicknameInfo))
                return (PartsArrayEnum.Nicknames, PartCardinality.Any);
            else if (partsArrayType == typeof(RoleInfo))
                return (PartsArrayEnum.Roles, PartCardinality.Any);
            else if (partsArrayType == typeof(LogoInfo))
                return (PartsArrayEnum.Logos, PartCardinality.Any);
            else if (partsArrayType == typeof(TimeDateZoneInfo))
                return (PartsArrayEnum.TimeZone, PartCardinality.Any);
            else if (partsArrayType == typeof(GeoInfo))
                return (PartsArrayEnum.Geo, PartCardinality.Any);
            else if (partsArrayType == typeof(SoundInfo))
                return (PartsArrayEnum.Sounds, PartCardinality.Any);
            else if (partsArrayType == typeof(ImppInfo))
                return (PartsArrayEnum.Impps, PartCardinality.Any);
            else if (partsArrayType == typeof(CategoryInfo))
                return (PartsArrayEnum.Categories, PartCardinality.Any);
            else if (partsArrayType == typeof(LangInfo))
                return (PartsArrayEnum.Langs, PartCardinality.Any);
            else if (partsArrayType == typeof(XmlInfo))
                return (PartsArrayEnum.Xml, PartCardinality.Any);
            else if (partsArrayType == typeof(KeyInfo))
                return (PartsArrayEnum.Key, PartCardinality.Any);
            else if (partsArrayType == typeof(RevisionInfo))
                return (PartsArrayEnum.Revision, PartCardinality.MayBeOneNoAltId);
            else if (partsArrayType == typeof(BirthDateInfo))
                return (PartsArrayEnum.Birthdate, PartCardinality.MayBeOne);
            else if (partsArrayType == typeof(AnniversaryInfo))
                return (PartsArrayEnum.Anniversary, PartCardinality.MayBeOne);
            else if (partsArrayType == typeof(GenderInfo))
                return (PartsArrayEnum.Gender, PartCardinality.MayBeOneNoAltId);
            else if (partsArrayType == typeof(FullNameInfo))
                return (PartsArrayEnum.FullName, cardVersion.Major >= 3 ? PartCardinality.AtLeastOne : PartCardinality.Any);
            else if (partsArrayType == typeof(UrlInfo))
                return (PartsArrayEnum.Url, PartCardinality.Any);
            else if (partsArrayType == typeof(NoteInfo))
                return (PartsArrayEnum.Notes, PartCardinality.Any);
            else if (partsArrayType == typeof(SourceInfo))
                return (PartsArrayEnum.Source, PartCardinality.Any);
            else if (partsArrayType == typeof(FreeBusyInfo))
                return (PartsArrayEnum.FreeBusyUrl, PartCardinality.Any);
            else if (partsArrayType == typeof(CalendarUrlInfo))
                return (PartsArrayEnum.CalendarUrl, PartCardinality.Any);
            else if (partsArrayType == typeof(CalendarSchedulingRequestUrlInfo))
                return (PartsArrayEnum.CalendarSchedulingRequestUrl, PartCardinality.Any);
            else if (partsArrayType == typeof(ContactUriInfo))
                return (PartsArrayEnum.ContactUri, PartCardinality.Any);
            else if (partsArrayType == typeof(MemberInfo))
                return (PartsArrayEnum.Member, PartCardinality.Any);

            // Extensions are allowed
            else if (partsArrayType == typeof(XNameInfo))
                return (PartsArrayEnum.NonstandardNames, PartCardinality.Any);
            return (PartsArrayEnum.IanaNames, PartCardinality.Any);
        }

        internal static (PartType type, object enumeration, Type? enumType, Func<string, string[], int, string[], string, string, Version, BaseCardPartInfo>? fromStringFunc, string defaultType, string defaultValue, string defaultValueType, string[] allowedExtraTypes) GetPartType(string prefix) =>
            prefix switch
            {
                VcardConstants._nameSpecifier => (PartType.PartsArray, PartsArrayEnum.Names, typeof(NameInfo), NameInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._telephoneSpecifier => (PartType.PartsArray, PartsArrayEnum.Telephones, typeof(TelephoneInfo), TelephoneInfo.FromStringVcardStatic, "CELL", "", "", ["TEXT", "VOICE", "FAX", "CELL", "VIDEO", "PAGER", "TEXTPHONE", "ISDN", "CAR", "MODEM", "BBS", "MSG", "PREF", "TLX", "MMS"]),
                VcardConstants._addressSpecifier => (PartType.PartsArray, PartsArrayEnum.Addresses, typeof(AddressInfo), AddressInfo.FromStringVcardStatic, "HOME", "", "", ["DOM", "INTL", "PARCEL", "POSTAL"]),
                VcardConstants._labelSpecifier => (PartType.PartsArray, PartsArrayEnum.Labels, typeof(LabelAddressInfo), LabelAddressInfo.FromStringVcardStatic, "HOME", "", "", ["DOM", "INTL", "PARCEL", "POSTAL"]),
                VcardConstants._agentSpecifier => (PartType.PartsArray, PartsArrayEnum.Agents, typeof(AgentInfo), AgentInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._emailSpecifier => (PartType.PartsArray, PartsArrayEnum.Mails, typeof(EmailInfo), EmailInfo.FromStringVcardStatic, "HOME", "", "", ["AOL", "APPLELINK", "ATTMAIL", "CIS", "EWORLD", "INTERNET", "IBMMAIL", "MCIMAIL", "POWERSHARE", "PRODIGY", "TLX", "X400", "CELL"]),
                VcardConstants._orgSpecifier => (PartType.PartsArray, PartsArrayEnum.Organizations, typeof(OrganizationInfo), OrganizationInfo.FromStringVcardStatic, "WORK", "", "", []),
                VcardConstants._titleSpecifier => (PartType.PartsArray, PartsArrayEnum.Titles, typeof(TitleInfo), TitleInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._photoSpecifier => (PartType.PartsArray, PartsArrayEnum.Photos, typeof(PhotoInfo), PhotoInfo.FromStringVcardStatic, "JPEG", "", "", ["JPG", "GIF", "CGM", "WMF", "BMP", "MET", "PMB", "DIB", "PICT", "TIFF", "PS", "PDF", "JPEG", "MPEG", "MPEG2", "AVI", "QTIME", "PNG", "WEBP"]),
                VcardConstants._nicknameSpecifier => (PartType.PartsArray, PartsArrayEnum.Nicknames, typeof(NicknameInfo), NicknameInfo.FromStringVcardStatic, "HOME", "", "", []),
                VcardConstants._roleSpecifier => (PartType.PartsArray, PartsArrayEnum.Roles, typeof(RoleInfo), RoleInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._logoSpecifier => (PartType.PartsArray, PartsArrayEnum.Logos, typeof(LogoInfo), LogoInfo.FromStringVcardStatic, "JPEG", "", "", ["JPG", "GIF", "CGM", "WMF", "BMP", "MET", "PMB", "DIB", "PICT", "TIFF", "PS", "PDF", "JPEG", "MPEG", "MPEG2", "AVI", "QTIME", "PNG", "WEBP"]),
                VcardConstants._timeZoneSpecifier => (PartType.PartsArray, PartsArrayEnum.TimeZone, typeof(TimeDateZoneInfo), TimeDateZoneInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._geoSpecifier => (PartType.PartsArray, PartsArrayEnum.Geo, typeof(GeoInfo), GeoInfo.FromStringVcardStatic, "", "", "uri", []),
                VcardConstants._soundSpecifier => (PartType.PartsArray, PartsArrayEnum.Sounds, typeof(SoundInfo), SoundInfo.FromStringVcardStatic, "MP3", "", "", ["MP3", "WAVE", "PCM", "AIFF", "AAC"]),
                VcardConstants._imppSpecifier => (PartType.PartsArray, PartsArrayEnum.Impps, typeof(ImppInfo), ImppInfo.FromStringVcardStatic, "SIP", "", "", ["SIP"]),
                VcardConstants._categoriesSpecifier => (PartType.PartsArray, PartsArrayEnum.Categories, typeof(CategoryInfo), CategoryInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._langSpecifier => (PartType.PartsArray, PartsArrayEnum.Langs, typeof(LangInfo), LangInfo.FromStringVcardStatic, "HOME", "", "", []),
                VcardConstants._xmlSpecifier => (PartType.PartsArray, PartsArrayEnum.Xml, typeof(XmlInfo), XmlInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._keySpecifier => (PartType.PartsArray, PartsArrayEnum.Key, typeof(KeyInfo), KeyInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._revSpecifier => (PartType.PartsArray, PartsArrayEnum.Revision, typeof(RevisionInfo), RevisionInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._birthSpecifier => (PartType.PartsArray, PartsArrayEnum.Birthdate, typeof(BirthDateInfo), BirthDateInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._anniversarySpecifier => (PartType.PartsArray, PartsArrayEnum.Anniversary, typeof(AnniversaryInfo), AnniversaryInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._genderSpecifier => (PartType.PartsArray, PartsArrayEnum.Gender, typeof(GenderInfo), GenderInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._fullNameSpecifier => (PartType.PartsArray, PartsArrayEnum.FullName, typeof(FullNameInfo), FullNameInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._urlSpecifier => (PartType.PartsArray, PartsArrayEnum.Url, typeof(UrlInfo), UrlInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._noteSpecifier => (PartType.PartsArray, PartsArrayEnum.Notes, typeof(NoteInfo), NoteInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._sourceSpecifier => (PartType.PartsArray, PartsArrayEnum.Source, typeof(SourceInfo), SourceInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._fbUrlSpecifier => (PartType.PartsArray, PartsArrayEnum.FreeBusyUrl, typeof(FreeBusyInfo), FreeBusyInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._calUriSpecifier => (PartType.PartsArray, PartsArrayEnum.CalendarUrl, typeof(CalendarUrlInfo), CalendarUrlInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._caladrUriSpecifier => (PartType.PartsArray, PartsArrayEnum.CalendarSchedulingRequestUrl, typeof(CalendarSchedulingRequestUrlInfo), CalendarSchedulingRequestUrlInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._contactUriSpecifier => (PartType.PartsArray, PartsArrayEnum.ContactUri, typeof(ContactUriInfo), ContactUriInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._memberSpecifier => (PartType.PartsArray, PartsArrayEnum.Member, typeof(MemberInfo), MemberInfo.FromStringVcardStatic, "", "", "", []),
                VcardConstants._kindSpecifier => (PartType.Strings, StringsEnum.Kind, null, null, "", "", "", []),
                VcardConstants._mailerSpecifier => (PartType.Strings, StringsEnum.Mailer, null, null, "", "", "", []),
                VcardConstants._productIdSpecifier => (PartType.Strings, StringsEnum.ProductId, null, null, "", "", "", []),
                VcardConstants._sortStringSpecifier => (PartType.Strings, StringsEnum.SortString, null, null, "", "", "", []),
                VcardConstants._classSpecifier => (PartType.Strings, StringsEnum.AccessClassification, null, null, "", "", "", []),
                VcardConstants._uidSpecifier => (PartType.Strings, StringsEnum.Uid, null, null, "", "", "", []),

                // Extensions are allowed
                VcardConstants._xSpecifier => (PartType.PartsArray, PartsArrayEnum.NonstandardNames, typeof(XNameInfo), XNameInfo.FromStringVcardStatic, "", "", "", []),
                _ => (PartType.PartsArray, PartsArrayEnum.IanaNames, typeof(ExtraInfo), ExtraInfo.FromStringVcardStatic, "", "", "", []),
            };

        internal static string ProcessStringValue(string value, string valueType)
        {
            // Now, handle each type individually
            string finalValue;
            switch (valueType.ToUpper())
            {
                case "":
                case "TEXT":
                    // Unescape the value
                    finalValue = Regex.Unescape(value);
                    break;
                case "URI":
                    // Unescape the value
                    finalValue = Regex.Unescape(value);
                    if (!Uri.TryCreate(finalValue, UriKind.Absolute, out Uri uri))
                        throw new InvalidDataException($"URL {finalValue} is invalid");
                    finalValue = uri is not null ? uri.ToString() : finalValue;
                    break;
                default:
                    throw new InvalidDataException($"The string value type {valueType} is invalid. Are you sure that you've specified the correct type in your vCard representation?");
            }

            // Return the result
            return finalValue;
        }
    }
}
