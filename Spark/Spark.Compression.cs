using System;

public static partial class Spark
{
	public static Func<byte[], int, int, byte[]> LZ4EncodeFunc = null;//LZ4s.LZ4Codec.Encode32;
	public static Func<byte[], int, int, int, byte[]> LZ4DecodeFunc = null;//LZ4s.LZ4Codec.Decode32;

	public static bool LZ4Compression
	{
		get { return (FormatFlags & EFormatFlags.LZ4Compression) == EFormatFlags.LZ4Compression; }
		set { FormatFlags = (value) ? (FormatFlags | EFormatFlags.LZ4Compression) : (FormatFlags & ~EFormatFlags.LZ4Compression); }
	}

	private static byte[] LZ4Encode(byte[] data)
	{
		if (LZ4EncodeFunc == null)
			return data;

		TypeHelper.IntTypeMapper mapper = new TypeHelper.IntTypeMapper();
		mapper.value = data.Length - HeaderSize;

		byte[] encodedData = LZ4EncodeFunc(data, HeaderSize, data.Length - HeaderSize); // Cut header
		byte[] output = new byte[encodedData.Length + HeaderSize + sizeof(int)];

		int index = 0;
		WriteHeader(output, ref index);

		output[HeaderSize + 0] = mapper.byte1;
		output[HeaderSize + 1] = mapper.byte2;
		output[HeaderSize + 2] = mapper.byte3;
		output[HeaderSize + 3] = mapper.byte4;

		Array.Copy(encodedData, 0, output, HeaderSize + sizeof(int), encodedData.Length);

		return output;
	}

	private static byte[] LZ4Decode(byte[] data)
	{
		if (LZ4DecodeFunc == null)
			return data;

		TypeHelper.IntTypeMapper mapper = new TypeHelper.IntTypeMapper();

		mapper.byte1 = data[HeaderSize + 0];
		mapper.byte2 = data[HeaderSize + 1];
		mapper.byte3 = data[HeaderSize + 2];
		mapper.byte4 = data[HeaderSize + 3];

		int dataSize = mapper.value;

		byte[] decodedData = LZ4DecodeFunc(data, HeaderSize + sizeof(int), data.Length - HeaderSize - sizeof(int), dataSize);
		byte[] output = new byte[HeaderSize + decodedData.Length];

		for (int i = 0; i < HeaderSize; ++i)
			output[i] = data[i];

		Array.Copy(decodedData, 0, output, HeaderSize, decodedData.Length);

		return output;
	}
}