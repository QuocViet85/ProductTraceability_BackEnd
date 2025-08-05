using App.Areas.TraceEvents.DTO;
using App.Areas.TraceEvents.Models;

namespace App.Areas.TraceEvents.Mapper;

public static class TraceEventMapper
{
    public static TraceEventDTO ModelToDto(TraceEventModel traceEvent)
    {
        return new TraceEventDTO()
        {
            Id = traceEvent.Id,
            Name = traceEvent.Name,
            TraceEventCode = traceEvent.TraceEventCode,
            Description = traceEvent.Description,
            Location = traceEvent.Location,
            TimeStamp = traceEvent.TimeStamp,
            CreatedAt = traceEvent.CreatedAt,
            UpdatedAt = traceEvent.UpdatedAt,
        };
    }

    public static TraceEventModel DtoToModel(TraceEventDTO traceEventDTO, TraceEventModel traceEventUpdate = null)
    {
        TraceEventModel traceEvent;
        if (traceEventUpdate == null)
        {
            traceEvent = new TraceEventModel();
        }
        else
        {
            traceEvent = traceEventUpdate;
        }
        traceEvent.Name = traceEventDTO.Name;
        traceEvent.Description = traceEventDTO.Description;
        traceEvent.Location = traceEventDTO.Location;

        if (traceEventDTO.TimeStamp != null)
        {
            traceEvent.TimeStamp = (DateTime)traceEventDTO.TimeStamp;
        }

        return traceEvent;
    }
}