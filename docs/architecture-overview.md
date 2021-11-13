```mermaid
  graph LR

  Upload
  FileService
  MessageQueue
  DocumentProcessor
  DocumentService
  
  OpenSearch
  FileIndex
  DocumentIndex
  OpenSearch---FileIndex
  OpenSearch---DocumentIndex


  Upload-->FileService
  
  FileService--Create file group for uploaded document-->FileIndex 

  FileService--Insert file upload-->MessageQueue

  MessageQueue-->DocumentProcessor

  DocumentProcessor--Analyzes files of upload and creates a document entry -->DocumentService

  DocumentService---DocumentIndex
  
```