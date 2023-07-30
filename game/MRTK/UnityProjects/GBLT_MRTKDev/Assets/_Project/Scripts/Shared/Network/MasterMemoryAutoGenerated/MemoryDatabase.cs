// <auto-generated />
#pragma warning disable CS0105
using MasterMemory.Validation;
using MasterMemory;
using MessagePack;
using Shared.Network;
using System.Collections.Generic;
using System;
using Shared.Network.Tables;

namespace Shared.Network
{
   public sealed class MemoryDatabase : MemoryDatabaseBase
   {
        public ClassRoomDefinitionTable ClassRoomDefinitionTable { get; private set; }
        public GeneralConfigDefinitionTable GeneralConfigDefinitionTable { get; private set; }

        public MemoryDatabase(
            ClassRoomDefinitionTable ClassRoomDefinitionTable,
            GeneralConfigDefinitionTable GeneralConfigDefinitionTable
        )
        {
            this.ClassRoomDefinitionTable = ClassRoomDefinitionTable;
            this.GeneralConfigDefinitionTable = GeneralConfigDefinitionTable;
        }

        public MemoryDatabase(byte[] databaseBinary, bool internString = true, MessagePack.IFormatterResolver formatterResolver = null, int maxDegreeOfParallelism = 1)
            : base(databaseBinary, internString, formatterResolver, maxDegreeOfParallelism)
        {
        }

        protected override void Init(Dictionary<string, (int offset, int count)> header, System.ReadOnlyMemory<byte> databaseBinary, MessagePack.MessagePackSerializerOptions options, int maxDegreeOfParallelism)
        {
            if(maxDegreeOfParallelism == 1)
            {
                InitSequential(header, databaseBinary, options, maxDegreeOfParallelism);
            }
            else
            {
                InitParallel(header, databaseBinary, options, maxDegreeOfParallelism);
            }
        }

        void InitSequential(Dictionary<string, (int offset, int count)> header, System.ReadOnlyMemory<byte> databaseBinary, MessagePack.MessagePackSerializerOptions options, int maxDegreeOfParallelism)
        {
            this.ClassRoomDefinitionTable = ExtractTableData<ClassRoomDefinition, ClassRoomDefinitionTable>(header, databaseBinary, options, xs => new ClassRoomDefinitionTable(xs));
            this.GeneralConfigDefinitionTable = ExtractTableData<GeneralConfigDefinition, GeneralConfigDefinitionTable>(header, databaseBinary, options, xs => new GeneralConfigDefinitionTable(xs));
        }

        void InitParallel(Dictionary<string, (int offset, int count)> header, System.ReadOnlyMemory<byte> databaseBinary, MessagePack.MessagePackSerializerOptions options, int maxDegreeOfParallelism)
        {
            var extracts = new Action[]
            {
                () => this.ClassRoomDefinitionTable = ExtractTableData<ClassRoomDefinition, ClassRoomDefinitionTable>(header, databaseBinary, options, xs => new ClassRoomDefinitionTable(xs)),
                () => this.GeneralConfigDefinitionTable = ExtractTableData<GeneralConfigDefinition, GeneralConfigDefinitionTable>(header, databaseBinary, options, xs => new GeneralConfigDefinitionTable(xs)),
            };
            
            System.Threading.Tasks.Parallel.Invoke(new System.Threading.Tasks.ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            }, extracts);
        }

        public ImmutableBuilder ToImmutableBuilder()
        {
            return new ImmutableBuilder(this);
        }

        public DatabaseBuilder ToDatabaseBuilder()
        {
            var builder = new DatabaseBuilder();
            builder.Append(this.ClassRoomDefinitionTable.GetRawDataUnsafe());
            builder.Append(this.GeneralConfigDefinitionTable.GetRawDataUnsafe());
            return builder;
        }

        public DatabaseBuilder ToDatabaseBuilder(MessagePack.IFormatterResolver resolver)
        {
            var builder = new DatabaseBuilder(resolver);
            builder.Append(this.ClassRoomDefinitionTable.GetRawDataUnsafe());
            builder.Append(this.GeneralConfigDefinitionTable.GetRawDataUnsafe());
            return builder;
        }

#if !DISABLE_MASTERMEMORY_VALIDATOR

        public ValidateResult Validate()
        {
            var result = new ValidateResult();
            var database = new ValidationDatabase(new object[]
            {
                ClassRoomDefinitionTable,
                GeneralConfigDefinitionTable,
            });

            ((ITableUniqueValidate)ClassRoomDefinitionTable).ValidateUnique(result);
            ValidateTable(ClassRoomDefinitionTable.All, database, "Id", ClassRoomDefinitionTable.PrimaryKeySelector, result);
            ((ITableUniqueValidate)GeneralConfigDefinitionTable).ValidateUnique(result);
            ValidateTable(GeneralConfigDefinitionTable.All, database, "Id", GeneralConfigDefinitionTable.PrimaryKeySelector, result);

            return result;
        }

#endif

        static MasterMemory.Meta.MetaDatabase metaTable;

        public static object GetTable(MemoryDatabase db, string tableName)
        {
            switch (tableName)
            {
                case "ClassRoomDefinition":
                    return db.ClassRoomDefinitionTable;
                case "GeneralConfig":
                    return db.GeneralConfigDefinitionTable;
                
                default:
                    return null;
            }
        }

#if !DISABLE_MASTERMEMORY_METADATABASE

        public static MasterMemory.Meta.MetaDatabase GetMetaDatabase()
        {
            if (metaTable != null) return metaTable;

            var dict = new Dictionary<string, MasterMemory.Meta.MetaTable>();
            dict.Add("ClassRoomDefinition", Shared.Network.Tables.ClassRoomDefinitionTable.CreateMetaTable());
            dict.Add("GeneralConfig", Shared.Network.Tables.GeneralConfigDefinitionTable.CreateMetaTable());

            metaTable = new MasterMemory.Meta.MetaDatabase(dict);
            return metaTable;
        }

#endif
    }
}