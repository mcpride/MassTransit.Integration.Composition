// Copyright 2014, 2015 Marco Stolze
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("MassTransit.Integration.Composition")]
[assembly: AssemblyDescription("MEF composition support for MassTransit .NET distributed application framework http://masstransit-project.com")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Marco Stolze")]
[assembly: AssemblyProduct("MassTransit.Integration.Composition")]
[assembly: AssemblyCopyright("Copyright © Marco Stolze 2014, 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: ComVisible(false)]
[assembly: Guid("3f6ee35f-17c4-4be9-9180-80985dc083da")]

[assembly: AssemblyVersion("1.0.5.0")]
[assembly: AssemblyFileVersion("1.0.5.0")]
