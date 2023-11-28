using Luval.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data.Entities
{
    [Index(nameof(ProviderName)), Index(nameof(ProviderKey)), Index(nameof(ChatType))]
    public class AppMessage : IdentityEntity
    {

        [MaxLength(150)]
        public string? ChannelId { get; set; }
        [Required, MaxLength(100)]
        public string? ChatType { get; set; }
        [Required, MaxLength(150)]
        public string? ProviderName { get; set; }
        [Required, MaxLength(150)]
        public string? ProviderKey { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string? UserPrompt { get; set; }
        [Required]
        public string? AgentText { get; set; }
        public string? UserMediaType { get; set; }
        public string? UserMediaItemUrl { get; set; }
        public string? AgentMediaType { get; set; }
        public string? AgentMediaItemUrl { get; set; }
        public uint? TokenCount { get; set; }
        [Required]
        public string? MessageData { get; set; }

    }
}
