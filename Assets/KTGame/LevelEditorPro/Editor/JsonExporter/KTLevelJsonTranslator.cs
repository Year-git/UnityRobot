using System.Collections.Generic;
using UnityEngine;
using KTEditor.LevelEditor;
using System.IO;
using System.Reflection;
using System.Linq;
using Sirenix.OdinInspector;

public class KTLevelJsonTranslator
{
    #region LinkInfo
    public class LinkInfo
    {
        public object instance;
        public FieldInfo field;
        public object address;
        public LinkInfo(object instance, FieldInfo field, object address)
        {
            this.instance = instance;
            this.field = field;
            this.address = address;
        }
    }
    #endregion

    #region MappingInfo
    public class MappingInfo
    {
        public KTLevelJsonTranslator context;
        public System.Type entityType;
        public System.Type mapType;
        public ExportInfo exportInfo;
        public System.Collections.IList entityList;
        public System.Func<KTLevelEntity> CreateEntity;

        public MappingInfo(
            KTLevelJsonTranslator context,
            System.Type entityType,
            System.Type mapType,
            System.Collections.IList entityList,
            ExportInfo exportInfo,
            System.Func<KTLevelEntity> CreateEntity)
        {
            this.context = context;
            this.entityType = entityType;
            this.mapType = mapType;
            this.entityList = entityList;
            this.exportInfo = exportInfo;
            this.CreateEntity = CreateEntity;
        }

        public void CalculateRange(ref float minX, ref float minZ, ref float maxX, ref float maxZ)
        {
            foreach (var entity in entityList)
            {
                Vector3 pos = (entity as KTLevelEntity).transform.position;
                if (pos.x < minX)
                    minX = pos.x;

                if (pos.x > maxX)
                    maxX = pos.x;

                if (pos.z < minZ)
                    minZ = pos.z;

                if (pos.z > maxZ)
                    maxZ = pos.z;
            }
        }

        public void CollectEntities()
        {
            var entities = GameObject.FindObjectsOfType(entityType);
            foreach (var entity in entities)
            {
                if (entity.GetType() == entityType)
                {
                    var levelEntity = entity as KTLevelEntity;
                    levelEntity.PreExport();

                    entityList.Add(entity);
                }
            }
        }

        public void Export()
        {
            foreach (var ent in entityList)
            {
                if (ent.GetType() != entityType)
                {
                    continue;
                }
                var entity = (KTLevelEntity)ent;
                var export = (NodeData)System.Activator.CreateInstance(mapType);
                var idx = exportInfo.mapList.Count;
                ExportNode(export, entity.gameObject, idx);
                ExportClass(entity, export);
                exportInfo.mapList.Add(export);
            }
        }

        public void PostExport()
        {
            foreach (var ent in entityList)
            {
                var entity = (KTLevelEntity)ent;
                entity.PostExport();
            }
        }

        private void ExportNode(NodeData export, GameObject go, int idx)
        {
            var trans = go.transform;
            export.localPosition = trans.localPosition;
            export.localRotation = trans.localRotation.eulerAngles;
            export.localScale = trans.localScale;
            export.name = go.name;
            export.nodeType = exportInfo.nodeType;
            export.worldPosition = trans.position;
            export.worldRotation = trans.rotation.eulerAngles;
            export.worldScale = trans.lossyScale;

            var sb = KTStringBuilderCache.Acquire();
            sb.Append(exportInfo.fieldName);
            sb.Append("##");
            sb.Append(idx);
            export.addr = KTStringBuilderCache.GetStringAndRelease(sb);
        }


        #region IsXXXType
        private bool IsBaseType(System.Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        private bool IsLink(FieldInfo info)
        {
            return info.FieldType == typeof(string) && info.Name.StartsWith("__");
        }

        private bool IsNormalStringField(FieldInfo f)
        {
            return f.FieldType == typeof(string) && !f.Name.StartsWith("__");
        }

        private bool IsEntity(System.Type type)
        {
            return type.IsSubclassOf(typeof(KTLevelEntity));
        }

        private bool IsEntityListField(System.Type type)
        {
            var listType = GetIListIterfaceType(type);
            if (listType == null)
            {
                return false;
            }

            var valueType = listType.GetGenericArguments()[0];

            return valueType.IsSubclassOf(typeof(KTLevelEntity));
        }
        #endregion


        #region Convert
        private object ConvertSingleValue(System.Type srcType, object src, System.Type dstType, bool isUUIDField)
        {
            object value = null;
            if (IsBaseType(srcType))
            {
                value = ConvertBaseType(src, dstType);
            }
            else if (IsEntity(srcType))
            {
                if (isUUIDField)
                {
                    value = ConvertEntityToUUID(src);
                }
                else
                {
                    value = ConvertEntityToLocalLink(src);
                }
            }
            else
            {
                value = ConvertClass(src, dstType);
            }
            return value;
        }

        private object ConvertClass(object src, System.Type dstType)
        {
            var value = System.Activator.CreateInstance(dstType);
            ExportClass(src, value);
            return value;
        }

        private string ConvertEntityToUUID(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            return ((KTLevelEntity)obj).uuid;
        }

        private string ConvertEntityToLocalLink(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            var objType = obj.GetType();
            var mappingInfo = context.GetMappingInfo(objType);
            var entityList = mappingInfo.entityList;
            var idx = entityList.IndexOf(obj);
            if (idx < 0)
            {
                return "";
            }

            var exportType = mappingInfo.mapType;
            var exportInfo = context.GetExportInfo(exportType);
            var listName = exportInfo.fieldName;
            return string.Format("{0}##{1}", listName, idx);
        }

        private object ConvertBaseType(object obj, System.Type dstType)
        {
            if (obj == null)
            {
                return null;
            }

            var objType = obj.GetType();
            if (objType == dstType)
            {
                return obj;
            }

            if (dstType == typeof(int))
            {
                return (int)obj;
            }

            //! 枚举可以强转
            if (dstType.IsEnum)
            {
                int value;
                if (objType == typeof(bool))
                {
                    value = (bool)obj ? 1 : 0;
                }
                else
                {
                    value = (int)obj;
                }
                return System.Enum.ToObject(dstType, value);
            }

            //! Bool也可以强转
            if (dstType == typeof(bool))
            {
                return (int)obj != 0;
            }

            Debug.LogErrorFormat("无法转换的类型 src={0}, dst={1}", objType, dstType);
            return null;
        }
        #endregion

        #region Export
        private void ExportClass(object src, object dst)
        {
            var srcType = src.GetType();
            var fields = srcType.GetFields();
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                var fieldValue = field.GetValue(src);

                if (!NeedExport(src, field))
                {
                    continue;
                }

                if (GetIListIterfaceType(fieldType) != null)
                {
                    ExportList(field, fieldValue, dst);
                }
                else if (!IsEntity(fieldType) && !IsBaseType(fieldType) && !IsEntityListField(fieldType))
                {
                    ExportClass(fieldValue, dst);
                }
                else
                {
                    ExportSingleValue(field, fieldValue, dst);
                }
            }
        }

