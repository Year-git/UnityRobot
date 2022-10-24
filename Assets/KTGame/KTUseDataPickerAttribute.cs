public class KTUseDataPickerAttribute : System.Attribute
{
    public string xlsName;
    public string valueColume;
    public string[] descColumes;
    public string editorWindowTypeName;

    public KTUseDataPickerAttribute(string xlsName, string valumeColume, string[] descColumes, string editorWindowTypeName = null)
    {
        this.xlsName = xlsName;
        this.valueColume = valumeColume;
        this.descColumes = descColumes;
        this.editorWindowTypeName = editorWindowTypeName;
    }
}
