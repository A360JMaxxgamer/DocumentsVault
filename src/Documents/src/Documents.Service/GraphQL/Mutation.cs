using Documents.Service.GraphQL.Exceptions;
using Documents.Service.Models;
using MongoDB.Driver;

namespace Documents.Service.GraphQL;

public class Mutation
{
    /// <summary>
    /// Adds a new document
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="fileIds">A list of all associated files</param>
    /// <param name="metadata">Metadata of document</param>
    /// <returns>Created document</returns>
    [Error(typeof(FileIdsExistOnDocumentException))]
    public Document AddDocument([Service] IMongoCollection<Document> collection, Metadata metadata)
    {
        var creationDate = DateTime.UtcNow;
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Metadata = metadata,
            CreationDate = creationDate,
            ModificationDate = creationDate
        };

        collection.InsertOne(document);
        
        return document;
    }

    /// <summary>
    /// Deletes a document by its id
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="documentId">Id of the document to delete</param>
    /// <returns>Updated document</returns>
    [Error(typeof(DocumentNotFoundException))]
    public void DeleteDocument([Service] IMongoCollection<Document> collection, Guid documentId)
    {
        var exists = collection
            .Find(doc => doc.Id == documentId)
            .Any();

        if (!exists)
        {
            throw new DocumentNotFoundException(documentId);
        }

        collection.DeleteOne(doc => doc.Id == documentId);
    }


    /// <summary>
    /// Adds <paramref name="tags"/> to the document with the <paramref name="documentId"/>
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="documentId">Id of the document</param>
    /// <param name="tags">Tags to add</param>
    /// <returns>Updated document</returns>
    [Error(typeof(DocumentNotFoundException))]
    public Document AddTags([Service] IMongoCollection<Document> collection, Guid documentId, string[] tags)
    {
        var document = FindDocument(collection, documentId);
        var newTags = tags
            .Where(tag => !document.Metadata.Tags.Contains(tag))
            .ToArray();
        document.Metadata.Tags.AddRange(newTags);
        return SaveDocument(collection, document);
    }

    /// <summary>
    /// Deletes <paramref name="tags"/> off the document with the <paramref name="documentId"/>
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="documentId">Id of the document</param>
    /// <param name="tags">Tags to delete</param>
    /// <returns>Updated document</returns>
    [Error(typeof(DocumentNotFoundException))]
    public Document DeleteTags([Service] IMongoCollection<Document> collection, Guid documentId, string[] tags)
    {
        var document = FindDocument(collection, documentId);
        var newTagList = document.Metadata.Tags
            .Where(tag => !tags.Contains(tag))
            .ToArray();
        document.Metadata.Tags.Clear();
        document.Metadata.Tags.AddRange(newTagList);
        return SaveDocument(collection, document);
    }

    /// <summary>
    /// Updates the title of the document
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="documentId">Id of the document</param>
    /// <param name="title">New title</param>
    /// <returns>Updated document</returns>
    [Error(typeof(DocumentNotFoundException))]
    public Document UpdateDocumentTitle([Service] IMongoCollection<Document> collection, Guid documentId, string title)
    {
        var document = FindDocument(collection, documentId);
        document.Metadata.Title = title;
        return SaveDocument(collection, document);
    }

    /// <summary>
    /// Updates the text of the document
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="documentId">Id of the document</param>
    /// <param name="text">New text</param>
    /// <returns>Updated document</returns>
    [Error(typeof(DocumentNotFoundException))]
    public Document UpdateDocumentText([Service] IMongoCollection<Document> collection, Guid documentId, string text)
    {
        var document = FindDocument(collection, documentId);
        document.Metadata.Text = text;
        return SaveDocument(collection, document);
    }

    private Document FindDocument(IMongoCollection<Document> collection, Guid documentId)
    {
        var document = collection
            .Find(doc => doc.Id == documentId)
            .FirstOrDefault();

        if (document is null)
        {
            throw new DocumentNotFoundException(documentId);
        }

        return document;
    }

    private Document SaveDocument(IMongoCollection<Document> collection, Document updatedDocument)
    {
        updatedDocument.ModificationDate = DateTime.UtcNow;
        collection.ReplaceOne(doc => doc.Id == updatedDocument.Id, updatedDocument);
        return updatedDocument;
    }
}