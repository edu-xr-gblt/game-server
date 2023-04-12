// <auto-generated />
#pragma warning disable CS0105
using MasterMemory.Validation;
using MasterMemory;
using MemoryPack;
using Shared.Network;
using System.Collections.Generic;
using System;
using Shared.Network.Tables;

namespace Shared.Network
{
   public sealed class DatabaseBuilder : DatabaseBuilderBase
   {
        public DatabaseBuilder() : this(null) { }
        public DatabaseBuilder(MessagePack.IFormatterResolver resolver) : base(resolver) { }

        public DatabaseBuilder Append(System.Collections.Generic.IEnumerable<GeneralConfigDefinition> dataSource)
        {
            AppendCore(dataSource, x => x.Id, System.StringComparer.Ordinal);
            return this;
        }

    }
}