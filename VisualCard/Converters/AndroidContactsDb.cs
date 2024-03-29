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

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VisualCard.Parsers;
using VisualCard.Parts;

namespace VisualCard.Converters
{
    /// <summary>
    /// Android contacts database management class
    /// </summary>
    public static class AndroidContactsDb
    {
        /// <summary>
        /// Gets all contacts from the Android database
        /// </summary>
        /// <param name="pathToDb">Path to the valid contacts2.db file, usually fetched from /data/data/android.providers.contacts/databases/ found in the rooted Android devices</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static Card[] GetContactsFromDb(string pathToDb)
        {
            // Check to see if the database exists
            string dbObtainTip = 
                "\n\nMake sure that your phone is rooted before being able to obtain the contacts2.db file " +
                "usually found in the /data/data/android.providers.contacts/databases/ folder. If the " +
                "file is not there, you may need to look for this file under the /data/user/0/com.android.providers.contacts/databases/ " +
                "folder. Some ROMs, such as Motorola, may store contacts in their own contact provider " +
                "folder, for example, /data/user/0/com.motorola.blur.providers.contacts/databases/.";
            if (!File.Exists(pathToDb))
                throw new FileNotFoundException("The Android contact database file obtained from the contact provider is not found." + dbObtainTip);

            try
            {
                // Since Android stores contacts using the SQLite databases found under this path:
                //     /data/data/android.providers.contacts/databases/contacts2.db
                //
                // the user will have to either use ADB root to pull this file to the computer or root the
                // phone and copy this file so that VisualCard knows how to parse Android contacts.
                // However, this is unintuitive since apparently root is required to do this. Android may
                // also store contacts in this path:
                //     /data/user/0/com.android.providers.contacts/databases/contacts2.db
                //
                // Of course, that depends on the device. For example, Motorola ROMs may store contacts
                // in this path:
                //     /data/data/com.motorola.blur.providers.contacts/databases/contacts2.db
                using var connection = new SqliteConnection($"Data Source={pathToDb}");
                connection.Open();

                // Get only the necessary values.
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    SELECT
                        raw_contacts._id,
                        mimetypes.mimetype,
                        REPLACE(REPLACE(data.data1, $MS_NEWLINE_QUOTED, '\n'), $NEWLINE_QUOTED, '\n'),
                        data.data2,
                        data.data3,
                        REPLACE(REPLACE(data.data4, $MS_NEWLINE_QUOTED, '\n'), $NEWLINE_QUOTED, '\n'),
                        data.data5,
                        data.data6,
                        data.data7,
                        data.data8,
                        data.data9,
                        data.data10,
                        data.data11,
                        data.data12,
                        data.data13,
                        data.data14,
                        quote(data.data15)
                    FROM
                        raw_contacts,
                        data,
                        mimetypes
                    WHERE
                        raw_contacts.deleted = 0 AND
                        raw_contacts._id = data.raw_contact_id AND
                        data.mimetype_id = mimetypes._id
                    ORDER BY
                        raw_contacts._id,
                        mimetypes._id,
                        data.data2
                ";
                command.Parameters.AddWithValue("$MS_NEWLINE_QUOTED", "\r\n");
                command.Parameters.AddWithValue("$NEWLINE_QUOTED", "\n");

                // Install the values!
                using var reader = command.ExecuteReader();
                var masterContactBuilder = new StringBuilder(
                    $"""
                    {VcardConstants._beginText}
                    {VcardConstants._versionSpecifier}:3.0

                    """
                );
                bool idChanged = false;
                bool birthdaySeen = false;
                string lastId = "";
                while (reader.Read())
                {
                    // Get the ID
                    var id = !reader.IsDBNull(0) ? reader.GetString(0) : "-1";

                    // Ignore ID -1
                    if (id == "-1")
                        continue;

                    // Check to see if the ID is changed
                    if (id != lastId && !string.IsNullOrEmpty(lastId))
                        idChanged = true;
                    if (idChanged)
                    {
                        idChanged = false;
                        masterContactBuilder.AppendLine(
                            $"""
                            {VcardConstants._endText}

                            {VcardConstants._beginText}
                            {VcardConstants._versionSpecifier}:3.0
                            """
                        );
                    }

                    // Get the metadata type and the associated data.
                    var mtype = !reader.IsDBNull(1) ? reader.GetString(1) : $"";
                    var d1 = !reader.IsDBNull(2) ? reader.GetString(2) : $"";
                    var d2 = !reader.IsDBNull(3) ? reader.GetString(3) : $"";
                    var d3 = !reader.IsDBNull(4) ? reader.GetString(4) : $"";
                    var d4 = !reader.IsDBNull(5) ? reader.GetString(5) : $"";
                    var d5 = !reader.IsDBNull(6) ? reader.GetString(6) : $"";
                    var d6 = !reader.IsDBNull(7) ? reader.GetString(7) : $"";
                    var d7 = !reader.IsDBNull(8) ? reader.GetString(8) : $"";
                    var d8 = !reader.IsDBNull(9) ? reader.GetString(9) : $"";
                    var d9 = !reader.IsDBNull(10) ? reader.GetString(10) : $"";
                    var d10 = !reader.IsDBNull(11) ? reader.GetString(11) : $"";
                    var d11 = !reader.IsDBNull(12) ? reader.GetString(12) : $"";
                    var d12 = !reader.IsDBNull(13) ? reader.GetString(13) : $"";
                    var d13 = !reader.IsDBNull(14) ? reader.GetString(14) : $"";
                    var d14 = !reader.IsDBNull(15) ? reader.GetString(15) : $"";
                    var d15 = !reader.IsDBNull(16) ? reader.GetString(16) : $"";

                    // Check the metadata type. They're not documented to us even in the official Android developers guide,
                    // so we have to figure things out.
                    switch (mtype)
                    {
                        case "vnd.android.cursor.item/name":
                            // Example:
                            //
                            // _id    mimetype                        data1                  data2      data3          data4    data5             data6
                            // 8      vnd.android.cursor.item/name    Neville Navasquillo    Neville    Navasquillo    Mr.      Neville,Nevile    Jr.
                            //
                            // | d1: Full name
                            // ---> will get converted to VCard line:
                            //      FN:d1
                            //
                            // | d2: First name
                            // | d3: Last name
                            // | d4: Title
                            // | d5: Alt Names (comma delimiter)
                            // | d6: Suffix
                            // ---> will get converted to VCard line:
                            //      N:d3;d2;d5;d4;d6;
                            const string _nameSpecifier     = "N:";
                            const string _fullNameSpecifier = "FN:";
                            masterContactBuilder.AppendLine($"{_fullNameSpecifier}{d1}");
                            masterContactBuilder.AppendLine($"{_nameSpecifier}{d3};{d2};{d5};{d4};{d6};");
                            break;
                        case "vnd.android.cursor.item/phone_v2":
                            // Example:
                            //
                            // _id    mimetype                            data1           data2
                            // 8      vnd.android.cursor.item/phone_v2    348-404-8404    1
                            //
                            // | d1: Phone number
                            // | d2: Phone type by integer
                            // ---> will get converted to VCard line:
                            //      TEL;TYPE=cell:1-234-567-890
                            const string _telephoneSpecifierWithType = "TEL;";
                            string _telephoneTypeName = "cell";
                            switch (d2)
                            {
                                // Let's do some translation work!
                                // From Android's source code:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/base/core/java/android/provider/ContactsContract.java;l=6297
                                //
                                // public static final int TYPE_HOME = 1;
                                // public static final int TYPE_MOBILE = 2;
                                // public static final int TYPE_WORK = 3;
                                // public static final int TYPE_FAX_WORK = 4;
                                // public static final int TYPE_FAX_HOME = 5;
                                // public static final int TYPE_PAGER = 6;
                                // public static final int TYPE_OTHER = 7;
                                // public static final int TYPE_CALLBACK = 8;
                                // public static final int TYPE_CAR = 9;
                                // public static final int TYPE_COMPANY_MAIN = 10;
                                // public static final int TYPE_ISDN = 11;
                                // public static final int TYPE_MAIN = 12;
                                // public static final int TYPE_OTHER_FAX = 13;
                                // public static final int TYPE_RADIO = 14;
                                // public static final int TYPE_TELEX = 15;
                                // public static final int TYPE_TTY_TDD = 16;
                                // public static final int TYPE_WORK_MOBILE = 17;
                                // public static final int TYPE_WORK_PAGER = 18;
                                // public static final int TYPE_ASSISTANT = 19;
                                // public static final int TYPE_MMS = 20;
                                //
                                // Translation to VCard code is found here:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/opt/vcard/java/com/android/vcard/VCardBuilder.java;l=1589
                                case "1":
                                    // TYPE_HOME -> home
                                    _telephoneTypeName = "home";
                                    break;
                                case "3":
                                    // TYPE_WORK -> work
                                    _telephoneTypeName = "work";
                                    break;
                                case "4":
                                    // TYPE_FAX_WORK -> work,fax
                                    _telephoneTypeName = "work,fax";
                                    break;
                                case "5":
                                    // TYPE_FAX_HOME -> home,fax
                                    _telephoneTypeName = "home,fax";
                                    break;
                                case "6":
                                    // TYPE_PAGER -> pager
                                    _telephoneTypeName = "pager";
                                    break;
                                case "7":
                                    // TYPE_OTHER -> voice
                                    _telephoneTypeName = "voice";
                                    break;
                                case "9":
                                    // TYPE_CAR -> car
                                    _telephoneTypeName = "car";
                                    break;
                                case "10":
                                    // TYPE_COMPANY_MAIN -> work
                                    _telephoneTypeName = "work";
                                    break;
                                case "11":
                                    // TYPE_ISDN -> isdn
                                    _telephoneTypeName = "isdn";
                                    break;
                                case "13":
                                    // TYPE_OTHER_FAX -> fax
                                    _telephoneTypeName = "fax";
                                    break;
                                case "15":
                                    // TYPE_TELEX -> tlx
                                    _telephoneTypeName = "tlx";
                                    break;
                                case "17":
                                    // TYPE_WORK_MOBILE -> work,cell
                                    _telephoneTypeName = "work,cell";
                                    break;
                                case "18":
                                    // TYPE_WORK_PAGER -> work,pager
                                    _telephoneTypeName = "work,pager";
                                    break;
                                case "20":
                                    // TYPE_MMS -> mms
                                    _telephoneTypeName = "mms";
                                    break;
                            }
                            masterContactBuilder.AppendLine($"{_telephoneSpecifierWithType}TYPE={_telephoneTypeName}:{d1}");
                            break;
                        case "vnd.android.cursor.item/nickname":
                            // Example:
                            //
                            // _id    mimetype                            data1
                            // 8      vnd.android.cursor.item/nickname    NVL.N
                            //
                            // | d1: Nickname
                            // ---> will get converted to VCard line:
                            //      NICKNAME:d1
                            const string _nicknameSpecifier = "NICKNAME:";
                            masterContactBuilder.AppendLine($"{_nicknameSpecifier}{d1}");
                            break;
                        case "vnd.android.cursor.item/email_v2":
                            // Example:
                            //
                            // _id    mimetype                            data1                    data2
                            // 8      vnd.android.cursor.item/email_v2    neville.nvs@gmail.com    1
                            // 8      vnd.android.cursor.item/email_v2    neville.nvs@nvsc.com     2
                            //
                            // | d1: E-mail
                            // | d2: E-mail type
                            // ---> will get converted to VCard line:
                            //      EMAIL;TYPE=d2:d1
                            const string _emailSpecifier = "EMAIL;";
                            string _emailTypeName = "home";
                            switch (d2)
                            {
                                // Let's do some translation work!
                                // From Android's source code:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/base/core/java/android/provider/ContactsContract.java;l=6582
                                //
                                // public static final int TYPE_HOME = 1;
                                // public static final int TYPE_WORK = 2;
                                // public static final int TYPE_OTHER = 3;
                                // public static final int TYPE_MOBILE = 4;
                                //
                                // Translation to VCard code is found here:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/opt/vcard/java/com/android/vcard/VCardBuilder.java;l=1526
                                case "2":
                                    // TYPE_WORK -> work
                                    _emailTypeName = "work";
                                    break;
                                case "4":
                                    // TYPE_MOBILE -> cell
                                    _emailTypeName = "cell";
                                    break;
                            }
                            masterContactBuilder.AppendLine($"{_emailSpecifier}TYPE={_emailTypeName}:{d1}");
                            break;
                        case "vnd.android.cursor.item/postal-address_v2":
                            // Example:
                            //
                            // _id    mimetype                                     data1                                                     data2    data3    data4                        data5    data6    data7    data8    data9    data10
                            // 8      vnd.android.cursor.item/postal-address_v2    Street Address                                            1                 Street Address                        
                            // 8      vnd.android.cursor.item/postal-address_v2    POBOX Street Address ExtAddress Reg Loc Postal Country    2                 Street Address ExtAddress    POBOX             Reg      Loc      Postal   Country
                            //
                            // | d1: Full address
                            // | d2: Address type
                            // | d3: Unknown
                            // | d4: Street address
                            // | d5: P.O. Box
                            // | d6: Extended address
                            // | d7: Region
                            // | d8: Locality (city)
                            // | d9: Postal code
                            // | d10: Country
                            // ---> will get converted to VCard line:
                            //      ADR;TYPE=(d2|d3):d5;d6;d4;d8;d7;d9;d10
                            const string _addressSpecifierWithType = "ADR;";
                            string _addressTypeName = "home";
                            switch (d2)
                            {
                                // Let's do some translation work!
                                // From Android's source code:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/base/core/java/android/provider/ContactsContract.java;l=6733
                                //
                                // public static final int TYPE_HOME = 1;
                                // public static final int TYPE_WORK = 2;
                                // public static final int TYPE_OTHER = 3;
                                //
                                // Translation to VCard code is found here:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/opt/vcard/java/com/android/vcard/VCardBuilder.java;l=1444
                                case "2":
                                    // TYPE_WORK -> work
                                    _addressTypeName = "work";
                                    break;
                                case "3":
                                    // TYPE_OTHER -> x-other
                                    _addressTypeName = $"x-other";
                                    break;
                            }
                            masterContactBuilder.AppendLine($"{_addressSpecifierWithType}TYPE={_addressTypeName}:{d5};{d6};{d4};{d8};{d7};{d9};{d10}");
                            break;
                        case "vnd.android.cursor.item/im":
                            // Example:
                            //
                            // _id    mimetype                      data1           data2    data3    data4    data5
                            // 8      vnd.android.cursor.item/im    IM              1                          0
                            // 8      vnd.android.cursor.item/im    Windows LIVE    1                          1
                            // 8      vnd.android.cursor.item/im    Yahoo           1                          2
                            //
                            // | d1: IM username
                            // | d2: IM account type
                            // | d3: Unknown
                            // | d4: Unknown
                            // | d5: IM protocol type (AOL, MSN, QQ, etc.)
                            // ---> will get converted to VCard line:
                            //      X-d5;TYPE=d2:d1
                            string _imTypeName = "home";
                            string _imSpecifier = "X-AIM";
                            switch (d2)
                            {
                                // Let's do some translation work!
                                // From Android's source code:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/base/core/java/android/provider/ContactsContract.java;l=6900
                                //
                                // public static final int TYPE_HOME = 1;
                                // public static final int TYPE_WORK = 2;
                                // public static final int TYPE_OTHER = 3;
                                //
                                // Translation to VCard code is found here:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/opt/vcard/java/com/android/vcard/VCardBuilder.java;l=1178
                                case "2":
                                    // TYPE_WORK -> work
                                    _imTypeName = "work";
                                    break;
                                case "3":
                                    // TYPE_OTHER -> x-other
                                    _imTypeName = $"x-other";
                                    break;
                            }
                            switch (d5)
                            {
                                // Additionally, there is no built-in "IM;TYPE=d2:d1" property found in any VCard standard, so we have to use the X-nonstandard
                                // properties to describe the type of IM messaging account instead.
                                // 
                                // public static final String PROPERTY_X_AIM = "X-AIM";
                                // public static final String PROPERTY_X_MSN = "X-MSN";
                                // public static final String PROPERTY_X_YAHOO = "X-YAHOO";
                                // public static final String PROPERTY_X_ICQ = "X-ICQ";
                                // public static final String PROPERTY_X_JABBER = "X-JABBER";
                                // public static final String PROPERTY_X_GOOGLE_TALK = "X-GOOGLE-TALK";
                                // public static final String PROPERTY_X_SKYPE_USERNAME = "X-SKYPE-USERNAME";
                                // public static final String PROPERTY_X_QQ = "X-QQ";
                                // public static final String PROPERTY_X_NETMEETING = "X-NETMEETING";
                                //
                                // Taken from: https://cs.android.com/android/platform/superproject/+/main:frameworks/opt/vcard/java/com/android/vcard/VCardConstants.java;l=78
                                case "1":
                                    // PROPERTY_X_MSN => X-MSN
                                    _imSpecifier = "X-MSN";
                                    break;
                                case "2":
                                    // PROPERTY_X_YAHOO => X-YAHOO
                                    _imSpecifier = "X-YAHOO";
                                    break;
                                case "3":
                                    // PROPERTY_X_SKYPE_USERNAME => X-SKYPE-USERNAME
                                    _imSpecifier = "X-SKYPE-USERNAME";
                                    break;
                                case "4":
                                    // PROPERTY_X_QQ => X-QQ
                                    _imSpecifier = "X-QQ";
                                    break;
                                case "5":
                                    // PROPERTY_X_GOOGLE_TALK => X-GOOGLE-TALK
                                    _imSpecifier = "X-GOOGLE-TALK";
                                    break;
                                case "6":
                                    // PROPERTY_X_ICQ => X-ICQ
                                    _imSpecifier = "X-ICQ";
                                    break;
                                case "7":
                                    // PROPERTY_X_JABBER => X-JABBER
                                    _imSpecifier = "X-JABBER";
                                    break;
                                case "8":
                                    // PROPERTY_X_NETMEETING => X-NETMEETING
                                    _imSpecifier = "X-NETMEETING";
                                    break;
                            }
                            masterContactBuilder.AppendLine($"{_imSpecifier};TYPE={_imTypeName}:{d1}");
                            break;
                        case "vnd.android.cursor.item/organization":
                            // Example:
                            //
                            // _id    mimetype                                data1           data2    data3    data4
                            // 8      vnd.android.cursor.item/organization    Organization    1                 Title
                            //
                            // | d1: Organization name
                            // | d2: Organization type
                            // | d3: Organization unit
                            // | d4: Your employee title or role in the organization
                            // ---> will get converted to VCard line:
                            //      ORG;TYPE=d2:d1;d3;d4
                            const string _orgSpecifierWithType = "ORG;";
                            string _orgTypeName = "work";
                            switch (d2)
                            {
                                // Let's do some translation work!
                                // From Android's source code:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/base/core/java/android/provider/ContactsContract.java;l=7120
                                //
                                // public static final int TYPE_WORK = 1;
                                // public static final int TYPE_OTHER = 2;
                                //
                                // Translation to VCard code is found here:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/opt/vcard/java/com/android/vcard/VCardBuilder.java;l=1178
                                case "2":
                                    // TYPE_OTHER -> x-other
                                    _orgTypeName = "x-other";
                                    break;
                            }
                            masterContactBuilder.AppendLine($"{_orgSpecifierWithType}TYPE={_orgTypeName}:{d1};{d3};{d4}");
                            break;
                        case "vnd.android.cursor.item/website":
                            // Example:
                            //
                            // _id    mimetype                           data1               data2
                            // 7      vnd.android.cursor.item/website    https://sso.org/    1
                            //
                            // | d1: URL to website
                            // | d2: Website type (invalid on VCard while valid on Android)
                            // ---> will get converted to VCard line:
                            //      URL:d1
                            const string _urlSpecifier = "URL:";
                            masterContactBuilder.AppendLine($"{_urlSpecifier}{d1}");
                            break;
                        case "vnd.android.cursor.item/sip_address":
                            // Example:
                            //
                            // _id    mimetype                               data1       data2
                            // 7      vnd.android.cursor.item/sip_address    sip test    3
                            //
                            // | d1: SIP address
                            // | d2: SIP address type
                            // ---> will get converted to VCard line:
                            //      IMPP;TYPE=d2:d1
                            const string _imppSpecifierWithType = "IMPP;";
                            string _imppTypeName = "home";
                            switch (d2)
                            {
                                // Let's do some translation work!
                                // From Android's source code:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/base/core/java/android/provider/ContactsContract.java;l=7711
                                //
                                // public static final int TYPE_HOME = 1;
                                // public static final int TYPE_WORK = 2;
                                // public static final int TYPE_OTHER = 3;
                                //
                                // Translation to VCard code is found here:
                                // https://cs.android.com/android/platform/superproject/+/main:frameworks/opt/vcard/java/com/android/vcard/VCardBuilder.java;l=1782
                                case "2":
                                    // TYPE_WORK -> work
                                    _imppTypeName = "work";
                                    break;
                                case "3":
                                    // TYPE_OTHER -> x-other
                                    _imppTypeName = "x-other";
                                    break;
                            }
                            masterContactBuilder.AppendLine($"{_imppSpecifierWithType}TYPE={_imppTypeName}:sip:{d1}");
                            break;
                        case "vnd.android.cursor.item/relation":
                            // Example:
                            //
                            // _id    mimetype                            data1   data2
                            // 7      vnd.android.cursor.item/relation    Name    2
                            //
                            // | d1: Relative name
                            // | d2: Relative type
                            // ---> will get converted to VCard line:
                            //      X-ANDROID-CUSTOM:vnd.android.cursor.item/relation;Name;2;;;;;;;;;;;;;
                            const string _relativeSpecifierWithType = "X-ANDROID-CUSTOM:vnd.android.cursor.item/relation;";
                            masterContactBuilder.AppendLine($"{_relativeSpecifierWithType}{d1};{d2};;;;;;;;;;;;;");
                            break;
                        case "vnd.android.cursor.item/contact_event":
                            // Example:
                            //
                            // _id    mimetype                                 data1        data2
                            // 7      vnd.android.cursor.item/contact_event    07/21/1993   1
                            //
                            // | d1: Event date
                            // | d2: Event type
                            // ---> will get converted to VCard line for birthday events:
                            //      BDAY:07/21/1993
                            // ---> will get converted to VCard line for other events:
                            //      X-ANDROID-CUSTOM:vnd.android.cursor.item/contact_event;07/21/1993;1;;;;;;;;;;;;;
                            const string _contactEventSpecifierWithType = "X-ANDROID-CUSTOM:vnd.android.cursor.item/contact_event;";
                            const string _contactEventBirthdaySpecifierWithType = "BDAY:";

                            // According to the translation done below, these are supported:
                            //
                            // public static final int TYPE_ANNIVERSARY = 1;
                            // public static final int TYPE_OTHER = 2;
                            // public static final int TYPE_BIRTHDAY = 3;
                            //
                            // Taken from https://cs.android.com/android/platform/superproject/+/main:frameworks/base/core/java/android/provider/ContactsContract.java;l=7379
                            if (d2 == "3" && !birthdaySeen)
                            {
                                // This event is a birthday, so we need not to add more than one.
                                birthdaySeen = true;
                                masterContactBuilder.AppendLine($"{_contactEventBirthdaySpecifierWithType}{d1}");
                            }
                            else
                                masterContactBuilder.AppendLine($"{_contactEventSpecifierWithType}{d1};{d2};;;;;;;;;;;;;");
                            break;
                        case "vnd.android.cursor.item/identity":
                            // Example:
                            //
                            // _id    mimetype                            data1   data2
                            // 7      vnd.android.cursor.item/identity    myid    com.aptivi
                            //
                            // | d1: Identity
                            // | d2: Identity namespace
                            // ---> will get converted to VCard line:
                            //      X-ANDROID-CUSTOM:vnd.android.cursor.item/identity;myid;com.aptivi;;;;;;;;;;;;;
                            const string _identitySpecifierWithType = "X-ANDROID-CUSTOM:vnd.android.cursor.item/identity;";
                            masterContactBuilder.AppendLine($"{_identitySpecifierWithType}{d1};{d2};;;;;;;;;;;;;");
                            break;
                        case "vnd.android.cursor.item/note":
                            // Example:
                            //
                            // _id    mimetype                        data1
                            // 8      vnd.android.cursor.item/note    Notes
                            //
                            // | d1: Notes
                            // ---> will get converted to VCard line:
                            //      NOTE:d1
                            const string _noteSpecifier = "NOTE:";
                            masterContactBuilder.AppendLine($"{_noteSpecifier}{d1}");
                            break;
                        case "vnd.android.cursor.item/photo":
                            // Example:
                            //
                            // _id    mimetype                         quote(data.data15)
                            // 3      vnd.android.cursor.item/photo    X'FFD8FFE...
                            //
                            // | d15: Encoded photo
                            // ---> will get converted to VCard line:
                            //      PHOTO;ENCODING=BLOB;JPEG:d15
                            const string _photoSpecifierWithType = "PHOTO;";
                            const int maxChars = 74;
                            const int maxCharsFirst = 48;

                            // Unquote the encoded photo
                            d15 = d15.Substring(d15.IndexOf("'") + 1);
                            d15 = d15.Substring(0, d15.IndexOf("'"));

                            // Construct the photo block
                            StringBuilder photoBlock = new();
                            int selectedMax = maxCharsFirst;
                            int processed = 0;
                            for (int currCharNum = 0; currCharNum < d15.Length; currCharNum++)
                            {
                                photoBlock.Append(d15[currCharNum]);
                                processed++;
                                if (processed >= selectedMax)
                                {
                                    // Append a new line because we reached the maximum limit
                                    selectedMax = maxChars;
                                    processed = 0;
                                    photoBlock.Append("\n ");
                                }
                            }
                            masterContactBuilder.AppendLine($"{_photoSpecifierWithType}ENCODING=BLOB;TYPE=JPEG:{photoBlock}");
                            break;
                    }

                    // Update lastId
                    lastId = id;
                }
                masterContactBuilder.AppendLine(VcardConstants._endText);

                // Now, invoke VisualCard to give us the card parsers
                return CardTools.GetCardsFromString(masterContactBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("The Android contact database file is not valid." + dbObtainTip, ex);
            }
        }
    }
}
