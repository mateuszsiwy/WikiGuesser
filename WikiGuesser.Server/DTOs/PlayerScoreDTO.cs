namespace WikiGuesser.Server.DTOs;

public class PlayerScoreDTO
{
    public Guid PlayerId { get; set; }
    public int Score { get; set; }
    public double Distance { get; set; }
}