        private bool IsUUIDField(string fieldName)
        {
            return fieldName.EndsWith("_uuid");
        }

        private void ExportSingleValue(FieldInfo srcField, object srcFieldValue, object dstInst)
        {
            var dstFields = GetExportFields(srcField, dstInst.GetType());
            if (dstFields == null)
            {
                return;
            }

            foreach (var dstField in dstFields)
            {
                if (dstField == null)
                    Debug.LogFormat("{0}字段未定义在ItemDate中", srcField.Name);

                object value = ConvertSingleValue(srcField.FieldType, srcFieldValue, dstField.FieldType, IsUUIDField(dstField.Name));
                dstField.SetValue(dstInst, value);
            }
        }

        /// <summary>
        /// by lijunfeng 2018/6/26 修改了对list内元素的递归解析，主要用于解析group
        /// </summary>
        /// <param name="srcField"></param>
        /// <param name="srcFieldValue"></param>
        /// <param name="dstInst"></param>
        private void ExportList(FieldInfo srcField, object srcFieldValue, object dstInst)
        {
            var dstFields = GetExportFields(srcField, dstInst.GetType());
            if (dstFields == null)
            {
                return;
            }

            foreach (var dstField in dstFields)
            {
                ExportListItems(dstField, srcFieldValue, dstInst);
            }
        }

        /// <summary>
        /// 递归导出group组
        /// by lijunfeng 2018/6/26
        /// </summary>
        /// <param name="dstField"></param>
        /// <param name="srcFieldValue"></param>
        /// <param name="dstInst"></param>
        private void ExportListItems(FieldInfo dstField, object srcFieldValue, object dstInst)
        {
            var isUUIDField = IsUUIDField(dstField.Name);
            var dstList = (System.Collections.IList)dstField.GetValue(dstInst);
            var dstItemType = dstList.GetType().GetGenericArguments()[0];

            foreach (var item in (System.Collections.IList)srcFieldValue)
            {
                if (item == null)
                {
                    Debug.LogErrorFormat("List item {0} is Null", dstField.Name);
                    continue;
                }

                var itemAttr = KTEditorUtils.GetAttribute<KTLevelGroupAttribute>(item.GetType());
                if (itemAttr != null)
                {
                    foreach (var groupField in itemAttr.fields)
                    {
                        FieldInfo itemFieldInfo = item.GetType().GetField(groupField, BindingFlags.Public | BindingFlags.Instance);
                        if (itemFieldInfo != null)
                        {
                            ExportListItems(dstField, itemFieldInfo.GetValue(item), dstInst);
                        }
                        else
                        {
                            Debug.LogFormat(string.Format("Can not find KTLevelGroup field {0} in {1}", groupField, KTUtils.GetDisplayName(item)));
                        }

                    }

                    continue;
                }

                object dstItem = ConvertSingleValue(item.GetType(), item, dstItemType, isUUIDField);
                if (!dstList.Contains(dstItem)) //主要去重 uuid和链接
                {
                    dstList.Add(dstItem);
                }
                else
                {
                    Debug.LogErrorFormat("List item named {0} is repeated in {1}", dstItem.ToString(), dstField.Name);
                }
            }
        }

        #endregion

        #region Import
        public void Import(KTLevelEntity entity, NodeData mapData)
        {
            entityList.Add(entity);
            ImportNode(mapData, entity);
            ImportClass(mapData, entity);
        }

        private void ImportNode(NodeData node, KTLevelEntity entity)
        {
            var trans = entity.transform;
            trans.localPosition = node.localPosition;
            trans.localRotation = Quaternion.Euler(node.localRotation);
            trans.localScale = node.localScale;
            entity.gameObject.name = node.name;
        }

        private void ImportClass(object src, object dst)
        {
            ImportBaseTypes(src, dst);
            ImportClasses(src, dst);
        }

        private void ImportClasses(object src, object dst)
        {
            var dstClassFields = GetImportClassesFields(dst.GetType());
            foreach (var dstClassField in dstClassFields)
            {
                if (!NeedExport(dst, dstClassField))
                {
                    continue;
                }

                var dstClassFieldType = dstClassField.FieldType;
                if (IsEntity(dstClassFieldType) || IsEntityListField(dstClassFieldType))
                {
                    var exportField = GetExportField(dstClassField, src.GetType());
                    var address = exportField.GetValue(src);
                    context.RegisterLink(new LinkInfo(dst, dstClassField, address));
                }
                else if (GetIListIterfaceType(dstClassFieldType) != null)
                {
                    // HACKING
                    if (dst.GetType() == typeof(KTLevelFightStage))
                    {
                        continue;
                    }

                    var list = (System.Collections.IList)System.Activator.CreateInstance(dstClassFieldType);
                    var dstClassItemType = dstClassFieldType.GetGenericArguments()[0];
                    var exportField = GetExportField(dstClassField, src.GetType());
                    var srcList = (System.Collections.IList)exportField.GetValue(src);

                    foreach (var srcItem in srcList)
                    {
                        var dstItemType = GetEntityTypeFromMapItem(srcItem);
                        object dstClassItem;

                        //! HACKING, So far so good
                        if (dstClassItemType.IsSubclassOf(typeof(KTLevelTriggerNode)))
                        {
                            var trigger = (KTLevelTrigger)dst;
                            dstClassItem = trigger.CreateTriggerNode(dstItemType);
                        }
                        else
                        {
                            dstClassItem = System.Activator.CreateInstance(dstItemType);
                        }

                        list.Add(dstClassItem);
                        ImportClass(srcItem, dstClassItem);
                    }

                    dstClassField.SetValue(dst, list);
                }
                else
                {
                    var dstClassValue = System.Activator.CreateInstance(dstClassFieldType);
                    dstClassField.SetValue(dst, dstClassValue);
                    ImportClass(src, dstClassValue);
                }
            }
        }

