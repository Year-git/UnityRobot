public static class KTExcels
{
    public const string kItems = "abiotic.xlsx";
    public const string kModelAssets = "model_path.xlsx";
    public const string kCreatures = "creature.xlsx";
    public const string kInteractionItems = "treasure.xlsx";
    public const string kCollectionItems = "collection_item.xlsx";
    public const string kTraps = "trap.xlsx";
    public const string kMap = "map.xlsx";

    public static string GetAssetIdColumeName(string tblName)
    {
        switch(tblName)
        {
            case kCreatures:
            case kItems: return "资源ID";
            case kInteractionItems: return "宝箱表现ID";
            case kCollectionItems: return "资源ID";
            case kTraps: return "模型ID";
        }
        return "";
    }
}
