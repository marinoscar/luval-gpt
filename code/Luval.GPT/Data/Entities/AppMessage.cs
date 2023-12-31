﻿using Luval.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data.Entities
{
    [Index(nameof(ProviderName)), Index(nameof(ProviderKey)), Index(nameof(ChatType))]
    public class AppMessage : IdentityEntity
    {
        [Required, MaxLength(50)]
        public string? ChatType { get; set; }
        [Required, MaxLength(150)]
        public string? ProviderName { get; set; }
        [Required, MaxLength(150)]
        public string? ProviderKey { get; set; }
        [Required]
        public DateTime UtcDateTime { get; set; }
        public string? UserPrompt { get; set; }
        public string? AgentText { get; set; }
        public string? UserMediaType { get; set; }
        public string? UserMediaItemUrl { get; set; }
        public string? AgentMediaType { get; set; }
        public string? AgentMediaItemUrl { get; set; }
        public uint? TokenCount { get; set; }
        public int UpVote { get; set; }
        public int DownVote { get; set; }
        public string? MessageData { get; set; }
        [NotMapped]
        public int Score { get { return UpVote - DownVote; } }

    }
}
