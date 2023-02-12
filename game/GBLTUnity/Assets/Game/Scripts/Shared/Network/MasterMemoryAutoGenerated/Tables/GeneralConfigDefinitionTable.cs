// <auto-generated />
#pragma warning disable CS0105
using MasterMemory.Validation;
using MasterMemory;
using MemoryPack;
using Shared.Network;
using System.Collections.Generic;
using System;

namespace Shared.Network.Tables
{
   public sealed partial class GeneralConfigDefinitionTable : TableBase<GeneralConfigDefinition>, ITableUniqueValidate
   {
        public Func<GeneralConfigDefinition, string> PrimaryKeySelector => primaryIndexSelector;
        readonly Func<GeneralConfigDefinition, string> primaryIndexSelector;


        public GeneralConfigDefinitionTable(GeneralConfigDefinition[] sortedData)
            : base(sortedData)
        {
            this.primaryIndexSelector = x => x.Id;
            OnAfterConstruct();
        }

        partial void OnAfterConstruct();


        public GeneralConfigDefinition FindById(string key)
        {
            return FindUniqueCore(data, primaryIndexSelector, System.StringComparer.Ordinal, key, false);
        }
        
        public bool TryFindById(string key, out GeneralConfigDefinition result)
        {
            return TryFindUniqueCore(data, primaryIndexSelector, System.StringComparer.Ordinal, key, out result);
        }

        public GeneralConfigDefinition FindClosestById(string key, bool selectLower = true)
        {
            return FindUniqueClosestCore(data, primaryIndexSelector, System.StringComparer.Ordinal, key, selectLower);
        }

        public RangeView<GeneralConfigDefinition> FindRangeById(string min, string max, bool ascendant = true)
        {
            return FindUniqueRangeCore(data, primaryIndexSelector, System.StringComparer.Ordinal, min, max, ascendant);
        }


        void ITableUniqueValidate.ValidateUnique(ValidateResult resultSet)
        {
#if !DISABLE_MASTERMEMORY_VALIDATOR

            ValidateUniqueCore(data, primaryIndexSelector, "Id", resultSet);       

#endif
        }

#if !DISABLE_MASTERMEMORY_METADATABASE

        public static MasterMemory.Meta.MetaTable CreateMetaTable()
        {
            return new MasterMemory.Meta.MetaTable(typeof(GeneralConfigDefinition), typeof(GeneralConfigDefinitionTable), "GeneralConfig",
                new MasterMemory.Meta.MetaProperty[]
                {
                    new MasterMemory.Meta.MetaProperty(typeof(GeneralConfigDefinition).GetProperty("Id")),
                },
                new MasterMemory.Meta.MetaIndex[]{
                    new MasterMemory.Meta.MetaIndex(new System.Reflection.PropertyInfo[] {
                        typeof(GeneralConfigDefinition).GetProperty("Id"),
                    }, true, true, System.StringComparer.Ordinal),
                });
        }

#endif
    }
}