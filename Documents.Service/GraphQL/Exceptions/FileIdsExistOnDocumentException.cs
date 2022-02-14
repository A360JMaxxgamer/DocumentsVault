using System.Runtime.Serialization;

namespace Documents.Service.GraphQL.Exceptions;

[Serializable]
public class FileIdsExistOnDocumentException : Exception
{
    public FileIdsExistOnDocumentException(Guid documentId) : 
        base($"Some file ids are already attached to document with id {documentId}")
    {
    }

    protected FileIdsExistOnDocumentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}