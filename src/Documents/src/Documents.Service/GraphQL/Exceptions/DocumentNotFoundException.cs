using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Documents.Service.GraphQL.Exceptions;

[Serializable]
public class DocumentNotFoundException : Exception 
{
    public DocumentNotFoundException(Guid documentId) : 
        base($"Document with id {documentId} does not exist")
    {
    }

    [ExcludeFromCodeCoverage]
    protected DocumentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}