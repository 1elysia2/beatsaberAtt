using UnityEngine;

public static class AudioUtility
{
    /// <summary>
    /// 将AudioClip转为WAV字节（用于STT传输）
    /// </summary>
    public static byte[] ToWAV(this AudioClip clip)
    {
        using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
        {
            // WAV头写入
            WriteWAVHeader(stream, clip);

            // 音频数据写入
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);
            byte[] bytes = ConvertAudioDataToByteArray(samples);
            stream.Write(bytes, 0, bytes.Length);

            return stream.ToArray();
        }
    }

    private static void WriteWAVHeader(System.IO.MemoryStream stream, AudioClip clip)
    {
        int hz = clip.frequency;
        int channels = clip.channels;
        int samples = clip.samples;

        stream.Seek(0, System.IO.SeekOrigin.Begin);

        // RIFF头
        stream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);
        stream.Write(System.BitConverter.GetBytes(samples * 2 + 36), 0, 4);
        stream.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, 4);

        // fmt子块
        stream.Write(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, 4);
        stream.Write(System.BitConverter.GetBytes(16), 0, 4);
        stream.Write(System.BitConverter.GetBytes((ushort)1), 0, 2);
        stream.Write(System.BitConverter.GetBytes((ushort)channels), 0, 2);
        stream.Write(System.BitConverter.GetBytes(hz), 0, 4);
        stream.Write(System.BitConverter.GetBytes(hz * channels * 2), 0, 4);
        stream.Write(System.BitConverter.GetBytes((ushort)(channels * 2)), 0, 2);
        stream.Write(System.BitConverter.GetBytes((ushort)16), 0, 2);

        // data子块
        stream.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);
        stream.Write(System.BitConverter.GetBytes(samples * 2), 0, 4);
    }

    private static byte[] ConvertAudioDataToByteArray(float[] samples)
    {
        byte[] bytes = new byte[samples.Length * 2];
        int rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(samples[i] * rescaleFactor);
            System.BitConverter.GetBytes(value).CopyTo(bytes, i * 2);
        }

        return bytes;
    }
}
