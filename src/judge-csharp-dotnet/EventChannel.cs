using System;
using System.IO;
using System.Text;

namespace judge_csharp
{
    // One NUnit event per frame: BinaryWriter's length-prefixed UTF-8 string, so events may contain newlines.
    public sealed class EventWriter
    {
        private readonly BinaryWriter Writer;
        private readonly object Lock = new object();

        public EventWriter(Stream stream)
        {
            Writer = new BinaryWriter(stream, new UTF8Encoding(false));
        }

        public void Send(string report)
        {
            lock (Lock)
            {
                Writer.Write(report);
                Writer.Flush();
            }
        }
    }

    public sealed class EventReader
    {
        private readonly BinaryReader Reader;

        public EventReader(Stream stream)
        {
            Reader = new BinaryReader(stream, new UTF8Encoding(false));
        }

        // Returns null once the stream ends, also when it ends inside a frame.
        public string Receive()
        {
            try
            {
                return Reader.ReadString();
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }
    }
}
