namespace frontend.Responses;

public record CommentsResponse(
    Guid Id, 
    string Text, 
    string UserName, 
    string Email, 
    string AttachmentUrl, 
    DateTime CommentCreated, 
    List<CommentsResponse>? ChildrenComments);