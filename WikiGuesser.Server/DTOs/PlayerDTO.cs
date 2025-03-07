namespace WikiGuesser.Server.DTOs;

public class PlayerDTO
{
    public Guid PlayerId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public bool IsReady { get; set; }
    public int Score { get; set; }
}