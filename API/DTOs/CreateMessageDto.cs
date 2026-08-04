namespace API.DTOs;

public class CreateMessageDto
{
    public required string Content { get; set; }
    public required string RecipientUsername { get; set; }
}
