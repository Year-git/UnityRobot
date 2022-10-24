#if UNITY_EDITOR

/// <summary>
/// 忽略导出的entity
/// </summary>
public class KTLevelClassIgnoreExportAttribute : System.Attribute
{

	public KTLevelClassIgnoreExportAttribute()
	{
	}

	public override string ToString()
	{
		return string.Format("KTLevelClassIgnoreExportAttribute");
	}
}

#endif
