using Luval.GPT.Data.Entities;

namespace Luval.WebGPT.Data.ViewModel
{
    public class PushAgentCollection
    {
        public PushAgentCollection()
        {
            Agents = new List<PushAgent>();
        }
        public List<PushAgent> Agents { get; set; }
    }
}
