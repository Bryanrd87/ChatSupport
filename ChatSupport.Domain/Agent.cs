using ChatSupport.Domain.Enums;

namespace ChatSupport.Domain;
public class Agent
{
    public int AgentId { get; set; }
    public string Name { get; set; }
    public SeniorityLevel SeniorityLevel { get; set; }
    public int ShiftStartHour { get; set; }
    public int ShiftEndHour { get; set; }
    public int CurrentChatCount { get; set; }
}

