// <auto-generated />
#pragma warning disable CS0105
using MasterMemory.Validation;
using MasterMemory;
using MessagePack;
using Shared.Network;
using System.Collections.Generic;
using System;

namespace Shared.Network.Tables
{
   public sealed partial class ClassRoomDefinitionTable : TableBase<ClassRoomDefinition>, ITableUniqueValidate
   {
        public Func<ClassRoomDefinition, string> PrimaryKeySelector => primaryIndexSelector;
        readonly Func<ClassRoomDefinition, string> primaryIndexSelector;


        public ClassRoomDefinitionTable(ClassRoomDefinition[] sortedData)
            : base(sortedData)
        {
            this.primaryIndexSelector = x => x.Id;
            OnAfterConstruct();
        }

        partial void OnAfterConstruct();


        public ClassRoomDefinition FindById(string key)
        {
            return FindUniqueCore(data, primaryIndexSelector, System.StringComparer.Ordinal, key, false);
        }
        
        public bool TryFindById(string key, out ClassRoomDefinition result)
        {
            return TryFindUniqueCore(data, primaryIndexSelector, System.StringComparer.Ordinal, key, out result);
        }

        public ClassRoomDefinition FindClosestById(string key, bool selectLower = true)
        {
            return FindUniqueClosestCore(data, primaryIndexSelector, System.StringComparer.Ordinal, key, selectLower);
        }

        public RangeView<ClassRoomDefinition> FindRangeById(string min, string max, bool ascendant = true)
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
            return new MasterMemory.Meta.MetaTable(typeof(ClassRoomDefinition), typeof(ClassRoomDefinitionTable), "ClassRoomDefinition",
                new MasterMemory.Meta.MetaProperty[]
                {
                    new MasterMemory.Meta.MetaProperty(typeof(ClassRoomDefinition).GetProperty("Id")),
                    new MasterMemory.Meta.MetaProperty(typeof(ClassRoomDefinition).GetProperty("TeacherSeatPosition")),
                    new MasterMemory.Meta.MetaProperty(typeof(ClassRoomDefinition).GetProperty("TeacherSeatRotation")),
                    new MasterMemory.Meta.MetaProperty(typeof(ClassRoomDefinition).GetProperty("StartCenterSeatPosition")),
                    new MasterMemory.Meta.MetaProperty(typeof(ClassRoomDefinition).GetProperty("MaxColPerRow")),
                    new MasterMemory.Meta.MetaProperty(typeof(ClassRoomDefinition).GetProperty("MinGenerateSeats")),
                    new MasterMemory.Meta.MetaProperty(typeof(ClassRoomDefinition).GetProperty("RowSpace")),
                    new MasterMemory.Meta.MetaProperty(typeof(ClassRoomDefinition).GetProperty("ColSpace")),
                },
                new MasterMemory.Meta.MetaIndex[]{
                    new MasterMemory.Meta.MetaIndex(new System.Reflection.PropertyInfo[] {
                        typeof(ClassRoomDefinition).GetProperty("Id"),
                    }, true, true, System.StringComparer.Ordinal),
                });
        }

#endif
    }
}