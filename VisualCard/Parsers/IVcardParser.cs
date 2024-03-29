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
using VisualCard.Parts;

namespace VisualCard.Parsers
{
    /// <summary>
    /// VCard parser interface
    /// </summary>
    internal interface IVcardParser
    {
        /// <summary>
        /// Contents of the VCard
        /// </summary>
        string CardContent { get; }

        /// <summary>
        /// The version of the card
        /// </summary>
        Version CardVersion { get; }

        /// <summary>
        /// VCard expected card version
        /// </summary>
        Version ExpectedCardVersion { get; }

        /// <summary>
        /// Parses the VCard file
        /// </summary>
        Card Parse();
    }
}
