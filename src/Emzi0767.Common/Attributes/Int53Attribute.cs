﻿// This file is a part of Emzi0767.Common project.
// 
// Copyright 2019 Emzi0767
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace System.Runtime.Serialization
{
    /// <summary>
    /// <para>Specifies that this 64-bit integer uses no more than 53 bits to represent its value.</para>
    /// <para>This is used to indicate that large numbers are safe for direct serialization into formats which do support 64-bit integers natively (such as JSON).</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class Int53Attribute : Attribute
    { }
}