// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 618

using MediatorLib;
using StructureMap;
using StructureMap.Graph;

namespace EventSourcingDemo.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            ObjectFactory.Initialize(c =>
            {
                c.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });

                c.For<IQueryMediator>().Use<QueryMediator>();
                c.For<ICommandMediator>().Use<CommandMediator>();
                c.For<IValidateMediator>().Use<ValidateMediator>();
                c.For<IMediator>().Use<Mediator>();

                c.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.AddAllTypesOf(typeof (IQueryHandler<,>));
                    s.AddAllTypesOf(typeof (ICommandHandler<>));
                    s.AddAllTypesOf(typeof (IValidateHandler<>));
                });
            });

            return ObjectFactory.Container;
        }
    }
}