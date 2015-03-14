using System;

public static partial class Spark
{
	public static Func<byte[], int, int, byte[]> LZ4EncodeFunc;
	public static Func<byte[], int, int, int, byte[]> LZ4DecodeFunc;

	public static bool LZ4Compression
	{
		get { return (s_formatFlags & FormatFlags.LZ4Compression) == FormatFlags.LZ4Compression; }
		set { s_formatFlags = (value) ? (s_formatFlags | FormatFlags.LZ4Compression) : (s_formatFlags & ~FormatFlags.LZ4Compression); }
	}

	private static byte[] LZ4Encode(byte[] data)
	{
		TypeHelper.IntTypeMapper mapper = new TypeHelper.IntTypeMapper();
		mapper.value = data.Length - s_headerSize;

		byte[] encodedData = LZ4EncodeFunc(data, s_headerSize, data.Length - s_headerSize); // Cut header
		byte[] output = new byte[encodedData.Length + s_headerSize + sizeof(int)];

		int index = 0;
		WriteHeader(output, ref index);

		output[s_headerSize + 0] = mapper.byte1;
		output[s_headerSize + 1] = mapper.byte2;
		output[s_headerSize + 2] = mapper.byte3;
		output[s_headerSize + 3] = mapper.byte4;

		Array.Copy(encodedData, 0, output, s_headerSize + sizeof(int), encodedData.Length);

		return output;
	}

	private static byte[] LZ4Decode(byte[] data)
	{
		TypeHelper.IntTypeMapper mapper = new TypeHelper.IntTypeMapper();

		mapper.byte1 = data[s_headerSize + 0];
		mapper.byte2 = data[s_headerSize + 1];
		mapper.byte3 = data[s_headerSize + 2];
		mapper.byte4 = data[s_headerSize + 3];

		int dataSize = mapper.value;

		byte[] decodedData = LZ4DecodeFunc(data, s_headerSize + sizeof(int), data.Length - s_headerSize - sizeof(int), dataSize);
		byte[] output = new byte[s_headerSize + decodedData.Length];

		for (int i = 0; i < s_headerSize; ++i)
			output[i] = data[i];

		Array.Copy(decodedData, 0, output, s_headerSize, decodedData.Length);

		return output;
	}
}