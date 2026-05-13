using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using API.Session;
using PacketLibrary.Handler;
using Serilog;
using SilkroadSecurityAPI.Message;

namespace DuckSoup.Library.Server;

public static class PacketRateLimiter
{
    private const int GlobalPacketLimit = 100;
    private static readonly TimeSpan Window = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(5);
    private static readonly ConcurrentDictionary<ushort, int> OpcodeLimits = new ConcurrentDictionary<ushort, int>
    {
        [0x3080] = 5,
        [0x3081] = 5,
        [0x3082] = 5,
        [0x3083] = 5,
        [0x3084] = 5,
        [0x3085] = 5,
        [0x70B1] = 5
    };

    public static bool IsAllowed(ISession session, Packet packet, string source)
    {
        DateTime now = DateTime.UtcNow;
        PacketRateState state = GetState(session);
        if (!OpcodeLimits.TryGetValue(packet.MsgId, out int opcodeLimit))
        {
            opcodeLimit = GlobalPacketLimit;
        }

        bool allowed = state.TryRecord(packet.MsgId, now, GlobalPacketLimit, opcodeLimit);

        if (!allowed)
        {
            Log.Warning("{0} flood protection disconnected session {1} from {2} for opcode 0x{3:X}",
                source, session.Guid, session.RemoteEndPoint, packet.MsgId);
        }

        Cleanup(session, state, now);
        return allowed;
    }

    private static PacketRateState GetState(ISession session)
    {
        session.GetData(Data.PacketRateState, out PacketRateState? state, null);
        if (state != null)
        {
            return state;
        }

        state = new PacketRateState(Window);
        session.SetData(Data.PacketRateState, state);
        return state;
    }

    private static void Cleanup(ISession session, PacketRateState state, DateTime now)
    {
        if (now - state.LastCleanup < CleanupInterval)
        {
            return;
        }

        state.Cleanup(now);
        session.SetData(Data.PacketRateState, state);
    }

    private sealed class PacketRateState
    {
        private readonly TimeSpan _window;
        private readonly ConcurrentDictionary<ushort, PacketCounter> _opcodeCounters = new ConcurrentDictionary<ushort, PacketCounter>();
        private PacketCounter _globalCounter;

        public PacketRateState(TimeSpan window)
        {
            _window = window;
            LastCleanup = DateTime.UtcNow;
            _globalCounter = new PacketCounter(LastCleanup);
        }

        public DateTime LastCleanup { get; private set; }

        public bool TryRecord(ushort opcode, DateTime now, int globalLimit, int opcodeLimit)
        {
            int globalCount = Increment(ref _globalCounter, now);
            PacketCounter opcodeCounter = _opcodeCounters.AddOrUpdate(opcode,
                new PacketCounter(now, 1),
                (_, current) => current.Increment(now, _window));

            return globalCount <= globalLimit && opcodeCounter.Count <= opcodeLimit;
        }

        public void Cleanup(DateTime now)
        {
            LastCleanup = now;
            foreach (KeyValuePair<ushort, PacketCounter> counter in _opcodeCounters)
            {
                if (now - counter.Value.WindowStart > _window)
                {
                    _opcodeCounters.TryRemove(counter.Key, out _);
                }
            }
        }

        private int Increment(ref PacketCounter counter, DateTime now)
        {
            PacketCounter current = counter;
            if (now - current.WindowStart > _window)
            {
                current = new PacketCounter(now);
            }

            current = current.Increment(now, _window);
            counter = current;
            return current.Count;
        }
    }

    private readonly record struct PacketCounter(DateTime WindowStart, int Count = 0)
    {
        public PacketCounter Increment(DateTime now, TimeSpan window)
        {
            if (now - WindowStart > window)
            {
                return new PacketCounter(now, 1);
            }

            return this with
            {
                Count = Count + 1
            };
        }
    }
}
