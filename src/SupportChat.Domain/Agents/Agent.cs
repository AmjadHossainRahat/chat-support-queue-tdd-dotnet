namespace SupportChat.Domain.Agents
{
    public class Agent
    {
        public Guid Id { get; }
        public Seniority Seniority { get; }

        public Agent(Guid id, Seniority seniority)
        {
            Id = id;
            Seniority = seniority;
        }

        public int GetMaxConcurrentChats()
        {
            throw new NotImplementedException();
        }
    }
}
