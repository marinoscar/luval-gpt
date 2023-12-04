using Luval.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data.Entities
{
    [Index(nameof(AppUserId))]
    public class PushAgentMessage : IdentityEntity
    {
        [Required]
        public string? UserPrompt { get; set; }
        [Required]
        public string? AgentText { get; set; }
        [Required, MaxLength(50)]
        public string? AppUserId { get; set; }
        [Required]
        public string? Title { get; set; }

        public string? CallToAction { get; set; }
        public uint? PromptTokenCount { get; set; }
        public uint? AgentTokenCount { get; set; }


    }
}