        private void ImportBaseTypes(object src, object dst)
        {
            var srcType = src.GetType();
            var dstFields = GetImportBaseTypeFields(dst.GetType());
            foreach (var dstField in dstFields)
            {
                var srcField = GetExportField(dstField, srcType);
                if (srcField != null)
                {
                    dstField.SetValue(dst, ConvertBaseType(srcField.GetValue(src), dstField.FieldType));
                }
            }
        }
        #endregion

        private IEnumerable<FieldInfo> GetLinkFields(System.Type t)
        {
            return t.GetFields().Where(f => IsLink(f));
        }

        private object GetFieldValue(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName);
            if (field == null)
            {
                return null;
            }

            return field.GetValue(obj);
        }

        private System.Type GetEntityTypeFromMapItem(object mapItem)
        {
            if (mapItem.GetType() == typeof(TriggerCondition))
            {
                return context.EnumTypeToClassType(GetFieldValue(mapItem, "conditionType"));
            }
            else if (mapItem.GetType() == typeof(TriggerResult))
            {
                return context.EnumTypeToClassType(GetFieldValue(mapItem, "resultType"));
            }

            return exportInfo.GetEntityType(mapItem);
        }

        private IEnumerable<FieldInfo> GetImportClassesFields(System.Type type)
        {
            return type.GetFields()
                .Where(f => !f.IsStatic)
                .Where(f => f.FieldType.IsClass && !IsNormalStringField(f));
        }

        private IEnumerable<FieldInfo> GetImportBaseTypeFields(System.Type type)
        {
            return type.GetFields()
                .Where(f => !f.IsStatic)
                .Where(f => f.FieldType.IsValueType || IsNormalStringField(f));
        }

        private static List<FieldInfo> GetExportFields(FieldInfo field, System.Type dstObjType)
        {
            var attrs = field.GetCustomAttributes(typeof(KTLevelExportAttribute), false);
            if (attrs == null)
            {
                return null;
            }

            return attrs.Select(attr =>
            {
                var exportInfo = (KTLevelExportAttribute)attr;
                return dstObjType.GetField(exportInfo.field);
            }).ToList();
        }

        private static FieldInfo GetExportField(FieldInfo field, System.Type dstObjType)
        {
            var exportAttr = KTEditorUtils.GetAttribute<KTLevelExportAttribute>(field);
            if (exportAttr == null)
            {
                return null;
            }

            return dstObjType.GetField(exportAttr.field);
        }

