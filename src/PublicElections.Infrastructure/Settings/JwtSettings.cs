﻿using System;

namespace PublicElections.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
    }
}
