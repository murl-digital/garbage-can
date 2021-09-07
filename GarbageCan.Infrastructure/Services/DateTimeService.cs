using GarbageCan.Application.Common.Interfaces;
using System;

namespace GarbageCan.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}