        private static System.Type GetIListIterfaceType(System.Type type)
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return iface;
                }
            }
            return null;
        }

        private static MemberInfo FindInheritMemeber(System.Type type, string name)
        {
            if (type == null)
            {
                return null;
            }

            var member = type.GetMember(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();
            if (member != null)
            {
                return member;
            }
            return FindInheritMemeber(type.BaseType, name);
        }

        private static object GetMember(object obj, MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    {
                        var field = (FieldInfo)member;
                        return field.GetValue(obj);
                    }
                case MemberTypes.Property:
                    {
                        var property = (PropertyInfo)member;
                        return property.GetValue(obj, null);
                    }
                case MemberTypes.Method:
                    {
                        var method = (MethodInfo)member;
                        return method.Invoke(obj, null);
                    }
            }
            return null;
        }

        private static bool NeedExport(object obj, FieldInfo field)
        {
            var showIfAttr = KTEditorUtils.GetAttribute<ShowIfAttribute>(field);
            if (showIfAttr == null)
            {
                return true;
            }

            var member = FindInheritMemeber(obj.GetType(), showIfAttr.MemberName);
            if (member == null)
            {
                return false;
            }


            return (bool)GetMember(obj, member);
        }
    }
    #endregion

    #region ExportInfo
    public class ExportInfo
    {
        public KTLevelJsonTranslator context;
        public System.Type mapType;
        public string fieldName;
        public ItemNodeType nodeType;
        public System.Collections.IList mapList;
        public System.Func<object, System.Type> GetEntityType;

        public ExportInfo(
            KTLevelJsonTranslator context,
            System.Type mapType,
            string fieldName,
            ItemNodeType nodeType,
            System.Collections.IList mapList,
            System.Func<object, System.Type> GetEntityType)
        {
            this.context = context;
            this.mapType = mapType;
            this.fieldName = fieldName;
            this.nodeType = nodeType;
            this.mapList = mapList;
            this.GetEntityType = GetEntityType;
        }
    }
    #endregion

    private KTLevel _level;
    private JsonData _jsonData;
    private List<KTLevelSpawnPoint> _spawnPoints;
    private List<KTLevelPoint> _points;
    private List<KTLevelSpawner> _spawners;
    private List<KTLevelTrigger> _triggers;
    private List<KTLevelTriggerClient> _clientTriggers;
    private List<KTLevelPatrolPoint> _patrolPoints;
    private List<KTLevelPatrolPath> _patrolPaths;
    private List<KTLevelPatrolZone> _patrolZones;
    private List<KTLevelFightStage> _fightStages;

    private List<MappingInfo> _mappingInfos;
    private Dictionary<System.Type, MappingInfo> _entityTypeToMappingInfo;
    private List<ExportInfo> _exportInfos;
    private Dictionary<System.Type, ExportInfo> _mapTypeToExportInfo;

    private Dictionary<string, KTLevelEntity> _addrToEntity;
    private List<LinkInfo> _linkInfos;
    private Dictionary<object, System.Type> _enumTypeToClassType;

    public KTLevelJsonTranslator(KTLevel level)
    {
        this._level = level;
        Reset(null);
    }

    #region Reset
    private void AddMapping<ET, MT>(System.Collections.IList entityList, System.Func<KTLevelEntity> CreateEntity)
    {
        var entityType = typeof(ET);
        var mapType = typeof(MT);
        var info = new MappingInfo(
            this,
            entityType,
            mapType,
            entityList,
            _mapTypeToExportInfo[mapType],
            CreateEntity);
        _mappingInfos.Add(info);
        _entityTypeToMappingInfo.Add(entityType, info);
    }

    public MappingInfo GetMappingInfo(System.Type entityType)
    {
        MappingInfo result;
        if (_entityTypeToMappingInfo.TryGetValue(entityType, out result))
        {
            return result;
        }
        return null;
    }

    public ExportInfo GetExportInfo(System.Type mapType)
    {
        ExportInfo result;
        if (_mapTypeToExportInfo.TryGetValue(mapType, out result))
        {
            return result;
        }
        return null;
    }

    private void AddExportInfo<MT>(string fieldName, ItemNodeType itemNodeType, System.Func<object, System.Type> GetEntityType)
    {
        var mapType = typeof(MT);
        var listField = _jsonData.GetType().GetField(fieldName);
        var mapList = (System.Collections.IList)listField.GetValue(_jsonData);
        var info = new ExportInfo(
            this,
            mapType,
            fieldName,
            itemNodeType,
            mapList,
            GetEntityType);
        _exportInfos.Add(info);
        _mapTypeToExportInfo.Add(mapType, info);
    }

    public void Reset(JsonData jsonData)
    {
        _jsonData = jsonData ?? new JsonData();
        _spawnPoints = new List<KTLevelSpawnPoint>();
        _points = new List<KTLevelPoint>();
        _spawners = new List<KTLevelSpawner>();
        _triggers = new List<KTLevelTrigger>();
        _clientTriggers = new List<KTLevelTriggerClient>();
        _patrolPoints = new List<KTLevelPatrolPoint>();
        _patrolPaths = new List<KTLevelPatrolPath>();
        _patrolZones = new List<KTLevelPatrolZone>();
        _fightStages = new List<KTLevelFightStage>();

        _mappingInfos = new List<MappingInfo>();
        _entityTypeToMappingInfo = new Dictionary<System.Type, MappingInfo>();
        _exportInfos = new List<ExportInfo>();
        _mapTypeToExportInfo = new Dictionary<System.Type, ExportInfo>();

        _addrToEntity = new Dictionary<string, KTLevelEntity>();
        _linkInfos = new List<LinkInfo>();
        _enumTypeToClassType = new Dictionary<object, System.Type>();

        LoadAllEnumTypeToClassType();

        AddExportInfo<MonsterPoint>("monsters", ItemNodeType.monster, obj => typeof(KTLevelPoint));
        AddExportInfo<BornLocation>("bornLocations", ItemNodeType.born_location, obj => typeof(KTLevelSpawnPoint));
        AddExportInfo<PatrolPoint>("patrolPoints", ItemNodeType.patrol_point, obj => typeof(KTLevelPatrolPoint));
        AddExportInfo<PatrolPath>("patrolPaths", ItemNodeType.patrol_path, obj => typeof(KTLevelPatrolPath));
        AddExportInfo<PatrolArea>("patrolAreas", ItemNodeType.patrol_area, obj => typeof(KTLevelPatrolZone));
        AddExportInfo<MonsterGenerator>("monsterGenerators", ItemNodeType.monster_generator, obj =>
        {
            var gen = (MonsterGenerator)obj;
			if (gen.generatorType == GeneratorType.item)
			{
				return typeof(KTLevelItemSpawner);
			}
			else if (gen.generatorType == GeneratorType.interactionItem)
			{
				return typeof(KTLevelInteractionItemSpawner);
			}
			else if (gen.generatorType == GeneratorType.battleUnit)
			{
				return typeof(KTLevelBattleUnitSpawner);
			}
            return null;
        });
        AddExportInfo<Trigger>("triggers", ItemNodeType.trigger, obj => typeof(KTLevelTrigger));
        AddExportInfo<ClientTrigger>("clientTriggers", ItemNodeType.trigger, obj => typeof(KTLevelTriggerClient));
        AddExportInfo<FightStage>("fightStages", ItemNodeType.fight_stage, obj => typeof(KTLevelFightStage));

        AddMapping<KTLevelSpawnPoint, BornLocation>(_spawnPoints, _level.NewSpawnPoint);
        AddMapping<KTLevelPoint, MonsterPoint>(_points, _level.NewPoint);
        AddMapping<KTLevelTrigger, Trigger>(_triggers, _level.NewServerTrigger);
        AddMapping<KTLevelTriggerClient, ClientTrigger>(_clientTriggers, _level.NewClientTrigger);
        AddMapping<KTLevelPatrolZone, PatrolArea>(_patrolZones, _level.NewPatrolZone);
        AddMapping<KTLevelPatrolPath, PatrolPath>(_patrolPaths, _level.NewPatrolPath);
        AddMapping<KTLevelPatrolPoint, PatrolPoint>(_patrolPoints, _level.NewPatrolPoint);
        AddMapping<KTLevelFightStage, FightStage>(_fightStages, _level.NewFightStage);
        AddMapping<KTLevelItemSpawner, MonsterGenerator>(_spawners, _level.NewItemSpawner);
        AddMapping<KTLevelBattleUnitSpawner, MonsterGenerator>(_spawners, _level.NewBattleUnitSpawner);
        AddMapping<KTLevelInteractionItemSpawner, MonsterGenerator>(_spawners, _level.NewInteractionItemSpawner);
    }
    #endregion

    #region EnumTypeToClassType
    private void LoadEnumTypeToClassType(System.Type type)
    {
        var subClasses = KTUtils.GetAllSubclass(type);
        foreach (var cls in subClasses)
        {
            var attr = KTEditorUtils.GetAttribute<KTLevelClassTypeAttribute>(cls);
            if (attr != null)
            {
                if (_enumTypeToClassType.ContainsKey(attr.type))
                {
                    Debug.LogErrorFormat("Key Already Exist, {0}", attr.type);
                }
                _enumTypeToClassType.Add(attr.type, cls);
            }
        }
    }

    private void LoadAllEnumTypeToClassType()
    {
        LoadEnumTypeToClassType(typeof(KTLevelTriggerCondition));
        LoadEnumTypeToClassType(typeof(KTLevelTriggerAction));
    }

    public System.Type EnumTypeToClassType(object type)
    {
        System.Type result;
        if (_enumTypeToClassType.TryGetValue(type, out result))
        {
            return result;
        }
        return null;
    }
    #endregion

    public void Export(string path)
    {
        //
        if (Distinct())
            return;

        Reset(null);
        _mappingInfos.ForEach(info => info.CollectEntities());
        _mappingInfos.ForEach(info => info.Export());
        CalculateRange();
        ExportRoot();
        SaveData(path);
        _mappingInfos.ForEach(info => info.PostExport());
    }

    private void CalculateRange()
    {
        float space = 10;//四周扩大10米
        float minX = 0;
        float minZ = 0;
        float maxX = 0;
        float maxZ = 0;
        _mappingInfos.ForEach(info => info.CalculateRange(ref minX, ref minZ, ref maxX, ref maxZ));
        this._level.x = minX - space;
        this._level.z = minZ - space;
        this._level.width = Mathf.Abs(maxX + space - minX);
        this._level.height = Mathf.Abs(maxZ + space - minZ);
    }

    private void ExportRoot()
    {
        var root = _jsonData.root;
        root.name = "根节点";
        root.nodeType = ItemNodeType.root;
        root.localScale = Vector3.one;
        root.localRotation = Vector3.zero;
        root.localPosition = Vector3.zero;
        root.worldScale = Vector3.one;
        root.worldPosition = Vector3.zero;
        root.worldRotation = Vector3.zero;

        root.__children = new List<string>();
        root.__children_uuid = new List<string>();
        AddAllEntitiesAsChildren(typeof(KTLevelSpawnPoint), root.__children, root.__children_uuid);
        AddAllEntitiesAsChildren(typeof(KTLevelFightStage), root.__children, root.__children_uuid);
    }

    //by lijunfeng 2018/7/9
    private bool Distinct()
    {
        if (!KTLevelPatrolZone.DistanctID())
        {
            Debug.LogError("ID of PatrolZone can not be repeat");
            return true;
        }

        return false;
    }

    private void AddAllEntitiesAsChildren(System.Type entityType, List<string> childList, List<string> childUUIDList)
    {
        var mappingInfo = GetMappingInfo(entityType);
        var exportList = mappingInfo.exportInfo.mapList;
        foreach (var obj in exportList)
        {
            var data = (NodeData)obj;
            childList.Add(data.addr);
            childUUIDList.Add(data.addr_uuid);
        }
    }

    #region external tools

    /// <summary>
    /// 导出区域到表
    /// </summary>
    /// <param name="dir"></param>
    public static void ExportAreaToExcel(string dir)
    {
        string jsonDir = Path.GetFullPath(dir);
        var filePaths = Directory.GetFiles(jsonDir);
        List<string[]> patrolAreaDatas = new List<string[]>();
        Dictionary<string, string> ids = new Dictionary<string, string>();

        foreach (var file in filePaths)
        {
            Debug.Log(file + " areas export over");
            if (file.EndsWith(".json"))
            {
                JsonData jsonData = LoadData(file);
                if (jsonData == null)
                {
                    continue;
                }

                jsonData.patrolAreas.ForEach((item) =>
                {
                    string id = item.id.ToString();
                    if (!ids.ContainsKey(id))
                    {
                        string localPosition = string.Format("{0};{1};{2}", item.localPosition.x, item.localPosition.y, item.localPosition.z);
                        string worldPosition = string.Format("{0};{1};{2}", item.worldPosition.x, item.worldPosition.y, item.worldPosition.z);
                        string localRotation = string.Format("{0};{1};{2}", item.localRotation.x, item.localRotation.y, item.localRotation.z);
                        string worldRotation = string.Format("{0};{1};{2}", item.worldRotation.x, item.worldRotation.y, item.worldRotation.z);
                        string localScale = string.Format("{0};{1};{2}", item.localScale.x, item.localScale.y, item.localScale.z);
                        string worldScale = string.Format("{0};{1};{2}", item.worldScale.x, item.worldScale.y, item.worldScale.z);
                        patrolAreaDatas.Add(new string[] { id, localPosition, worldPosition, localRotation, worldRotation, localScale, worldScale, item.radius.ToString() });
                        ids.Add(id, file);
                    }
                    else
                    {
                        Debug.LogErrorFormat("area id {0} is repeated in {1} and {2}", id, file, ids[id]);
                    }
                });
            }
        }

        if (patrolAreaDatas.Count > 0)
            ExportContextUtils.RefreshExcel("area_map_info", new string[] { "ID", "局部位置", "世界位置", "局部旋转", "世界旋转", "局部缩放", "世界缩放", "半径" }, patrolAreaDatas);
    }

    /// <summary>
    /// 导出npc位置到表
    /// </summary>
    /// <param name="dir"></param>
    public static void ExportSpawnerPosToExcel(string dir)
    {
        string jsonDir = Path.GetFullPath(dir);
        string[] filePaths = Directory.GetFiles(jsonDir);
        List<string[]> spawnerPosDatas = new List<string[]>();
        Dictionary<int, string> ids = new Dictionary<int, string>();

        foreach (var file in filePaths)
        {
            Debug.Log(file + " spawner pos export over");
            if (file.EndsWith(".json"))
            {
                JsonData jsonData = LoadData(file);
                if (jsonData == null)
                    continue;

                jsonData.monsterGenerators.ForEach((item) =>
                {
                    if (item.selectedRoleType == (int)RoleType.other)
                    {
                        if (!ids.ContainsKey(item.selectedRoleId))
                        {
                            List<string> xListLocal = new List<string>();
                            List<string> yListLocal = new List<string>();
                            List<string> zListLocal = new List<string>();
                            List<string> xListWorld = new List<string>();
                            List<string> yListWorld = new List<string>();
                            List<string> zListWorld = new List<string>();
                            for (int i = 0; i < item.__children.Count; i++)
                            {
                                MonsterPoint monster = jsonData.monsters.Find((item2) => item2.addr == item.__children[i]);
                                if (monster != null)
                                {
                                    xListLocal.Add(monster.localPosition.x.ToString());
                                    yListLocal.Add(monster.localPosition.y.ToString());
                                    zListLocal.Add(monster.localPosition.z.ToString());
                                    xListWorld.Add(monster.worldPosition.x.ToString());
                                    yListWorld.Add(monster.worldPosition.y.ToString());
                                    zListWorld.Add(monster.worldPosition.z.ToString());
                                }
                            }

                            string xLocalStr = string.Join(";", xListLocal.ToArray());
                            string yLocalStr = string.Join(";", yListLocal.ToArray());
                            string zLocalStr = string.Join(";", zListLocal.ToArray());
                            string xWorldStr = string.Join(";", xListWorld.ToArray());
                            string yWorldStr = string.Join(";", yListWorld.ToArray());
                            string zWorldStr = string.Join(";", zListWorld.ToArray());
                            spawnerPosDatas.Add(new string[] { item.selectedRoleId.ToString(), xLocalStr, yLocalStr, zLocalStr, xWorldStr, yWorldStr, zWorldStr });
                            ids.Add(item.selectedRoleId, file);
                        }
                        else
                        {
                            Debug.LogErrorFormat("creature id {0} is repeated in {1} and {2}", item.selectedRoleId.ToString(), file, ids[item.selectedRoleId]);
                        }
                    }
                });
            }
        }

        if (spawnerPosDatas.Count > 0)
            ExportContextUtils.RefreshExcel("giver_pos_info", new string[] { "ID", "局部位置x", "局部位置y", "局部位置z", "世界位置x", "世界位置y", "世界位置z" }, spawnerPosDatas);
    }

    /// <summary>
    /// 导出交互物位置到表
    /// </summary>
    /// <param name="dir"></param>
    public static void ExportInteractItemSpawnerPosToExcel(string dir)
    {
        string jsonDir = Path.GetFullPath(dir);
        string[] filePaths = Directory.GetFiles(jsonDir);
        List<string[]> spawnerPosDatas = new List<string[]>();
        Dictionary<int, string> ids = new Dictionary<int, string>();

        foreach (var file in filePaths)
        {
            Debug.Log(file + " spawner pos export over");
            if (file.EndsWith(".json"))
            {
                JsonData jsonData = LoadData(file);
                if (jsonData == null)
                    continue;

                jsonData.monsterGenerators.ForEach((item) =>
                {
                    if (item.generatorType == GeneratorType.interactionItem)
                    {
                        if (!ids.ContainsKey(item.selectedRoleId))
                        {
                            List<string> xListLocal = new List<string>();
                            List<string> yListLocal = new List<string>();
                            List<string> zListLocal = new List<string>();
                            List<string> xListWorld = new List<string>();
                            List<string> yListWorld = new List<string>();
                            List<string> zListWorld = new List<string>();
                            for (int i = 0; i < item.__children.Count; i++)
                            {
                                MonsterPoint monster = jsonData.monsters.Find((item2) => item2.addr == item.__children[i]);
                                if (monster != null)
                                {
                                    xListLocal.Add(monster.localPosition.x.ToString());
                                    yListLocal.Add(monster.localPosition.y.ToString());
                                    zListLocal.Add(monster.localPosition.z.ToString());
                                    xListWorld.Add(monster.worldPosition.x.ToString());
                                    yListWorld.Add(monster.worldPosition.y.ToString());
                                    zListWorld.Add(monster.worldPosition.z.ToString());
                                }
                            }

                            string xLocalStr = string.Join(";", xListLocal.ToArray());
                            string yLocalStr = string.Join(";", yListLocal.ToArray());
                            string zLocalStr = string.Join(";", zListLocal.ToArray());
                            string xWorldStr = string.Join(";", xListWorld.ToArray());
                            string yWorldStr = string.Join(";", yListWorld.ToArray());
                            string zWorldStr = string.Join(";", zListWorld.ToArray());
                            spawnerPosDatas.Add(new string[] { item.selectedRoleId.ToString(), xLocalStr, yLocalStr, zLocalStr, xWorldStr, yWorldStr, zWorldStr });
                            ids.Add(item.selectedRoleId, file);
                        }
                        else
                        {
                            Debug.LogErrorFormat("creature id {0} is repeated in {1} and {2}", item.selectedRoleId.ToString(), file, ids[item.selectedRoleId]);
                        }
                    }
                });
            }
        }

        if (spawnerPosDatas.Count > 0)
            ExportContextUtils.RefreshExcel("treasure_pos_info", new string[] { "ID", "局部位置x", "局部位置y", "局部位置z", "世界位置x", "世界位置y", "世界位置z" }, spawnerPosDatas);
    }

    /// <summary>
    /// 导出npc巡逻路径点到表
    /// </summary>
    /// <param name="dir"></param>
    public static void ExportPatrolPathToExcel(string dir)
    {
        string jsonDir = Path.GetFullPath(dir);
        string[] filePaths = Directory.GetFiles(jsonDir);
        List<string[]> spawnerPatrolPointsData = new List<string[]>();
        Dictionary<int, string> ids = new Dictionary<int, string>();

        foreach (var file in filePaths)
        {
            Debug.Log(file + " spawner pos export over");
            if (file.EndsWith(".json"))
            {
                JsonData jsonData = LoadData(file);
                if (jsonData == null)
                    continue;

                jsonData.monsterGenerators.ForEach((item) =>
                {
                    if (item.selectedRoleType == (int)RoleType.other)
                    {
                        if (!ids.ContainsKey(item.selectedRoleId))
                        {
                            if (string.IsNullOrEmpty(item.__patrolPath))
                                return;

                            PatrolPath patrolPath = jsonData.patrolPaths.Find((item2) => item.__patrolPath == item2.addr);
                            List<string> xListLocal = new List<string>();
                            List<string> yListLocal = new List<string>();
                            List<string> zListLocal = new List<string>();
                            List<string> xListWorld = new List<string>();
                            List<string> yListWorld = new List<string>();
                            List<string> zListWorld = new List<string>();
                            for (int i = 0; i < patrolPath.__children.Count; i++)
                            {
                                PatrolPoint patrolPoint = jsonData.patrolPoints.Find((item3) => item3.addr == patrolPath.__children[i]);
                                if (patrolPoint != null)
                                {
                                    xListLocal.Add(patrolPoint.localPosition.x.ToString());
                                    yListLocal.Add(patrolPoint.localPosition.y.ToString());
                                    zListLocal.Add(patrolPoint.localPosition.z.ToString());
                                    xListWorld.Add(patrolPoint.worldPosition.x.ToString());
                                    yListWorld.Add(patrolPoint.worldPosition.y.ToString());
                                    zListWorld.Add(patrolPoint.worldPosition.z.ToString());
                                }
                            }

                            string xLocalStr = string.Join(";", xListLocal.ToArray());
                            string yLocalStr = string.Join(";", yListLocal.ToArray());
                            string zLocalStr = string.Join(";", zListLocal.ToArray());
                            string xWorldStr = string.Join(";", xListWorld.ToArray());
                            string yWorldStr = string.Join(";", yListWorld.ToArray());
                            string zWorldStr = string.Join(";", zListWorld.ToArray());
                            spawnerPatrolPointsData.Add(new string[] { item.selectedRoleId.ToString(), xLocalStr, yLocalStr, zLocalStr, xWorldStr, yWorldStr, zWorldStr });
                            ids.Add(item.selectedRoleId, file);
                        }
                        else
                        {
                            Debug.LogErrorFormat("creature id {0} is repeated in {1} and {2}", item.selectedRoleId.ToString(), file, ids[item.selectedRoleId]);
                        }
                    }
                });
            }
        }

        if (spawnerPatrolPointsData.Count > 0)
            ExportContextUtils.RefreshExcel("giver_patrolpath_info", new string[] { "ID", "局部位置x", "局部位置y", "局部位置z", "世界位置x", "世界位置y", "世界位置z" }, spawnerPatrolPointsData);
    }

    /// <summary>
    /// 检查json文件里的creature id在creature表里是否存在
    /// </summary>
    /// <param name="dir"></param>
    public static void CheckIDExistInCreature(string path)
    {
        if (!path.EndsWith(".json"))
            return;

        JsonData jsonData = LoadData(path);
        if (jsonData == null)
            return;

        HashSet<int> ids = new HashSet<int>();
        jsonData.monsterGenerators.ForEach((item) =>
        {
            if (item.selectedRoleType == (int)RoleType.other)
            {
                if (!ids.Contains(item.selectedRoleId))
                {
                    var hasRow = KTExcelManager.instance.Get(KTExcels.kCreatures, item.selectedRoleId, "id");
                    if (string.IsNullOrEmpty(hasRow))
                    {
                        Debug.LogErrorFormat("{0} is not exist in creature", item.selectedRoleId);
                    }
                    ids.Add(item.selectedRoleId);
                }
            }
        });
    }

    /// <summary>
    /// 检查战斗单位孵化器配置的单位类型与表中填的是否一致
    /// </summary>
    public static void CheckUnitTypeInvalide(string dir)
    {
        string jsonDir = Path.GetFullPath(dir);
        string[] filePaths = Directory.GetFiles(jsonDir);
        foreach (var file in filePaths)
        {
            if (file.EndsWith(".json"))
            {
                JsonData jsonData = LoadData(file);
                if (jsonData == null)
                    continue;

                jsonData.monsterGenerators.ForEach((item) =>
                {
                    if (item.generatorType == GeneratorType.interactionItem || item.generatorType == GeneratorType.battleUnit && item.selectedRoleType >= (int)RoleType.npc && item.selectedRoleType <= (int)RoleType.other)
                    {
                        string unitType = KTExcelManager.instance.Get(KTExcels.kCreatures, item.selectedRoleId, "单位类型");
                        if (string.IsNullOrEmpty(unitType))
                        {
                            Debug.LogErrorFormat("文件{0}中，ID为{1}孵化器{2}在creature表中的单位类型不存在", file, item.selectedRoleId, item.addr);
                            return;
                        }

                        if (int.Parse(unitType) != item.selectedRoleType)
                        {
                            Debug.LogErrorFormat("文件{0}中，ID为{1}孵化器{2}所填单位类型{3}与creature表中的单位类型{4}不一致", file, item.selectedRoleId, item.addr, item.selectedRoleType, int.Parse(unitType));
                        }
                    }
                });
            }
        }
    }

    /// <summary>
    /// 检查区域，孵化器，位置，旋转周x,z是否为0，否则非法
    /// </summary>
    public static void CheckRotationInvalide(string dir)
    {
        string jsonDir = Path.GetFullPath(dir);
        string[] filePaths = Directory.GetFiles(jsonDir);
        foreach (var file in filePaths)
        {
            if (file.EndsWith(".json"))
            {
                JsonData jsonData = LoadData(file);
                if (jsonData == null)
                    continue;

                jsonData.patrolAreas.ForEach((item) =>
                {
                    //Debug.LogFormat("检查文件{0}，区域{2}的旋转{3},{4}", file, item.addr,item.localRotation.ToString(),item.worldRotation.ToString());
                    if (item.worldRotation.x != 0 || item.localRotation.z != 0 || item.worldRotation.x != 0 || item.localRotation.z != 0)
                    {
                        Debug.LogErrorFormat("文件{0}中，区域{1}的旋转轴xz不为0", file, item.addr);
                    }
                });


                jsonData.monsterGenerators.ForEach((item) =>
                {
                    //Debug.LogFormat("检查文件{0}，孵化器{2}的旋转{3},{4}", file, item.addr, item.localRotation.ToString(), item.worldRotation.ToString());
                    if (item.worldRotation.x != 0 || item.localRotation.z != 0 || item.worldRotation.x != 0 || item.localRotation.z != 0)
                    {
                        Debug.LogErrorFormat("文件{0}中，孵化器{1}的旋转轴xz不为0", file, item.addr);
                    }
                });

                jsonData.monsters.ForEach((item) =>
                {
                    //Debug.LogFormat("检查文件{0}，位置{2}的旋转{3},{4}", file, item.addr, item.localRotation.ToString(), item.worldRotation.ToString());
                    if (item.worldRotation.x != 0 || item.localRotation.z != 0 || item.worldRotation.x != 0 || item.localRotation.z != 0)
                    {
                        Debug.LogErrorFormat("文件{0}中，位置{1}的旋转轴xz不为0", file, item.addr);
                    }
                });
            }
        }
    }
    #endregion

    #region Import
    public void Import(string path)
    {
        JsonData jsonData = LoadData(path);
        if (jsonData == null)
        {
            return;
        }
        Reset(jsonData);

        var root = CreateRootGo();
        CreateAllEntities();
        LinkAll();
        RefreshAll();
        LayoutAll(root.transform);
    }

    private void CreateAllEntities()
    {
        _exportInfos.ForEach(info => CreateEntities(info));
    }

    private void CreateEntities(ExportInfo info)
    {
        foreach (var item in info.mapList)
        {
            var entityType = info.GetEntityType(item);
            var mappingInfo = GetMappingInfo(entityType);
            var entity = mappingInfo.CreateEntity();
            var itemNode = (NodeData)item;
            ReigsterAddress(itemNode.addr, entity);
            mappingInfo.Import(entity, itemNode);
        }
    }

    private void RefreshAll()
    {
        _spawners.ForEach(spawner => spawner.RefreshView());
    }

    private void LayoutAll(Transform root)
    {
        Layout(_jsonData.root, root);
        _exportInfos.ForEach(exportInfo =>
        {
            foreach (var it in exportInfo.mapList)
            {
                Layout((NodeData)it);
            }
        });
    }

    private void Layout(NodeData item, Transform root = null)
    {
        var rootTrans = root ?? GetEntityByAddress(item.addr).transform;

        foreach (var address in item.__children)
        {
            var entity = GetEntityByAddress(address);
            entity.transform.SetParent(rootTrans, false);
        }
    }

    private void LinkAll()
    {
        foreach (var linkInfo in _linkInfos)
        {
            if (linkInfo.address.GetType() == typeof(string))
            {
                var strAddress = (string)linkInfo.address;
                if (string.IsNullOrEmpty(strAddress))
                {
                    continue;
                }

                var linkTarget = GetEntityByAddress(strAddress);
                if (linkTarget == null)
                {
                    Debug.LogWarningFormat("无效的链接{0}", strAddress);
                    continue;
                }

                linkInfo.field.SetValue(linkInfo.instance, linkTarget);
            }
            else
            {
                var addresses = (List<string>)linkInfo.address;
                var list = (System.Collections.IList)System.Activator.CreateInstance(linkInfo.field.FieldType);
                foreach (var address in addresses)
                {
                    var linkTarget = GetEntityByAddress(address);
                    if (linkTarget == null)
                    {
                        Debug.LogWarningFormat("无效的链接{0}", address);
                        continue;
                    }
                    list.Add(linkTarget);
                }
                linkInfo.field.SetValue(linkInfo.instance, list);
            }
        }
    }

    public void ReigsterAddress(string address, KTLevelEntity entity)
    {
        _addrToEntity.Add(address, entity);
    }

    public void RegisterLink(LinkInfo info)
    {
        _linkInfos.Add(info);
    }

    public KTLevelEntity GetEntityByAddress(string address)
    {
        KTLevelEntity entity;
        if (_addrToEntity.TryGetValue(address, out entity))
        {
            return entity;
        }
        return null;
    }
    #endregion

    private GameObject CreateRootGo()
    {
        var root = new GameObject("root");
        var jsonRoot = _jsonData.root;
        var rootTrans = root.transform;
        rootTrans.localPosition = jsonRoot.localPosition;
        rootTrans.localRotation = Quaternion.Euler(jsonRoot.localRotation);
        rootTrans.localScale = jsonRoot.localScale;
        return root;
    }

    private void SaveData(string path)
    {
        _jsonData.ver = this._level.ver;//by lijunfeng 2018/6/28
        _jsonData.x = this._level.x;
        _jsonData.y = this._level.z;
        _jsonData.width = this._level.width == 0 ? 1024 : this._level.width;
        _jsonData.height = this._level.height == 0 ? 1024 : this._level.height;
        string jsonStr = JsonUtility.ToJson(_jsonData, true);
        File.WriteAllText(path, jsonStr);
    }

    private static JsonData LoadData(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogErrorFormat("关卡文件不存在, file:{0}", path);
            return null;
        }

        string jsonStr = File.ReadAllText(path);
        var jsonData = JsonUtility.FromJson<JsonData>(jsonStr);
        if (jsonData == null)
        {
            Debug.LogErrorFormat("关卡文件解析失败, file:{0}", path);
        }
        return jsonData;
    }
}
