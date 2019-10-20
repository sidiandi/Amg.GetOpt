using System.IO;

namespace Amg.Extensions
{
    /// <summary>
    /// Something that can be written to a TextWriter
    /// </summary>
    internal interface IWritable
    {
        /// <summary>
        /// Write the object to a text writer.
        /// </summary>
        /// <param name="textWriter"></param>
        void Write(TextWriter textWriter);
    }
}