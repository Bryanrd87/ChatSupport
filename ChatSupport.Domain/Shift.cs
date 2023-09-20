namespace ChatSupport.Domain;
public class Shift
{
    public int ShiftId { get; set; }
    public string Name { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public ICollection<Agent> Agents { get; set; } = new List<Agent>();
}

