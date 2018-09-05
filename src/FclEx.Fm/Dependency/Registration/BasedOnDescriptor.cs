﻿// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FclEx.Fm.Dependency.Registration
{
    /// <summary>
	///   Describes how to register a group of related types.
	/// </summary>
	public class BasedOnDescriptor : IHasTypes
    {
        private readonly IEnumerable<Type> _types;
        private readonly Type _baseType;

        public BasedOnDescriptor(IHasTypes hasTypes, Type baseType)
        {
            _types = hasTypes.GetTypes();
            _baseType = baseType;
        }

        public IEnumerable<Type> GetTypes()
        {
            return _types.Where(m => _baseType.IsAssignableFrom(m));
        }
    }
}