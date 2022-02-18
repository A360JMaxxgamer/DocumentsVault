using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Files.Service.Services.Exceptions;

[Serializable]
public class FileIdFormatException : Exception 
{
    public FileIdFormatException(string fileId) : 
        base($"{fileId} can not be parsed as a valid uuid format")
    {
    }

    [ExcludeFromCodeCoverage]
    protected FileIdFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}