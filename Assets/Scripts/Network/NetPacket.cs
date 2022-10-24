public class NetPacket
{
    public short Length { get; set; }
    public byte[] Data { get; set; }

    public NetPacket(short length, byte[] data)
    {
        Length = length;
        Data = data;
    }
}