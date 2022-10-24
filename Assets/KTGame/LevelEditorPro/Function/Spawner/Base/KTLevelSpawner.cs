#if UNITY_EDITOR

public abstract class KTLevelSpawner : KTLevelEntity
{
    protected override string uuidPrefix
    {
        get
        {
            return "monsterGenerators";
        }
    }

    public virtual void RefreshView()
    {

    }
}
#endif