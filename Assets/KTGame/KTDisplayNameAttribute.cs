public class KTDisplayNameAttribute : System.Attribute
{
    public string displayName;
    public KTDisplayNameAttribute(string name)
    {
        displayName = name;
    }

    public override string ToString()
    {
        return string.Format("KTDisplayName(\"{0}\")", displayName);
    }
}
