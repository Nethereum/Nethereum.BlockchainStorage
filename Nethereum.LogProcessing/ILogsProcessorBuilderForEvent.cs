using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Nethereum.LogProcessing
{
    public interface ILogsProcessorBuilder<TEventDto> : ILogsProcessorBuilder where TEventDto : class, IEventDTO, new()
    {
        ILogsProcessorBuilder<TEventDto> OnEvents(Action<IEnumerable<EventLog<TEventDto>>> callBack);
        ILogsProcessorBuilder<TEventDto> OnEvents(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack);
    }
}