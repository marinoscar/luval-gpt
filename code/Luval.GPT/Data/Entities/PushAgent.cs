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
    public class PushAgent : IdentityEntity
    {
        [Required, MaxLength(50)]
        public string? Name { get; set; }
        [Required, MaxLength(1000)]
        public string? Description { get; set; }
        public string? FriendlyName { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; set; }
        public string? AppUserId { get; set; }

        [Required]
        public string? AgentPurpose { get; set; }
        public string? UserPrompt { get; set; }
        public string? PromptPrefix { get; set; }
        public string? PromptSuffix { get; set; }
        public string? SystemMessage { get; set; }

        public string? Timezone { get; set; }

        public string? ChronExpressionPrompt { get; set; }

        [Required]
        public string? ChronExpression { get; set; }

    }
}
